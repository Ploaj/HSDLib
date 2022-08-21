namespace HSDRawViewer.GUI.Controls.JObjEditor
{
    partial class DockableMeshList
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockableMeshList));
            this.listDOBJ = new HSDRawViewer.GUI.Controls.JObjEditor.MeshList();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.materialDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonMoveUp = new System.Windows.Forms.ToolStripButton();
            this.buttonMoveDown = new System.Windows.Forms.ToolStripButton();
            this.buttonDOBJDelete = new System.Windows.Forms.ToolStripButton();
            this.buttonShowAll = new System.Windows.Forms.ToolStripButton();
            this.buttonHidePoly = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.massTextureEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addDummyDOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearSelectedPOBJsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listDOBJ
            // 
            this.listDOBJ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listDOBJ.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listDOBJ.ItemHeight = 16;
            this.listDOBJ.Location = new System.Drawing.Point(0, 27);
            this.listDOBJ.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listDOBJ.Name = "listDOBJ";
            this.listDOBJ.Size = new System.Drawing.Size(475, 406);
            this.listDOBJ.TabIndex = 2;
            this.listDOBJ.ItemVisiblilityChanged += new System.EventHandler(this.listDOBJ_ItemVisiblilityChanged);
            this.listDOBJ.SelectedIndexChanged += new System.EventHandler(this.listDOBJ_SelectedIndexChanged);
            // 
            // toolStrip2
            // 
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.materialDropDownButton1,
            this.buttonMoveUp,
            this.buttonMoveDown,
            this.buttonDOBJDelete,
            this.buttonShowAll,
            this.buttonHidePoly,
            this.toolStripDropDownButton3});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(475, 27);
            this.toolStrip2.TabIndex = 3;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // materialDropDownButton1
            // 
            this.materialDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.materialDropDownButton1.Image = global::HSDRawViewer.Properties.Resources.ico_mobj;
            this.materialDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.materialDropDownButton1.Name = "materialDropDownButton1";
            this.materialDropDownButton1.Size = new System.Drawing.Size(83, 24);
            this.materialDropDownButton1.Text = "Material";
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
            // toolStripDropDownButton3
            // 
            this.toolStripDropDownButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.massTextureEditorToolStripMenuItem,
            this.addDummyDOBJToolStripMenuItem,
            this.clearSelectedPOBJsToolStripMenuItem});
            this.toolStripDropDownButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton3.Image")));
            this.toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            this.toolStripDropDownButton3.Size = new System.Drawing.Size(47, 24);
            this.toolStripDropDownButton3.Text = "Tools";
            // 
            // massTextureEditorToolStripMenuItem
            // 
            this.massTextureEditorToolStripMenuItem.Name = "massTextureEditorToolStripMenuItem";
            this.massTextureEditorToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.massTextureEditorToolStripMenuItem.Text = "Mass Texture Editor";
            this.massTextureEditorToolStripMenuItem.Click += new System.EventHandler(this.massTextureEditorToolStripMenuItem_Click);
            // 
            // addDummyDOBJToolStripMenuItem
            // 
            this.addDummyDOBJToolStripMenuItem.Name = "addDummyDOBJToolStripMenuItem";
            this.addDummyDOBJToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.addDummyDOBJToolStripMenuItem.Text = "Generate Dummy Objects";
            this.addDummyDOBJToolStripMenuItem.Click += new System.EventHandler(this.addDummyDOBJToolStripMenuItem_Click);
            // 
            // clearSelectedPOBJsToolStripMenuItem
            // 
            this.clearSelectedPOBJsToolStripMenuItem.Name = "clearSelectedPOBJsToolStripMenuItem";
            this.clearSelectedPOBJsToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.clearSelectedPOBJsToolStripMenuItem.Text = "Delete Selected Polygons";
            this.clearSelectedPOBJsToolStripMenuItem.Click += new System.EventHandler(this.clearSelectedPOBJsToolStripMenuItem_Click);
            // 
            // DockableMeshList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 433);
            this.Controls.Add(this.listDOBJ);
            this.Controls.Add(this.toolStrip2);
            this.Name = "DockableMeshList";
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MeshList listDOBJ;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton3;
        private System.Windows.Forms.ToolStripMenuItem addDummyDOBJToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearSelectedPOBJsToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton materialDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton buttonMoveUp;
        private System.Windows.Forms.ToolStripButton buttonMoveDown;
        private System.Windows.Forms.ToolStripButton buttonDOBJDelete;
        private System.Windows.Forms.ToolStripButton buttonShowAll;
        private System.Windows.Forms.ToolStripButton buttonHidePoly;
        private System.Windows.Forms.ToolStripMenuItem massTextureEditorToolStripMenuItem;
    }
}
