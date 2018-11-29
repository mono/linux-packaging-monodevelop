using System;
using System.Linq;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using Microsoft.CodeAnalysis.Operations;

namespace RefactoringEssentials.CSharp.Diagnostics
{
	using UnusedParameterDictionary = IDictionary<IMethodSymbol, ISet<IParameterSymbol>>;

	[DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UnusedParameterAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor descriptor = new DiagnosticDescriptor(
            CSharpDiagnosticIDs.UnusedParameterAnalyzerID,
            GettextCatalog.GetString("Parameter is never used"),
            GettextCatalog.GetString("Parameter '{0}' is never used"),
            DiagnosticAnalyzerCategories.RedundanciesInDeclarations,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.CreateFor(CSharpDiagnosticIDs.UnusedParameterAnalyzerID)
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor);

        public override void Initialize(AnalysisContext context)
        {
			// TODO: Consider making this analyzer thread-safe.
			//context.EnableConcurrentExecution();

			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

			context.RegisterCompilationStartAction(compilationStartContext =>
			{
				var compilation = compilationStartContext.Compilation;
				INamedTypeSymbol eventsArgSymbol = compilation.GetTypeByMetadataName("System.EventArgs");

				INamedTypeSymbol streamingContext = compilation.GetTypeByMetadataName("System.Runtime.Serialization.StreamingContext");
				INamedTypeSymbol serializationInfo = compilation.GetTypeByMetadataName("System.Runtime.Serialization.SerializationInfo");

				// Ignore conditional methods (FxCop compat - One conditional will often call another conditional method as its only use of a parameter)
				INamedTypeSymbol conditionalAttributeSymbol = compilation.GetTypeByMetadataName("System.Diagnostics.ConditionalAttribute");

				// Ignore methods with special serialization attributes (FxCop compat - All serialization methods need to take 'StreamingContext')
				INamedTypeSymbol onDeserializingAttribute = compilation.GetTypeByMetadataName("System.Runtime.Serialization.OnDeserializingAttribute");
				INamedTypeSymbol onDeserializedAttribute = compilation.GetTypeByMetadataName("System.Runtime.Serialization.OnDeserializedAttribute");
				INamedTypeSymbol onSerializingAttribute = compilation.GetTypeByMetadataName("System.Runtime.Serialization.OnSerializingAttribute");
				INamedTypeSymbol onSerializedAttribute = compilation.GetTypeByMetadataName("System.Runtime.Serialization.OnSerializedAttribute");
				INamedTypeSymbol obsoleteAttribute = compilation.GetTypeByMetadataName("System.ObsoleteAttribute");
				INamedTypeSymbol exportXamMacIosAttribute = compilation.GetTypeByMetadataName("Foundation.ExportAttribute");
				INamedTypeSymbol exportXamAndroidAttribute = compilation.GetTypeByMetadataName("Java.Interop.ExportAttribute");

				ImmutableHashSet<INamedTypeSymbol> attributeSetForMethodsToIgnore = ImmutableHashSet.Create(
					conditionalAttributeSymbol,
					onDeserializedAttribute,
					onDeserializingAttribute,
					onSerializedAttribute,
					onSerializingAttribute,
					obsoleteAttribute,
					exportXamMacIosAttribute,
					exportXamAndroidAttribute);

				UnusedParameterDictionary unusedMethodParameters = new ConcurrentDictionary<IMethodSymbol, ISet<IParameterSymbol>>();
				ISet<IMethodSymbol> methodsUsedAsDelegates = new HashSet<IMethodSymbol>();

				// Create a list of functions to exclude from analysis. We assume that any function that is used in an IMethodBindingExpression
				// cannot have its signature changed, and add it to the list of methods to be excluded from analysis.
				compilationStartContext.RegisterOperationAction(operationContext =>
				{
					var methodBinding = (IMethodReferenceOperation)operationContext.Operation;
					methodsUsedAsDelegates.Add(methodBinding.Method.OriginalDefinition);
				}, OperationKind.MethodReference);

				compilationStartContext.RegisterOperationBlockStartAction(startOperationBlockContext =>
				{
					// We only care about methods.
					if (startOperationBlockContext.OwningSymbol.Kind != SymbolKind.Method)
					{
						return;
					}

					// We only care about methods with parameters.
					var method = (IMethodSymbol)startOperationBlockContext.OwningSymbol;
					if (method.Parameters.IsEmpty)
					{
						return;
					}

					// Ignore implicitly declared methods, extern methods, abstract methods, virtual methods, interface implementations and finalizers (FxCop compat).
					if (method.IsImplicitlyDeclared ||
						method.IsExtern ||
						method.IsAbstract ||
						method.IsVirtual ||
						method.IsOverride ||
						method.IsImplementationOfAnyInterfaceMember() ||
						method.IsFinalizer())
					{
						return;
					}

					// Ignore property accessors.
					if (method.IsPropertyAccessor())
					{
						return;
					}

					// RE fixup: Added ISerializable support
					if (serializationInfo != null && streamingContext != null &&
						method.Parameters.Length == 2 &&
						method.Parameters[0].Type == serializationInfo &&
						method.Parameters[1].Type == streamingContext)
					{
						return;
					}

					// Ignore methods with any attributes in 'attributeSetForMethodsToIgnore'.
					if (method.GetAttributes().Any(a => a.AttributeClass != null && attributeSetForMethodsToIgnore.Contains(a.AttributeClass)))
					{
						return;
					}

					// Ignore methods that were used as delegates
					if (methodsUsedAsDelegates.Contains(method))
					{
						return;
					}

					// RE fixup: Don't do this, we already look at methods used as delegates.
					//// Ignore event handler methods "Handler(object, MyEventArgs)"
					//if (eventsArgSymbol != null &&
					//	method.Parameters.Length == 2 &&
					//	method.Parameters[0].Type.SpecialType == SpecialType.System_Object &&
					//	method.Parameters[1].Type.Inherits(eventsArgSymbol))
					//{
					//	return;
					//}

					// Initialize local mutable state in the start action.
					var analyzer = new UnusedParametersAnalyzer(method, unusedMethodParameters);

					// Register an intermediate non-end action that accesses and modifies the state.
					startOperationBlockContext.RegisterOperationAction(analyzer.AnalyzeOperation, OperationKind.ParameterReference);

					// Register an end action to add unused parameters to the unusedMethodParameters dictionary
					startOperationBlockContext.RegisterOperationBlockEndAction(analyzer.OperationBlockEndAction);
				});

				// Register a compilation end action to filter all methods used as delegates and report any diagnostics
				compilationStartContext.RegisterCompilationEndAction(compilationAnalysisContext =>
				{
					// Report diagnostics for unused parameters.
					var unusedParameters = unusedMethodParameters.Where(kvp => !methodsUsedAsDelegates.Contains(kvp.Key)).SelectMany(kvp => kvp.Value);
					foreach (var parameter in unusedParameters)
					{
						var diagnostic = Diagnostic.Create(descriptor, parameter.Locations[0], parameter.Name);
						compilationAnalysisContext.ReportDiagnostic(diagnostic);
					}
				});
			});
        }

		private class UnusedParametersAnalyzer
		{
			#region Per-CodeBlock mutable state

			private readonly HashSet<IParameterSymbol> _unusedParameters;
			private readonly UnusedParameterDictionary _finalUnusedParameters;
			private readonly IMethodSymbol _method;

			#endregion

			#region State intialization

			public UnusedParametersAnalyzer(IMethodSymbol method, UnusedParameterDictionary finalUnusedParameters)
			{
				// Initialization: Assume all parameters are unused.
				_unusedParameters = new HashSet<IParameterSymbol>(method.Parameters);
				_finalUnusedParameters = finalUnusedParameters;
				_method = method;
			}

			#endregion

			#region Intermediate actions

			public void AnalyzeOperation(OperationAnalysisContext context)
			{
				// Check if we have any pending unreferenced parameters.
				if (_unusedParameters.Count == 0)
				{
					return;
				}

				// Mark this parameter as used.
				IParameterSymbol parameter = ((IParameterReferenceOperation)context.Operation).Parameter;
				_unusedParameters.Remove(parameter);
			}

			#endregion

			#region End action

			public void OperationBlockEndAction(OperationBlockAnalysisContext context)
			{
				// Check to see if the method just throws a NotImplementedException/NotSupportedException
				// We shouldn't warn about parameters in that case
				if (context.IsMethodNotImplementedOrSupported())
				{
					return;
				}

				// Do not raise warning for unused 'this' parameter of an extension method.
				if (_method.IsExtensionMethod)
				{
					var thisParamter = _unusedParameters.Where(p => p.Ordinal == 0).FirstOrDefault();
					_unusedParameters.Remove(thisParamter);
				}

				_finalUnusedParameters.Add(_method, _unusedParameters);
			}

			#endregion
		}
	}

