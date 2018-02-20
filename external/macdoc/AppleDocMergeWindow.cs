using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace macdoc
{
	public partial class AppleDocMergeWindow : AppKit.NSWindow
	{
		public AppleDocMergeWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
	}
}