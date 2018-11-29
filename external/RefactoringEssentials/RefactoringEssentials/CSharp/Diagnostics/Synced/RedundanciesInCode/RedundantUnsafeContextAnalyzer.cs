using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace RefactoringEssentials.CSharp.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RedundantUnsafeContextAnalyzer : DiagnosticAnalyzer
    {
        static readonly DiagnosticDescriptor descriptor = new DiagnosticDescriptor(
            CSharpDiagnosticIDs.RedundantUnsafeContextAnalyzerID,
            GettextCatalog.GetString("Unsafe modifier in redundant in unsafe context or when no unsafe constructs are used"),
            GettextCatalog.GetString("'unsafe' modifier is redundant"),
            DiagnosticAnalyzerCategories.RedundanciesInCode,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.CreateFor(CSharpDiagnosticIDs.RedundantUnsafeContextAnalyzerID),
            customTags: DiagnosticCustomTags.Unnecessary
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(descriptor);


        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

			// Handle types
			context.RegisterSyntaxNodeAction(ctx =>
			{
				var node = ctx.Node;
				if (!(node is TypeDeclarationSyntax typeNode))
					return;

				// Skip non-unsafe.
				var unsafeModifier = typeNode.Modifiers.FirstOrNullable(x => x.Kind() == SyntaxKind.UnsafeKeyword);
				if (!unsafeModifier.HasValue)
					return;

				foreach (var member in typeNode.Members)
				{
					// If the member is marked as unsafe, the containing unsafe is not needed.
					bool isMemberUnsafeSelfContained = member.GetModifiers().Any(x => x.Kind() == SyntaxKind.UnsafeKeyword);
					if (isMemberUnsafeSelfContained)
						continue;

					// Look into unsafe constructs which spill
					if (IsMethodUnsafe (member) || IsFieldUnsafe(member) || IsDelegateUnsafe (member) || IsPropertyUnsafe (member))
						return;
				}

				ctx.ReportDiagnostic(Diagnostic.Create(
					descriptor,
					unsafeModifier.Value.GetLocation ()
				));
			}, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration);

			// Handle method bodies
			context.RegisterSyntaxNodeAction(ctx =>
			{
				var node = ctx.Node;
				if (!(node is MemberDeclarationSyntax memberNode))
					return;

				// Skip non-unsafe.
				var unsafeModifier = memberNode.GetModifiers().FirstOrNullable(x => x.Kind() == SyntaxKind.UnsafeKeyword);
				if (!unsafeModifier.HasValue)
					return;

				if (IsMethodUnsafe(memberNode) || IsPropertyUnsafe(memberNode))
					return;

				var visitor = new DeclarationGatherVisitor(ctx, ctx.SemanticModel);
				visitor.Visit(node);
			}, SyntaxKind.MethodDeclaration, SyntaxKind.PropertyDeclaration);

			context.RegisterSyntaxNodeAction(ctx =>
			{
				var visitor = new UnsafeStatementGatherVisitor(ctx, ctx.SemanticModel);
				visitor.Visit(ctx.Node);
			}, SyntaxKind.UnsafeStatement);

			context.RegisterSyntaxNodeAction(ctx =>
			{
				var node = ctx.Node;
				if (!(node is MemberDeclarationSyntax memberNode))
					return;

				// Skip non-unsafe.
				var unsafeModifier = memberNode.GetModifiers().FirstOrNullable(x => x.Kind() == SyntaxKind.UnsafeKeyword);
				if (!unsafeModifier.HasValue)
					return;

				if (IsFieldUnsafe(memberNode) || IsDelegateUnsafe(memberNode))
					return;

				ctx.ReportDiagnostic(Diagnostic.Create(
					descriptor,
					unsafeModifier.Value.GetLocation ()
				));
			}, SyntaxKind.FieldDeclaration, SyntaxKind.DelegateDeclaration);
        }

		static bool IsPropertyUnsafe (MemberDeclarationSyntax member)
		{
			if (!(member is PropertyDeclarationSyntax prop))
				return false;

			return TypeIsPointerType(prop.Type);
		}

		static bool IsDelegateUnsafe (MemberDeclarationSyntax member)
		{
			if (!(member is DelegateDeclarationSyntax method))
				return false;

			if (TypeIsPointerType(method.ReturnType))
				return true;

			foreach (var arg in method.ParameterList.Parameters)
			{
				if (TypeIsPointerType(arg?.Type))
					return true;
			}
			return false;
		}

		static bool IsMethodUnsafe (MemberDeclarationSyntax member)
		{
			if (!(member is MethodDeclarationSyntax method))
				return false;

			if (TypeIsPointerType(method.ReturnType))
				return true;

			foreach (var arg in method.ParameterList.Parameters)
			{
				if (TypeIsPointerType(arg?.Type))
					return true;
			}
			return false;
		}

		static bool IsFieldUnsafe (MemberDeclarationSyntax member)
		{
			if (!(member is FieldDeclarationSyntax field))
				return false;

			bool isFixed = field.Modifiers.Any(SyntaxKind.FixedKeyword);
			return isFixed || TypeIsPointerType(field?.Declaration?.Type);
		}

		static bool TypeIsPointerType (TypeSyntax type)
		{
			return type != null && type.Kind() == SyntaxKind.PointerType;
		}

		class DeclarationGatherVisitor : BaseGatherVisitor
		{
			public DeclarationGatherVisitor(SyntaxNodeAnalysisContext ctx, SemanticModel semanticModel) : base(ctx, semanticModel)
			{
			}

			public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
			{
				AddScope();
				base.VisitPropertyDeclaration(node);
				PopScope(node.Modifiers);
			}

			public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
			{
				AddScope();
				base.VisitMethodDeclaration(node);
				PopScope(node.Modifiers);
			}
		}

		class UnsafeStatementGatherVisitor : BaseGatherVisitor
		{
			public UnsafeStatementGatherVisitor(SyntaxNodeAnalysisContext ctx, SemanticModel semanticModel) : base(ctx, semanticModel)
			{
			}

			public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
            {
				// Prefer eliminating containing body's modifier rather than block.
				AddScope();
                base.VisitUnsafeStatement(node);
                bool isRedundant = !unsafeStateStack.Pop().UseUnsafeConstructs;

                if (isRedundant)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        descriptor,
                        node.UnsafeKeyword.GetLocation()
                    ));
                }
            }
		}

		class BaseGatherVisitor : CSharpSyntaxWalker
		{
			protected readonly SyntaxNodeAnalysisContext ctx;
            readonly SemanticModel semanticModel;

            public BaseGatherVisitor(SyntaxNodeAnalysisContext ctx, SemanticModel semanticModel)
            {
                this.ctx = ctx;
                this.semanticModel = semanticModel;
            }

            public class UnsafeState
            {
				public UnsafeState Next;
                public bool UseUnsafeConstructs;

                public UnsafeState()
                {
					UseUnsafeConstructs = false;
                }

                public override string ToString()
                {
                    return string.Format("[UnsafeState: UseUnsafeConstructs={0}]", UseUnsafeConstructs);
                }
            }

			public class UnsafeStateStack
			{
				UnsafeState currentState;

				public bool IsEmpty => currentState == null;

				public UnsafeState Pop ()
				{
					var current = currentState;
					if (current != null)
						currentState = current.Next;
					return current;
				}

				public void Push (UnsafeState item)
				{
					item.Next = currentState;
					currentState = item;
				}

				public UnsafeState Peek() => currentState;
			}

			protected readonly UnsafeStateStack unsafeStateStack = new UnsafeStateStack ();

			protected void AddScope() => unsafeStateStack.Push(new UnsafeState());
            protected void PopScope(SyntaxTokenList modifiers)
            {
                var state = unsafeStateStack.Pop();
                if (!state.UseUnsafeConstructs)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        descriptor,
                        modifiers.First(m => m.IsKind(SyntaxKind.UnsafeKeyword)).GetLocation()
                    ));
                }
            }

			bool IsUnsafeContext() => !unsafeStateStack.IsEmpty;

            void MarkUnsafe(bool isUnsafe = true)
            {
                if (unsafeStateStack.IsEmpty)
                    return;
                unsafeStateStack.Peek().UseUnsafeConstructs = isUnsafe;
            }

			public override void VisitPointerType(PointerTypeSyntax node)
            {
                base.VisitPointerType(node);
                MarkUnsafe();
            }

            public override void VisitFixedStatement(FixedStatementSyntax node)
            {
                base.VisitFixedStatement(node);
                MarkUnsafe();
            }

            public override void VisitSizeOfExpression(SizeOfExpressionSyntax node)
            {
                base.VisitSizeOfExpression(node);
                MarkUnsafe();
            }

            public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
            {
                base.VisitPrefixUnaryExpression(node);
                if (node.IsKind(SyntaxKind.AddressOfExpression) || node.IsKind(SyntaxKind.PointerIndirectionExpression))  // TODO: Check
                    MarkUnsafe();
            }

            public override void VisitIdentifierName(IdentifierNameSyntax node)
            {
                base.VisitIdentifierName(node);
                if (!IsUnsafeContext ())
                    return;

                ISymbol symbol = semanticModel.GetSymbolInfo(node).Symbol;
                if (symbol != null)
                {
                    switch (symbol.Kind)
                    {
                        case SymbolKind.ArrayType:
                        case SymbolKind.DynamicType:
                        case SymbolKind.ErrorType:
                        case SymbolKind.Event:
                        case SymbolKind.Field:
                        case SymbolKind.Method:
                        case SymbolKind.NamedType:
                        case SymbolKind.Parameter:
                        case SymbolKind.PointerType:
                        case SymbolKind.Property:
                        case SymbolKind.RangeVariable:
                            if (symbol.IsUnsafe())
                                MarkUnsafe();
                            break;
                        default:
                            break;
                    }
                }
            }
		}
    }
}