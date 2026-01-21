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
            jointTree = new System.Windows.Forms.TreeView();
            splitter1 = new System.Windows.Forms.Splitter();
            graphEditor1 = new Controls.GraphEditor();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            optimizeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // jointTree
            // 
            jointTree.Dock = System.Windows.Forms.DockStyle.Left;
            jointTree.HideSelection = false;
            jointTree.Indent = 12;
            jointTree.ItemHeight = 24;
            jointTree.Location = new System.Drawing.Point(0, 27);
            jointTree.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            jointTree.Name = "jointTree";
            jointTree.Size = new System.Drawing.Size(327, 841);
            jointTree.TabIndex = 1;
            jointTree.AfterSelect += jointTree_AfterSelect;
            // 
            // splitter1
            // 
            splitter1.Location = new System.Drawing.Point(327, 27);
            splitter1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            splitter1.Name = "splitter1";
            splitter1.Size = new System.Drawing.Size(5, 841);
            splitter1.TabIndex = 2;
            splitter1.TabStop = false;
            // 
            // graphEditor1
            // 
            graphEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            graphEditor1.Location = new System.Drawing.Point(332, 27);
            graphEditor1.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            graphEditor1.Name = "graphEditor1";
            graphEditor1.Size = new System.Drawing.Size(1111, 841);
            graphEditor1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripDropDownButton1 });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(1443, 27);
            toolStrip1.TabIndex = 3;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { optimizeAllToolStripMenuItem });
            toolStripDropDownButton1.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new System.Drawing.Size(92, 24);
            toolStripDropDownButton1.Text = "Animation";
            // 
            // optimizeAllToolStripMenuItem
            // 
            optimizeAllToolStripMenuItem.Name = "optimizeAllToolStripMenuItem";
            optimizeAllToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            optimizeAllToolStripMenuItem.Text = "Optimize All";
            optimizeAllToolStripMenuItem.Click += optimizeAllToolStripMenuItem_Click;
            // 
            // PopoutJointAnimationEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1443, 868);
            Controls.Add(graphEditor1);
            Controls.Add(splitter1);
            Controls.Add(jointTree);
            Controls.Add(toolStrip1);
            Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            Name = "PopoutJointAnimationEditor";
            Text = "Joint Animation Editor";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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