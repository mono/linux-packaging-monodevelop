
// This file has been generated by the GUI designer. Do not modify.
namespace MonoDevelop.AspNet.Deployment
{
	public partial class WebDeployLaunchDialog
	{
		private global::Gtk.Label titleLabel;

		private global::Gtk.Label label1;

		private global::Gtk.ScrolledWindow scrolledwindow4;

		private global::Gtk.TreeView targetView;

		private global::Gtk.Button button6;

		private global::Gtk.Button buttonCancel;

		private global::Gtk.Button buttonDeploy;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoDevelop.AspNet.Deployment.WebDeployLaunchDialog
			this.Name = "MonoDevelop.AspNet.Deployment.WebDeployLaunchDialog";
			this.Title = global::MonoDevelop.Core.GettextCatalog.GetString ("Deploy to Web");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			this.Modal = true;
			this.BorderWidth = ((uint)(9));
			// Internal child MonoDevelop.AspNet.Deployment.WebDeployLaunchDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.Spacing = 6;
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.titleLabel = new global::Gtk.Label ();
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Xalign = 0f;
			this.titleLabel.LabelProp = global::MonoDevelop.Core.GettextCatalog.GetString ("<big><b>Deploying Web Project...</b></big>");
			this.titleLabel.UseMarkup = true;
			w1.Add (this.titleLabel);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(w1[this.titleLabel]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.Xalign = 0f;
			this.label1.LabelProp = global::MonoDevelop.Core.GettextCatalog.GetString ("Targets to which the project should be deployed:");
			w1.Add (this.label1);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(w1[this.label1]));
			w3.Position = 1;
			w3.Expand = false;
			w3.Fill = false;
			w3.Padding = ((uint)(4));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.scrolledwindow4 = new global::Gtk.ScrolledWindow ();
			this.scrolledwindow4.CanFocus = true;
			this.scrolledwindow4.Name = "scrolledwindow4";
			this.scrolledwindow4.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child scrolledwindow4.Gtk.Container+ContainerChild
			this.targetView = new global::Gtk.TreeView ();
			this.targetView.CanFocus = true;
			this.targetView.Name = "targetView";
			this.scrolledwindow4.Add (this.targetView);
			w1.Add (this.scrolledwindow4);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(w1[this.scrolledwindow4]));
			w5.Position = 2;
			// Internal child MonoDevelop.AspNet.Deployment.WebDeployLaunchDialog.ActionArea
			global::Gtk.HButtonBox w6 = this.ActionArea;
			w6.Name = "dialog1_ActionArea";
			w6.Spacing = 6;
			w6.BorderWidth = ((uint)(5));
			w6.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.button6 = new global::Gtk.Button ();
			this.button6.CanFocus = true;
			this.button6.Name = "button6";
			this.button6.UseUnderline = true;
			// Container child button6.Gtk.Container+ContainerChild
			global::Gtk.Alignment w7 = new global::Gtk.Alignment (0.5f, 0.5f, 0f, 0f);
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			global::Gtk.HBox w8 = new global::Gtk.HBox ();
			w8.Spacing = 2;
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Image w9 = new global::Gtk.Image ();
			w9.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-edit", global::Gtk.IconSize.Menu);
			w8.Add (w9);
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Label w11 = new global::Gtk.Label ();
			w11.LabelProp = global::MonoDevelop.Core.GettextCatalog.GetString ("_Edit targets");
			w11.UseUnderline = true;
			w8.Add (w11);
			w7.Add (w8);
			this.button6.Add (w7);
			this.AddActionWidget (this.button6, -11);
			global::Gtk.ButtonBox.ButtonBoxChild w15 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w6[this.button6]));
			w15.Expand = false;
			w15.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w16 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w6[this.buttonCancel]));
			w16.Position = 1;
			w16.Expand = false;
			w16.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonDeploy = new global::Gtk.Button ();
			this.buttonDeploy.CanDefault = true;
			this.buttonDeploy.CanFocus = true;
			this.buttonDeploy.Name = "buttonDeploy";
			this.buttonDeploy.UseUnderline = true;
			// Container child buttonDeploy.Gtk.Container+ContainerChild
			global::Gtk.Alignment w17 = new global::Gtk.Alignment (0.5f, 0.5f, 0f, 0f);
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			global::Gtk.HBox w18 = new global::Gtk.HBox ();
			w18.Spacing = 2;
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Image w19 = new global::Gtk.Image ();
			w19.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-ok", global::Gtk.IconSize.Menu);
			w18.Add (w19);
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Label w21 = new global::Gtk.Label ();
			w21.LabelProp = global::MonoDevelop.Core.GettextCatalog.GetString ("_Deploy");
			w21.UseUnderline = true;
			w18.Add (w21);
			w17.Add (w18);
			this.buttonDeploy.Add (w17);
			this.AddActionWidget (this.buttonDeploy, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w25 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w6[this.buttonDeploy]));
			w25.Position = 2;
			w25.Expand = false;
			w25.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 511;
			this.DefaultHeight = 353;
			this.Show ();
			this.button6.Clicked += new global::System.EventHandler (this.editTargetsClicked);
		}
	}
}