	static class Extensions
	{
		public static bool IsFinalizer(this IMethodSymbol method)
		{
			if (method.MethodKind == MethodKind.Destructor)
			{
				return true; // for C#
			}

			if (method.Name != WellKnownMemberNames.DestructorName || method.Parameters.Length != 0 || !method.ReturnsVoid)
			{
				return false;
			}

			IMethodSymbol overridden = method.OverriddenMethod;

			if (method.ContainingType.SpecialType == SpecialType.System_Object)
			{
				// This is object.Finalize
				return true;
			}

			if (overridden == null)
			{
				return false;
			}

			for (IMethodSymbol o = overridden.OverriddenMethod; o != null; o = o.OverriddenMethod)
			{
				overridden = o;
			}

			return overridden.ContainingType.SpecialType == SpecialType.System_Object; // it is object.Finalize
		}

		public static bool IsImplementationOfAnyInterfaceMember(this ISymbol symbol)
		{
			return symbol.IsImplementationOfAnyExplicitInterfaceMember() || symbol.IsImplementationOfAnyImplicitInterfaceMember();
		}

		public static bool IsImplementationOfAnyExplicitInterfaceMember(this ISymbol symbol)
		{
			if (symbol is IMethodSymbol methodSymbol && methodSymbol.ExplicitInterfaceImplementations.Any())
			{
				return true;
			}

			if (symbol is IPropertySymbol propertySymbol && propertySymbol.ExplicitInterfaceImplementations.Any())
			{
				return true;
			}

			if (symbol is IEventSymbol eventSymbol && eventSymbol.ExplicitInterfaceImplementations.Any())
			{
				return true;
			}

			return false;
		}

