using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeFixes;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace RefactoringEssentials.CSharp.Diagnostics
{

    [ExportCodeFixProvider(LanguageNames.CSharp), System.Composition.Shared]
    public class RedundantUnsafeContextCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(CSharpDiagnosticIDs.RedundantUnsafeContextAnalyzerID);
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
            var token = root.FindToken(context.Span.Start);
            var node = token.Parent;
            if (node is MemberDeclarationSyntax memberDecl) {
                var newRoot = root.ReplaceNode(memberDecl, memberDecl.WithModifiers(SyntaxFactory.TokenList(memberDecl.GetModifiers().Where(m => !m.IsKind(SyntaxKind.UnsafeKeyword)))));
                context.RegisterCodeFix(CodeActionFactory.Create(token.Span, diagnostic.Severity, "Remove redundant 'unsafe' modifier", document.WithSyntaxRoot(newRoot)), diagnostic);
            }
            if (node is TypeDeclarationSyntax typeDecl) {
                var newRoot = root.ReplaceNode(typeDecl, typeDecl.WithModifiers(SyntaxFactory.TokenList(typeDecl.Modifiers.Where(m => !m.IsKind(SyntaxKind.UnsafeKeyword)))));
                context.RegisterCodeFix(CodeActionFactory.Create(token.Span, diagnostic.Severity, "Remove redundant 'unsafe' modifier", document.WithSyntaxRoot(newRoot)), diagnostic);
            }
            if (node.IsKind(SyntaxKind.UnsafeStatement)) {
                var decl = node as UnsafeStatementSyntax;
                var newRoot = root.ReplaceNode(decl, decl.Block.Statements.Select(s => s.WithAdditionalAnnotations(Formatter.Annotation)));
                context.RegisterCodeFix(CodeActionFactory.Create(token.Span, diagnostic.Severity, "Replace 'unsafe' statement with its body", document.WithSyntaxRoot(newRoot)), diagnostic);
            }
        }
    }
}