using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace RefactoringEssentials.CSharp.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConditionIsAlwaysTrueOrFalseAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor descriptor = new DiagnosticDescriptor(
            CSharpDiagnosticIDs.ConditionIsAlwaysTrueOrFalseAnalyzerID,
            GettextCatalog.GetString("Expression is always 'true' or always 'false'"),
            GettextCatalog.GetString("Expression is always '{0}'"),
            DiagnosticAnalyzerCategories.RedundanciesInCode,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.CreateFor(CSharpDiagnosticIDs.ConditionIsAlwaysTrueOrFalseAnalyzerID),
            customTags: DiagnosticCustomTags.Unnecessary
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.RegisterOperationAction(
				nodeContext =>
				{
					var operation = (IBinaryOperation)nodeContext.Operation;
					if (operation.OperatorKind != BinaryOperatorKind.Equals &&
						operation.OperatorKind != BinaryOperatorKind.NotEquals &&
						operation.OperatorKind != BinaryOperatorKind.LessThan &&
						operation.OperatorKind != BinaryOperatorKind.LessThanOrEqual &&
						operation.OperatorKind != BinaryOperatorKind.GreaterThan &&
						operation.OperatorKind != BinaryOperatorKind.GreaterThanOrEqual)
					{
						return;
					}

					if (operation.ConstantValue.HasValue && operation.ConstantValue.Value is bool result)
					{
						nodeContext.ReportDiagnostic(Diagnostic.Create(
							descriptor,
							operation.Syntax.GetLocation(),
							result.ToString()
						));
						return;
					}

					if (operation.OperatorKind != BinaryOperatorKind.Equals && operation.OperatorKind != BinaryOperatorKind.NotEquals)
						return;

					IOperation compareToNullOperand;
					var left = operation.LeftOperand.ConstantValue;
					if (left.HasValue && left.Value == null)
					{
						compareToNullOperand = operation.RightOperand;
					}
					else
					{
						var right = operation.RightOperand.ConstantValue;
						if (right.HasValue && right.Value == null)
						{
							compareToNullOperand = operation.LeftOperand;
						}
						else
							return;
					}

					var type = compareToNullOperand.Type;
					if (!type.IsValueType || type.IsPointerType())
						return;

					var operatorMethod = operation.OperatorMethod;
					if (operatorMethod != null && operatorMethod.IsUserDefinedOperator())
					{
						if (operatorMethod.Parameters[0].Type.IsReferenceType || operatorMethod.Parameters[1].Type.IsReferenceType)
							return;
					}

					if (compareToNullOperand is IConversionOperation conversion)
					{
						if (conversion.OperatorMethod.IsUserDefinedOperator())
							return;

						if (!type.IsNullableType())
						{
							return;
						}
					}
					else if (type.IsNullableType())
					{
						return;
					}

					nodeContext.ReportDiagnostic (Diagnostic.Create(
                   		descriptor,
                    	operation.Syntax.GetLocation(),
						(operation.OperatorKind == BinaryOperatorKind.NotEquals).ToString ()
					));

				}, OperationKind.BinaryOperator
			);
			context.RegisterOperationAction(
				nodeContext =>
				{
					var operation = (IUnaryOperation)nodeContext.Operation;
					if (operation.OperatorKind != UnaryOperatorKind.Not)
					{
						return;
					}

					if (operation.ConstantValue.HasValue && operation.ConstantValue.Value is bool result)
					{
						nodeContext.ReportDiagnostic(Diagnostic.Create(
							descriptor,
							operation.Syntax.GetLocation(),
							result.ToString()
						));
						return;
					}

				}, OperationKind.UnaryOperator
			);
        }
    }
}