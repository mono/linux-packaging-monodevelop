
// This file has been generated by the GUI designer. Do not modify.
namespace MonoDevelop.VersionControl.Git
{
	internal partial class EditBranchDialog
	{
		private global::Gtk.VBox vbox5;
		
		private global::Gtk.Table table4;
		
		private global::Gtk.Entry entryName;
		
		private global::Gtk.Label label4;
		
		private global::Gtk.Label labelError;
		
		private global::Gtk.CheckButton checkTrack;
		
		private global::Gtk.Alignment alignment1;
		
		private global::Gtk.ComboBox comboSources;
		
		private global::Gtk.Button buttonCancel;
		
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoDevelop.VersionControl.Git.EditBranchDialog
			this.Name = "MonoDevelop.VersionControl.Git.EditBranchDialog";
			this.Title = global::Mono.Unix.Catalog.GetString ("Branch Properties");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child MonoDevelop.VersionControl.Git.EditBranchDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.vbox5 = new global::Gtk.VBox ();
			this.vbox5.Name = "vbox5";
			this.vbox5.Spacing = 6;
			this.vbox5.BorderWidth = ((uint)(9));
			// Container child vbox5.Gtk.Box+BoxChild
			this.table4 = new global::Gtk.Table (((uint)(2)), ((uint)(2)), false);
			this.table4.Name = "table4";
			this.table4.RowSpacing = ((uint)(6));
			this.table4.ColumnSpacing = ((uint)(6));
			// Container child table4.Gtk.Table+TableChild
			this.entryName = new global::Gtk.Entry ();
			this.entryName.CanFocus = true;
			this.entryName.Name = "entryName";
			this.entryName.IsEditable = true;
			this.entryName.InvisibleChar = '●';
			this.table4.Add (this.entryName);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table4 [this.entryName]));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table4.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Name:");
			this.table4.Add (this.label4);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table4 [this.label4]));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table4.Gtk.Table+TableChild
			this.labelError = new global::Gtk.Label ();
			this.labelError.Name = "labelError";
			this.labelError.Xalign = 0F;
			this.labelError.LabelProp = "Error";
			this.table4.Add (this.labelError);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table4 [this.labelError]));
			w4.TopAttach = ((uint)(1));
			w4.BottomAttach = ((uint)(2));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox5.Add (this.table4);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.table4]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.checkTrack = new global::Gtk.CheckButton ();
			this.checkTrack.CanFocus = true;
			this.checkTrack.Name = "checkTrack";
			this.checkTrack.Label = global::Mono.Unix.Catalog.GetString ("Track a branch or tag:");
			this.checkTrack.DrawIndicator = true;
			this.checkTrack.UseUnderline = true;
			this.vbox5.Add (this.checkTrack);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.checkTrack]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.alignment1 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.alignment1.Name = "alignment1";
			this.alignment1.LeftPadding = ((uint)(18));
			// Container child alignment1.Gtk.Container+ContainerChild
			this.comboSources = new global::Gtk.ComboBox ();
			this.comboSources.Name = "comboSources";
			this.alignment1.Add (this.comboSources);
			this.vbox5.Add (this.alignment1);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.alignment1]));
			w8.Position = 2;
			w8.Expand = false;
			w8.Fill = false;
			w1.Add (this.vbox5);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(w1 [this.vbox5]));
			w9.Position = 0;
			w9.Expand = false;
			w9.Fill = false;
			// Internal child MonoDevelop.VersionControl.Git.EditBranchDialog.ActionArea
			global::Gtk.HButtonBox w10 = this.ActionArea;
			w10.Name = "dialog1_ActionArea";
			w10.Spacing = 10;
			w10.BorderWidth = ((uint)(5));
			w10.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w11 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w10 [this.buttonCancel]));
			w11.Expand = false;
			w11.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			this.AddActionWidget (this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w12 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w10 [this.buttonOk]));
			w12.Position = 1;
			w12.Expand = false;
			w12.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 400;
			this.DefaultHeight = 200;
			this.Hide ();
			this.entryName.Changed += new global::System.EventHandler (this.OnEntryNameChanged);
			this.checkTrack.Toggled += new global::System.EventHandler (this.OnCheckTrackToggled);
		}
	}
}