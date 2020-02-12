namespace HSDRawViewer.GUI.Extra
{
    partial class DSPViewer
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.channelBox = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonExport = new System.Windows.Forms.ToolStripButton();
            this.buttonReplace = new System.Windows.Forms.ToolStripButton();
            this.playButton = new System.Windows.Forms.ToolStripButton();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.propertyGrid);
            this.groupBox1.Controls.Add(this.splitter1);
            this.groupBox1.Controls.Add(this.channelBox);
            this.groupBox1.Controls.Add(this.toolStrip1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(499, 201);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "DSP Player";
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(3, 87);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(493, 111);
            this.propertyGrid.TabIndex = 2;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(3, 84);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(493, 3);
            this.splitter1.TabIndex = 9;
            this.splitter1.TabStop = false;
            // 
            // channelBox
            // 
            this.channelBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.channelBox.FormattingEnabled = true;
            this.channelBox.Location = new System.Drawing.Point(3, 41);
            this.channelBox.Name = "channelBox";
            this.channelBox.Size = new System.Drawing.Size(493, 43);
            this.channelBox.TabIndex = 7;
            this.channelBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.channelBox_MouseDoubleClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Enabled = false;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonExport,
            this.buttonReplace,
            this.playButton,
            this.stopButton});
            this.toolStrip1.Location = new System.Drawing.Point(3, 16);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(493, 25);
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonExport
            // 
            this.buttonExport.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.buttonExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(61, 22);
            this.buttonExport.Text = "Export";
            this.buttonExport.ToolTipText = "Save";
            this.buttonExport.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // buttonReplace
            // 
            this.buttonReplace.Image = global::HSDRawViewer.Properties.Resources.ico_replace;
            this.buttonReplace.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonReplace.Name = "buttonReplace";
            this.buttonReplace.Size = new System.Drawing.Size(68, 22);
            this.buttonReplace.Text = "Replace";
            this.buttonReplace.Click += new System.EventHandler(this.replaceButton_CLick);
            // 
            // playButton
            // 
            this.playButton.Image = global::HSDRawViewer.Properties.Resources.ts_play;
            this.playButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(49, 22);
            this.playButton.Text = "Play";
            this.playButton.ToolTipText = "Play";
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Image = global::HSDRawViewer.Properties.Resources.ts_stop;
            this.stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(51, 22);
            this.stopButton.Text = "Stop";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // DSPViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "DSPViewer";
            this.Size = new System.Drawing.Size(499, 201);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ListBox channelBox;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonExport;
        private System.Windows.Forms.ToolStripButton playButton;
        private System.Windows.Forms.ToolStripButton stopButton;
        private System.Windows.Forms.ToolStripButton buttonReplace;
    }
}
