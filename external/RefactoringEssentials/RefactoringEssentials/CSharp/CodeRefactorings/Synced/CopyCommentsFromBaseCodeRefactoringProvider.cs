using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using System.Threading;
using System.Xml.Linq;

namespace RefactoringEssentials.CSharp.CodeRefactorings
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = "Copy comments from base")]
    public class CopyCommentsFromBaseCodeRefactoringProvider : CodeRefactoringProvider
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
            if (!token.IsKind(SyntaxKind.IdentifierToken))
                return;

            var node = token.Parent as MemberDeclarationSyntax;
            if (node == null) 
                return;
            if (node is BaseTypeDeclarationSyntax cdex && cdex.BaseList == null)
                return;

            var declaredSymbol = model.GetDeclaredSymbol(node, cancellationToken);
            if (declaredSymbol == null || !string.IsNullOrEmpty(declaredSymbol.GetDocumentationCommentXml(null, false, cancellationToken)))
                return;

            string documentation;
            var baseMember = GetBaseMember(declaredSymbol, out documentation, cancellationToken);
            if (baseMember == null || string.IsNullOrEmpty(documentation))
                return;
            if (baseMember is INamedTypeSymbol typeSymbol)
            {
                if (typeSymbol.SpecialType == SpecialType.System_Object ||
                    typeSymbol.SpecialType == SpecialType.System_ValueType ||
                    typeSymbol.SpecialType == SpecialType.System_Enum)
                    return;
            }
            // "Copy comments from interface"
            context.RegisterRefactoring(
                CodeActionFactory.Create(
                    span,
                    DiagnosticSeverity.Info,
                    baseMember.ContainingType != null && baseMember.ContainingType.TypeKind == TypeKind.Interface ? GettextCatalog.GetString("Copy comments from interface") : GettextCatalog.GetString("Copy comments from base"),
                    t2 =>
                    {
                        string inner = StripRootNode(documentation);
                        if (inner == null)
                            return Task.FromResult(document);
                        var triva = node.GetLeadingTrivia();

                        var indentTrivia = triva.FirstOrDefault(t => t.IsKind(SyntaxKind.WhitespaceTrivia));
                        var indent = indentTrivia.ToString();

                        string[] lines = NewLine.SplitLines(inner);
                        for (int i = 0; i < lines.Length; i++)
                        {
                            lines[i] = indent + "/// " + lines[i].Trim();
                        }


                        var eol = "\r\n";
                        int idx = 0;
                        while (idx < triva.Count && triva[idx].IsKind(SyntaxKind.EndOfLineTrivia))
                            idx++;
                        triva = triva.Insert(idx, SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, string.Join(eol, lines) + eol));
                        var newRoot = root.ReplaceNode(node, node.WithLeadingTrivia(triva));
                        return Task.FromResult(document.WithSyntaxRoot(newRoot));
                    }
                )
            );
        }

        static string StripRootNode(string outerXml)
        {
            var idx1 = outerXml.IndexOf(">", System.StringComparison.Ordinal);
            var idx2 = outerXml.LastIndexOf("<", System.StringComparison.Ordinal);
            if (idx1 < 0 || idx2 < 0)
                return null;
            idx1++;
            while (idx1 < outerXml.Length && char.IsWhiteSpace(outerXml[idx1]))
                idx1++;
            while (idx2 > 0 && char.IsWhiteSpace(outerXml[idx2 - 1]))
                idx2--;
            if (idx1 >= idx2)
                return null;
            return outerXml.Substring(idx1, idx2 - idx1);
        }

        static string RemoveLineBreaksFromXml(string innerXml)
        {
            return innerXml.TrimStart('\r').TrimStart('\n').TrimEnd('\n').TrimEnd('\r');
        }

        static ISymbol GetBaseMember(ISymbol declaredSymbol, out string documentation, CancellationToken cancellationToken)
        {
            var overriddenMember = declaredSymbol.OverriddenMember();
            documentation = overriddenMember != null ? overriddenMember.GetDocumentationCommentXml(null, false, cancellationToken) : "";

            if (!string.IsNullOrEmpty(documentation) || declaredSymbol.Kind == SymbolKind.NamedType)
                return overriddenMember;

            var containingType = declaredSymbol.ContainingType;
            if (containingType == null)
            {
                documentation = null;
                return null;
            }
            foreach (var iface in containingType.AllInterfaces)
            {
                foreach (var member in iface.GetMembers())
                {
                    var implementation = containingType.FindImplementationForInterfaceMember(member);
                    if (implementation == declaredSymbol)
                    {
                        documentation = member.GetDocumentationCommentXml(null, false, cancellationToken);
                        if (!string.IsNullOrEmpty(documentation))
                            return implementation;
                    }
                }
            }
            return null;
        }
    }
}