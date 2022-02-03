using HSDRawViewer.GUI.Controls;

namespace HSDRawViewer.GUI.Plugins
{
    partial class JobjEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JobjEditor));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.treeJOBJ = new System.Windows.Forms.TreeView();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.jointOptionsDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.importBoneINIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportBoneLabelINIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.autoUpdateFlagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recalculateInverseBindsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeParticleJointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceBonesFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listDOBJ = new HSDRawViewer.GUI.Controls.MeshList();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.createOutlineMeshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addDummyDOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.clearSelectedPOBJsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearAllPOBJsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.materialDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonMoveUp = new System.Windows.Forms.ToolStripButton();
            this.buttonMoveDown = new System.Windows.Forms.ToolStripButton();
            this.buttonDOBJDelete = new System.Windows.Forms.ToolStripButton();
            this.buttonShowAll = new System.Windows.Forms.ToolStripButton();
            this.buttonHidePoly = new System.Windows.Forms.ToolStripButton();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.textureArrayEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.replaceTextureButton = new System.Windows.Forms.ToolStripButton();
            this.previewBox = new System.Windows.Forms.GroupBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.importModelFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportModelToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.importSceneSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSceneSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.importToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.trimToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.fSMApplyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.displaySettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fogSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.showBonesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showBoneOrientationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showMeshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSelectionOutlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSplinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.showInViewportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.renderModeBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox2 = new System.Windows.Forms.ToolStripComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.exporttoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.toolStrip4.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 16);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(312, 248);
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
            this.tabPage1.Size = new System.Drawing.Size(304, 222);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Joints";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // treeJOBJ
            // 
            this.treeJOBJ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeJOBJ.HideSelection = false;
            this.treeJOBJ.ItemHeight = 20;
            this.treeJOBJ.Location = new System.Drawing.Point(3, 28);
            this.treeJOBJ.Name = "treeJOBJ";
            this.treeJOBJ.Size = new System.Drawing.Size(298, 191);
            this.treeJOBJ.TabIndex = 0;
            this.treeJOBJ.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.treeJOBJ_KeyPress);
            // 
            // toolStrip3
            // 
            this.toolStrip3.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jointOptionsDropDownButton1});
            this.toolStrip3.Location = new System.Drawing.Point(3, 3);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(298, 25);
            this.toolStrip3.TabIndex = 1;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // jointOptionsDropDownButton1
            // 
            this.jointOptionsDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.jointOptionsDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importBoneINIToolStripMenuItem,
            this.exportBoneLabelINIToolStripMenuItem,
            this.toolStripSeparator1,
            this.autoUpdateFlagsToolStripMenuItem,
            this.recalculateInverseBindsToolStripMenuItem,
            this.makeParticleJointToolStripMenuItem,
            this.replaceBonesFromFileToolStripMenuItem});
            this.jointOptionsDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("jointOptionsDropDownButton1.Image")));
            this.jointOptionsDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.jointOptionsDropDownButton1.Name = "jointOptionsDropDownButton1";
            this.jointOptionsDropDownButton1.Size = new System.Drawing.Size(90, 22);
            this.jointOptionsDropDownButton1.Text = "Joint Options";
            // 
            // importBoneINIToolStripMenuItem
            // 
            this.importBoneINIToolStripMenuItem.Name = "importBoneINIToolStripMenuItem";
            this.importBoneINIToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.importBoneINIToolStripMenuItem.Text = "Import Bone Label INI";
            this.importBoneINIToolStripMenuItem.Click += new System.EventHandler(this.importBoneLabelINIToolStripMenuItem_Click);
            // 
            // exportBoneLabelINIToolStripMenuItem
            // 
            this.exportBoneLabelINIToolStripMenuItem.Name = "exportBoneLabelINIToolStripMenuItem";
            this.exportBoneLabelINIToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.exportBoneLabelINIToolStripMenuItem.Text = "Export Bone Label INI";
            this.exportBoneLabelINIToolStripMenuItem.Click += new System.EventHandler(this.exportBoneLabelINIToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(203, 6);
            // 
            // autoUpdateFlagsToolStripMenuItem
            // 
            this.autoUpdateFlagsToolStripMenuItem.Name = "autoUpdateFlagsToolStripMenuItem";
            this.autoUpdateFlagsToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.autoUpdateFlagsToolStripMenuItem.Text = "Update Flags";
            this.autoUpdateFlagsToolStripMenuItem.Click += new System.EventHandler(this.autoUpdateFlagsToolStripMenuItem_Click);
            // 
            // recalculateInverseBindsToolStripMenuItem
            // 
            this.recalculateInverseBindsToolStripMenuItem.Name = "recalculateInverseBindsToolStripMenuItem";
            this.recalculateInverseBindsToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.recalculateInverseBindsToolStripMenuItem.Text = "Recalculate Inverse Binds";
            this.recalculateInverseBindsToolStripMenuItem.Click += new System.EventHandler(this.recalculateInverseBindsToolStripMenuItem_Click);
            // 
            // makeParticleJointToolStripMenuItem
            // 
            this.makeParticleJointToolStripMenuItem.Name = "makeParticleJointToolStripMenuItem";
            this.makeParticleJointToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.makeParticleJointToolStripMenuItem.Text = "Make Particle Joint";
            this.makeParticleJointToolStripMenuItem.Click += new System.EventHandler(this.makeParticleJointToolStripMenuItem_Click);
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
            this.tabPage2.Size = new System.Drawing.Size(304, 222);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Objects";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listDOBJ
            // 
            this.listDOBJ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listDOBJ.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listDOBJ.ItemHeight = 16;
            this.listDOBJ.Location = new System.Drawing.Point(3, 30);
            this.listDOBJ.Name = "listDOBJ";
            this.listDOBJ.Size = new System.Drawing.Size(298, 189);
            this.listDOBJ.TabIndex = 0;
            this.listDOBJ.BindingContextChanged += new System.EventHandler(this.listDOBJ_BindingContextChanged);
            this.listDOBJ.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.listDOBJ_KeyPress);
            // 
            // toolStrip2
            // 
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton3,
            this.materialDropDownButton1,
            this.buttonMoveUp,
            this.buttonMoveDown,
            this.buttonDOBJDelete,
            this.buttonShowAll,
            this.buttonHidePoly});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(298, 27);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripDropDownButton3
            // 
            this.toolStripDropDownButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createOutlineMeshToolStripMenuItem,
            this.addDummyDOBJToolStripMenuItem,
            this.toolStripSeparator5,
            this.clearSelectedPOBJsToolStripMenuItem,
            this.clearAllPOBJsToolStripMenuItem,
            this.toolStripSeparator6});
            this.toolStripDropDownButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton3.Image")));
            this.toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            this.toolStripDropDownButton3.Size = new System.Drawing.Size(40, 24);
            this.toolStripDropDownButton3.Text = "Edit";
            // 
            // createOutlineMeshToolStripMenuItem
            // 
            this.createOutlineMeshToolStripMenuItem.Name = "createOutlineMeshToolStripMenuItem";
            this.createOutlineMeshToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.createOutlineMeshToolStripMenuItem.Text = "Create Outline Mesh";
            this.createOutlineMeshToolStripMenuItem.Click += new System.EventHandler(this.createOutlineMeshToolStripMenuItem_Click);
            // 
            // addDummyDOBJToolStripMenuItem
            // 
            this.addDummyDOBJToolStripMenuItem.Name = "addDummyDOBJToolStripMenuItem";
            this.addDummyDOBJToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.addDummyDOBJToolStripMenuItem.Text = "Add Dummy DOBJ";
            this.addDummyDOBJToolStripMenuItem.Click += new System.EventHandler(this.addDummyDOBJToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(180, 6);
            // 
            // clearSelectedPOBJsToolStripMenuItem
            // 
            this.clearSelectedPOBJsToolStripMenuItem.Name = "clearSelectedPOBJsToolStripMenuItem";
            this.clearSelectedPOBJsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.clearSelectedPOBJsToolStripMenuItem.Text = "Clear Selected POBJs";
            this.clearSelectedPOBJsToolStripMenuItem.Click += new System.EventHandler(this.clearSelectedPOBJsToolStripMenuItem_Click);
            // 
            // clearAllPOBJsToolStripMenuItem
            // 
            this.clearAllPOBJsToolStripMenuItem.Name = "clearAllPOBJsToolStripMenuItem";
            this.clearAllPOBJsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.clearAllPOBJsToolStripMenuItem.Text = "Clear All POBJs";
            this.clearAllPOBJsToolStripMenuItem.Click += new System.EventHandler(this.clearAllPOBJsToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(180, 6);
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
            this.materialDropDownButton1.Size = new System.Drawing.Size(83, 24);
            this.materialDropDownButton1.Text = "Material";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(184, 26);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Image = global::HSDRawViewer.Properties.Resources.ts_importfile;
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(184, 26);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Image = global::HSDRawViewer.Properties.Resources.ts_exportfile;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(184, 26);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonMoveUp.Image = global::HSDRawViewer.Properties.Resources.ts_up;
            this.buttonMoveUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(24, 24);
            this.buttonMoveUp.Text = "Move Up";
            this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonMoveDown.Image = global::HSDRawViewer.Properties.Resources.ts_down;
            this.buttonMoveDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(24, 24);
            this.buttonMoveDown.Text = "Move Down";
            this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
            // 
            // buttonDOBJDelete
            // 
            this.buttonDOBJDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDOBJDelete.Image = global::HSDRawViewer.Properties.Resources.ts_subtract;
            this.buttonDOBJDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDOBJDelete.Name = "buttonDOBJDelete";
            this.buttonDOBJDelete.Size = new System.Drawing.Size(24, 24);
            this.buttonDOBJDelete.Text = "Delete Polygon";
            this.buttonDOBJDelete.Click += new System.EventHandler(this.buttonDOBJDelete_Click);
            // 
            // buttonShowAll
            // 
            this.buttonShowAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonShowAll.Image = global::HSDRawViewer.Properties.Resources.ts_visible;
            this.buttonShowAll.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.buttonShowAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonShowAll.Name = "buttonShowAll";
            this.buttonShowAll.Size = new System.Drawing.Size(23, 24);
            this.buttonShowAll.Text = "Show All";
            this.buttonShowAll.Click += new System.EventHandler(this.buttonShowAll_Click);
            // 
            // buttonHidePoly
            // 
            this.buttonHidePoly.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonHidePoly.Image = global::HSDRawViewer.Properties.Resources.ts_hidden;
            this.buttonHidePoly.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.buttonHidePoly.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonHidePoly.Name = "buttonHidePoly";
            this.buttonHidePoly.Size = new System.Drawing.Size(23, 24);
            this.buttonHidePoly.Text = "Hide All";
            this.buttonHidePoly.Click += new System.EventHandler(this.buttonHidePoly_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.textureArrayEditor);
            this.tabPage3.Controls.Add(this.toolStrip4);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(304, 222);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Textures";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // textureArrayEditor
            // 
            this.textureArrayEditor.AllowDrop = true;
            this.textureArrayEditor.DisplayItemImages = true;
            this.textureArrayEditor.DisplayItemIndices = true;
            this.textureArrayEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textureArrayEditor.EnablePropertyView = false;
            this.textureArrayEditor.EnablePropertyViewDescription = false;
            this.textureArrayEditor.EnableToolStrip = false;
            this.textureArrayEditor.ImageHeight = ((ushort)(64));
            this.textureArrayEditor.ImageWidth = ((ushort)(64));
            this.textureArrayEditor.ItemHeight = 64;
            this.textureArrayEditor.ItemIndexOffset = 0;
            this.textureArrayEditor.Location = new System.Drawing.Point(0, 27);
            this.textureArrayEditor.Margin = new System.Windows.Forms.Padding(4);
            this.textureArrayEditor.Name = "textureArrayEditor";
            this.textureArrayEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.textureArrayEditor.Size = new System.Drawing.Size(304, 195);
            this.textureArrayEditor.TabIndex = 0;
            this.textureArrayEditor.SelectedObjectChanged += new System.EventHandler(this.textureArrayEditor_SelectedObjectChanged);
            this.textureArrayEditor.DragDrop += new System.Windows.Forms.DragEventHandler(this.textureArrayEditor_DragDrop);
            this.textureArrayEditor.DragEnter += new System.Windows.Forms.DragEventHandler(this.textureArrayEditor_DragEnter);
            // 
            // toolStrip4
            // 
            this.toolStrip4.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exporttoolStripButton,
            this.replaceTextureButton});
            this.toolStrip4.Location = new System.Drawing.Point(0, 0);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.Size = new System.Drawing.Size(304, 27);
            this.toolStrip4.TabIndex = 1;
            this.toolStrip4.Text = "toolStrip4";
            // 
            // replaceTextureButton
            // 
            this.replaceTextureButton.Image = global::HSDRawViewer.Properties.Resources.ico_replace;
            this.replaceTextureButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.replaceTextureButton.Name = "replaceTextureButton";
            this.replaceTextureButton.Size = new System.Drawing.Size(113, 24);
            this.replaceTextureButton.Text = "Replace Texture";
            this.replaceTextureButton.Click += new System.EventHandler(this.replaceTextureButton_Click);
            // 
            // previewBox
            // 
            this.previewBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewBox.Location = new System.Drawing.Point(318, 25);
            this.previewBox.Margin = new System.Windows.Forms.Padding(5);
            this.previewBox.Name = "previewBox";
            this.previewBox.Size = new System.Drawing.Size(503, 477);
            this.previewBox.TabIndex = 4;
            this.previewBox.TabStop = false;
            this.previewBox.Text = "Preview";
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton2,
            this.toolStripDropDownButton1,
            this.renderDropDown,
            this.toolStripLabel2,
            this.renderModeBox,
            this.toolStripLabel3,
            this.toolStripComboBox2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(821, 25);
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
            this.importSceneSettingsToolStripMenuItem,
            this.exportSceneSettingsToolStripMenuItem});
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
            // importSceneSettingsToolStripMenuItem
            // 
            this.importSceneSettingsToolStripMenuItem.Name = "importSceneSettingsToolStripMenuItem";
            this.importSceneSettingsToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.importSceneSettingsToolStripMenuItem.Text = "Import Scene Settings";
            this.importSceneSettingsToolStripMenuItem.Click += new System.EventHandler(this.importSceneSettingsToolStripMenuItem_Click);
            // 
            // exportSceneSettingsToolStripMenuItem
            // 
            this.exportSceneSettingsToolStripMenuItem.Name = "exportSceneSettingsToolStripMenuItem";
            this.exportSceneSettingsToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.exportSceneSettingsToolStripMenuItem.Text = "Export Scene Settings";
            this.exportSceneSettingsToolStripMenuItem.Click += new System.EventHandler(this.exportSceneSettingsToolStripMenuItem_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem1,
            this.toolStripMenuItem1,
            this.toolStripSeparator9,
            this.createToolStripMenuItem,
            this.editToolStripMenuItem1,
            this.trimToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.toolStripSeparator7,
            this.fSMApplyToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(76, 22);
            this.toolStripDropDownButton1.Text = "Animation";
            // 
            // importToolStripMenuItem1
            // 
            this.importToolStripMenuItem1.Name = "importToolStripMenuItem1";
            this.importToolStripMenuItem1.Size = new System.Drawing.Size(131, 22);
            this.importToolStripMenuItem1.Text = "Import";
            this.importToolStripMenuItem1.Click += new System.EventHandler(this.importFromFileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(131, 22);
            this.toolStripMenuItem1.Text = "Export";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItem2.Text = "As Maya ANIM";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.mayaANIMToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItem3.Text = "As Figatree";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.figaTreeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItem4.Text = "As AnimJoint";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.animJointToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItem5.Text = "As MOT";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.motToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItem6.Text = "As XML";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.xmlToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(128, 6);
            // 
            // createToolStripMenuItem
            // 
            this.createToolStripMenuItem.Name = "createToolStripMenuItem";
            this.createToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.createToolStripMenuItem.Text = "Create";
            this.createToolStripMenuItem.Click += new System.EventHandler(this.createAnimationToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem1
            // 
            this.editToolStripMenuItem1.Name = "editToolStripMenuItem1";
            this.editToolStripMenuItem1.Size = new System.Drawing.Size(131, 22);
            this.editToolStripMenuItem1.Text = "Edit";
            this.editToolStripMenuItem1.Click += new System.EventHandler(this.viewAnimationGraphToolStripMenuItem_Click);
            // 
            // trimToolStripMenuItem
            // 
            this.trimToolStripMenuItem.Name = "trimToolStripMenuItem";
            this.trimToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.trimToolStripMenuItem.Text = "Trim";
            this.trimToolStripMenuItem.Click += new System.EventHandler(this.editAnimationToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearAnimButton_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(128, 6);
            // 
            // fSMApplyToolStripMenuItem
            // 
            this.fSMApplyToolStripMenuItem.Name = "fSMApplyToolStripMenuItem";
            this.fSMApplyToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.fSMApplyToolStripMenuItem.Text = "FSM Apply";
            this.fSMApplyToolStripMenuItem.Click += new System.EventHandler(this.fSMApplyToolStripMenuItem_Click);
            // 
            // renderDropDown
            // 
            this.renderDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.renderDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displaySettingsToolStripMenuItem,
            this.fogSettingsToolStripMenuItem,
            this.toolStripSeparator4,
            this.showBonesToolStripMenuItem,
            this.showBoneOrientationToolStripMenuItem,
            this.showMeshToolStripMenuItem,
            this.showSelectionOutlineToolStripMenuItem,
            this.showSplinesToolStripMenuItem,
            this.toolStripSeparator3,
            this.showInViewportToolStripMenuItem});
            this.renderDropDown.Image = ((System.Drawing.Image)(resources.GetObject("renderDropDown.Image")));
            this.renderDropDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.renderDropDown.Name = "renderDropDown";
            this.renderDropDown.Size = new System.Drawing.Size(74, 22);
            this.renderDropDown.Text = "Rendering";
            // 
            // displaySettingsToolStripMenuItem
            // 
            this.displaySettingsToolStripMenuItem.Name = "displaySettingsToolStripMenuItem";
            this.displaySettingsToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.displaySettingsToolStripMenuItem.Text = "Light Settings";
            this.displaySettingsToolStripMenuItem.Click += new System.EventHandler(this.displaySettingsToolStripMenuItem_Click);
            // 
            // fogSettingsToolStripMenuItem
            // 
            this.fogSettingsToolStripMenuItem.Name = "fogSettingsToolStripMenuItem";
            this.fogSettingsToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.fogSettingsToolStripMenuItem.Text = "Fog Settings";
            this.fogSettingsToolStripMenuItem.Click += new System.EventHandler(this.fogSettingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(193, 6);
            // 
            // showBonesToolStripMenuItem
            // 
            this.showBonesToolStripMenuItem.CheckOnClick = true;
            this.showBonesToolStripMenuItem.Name = "showBonesToolStripMenuItem";
            this.showBonesToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showBonesToolStripMenuItem.Text = "Show Bones";
            // 
            // showBoneOrientationToolStripMenuItem
            // 
            this.showBoneOrientationToolStripMenuItem.CheckOnClick = true;
            this.showBoneOrientationToolStripMenuItem.Name = "showBoneOrientationToolStripMenuItem";
            this.showBoneOrientationToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showBoneOrientationToolStripMenuItem.Text = "Show Bone Orientation";
            // 
            // showMeshToolStripMenuItem
            // 
            this.showMeshToolStripMenuItem.CheckOnClick = true;
            this.showMeshToolStripMenuItem.Name = "showMeshToolStripMenuItem";
            this.showMeshToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showMeshToolStripMenuItem.Text = "Show Mesh";
            // 
            // showSelectionOutlineToolStripMenuItem
            // 
            this.showSelectionOutlineToolStripMenuItem.Checked = true;
            this.showSelectionOutlineToolStripMenuItem.CheckOnClick = true;
            this.showSelectionOutlineToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showSelectionOutlineToolStripMenuItem.Name = "showSelectionOutlineToolStripMenuItem";
            this.showSelectionOutlineToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showSelectionOutlineToolStripMenuItem.Text = "Show Mesh Selection";
            // 
            // showSplinesToolStripMenuItem
            // 
            this.showSplinesToolStripMenuItem.CheckOnClick = true;
            this.showSplinesToolStripMenuItem.Name = "showSplinesToolStripMenuItem";
            this.showSplinesToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showSplinesToolStripMenuItem.Text = "Show Splines";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(193, 6);
            // 
            // showInViewportToolStripMenuItem
            // 
            this.showInViewportToolStripMenuItem.CheckOnClick = true;
            this.showInViewportToolStripMenuItem.Name = "showInViewportToolStripMenuItem";
            this.showInViewportToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showInViewportToolStripMenuItem.Text = "Show in Viewport";
            this.showInViewportToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.mainRender_CheckStateChanged);
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
            this.toolStripLabel3.Size = new System.Drawing.Size(107, 22);
            this.toolStripLabel3.Text = "Object View Mode:";
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.splitter2);
            this.groupBox2.Controls.Add(this.tabControl1);
            this.groupBox2.Controls.Add(this.groupBox1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox2.Location = new System.Drawing.Point(0, 25);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(318, 477);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Nodes";
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(3, 261);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(312, 3);
            this.splitter2.TabIndex = 4;
            this.splitter2.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.propertyGrid1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(3, 264);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(312, 210);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameters";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.HelpVisible = false;
            this.propertyGrid1.Location = new System.Drawing.Point(11, 19);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid1.Size = new System.Drawing.Size(292, 185);
            this.propertyGrid1.TabIndex = 1;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(318, 25);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 477);
            this.splitter1.TabIndex = 8;
            this.splitter1.TabStop = false;
            // 
            // exporttoolStripButton
            // 
            this.exporttoolStripButton.Image = global::HSDRawViewer.Properties.Resources.ts_exportfile;
            this.exporttoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exporttoolStripButton.Name = "exporttoolStripButton";
            this.exporttoolStripButton.Size = new System.Drawing.Size(106, 24);
            this.exporttoolStripButton.Text = "Export Texture";
            this.exporttoolStripButton.Click += new System.EventHandler(this.exporttoolStripButton_Click);
            // 
            // JobjEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.previewBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.toolStrip1);
            this.Name = "JobjEditor";
            this.Size = new System.Drawing.Size(821, 502);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MeshList listDOBJ;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TreeView treeJOBJ;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.GroupBox previewBox;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem importModelFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportModelToFileToolStripMenuItem;
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
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox2;
        private System.Windows.Forms.ToolStripDropDownButton renderDropDown;
        private System.Windows.Forms.ToolStripMenuItem showInViewportToolStripMenuItem;
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
        private System.Windows.Forms.ToolStripMenuItem exportBoneLabelINIToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripMenuItem autoUpdateFlagsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displaySettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSelectionOutlineToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem clearSelectedPOBJsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem importSceneSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportSceneSettingsToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage3;
        private ArrayMemberEditor textureArrayEditor;
        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.ToolStripButton replaceTextureButton;
        private System.Windows.Forms.ToolStripMenuItem showBonesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showBoneOrientationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showMeshToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem fogSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem trimToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem fSMApplyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeParticleJointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSplinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton buttonShowAll;
        private System.Windows.Forms.ToolStripButton buttonHidePoly;
        private System.Windows.Forms.ToolStripButton exporttoolStripButton;
    }
}