using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace RefactoringEssentials.CSharp.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AccessToStaticMemberViaDerivedTypeAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor descriptor = new DiagnosticDescriptor(
            CSharpDiagnosticIDs.AccessToStaticMemberViaDerivedTypeAnalyzerID,
            GettextCatalog.GetString("Suggests using the class declaring a static function when calling it"),
            GettextCatalog.GetString("Call to static member via a derived class"),
            DiagnosticAnalyzerCategories.PracticesAndImprovements,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.CreateFor(CSharpDiagnosticIDs.AccessToStaticMemberViaDerivedTypeAnalyzerID)
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.RegisterOperationAction(ctx =>
			{
				var invocation = (IInvocationOperation)ctx.Operation;
				var method = invocation.TargetMethod;
				if (!method.IsStatic ||
					!(invocation.Syntax is InvocationExpressionSyntax invocationSyntax) ||
					!(invocationSyntax.Expression is MemberAccessExpressionSyntax memberAccessSyntax))
					return;

				SyntaxNode extensionParameter = null;
				if (method.IsExtensionMethod)
				{
					var argument = invocation.Arguments[0].Value;
					if (argument is IConversionOperation conversion)
						argument = conversion.Operand;

					if (argument.Syntax is ExpressionSyntax syntax)
						extensionParameter = syntax.GetRightmostName ();
				}

				if (TryGetDiagnostic (memberAccessSyntax, method.ContainingType, extensionParameter, out var diagnostic))
					ctx.ReportDiagnostic(Diagnostic.Create(descriptor, memberAccessSyntax.Expression.GetLocation()));
			}, OperationKind.Invocation);

			context.RegisterOperationAction(ctx =>
			{
				var memberReference = (IMemberReferenceOperation)ctx.Operation;
				var member = memberReference.Member;
				if (!member.IsStatic || !(memberReference.Syntax is MemberAccessExpressionSyntax memberAccessSyntax))
					return;

				if (TryGetDiagnostic(memberAccessSyntax, member.ContainingType, null, out var diagnostic))
					ctx.ReportDiagnostic(Diagnostic.Create(descriptor, memberAccessSyntax.Expression.GetLocation()));
			}, OperationKind.PropertyReference, OperationKind.FieldReference, OperationKind.MethodReference, OperationKind.EventReference);
		}

		static bool TryGetDiagnostic(MemberAccessExpressionSyntax memberAccessSyntax, ITypeSymbol containingType, SyntaxNode extensionParameter, out Diagnostic diagnostic)
		{
			diagnostic = default(Diagnostic);

			if (memberAccessSyntax.IsKind(SyntaxKind.ThisKeyword) || memberAccessSyntax.IsKind(SyntaxKind.BaseExpression))
				return false;

			var rightMostName = memberAccessSyntax.Expression.GetRightmostName();
			if (rightMostName == null || rightMostName == extensionParameter)
				return false;

			var rightMostType = rightMostName.Identifier.Text;
			if (rightMostType == containingType.Name)
				return false;

			if (CheckCuriouslyRecurringTemplatePattern(containingType, rightMostType))
				return false;

			diagnostic = Diagnostic.Create(
				descriptor,
				memberAccessSyntax.GetLocation()
			);
			return true;
		}

		static bool CheckCuriouslyRecurringTemplatePattern(ITypeSymbol containingType, string type)
        {
            if (containingType.Name == type)
                return true;
			if (!(containingType is INamedTypeSymbol nt))
				return false;
			foreach (var typeArg in nt.TypeArguments)
            {
                if (CheckCuriouslyRecurringTemplatePattern(typeArg, type))
                    return true;
            }
            return false;
        }
    }
}