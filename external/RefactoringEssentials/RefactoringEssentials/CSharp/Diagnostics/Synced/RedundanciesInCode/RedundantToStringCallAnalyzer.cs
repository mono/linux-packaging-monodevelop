using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Operations;

namespace RefactoringEssentials.CSharp.Diagnostics
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RedundantToStringCallAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor descriptor1 = new DiagnosticDescriptor(
            CSharpDiagnosticIDs.RedundantToStringCallAnalyzerID,
            GettextCatalog.GetString("Finds calls to ToString() which would be generated automatically by the compiler"),
            GettextCatalog.GetString("Redundant 'ToString()' call"),
            DiagnosticAnalyzerCategories.RedundanciesInCode,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.CreateFor(CSharpDiagnosticIDs.RedundantToStringCallAnalyzerID),
            customTags: DiagnosticCustomTags.Unnecessary
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor1);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.RegisterOperationAction(
				ctx => AnalyzeBinaryExpression (ctx),
				OperationKind.BinaryOperator
			);

            context.RegisterOperationAction(
				ctx => AnalyzeInvocationExpression (ctx),
				OperationKind.Invocation
            );
        }

        static void AnalyzeBinaryExpression(OperationAnalysisContext nodeContext)
        {
            var node = (IBinaryOperation)nodeContext.Operation;
			if (node.OperatorKind == BinaryOperatorKind.Add)
			{
				var visitor = new BinaryExpressionVisitor(nodeContext);
				visitor.Visit(node);
			}
		}

        static void AnalyzeInvocationExpression(OperationAnalysisContext nodeContext)
        {
            var invocationExpression = (IInvocationOperation)nodeContext.Operation;
			if (invocationExpression.Parent is IBinaryOperation)
				return;

            // "".ToString()
            CheckTargetedObject(nodeContext, invocationExpression);

            // Check list of members that call ToString() automatically
            CheckAutomaticToStringCallers(nodeContext, invocationExpression);

            // Check formatting calls
            CheckFormattingCall(nodeContext, invocationExpression);
        }


        class BinaryExpressionVisitor : OperationWalker
        {
            readonly OperationAnalysisContext nodeContext;

            int stringExpressionCount;
            IOperation firstStringExpression;
            HashSet<IOperation> processedNodes = new HashSet<IOperation>();

            public BinaryExpressionVisitor(OperationAnalysisContext nodeContext)
            {
                this.nodeContext = nodeContext;
            }

            public void Reset()
            {
                stringExpressionCount = 0;
                firstStringExpression = null;
            }

            void Check(IOperation expression)
            {
                if (stringExpressionCount <= 1)
                {
					var resolvedType = expression.Type;
                    if (resolvedType != null && resolvedType.SpecialType == SpecialType.System_String)
                    {
                        stringExpressionCount++;
                        if (stringExpressionCount == 1)
                        {
                            firstStringExpression = expression;
                        }
                        else {
                            CheckExpressionInAutoCallContext(firstStringExpression);
                            CheckExpressionInAutoCallContext(expression);
                        }
                    }
                }
                else {
                    CheckExpressionInAutoCallContext(expression);
                }
            }

			public override void VisitBinaryOperator(IBinaryOperation operation)
			{
				Check(operation.LeftOperand);
				Check(operation.RightOperand);
			}

            void CheckExpressionInAutoCallContext(IOperation expression)
            {
                if (expression is IInvocationOperation invocationOperation && !processedNodes.Contains(expression))
                {
					CheckInvocationInAutoCallContext(invocationOperation);
                }
            }

            void CheckInvocationInAutoCallContext(IInvocationOperation invocationExpression)
            {
				if (invocationExpression.Arguments.Length != 0)
					return;

				var method = invocationExpression.TargetMethod;
				if (method.Name != "ToString")
				{
					return;
				}

                if (!OverridesObjectToStringMethod(method))
                    return;

				var type = invocationExpression.Instance?.Type;
                if (type != null && type.IsValueType)
                    return;
                
                AddRedundantToStringIssue(invocationExpression);
            }

            void AddRedundantToStringIssue(IInvocationOperation invocationExpression)
            {
                // Simon Lindgren 2012-09-14: Previously there was a check here to see if the node had already been processed
                // This has been moved out to the callers, to check it earlier for a 30-40% run time reduction
                processedNodes.Add(invocationExpression);

				if (TryGetLocation (invocationExpression, out var location))
		            nodeContext.ReportDiagnostic(Diagnostic.Create(descriptor1, location));
            }
        }

        #region Invocation expression

        static bool OverridesObjectToStringMethod(IMethodSymbol toStringSymbol)
        {
            IMethodSymbol currentMethodSymbol = toStringSymbol;
            while (currentMethodSymbol != null)
            {
                if (currentMethodSymbol.ContainingType != null && currentMethodSymbol.ContainingType.SpecialType == SpecialType.System_Object)
                {
                    // Found object.ToString()
                    return true;
                }
				currentMethodSymbol = currentMethodSymbol.OverriddenMethod;
            }

            return false;
        }

        static void CheckTargetedObject(OperationAnalysisContext nodeContext, IInvocationOperation invocationExpression)
        {
			var method = invocationExpression.TargetMethod;
			if (method != null)
			{
				if (method.ContainingType.SpecialType == SpecialType.System_String && method.Name == "ToString")
				{
					if (TryGetLocation (invocationExpression, out var location))
			            nodeContext.ReportDiagnostic(Diagnostic.Create(descriptor1, location));
				}
			}
        }
        
        static void CheckAutomaticToStringCallers(OperationAnalysisContext nodeContext, IInvocationOperation invocationExpression)
        {
			var method = invocationExpression.TargetMethod;
            if (method.IsOverride)
            {
                method = method.OverriddenMethod;
                if (method == null)
                {
                    return;
                }
            }

			bool methodDoesToString = false;
			if (method.Name.StartsWith ("Write", StringComparison.Ordinal))
			{
				var typeSymbol = method.ContainingType;
				if (typeSymbol.Name == "TextWriter")
				{
					var ioNs = typeSymbol.ContainingNamespace;
					if (ioNs.Name == "IO" && ioNs.ContainingNamespace?.Name == "System")
						methodDoesToString = true;
				}
				else if (typeSymbol.Name == "Console")
				{
					var systemNs = typeSymbol.ContainingNamespace;
					if (systemNs.Name == "System")
						methodDoesToString = true;
				}
			}
			if (methodDoesToString == false)
                return;

            var arguments = invocationExpression.Arguments;
            for (int i = 0; i < arguments.Length; ++i)
            {
                CheckExpressionInAutoCallContext(nodeContext, arguments[i]);
            }
        }

        static void CheckExpressionInAutoCallContext(OperationAnalysisContext nodeContext, IArgumentOperation expression)
        {
			var value = expression.Value;

			IInvocationOperation invocationOperation;
			if (value is IConversionOperation conversion)
			{
				invocationOperation = conversion.Operand as IInvocationOperation;
			}
			else
			{
				invocationOperation = value as IInvocationOperation;
			}

			if (invocationOperation != null)
			{
				CheckInvocationInAutoCallContext(nodeContext, invocationOperation);
			}
        }

        static void CheckInvocationInAutoCallContext(OperationAnalysisContext nodeContext, IInvocationOperation invocationExpression)
        {
			if (invocationExpression.Arguments.Length != 0)
				return;

			var method = invocationExpression.TargetMethod;
			if (method == null || method.Name != "ToString")
				return;

			var targetType = invocationExpression.Instance?.Type;
            if (targetType != null && targetType.IsValueType)
                return;

            if (!OverridesObjectToStringMethod(method))
                return;

			if (TryGetLocation (invocationExpression, out var location))
	            nodeContext.ReportDiagnostic(Diagnostic.Create(descriptor1, location));
        }

		static bool TryGetLocation (IInvocationOperation operation, out Location location)
		{
			location = null;
			if (!(operation.Syntax is InvocationExpressionSyntax syntax))
				return false;

			if (!(syntax.Expression is MemberAccessExpressionSyntax access))
				return false;

			location = Location.Create(syntax.SyntaxTree, TextSpan.FromBounds(access.Expression.Span.End, syntax.Span.End));
			return true;
		}

		static void CheckFormattingCall(OperationAnalysisContext nodeContext, IInvocationOperation invocationExpression)
        {
            IArgumentOperation formatArgument;
            IList<IArgumentOperation> formatArguments;
            // Only check parameters that are of type object: String means it is neccessary, others
            // means that there is another problem (ie no matching overload of the method).
            Func<IParameterSymbol, IOperation, bool> predicate = (parameter, argument) =>
            {
                var type = parameter.Type;
/*                if (type is TypeWithElementType && parameter.IsParams)
                {
                    type = ((TypeWithElementType)type).ElementType;
                }*/
                return type.SpecialType == SpecialType.System_Object;
            };

            if (FormatStringHelper.TryGetFormattingParameters(invocationExpression, out formatArgument, out formatArguments, predicate))
            {
                foreach (var argument in formatArguments)
                {
                    CheckExpressionInAutoCallContext(nodeContext, argument);
                }
            }
        }
        #endregion
    }
}