
namespace HSDRawViewer.GUI.Plugins
{
    partial class TexAnimEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TexAnimEditor));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.texturePanel1 = new HSDRawViewer.GUI.Controls.TexturePanel();
            this.arrayMemberEditor1 = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonImport = new System.Windows.Forms.ToolStripButton();
            this.buttonReplace = new System.Windows.Forms.ToolStripButton();
            this.buttonExport = new System.Windows.Forms.ToolStripButton();
            this.buttonExportAll = new System.Windows.Forms.ToolStripButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.graphEditor1 = new HSDRawViewer.GUI.Controls.GraphEditor();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(986, 562);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.texturePanel1);
            this.tabPage1.Controls.Add(this.arrayMemberEditor1);
            this.tabPage1.Controls.Add(this.toolStrip1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(978, 536);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Textures";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // texturePanel1
            // 
            this.texturePanel1.AutoScroll = true;
            this.texturePanel1.BackColor = System.Drawing.Color.Transparent;
            this.texturePanel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("texturePanel1.BackgroundImage")));
            this.texturePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.texturePanel1.ForeColor = System.Drawing.Color.White;
            this.texturePanel1.Image = null;
            this.texturePanel1.Location = new System.Drawing.Point(300, 28);
            this.texturePanel1.Name = "texturePanel1";
            this.texturePanel1.Size = new System.Drawing.Size(675, 505);
            this.texturePanel1.TabIndex = 4;
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
            this.arrayMemberEditor1.Location = new System.Drawing.Point(3, 28);
            this.arrayMemberEditor1.Name = "arrayMemberEditor1";
            this.arrayMemberEditor1.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.arrayMemberEditor1.Size = new System.Drawing.Size(297, 505);
            this.arrayMemberEditor1.TabIndex = 3;
            this.arrayMemberEditor1.SelectedObjectChanged += new System.EventHandler(this.arrayMemberEditor1_SelectedObjectChanged);
            this.arrayMemberEditor1.ArrayUpdated += new System.EventHandler(this.arrayMemberEditor1_ArrayUpdated);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonImport,
            this.buttonReplace,
            this.buttonExport,
            this.buttonExportAll});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(972, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
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
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.graphEditor1);
            this.tabPage2.Controls.Add(this.toolStrip2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(978, 536);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Animation";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // graphEditor1
            // 
            this.graphEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphEditor1.Location = new System.Drawing.Point(3, 28);
            this.graphEditor1.Name = "graphEditor1";
            this.graphEditor1.Size = new System.Drawing.Size(972, 505);
            this.graphEditor1.TabIndex = 4;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(972, 25);
            this.toolStrip2.TabIndex = 5;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(100, 22);
            this.toolStripButton1.Text = "Save Changes";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // TexAnimEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(986, 562);
            this.Controls.Add(this.tabControl1);
            this.Name = "TexAnimEditor";
            this.TabText = "TEXGEditor";
            this.Text = "TEXGEditor";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private Controls.TexturePanel texturePanel1;
        private ArrayMemberEditor arrayMemberEditor1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonImport;
        private System.Windows.Forms.ToolStripButton buttonReplace;
        private System.Windows.Forms.ToolStripButton buttonExport;
        private System.Windows.Forms.ToolStripButton buttonExportAll;
        private System.Windows.Forms.TabPage tabPage2;
        private Controls.GraphEditor graphEditor1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}