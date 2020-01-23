namespace HSDRawViewer.GUI
{
    partial class AJSplitDialog
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
            this.lbFighting = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.buttonLoadAnims = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lbResult = new System.Windows.Forms.ListBox();
            this.buttonLoadResult = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.openPldatToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDATsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.exportAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbFighting
            // 
            this.lbFighting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbFighting.FormattingEnabled = true;
            this.lbFighting.Location = new System.Drawing.Point(3, 26);
            this.lbFighting.Name = "lbFighting";
            this.lbFighting.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFighting.Size = new System.Drawing.Size(511, 201);
            this.lbFighting.TabIndex = 10;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tabControl1);
            this.groupBox1.Controls.Add(this.toolStrip1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(531, 300);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Animations";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 41);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(525, 256);
            this.tabControl1.TabIndex = 12;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lbFighting);
            this.tabPage1.Controls.Add(this.buttonLoadAnims);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(517, 230);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Fighting Animations";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // buttonLoadAnims
            // 
            this.buttonLoadAnims.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonLoadAnims.Enabled = false;
            this.buttonLoadAnims.Location = new System.Drawing.Point(3, 3);
            this.buttonLoadAnims.Name = "buttonLoadAnims";
            this.buttonLoadAnims.Size = new System.Drawing.Size(511, 23);
            this.buttonLoadAnims.TabIndex = 11;
            this.buttonLoadAnims.Text = "Load Pl**AJ.dat";
            this.buttonLoadAnims.UseVisualStyleBackColor = true;
            this.buttonLoadAnims.Click += new System.EventHandler(this.openPlAJdatToolStripMenuItem_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lbResult);
            this.tabPage2.Controls.Add(this.buttonLoadResult);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(517, 230);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Result Animations";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lbResult
            // 
            this.lbResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbResult.FormattingEnabled = true;
            this.lbResult.Location = new System.Drawing.Point(3, 26);
            this.lbResult.Name = "lbResult";
            this.lbResult.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbResult.Size = new System.Drawing.Size(511, 201);
            this.lbResult.TabIndex = 11;
            // 
            // buttonLoadResult
            // 
            this.buttonLoadResult.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonLoadResult.Enabled = false;
            this.buttonLoadResult.Location = new System.Drawing.Point(3, 3);
            this.buttonLoadResult.Name = "buttonLoadResult";
            this.buttonLoadResult.Size = new System.Drawing.Size(511, 23);
            this.buttonLoadResult.TabIndex = 12;
            this.buttonLoadResult.Text = "Load GmRstM**.dat";
            this.buttonLoadResult.UseVisualStyleBackColor = true;
            this.buttonLoadResult.Click += new System.EventHandler(this.openGmRstMdatToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton3,
            this.toolStripDropDownButton1,
            this.toolStripDropDownButton2});
            this.toolStrip1.Location = new System.Drawing.Point(3, 16);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(525, 25);
            this.toolStrip1.TabIndex = 11;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton3
            // 
            this.toolStripDropDownButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openPldatToolStripMenuItem1,
            this.exportDATsToolStripMenuItem});
            this.toolStripDropDownButton3.Image = global::HSDRawViewer.Properties.Resources.ico_folder;
            this.toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            this.toolStripDropDownButton3.Size = new System.Drawing.Size(54, 22);
            this.toolStripDropDownButton3.Text = "File";
            // 
            // openPldatToolStripMenuItem1
            // 
            this.openPldatToolStripMenuItem1.Name = "openPldatToolStripMenuItem1";
            this.openPldatToolStripMenuItem1.Size = new System.Drawing.Size(146, 22);
            this.openPldatToolStripMenuItem1.Text = "Open Pl**.dat";
            this.openPldatToolStripMenuItem1.Click += new System.EventHandler(this.openPldatToolStripMenuItem_Click);
            // 
            // exportDATsToolStripMenuItem
            // 
            this.exportDATsToolStripMenuItem.Enabled = false;
            this.exportDATsToolStripMenuItem.Name = "exportDATsToolStripMenuItem";
            this.exportDATsToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.exportDATsToolStripMenuItem.Text = "Export DATs";
            this.exportDATsToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportAllToolStripMenuItem,
            this.exportSelected});
            this.toolStripDropDownButton1.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(70, 22);
            this.toolStripDropDownButton1.Text = "Export";
            // 
            // exportAllToolStripMenuItem
            // 
            this.exportAllToolStripMenuItem.Name = "exportAllToolStripMenuItem";
            this.exportAllToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.exportAllToolStripMenuItem.Text = "All";
            this.exportAllToolStripMenuItem.Click += new System.EventHandler(this.exportAllToolStripMenuItem_Click);
            // 
            // exportSelected
            // 
            this.exportSelected.Name = "exportSelected";
            this.exportSelected.Size = new System.Drawing.Size(118, 22);
            this.exportSelected.Text = "Selected";
            this.exportSelected.Click += new System.EventHandler(this.exportSelected_Click);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allToolStripMenuItem,
            this.selectedToolStripMenuItem});
            this.toolStripDropDownButton2.Image = global::HSDRawViewer.Properties.Resources.ico_replace;
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(77, 22);
            this.toolStripDropDownButton2.Text = "Replace";
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            this.allToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.allToolStripMenuItem.Text = "All";
            this.allToolStripMenuItem.Click += new System.EventHandler(this.buttonReplaceAll_Click);
            // 
            // selectedToolStripMenuItem
            // 
            this.selectedToolStripMenuItem.Name = "selectedToolStripMenuItem";
            this.selectedToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.selectedToolStripMenuItem.Text = "Selected";
            this.selectedToolStripMenuItem.Click += new System.EventHandler(this.buttonReplace_Click);
            // 
            // AJSplitDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 300);
            this.Controls.Add(this.groupBox1);
            this.Name = "AJSplitDialog";
            this.Text = "AJ Split";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbFighting;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem exportSelected;
        private System.Windows.Forms.ToolStripMenuItem exportAllToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button buttonLoadAnims;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListBox lbResult;
        private System.Windows.Forms.Button buttonLoadResult;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton3;
        private System.Windows.Forms.ToolStripMenuItem openPldatToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exportDATsToolStripMenuItem;
    }
}