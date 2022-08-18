
namespace HSDRawViewer.GUI.Plugins
{
    partial class TEXGEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TEXGEditor));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonExport = new System.Windows.Forms.ToolStripButton();
            this.buttonExportAll = new System.Windows.Forms.ToolStripButton();
            this.buttonImport = new System.Windows.Forms.ToolStripButton();
            this.buttonReplace = new System.Windows.Forms.ToolStripButton();
            this.texturePanel1 = new HSDRawViewer.GUI.Controls.TextureViewControl();
            this.arrayMemberEditor1 = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonImport,
            this.buttonReplace,
            this.buttonExport,
            this.buttonExportAll});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonExport
            // 
            this.buttonExport.Image = global::HSDRawViewer.Properties.Resources.ts_exportfile;
            this.buttonExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(61, 22);
            this.buttonExport.Text = "Export";
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // buttonExportAll
            // 
            this.buttonExportAll.Image = global::HSDRawViewer.Properties.Resources.ts_exportfile;
            this.buttonExportAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonExportAll.Name = "buttonExportAll";
            this.buttonExportAll.Size = new System.Drawing.Size(78, 22);
            this.buttonExportAll.Text = "Export All";
            this.buttonExportAll.Click += new System.EventHandler(this.buttonExportAll_Click);
            // 
            // buttonImport
            // 
            this.buttonImport.Image = global::HSDRawViewer.Properties.Resources.ts_importfile;
            this.buttonImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(63, 22);
            this.buttonImport.Text = "Import";
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
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
            // texturePanel1
            // 
            this.texturePanel1.AutoScroll = true;
            this.texturePanel1.BackColor = System.Drawing.Color.Transparent;
            this.texturePanel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("texturePanel1.BackgroundImage")));
            this.texturePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.texturePanel1.ForeColor = System.Drawing.Color.White;
            this.texturePanel1.Image = null;
            this.texturePanel1.Location = new System.Drawing.Point(297, 25);
            this.texturePanel1.Name = "texturePanel1";
            this.texturePanel1.Size = new System.Drawing.Size(503, 425);
            this.texturePanel1.TabIndex = 2;
            // 
            // arrayMemberEditor1
            // 
            this.arrayMemberEditor1.CanAdd = false;
            this.arrayMemberEditor1.DisplayItemImages = true;
            this.arrayMemberEditor1.DisplayItemIndices = true;
            this.arrayMemberEditor1.Dock = System.Windows.Forms.DockStyle.Left;
            this.arrayMemberEditor1.EnablePropertyView = false;
            this.arrayMemberEditor1.EnablePropertyViewDescription = false;
            this.arrayMemberEditor1.ImageHeight = ((ushort)(64));
            this.arrayMemberEditor1.ImageWidth = ((ushort)(64));
            this.arrayMemberEditor1.ItemHeight = 64;
            this.arrayMemberEditor1.ItemIndexOffset = 0;
            this.arrayMemberEditor1.Location = new System.Drawing.Point(0, 25);
            this.arrayMemberEditor1.Name = "arrayMemberEditor1";
            this.arrayMemberEditor1.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.arrayMemberEditor1.Size = new System.Drawing.Size(297, 425);
            this.arrayMemberEditor1.TabIndex = 0;
            this.arrayMemberEditor1.SelectedObjectChanged += new System.EventHandler(this.arrayMemberEditor1_SelectedObjectChanged);
            this.arrayMemberEditor1.ArrayUpdated += new System.EventHandler(this.arrayMemberEditor1_ArrayUpdated);
            // 
            // TEXGEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.texturePanel1);
            this.Controls.Add(this.arrayMemberEditor1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "TEXGEditor";
            this.TabText = "TEXGEditor";
            this.Text = "TEXGEditor";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ArrayMemberEditor arrayMemberEditor1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private Controls.TextureViewControl texturePanel1;
        private System.Windows.Forms.ToolStripButton buttonReplace;
        private System.Windows.Forms.ToolStripButton buttonExportAll;
        private System.Windows.Forms.ToolStripButton buttonExport;
        private System.Windows.Forms.ToolStripButton buttonImport;
    }
}