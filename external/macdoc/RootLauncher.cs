using System;
using System.Diagnostics;

namespace macdoc
{
	public static class UrlLauncher
	{
		public static void Launch (string url)
		{
			if (string.IsNullOrEmpty (url))
				throw new ArgumentNullException (url);
			Process.Start (new ProcessStartInfo (url));
		}
	}
}