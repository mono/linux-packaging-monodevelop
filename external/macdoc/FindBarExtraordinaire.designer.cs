// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace macdoc
{
	[Register ("FindBarExtraordinaire")]
	partial class FindBarExtraordinaire
	{
		[Outlet]
		AppKit.NSButton caseSensitiveButton { get; set; }

		[Outlet]
		AppKit.NSSearchField searchField { get; set; }

		[Outlet]
		AppKit.NSButton wrapButton { get; set; }

		[Action ("StartSearch:")]
		partial void StartSearch (Foundation.NSObject sender);

		[Action ("CloseFind:")]
		partial void CloseFind (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (caseSensitiveButton != null) {
				caseSensitiveButton.Dispose ();
				caseSensitiveButton = null;
			}

			if (searchField != null) {
				searchField.Dispose ();
				searchField = null;
			}

			if (wrapButton != null) {
				wrapButton.Dispose ();
				wrapButton = null;
			}
		}
	}

	[Register ("FindBarExtraordinaireController")]
	partial class FindBarExtraordinaireController
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
