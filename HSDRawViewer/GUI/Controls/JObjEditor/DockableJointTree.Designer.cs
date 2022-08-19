namespace HSDRawViewer.GUI.Controls.JObjEditor
{
    partial class DockableJointTree
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockableJointTree));
            this.treeJOBJ = new System.Windows.Forms.TreeView();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.importFromINIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToINIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jointOptionsDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.makeParticleJointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceBonesFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeJOBJ
            // 
            this.treeJOBJ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeJOBJ.HideSelection = false;
            this.treeJOBJ.ItemHeight = 20;
            this.treeJOBJ.Location = new System.Drawing.Point(0, 25);
            this.treeJOBJ.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.treeJOBJ.Name = "treeJOBJ";
            this.treeJOBJ.Size = new System.Drawing.Size(533, 247);
            this.treeJOBJ.TabIndex = 2;
            this.treeJOBJ.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeJOBJ_AfterSelect);
            // 
            // toolStrip3
            // 
            this.toolStrip3.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.jointOptionsDropDownButton1});
            this.toolStrip3.Location = new System.Drawing.Point(0, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(533, 25);
            this.toolStrip3.TabIndex = 3;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importFromINIToolStripMenuItem,
            this.exportToINIToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(83, 22);
            this.toolStripDropDownButton1.Text = "Bone Labels";
            // 
            // importFromINIToolStripMenuItem
            // 
            this.importFromINIToolStripMenuItem.Name = "importFromINIToolStripMenuItem";
            this.importFromINIToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.importFromINIToolStripMenuItem.Text = "Import from INI";
            this.importFromINIToolStripMenuItem.Click += new System.EventHandler(this.importFromINIToolStripMenuItem_Click);
            // 
            // exportToINIToolStripMenuItem
            // 
            this.exportToINIToolStripMenuItem.Name = "exportToINIToolStripMenuItem";
            this.exportToINIToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.exportToINIToolStripMenuItem.Text = "Export to INI";
            this.exportToINIToolStripMenuItem.Click += new System.EventHandler(this.exportToINIToolStripMenuItem_Click);
            // 
            // jointOptionsDropDownButton1
            // 
            this.jointOptionsDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.jointOptionsDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.makeParticleJointToolStripMenuItem,
            this.replaceBonesFromFileToolStripMenuItem});
            this.jointOptionsDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("jointOptionsDropDownButton1.Image")));
            this.jointOptionsDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.jointOptionsDropDownButton1.Name = "jointOptionsDropDownButton1";
            this.jointOptionsDropDownButton1.Size = new System.Drawing.Size(47, 22);
            this.jointOptionsDropDownButton1.Text = "Tools";
            // 
            // makeParticleJointToolStripMenuItem
            // 
            this.makeParticleJointToolStripMenuItem.Name = "makeParticleJointToolStripMenuItem";
            this.makeParticleJointToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.makeParticleJointToolStripMenuItem.Text = "Make Particle Joint";
            this.makeParticleJointToolStripMenuItem.Click += new System.EventHandler(this.makeParticleJointToolStripMenuItem_Click);
            // 
            // replaceBonesFromFileToolStripMenuItem
            // 
            this.replaceBonesFromFileToolStripMenuItem.Name = "replaceBonesFromFileToolStripMenuItem";
            this.replaceBonesFromFileToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.replaceBonesFromFileToolStripMenuItem.Text = "Replace Bones From File";
            this.replaceBonesFromFileToolStripMenuItem.Click += new System.EventHandler(this.replaceBonesFromFileToolStripMenuItem_Click);
            // 
            // DockableJointTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 272);
            this.Controls.Add(this.treeJOBJ);
            this.Controls.Add(this.toolStrip3);
            this.Name = "DockableJointTree";
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeJOBJ;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripDropDownButton jointOptionsDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem makeParticleJointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceBonesFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem importFromINIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToINIToolStripMenuItem;
    }
}
