using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace RefactoringEssentials.CSharp.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PossibleMistakenCallToGetTypeAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor descriptor = new DiagnosticDescriptor(
            CSharpDiagnosticIDs.PossibleMistakenCallToGetTypeAnalyzerID,
            GettextCatalog.GetString("Possible mistaken call to 'object.GetType()'"),
            GettextCatalog.GetString("Possible mistaken call to 'object.GetType()'"),
            DiagnosticAnalyzerCategories.PracticesAndImprovements,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.CreateFor(CSharpDiagnosticIDs.PossibleMistakenCallToGetTypeAnalyzerID)
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.RegisterCompilationStartAction(compilationContext =>
			{
				var systemType = compilationContext.Compilation.GetTypeByMetadataName("System.Type");
				if (systemType == null)
					return;

				compilationContext.RegisterOperationAction(
					(nodeContext) =>
					{
						var invoke = (IInvocationOperation)nodeContext.Operation;
						var method = invoke.TargetMethod;

						if (method.IsStatic || method.Name != "GetType" || method.ContainingType != systemType)
							return;

						nodeContext.ReportDiagnostic(Diagnostic.Create(
							descriptor,
							invoke.Syntax.GetLocation ()
						));
					},
					OperationKind.Invocation
				);
			});
        }
    }
}