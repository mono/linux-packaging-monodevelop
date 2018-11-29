using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RefactoringEssentials.CSharp.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MemberHidesStaticFromOuterClassAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor descriptor = new DiagnosticDescriptor(
            CSharpDiagnosticIDs.MemberHidesStaticFromOuterClassAnalyzerID,
            GettextCatalog.GetString("Member hides static member from outer class"),
            GettextCatalog.GetString("{0} '{1}' hides {2} from outer class"),
            DiagnosticAnalyzerCategories.CodeQualityIssues,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.CreateFor(CSharpDiagnosticIDs.MemberHidesStaticFromOuterClassAnalyzerID)
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.RegisterSymbolAction(ctx =>
			{
				var symbol = ctx.Symbol;

				// Do not report properties multiple times.
				if (symbol.Kind == SymbolKind.Method && symbol.IsAccessorMethod())
					return;

				var symbolDeclaringType = symbol.ContainingType;
				var nestedSymbolContainingType = symbolDeclaringType.ContainingType;

				if (ReportHiddenMembers (symbol.Name, nestedSymbolContainingType, out string parentMemberKind))
				{
					var memberKind = GetMemberKind(symbol);
					foreach (var syntax in symbol.DeclaringSyntaxReferences)
					{
						var node = syntax.GetSyntax(ctx.CancellationToken);
						Location actualLocation;
						if (node is PropertyDeclarationSyntax property)
							actualLocation = property.Identifier.GetLocation();
						else if (node is MethodDeclarationSyntax method)
							actualLocation = method.Identifier.GetLocation();
						else if (node is EventDeclarationSyntax @event)
							actualLocation = @event.Identifier.GetLocation();
						else if (node is VariableDeclaratorSyntax field)
							actualLocation = field.Identifier.GetLocation();
						else
							continue;

						ctx.ReportDiagnostic(Diagnostic.Create(
	                        descriptor, 
							actualLocation,
	                        memberKind, symbol.Name, parentMemberKind
						));
					}
				}

			}, SymbolKind.Event, SymbolKind.Method, SymbolKind.Field, SymbolKind.Property);
        }


		bool ReportHiddenMembers (string name, INamedTypeSymbol parentSymbol, out string parentMemberKind)
		{
			parentMemberKind = null;
			if (parentSymbol == null)
				return false;

			var members = parentSymbol.GetMembers(name);
			if (!members.IsDefaultOrEmpty)
			{
				var hiddenMember = members.FirstOrDefault(x => x.IsStatic);
				if (hiddenMember != null)
				{
					parentMemberKind = GetMemberKind(hiddenMember);
					return true;
				}
			}

			return ReportHiddenMembers(name, parentSymbol.ContainingType, out parentMemberKind);
		}

		string GetMemberKind (ISymbol symbol)
		{
			switch (symbol.Kind)
			{
				case SymbolKind.Field:
					return GettextCatalog.GetString("field");
				case SymbolKind.Property:
					return GettextCatalog.GetString("property");
				case SymbolKind.Event:
					return GettextCatalog.GetString("event");
				case SymbolKind.Method:
					return GettextCatalog.GetString("method");
				default:
					return GettextCatalog.GetString("member");
			}
		}
    }
}