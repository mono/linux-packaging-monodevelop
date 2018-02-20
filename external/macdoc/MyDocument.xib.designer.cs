// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace macdoc
{
	[Register ("MyDocument")]
	partial class MyDocument
	{
		[Outlet]
		AppKit.NSSegmentedCell navigationCells { get; set; }

		[Outlet]
		AppKit.NSOutlineView outlineView { get; set; }

		[Outlet]
		AppKit.NSTableView searchResults { get; set; }

		[Outlet]
		WebKit.WebView webView { get; set; }

		[Outlet]
		AppKit.NSTabView tabSelector { get; set; }

		[Outlet]
		AppKit.NSView spinnerView { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator spinnerWidget { get; set; }

		[Outlet]
		AppKit.NSView indexSpinnerView { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator indexSpinnerWidget { get; set; }

		[Outlet]
		AppKit.NSButton addBookmarkBtn { get; set; }

		[Outlet]
		AppKit.NSPopUpButton bookmarkSelector { get; set; }

		[Outlet]
		AppKit.NSButton viewBookmarksBtn { get; set; }

		[Outlet]
		AppKit.NSSegmentedCell bookmarkToolbar { get; set; }

		[Outlet]
		AppKit.NSSplitView splitView { get; set; }

		[Outlet]
		AppKit.NSTableView multipleMatchResults { get; set; }

		[Outlet]
		AppKit.NSTableView indexResults { get; set; }

		[Outlet]
		AppKit.NSSearchField indexSearchEntry { get; set; }

		[Outlet]
		AppKit.NSSearchField toolbarSearchEntry { get; set; }

		[Outlet]
		AppKit.NSScrollView searchScrollView { get; set; }
		
		[Action ("IndexItemClicked:")]
		partial void IndexItemClicked (AppKit.NSTableView sender);

		[Action ("StartSearch:")]
		partial void StartSearch (AppKit.NSSearchField sender);

		[Action ("MultipleMatchItemClicked:")]
		partial void MultipleMatchItemClicked (AppKit.NSTableView sender);

		[Action ("SearchItemClicked:")]
		partial void SearchItemClicked (AppKit.NSTableView sender);
 
		[Action ("StartIndexSearch:")]
		partial void StartIndexSearch (AppKit.NSSearchField sender);

		[Action ("BookmarkToolbarClicked:")]
		partial void BookmarkToolbarClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (navigationCells != null) {
				navigationCells.Dispose ();
				navigationCells = null;
			}

			if (outlineView != null) {
				outlineView.Dispose ();
				outlineView = null;
			}

			if (searchResults != null) {
				searchResults.Dispose ();
				searchResults = null;
			}

			if (webView != null) {
				webView.Dispose ();
				webView = null;
			}

			if (tabSelector != null) {
				tabSelector.Dispose ();
				tabSelector = null;
			}

			if (spinnerView != null) {
				spinnerView.Dispose ();
				spinnerView = null;
			}

			if (spinnerWidget != null) {
				spinnerWidget.Dispose ();
				spinnerWidget = null;
			}

			if (indexSpinnerView != null) {
				indexSpinnerView.Dispose ();
				indexSpinnerView = null;
			}

			if (indexSpinnerWidget != null) {
				indexSpinnerWidget.Dispose ();
				indexSpinnerWidget = null;
			}

			if (addBookmarkBtn != null) {
				addBookmarkBtn.Dispose ();
				addBookmarkBtn = null;
			}

			if (bookmarkSelector != null) {
				bookmarkSelector.Dispose ();
				bookmarkSelector = null;
			}

			if (viewBookmarksBtn != null) {
				viewBookmarksBtn.Dispose ();
				viewBookmarksBtn = null;
			}

			if (bookmarkToolbar != null) {
				bookmarkToolbar.Dispose ();
				bookmarkToolbar = null;
			}

			if (splitView != null) {
				splitView.Dispose ();
				splitView = null;
			}

			if (multipleMatchResults != null) {
				multipleMatchResults.Dispose ();
				multipleMatchResults = null;
			}

			if (indexResults != null) {
				indexResults.Dispose ();
				indexResults = null;
			}

			if (indexSearchEntry != null) {
				indexSearchEntry.Dispose ();
				indexSearchEntry = null;
			}

			if (toolbarSearchEntry != null) {
				toolbarSearchEntry.Dispose ();
				toolbarSearchEntry = null;
			}

			if (searchScrollView != null) {
				searchScrollView.Dispose ();
				searchScrollView = null;
			}
		}
	}
}
