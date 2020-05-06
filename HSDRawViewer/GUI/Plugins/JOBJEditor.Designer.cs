namespace HSDRawViewer.GUI.Plugins
{
    partial class JOBJEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JOBJEditor));
            this.listDOBJ = new System.Windows.Forms.CheckedListBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.treeJOBJ = new System.Windows.Forms.TreeView();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.jointOptionsDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.recalculateInverseBindsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceBonesFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.createOutlineMeshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addDummyDOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearAllPOBJsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.materialDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonMoveUp = new System.Windows.Forms.ToolStripButton();
            this.buttonMoveDown = new System.Windows.Forms.ToolStripButton();
            this.buttonDOBJDelete = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.previewBox = new System.Windows.Forms.GroupBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.importModelFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportModelToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.importAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asMayaANIMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asFigatreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asAnimJointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.showInViewportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showBonesToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.showSelectionOutlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.renderModeBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox2 = new System.Windows.Forms.ToolStripComboBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.importBoneINIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.meleeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reorientSkeletonForFighterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listDOBJ
            // 
            this.listDOBJ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listDOBJ.Location = new System.Drawing.Point(3, 28);
            this.listDOBJ.Name = "listDOBJ";
            this.listDOBJ.Size = new System.Drawing.Size(323, 100);
            this.listDOBJ.TabIndex = 0;
            this.listDOBJ.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ListDOBJ_ItemCheck);
            this.listDOBJ.BindingContextChanged += new System.EventHandler(this.listDOBJ_BindingContextChanged);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(11, 19);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid1.Size = new System.Drawing.Size(747, 237);
            this.propertyGrid1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(337, 157);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.treeJOBJ);
            this.tabPage1.Controls.Add(this.toolStrip3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(329, 131);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Joints";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // treeJOBJ
            // 
            this.treeJOBJ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeJOBJ.ItemHeight = 20;
            this.treeJOBJ.Location = new System.Drawing.Point(3, 28);
            this.treeJOBJ.Name = "treeJOBJ";
            this.treeJOBJ.Size = new System.Drawing.Size(323, 100);
            this.treeJOBJ.TabIndex = 0;
            // 
            // toolStrip3
            // 
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jointOptionsDropDownButton1});
            this.toolStrip3.Location = new System.Drawing.Point(3, 3);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(323, 25);
            this.toolStrip3.TabIndex = 1;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // jointOptionsDropDownButton1
            // 
            this.jointOptionsDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.jointOptionsDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importBoneINIToolStripMenuItem,
            this.toolStripSeparator1,
            this.meleeToolStripMenuItem,
            this.toolStripSeparator3,
            this.recalculateInverseBindsToolStripMenuItem,
            this.replaceBonesFromFileToolStripMenuItem});
            this.jointOptionsDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("jointOptionsDropDownButton1.Image")));
            this.jointOptionsDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.jointOptionsDropDownButton1.Name = "jointOptionsDropDownButton1";
            this.jointOptionsDropDownButton1.Size = new System.Drawing.Size(62, 22);
            this.jointOptionsDropDownButton1.Text = "Options";
            // 
            // recalculateInverseBindsToolStripMenuItem
            // 
            this.recalculateInverseBindsToolStripMenuItem.Name = "recalculateInverseBindsToolStripMenuItem";
            this.recalculateInverseBindsToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.recalculateInverseBindsToolStripMenuItem.Text = "Recalculate Inverse Binds";
            this.recalculateInverseBindsToolStripMenuItem.Click += new System.EventHandler(this.recalculateInverseBindsToolStripMenuItem_Click);
            // 
            // replaceBonesFromFileToolStripMenuItem
            // 
            this.replaceBonesFromFileToolStripMenuItem.Name = "replaceBonesFromFileToolStripMenuItem";
            this.replaceBonesFromFileToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.replaceBonesFromFileToolStripMenuItem.Text = "Replace Bones From File";
            this.replaceBonesFromFileToolStripMenuItem.Click += new System.EventHandler(this.replaceBonesFromFileToolStripMenuItem_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listDOBJ);
            this.tabPage2.Controls.Add(this.toolStrip2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(329, 131);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Objects";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton3,
            this.materialDropDownButton1,
            this.buttonMoveUp,
            this.buttonMoveDown,
            this.buttonDOBJDelete});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(323, 25);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripDropDownButton3
            // 
            this.toolStripDropDownButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createOutlineMeshToolStripMenuItem,
            this.addDummyDOBJToolStripMenuItem,
            this.clearAllPOBJsToolStripMenuItem});
            this.toolStripDropDownButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton3.Image")));
            this.toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            this.toolStripDropDownButton3.Size = new System.Drawing.Size(40, 22);
            this.toolStripDropDownButton3.Text = "Edit";
            // 
            // createOutlineMeshToolStripMenuItem
            // 
            this.createOutlineMeshToolStripMenuItem.Name = "createOutlineMeshToolStripMenuItem";
            this.createOutlineMeshToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.createOutlineMeshToolStripMenuItem.Text = "Create Outline Mesh";
            this.createOutlineMeshToolStripMenuItem.Click += new System.EventHandler(this.createOutlineMeshToolStripMenuItem_Click);
            // 
            // addDummyDOBJToolStripMenuItem
            // 
            this.addDummyDOBJToolStripMenuItem.Name = "addDummyDOBJToolStripMenuItem";
            this.addDummyDOBJToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.addDummyDOBJToolStripMenuItem.Text = "Add Dummy DOBJ";
            this.addDummyDOBJToolStripMenuItem.Click += new System.EventHandler(this.addDummyDOBJToolStripMenuItem_Click);
            // 
            // clearAllPOBJsToolStripMenuItem
            // 
            this.clearAllPOBJsToolStripMenuItem.Name = "clearAllPOBJsToolStripMenuItem";
            this.clearAllPOBJsToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.clearAllPOBJsToolStripMenuItem.Text = "Clear All POBJs";
            this.clearAllPOBJsToolStripMenuItem.Click += new System.EventHandler(this.clearAllPOBJsToolStripMenuItem_Click);
            // 
            // materialDropDownButton1
            // 
            this.materialDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.materialDropDownButton1.Image = global::HSDRawViewer.Properties.Resources.ico_mobj;
            this.materialDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.materialDropDownButton1.Name = "materialDropDownButton1";
            this.materialDropDownButton1.Size = new System.Drawing.Size(79, 22);
            this.materialDropDownButton1.Text = "Material";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Image = global::HSDRawViewer.Properties.Resources.ts_importfile;
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Image = global::HSDRawViewer.Properties.Resources.ts_exportfile;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonMoveUp.Image = global::HSDRawViewer.Properties.Resources.ts_up;
            this.buttonMoveUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(23, 22);
            this.buttonMoveUp.Text = "Move Up";
            this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonMoveDown.Image = global::HSDRawViewer.Properties.Resources.ts_down;
            this.buttonMoveDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(23, 22);
            this.buttonMoveDown.Text = "Move Down";
            this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
            // 
            // buttonDOBJDelete
            // 
            this.buttonDOBJDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDOBJDelete.Image = global::HSDRawViewer.Properties.Resources.ts_subtract;
            this.buttonDOBJDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDOBJDelete.Name = "buttonDOBJDelete";
            this.buttonDOBJDelete.Size = new System.Drawing.Size(23, 22);
            this.buttonDOBJDelete.Text = "toolStripButton3";
            this.buttonDOBJDelete.Click += new System.EventHandler(this.buttonDOBJDelete_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.propertyGrid1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 185);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(767, 262);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // previewBox
            // 
            this.previewBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewBox.Location = new System.Drawing.Point(337, 25);
            this.previewBox.Margin = new System.Windows.Forms.Padding(5);
            this.previewBox.Name = "previewBox";
            this.previewBox.Size = new System.Drawing.Size(430, 157);
            this.previewBox.TabIndex = 4;
            this.previewBox.TabStop = false;
            this.previewBox.Text = "Preview";
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(337, 25);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 157);
            this.splitter2.TabIndex = 6;
            this.splitter2.TabStop = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton2,
            this.renderDropDown,
            this.toolStripLabel2,
            this.renderModeBox,
            this.toolStripLabel3,
            this.toolStripComboBox2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(767, 25);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importModelFromFileToolStripMenuItem,
            this.exportModelToFileToolStripMenuItem,
            this.toolStripSeparator2,
            this.importAnimationToolStripMenuItem,
            this.exportAnimationToolStripMenuItem,
            this.editAnimationToolStripMenuItem,
            this.clearAnimationToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(38, 22);
            this.toolStripDropDownButton2.Text = "File";
            // 
            // importModelFromFileToolStripMenuItem
            // 
            this.importModelFromFileToolStripMenuItem.Name = "importModelFromFileToolStripMenuItem";
            this.importModelFromFileToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.importModelFromFileToolStripMenuItem.Text = "Import Model From File";
            this.importModelFromFileToolStripMenuItem.Click += new System.EventHandler(this.importModelFromFileToolStripMenuItem_Click);
            // 
            // exportModelToFileToolStripMenuItem
            // 
            this.exportModelToFileToolStripMenuItem.Name = "exportModelToFileToolStripMenuItem";
            this.exportModelToFileToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.exportModelToFileToolStripMenuItem.Text = "Export Model To File";
            this.exportModelToFileToolStripMenuItem.Click += new System.EventHandler(this.exportModelToFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(196, 6);
            // 
            // importAnimationToolStripMenuItem
            // 
            this.importAnimationToolStripMenuItem.Name = "importAnimationToolStripMenuItem";
            this.importAnimationToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.importAnimationToolStripMenuItem.Text = "Import Animation";
            this.importAnimationToolStripMenuItem.Click += new System.EventHandler(this.importFromFileToolStripMenuItem_Click);
            // 
            // exportAnimationToolStripMenuItem
            // 
            this.exportAnimationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.asMayaANIMToolStripMenuItem,
            this.asFigatreeToolStripMenuItem,
            this.asAnimJointToolStripMenuItem});
            this.exportAnimationToolStripMenuItem.Name = "exportAnimationToolStripMenuItem";
            this.exportAnimationToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.exportAnimationToolStripMenuItem.Text = "Export Animation";
            // 
            // asMayaANIMToolStripMenuItem
            // 
            this.asMayaANIMToolStripMenuItem.Name = "asMayaANIMToolStripMenuItem";
            this.asMayaANIMToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.asMayaANIMToolStripMenuItem.Text = "As Maya ANIM";
            this.asMayaANIMToolStripMenuItem.Click += new System.EventHandler(this.mayaANIMToolStripMenuItem_Click);
            // 
            // asFigatreeToolStripMenuItem
            // 
            this.asFigatreeToolStripMenuItem.Name = "asFigatreeToolStripMenuItem";
            this.asFigatreeToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.asFigatreeToolStripMenuItem.Text = "As Figatree";
            this.asFigatreeToolStripMenuItem.Click += new System.EventHandler(this.figaTreeToolStripMenuItem_Click);
            // 
            // asAnimJointToolStripMenuItem
            // 
            this.asAnimJointToolStripMenuItem.Name = "asAnimJointToolStripMenuItem";
            this.asAnimJointToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.asAnimJointToolStripMenuItem.Text = "As AnimJoint";
            this.asAnimJointToolStripMenuItem.Click += new System.EventHandler(this.animJointToolStripMenuItem_Click);
            // 
            // editAnimationToolStripMenuItem
            // 
            this.editAnimationToolStripMenuItem.Name = "editAnimationToolStripMenuItem";
            this.editAnimationToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.editAnimationToolStripMenuItem.Text = "Edit Animation";
            this.editAnimationToolStripMenuItem.Click += new System.EventHandler(this.editAnimationToolStripMenuItem_Click);
            // 
            // clearAnimationToolStripMenuItem
            // 
            this.clearAnimationToolStripMenuItem.Name = "clearAnimationToolStripMenuItem";
            this.clearAnimationToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.clearAnimationToolStripMenuItem.Text = "Clear Animation";
            this.clearAnimationToolStripMenuItem.Click += new System.EventHandler(this.clearAnimButton_Click);
            // 
            // renderDropDown
            // 
            this.renderDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.renderDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showInViewportToolStripMenuItem,
            this.showBonesToolStrip,
            this.showSelectionOutlineToolStripMenuItem});
            this.renderDropDown.Image = ((System.Drawing.Image)(resources.GetObject("renderDropDown.Image")));
            this.renderDropDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.renderDropDown.Name = "renderDropDown";
            this.renderDropDown.Size = new System.Drawing.Size(45, 22);
            this.renderDropDown.Text = "View";
            // 
            // showInViewportToolStripMenuItem
            // 
            this.showInViewportToolStripMenuItem.CheckOnClick = true;
            this.showInViewportToolStripMenuItem.Name = "showInViewportToolStripMenuItem";
            this.showInViewportToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showInViewportToolStripMenuItem.Text = "Show in Viewport";
            this.showInViewportToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.mainRender_CheckStateChanged);
            // 
            // showBonesToolStrip
            // 
            this.showBonesToolStrip.Checked = true;
            this.showBonesToolStrip.CheckOnClick = true;
            this.showBonesToolStrip.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showBonesToolStrip.Name = "showBonesToolStrip";
            this.showBonesToolStrip.Size = new System.Drawing.Size(196, 22);
            this.showBonesToolStrip.Text = "Show Bones";
            this.showBonesToolStrip.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // showSelectionOutlineToolStripMenuItem
            // 
            this.showSelectionOutlineToolStripMenuItem.Checked = true;
            this.showSelectionOutlineToolStripMenuItem.CheckOnClick = true;
            this.showSelectionOutlineToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showSelectionOutlineToolStripMenuItem.Name = "showSelectionOutlineToolStripMenuItem";
            this.showSelectionOutlineToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showSelectionOutlineToolStripMenuItem.Text = "Show Selection Outline";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(81, 22);
            this.toolStripLabel2.Text = "Render Mode:";
            // 
            // renderModeBox
            // 
            this.renderModeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.renderModeBox.Name = "renderModeBox";
            this.renderModeBox.Size = new System.Drawing.Size(121, 25);
            this.renderModeBox.SelectedIndexChanged += new System.EventHandler(this.renderModeBox_SelectedIndexChanged);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(72, 22);
            this.toolStripLabel3.Text = "DOBJ Mode:";
            // 
            // toolStripComboBox2
            // 
            this.toolStripComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox2.Items.AddRange(new object[] {
            "All",
            "Selected",
            "None"});
            this.toolStripComboBox2.Name = "toolStripComboBox2";
            this.toolStripComboBox2.Size = new System.Drawing.Size(121, 25);
            this.toolStripComboBox2.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 182);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(767, 3);
            this.splitter1.TabIndex = 8;
            this.splitter1.TabStop = false;
            // 
            // importBoneINIToolStripMenuItem
            // 
            this.importBoneINIToolStripMenuItem.Name = "importBoneINIToolStripMenuItem";
            this.importBoneINIToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.importBoneINIToolStripMenuItem.Text = "Import Bone Label INI";
            this.importBoneINIToolStripMenuItem.Click += new System.EventHandler(this.importBoneLabelINIToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(203, 6);
            // 
            // meleeToolStripMenuItem
            // 
            this.meleeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reorientSkeletonForFighterToolStripMenuItem});
            this.meleeToolStripMenuItem.Name = "meleeToolStripMenuItem";
            this.meleeToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.meleeToolStripMenuItem.Text = "Melee";
            // 
            // reorientSkeletonForFighterToolStripMenuItem
            // 
            this.reorientSkeletonForFighterToolStripMenuItem.Name = "reorientSkeletonForFighterToolStripMenuItem";
            this.reorientSkeletonForFighterToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.reorientSkeletonForFighterToolStripMenuItem.Text = "Reorient Skeleton for Fighter";
            this.reorientSkeletonForFighterToolStripMenuItem.Click += new System.EventHandler(this.reorientSkeletonForFighterToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(203, 6);
            // 
            // JOBJEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(767, 447);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.previewBox);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.groupBox1);
            this.Name = "JOBJEditor";
            this.TabText = "JOBJEditor";
            this.Text = "JOBJEditor";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox listDOBJ;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView treeJOBJ;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.GroupBox previewBox;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem importModelFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportModelToFileToolStripMenuItem;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton3;
        private System.Windows.Forms.ToolStripMenuItem addDummyDOBJToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton buttonMoveUp;
        private System.Windows.Forms.ToolStripButton buttonMoveDown;
        private System.Windows.Forms.ToolStripMenuItem createOutlineMeshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearAllPOBJsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton buttonDOBJDelete;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox renderModeBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem importAnimationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAnimationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asMayaANIMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asFigatreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asAnimJointToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox2;
        private System.Windows.Forms.ToolStripDropDownButton renderDropDown;
        private System.Windows.Forms.ToolStripMenuItem showBonesToolStrip;
        private System.Windows.Forms.ToolStripMenuItem showInViewportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSelectionOutlineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearAnimationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editAnimationToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripDropDownButton jointOptionsDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem recalculateInverseBindsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceBonesFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton materialDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importBoneINIToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem meleeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reorientSkeletonForFighterToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}