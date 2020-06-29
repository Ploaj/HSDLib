namespace HSDRawViewer.GUI.Extra
{
    partial class SEMEditorTool
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSEMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSEMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smStdatToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.smStdatToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(926, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSEMToolStripMenuItem,
            this.exportSEMToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openSEMToolStripMenuItem
            // 
            this.openSEMToolStripMenuItem.Name = "openSEMToolStripMenuItem";
            this.openSEMToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.openSEMToolStripMenuItem.Text = "Open SEM";
            this.openSEMToolStripMenuItem.Click += new System.EventHandler(this.openSEMToolStripMenuItem_Click);
            // 
            // exportSEMToolStripMenuItem
            // 
            this.exportSEMToolStripMenuItem.Name = "exportSEMToolStripMenuItem";
            this.exportSEMToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.exportSEMToolStripMenuItem.Text = "Export SEM";
            this.exportSEMToolStripMenuItem.Click += new System.EventHandler(this.exportSEMToolStripMenuItem_Click);
            // 
            // smStdatToolStripMenuItem1
            // 
            this.smStdatToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.smStdatToolStripMenuItem1.Enabled = false;
            this.smStdatToolStripMenuItem1.Name = "smStdatToolStripMenuItem1";
            this.smStdatToolStripMenuItem1.Size = new System.Drawing.Size(66, 20);
            this.smStdatToolStripMenuItem1.Text = "SmSt.dat";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.ImportToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportSmStdatToolStripMenuItem_Click);
            // 
            // SEMEditorTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 473);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SEMEditorTool";
            this.Text = "SEM Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSEMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportSEMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smStdatToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
    }
}