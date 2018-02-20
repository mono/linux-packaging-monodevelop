// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace macdoc
{
	[Register ("AppleDocMergeWindowController")]
	partial class AppleDocMergeWindowController
	{
		[Outlet]
		AppKit.NSProgressIndicator ProgressWidget { get; set; }

		[Outlet]
		AppKit.NSTextField WizardText { get; set; }

		[Outlet]
		AppKit.NSButton WizardButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ProgressWidget != null) {
				ProgressWidget.Dispose ();
				ProgressWidget = null;
			}

			if (WizardText != null) {
				WizardText.Dispose ();
				WizardText = null;
			}

			if (WizardButton != null) {
				WizardButton.Dispose ();
				WizardButton = null;
			}
		}
	}

	[Register ("AppleDocMergeWindow")]
	partial class AppleDocMergeWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
