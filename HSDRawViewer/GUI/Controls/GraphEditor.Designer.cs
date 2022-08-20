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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.keyProperty = new System.Windows.Forms.PropertyGrid();
            this.trackTree = new System.Windows.Forms.TreeView();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.addTrackButton = new System.Windows.Forms.ToolStripButton();
            this.removeTrackButton = new System.Windows.Forms.ToolStripButton();
            this.trackTypeBox = new System.Windows.Forms.ToolStripComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.nudFrame = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.importKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addKeyButton = new System.Windows.Forms.ToolStripButton();
            this.deleteKeyButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.bakeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reverseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.showAllTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showFrameTicksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTangentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpButton = new System.Windows.Forms.ToolStripButton();
            this.groupBox2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // graphBox
            // 
            this.graphBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphBox.Location = new System.Drawing.Point(366, 49);
            this.graphBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.graphBox.Name = "graphBox";
            this.graphBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.graphBox.Size = new System.Drawing.Size(482, 365);
            this.graphBox.TabIndex = 0;
            this.graphBox.TabStop = false;
            this.graphBox.Text = "Graph";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.trackTree);
            this.groupBox2.Controls.Add(this.splitter2);
            this.groupBox2.Controls.Add(this.keyProperty);
            this.groupBox2.Controls.Add(this.toolStrip2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Size = new System.Drawing.Size(366, 414);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tracks";
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter2.Location = new System.Drawing.Point(174, 44);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 367);
            this.splitter2.TabIndex = 11;
            this.splitter2.TabStop = false;
            // 
            // keyProperty
            // 
            this.keyProperty.Dock = System.Windows.Forms.DockStyle.Right;
            this.keyProperty.HelpVisible = false;
            this.keyProperty.Location = new System.Drawing.Point(177, 44);
            this.keyProperty.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.keyProperty.Name = "keyProperty";
            this.keyProperty.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.keyProperty.Size = new System.Drawing.Size(185, 367);
            this.keyProperty.TabIndex = 10;
            this.keyProperty.ToolbarVisible = false;
            this.keyProperty.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.keyProperty_PropertyValueChanged);
            // 
            // trackTree
            // 
            this.trackTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackTree.HideSelection = false;
            this.trackTree.Location = new System.Drawing.Point(4, 44);
            this.trackTree.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.trackTree.Name = "trackTree";
            this.trackTree.Size = new System.Drawing.Size(170, 367);
            this.trackTree.TabIndex = 8;
            this.trackTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trackTree_AfterSelect);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addTrackButton,
            this.removeTrackButton,
            this.trackTypeBox});
            this.toolStrip2.Location = new System.Drawing.Point(4, 19);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(358, 25);
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
            this.trackTypeBox.Size = new System.Drawing.Size(140, 25);
            this.trackTypeBox.SelectedIndexChanged += new System.EventHandler(this.trackTypeBox_SelectedIndexChanged);
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
            this.panel1.Location = new System.Drawing.Point(366, 25);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(482, 24);
            this.panel1.TabIndex = 10;
            // 
            // nudFrame
            // 
            this.nudFrame.Location = new System.Drawing.Point(54, 0);
            this.nudFrame.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.nudFrame.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudFrame.Name = "nudFrame";
            this.nudFrame.Size = new System.Drawing.Size(166, 23);
            this.nudFrame.TabIndex = 0;
            this.nudFrame.ValueChanged += new System.EventHandler(this.nudFrame_ValueChanged);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(278, 0);
            this.numericUpDown1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(77, 23);
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
            this.label4.Location = new System.Drawing.Point(460, 6);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(363, 6);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Value At Frame:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(229, 5);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Zoom:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 4);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Frame:";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton2,
            this.addKeyButton,
            this.deleteKeyButton,
            this.toolStripDropDownButton3,
            this.toolStripDropDownButton1,
            this.helpButton});
            this.toolStrip1.Location = new System.Drawing.Point(366, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(482, 25);
            this.toolStrip1.TabIndex = 11;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importKeysToolStripMenuItem,
            this.exportKeysToolStripMenuItem});
            this.toolStripDropDownButton2.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(54, 22);
            this.toolStripDropDownButton2.Text = "File";
            // 
            // importKeysToolStripMenuItem
            // 
            this.importKeysToolStripMenuItem.Image = global::HSDRawViewer.Properties.Resources.ts_importfile;
            this.importKeysToolStripMenuItem.Name = "importKeysToolStripMenuItem";
            this.importKeysToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.importKeysToolStripMenuItem.Text = "Import Keys";
            this.importKeysToolStripMenuItem.Click += new System.EventHandler(this.importKeyButton_Click);
            // 
            // exportKeysToolStripMenuItem
            // 
            this.exportKeysToolStripMenuItem.Image = global::HSDRawViewer.Properties.Resources.ts_exportfile;
            this.exportKeysToolStripMenuItem.Name = "exportKeysToolStripMenuItem";
            this.exportKeysToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.exportKeysToolStripMenuItem.Text = "Export Keys";
            this.exportKeysToolStripMenuItem.Click += new System.EventHandler(this.exportKeyButton_Click);
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
            // toolStripDropDownButton3
            // 
            this.toolStripDropDownButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bakeToolStripMenuItem,
            this.compressToolStripMenuItem,
            this.reverseToolStripMenuItem});
            this.toolStripDropDownButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton3.Image")));
            this.toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            this.toolStripDropDownButton3.Size = new System.Drawing.Size(47, 22);
            this.toolStripDropDownButton3.Text = "Tools";
            // 
            // bakeToolStripMenuItem
            // 
            this.bakeToolStripMenuItem.Name = "bakeToolStripMenuItem";
            this.bakeToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.bakeToolStripMenuItem.Text = "Bake";
            this.bakeToolStripMenuItem.Click += new System.EventHandler(this.buttonBakeTrack_Click);
            // 
            // compressToolStripMenuItem
            // 
            this.compressToolStripMenuItem.Name = "compressToolStripMenuItem";
            this.compressToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.compressToolStripMenuItem.Text = "Compress";
            this.compressToolStripMenuItem.Click += new System.EventHandler(this.buttonCompressTrack_Click);
            // 
            // reverseToolStripMenuItem
            // 
            this.reverseToolStripMenuItem.Name = "reverseToolStripMenuItem";
            this.reverseToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.reverseToolStripMenuItem.Text = "Reverse";
            this.reverseToolStripMenuItem.Click += new System.EventHandler(this.reverseButton_Click);
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
            this.showAllTracksToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.showAllTracksToolStripMenuItem.Text = "Show All Tracks";
            this.showAllTracksToolStripMenuItem.Click += new System.EventHandler(this.OptionCheckChanged);
            // 
            // showFrameTicksToolStripMenuItem
            // 
            this.showFrameTicksToolStripMenuItem.Checked = true;
            this.showFrameTicksToolStripMenuItem.CheckOnClick = true;
            this.showFrameTicksToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showFrameTicksToolStripMenuItem.Name = "showFrameTicksToolStripMenuItem";
            this.showFrameTicksToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.showFrameTicksToolStripMenuItem.Text = "Show Frame Ticks";
            this.showFrameTicksToolStripMenuItem.Click += new System.EventHandler(this.OptionCheckChanged);
            // 
            // showTangentsToolStripMenuItem
            // 
            this.showTangentsToolStripMenuItem.Checked = true;
            this.showTangentsToolStripMenuItem.CheckOnClick = true;
            this.showTangentsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showTangentsToolStripMenuItem.Name = "showTangentsToolStripMenuItem";
            this.showTangentsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.showTangentsToolStripMenuItem.Text = "Show Tangents";
            this.showTangentsToolStripMenuItem.Click += new System.EventHandler(this.OptionCheckChanged);
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
            // GraphEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.graphBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.groupBox2);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "GraphEditor";
            this.Size = new System.Drawing.Size(848, 414);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox graphBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.PropertyGrid keyProperty;
        private System.Windows.Forms.TreeView trackTree;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton addTrackButton;
        private System.Windows.Forms.ToolStripButton removeTrackButton;
        private System.Windows.Forms.ToolStripComboBox trackTypeBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown nudFrame;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem showAllTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showFrameTicksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showTangentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton addKeyButton;
        private System.Windows.Forms.ToolStripButton deleteKeyButton;
        private System.Windows.Forms.ToolStripButton helpButton;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem importKeysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportKeysToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton3;
        private System.Windows.Forms.ToolStripMenuItem bakeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reverseToolStripMenuItem;
    }
}
