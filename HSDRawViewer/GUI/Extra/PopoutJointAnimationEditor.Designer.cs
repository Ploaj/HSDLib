namespace HSDRawViewer.GUI.Extra
{
    partial class PopoutJointAnimationEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopoutJointAnimationEditor));
            this.jointTree = new System.Windows.Forms.TreeView();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.graphEditor1 = new HSDRawViewer.GUI.Controls.GraphEditor();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.optimizeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // jointTree
            // 
            this.jointTree.Dock = System.Windows.Forms.DockStyle.Left;
            this.jointTree.HideSelection = false;
            this.jointTree.Indent = 12;
            this.jointTree.ItemHeight = 24;
            this.jointTree.Location = new System.Drawing.Point(0, 31);
            this.jointTree.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.jointTree.Name = "jointTree";
            this.jointTree.Size = new System.Drawing.Size(327, 663);
            this.jointTree.TabIndex = 1;
            this.jointTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.jointTree_AfterSelect);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(327, 31);
            this.splitter1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(4, 663);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // graphEditor1
            // 
            this.graphEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphEditor1.Location = new System.Drawing.Point(331, 31);
            this.graphEditor1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.graphEditor1.Name = "graphEditor1";
            this.graphEditor1.Size = new System.Drawing.Size(1112, 663);
            this.graphEditor1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1443, 31);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optimizeAllToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(92, 28);
            this.toolStripDropDownButton1.Text = "Animation";
            // 
            // optimizeAllToolStripMenuItem
            // 
            this.optimizeAllToolStripMenuItem.Name = "optimizeAllToolStripMenuItem";
            this.optimizeAllToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.optimizeAllToolStripMenuItem.Text = "Optimize All";
            this.optimizeAllToolStripMenuItem.Click += new System.EventHandler(this.optimizeAllToolStripMenuItem_Click);
            // 
            // PopoutJointAnimationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1443, 694);
            this.Controls.Add(this.graphEditor1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.jointTree);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "PopoutJointAnimationEditor";
            this.Text = "Joint Animation Editor";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.GraphEditor graphEditor1;
        private System.Windows.Forms.TreeView jointTree;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem optimizeAllToolStripMenuItem;
    }
}