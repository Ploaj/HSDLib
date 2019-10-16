namespace HSDRawViewer.GUI.Plugins
{
    partial class TextureAnimationEditor
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.exportStripToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.importStripToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportFramesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importFramesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportPNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importPNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripComboBox1,
            this.toolStripLabel1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(284, 31);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(35, 28);
            this.toolStripLabel1.Text = "View:";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.toolStripComboBox1.Items.AddRange(new object[] {
            "Fill",
            "200%",
            "Actual"});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 31);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportStripToolStripMenuItem1,
            this.importStripToolStripMenuItem1,
            this.exportFramesToolStripMenuItem,
            this.importFramesToolStripMenuItem,
            this.exportPNGToolStripMenuItem,
            this.importPNGToolStripMenuItem});
            this.toolStripDropDownButton1.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.toolStripDropDownButton1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(37, 28);
            this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
            // 
            // exportStripToolStripMenuItem1
            // 
            this.exportStripToolStripMenuItem1.Name = "exportStripToolStripMenuItem1";
            this.exportStripToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.exportStripToolStripMenuItem1.Text = "Export Strip";
            this.exportStripToolStripMenuItem1.Click += new System.EventHandler(this.exportStripToolStripMenuItem1_Click);
            // 
            // importStripToolStripMenuItem1
            // 
            this.importStripToolStripMenuItem1.Name = "importStripToolStripMenuItem1";
            this.importStripToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.importStripToolStripMenuItem1.Text = "Import Strip";
            this.importStripToolStripMenuItem1.Click += new System.EventHandler(this.importStripToolStripMenuItem1_Click);
            // 
            // exportFramesToolStripMenuItem
            // 
            this.exportFramesToolStripMenuItem.Name = "exportFramesToolStripMenuItem";
            this.exportFramesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportFramesToolStripMenuItem.Text = "Export Frames";
            this.exportFramesToolStripMenuItem.Click += new System.EventHandler(this.exportFramesToolStripMenuItem_Click);
            // 
            // importFramesToolStripMenuItem
            // 
            this.importFramesToolStripMenuItem.Name = "importFramesToolStripMenuItem";
            this.importFramesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.importFramesToolStripMenuItem.Text = "Import Frames";
            this.importFramesToolStripMenuItem.Click += new System.EventHandler(this.importFramesToolStripMenuItem_Click);
            // 
            // exportPNGToolStripMenuItem
            // 
            this.exportPNGToolStripMenuItem.Name = "exportPNGToolStripMenuItem";
            this.exportPNGToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportPNGToolStripMenuItem.Text = "Export PNG";
            this.exportPNGToolStripMenuItem.Click += new System.EventHandler(this.exportPNGToolStripMenuItem_Click);
            // 
            // importPNGToolStripMenuItem
            // 
            this.importPNGToolStripMenuItem.Name = "importPNGToolStripMenuItem";
            this.importPNGToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.importPNGToolStripMenuItem.Text = "Import PNG";
            this.importPNGToolStripMenuItem.Click += new System.EventHandler(this.importPNGToolStripMenuItem_Click);
            // 
            // TextureAnimationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.toolStrip1);
            this.Name = "TextureAnimationEditor";
            this.TabText = "TextureAnimationEditor";
            this.Text = "TextureAnimationEditor";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem exportStripToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem importStripToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exportFramesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importFramesToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripMenuItem exportPNGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importPNGToolStripMenuItem;
    }
}