using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RefactoringEssentials.CSharp.Diagnostics
{
    [ExportCodeFixProvider(LanguageNames.CSharp), System.Composition.Shared]
    public class SealedMemberInSealedClassCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(CSharpDiagnosticIDs.SealedMemberInSealedClassAnalyzerID);
            }
        }

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public async override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            var cancellationToken = context.CancellationToken;
            var span = context.Span;
            var diagnostics = context.Diagnostics;
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var diagnostic = diagnostics.First();
            var node = root.FindNode(context.Span);
            var newRoot = root.ReplaceNode((SyntaxNode)node, RemoveModifierFromNode(node, SyntaxKind.SealedKeyword));
            context.RegisterCodeFix(CodeActionFactory.Create(node.Span, diagnostic.Severity, "Remove redundant 'sealed' modifier", document.WithSyntaxRoot(newRoot)), diagnostic);
        }

		public static SyntaxNode RemoveModifierFromNode(SyntaxNode node, SyntaxKind modifier)
		{
			//there seem to be no base classes to support WithModifiers.
			//dynamic modifiersNode = node;
			//return modifiersNode.WithModifiers(SyntaxFactory.TokenList(modifiersNode.Modifiers.Where(m => !m.IsKind(SyntaxKind.PrivateKeyword))));

			MethodDeclarationSyntax methodNode = node as MethodDeclarationSyntax;
			if (methodNode != null)
				return methodNode.WithModifiers(SyntaxFactory.TokenList(methodNode.Modifiers.Where(m => !m.IsKind(modifier))))
								 .WithLeadingTrivia(methodNode.GetLeadingTrivia());

			FieldDeclarationSyntax fieldNode = node as FieldDeclarationSyntax;
			if (fieldNode != null)
				return fieldNode.WithModifiers(SyntaxFactory.TokenList(fieldNode.Modifiers.Where(m => !m.IsKind(modifier))))
								.WithLeadingTrivia(fieldNode.GetLeadingTrivia());

			PropertyDeclarationSyntax propertyNode = node as PropertyDeclarationSyntax;
			if (propertyNode != null)
				return propertyNode.WithModifiers(SyntaxFactory.TokenList(propertyNode.Modifiers.Where(m => !m.IsKind(modifier))))
								   .WithLeadingTrivia(propertyNode.GetLeadingTrivia());

			IndexerDeclarationSyntax indexerNode = node as IndexerDeclarationSyntax;
			if (indexerNode != null)
				return indexerNode.WithModifiers(SyntaxFactory.TokenList(indexerNode.Modifiers.Where(m => !m.IsKind(modifier))))
								  .WithLeadingTrivia(indexerNode.GetLeadingTrivia());

			EventDeclarationSyntax eventNode = node as EventDeclarationSyntax;
			if (eventNode != null)
				return eventNode.WithModifiers(SyntaxFactory.TokenList(eventNode.Modifiers.Where(m => !m.IsKind(modifier))))
								.WithLeadingTrivia(eventNode.GetLeadingTrivia());

			ConstructorDeclarationSyntax ctrNode = node as ConstructorDeclarationSyntax;
			if (ctrNode != null)
				return ctrNode.WithModifiers(SyntaxFactory.TokenList(ctrNode.Modifiers.Where(m => !m.IsKind(modifier))))
							  .WithLeadingTrivia(ctrNode.GetLeadingTrivia());

			OperatorDeclarationSyntax opNode = node as OperatorDeclarationSyntax;
			if (opNode != null)
				return opNode.WithModifiers(SyntaxFactory.TokenList(opNode.Modifiers.Where(m => !m.IsKind(modifier))))
							 .WithLeadingTrivia(opNode.GetLeadingTrivia());

			ClassDeclarationSyntax classNode = node as ClassDeclarationSyntax;
			if (classNode != null)
				return classNode.WithModifiers(SyntaxFactory.TokenList(classNode.Modifiers.Where(m => !m.IsKind(modifier))))
								.WithLeadingTrivia(classNode.GetLeadingTrivia());

			InterfaceDeclarationSyntax interfaceNode = node as InterfaceDeclarationSyntax;
			if (interfaceNode != null)
				return interfaceNode.WithModifiers(SyntaxFactory.TokenList(interfaceNode.Modifiers.Where(m => !m.IsKind(modifier))))
									.WithLeadingTrivia(interfaceNode.GetLeadingTrivia());

			StructDeclarationSyntax structNode = node as StructDeclarationSyntax;
			if (structNode != null)
				return structNode.WithModifiers(SyntaxFactory.TokenList(structNode.Modifiers.Where(m => !m.IsKind(modifier))))
								 .WithLeadingTrivia(structNode.GetLeadingTrivia());

			var enumNode = node as EnumDeclarationSyntax;
			if (enumNode != null)
				return enumNode.WithModifiers(SyntaxFactory.TokenList(enumNode.Modifiers.Where(m => !m.IsKind(modifier))))
								 .WithLeadingTrivia(enumNode.GetLeadingTrivia());

			var delegateNode = node as DelegateDeclarationSyntax;
			if (delegateNode != null)
				return delegateNode.WithModifiers(SyntaxFactory.TokenList(delegateNode.Modifiers.Where(m => !m.IsKind(modifier))))
								 .WithLeadingTrivia(delegateNode.GetLeadingTrivia());
			return node;
		}
	}
}
