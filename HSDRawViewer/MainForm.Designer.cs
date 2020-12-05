namespace HSDRawViewer
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsUnoptimizedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRootFromFileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addRootFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRootFromTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trimExcessDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aJToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sSMEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sEMEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAudioPlaybackDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.LocationLabel = new System.Windows.Forms.Label();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.showHideButton = new System.Windows.Forms.Button();
            this.nodeBox = new System.Windows.Forms.GroupBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.menuStrip1.SuspendLayout();
            this.nodeBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.HideSelection = false;
            this.treeView1.Indent = 16;
            this.treeView1.ItemHeight = 24;
            this.treeView1.LabelEdit = true;
            this.treeView1.Location = new System.Drawing.Point(3, 16);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(194, 382);
            this.treeView1.TabIndex = 0;
            this.treeView1.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_BeforeLabelEdit);
            this.treeView1.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_AfterLabelEdit);
            this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(704, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.saveAsUnoptimizedToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(192, 22);
            this.saveToolStripMenuItem1.Text = "Save";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.saveToolStripMenuItem.Text = "Save As";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsUnoptimizedToolStripMenuItem
            // 
            this.saveAsUnoptimizedToolStripMenuItem.Name = "saveAsUnoptimizedToolStripMenuItem";
            this.saveAsUnoptimizedToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.saveAsUnoptimizedToolStripMenuItem.Text = "Save As (No Optimize)";
            this.saveAsUnoptimizedToolStripMenuItem.Click += new System.EventHandler(this.saveAsUnoptimizedToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addRootFromFileToolStripMenuItem1,
            this.addRootFromFileToolStripMenuItem,
            this.addRootFromTypeToolStripMenuItem,
            this.trimExcessDataToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // addRootFromFileToolStripMenuItem1
            // 
            this.addRootFromFileToolStripMenuItem1.Name = "addRootFromFileToolStripMenuItem1";
            this.addRootFromFileToolStripMenuItem1.Size = new System.Drawing.Size(200, 22);
            this.addRootFromFileToolStripMenuItem1.Text = "Add Root From File";
            this.addRootFromFileToolStripMenuItem1.Click += new System.EventHandler(this.addRootFromFileToolStripMenuItem1_Click);
            // 
            // addRootFromFileToolStripMenuItem
            // 
            this.addRootFromFileToolStripMenuItem.Name = "addRootFromFileToolStripMenuItem";
            this.addRootFromFileToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.addRootFromFileToolStripMenuItem.Text = "Add Root From DAT File";
            this.addRootFromFileToolStripMenuItem.Click += new System.EventHandler(this.addRootFromFileToolStripMenuItem_Click);
            // 
            // addRootFromTypeToolStripMenuItem
            // 
            this.addRootFromTypeToolStripMenuItem.Name = "addRootFromTypeToolStripMenuItem";
            this.addRootFromTypeToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.addRootFromTypeToolStripMenuItem.Text = "Add Root From Type";
            this.addRootFromTypeToolStripMenuItem.Click += new System.EventHandler(this.addRootFromTypeToolStripMenuItem_Click);
            // 
            // trimExcessDataToolStripMenuItem
            // 
            this.trimExcessDataToolStripMenuItem.Name = "trimExcessDataToolStripMenuItem";
            this.trimExcessDataToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.trimExcessDataToolStripMenuItem.Text = "Trim Excess Data";
            this.trimExcessDataToolStripMenuItem.Click += new System.EventHandler(this.trimExcessDataToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.propertyViewToolStripMenuItem,
            this.viewportToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // propertyViewToolStripMenuItem
            // 
            this.propertyViewToolStripMenuItem.Checked = true;
            this.propertyViewToolStripMenuItem.CheckOnClick = true;
            this.propertyViewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.propertyViewToolStripMenuItem.Name = "propertyViewToolStripMenuItem";
            this.propertyViewToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.propertyViewToolStripMenuItem.Text = "Property View";
            this.propertyViewToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.propertyViewToolStripMenuItem_CheckStateChanged);
            // 
            // viewportToolStripMenuItem
            // 
            this.viewportToolStripMenuItem.Checked = true;
            this.viewportToolStripMenuItem.CheckOnClick = true;
            this.viewportToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewportToolStripMenuItem.Name = "viewportToolStripMenuItem";
            this.viewportToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.viewportToolStripMenuItem.Text = "Viewport";
            this.viewportToolStripMenuItem.CheckedChanged += new System.EventHandler(this.viewportToolStripMenuItem_CheckedChanged);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aJToolToolStripMenuItem,
            this.sSMEditorToolStripMenuItem,
            this.sEMEditorToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // aJToolToolStripMenuItem
            // 
            this.aJToolToolStripMenuItem.Name = "aJToolToolStripMenuItem";
            this.aJToolToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.aJToolToolStripMenuItem.Text = "AJ Tool";
            this.aJToolToolStripMenuItem.Click += new System.EventHandler(this.aJToolToolStripMenuItem_Click);
            // 
            // sSMEditorToolStripMenuItem
            // 
            this.sSMEditorToolStripMenuItem.Name = "sSMEditorToolStripMenuItem";
            this.sSMEditorToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.sSMEditorToolStripMenuItem.Text = "SSM Editor";
            this.sSMEditorToolStripMenuItem.Click += new System.EventHandler(this.sSMEditorToolStripMenuItem_Click);
            // 
            // sEMEditorToolStripMenuItem
            // 
            this.sEMEditorToolStripMenuItem.Name = "sEMEditorToolStripMenuItem";
            this.sEMEditorToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.sEMEditorToolStripMenuItem.Text = "SEM Editor";
            this.sEMEditorToolStripMenuItem.Click += new System.EventHandler(this.sEMEditorToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAudioPlaybackDeviceToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // selectAudioPlaybackDeviceToolStripMenuItem
            // 
            this.selectAudioPlaybackDeviceToolStripMenuItem.Name = "selectAudioPlaybackDeviceToolStripMenuItem";
            this.selectAudioPlaybackDeviceToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.selectAudioPlaybackDeviceToolStripMenuItem.Text = "Select Audio Playback Device";
            this.selectAudioPlaybackDeviceToolStripMenuItem.Click += new System.EventHandler(this.selectAudioPlaybackDeviceToolStripMenuItem_Click);
            // 
            // LocationLabel
            // 
            this.LocationLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.LocationLabel.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LocationLabel.Location = new System.Drawing.Point(0, 24);
            this.LocationLabel.Name = "LocationLabel";
            this.LocationLabel.Size = new System.Drawing.Size(704, 16);
            this.LocationLabel.TabIndex = 5;
            this.LocationLabel.Text = "Location:";
            // 
            // dockPanel
            // 
            this.dockPanel.ActiveAutoHideContent = null;
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dockPanel.Location = new System.Drawing.Point(216, 40);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size(488, 401);
            this.dockPanel.TabIndex = 6;
            // 
            // showHideButton
            // 
            this.showHideButton.BackColor = System.Drawing.SystemColors.Control;
            this.showHideButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.showHideButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.showHideButton.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showHideButton.Location = new System.Drawing.Point(203, 40);
            this.showHideButton.Name = "showHideButton";
            this.showHideButton.Size = new System.Drawing.Size(13, 401);
            this.showHideButton.TabIndex = 10;
            this.showHideButton.Text = "<";
            this.showHideButton.UseVisualStyleBackColor = false;
            this.showHideButton.Click += new System.EventHandler(this.showHideButton_Click);
            // 
            // nodeBox
            // 
            this.nodeBox.Controls.Add(this.treeView1);
            this.nodeBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.nodeBox.Location = new System.Drawing.Point(0, 40);
            this.nodeBox.Name = "nodeBox";
            this.nodeBox.Size = new System.Drawing.Size(200, 401);
            this.nodeBox.TabIndex = 11;
            this.nodeBox.TabStop = false;
            this.nodeBox.Text = "Nodes";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(200, 40);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 401);
            this.splitter1.TabIndex = 12;
            this.splitter1.TabStop = false;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 441);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.showHideButton);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.nodeBox);
            this.Controls.Add(this.LocationLabel);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.TabText = "Hal DAT Browser";
            this.Text = "HSDraw";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.nodeBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addRootFromFileToolStripMenuItem;
        private System.Windows.Forms.Label LocationLabel;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertyViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewportToolStripMenuItem;
        private System.Windows.Forms.Button showHideButton;
        private System.Windows.Forms.GroupBox nodeBox;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aJToolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsUnoptimizedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sSMEditorToolStripMenuItem;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem sEMEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addRootFromTypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addRootFromFileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAudioPlaybackDeviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trimExcessDataToolStripMenuItem;
    }
}

