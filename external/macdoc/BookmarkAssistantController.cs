using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace macdoc
{
	public partial class BookmarkAssistantController : AppKit.NSViewController
	{
		IList<BookmarkManager.Entry> entries;
		BookmarkDataSource source;
		
		public BookmarkAssistantController (IntPtr handle) : base (handle)
		{
		}

		public BookmarkAssistantController (IList<BookmarkManager.Entry> entries) : base ("BookmarkAssistant", NSBundle.MainBundle)
		{
			this.entries = entries;
			this.source = new BookmarkDataSource (entries, AppDelegate.BookmarkManager);
			View.TableView.DataSource = source;
			View.BookmarkDeleted += HandleBookmarkDeleted;
		}

		void HandleBookmarkDeleted (int row)
		{
			var entry = entries[row];
			AppDelegate.BookmarkManager.DeleteBookmark (entry);
			Logger.Log ("Removed entry {0}", entry.Name);
			View.TableView.ReloadData ();
		}

		public new BookmarkAssistant View {
			get {
				return (BookmarkAssistant)base.View;
			}
		}
		
		public class BookmarkDataSource : NSTableViewDataSource
		{
			IList<BookmarkManager.Entry> entries;
			BookmarkManager manager;
			
			public BookmarkDataSource (IList<BookmarkManager.Entry> entries, BookmarkManager manager)
			{
				this.entries = entries;
				this.manager = manager;
			}
			
			public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
			{
				if (tableColumn == null)
					return null;

				var columnIndex = tableView.FindColumn ((NSString)tableColumn.Identifier);
				switch (columnIndex) {
				case 0:
					return new NSString (entries[(int)row].Name);
				case 1:
					return new NSString (entries[(int)row].Notes);
				case 2:
					return new NSString (entries[(int)row].Url);
				default:
					return null;
				}
			}
			
			public override void SetObjectValue (NSTableView tableView, NSObject theObject, NSTableColumn tableColumn, nint row)
			{
				NSString newNSValue = theObject as NSString;
				if (newNSValue == null)
					return;
				string newValue = newNSValue.ToString ();
				var columnIndex = tableView.FindColumn ((NSString)tableColumn.Identifier);
				BookmarkManager.Entry entry = entries[(int)row];
				switch (columnIndex) {
				case 0:
					if (!string.IsNullOrWhiteSpace (newValue))
						entry.Name = newValue;
					break;
				case 1:
					entry.Notes = newValue;
					break;
				case 2:
					if (!string.IsNullOrWhiteSpace (newValue))
						entry.Url = newValue;
					break;
				default:
					break;
				}
				manager.CommitBookmarkChange (entry);
			}
			
			public override nint GetRowCount (NSTableView tableView)
			{
				return entries.Count;
			}
		}
	}
}

