namespace HSDRawViewer.GUI.MEX.Controls
{
    partial class MEXMenuSSSControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MEXMenuSSSControl));
            this.sssEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.editStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.regenerateAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateNameTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importNewIconImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadGameCameraButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // sssEditor
            // 
            this.sssEditor.DisplayItemIndices = true;
            this.sssEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sssEditor.EnablePropertyViewDescription = true;
            this.sssEditor.ItemIndexOffset = 0;
            this.sssEditor.Location = new System.Drawing.Point(0, 25);
            this.sssEditor.Name = "sssEditor";
            this.sssEditor.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.sssEditor.Size = new System.Drawing.Size(300, 375);
            this.sssEditor.TabIndex = 1;
            this.sssEditor.SelectedObjectChanged += new System.EventHandler(this.sssEditor_SelectedObjectChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editStripDropDownButton1,
            this.loadGameCameraButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(300, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // editStripDropDownButton1
            // 
            this.editStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.editStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.regenerateAnimationToolStripMenuItem,
            this.generateNameTagToolStripMenuItem,
            this.importNewIconImageToolStripMenuItem});
            this.editStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("editStripDropDownButton1.Image")));
            this.editStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editStripDropDownButton1.Name = "editStripDropDownButton1";
            this.editStripDropDownButton1.Size = new System.Drawing.Size(40, 22);
            this.editStripDropDownButton1.Text = "Edit";
            // 
            // regenerateAnimationToolStripMenuItem
            // 
            this.regenerateAnimationToolStripMenuItem.Name = "regenerateAnimationToolStripMenuItem";
            this.regenerateAnimationToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.regenerateAnimationToolStripMenuItem.Text = "Edit Animation";
            this.regenerateAnimationToolStripMenuItem.Click += new System.EventHandler(this.regenerateAnimationToolStripMenuItem_Click);
            // 
            // generateNameTagToolStripMenuItem
            // 
            this.generateNameTagToolStripMenuItem.Name = "generateNameTagToolStripMenuItem";
            this.generateNameTagToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.generateNameTagToolStripMenuItem.Text = "Generate Name Tag";
            this.generateNameTagToolStripMenuItem.Click += new System.EventHandler(this.generateNameTagToolStripMenuItem_Click);
            // 
            // importNewIconImageToolStripMenuItem
            // 
            this.importNewIconImageToolStripMenuItem.Name = "importNewIconImageToolStripMenuItem";
            this.importNewIconImageToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.importNewIconImageToolStripMenuItem.Text = "Import New Icon Image";
            this.importNewIconImageToolStripMenuItem.Click += new System.EventHandler(this.importNewIconImageToolStripMenuItem_Click);
            // 
            // loadGameCameraButton
            // 
            this.loadGameCameraButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.loadGameCameraButton.Image = ((System.Drawing.Image)(resources.GetObject("loadGameCameraButton.Image")));
            this.loadGameCameraButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loadGameCameraButton.Name = "loadGameCameraButton";
            this.loadGameCameraButton.Size = new System.Drawing.Size(115, 22);
            this.loadGameCameraButton.Text = "Load Game Camera";
            this.loadGameCameraButton.Click += new System.EventHandler(this.loadGameCameraButton_Click);
            // 
            // MEXMenuSSSControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sssEditor);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MEXMenuSSSControl";
            this.Size = new System.Drawing.Size(300, 400);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton editStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem regenerateAnimationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateNameTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importNewIconImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton loadGameCameraButton;
        public ArrayMemberEditor sssEditor;
    }
}
