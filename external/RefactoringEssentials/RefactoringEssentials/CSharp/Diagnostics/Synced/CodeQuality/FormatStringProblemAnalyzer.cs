using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using RefactoringEssentials.Util.CompositieFormatStringParser;
using Microsoft.CodeAnalysis.Operations;

namespace RefactoringEssentials.CSharp.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FormatStringProblemAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor descriptor = new DiagnosticDescriptor(
            CSharpDiagnosticIDs.FormatStringProblemAnalyzerID,
            GettextCatalog.GetString("Finds issues with format strings"),
            "{0}",
            DiagnosticAnalyzerCategories.CodeQualityIssues,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.CreateFor(CSharpDiagnosticIDs.FormatStringProblemAnalyzerID)
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
			context.RegisterOperationAction(
				ctx =>
				{
					var invoke = (IInvocationOperation)ctx.Operation;
					bool hasAnyConstant = invoke.Arguments.Any(x => x.Value.ConstantValue.HasValue);
					if (!hasAnyConstant)
						return;

					if (!FormatStringHelper.TryGetFormattingParameters(invoke, out var formatArgument, out var formatArguments, null, ctx.CancellationToken))
						return;

					var format = (string)formatArgument.Value.ConstantValue.Value;
					var parsingResult = new CompositeFormatStringParser().Parse(format);
					CheckSegments(ctx, parsingResult.Segments, formatArgument.Syntax.SpanStart + 1, formatArguments);

					var argUsed = new HashSet<int>();

					foreach (var item in parsingResult.Segments)
					{
						if (!(item is FormatItem fi))
							continue;
						argUsed.Add(fi.Index);
					}

					var text = GettextCatalog.GetString("Argument is not used in format string");
					for (int i = 0; i < formatArguments.Count; i++)
					{
						if (!argUsed.Contains(i))
						{
							ctx.ReportDiagnostic(Diagnostic.Create(
								descriptor,
								formatArguments[i].Syntax.GetLocation(),
								text
							));
						}
					}

				},
				OperationKind.Invocation
			);
        }

        static void CheckSegments(OperationAnalysisContext ctx, IList<IFormatStringSegment> segments, int formatStart, IList<IArgumentOperation> formatArguments)
		{
			if (segments.Count == 0)
				return;

			SyntaxTree syntaxTree = ctx.Operation.Syntax.SyntaxTree;

			var outOfBoundsFormat = GettextCatalog.GetString("The index '{0}' is out of bounds of the passed arguments");
			var multipleFormat = GettextCatalog.GetString("Multiple:\n{0}");

			int argumentCount = formatArguments.Count;
            foreach (var segment in segments) {
                var errors = segment.Errors.ToList();
				if (segment is FormatItem formatItem)
				{
					var location = Location.Create(syntaxTree, new Microsoft.CodeAnalysis.Text.TextSpan(formatStart + segment.StartLocation, segment.EndLocation - segment.StartLocation));

					if (formatItem.Index >= argumentCount)
					{
						ctx.ReportDiagnostic(Diagnostic.Create(
							descriptor,
							location,
							string.Format(outOfBoundsFormat, formatItem.Index)
						));
					}

					if (formatItem.HasErrors)
					{
						var errorMessage = string.Join(Environment.NewLine, errors.Select(error => error.Message).ToArray());
						string messageFormat;
						if (errors.Count > 1)
						{
							messageFormat = multipleFormat;
						}
						else
						{
							messageFormat = "{0}";
						}
						ctx.ReportDiagnostic(Diagnostic.Create(
							descriptor,
							location,
							string.Format(messageFormat, errorMessage)
						));
					}
				}
				else if (segment.HasErrors)
				{
					foreach (var error in errors)
					{
						ctx.ReportDiagnostic(Diagnostic.Create(
							descriptor,
							Location.Create(syntaxTree, new Microsoft.CodeAnalysis.Text.TextSpan(formatStart + error.StartLocation, error.EndLocation - error.StartLocation)),
							error.Message
						));
					}
				}
			}
		}
    }
}