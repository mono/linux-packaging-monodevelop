// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace CBinding {
    
    public partial class PackageDetails {
        
        private Gtk.VBox vbox3;
        
        private Gtk.Table table1;
        
        private Gtk.Label descriptionLabel;
        
        private Gtk.Label label7;
        
        private Gtk.Label label8;
        
        private Gtk.Label label9;
        
        private Gtk.Label nameLabel;
        
        private Gtk.Label versionLabel;
        
        private Gtk.VBox vbox4;
        
        private Gtk.Label label13;
        
        private Gtk.ScrolledWindow scrolledwindow1;
        
        private Gtk.TreeView requiresTreeView;
        
        private Gtk.VBox vbox2;
        
        private Gtk.Label label1;
        
        private Gtk.HBox hbox1;
        
        private Gtk.ScrolledWindow scrolledwindow2;
        
        private Gtk.TreeView libPathsTreeView;
        
        private Gtk.ScrolledWindow scrolledwindow3;
        
        private Gtk.TreeView libsTreeView;
        
        private Gtk.VBox vbox5;
        
        private Gtk.Label label2;
        
        private Gtk.ScrolledWindow scrolledwindow4;
        
        private Gtk.TreeView cflagsTreeView;
        
        private Gtk.Button buttonOk;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget CBinding.PackageDetails
            this.Name = "CBinding.PackageDetails";
            this.Title = Mono.Unix.Catalog.GetString("Package Details");
            this.WindowPosition = ((Gtk.WindowPosition)(4));
            // Internal child CBinding.PackageDetails.VBox
            Gtk.VBox w1 = this.VBox;
            w1.Name = "dialog1_VBox";
            w1.BorderWidth = ((uint)(2));
            // Container child dialog1_VBox.Gtk.Box+BoxChild
            this.vbox3 = new Gtk.VBox();
            this.vbox3.Name = "vbox3";
            this.vbox3.Spacing = 6;
            this.vbox3.BorderWidth = ((uint)(3));
            // Container child vbox3.Gtk.Box+BoxChild
            this.table1 = new Gtk.Table(((uint)(3)), ((uint)(2)), false);
            this.table1.Name = "table1";
            this.table1.RowSpacing = ((uint)(6));
            this.table1.ColumnSpacing = ((uint)(6));
            // Container child table1.Gtk.Table+TableChild
            this.descriptionLabel = new Gtk.Label();
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Xalign = 0F;
            this.descriptionLabel.Yalign = 0F;
            this.descriptionLabel.LabelProp = "label12";
            this.table1.Add(this.descriptionLabel);
            Gtk.Table.TableChild w2 = ((Gtk.Table.TableChild)(this.table1[this.descriptionLabel]));
            w2.TopAttach = ((uint)(2));
            w2.BottomAttach = ((uint)(3));
            w2.LeftAttach = ((uint)(1));
            w2.RightAttach = ((uint)(2));
            w2.XOptions = ((Gtk.AttachOptions)(4));
            w2.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label7 = new Gtk.Label();
            this.label7.Name = "label7";
            this.label7.Xalign = 0F;
            this.label7.LabelProp = Mono.Unix.Catalog.GetString("Name:");
            this.table1.Add(this.label7);
            Gtk.Table.TableChild w3 = ((Gtk.Table.TableChild)(this.table1[this.label7]));
            w3.XOptions = ((Gtk.AttachOptions)(4));
            w3.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label8 = new Gtk.Label();
            this.label8.Name = "label8";
            this.label8.Xalign = 0F;
            this.label8.LabelProp = Mono.Unix.Catalog.GetString("Version:");
            this.table1.Add(this.label8);
            Gtk.Table.TableChild w4 = ((Gtk.Table.TableChild)(this.table1[this.label8]));
            w4.TopAttach = ((uint)(1));
            w4.BottomAttach = ((uint)(2));
            w4.XOptions = ((Gtk.AttachOptions)(4));
            w4.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label9 = new Gtk.Label();
            this.label9.Name = "label9";
            this.label9.Xalign = 0F;
            this.label9.Yalign = 0F;
            this.label9.LabelProp = Mono.Unix.Catalog.GetString("Description:");
            this.table1.Add(this.label9);
            Gtk.Table.TableChild w5 = ((Gtk.Table.TableChild)(this.table1[this.label9]));
            w5.TopAttach = ((uint)(2));
            w5.BottomAttach = ((uint)(3));
            w5.XOptions = ((Gtk.AttachOptions)(4));
            w5.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.nameLabel = new Gtk.Label();
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Xalign = 0F;
            this.nameLabel.LabelProp = "label10";
            this.table1.Add(this.nameLabel);
            Gtk.Table.TableChild w6 = ((Gtk.Table.TableChild)(this.table1[this.nameLabel]));
            w6.LeftAttach = ((uint)(1));
            w6.RightAttach = ((uint)(2));
            w6.XOptions = ((Gtk.AttachOptions)(4));
            w6.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.versionLabel = new Gtk.Label();
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Xalign = 0F;
            this.versionLabel.LabelProp = "label11";
            this.table1.Add(this.versionLabel);
            Gtk.Table.TableChild w7 = ((Gtk.Table.TableChild)(this.table1[this.versionLabel]));
            w7.TopAttach = ((uint)(1));
            w7.BottomAttach = ((uint)(2));
            w7.LeftAttach = ((uint)(1));
            w7.RightAttach = ((uint)(2));
            w7.XOptions = ((Gtk.AttachOptions)(4));
            w7.YOptions = ((Gtk.AttachOptions)(4));
            this.vbox3.Add(this.table1);
            Gtk.Box.BoxChild w8 = ((Gtk.Box.BoxChild)(this.vbox3[this.table1]));
            w8.Position = 0;
            w8.Expand = false;
            w8.Fill = false;
            // Container child vbox3.Gtk.Box+BoxChild
            this.vbox4 = new Gtk.VBox();
            this.vbox4.Name = "vbox4";
            this.vbox4.Spacing = 6;
            // Container child vbox4.Gtk.Box+BoxChild
            this.label13 = new Gtk.Label();
            this.label13.Name = "label13";
            this.label13.Xalign = 0F;
            this.label13.LabelProp = Mono.Unix.Catalog.GetString("Requires:");
            this.vbox4.Add(this.label13);
            Gtk.Box.BoxChild w9 = ((Gtk.Box.BoxChild)(this.vbox4[this.label13]));
            w9.Position = 0;
            w9.Expand = false;
            w9.Fill = false;
            // Container child vbox4.Gtk.Box+BoxChild
            this.scrolledwindow1 = new Gtk.ScrolledWindow();
            this.scrolledwindow1.CanFocus = true;
            this.scrolledwindow1.Name = "scrolledwindow1";
            this.scrolledwindow1.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow1.Gtk.Container+ContainerChild
            Gtk.Viewport w10 = new Gtk.Viewport();
            w10.ShadowType = ((Gtk.ShadowType)(0));
            // Container child GtkViewport.Gtk.Container+ContainerChild
            this.requiresTreeView = new Gtk.TreeView();
            this.requiresTreeView.CanFocus = true;
            this.requiresTreeView.Name = "requiresTreeView";
            w10.Add(this.requiresTreeView);
            this.scrolledwindow1.Add(w10);
            this.vbox4.Add(this.scrolledwindow1);
            Gtk.Box.BoxChild w13 = ((Gtk.Box.BoxChild)(this.vbox4[this.scrolledwindow1]));
            w13.Position = 1;
            this.vbox3.Add(this.vbox4);
            Gtk.Box.BoxChild w14 = ((Gtk.Box.BoxChild)(this.vbox3[this.vbox4]));
            w14.Position = 1;
            // Container child vbox3.Gtk.Box+BoxChild
            this.vbox2 = new Gtk.VBox();
            this.vbox2.Name = "vbox2";
            this.vbox2.Spacing = 6;
            // Container child vbox2.Gtk.Box+BoxChild
            this.label1 = new Gtk.Label();
            this.label1.Name = "label1";
            this.label1.Xalign = 0F;
            this.label1.LabelProp = Mono.Unix.Catalog.GetString("Libs:");
            this.vbox2.Add(this.label1);
            Gtk.Box.BoxChild w15 = ((Gtk.Box.BoxChild)(this.vbox2[this.label1]));
            w15.Position = 0;
            w15.Expand = false;
            w15.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.hbox1 = new Gtk.HBox();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 6;
            // Container child hbox1.Gtk.Box+BoxChild
            this.scrolledwindow2 = new Gtk.ScrolledWindow();
            this.scrolledwindow2.CanFocus = true;
            this.scrolledwindow2.Name = "scrolledwindow2";
            this.scrolledwindow2.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow2.Gtk.Container+ContainerChild
            Gtk.Viewport w16 = new Gtk.Viewport();
            w16.ShadowType = ((Gtk.ShadowType)(0));
            // Container child GtkViewport1.Gtk.Container+ContainerChild
            this.libPathsTreeView = new Gtk.TreeView();
            this.libPathsTreeView.CanFocus = true;
            this.libPathsTreeView.Name = "libPathsTreeView";
            w16.Add(this.libPathsTreeView);
            this.scrolledwindow2.Add(w16);
            this.hbox1.Add(this.scrolledwindow2);
            Gtk.Box.BoxChild w19 = ((Gtk.Box.BoxChild)(this.hbox1[this.scrolledwindow2]));
            w19.Position = 0;
            // Container child hbox1.Gtk.Box+BoxChild
            this.scrolledwindow3 = new Gtk.ScrolledWindow();
            this.scrolledwindow3.CanFocus = true;
            this.scrolledwindow3.Name = "scrolledwindow3";
            this.scrolledwindow3.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow3.Gtk.Container+ContainerChild
            Gtk.Viewport w20 = new Gtk.Viewport();
            w20.ShadowType = ((Gtk.ShadowType)(0));
            // Container child GtkViewport2.Gtk.Container+ContainerChild
            this.libsTreeView = new Gtk.TreeView();
            this.libsTreeView.CanFocus = true;
            this.libsTreeView.Name = "libsTreeView";
            w20.Add(this.libsTreeView);
            this.scrolledwindow3.Add(w20);
            this.hbox1.Add(this.scrolledwindow3);
            Gtk.Box.BoxChild w23 = ((Gtk.Box.BoxChild)(this.hbox1[this.scrolledwindow3]));
            w23.Position = 1;
            this.vbox2.Add(this.hbox1);
            Gtk.Box.BoxChild w24 = ((Gtk.Box.BoxChild)(this.vbox2[this.hbox1]));
            w24.Position = 1;
            this.vbox3.Add(this.vbox2);
            Gtk.Box.BoxChild w25 = ((Gtk.Box.BoxChild)(this.vbox3[this.vbox2]));
            w25.Position = 2;
            // Container child vbox3.Gtk.Box+BoxChild
            this.vbox5 = new Gtk.VBox();
            this.vbox5.Name = "vbox5";
            this.vbox5.Spacing = 6;
            // Container child vbox5.Gtk.Box+BoxChild
            this.label2 = new Gtk.Label();
            this.label2.Name = "label2";
            this.label2.Xalign = 0F;
            this.label2.LabelProp = Mono.Unix.Catalog.GetString("CFlags:");
            this.vbox5.Add(this.label2);
            Gtk.Box.BoxChild w26 = ((Gtk.Box.BoxChild)(this.vbox5[this.label2]));
            w26.Position = 0;
            w26.Expand = false;
            w26.Fill = false;
            // Container child vbox5.Gtk.Box+BoxChild
            this.scrolledwindow4 = new Gtk.ScrolledWindow();
            this.scrolledwindow4.CanFocus = true;
            this.scrolledwindow4.Name = "scrolledwindow4";
            this.scrolledwindow4.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow4.Gtk.Container+ContainerChild
            Gtk.Viewport w27 = new Gtk.Viewport();
            w27.ShadowType = ((Gtk.ShadowType)(0));
            // Container child GtkViewport3.Gtk.Container+ContainerChild
            this.cflagsTreeView = new Gtk.TreeView();
            this.cflagsTreeView.CanFocus = true;
            this.cflagsTreeView.Name = "cflagsTreeView";
            w27.Add(this.cflagsTreeView);
            this.scrolledwindow4.Add(w27);
            this.vbox5.Add(this.scrolledwindow4);
            Gtk.Box.BoxChild w30 = ((Gtk.Box.BoxChild)(this.vbox5[this.scrolledwindow4]));
            w30.Position = 1;
            this.vbox3.Add(this.vbox5);
            Gtk.Box.BoxChild w31 = ((Gtk.Box.BoxChild)(this.vbox3[this.vbox5]));
            w31.Position = 3;
            w1.Add(this.vbox3);
            Gtk.Box.BoxChild w32 = ((Gtk.Box.BoxChild)(w1[this.vbox3]));
            w32.Position = 0;
            // Internal child CBinding.PackageDetails.ActionArea
            Gtk.HButtonBox w33 = this.ActionArea;
            w33.Name = "dialog1_ActionArea";
            w33.Spacing = 6;
            w33.BorderWidth = ((uint)(5));
            w33.LayoutStyle = ((Gtk.ButtonBoxStyle)(4));
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonOk = new Gtk.Button();
            this.buttonOk.CanDefault = true;
            this.buttonOk.CanFocus = true;
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseStock = true;
            this.buttonOk.UseUnderline = true;
            this.buttonOk.Label = "gtk-ok";
            this.AddActionWidget(this.buttonOk, -5);
            Gtk.ButtonBox.ButtonBoxChild w34 = ((Gtk.ButtonBox.ButtonBoxChild)(w33[this.buttonOk]));
            w34.Expand = false;
            w34.Fill = false;
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 608;
            this.DefaultHeight = 518;
            this.Show();
            this.buttonOk.Clicked += new System.EventHandler(this.OnButtonOkClicked);
        }
    }
}