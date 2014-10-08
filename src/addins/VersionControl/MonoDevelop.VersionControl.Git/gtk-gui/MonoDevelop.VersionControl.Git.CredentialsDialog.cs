
// This file has been generated by the GUI designer. Do not modify.
namespace MonoDevelop.VersionControl.Git
{
	internal partial class CredentialsDialog
	{
		private global::Gtk.VBox vbox;
		private global::Gtk.Label labelTop;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;
		private global::Gtk.Button buttonNo;
		private global::Gtk.Button buttonYes;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoDevelop.VersionControl.Git.CredentialsDialog
			this.Name = "MonoDevelop.VersionControl.Git.CredentialsDialog";
			this.Title = global::Mono.Unix.Catalog.GetString ("Git Credentials");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child MonoDevelop.VersionControl.Git.CredentialsDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.vbox = new global::Gtk.VBox ();
			this.vbox.Name = "vbox";
			this.vbox.Spacing = 6;
			this.vbox.BorderWidth = ((uint)(9));
			// Container child vbox.Gtk.Box+BoxChild
			this.labelTop = new global::Gtk.Label ();
			this.labelTop.Name = "labelTop";
			this.labelTop.Xalign = 0F;
			this.labelTop.LabelProp = global::Mono.Unix.Catalog.GetString ("Credentials required for the repository: {0}");
			this.vbox.Add (this.labelTop);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox [this.labelTop]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			w1.Add (this.vbox);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(w1 [this.vbox]));
			w3.Position = 0;
			// Internal child MonoDevelop.VersionControl.Git.CredentialsDialog.ActionArea
			global::Gtk.HButtonBox w4 = this.ActionArea;
			w4.Name = "dialog1_ActionArea";
			w4.Spacing = 10;
			w4.BorderWidth = ((uint)(5));
			w4.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w5 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w4 [this.buttonCancel]));
			w5.Expand = false;
			w5.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			this.AddActionWidget (this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w6 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w4 [this.buttonOk]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonNo = new global::Gtk.Button ();
			this.buttonNo.CanFocus = true;
			this.buttonNo.Name = "buttonNo";
			this.buttonNo.UseStock = true;
			this.buttonNo.UseUnderline = true;
			this.buttonNo.Label = "gtk-no";
			w4.Add (this.buttonNo);
			global::Gtk.ButtonBox.ButtonBoxChild w7 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w4 [this.buttonNo]));
			w7.Position = 2;
			w7.Expand = false;
			w7.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonYes = new global::Gtk.Button ();
			this.buttonYes.CanFocus = true;
			this.buttonYes.Name = "buttonYes";
			this.buttonYes.UseStock = true;
			this.buttonYes.UseUnderline = true;
			this.buttonYes.Label = "gtk-yes";
			w4.Add (this.buttonYes);
			global::Gtk.ButtonBox.ButtonBoxChild w8 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w4 [this.buttonYes]));
			w8.Position = 3;
			w8.Expand = false;
			w8.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 500;
			this.DefaultHeight = 132;
			this.buttonNo.Hide ();
			this.buttonYes.Hide ();
			this.Show ();
			this.buttonNo.Clicked += new global::System.EventHandler (this.OnButtonNoClicked);
			this.buttonYes.Clicked += new global::System.EventHandler (this.OnButtonYesClicked);
		}
	}
}