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
            graphBox = new System.Windows.Forms.GroupBox();
            glviewport = new OpenTK.WinForms.GLControl();
            groupBox2 = new System.Windows.Forms.GroupBox();
            trackTree = new System.Windows.Forms.TreeView();
            splitter2 = new System.Windows.Forms.Splitter();
            keyProperty = new System.Windows.Forms.PropertyGrid();
            toolStrip2 = new System.Windows.Forms.ToolStrip();
            toolStripDropDownButton4 = new System.Windows.Forms.ToolStripDropDownButton();
            menuImportTracks = new System.Windows.Forms.ToolStripMenuItem();
            menuExportTracks = new System.Windows.Forms.ToolStripMenuItem();
            addTrackButton = new System.Windows.Forms.ToolStripButton();
            removeTrackButton = new System.Windows.Forms.ToolStripButton();
            trackTypeBox = new System.Windows.Forms.ToolStripComboBox();
            panel1 = new System.Windows.Forms.Panel();
            nudFrame = new System.Windows.Forms.NumericUpDown();
            numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            importKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exportKeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            addKeyButton = new System.Windows.Forms.ToolStripButton();
            deleteKeyButton = new System.Windows.Forms.ToolStripButton();
            toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            bakeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            compressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            reverseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            shiftValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            showAllTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            showFrameTicksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            showTangentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpButton = new System.Windows.Forms.ToolStripButton();
            graphBox.SuspendLayout();
            groupBox2.SuspendLayout();
            toolStrip2.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudFrame).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // graphBox
            // 
            graphBox.Controls.Add(glviewport);
            graphBox.Dock = System.Windows.Forms.DockStyle.Fill;
            graphBox.Location = new System.Drawing.Point(418, 59);
            graphBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            graphBox.Name = "graphBox";
            graphBox.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            graphBox.Size = new System.Drawing.Size(551, 493);
            graphBox.TabIndex = 0;
            graphBox.TabStop = false;
            graphBox.Text = "Graph";
            // 
            // glviewport
            // 
            glviewport.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            glviewport.APIVersion = new System.Version(3, 3, 0, 0);
            glviewport.Dock = System.Windows.Forms.DockStyle.Fill;
            glviewport.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            glviewport.IsEventDriven = true;
            glviewport.Location = new System.Drawing.Point(5, 24);
            glviewport.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            glviewport.Name = "glviewport";
            glviewport.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
            glviewport.Size = new System.Drawing.Size(541, 465);
            glviewport.TabIndex = 0;
            glviewport.Load += glviewport_Load;
            glviewport.Resize += glviewport_Resize;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(trackTree);
            groupBox2.Controls.Add(splitter2);
            groupBox2.Controls.Add(keyProperty);
            groupBox2.Controls.Add(toolStrip2);
            groupBox2.Dock = System.Windows.Forms.DockStyle.Left;
            groupBox2.Location = new System.Drawing.Point(0, 0);
            groupBox2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            groupBox2.Size = new System.Drawing.Size(418, 552);
            groupBox2.TabIndex = 9;
            groupBox2.TabStop = false;
            groupBox2.Text = "Tracks";
            // 
            // trackTree
            // 
            trackTree.Dock = System.Windows.Forms.DockStyle.Fill;
            trackTree.HideSelection = false;
            trackTree.Location = new System.Drawing.Point(5, 52);
            trackTree.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            trackTree.Name = "trackTree";
            trackTree.Size = new System.Drawing.Size(194, 496);
            trackTree.TabIndex = 8;
            trackTree.AfterSelect += trackTree_AfterSelect;
            // 
            // splitter2
            // 
            splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            splitter2.Location = new System.Drawing.Point(199, 52);
            splitter2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            splitter2.Name = "splitter2";
            splitter2.Size = new System.Drawing.Size(3, 496);
            splitter2.TabIndex = 11;
            splitter2.TabStop = false;
            // 
            // keyProperty
            // 
            keyProperty.Dock = System.Windows.Forms.DockStyle.Right;
            keyProperty.HelpVisible = false;
            keyProperty.Location = new System.Drawing.Point(202, 52);
            keyProperty.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            keyProperty.Name = "keyProperty";
            keyProperty.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            keyProperty.Size = new System.Drawing.Size(211, 496);
            keyProperty.TabIndex = 10;
            keyProperty.ToolbarVisible = false;
            keyProperty.PropertyValueChanged += keyProperty_PropertyValueChanged;
            // 
            // toolStrip2
            // 
            toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripDropDownButton4, addTrackButton, removeTrackButton, trackTypeBox });
            toolStrip2.Location = new System.Drawing.Point(5, 24);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.Size = new System.Drawing.Size(408, 28);
            toolStrip2.TabIndex = 7;
            toolStrip2.Text = "toolStrip2";
            // 
            // toolStripDropDownButton4
            // 
            toolStripDropDownButton4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { menuImportTracks, menuExportTracks });
            toolStripDropDownButton4.Image = Properties.Resources.ico_folder;
            toolStripDropDownButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton4.Name = "toolStripDropDownButton4";
            toolStripDropDownButton4.Size = new System.Drawing.Size(66, 25);
            toolStripDropDownButton4.Text = "File";
            // 
            // menuImportTracks
            // 
            menuImportTracks.Image = Properties.Resources.ts_importfile;
            menuImportTracks.Name = "menuImportTracks";
            menuImportTracks.Size = new System.Drawing.Size(224, 26);
            menuImportTracks.Text = "Import Tracks";
            menuImportTracks.Click += menuImportTracks_Click;
            // 
            // menuExportTracks
            // 
            menuExportTracks.Image = Properties.Resources.ts_exportfile;
            menuExportTracks.Name = "menuExportTracks";
            menuExportTracks.Size = new System.Drawing.Size(224, 26);
            menuExportTracks.Text = "Export Tracks";
            menuExportTracks.Click += menuExportTracks_Click;
            // 
            // addTrackButton
            // 
            addTrackButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            addTrackButton.Image = Properties.Resources.ts_add;
            addTrackButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            addTrackButton.Name = "addTrackButton";
            addTrackButton.Size = new System.Drawing.Size(29, 25);
            addTrackButton.Text = "Add Track";
            addTrackButton.Click += addTrackButton_Click;
            // 
            // removeTrackButton
            // 
            removeTrackButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            removeTrackButton.Image = Properties.Resources.ts_subtract;
            removeTrackButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            removeTrackButton.Name = "removeTrackButton";
            removeTrackButton.Size = new System.Drawing.Size(29, 25);
            removeTrackButton.Text = "Remove Track";
            removeTrackButton.Click += removeTrackButton_Click;
            // 
            // trackTypeBox
            // 
            trackTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            trackTypeBox.Name = "trackTypeBox";
            trackTypeBox.Size = new System.Drawing.Size(159, 28);
            trackTypeBox.SelectedIndexChanged += trackTypeBox_SelectedIndexChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(nudFrame);
            panel1.Controls.Add(numericUpDown1);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(418, 27);
            panel1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(551, 32);
            panel1.TabIndex = 10;
            // 
            // nudFrame
            // 
            nudFrame.Location = new System.Drawing.Point(62, 0);
            nudFrame.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            nudFrame.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudFrame.Name = "nudFrame";
            nudFrame.Size = new System.Drawing.Size(190, 27);
            nudFrame.TabIndex = 0;
            nudFrame.ValueChanged += nudFrame_ValueChanged;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new System.Drawing.Point(318, 0);
            numericUpDown1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            numericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new System.Drawing.Size(88, 27);
            numericUpDown1.TabIndex = 6;
            numericUpDown1.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(526, 8);
            label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(17, 20);
            label4.TabIndex = 5;
            label4.Text = "0";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(415, 8);
            label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(112, 20);
            label3.TabIndex = 5;
            label3.Text = "Value At Frame:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(262, 7);
            label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(52, 20);
            label2.TabIndex = 5;
            label2.Text = "Zoom:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(7, 5);
            label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(53, 20);
            label1.TabIndex = 5;
            label1.Text = "Frame:";
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripDropDownButton2, addKeyButton, deleteKeyButton, toolStripDropDownButton3, toolStripDropDownButton1, helpButton });
            toolStrip1.Location = new System.Drawing.Point(418, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(551, 27);
            toolStrip1.TabIndex = 11;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton2
            // 
            toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { importKeysToolStripMenuItem, exportKeysToolStripMenuItem });
            toolStripDropDownButton2.Image = Properties.Resources.ico_folder;
            toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            toolStripDropDownButton2.Size = new System.Drawing.Size(66, 24);
            toolStripDropDownButton2.Text = "File";
            // 
            // importKeysToolStripMenuItem
            // 
            importKeysToolStripMenuItem.Image = Properties.Resources.ts_importfile;
            importKeysToolStripMenuItem.Name = "importKeysToolStripMenuItem";
            importKeysToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            importKeysToolStripMenuItem.Text = "Import Keys";
            importKeysToolStripMenuItem.Click += importKeyButton_Click;
            // 
            // exportKeysToolStripMenuItem
            // 
            exportKeysToolStripMenuItem.Image = Properties.Resources.ts_exportfile;
            exportKeysToolStripMenuItem.Name = "exportKeysToolStripMenuItem";
            exportKeysToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            exportKeysToolStripMenuItem.Text = "Export Keys";
            exportKeysToolStripMenuItem.Click += exportKeyButton_Click;
            // 
            // addKeyButton
            // 
            addKeyButton.Image = Properties.Resources.ts_add;
            addKeyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            addKeyButton.Name = "addKeyButton";
            addKeyButton.Size = new System.Drawing.Size(97, 24);
            addKeyButton.Text = "Insert Key";
            addKeyButton.Click += addKeyButton_Click;
            // 
            // deleteKeyButton
            // 
            deleteKeyButton.Image = Properties.Resources.ts_subtract;
            deleteKeyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            deleteKeyButton.Name = "deleteKeyButton";
            deleteKeyButton.Size = new System.Drawing.Size(105, 24);
            deleteKeyButton.Text = "Delete Key";
            deleteKeyButton.Click += deleteKeyButton_Click;
            // 
            // toolStripDropDownButton3
            // 
            toolStripDropDownButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { bakeToolStripMenuItem, compressToolStripMenuItem, reverseToolStripMenuItem, shiftValuesToolStripMenuItem });
            toolStripDropDownButton3.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton3.Image");
            toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            toolStripDropDownButton3.Size = new System.Drawing.Size(58, 24);
            toolStripDropDownButton3.Text = "Tools";
            // 
            // bakeToolStripMenuItem
            // 
            bakeToolStripMenuItem.Name = "bakeToolStripMenuItem";
            bakeToolStripMenuItem.Size = new System.Drawing.Size(168, 26);
            bakeToolStripMenuItem.Text = "Bake";
            bakeToolStripMenuItem.Click += buttonBakeTrack_Click;
            // 
            // compressToolStripMenuItem
            // 
            compressToolStripMenuItem.Name = "compressToolStripMenuItem";
            compressToolStripMenuItem.Size = new System.Drawing.Size(168, 26);
            compressToolStripMenuItem.Text = "Compress";
            compressToolStripMenuItem.Click += buttonCompressTrack_Click;
            // 
            // reverseToolStripMenuItem
            // 
            reverseToolStripMenuItem.Name = "reverseToolStripMenuItem";
            reverseToolStripMenuItem.Size = new System.Drawing.Size(168, 26);
            reverseToolStripMenuItem.Text = "Reverse";
            reverseToolStripMenuItem.Click += reverseButton_Click;
            // 
            // shiftValuesToolStripMenuItem
            // 
            shiftValuesToolStripMenuItem.Name = "shiftValuesToolStripMenuItem";
            shiftValuesToolStripMenuItem.Size = new System.Drawing.Size(168, 26);
            shiftValuesToolStripMenuItem.Text = "Shift Values";
            shiftValuesToolStripMenuItem.Click += shiftValuesToolStripMenuItem_Click;
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { showAllTracksToolStripMenuItem, showFrameTicksToolStripMenuItem, showTangentsToolStripMenuItem });
            toolStripDropDownButton1.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new System.Drawing.Size(128, 24);
            toolStripDropDownButton1.Text = "Display Options";
            // 
            // showAllTracksToolStripMenuItem
            // 
            showAllTracksToolStripMenuItem.CheckOnClick = true;
            showAllTracksToolStripMenuItem.Name = "showAllTracksToolStripMenuItem";
            showAllTracksToolStripMenuItem.Size = new System.Drawing.Size(209, 26);
            showAllTracksToolStripMenuItem.Text = "Show All Tracks";
            showAllTracksToolStripMenuItem.Click += OptionCheckChanged;
            // 
            // showFrameTicksToolStripMenuItem
            // 
            showFrameTicksToolStripMenuItem.Checked = true;
            showFrameTicksToolStripMenuItem.CheckOnClick = true;
            showFrameTicksToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            showFrameTicksToolStripMenuItem.Name = "showFrameTicksToolStripMenuItem";
            showFrameTicksToolStripMenuItem.Size = new System.Drawing.Size(209, 26);
            showFrameTicksToolStripMenuItem.Text = "Show Frame Ticks";
            showFrameTicksToolStripMenuItem.Click += OptionCheckChanged;
            // 
            // showTangentsToolStripMenuItem
            // 
            showTangentsToolStripMenuItem.Checked = true;
            showTangentsToolStripMenuItem.CheckOnClick = true;
            showTangentsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            showTangentsToolStripMenuItem.Name = "showTangentsToolStripMenuItem";
            showTangentsToolStripMenuItem.Size = new System.Drawing.Size(209, 26);
            showTangentsToolStripMenuItem.Text = "Show Tangents";
            showTangentsToolStripMenuItem.Click += OptionCheckChanged;
            // 
            // helpButton
            // 
            helpButton.Image = Properties.Resources.ico_known;
            helpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            helpButton.Name = "helpButton";
            helpButton.Size = new System.Drawing.Size(65, 24);
            helpButton.Text = "Help";
            helpButton.Click += helpButton_Click;
            // 
            // GraphEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(graphBox);
            Controls.Add(panel1);
            Controls.Add(toolStrip1);
            Controls.Add(groupBox2);
            Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            Name = "GraphEditor";
            Size = new System.Drawing.Size(969, 552);
            graphBox.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            toolStrip2.ResumeLayout(false);
            toolStrip2.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudFrame).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private OpenTK.WinForms.GLControl glviewport;
        private System.Windows.Forms.ToolStripMenuItem shiftValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton4;
        private System.Windows.Forms.ToolStripMenuItem menuImportTracks;
        private System.Windows.Forms.ToolStripMenuItem menuExportTracks;
    }
}