		public static bool IsImplementationOfAnyImplicitInterfaceMember(this ISymbol symbol)
		{
			return IsImplementationOfAnyImplicitInterfaceMember<ISymbol>(symbol);
		}

		public static bool IsImplementationOfAnyImplicitInterfaceMember<TSymbol>(this ISymbol symbol)
		where TSymbol : ISymbol
		{
			if (symbol.ContainingType != null)
			{
				foreach (INamedTypeSymbol interfaceSymbol in symbol.ContainingType.AllInterfaces)
				{
					foreach (var interfaceMember in interfaceSymbol.GetMembers().OfType<TSymbol>())
					{
						if (IsImplementationOfInterfaceMember(symbol, interfaceMember))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		public static bool IsImplementationOfInterfaceMember(this ISymbol symbol, ISymbol interfaceMember)
		{
			return interfaceMember != null &&
				   symbol.Equals(symbol.ContainingType.FindImplementationForInterfaceMember(interfaceMember));
		}

		public static bool IsPropertyAccessor(this IMethodSymbol method)
		{
			// RE fixup.
			if (method.Parameters.Length > 0)
				return false;

			return method.MethodKind == MethodKind.PropertyGet ||
				   method.MethodKind == MethodKind.PropertySet;
		}

		public static bool Inherits(this ITypeSymbol type, ITypeSymbol possibleBase)
		{
			if (type == null || possibleBase == null)
			{
				return false;
			}

			switch (possibleBase.TypeKind)
			{
				case TypeKind.Class:
					if (type.TypeKind == TypeKind.Interface)
					{
						return false;
					}

					return DerivesFrom(type, possibleBase, baseTypesOnly: true);

				case TypeKind.Interface:
					return DerivesFrom(type, possibleBase);

				default:
					return false;
			}
		}

		public static bool DerivesFrom(this ITypeSymbol symbol, ITypeSymbol candidateBaseType, bool baseTypesOnly = false, bool checkTypeParameterConstraints = true)
		{
			if (candidateBaseType == null || symbol == null)
			{
				return false;
			}

			if (!baseTypesOnly && symbol.AllInterfaces.OfType<ITypeSymbol>().Contains(candidateBaseType))
			{
				return true;
			}

			if (checkTypeParameterConstraints && symbol.TypeKind == TypeKind.TypeParameter)
			{
				var typeParameterSymbol = (ITypeParameterSymbol)symbol;
				foreach (var constraintType in typeParameterSymbol.ConstraintTypes)
				{
					if (constraintType.DerivesFrom(candidateBaseType, baseTypesOnly, checkTypeParameterConstraints))
					{
						return true;
					}
				}
			}

			while (symbol != null)
			{
				if (symbol.Equals(candidateBaseType))
				{
					return true;
				}

				symbol = symbol.BaseType;
			}

			return false;
		}

		public static bool IsMethodNotImplementedOrSupported(this OperationBlockAnalysisContext context)
		{
			// Note that VB method bodies with 1 action have 3 operations.
			// The first is the actual operation, the second is a label statement, and the third is a return
			// statement. The last two are implicit in these scenarios.

			var operationBlocks = context.OperationBlocks.WhereAsArray(operation => !operation.IsOperationNoneRoot());

			IBlockOperation methodBlock = null;
			if (operationBlocks.Length == 1 && operationBlocks[0].Kind == OperationKind.Block)
			{
				methodBlock = (IBlockOperation)operationBlocks[0];
			}
			else if (operationBlocks.Length > 1)
			{
				foreach (var block in operationBlocks)
				{
					if (block.Kind == OperationKind.Block)
					{
						methodBlock = (IBlockOperation)block;
						break;
					}
				}
			}

			if (methodBlock != null)
			{
				bool IsSingleStatementBody(IBlockOperation body)
				{
					return body.Operations.Length == 1 ||
						(body.Operations.Length == 3 && body.Syntax.Language == LanguageNames.VisualBasic &&
						 body.Operations[1] is ILabeledOperation labeledOp && labeledOp.IsImplicit &&
						 body.Operations[2] is IReturnOperation returnOp && returnOp.IsImplicit);
				}

				if (IsSingleStatementBody(methodBlock))
				{
					var innerOperation = methodBlock.Operations.First();

					// Because of https://github.com/dotnet/roslyn/issues/23152, there can be an expression-statement
					// wrapping expression-bodied throw operations. Compensate by unwrapping if necessary.
					if (innerOperation.Kind == OperationKind.ExpressionStatement &&
						innerOperation is IExpressionStatementOperation exprStatement)
					{
						innerOperation = exprStatement.Operation;
					}

					if (innerOperation.Kind == OperationKind.Throw &&
						innerOperation is IThrowOperation throwOperation &&
						throwOperation.Exception.Kind == OperationKind.ObjectCreation &&
						throwOperation.Exception is IObjectCreationOperation createdException)
					{
						if (context.Compilation.GetTypeByMetadataName("System.NotImplementedException") == createdException.Type.OriginalDefinition
							|| context.Compilation.GetTypeByMetadataName("System.NotSupportedException") == createdException.Type.OriginalDefinition)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		public static bool IsOperationNoneRoot(this IOperation operation)
		{
			return operation.Kind == OperationKind.None && operation.Parent == null;
		}

		public static ImmutableArray<TSource> WhereAsArray<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> selector)
		{
			var builder = ImmutableArray.CreateBuilder<TSource>();
			bool any = false;
			foreach (var element in source)
			{
				if (selector(element))
				{
					any = true;
					builder.Add(element);
				}
			}

			if (any)
			{
				return builder.ToImmutable();
			}
			else
			{
				return ImmutableArray<TSource>.Empty;
			}
		}
	}
}
