using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace macdoc
{
	public partial class FindBarExtraordinaireController : AppKit.NSViewController
	{
		public FindBarExtraordinaireController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		public FindBarExtraordinaireController () : base ("FindBarExtraordinaire", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		void Initialize ()
		{
		}
		
		public new FindBarExtraordinaire View {
			get {
				return (FindBarExtraordinaire)base.View;
			}
		}
	}
}

