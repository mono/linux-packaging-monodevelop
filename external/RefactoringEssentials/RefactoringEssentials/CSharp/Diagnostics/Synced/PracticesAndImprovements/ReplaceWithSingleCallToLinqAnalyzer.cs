using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace RefactoringEssentials.CSharp.Diagnostics
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReplaceWithSingleCallToLinqAnalyzer : DiagnosticAnalyzer
    {
		static readonly string[] replaceableMethods =
		{
			"Any",
			"Count",
			"First",
			"FirstOrDefault",
			"Last",
			"LastOrDefault",
			"LongCount",
			"Single",
			"SingleOrDefault",
		};

		static readonly DiagnosticDescriptor descriptor = new DiagnosticDescriptor(
            CSharpDiagnosticIDs.ReplaceWithSingleCallToLinqAnalyzerID,
            GettextCatalog.GetString("Redundant Where() call with predicate followed by {0}()"),
            GettextCatalog.GetString("Replace with single call to '{0}()'"),
            DiagnosticAnalyzerCategories.PracticesAndImprovements,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.CreateFor(CSharpDiagnosticIDs.ReplaceWithSingleCallToLinqAnalyzerID)
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.RegisterCompilationStartAction(compilationContext =>
			{
				var compilation = compilationContext.Compilation;

				var enumerableType = compilation.GetTypeByMetadataName("System.Linq.Enumerable");
				var queryableType = compilation.GetTypeByMetadataName("System.Linq.Queryable");
				var parallelEnumerableType = compilation.GetTypeByMetadataName("System.Linq.ParallelEnumerable");
				if (enumerableType == null || queryableType == null || parallelEnumerableType == null)
					return;

				compilationContext.RegisterOperationAction((nodeContext) =>
					{
			            var anyInvoke = (IInvocationOperation)nodeContext.Operation;
						if (anyInvoke.Arguments.Length != 1)
							return;

						var method = anyInvoke.TargetMethod;
			            if (!IsLinqExtension (method) || !IsPredicateMethod(method))
			                return;

						if (!MatchWhere(anyInvoke, out var whereInvoke) || whereInvoke.Arguments.Length != 2)
							return;

						var predicate = whereInvoke.Arguments[1].Parameter.Type;
			            if (predicate.GetTypeParameters ().Length != 2)
			                return;
						nodeContext.ReportDiagnostic(
							Diagnostic.Create(
								descriptor,
								anyInvoke.Syntax.GetLocation(),
								method.Name
							)
						);
					},
					OperationKind.Invocation
				);

				bool IsLinqExtension (IMethodSymbol symbol)
				{
					var methodType = symbol.ContainingType;
					return methodType == enumerableType || methodType == queryableType || methodType == parallelEnumerableType;
				}

				bool IsPredicateMethod(IMethodSymbol member)
				{
					return Array.IndexOf(replaceableMethods, member.Name) >= 0;
				}
			});
        }

		static bool MatchWhere (IInvocationOperation fromCall, out IInvocationOperation whereCall)
		{
			whereCall = null;
			if (fromCall.Arguments.Length != 1)
				return false;

			if (!(fromCall.Arguments[0].Value is IInvocationOperation whereInvocation))
				return false;

			var whereMethod = whereInvocation.TargetMethod;
			if (whereMethod.Name != "Where")
				return false;

			whereCall = whereInvocation;
			return true;
		}
    }
}