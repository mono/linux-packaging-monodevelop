using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace macdoc
{
	class MainClass
	{
		static void Main (string[] args)
		{
			NSApplication.Init ();
			NSApplication.Main (args);
		}
	}
}

