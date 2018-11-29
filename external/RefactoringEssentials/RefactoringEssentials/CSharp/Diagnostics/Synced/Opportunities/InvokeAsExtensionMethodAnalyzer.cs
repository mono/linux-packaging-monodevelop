using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace RefactoringEssentials.CSharp.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InvokeAsExtensionMethodAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor descriptor = new DiagnosticDescriptor(
            CSharpDiagnosticIDs.InvokeAsExtensionMethodAnalyzerID,
            GettextCatalog.GetString("If an extension method is called as static method convert it to method syntax"),
            GettextCatalog.GetString("Convert static method call to extension method call"),
            DiagnosticAnalyzerCategories.Opportunities,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.CreateFor(CSharpDiagnosticIDs.InvokeAsExtensionMethodAnalyzerID)
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterOperationAction(
                (nodeContext) =>
                {
                    Diagnostic diagnostic;
                    if (TryGetDiagnostic(nodeContext, out diagnostic))
                    {
                        nodeContext.ReportDiagnostic(diagnostic);
                    }
                },
				OperationKind.Invocation
			);
        }

        static bool TryGetDiagnostic(OperationAnalysisContext nodeContext, out Diagnostic diagnostic)
        {
			diagnostic = default(Diagnostic);

			var node = (IInvocationOperation)nodeContext.Operation;
			var method = node.TargetMethod;
			if (!method.IsExtensionMethod || method.MethodKind == MethodKind.ReducedExtension)
				return false;

			if (!TryGetMemberAccess(node, out var access))
				return false;

			// Only report if it's not a qualified invocation
			if (!(access.Expression is SimpleNameSyntax name) || name.Identifier.Text != method.ContainingType.Name)
				return false;

			if (node.Arguments.Length < 1)
				return false;

			var firstArgument = node.Arguments[0].Value;
			if (firstArgument is IConversionOperation conversion && conversion.IsImplicit)
				firstArgument = conversion.Operand;

			// Ignore null literals
			if (firstArgument is ILiteralOperation literal && literal.ConstantValue.HasValue && literal.ConstantValue.Value == null)
				return false;

			// Ignore mismatched parameters
			if (!firstArgument.Type.Equals (method.Parameters[0].Type))
				return false;

			// Ignore delegates.
			if (node.Arguments[0].Value is IDelegateCreationOperation)
				return false;

            //// Don't allow conversion if first parameter is a method name instead of variable (extension method on delegate type)
            //if (firstArgument is IdentifierNameSyntax)
            //{
            //    var extensionMethodTargetExpression = semanticModel.GetSymbolInfo(firstArgument).Symbol as IMethodSymbol;
            //    if (extensionMethodTargetExpression != null)
            //        return false;
            //}

			if (TryGetMemberAccess (node, out var location))
			{
				diagnostic = Diagnostic.Create(
					descriptor,
					access.Name.GetLocation()
				);
				return true;
			}
			return false;
        }

		static bool TryGetMemberAccess(IInvocationOperation operation, out MemberAccessExpressionSyntax memberAccess)
		{
			memberAccess = default(MemberAccessExpressionSyntax);
			if (!(operation.Syntax is InvocationExpressionSyntax syntax))
				return false;

			if (!(syntax.Expression is MemberAccessExpressionSyntax access))
				return false;

			memberAccess = access;
			return true;
		}
	}
}