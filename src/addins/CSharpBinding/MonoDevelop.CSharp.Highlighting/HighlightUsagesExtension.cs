// 
// HighlightUsagesExtension.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using MonoDevelop.Ide.Gui.Content;
using Mono.TextEditor;
using MonoDevelop.Projects.Dom;
using MonoDevelop.CSharp.Refactoring;
using System.Collections.Generic;
using MonoDevelop.Projects.Dom.Parser;
using Gdk;
using MonoDevelop.CSharp.Resolver;
using MonoDevelop.Projects.Text;
using System.Linq;
using MonoDevelop.Core;

namespace MonoDevelop.CSharp.Highlighting
{
	public class HighlightUsagesExtension : TextEditorExtension
	{
		ProjectDom dom;
		
		TextEditorData textEditorData;
		ITextEditorResolver textEditorResolver;
		
		public override void Initialize ()
		{
			base.Initialize ();
			
			dom = Document.Dom;
			
			textEditorResolver = base.Document.GetContent<ITextEditorResolver> ();
			textEditorData = base.Document.TextEditorData;
			textEditorData.Caret.PositionChanged += HandleTextEditorDataCaretPositionChanged;
			textEditorData.Document.TextReplaced += HandleTextEditorDataDocumentTextReplaced;
			
		}

		void HandleTextEditorDataDocumentTextReplaced (object sender, ReplaceEventArgs e)
		{
			RemoveMarkers (false);
		}
		
		public override void Dispose ()
		{
			textEditorData.Caret.PositionChanged -= HandleTextEditorDataCaretPositionChanged;
			textEditorData.Document.TextReplaced -= HandleTextEditorDataDocumentTextReplaced;
			base.Dispose ();
			RemoveTimer ();
		}
		
		uint popupTimer = 0;
		
		public bool IsTimerOnQueue {
			get {
				return popupTimer != 0;
			}
		}
		
		public void ForceUpdate ()
		{
			RemoveTimer ();
			DelayedTooltipShow ();
		}
		
		void RemoveTimer ()
		{
			if (popupTimer != 0) {
				GLib.Source.Remove (popupTimer);
				popupTimer = 0;
			}
		}

		void HandleTextEditorDataCaretPositionChanged (object sender, DocumentLocationEventArgs e)
		{
			if (!textEditorData.IsSomethingSelected && markers.Values.Any (m => m.Contains (textEditorData.Caret.Offset)))
				return;
			RemoveMarkers (textEditorData.IsSomethingSelected);
			RemoveTimer ();
			if (!textEditorData.IsSomethingSelected)
				popupTimer = GLib.Timeout.Add (1000, DelayedTooltipShow);
		}
		
		bool DelayedTooltipShow ()
		{
			try {
				int caretOffset = textEditorData.Caret.Offset;
				int start = Math.Min (caretOffset, textEditorData.Document.Length - 1);
				while (start > 0) {
					char ch = textEditorData.Document.GetCharAt (start);
					if (!char.IsLetterOrDigit (ch) && ch != '_' && ch != '.') {
						start++;
						break;
					}
					start--;
				}
				
				int end = Math.Max (caretOffset, 0);
				while (end < textEditorData.Document.Length) {
					char ch = textEditorData.Document.GetCharAt (end);
					if (!char.IsLetterOrDigit (ch) && ch != '_')
						break;
					end++;
				}
				
				if (start < 0 || start >= end) 
					return false;
				
				string expression = textEditorData.Document.GetTextBetween (start, end);
				ResolveResult resolveResult = textEditorResolver.GetLanguageItem (caretOffset, expression);
				if (resolveResult == null)
					return false;
				if (resolveResult is AggregatedResolveResult) {
					foreach (var curResult in ((AggregatedResolveResult)resolveResult).ResolveResults) {
						var references = GetReferences (curResult);
						if (references.Any (r => r.Position <= caretOffset && caretOffset <= r.Position  + r.Name.Length )) {
							ShowReferences (references);
							break;
						}
					}
				} else {
					ShowReferences (GetReferences (resolveResult));
				}
			} catch (Exception e) {
				LoggingService.LogError ("Unhandled Exception in HighlightingUsagesExtension", e);
			} finally {
				popupTimer = 0;
			}
			return false;
		}
		
		void ShowReferences (List<MonoDevelop.Projects.CodeGeneration.MemberReference> references)
		{
			RemoveMarkers (false);
			if (references != null) {
				bool alphaBlend = false;
				foreach (var r in references) {
					UsageMarker marker = GetMarker (r.Line - 1);
					int offset = textEditorData.Document.LocationToOffset (r.Line - 1, r.Column - 1);
					if (!alphaBlend && textEditorData.Parent.TextViewMargin.SearchResults.Any (sr => sr.Contains (offset) || sr.Contains (offset + r.Name.Length) ||
					                                                        offset < sr.Offset && sr.EndOffset < offset + r.Name.Length)) {
						textEditorData.Parent.TextViewMargin.AlphaBlendSearchResults = alphaBlend = true;
					}
					marker.Usages.Add (new Mono.TextEditor.Segment (offset, r.Name.Length));
				}
			}
			textEditorData.Document.CommitUpdateAll ();
		}
		
