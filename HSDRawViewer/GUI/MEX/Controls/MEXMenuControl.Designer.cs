namespace HSDRawViewer.GUI.MEX.Controls
{
    partial class MEXMenuControl
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
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.buttonSaveCSS = new System.Windows.Forms.ToolStripButton();
            this.buttonImportMnSlcChr = new System.Windows.Forms.ToolStripButton();
            this.buttonImportMnSlMap = new System.Windows.Forms.ToolStripButton();
            this.playButton = new System.Windows.Forms.ToolStripButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.toolStrip4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip4
            // 
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSaveCSS,
            this.buttonImportMnSlcChr,
            this.buttonImportMnSlMap,
            this.playButton});
            this.toolStrip4.Location = new System.Drawing.Point(0, 0);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.Size = new System.Drawing.Size(720, 25);
            this.toolStrip4.TabIndex = 2;
            this.toolStrip4.Text = "toolStrip4";
            // 
            // buttonSaveCSS
            // 
            this.buttonSaveCSS.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.buttonSaveCSS.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSaveCSS.Name = "buttonSaveCSS";
            this.buttonSaveCSS.Size = new System.Drawing.Size(134, 22);
            this.buttonSaveCSS.Text = "Save Menu Changes";
            this.buttonSaveCSS.Click += new System.EventHandler(this.buttonSaveCSS_Click);
            // 
            // buttonImportMnSlcChr
            // 
            this.buttonImportMnSlcChr.Image = global::HSDRawViewer.Properties.Resources.ts_importfile;
            this.buttonImportMnSlcChr.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonImportMnSlcChr.Name = "buttonImportMnSlcChr";
            this.buttonImportMnSlcChr.Size = new System.Drawing.Size(141, 22);
            this.buttonImportMnSlcChr.Text = "Import MnSlChr+IfAll";
            this.buttonImportMnSlcChr.Click += new System.EventHandler(this.buttonImportMnSlcChr_Click);
            // 
            // buttonImportMnSlMap
            // 
            this.buttonImportMnSlMap.Image = global::HSDRawViewer.Properties.Resources.ts_importfile;
            this.buttonImportMnSlMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonImportMnSlMap.Name = "buttonImportMnSlMap";
            this.buttonImportMnSlMap.Size = new System.Drawing.Size(117, 22);
            this.buttonImportMnSlMap.Text = "Import MnSlMap";
            this.buttonImportMnSlMap.Click += new System.EventHandler(this.buttonImportMnSlMap_Click);
            // 
            // playButton
            // 
            this.playButton.Image = global::HSDRawViewer.Properties.Resources.ts_play;
            this.playButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(49, 22);
            this.playButton.Text = "Play";
            this.playButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tabControl);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 25);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(720, 375);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Preview";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.tabControl.Location = new System.Drawing.Point(3, 16);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(242, 356);
            this.tabControl.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(234, 330);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "CSS";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(234, 330);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "SSS";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // MEXMenuControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.toolStrip4);
            this.Name = "MEXMenuControl";
            this.Size = new System.Drawing.Size(720, 400);
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.ToolStripButton buttonSaveCSS;
        private System.Windows.Forms.ToolStripButton buttonImportMnSlcChr;
        private System.Windows.Forms.ToolStripButton buttonImportMnSlMap;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripButton playButton;
    }
}
