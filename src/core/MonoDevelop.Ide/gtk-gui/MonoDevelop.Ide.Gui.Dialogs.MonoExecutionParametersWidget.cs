// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace MonoDevelop.Ide.Gui.Dialogs {
    
    public partial class MonoExecutionParametersWidget {
        
        private Gtk.HBox hbox1;
        
        private MonoDevelop.Components.PropertyGrid.PropertyGrid propertyGrid;
        
        private Gtk.VBox vbox4;
        
        private Gtk.Button buttonReset;
        
        private Gtk.Button buttonPreview;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget MonoDevelop.Ide.Gui.Dialogs.MonoExecutionParametersWidget
            Stetic.BinContainer.Attach(this);
            this.Name = "MonoDevelop.Ide.Gui.Dialogs.MonoExecutionParametersWidget";
            // Container child MonoDevelop.Ide.Gui.Dialogs.MonoExecutionParametersWidget.Gtk.Container+ContainerChild
            this.hbox1 = new Gtk.HBox();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 6;
            this.hbox1.BorderWidth = ((uint)(6));
            // Container child hbox1.Gtk.Box+BoxChild
            this.propertyGrid = new MonoDevelop.Components.PropertyGrid.PropertyGrid();
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.ShowToolbar = false;
            this.propertyGrid.ShowHelp = true;
            this.hbox1.Add(this.propertyGrid);
            Gtk.Box.BoxChild w1 = ((Gtk.Box.BoxChild)(this.hbox1[this.propertyGrid]));
            w1.Position = 0;
            // Container child hbox1.Gtk.Box+BoxChild
            this.vbox4 = new Gtk.VBox();
            this.vbox4.Name = "vbox4";
            this.vbox4.Spacing = 6;
            // Container child vbox4.Gtk.Box+BoxChild
            this.buttonReset = new Gtk.Button();
            this.buttonReset.CanFocus = true;
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.UseUnderline = true;
            // Container child buttonReset.Gtk.Container+ContainerChild
            Gtk.Alignment w2 = new Gtk.Alignment(0.5F, 0.5F, 0F, 0F);
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            Gtk.HBox w3 = new Gtk.HBox();
            w3.Spacing = 2;
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Image w4 = new Gtk.Image();
            w4.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-clear", Gtk.IconSize.Menu, 16);
            w3.Add(w4);
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Label w6 = new Gtk.Label();
            w6.LabelProp = Mono.Unix.Catalog.GetString("Clear All Options");
            w6.UseUnderline = true;
            w3.Add(w6);
            w2.Add(w3);
            this.buttonReset.Add(w2);
            this.vbox4.Add(this.buttonReset);
            Gtk.Box.BoxChild w10 = ((Gtk.Box.BoxChild)(this.vbox4[this.buttonReset]));
            w10.Position = 0;
            w10.Expand = false;
            w10.Fill = false;
            // Container child vbox4.Gtk.Box+BoxChild
            this.buttonPreview = new Gtk.Button();
            this.buttonPreview.CanFocus = true;
            this.buttonPreview.Name = "buttonPreview";
            this.buttonPreview.UseUnderline = true;
            this.buttonPreview.Label = Mono.Unix.Catalog.GetString("Preview Options");
            this.vbox4.Add(this.buttonPreview);
            Gtk.Box.BoxChild w11 = ((Gtk.Box.BoxChild)(this.vbox4[this.buttonPreview]));
            w11.Position = 1;
            w11.Expand = false;
            w11.Fill = false;
            this.hbox1.Add(this.vbox4);
            Gtk.Box.BoxChild w12 = ((Gtk.Box.BoxChild)(this.hbox1[this.vbox4]));
            w12.Position = 1;
            w12.Expand = false;
            w12.Fill = false;
            this.Add(this.hbox1);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Hide();
            this.buttonReset.Clicked += new System.EventHandler(this.OnButtonResetClicked);
            this.buttonPreview.Clicked += new System.EventHandler(this.OnButtonPreviewClicked);
        }
    }
}