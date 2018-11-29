using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

namespace RefactoringEssentials.CSharp.Diagnostics
{
    [ExportCodeFixProvider(LanguageNames.CSharp), System.Composition.Shared]
    public class ReplaceWithOfTypeLinqCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(CSharpDiagnosticIDs.ReplaceWithOfTypeLinqAnalyzerID);
            }
        }

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }


        internal static bool MatchSelect(InvocationExpressionSyntax anyInvoke, out ExpressionSyntax target, out TypeSyntax type, out ParameterSyntax lambdaParam, out ExpressionSyntax followUpExpression, out string methodName)
        {
            target = null;
            type = null;
            lambdaParam = null;
            followUpExpression = null;
			methodName = null;

            if (anyInvoke.ArgumentList.Arguments.Count != 1)
                return false;
            var anyInvokeBase = anyInvoke.Expression as MemberAccessExpressionSyntax;
            if (anyInvokeBase == null)
                return false;
			methodName = anyInvokeBase.Name.Identifier.Text;
            var selectInvoc = anyInvokeBase.Expression as InvocationExpressionSyntax;
            if (selectInvoc == null || selectInvoc.ArgumentList.Arguments.Count != 1)
                return false;
            var baseMember = selectInvoc.Expression as MemberAccessExpressionSyntax;
            if (baseMember == null || baseMember.Name.Identifier.Text != "Select")
                return false;
            target = baseMember.Expression;

            ParameterSyntax param1, param2;
            BinaryExpressionSyntax expr1, expr2;
            if (!ExtractLambda(selectInvoc.ArgumentList.Arguments[0], out param1, out expr1))
                return false;
            if (!ExtractLambda(anyInvoke.ArgumentList.Arguments[0], out param2, out expr2))
                return false;
            lambdaParam = param2;
            if (!CompareNames(param1, expr1.Left as IdentifierNameSyntax))
                return false;
            if (expr2.IsKind(SyntaxKind.LogicalAndExpression))
            {
                if (CheckNotEqualsNullExpr(expr2.Left as BinaryExpressionSyntax, param2))
                    followUpExpression = expr2.Right;
                else if (CheckNotEqualsNullExpr(expr2.Right as BinaryExpressionSyntax, param2))
                    followUpExpression = expr2.Left;
                else
                    return false;
            }
            else if (!CheckNotEqualsNullExpr(expr2, param2))
                return false;

            if (expr1.IsKind(SyntaxKind.AsExpression))
                type = expr1.Right as TypeSyntax;

            return target != null && type != null;
        }

        static bool CheckNotEqualsNullExpr(BinaryExpressionSyntax expr, ParameterSyntax param)
        {
            if (expr == null)
                return false;
            if (!expr.IsKind(SyntaxKind.NotEqualsExpression))
                return false;
            if (expr.Right.IsKind(SyntaxKind.NullLiteralExpression) && CompareNames(param, expr.Left as IdentifierNameSyntax))
                return true;
            if (expr.Left.IsKind(SyntaxKind.NullLiteralExpression) && CompareNames(param, expr.Right as IdentifierNameSyntax))
                return true;
            return false;
        }

        static bool ExtractLambda(ArgumentSyntax argument, out ParameterSyntax parameter, out BinaryExpressionSyntax body)
        {
            if (argument.Expression is SimpleLambdaExpressionSyntax)
            {
                var simple = (SimpleLambdaExpressionSyntax)argument.Expression;
                parameter = simple.Parameter;
                body = simple.Body as BinaryExpressionSyntax;
                if (body == null)
                {
                    return false;
                }
                body = body.SkipParens() as BinaryExpressionSyntax;
                return true;
            }

            parameter = null;
            body = null;
            return false;
        }

        internal static InvocationExpressionSyntax MakeOfTypeCall(InvocationExpressionSyntax anyInvoke, out string methodName)
        {
            var member = ((MemberAccessExpressionSyntax)anyInvoke.Expression).Name;
            ExpressionSyntax target, followUp;
            TypeSyntax type;
            ParameterSyntax param;
            if (MatchSelect(anyInvoke, out target, out type, out param, out followUp, out methodName))
            {
                var ofTypeIdentifier = ((SimpleNameSyntax)SyntaxFactory.ParseName("OfType")).Identifier;
                var typeParams = SyntaxFactory.SeparatedList(new[] { type });
                var ofTypeName = SyntaxFactory.GenericName(ofTypeIdentifier, SyntaxFactory.TypeArgumentList(typeParams));
                InvocationExpressionSyntax ofTypeCall = SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, target, ofTypeName));
                var callerExpr = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ofTypeCall, member).WithAdditionalAnnotations(Formatter.Annotation);
                if (followUp == null)
                    return SyntaxFactory.InvocationExpression(callerExpr);
                var lambdaExpr = SyntaxFactory.SimpleLambdaExpression(param, followUp);
                var argument = SyntaxFactory.Argument(lambdaExpr).WithAdditionalAnnotations(Formatter.Annotation);
                return SyntaxFactory.InvocationExpression(callerExpr, SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { argument })));
            }

            return null;
        }

        public async override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            var cancellationToken = context.CancellationToken;
            var span = context.Span;
            var diagnostics = context.Diagnostics;
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var diagnostic = diagnostics.First();
            var node = root.FindNode(context.Span, getInnermostNodeForTie: true) as InvocationExpressionSyntax;
            var newRoot = root.ReplaceNode(node, MakeOfTypeCall(node, out string methodName));
            context.RegisterCodeFix(CodeActionFactory.Create(node.Span, diagnostic.Severity, string.Format ("Replace with call to OfType<T>().{0}()", methodName), document.WithSyntaxRoot(newRoot)), diagnostic);
		}

		static bool CompareNames(ParameterSyntax param, IdentifierNameSyntax expr)
		{
			if (param == null || expr == null)
				return false;
			return param.Identifier.ValueText == expr.Identifier.ValueText;
		}
	}
}