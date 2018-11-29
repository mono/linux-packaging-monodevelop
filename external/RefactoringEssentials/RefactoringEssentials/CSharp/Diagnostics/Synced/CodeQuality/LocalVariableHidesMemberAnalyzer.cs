using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Microsoft.CodeAnalysis.Operations;

namespace RefactoringEssentials.CSharp.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LocalVariableHidesMemberAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor descriptor = new DiagnosticDescriptor(
            CSharpDiagnosticIDs.LocalVariableHidesMemberAnalyzerID,
            GettextCatalog.GetString("Local variable has the same name as a member and hides it"),
            GettextCatalog.GetString("Local variable '{0}' hides {1} '{2}'"),
            DiagnosticAnalyzerCategories.CodeQualityIssues,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.CreateFor(CSharpDiagnosticIDs.LocalVariableHidesMemberAnalyzerID)
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

			context.RegisterOperationAction(operationContext =>
			{
				Diagnostic diagnostic;
				if (TryGetDiagnostic (operationContext, out diagnostic))
				{
					operationContext.ReportDiagnostic(diagnostic);
				}
			}, OperationKind.VariableDeclaration);

			context.RegisterOperationAction(operationContext =>
			{
				Diagnostic diagnostic;
				if (TryGetDiagnosticFromForeach(operationContext, out diagnostic))
				{
					operationContext.ReportDiagnostic(diagnostic);
				}
			}, OperationKind.Loop);
        }

        static string GetMemberType(SymbolKind symbolKind)
        {
            switch (symbolKind)
            {
                case SymbolKind.Field:
                    return GettextCatalog.GetString("field");
                case SymbolKind.Method:
                    return GettextCatalog.GetString("method");
                case SymbolKind.Property:
                    return GettextCatalog.GetString("property");
                case SymbolKind.Event:
                    return GettextCatalog.GetString("event");
            }

            return GettextCatalog.GetString("member");
        }

        static bool TryGetDiagnostic(OperationAnalysisContext nodeContext, out Diagnostic diagnostic)
        {
            diagnostic = default(Diagnostic);
            var node = (IVariableDeclarationOperation)nodeContext.Operation;
			var containingSymbol = nodeContext.ContainingSymbol;
			if (containingSymbol == null)
				return false;
			bool staticContext = containingSymbol.IsStatic;
			var containingType = containingSymbol.GetContainingTypeOrThis ();
			if (containingType == null)
				return false;

			foreach (var variable in node.Declarators)
			{
				var name = variable.Symbol.Name;

				var initializer = variable.Initializer;
				if (initializer?.Value is IFieldReferenceOperation fieldReferenceOperation)
				{
					if (fieldReferenceOperation.Field.Name == name && fieldReferenceOperation.Instance?.Syntax.IsKind(SyntaxKind.ThisExpression) == true)
					{
						continue;
					}
				}

				ISymbol hidingMember;
				INamedTypeSymbol currentSymbolToSearch = containingType;
				do
				{
					if (!TryFindMember(name, currentSymbolToSearch, containingType, staticContext, out hidingMember))
						currentSymbolToSearch = currentSymbolToSearch.BaseType;
				} while (hidingMember == null && currentSymbolToSearch != null);

				if (hidingMember == null)
					continue;

				string memberType = GetMemberType(hidingMember.Kind);
				if (variable.Syntax is VariableDeclaratorSyntax declaratorSyntax)
				{
					diagnostic = Diagnostic.Create(descriptor, declaratorSyntax.Identifier.GetLocation(), name, memberType, hidingMember.Name);
					return true;
				}
			}
			return false;
        }

		internal static bool TryFindMember (string name, INamedTypeSymbol inSymbol, INamedTypeSymbol accessibleWithin, bool staticContext, out ISymbol hidingMember)
		{
			hidingMember = null;

			var members = inSymbol.GetMembers();
			foreach (var memberMember in members)
			{
				if (memberMember.Name == name &&
					memberMember.IsAccessibleWithin(accessibleWithin) &&
					((staticContext && memberMember.IsStatic) || !staticContext))
				{
					hidingMember = memberMember;
					break;
				}
			}

			return hidingMember != null;
		}

		static bool TryGetDiagnosticFromForeach(OperationAnalysisContext nodeContext, out Diagnostic diagnostic)
        {
            diagnostic = default(Diagnostic);
			var node = (ILoopOperation)nodeContext.Operation;
			if (!(node is IForEachLoopOperation forEachLoop))
				return false;

			if (!(forEachLoop.Syntax is ForEachStatementSyntax syntax))
				return false;
			var location = syntax.Identifier.GetLocation();
			var containingSymbol = nodeContext.ContainingSymbol;
			if (containingSymbol == null)
				return false;
			bool staticContext = containingSymbol.IsStatic;
			var containingType = containingSymbol.GetContainingTypeOrThis();
			if (containingType == null)
				return false;

			if (node.Locals.Length != 1)
				return false;

			var name = node.Locals[0].Name;

			ISymbol hidingMember;
			INamedTypeSymbol currentSymbolToSearch = containingType;
			do
			{
				if (!TryFindMember(name, currentSymbolToSearch, containingType, staticContext, out hidingMember))
					currentSymbolToSearch = currentSymbolToSearch.BaseType;
			} while (hidingMember == null && currentSymbolToSearch != null);

			if (hidingMember == null)
				return false;
				
            string memberType = GetMemberType(hidingMember.Kind);
            diagnostic = Diagnostic.Create(descriptor, location, name, memberType, hidingMember.Name);
            return true;
        }
    }
}