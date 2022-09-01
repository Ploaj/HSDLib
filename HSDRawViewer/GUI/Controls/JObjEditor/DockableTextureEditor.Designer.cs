namespace HSDRawViewer.GUI.Controls.JObjEditor
{
    partial class DockableTextureEditor
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonSaveTexture = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.buttonReplace = new System.Windows.Forms.ToolStripButton();
            this.textureArrayEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSaveTexture,
            this.toolStripButton1,
            this.buttonReplace});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(798, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonSaveTexture
            // 
            this.buttonSaveTexture.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.buttonSaveTexture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSaveTexture.Name = "buttonSaveTexture";
            this.buttonSaveTexture.Size = new System.Drawing.Size(61, 22);
            this.buttonSaveTexture.Text = "Export";
            this.buttonSaveTexture.Click += new System.EventHandler(this.buttonSaveTexture_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::HSDRawViewer.Properties.Resources.ts_add;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(63, 22);
            this.toolStripButton1.Text = "Import";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // buttonReplace
            // 
            this.buttonReplace.Image = global::HSDRawViewer.Properties.Resources.ico_replace;
            this.buttonReplace.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonReplace.Name = "buttonReplace";
            this.buttonReplace.Size = new System.Drawing.Size(68, 22);
            this.buttonReplace.Text = "Replace";
            this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
            // 
            // textureArrayEditor
            // 
            this.textureArrayEditor.CanAdd = false;
            this.textureArrayEditor.CanClone = false;
            this.textureArrayEditor.DisplayItemImages = true;
            this.textureArrayEditor.DisplayItemIndices = true;
            this.textureArrayEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textureArrayEditor.EnablePropertyView = false;
            this.textureArrayEditor.EnablePropertyViewDescription = true;
            this.textureArrayEditor.ImageHeight = ((ushort)(64));
            this.textureArrayEditor.ImageWidth = ((ushort)(64));
            this.textureArrayEditor.InsertCloneAfterSelected = false;
            this.textureArrayEditor.ItemHeight = 64;
            this.textureArrayEditor.ItemIndexOffset = 0;
            this.textureArrayEditor.Location = new System.Drawing.Point(0, 25);
            this.textureArrayEditor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textureArrayEditor.Name = "textureArrayEditor";
            this.textureArrayEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.textureArrayEditor.Size = new System.Drawing.Size(798, 336);
            this.textureArrayEditor.TabIndex = 7;
            this.textureArrayEditor.SelectedObjectChanged += new System.EventHandler(this.textureArrayEditor_SelectedObjectChanged);
            this.textureArrayEditor.ArrayUpdated += new System.EventHandler(this.textureArrayEditor_ArrayUpdated);
            // 
            // DockableTextureEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(798, 361);
            this.Controls.Add(this.textureArrayEditor);
            this.Controls.Add(this.toolStrip1);
            this.Name = "DockableTextureEditor";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonSaveTexture;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton buttonReplace;
        private ArrayMemberEditor textureArrayEditor;
    }
}
