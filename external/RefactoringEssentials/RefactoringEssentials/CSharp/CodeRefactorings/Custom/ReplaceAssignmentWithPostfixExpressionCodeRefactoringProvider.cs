using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

namespace RefactoringEssentials.CSharp.CodeRefactorings
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = "Replace assignment with postfix expression")]
    public class ReplaceAssignmentWithPostfixExpressionCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var document = context.Document;
            if (document.Project.Solution.Workspace.Kind == WorkspaceKind.MiscellaneousFiles)
                return;
            var span = context.Span;
            if (!span.IsEmpty)
                return;
            var cancellationToken = context.CancellationToken;
            if (cancellationToken.IsCancellationRequested)
                return;
            var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            if (model.IsFromGeneratedCode(cancellationToken))
                return;
            var root = await model.SyntaxTree.GetRootAsync(cancellationToken).ConfigureAwait(false);
            var token = root.FindToken(span.Start);

			if (!(token.Parent is AssignmentExpressionSyntax node) || !node.OperatorToken.Span.Contains(span))
				return;

			if (!TryCheckIncrementOrDecrement(node, out var updatedNode))
				return;

			bool isIncrement = updatedNode.IsKind(SyntaxKind.AddAssignmentExpression);
			if (!IsIntegralTypeOrHasOperatorOverloaded(model, node.Left, isIncrement) || !IsIntegralTypeOrHasOperatorOverloaded(model, node.Right, isIncrement))
				return;

			string description = isIncrement ? GettextCatalog.GetString("To '{0}++'") : GettextCatalog.GetString("To '{0}--'");

			context.RegisterRefactoring(
				CodeActionFactory.Create(
					token.Span,
					DiagnosticSeverity.Info,
					string.Format (description, node.Left),
                    t2 =>
                    {
                        var newNode = SyntaxFactory.PostfixUnaryExpression(isIncrement ? SyntaxKind.PostIncrementExpression : SyntaxKind.PostDecrementExpression, updatedNode.Left);
                        var newRoot = root.ReplaceNode(node, newNode.WithAdditionalAnnotations(Formatter.Annotation).WithLeadingTrivia(node.GetLeadingTrivia()));
                        return Task.FromResult(document.WithSyntaxRoot(newRoot));
                    }
                )
            );
        }

		static bool TryCheckIncrementOrDecrement (AssignmentExpressionSyntax node, out AssignmentExpressionSyntax updatedNode)
		{
			updatedNode = ReplaceWithOperatorAssignmentCodeRefactoringProvider.CreateAssignment(node) ?? node;

			if ((!updatedNode.IsKind(SyntaxKind.AddAssignmentExpression) && !updatedNode.IsKind(SyntaxKind.SubtractAssignmentExpression)))
				return false;

			if (!(updatedNode.Right is LiteralExpressionSyntax rightLiteral))
				return false;

			return rightLiteral.Token.Value is 1;
		}

		static bool IsIntegralTypeOrHasOperatorOverloaded(SemanticModel model, SyntaxNode node, bool isIncrement)
		{
			var type = model.GetTypeInfo(node).Type;
			if (type == null)
				return false;

			switch (type.SpecialType)
			{
				case SpecialType.System_Byte:
				case SpecialType.System_Char:
				case SpecialType.System_Decimal:
				case SpecialType.System_Double:
				case SpecialType.System_Int16:
				case SpecialType.System_Int32:
				case SpecialType.System_Int64:
				case SpecialType.System_IntPtr:
				case SpecialType.System_SByte:
				case SpecialType.System_Single:
				case SpecialType.System_UInt16:
				case SpecialType.System_UInt32:
				case SpecialType.System_UInt64:
				case SpecialType.System_UIntPtr:
					return true;
			}

			var memberToLookup = isIncrement ? WellKnownMemberNames.IncrementOperatorName : WellKnownMemberNames.DecrementOperatorName;
			var members = type.GetMembers(memberToLookup);
			return !members.IsDefaultOrEmpty;
		}
	}
}

