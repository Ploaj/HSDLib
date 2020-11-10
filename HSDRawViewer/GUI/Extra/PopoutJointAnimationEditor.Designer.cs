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
            this.jointTree = new System.Windows.Forms.TreeView();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.graphEditor1 = new HSDRawViewer.GUI.Controls.GraphEditor();
            this.SuspendLayout();
            // 
            // jointTree
            // 
            this.jointTree.Dock = System.Windows.Forms.DockStyle.Left;
            this.jointTree.HideSelection = false;
            this.jointTree.ItemHeight = 24;
            this.jointTree.Location = new System.Drawing.Point(0, 0);
            this.jointTree.Name = "jointTree";
            this.jointTree.Size = new System.Drawing.Size(246, 486);
            this.jointTree.TabIndex = 1;
            this.jointTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.jointTree_AfterSelect);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(246, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 486);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // graphEditor1
            // 
            this.graphEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphEditor1.Location = new System.Drawing.Point(249, 0);
            this.graphEditor1.Name = "graphEditor1";
            this.graphEditor1.Size = new System.Drawing.Size(833, 486);
            this.graphEditor1.TabIndex = 0;
            // 
            // PopoutJointAnimationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1082, 486);
            this.Controls.Add(this.graphEditor1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.jointTree);
            this.Name = "PopoutJointAnimationEditor";
            this.Text = "Joint Animation Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.GraphEditor graphEditor1;
        private System.Windows.Forms.TreeView jointTree;
        private System.Windows.Forms.Splitter splitter1;
    }
}