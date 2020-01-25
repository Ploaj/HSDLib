namespace HSDRawViewer.GUI.Extra
{
    partial class SEMEditor
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
            this.entryList = new System.Windows.Forms.ListBox();
            this.soundList = new System.Windows.Forms.ListBox();
            this.entryBox = new System.Windows.Forms.GroupBox();
            this.soundBox = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.exportSEMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.entryBox.SuspendLayout();
            this.soundBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
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
            this.openSEMToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openSEMToolStripMenuItem.Text = "Open SEM";
            this.openSEMToolStripMenuItem.Click += new System.EventHandler(this.openSEMToolStripMenuItem_Click);
            // 
            // entryList
            // 
            this.entryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entryList.FormattingEnabled = true;
            this.entryList.Location = new System.Drawing.Point(3, 16);
            this.entryList.Name = "entryList";
            this.entryList.Size = new System.Drawing.Size(127, 306);
            this.entryList.TabIndex = 1;
            this.entryList.SelectedIndexChanged += new System.EventHandler(this.entryList_SelectedIndexChanged);
            // 
            // soundList
            // 
            this.soundList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.soundList.FormattingEnabled = true;
            this.soundList.Location = new System.Drawing.Point(3, 16);
            this.soundList.Name = "soundList";
            this.soundList.Size = new System.Drawing.Size(194, 306);
            this.soundList.TabIndex = 3;
            this.soundList.SelectedIndexChanged += new System.EventHandler(this.soundList_SelectedIndexChanged);
            // 
            // entryBox
            // 
            this.entryBox.Controls.Add(this.entryList);
            this.entryBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.entryBox.Location = new System.Drawing.Point(0, 24);
            this.entryBox.Name = "entryBox";
            this.entryBox.Size = new System.Drawing.Size(133, 325);
            this.entryBox.TabIndex = 4;
            this.entryBox.TabStop = false;
            this.entryBox.Text = "Entries";
            // 
            // soundBox
            // 
            this.soundBox.Controls.Add(this.soundList);
            this.soundBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.soundBox.Location = new System.Drawing.Point(136, 24);
            this.soundBox.Name = "soundBox";
            this.soundBox.Size = new System.Drawing.Size(200, 325);
            this.soundBox.TabIndex = 5;
            this.soundBox.TabStop = false;
            this.soundBox.Text = "Sounds";
            // 
            // groupBox1
            // 
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(336, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(590, 325);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(133, 24);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 325);
            this.splitter1.TabIndex = 7;
            this.splitter1.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(336, 24);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 325);
            this.splitter2.TabIndex = 8;
            this.splitter2.TabStop = false;
            // 
            // exportSEMToolStripMenuItem
            // 
            this.exportSEMToolStripMenuItem.Name = "exportSEMToolStripMenuItem";
            this.exportSEMToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportSEMToolStripMenuItem.Text = "Export SEM";
            this.exportSEMToolStripMenuItem.Click += new System.EventHandler(this.exportSEMToolStripMenuItem_Click);
            // 
            // SEMEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 349);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.soundBox);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.entryBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SEMEditor";
            this.Text = "SEM Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.entryBox.ResumeLayout(false);
            this.soundBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSEMToolStripMenuItem;
        private System.Windows.Forms.ListBox entryList;
        private System.Windows.Forms.ListBox soundList;
        private System.Windows.Forms.GroupBox entryBox;
        private System.Windows.Forms.GroupBox soundBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.ToolStripMenuItem exportSEMToolStripMenuItem;
    }
}