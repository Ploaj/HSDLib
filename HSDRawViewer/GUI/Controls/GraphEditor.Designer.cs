namespace HSDRawViewer.GUI.Controls
{
    partial class GraphEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphEditor));
            this.graphBox = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.nudFrame = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.keyProperty = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.showAllTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showFrameTicksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTangentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addKeyButton = new System.Windows.Forms.ToolStripButton();
            this.deleteKeyButton = new System.Windows.Forms.ToolStripButton();
            this.importKeyButton = new System.Windows.Forms.ToolStripButton();
            this.exportKeyButton = new System.Windows.Forms.ToolStripButton();
            this.helpButton = new System.Windows.Forms.ToolStripButton();
            this.buttonBakeTrack = new System.Windows.Forms.ToolStripButton();
            this.buttonCompressTrack = new System.Windows.Forms.ToolStripButton();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.trackTree = new System.Windows.Forms.TreeView();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.addTrackButton = new System.Windows.Forms.ToolStripButton();
            this.removeTrackButton = new System.Windows.Forms.ToolStripButton();
            this.trackTypeBox = new System.Windows.Forms.ToolStripComboBox();
            this.graphBox.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // graphBox
            // 
            this.graphBox.Controls.Add(this.panel1);
            this.graphBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphBox.Location = new System.Drawing.Point(3, 177);
            this.graphBox.Name = "graphBox";
            this.graphBox.Size = new System.Drawing.Size(724, 182);
            this.graphBox.TabIndex = 0;
            this.graphBox.TabStop = false;
            this.graphBox.Text = "Graph";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.nudFrame);
            this.panel1.Controls.Add(this.numericUpDown1);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(718, 31);
            this.panel1.TabIndex = 0;
            // 
            // nudFrame
            // 
            this.nudFrame.Location = new System.Drawing.Point(48, 9);
            this.nudFrame.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudFrame.Name = "nudFrame";
            this.nudFrame.Size = new System.Drawing.Size(142, 20);
            this.nudFrame.TabIndex = 0;
            this.nudFrame.ValueChanged += new System.EventHandler(this.nudFrame_ValueChanged);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(239, 9);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(66, 20);
            this.numericUpDown1.TabIndex = 6;
            this.numericUpDown1.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(390, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(311, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Value At Frame:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(196, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Zoom:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Frame:";
            // 
            // keyProperty
            // 
            this.keyProperty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keyProperty.HelpVisible = false;
            this.keyProperty.Location = new System.Drawing.Point(228, 16);
            this.keyProperty.Name = "keyProperty";
            this.keyProperty.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.keyProperty.Size = new System.Drawing.Size(496, 133);
            this.keyProperty.TabIndex = 0;
            this.keyProperty.ToolbarVisible = false;
            this.keyProperty.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.keyProperty_PropertyValueChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.addKeyButton,
            this.deleteKeyButton,
            this.importKeyButton,
            this.exportKeyButton,
            this.helpButton,
            this.buttonBakeTrack,
            this.buttonCompressTrack});
            this.toolStrip1.Location = new System.Drawing.Point(0, 152);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(727, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showAllTracksToolStripMenuItem,
            this.showFrameTicksToolStripMenuItem,
            this.showTangentsToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(103, 22);
            this.toolStripDropDownButton1.Text = "Display Options";
            // 
            // showAllTracksToolStripMenuItem
            // 
            this.showAllTracksToolStripMenuItem.CheckOnClick = true;
            this.showAllTracksToolStripMenuItem.Name = "showAllTracksToolStripMenuItem";
            this.showAllTracksToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.showAllTracksToolStripMenuItem.Text = "Show All Tracks";
            this.showAllTracksToolStripMenuItem.CheckedChanged += new System.EventHandler(this.OptionCheckChanged);
            // 
            // showFrameTicksToolStripMenuItem
            // 
            this.showFrameTicksToolStripMenuItem.Checked = true;
            this.showFrameTicksToolStripMenuItem.CheckOnClick = true;
            this.showFrameTicksToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showFrameTicksToolStripMenuItem.Name = "showFrameTicksToolStripMenuItem";
            this.showFrameTicksToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.showFrameTicksToolStripMenuItem.Text = "Show Frame Ticks";
            this.showFrameTicksToolStripMenuItem.CheckedChanged += new System.EventHandler(this.OptionCheckChanged);
            // 
            // showTangentsToolStripMenuItem
            // 
            this.showTangentsToolStripMenuItem.Checked = true;
            this.showTangentsToolStripMenuItem.CheckOnClick = true;
            this.showTangentsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showTangentsToolStripMenuItem.Name = "showTangentsToolStripMenuItem";
            this.showTangentsToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.showTangentsToolStripMenuItem.Text = "Show Tangents";
            this.showTangentsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.OptionCheckChanged);
            // 
            // addKeyButton
            // 
            this.addKeyButton.Image = global::HSDRawViewer.Properties.Resources.ts_add;
            this.addKeyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addKeyButton.Name = "addKeyButton";
            this.addKeyButton.Size = new System.Drawing.Size(78, 22);
            this.addKeyButton.Text = "Insert Key";
            this.addKeyButton.Click += new System.EventHandler(this.addKeyButton_Click);
            // 
            // deleteKeyButton
            // 
            this.deleteKeyButton.Image = global::HSDRawViewer.Properties.Resources.ts_subtract;
            this.deleteKeyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteKeyButton.Name = "deleteKeyButton";
            this.deleteKeyButton.Size = new System.Drawing.Size(82, 22);
            this.deleteKeyButton.Text = "Delete Key";
            this.deleteKeyButton.Click += new System.EventHandler(this.deleteKeyButton_Click);
            // 
            // importKeyButton
            // 
            this.importKeyButton.Image = global::HSDRawViewer.Properties.Resources.ts_importfile;
            this.importKeyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.importKeyButton.Name = "importKeyButton";
            this.importKeyButton.Size = new System.Drawing.Size(90, 22);
            this.importKeyButton.Text = "Import Keys";
            this.importKeyButton.Click += new System.EventHandler(this.importKeyButton_Click);
            // 
            // exportKeyButton
            // 
            this.exportKeyButton.Image = global::HSDRawViewer.Properties.Resources.ts_exportfile;
            this.exportKeyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exportKeyButton.Name = "exportKeyButton";
            this.exportKeyButton.Size = new System.Drawing.Size(88, 22);
            this.exportKeyButton.Text = "Export Keys";
            this.exportKeyButton.Click += new System.EventHandler(this.exportKeyButton_Click);
            // 
            // helpButton
            // 
            this.helpButton.Image = global::HSDRawViewer.Properties.Resources.ico_known;
            this.helpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(52, 22);
            this.helpButton.Text = "Help";
            this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // buttonBakeTrack
            // 
            this.buttonBakeTrack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonBakeTrack.Image = ((System.Drawing.Image)(resources.GetObject("buttonBakeTrack.Image")));
            this.buttonBakeTrack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonBakeTrack.Name = "buttonBakeTrack";
            this.buttonBakeTrack.Size = new System.Drawing.Size(36, 22);
            this.buttonBakeTrack.Text = "Bake";
            this.buttonBakeTrack.Click += new System.EventHandler(this.buttonBakeTrack_Click);
            // 
            // buttonCompressTrack
            // 
            this.buttonCompressTrack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonCompressTrack.Image = ((System.Drawing.Image)(resources.GetObject("buttonCompressTrack.Image")));
            this.buttonCompressTrack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonCompressTrack.Name = "buttonCompressTrack";
            this.buttonCompressTrack.Size = new System.Drawing.Size(64, 22);
            this.buttonCompressTrack.Text = "Compress";
            this.buttonCompressTrack.Click += new System.EventHandler(this.buttonCompressTrack_Click);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 177);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 182);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.keyProperty);
            this.groupBox1.Controls.Add(this.splitter2);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(727, 152);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tracks and Keys";
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(225, 16);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 133);
            this.splitter2.TabIndex = 4;
            this.splitter2.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.trackTree);
            this.groupBox2.Controls.Add(this.toolStrip2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox2.Location = new System.Drawing.Point(3, 16);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(222, 133);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tracks";
            // 
            // trackTree
            // 
            this.trackTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackTree.HideSelection = false;
            this.trackTree.Location = new System.Drawing.Point(3, 41);
            this.trackTree.Name = "trackTree";
            this.trackTree.Size = new System.Drawing.Size(216, 89);
            this.trackTree.TabIndex = 8;
            this.trackTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trackTree_AfterSelect);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addTrackButton,
            this.removeTrackButton,
            this.trackTypeBox});
            this.toolStrip2.Location = new System.Drawing.Point(3, 16);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(216, 25);
            this.toolStrip2.TabIndex = 7;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // addTrackButton
            // 
            this.addTrackButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addTrackButton.Image = global::HSDRawViewer.Properties.Resources.ts_add;
            this.addTrackButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addTrackButton.Name = "addTrackButton";
            this.addTrackButton.Size = new System.Drawing.Size(23, 22);
            this.addTrackButton.Text = "Add Track";
            this.addTrackButton.Click += new System.EventHandler(this.addTrackButton_Click);
            // 
            // removeTrackButton
            // 
            this.removeTrackButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.removeTrackButton.Image = global::HSDRawViewer.Properties.Resources.ts_subtract;
            this.removeTrackButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeTrackButton.Name = "removeTrackButton";
            this.removeTrackButton.Size = new System.Drawing.Size(23, 22);
            this.removeTrackButton.Text = "Remove Track";
            this.removeTrackButton.Click += new System.EventHandler(this.removeTrackButton_Click);
            // 
            // trackTypeBox
            // 
            this.trackTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.trackTypeBox.Name = "trackTypeBox";
            this.trackTypeBox.Size = new System.Drawing.Size(121, 25);
            this.trackTypeBox.SelectedIndexChanged += new System.EventHandler(this.trackTypeBox_SelectedIndexChanged);
            // 
            // GraphEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.graphBox);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.groupBox1);
            this.Name = "GraphEditor";
            this.Size = new System.Drawing.Size(727, 359);
            this.graphBox.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox graphBox;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.PropertyGrid keyProperty;
        private System.Windows.Forms.ToolStripButton deleteKeyButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.ToolStripButton addKeyButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TreeView trackTree;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton addTrackButton;
        private System.Windows.Forms.ToolStripButton removeTrackButton;
        private System.Windows.Forms.ToolStripComboBox trackTypeBox;
        private System.Windows.Forms.ToolStripButton importKeyButton;
        private System.Windows.Forms.ToolStripButton exportKeyButton;
        private System.Windows.Forms.ToolStripButton helpButton;
        private System.Windows.Forms.NumericUpDown nudFrame;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem showAllTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showFrameTicksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showTangentsToolStripMenuItem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripButton buttonBakeTrack;
        private System.Windows.Forms.ToolStripButton buttonCompressTrack;
    }
}