		List<MonoDevelop.Projects.CodeGeneration.MemberReference> GetReferences (ResolveResult resolveResult)
		{
			INode member = null;
			
			if (resolveResult is MemberResolveResult) {
				member = ((MemberResolveResult)resolveResult).ResolvedMember;
				if (member == null)
					member = dom.GetType (resolveResult.ResolvedType);
			}
			if (resolveResult is ParameterResolveResult)
				member = ((ParameterResolveResult)resolveResult).Parameter;
			if (resolveResult is LocalVariableResolveResult)
				member = ((LocalVariableResolveResult)resolveResult).LocalVariable;
			
			if (member != null) {
				try {
					NRefactoryResolver resolver = new NRefactoryResolver (dom, Document.CompilationUnit, ICSharpCode.NRefactory.SupportedLanguage.CSharp, Document.TextEditor, Document.FileName);
					FindMemberAstVisitor visitor = new FindMemberAstVisitor (textEditorData.Document, resolver, member);
					visitor.IncludeXmlDocumentation = true;
					ICSharpCode.NRefactory.Ast.CompilationUnit unit = Document.CompilationUnit.Tag as ICSharpCode.NRefactory.Ast.CompilationUnit;
					if (unit == null)
						return null;
					visitor.RunVisitor (unit);
					return visitor.FoundReferences;
				} catch (Exception e) {
					LoggingService.LogError ("Error in highlight usages extension.", e);
				}
			}
			return null;
		}
		
		Dictionary<int, UsageMarker> markers = new Dictionary<int, UsageMarker> ();
		
		public Dictionary<int, UsageMarker> Markers {
			get { return this.markers; }
		}
		
		void RemoveMarkers (bool updateLine)
		{
			textEditorData.Parent.TextViewMargin.AlphaBlendSearchResults = false;
			if (markers.Count == 0)
				return;
			
			foreach (var pair in markers) {
				textEditorData.Document.RemoveMarker (pair.Value, true);
			}
			markers.Clear ();
		}
		
		UsageMarker GetMarker (int line)
		{
			UsageMarker result;
			if (!markers.TryGetValue (line, out result)) {
				result = new UsageMarker ();
				textEditorData.Document.AddMarker (line, result);
				markers.Add (line, result);
			}
			return result;
		}

		
		public class UsageMarker : TextMarker, IBackgroundMarker
		{
			List<ISegment> usages = new List<ISegment> ();
			
			public List<ISegment> Usages {
				get { return this.usages; }
			}
			
			public bool Contains (int offset)
			{
				return usages.Any (u => u.Offset <= offset && offset <= u.EndOffset);
			}
			
			public bool DrawBackground (TextEditor editor, Gdk.Drawable win, TextViewMargin.LayoutWrapper layout, int selectionStart, int selectionEnd, int startOffset, int endOffset, int y, int startXPos, int endXPos, ref bool drawBg)
			{
				drawBg = false;
				if (selectionStart >= 0 || editor.CurrentMode is TextLinkEditMode)
					return true;
				foreach (ISegment usage in Usages) {
					int markerStart = usage.Offset;
					int markerEnd = usage.EndOffset;
					
					if (markerEnd < startOffset || markerStart > endOffset) 
						return true; 
					
					int @from;
					int to;
					
					if (markerStart < startOffset && endOffset < markerEnd) {
						@from = startXPos;
						to = endXPos;
					} else {
						int start = startOffset < markerStart ? markerStart : startOffset;
						int end = endOffset < markerEnd ? endOffset : markerEnd;
						int x_pos = layout.Layout.IndexToPos (start - startOffset).X;
			
						@from = startXPos + (int)(x_pos / Pango.Scale.PangoScale);
			
						x_pos = layout.Layout.IndexToPos (end - startOffset).X;
			
						to = startXPos + (int)(x_pos / Pango.Scale.PangoScale);
					}
		
					@from = System.Math.Max (@from, editor.TextViewMargin.XOffset);
					to = System.Math.Max (to, editor.TextViewMargin.XOffset);
					if (@from < to) {
						using (Gdk.GC gc = new Gdk.GC(win)) {
							gc.RgbFgColor = editor.ColorStyle.BracketHighlightRectangle.BackgroundColor;
							win.DrawRectangle (gc, true, @from + 1, y + 1, to - @from - 1, editor.LineHeight - 2);
							gc.RgbFgColor = editor.ColorStyle.BracketHighlightRectangle.Color;
							win.DrawRectangle (gc, false, @from, y, to - @from, editor.LineHeight - 1);
						}
					}
				}
				return true;
			}
		}
	}
}

