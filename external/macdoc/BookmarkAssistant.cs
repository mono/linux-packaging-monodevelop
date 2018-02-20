using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace macdoc
{
	public partial class BookmarkAssistant : AppKit.NSView
	{
		public event Action<int> BookmarkDeleted;
		
		public BookmarkAssistant (IntPtr handle) : base (handle)
		{
		}

		partial void DeleteButtonClicked (NSButton sender)
		{
			if (bookmarkTableView.SelectedRowCount != 1)
				return;
			
			var index = bookmarkTableView.SelectedRow;
			if (index < 0 || index > bookmarkTableView.RowCount)
				return;
			var temp = BookmarkDeleted;
			if (temp != null)
				temp ((int)index);
		}
		
		public NSTableView TableView {
			get {
				return bookmarkTableView;
			}
		}
	}
}

