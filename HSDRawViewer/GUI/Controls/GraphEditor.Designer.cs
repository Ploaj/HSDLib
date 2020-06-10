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
            this.keyProperty = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.deleteKeyButton = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.addKeyButton = new System.Windows.Forms.ToolStripButton();
            this.optionsButton = new System.Windows.Forms.ToolStripButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.trackTree = new System.Windows.Forms.TreeView();
            this.addTrackButton = new System.Windows.Forms.ToolStripButton();
            this.removeTrackButton = new System.Windows.Forms.ToolStripButton();
            this.trackTypeBox = new System.Windows.Forms.ToolStripComboBox();
            this.importKeyButton = new System.Windows.Forms.ToolStripButton();
            this.exportKeyButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // graphBox
            // 
            this.graphBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphBox.Location = new System.Drawing.Point(3, 177);
            this.graphBox.Name = "graphBox";
            this.graphBox.Size = new System.Drawing.Size(724, 182);
            this.graphBox.TabIndex = 0;
            this.graphBox.TabStop = false;
            this.graphBox.Text = "Graph";
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
            this.optionsButton,
            this.addKeyButton,
            this.deleteKeyButton,
            this.importKeyButton,
            this.exportKeyButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 152);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(727, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 177);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 182);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // deleteKeyButton
            // 
            this.deleteKeyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteKeyButton.Image = global::HSDRawViewer.Properties.Resources.ts_subtract;
            this.deleteKeyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteKeyButton.Name = "deleteKeyButton";
            this.deleteKeyButton.Size = new System.Drawing.Size(23, 22);
            this.deleteKeyButton.Text = "Delete Key";
            this.deleteKeyButton.Click += new System.EventHandler(this.deleteKeyButton_Click);
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
            // addKeyButton
            // 
            this.addKeyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addKeyButton.Image = global::HSDRawViewer.Properties.Resources.ts_add;
            this.addKeyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addKeyButton.Name = "addKeyButton";
            this.addKeyButton.Size = new System.Drawing.Size(23, 22);
            this.addKeyButton.Text = "Add Key";
            this.addKeyButton.Click += new System.EventHandler(this.addKeyButton_Click);
            // 
            // optionsButton
            // 
            this.optionsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.optionsButton.Image = ((System.Drawing.Image)(resources.GetObject("optionsButton.Image")));
            this.optionsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.optionsButton.Name = "optionsButton";
            this.optionsButton.Size = new System.Drawing.Size(94, 22);
            this.optionsButton.Text = "Display Options";
            this.optionsButton.Click += new System.EventHandler(this.optionsButton_Click);
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
        private System.Windows.Forms.ToolStripButton optionsButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TreeView trackTree;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton addTrackButton;
        private System.Windows.Forms.ToolStripButton removeTrackButton;
        private System.Windows.Forms.ToolStripComboBox trackTypeBox;
        private System.Windows.Forms.ToolStripButton importKeyButton;
        private System.Windows.Forms.ToolStripButton exportKeyButton;
    }
}
