namespace HSDRawViewer.GUI.Plugins.SubactionEditor
{
    partial class ScriptEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptEditor));
            this.dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.importModelFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAllAsTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asMayaAnimToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asFigaTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.applyFSMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applyFSMToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.fighterModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.hitboxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hurtboxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hitboxInterpolationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hitboxInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.environmentCollisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groundedECBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ledgeGrabBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shieldBubbleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton4 = new System.Windows.Forms.ToolStripDropDownButton();
            this.exportTXTOnSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dockPanel1
            // 
            this.dockPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel1.Location = new System.Drawing.Point(0, 25);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Size = new System.Drawing.Size(800, 425);
            this.dockPanel1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton2,
            this.toolStripDropDownButton1,
            this.toolStripDropDownButton3,
            this.toolStripDropDownButton4});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importModelFromFileToolStripMenuItem,
            this.toolStripSeparator5,
            this.exportAllAsTextToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(38, 22);
            this.toolStripDropDownButton2.Text = "File";
            // 
            // importModelFromFileToolStripMenuItem
            // 
            this.importModelFromFileToolStripMenuItem.Name = "importModelFromFileToolStripMenuItem";
            this.importModelFromFileToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.importModelFromFileToolStripMenuItem.Text = "Load Model and Animation";
            this.importModelFromFileToolStripMenuItem.Click += new System.EventHandler(this.loadModelAndAnimationToolStripMenuItem_Click);
            // 
            // exportAllAsTextToolStripMenuItem
            // 
            this.exportAllAsTextToolStripMenuItem.Name = "exportAllAsTextToolStripMenuItem";
            this.exportAllAsTextToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.exportAllAsTextToolStripMenuItem.Text = "Export All As Text";
            this.exportAllAsTextToolStripMenuItem.Click += new System.EventHandler(this.exportAllAsTextToolStripMenuItem_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.toolStripSeparator2,
            this.editToolStripMenuItem,
            this.toolStripSeparator1,
            this.applyFSMToolStripMenuItem,
            this.applyFSMToolStripMenuItem1});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(76, 22);
            this.toolStripDropDownButton1.Text = "Animation";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.asMayaAnimToolStripMenuItem,
            this.asFigaTreeToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // asMayaAnimToolStripMenuItem
            // 
            this.asMayaAnimToolStripMenuItem.Name = "asMayaAnimToolStripMenuItem";
            this.asMayaAnimToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.asMayaAnimToolStripMenuItem.Text = "As MayaAnim";
            this.asMayaAnimToolStripMenuItem.Click += new System.EventHandler(this.asMayaAnimToolStripMenuItem_Click);
            // 
            // asFigaTreeToolStripMenuItem
            // 
            this.asFigaTreeToolStripMenuItem.Name = "asFigaTreeToolStripMenuItem";
            this.asFigaTreeToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.asFigaTreeToolStripMenuItem.Text = "As FigaTree";
            this.asFigaTreeToolStripMenuItem.Click += new System.EventHandler(this.asFigaTreeToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(128, 6);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(128, 6);
            // 
            // applyFSMToolStripMenuItem
            // 
            this.applyFSMToolStripMenuItem.Name = "applyFSMToolStripMenuItem";
            this.applyFSMToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.applyFSMToolStripMenuItem.Text = "Edit FSM";
            this.applyFSMToolStripMenuItem.Click += new System.EventHandler(this.applyFSMToolStripMenuItem_Click);
            // 
            // applyFSMToolStripMenuItem1
            // 
            this.applyFSMToolStripMenuItem1.Name = "applyFSMToolStripMenuItem1";
            this.applyFSMToolStripMenuItem1.Size = new System.Drawing.Size(131, 22);
            this.applyFSMToolStripMenuItem1.Text = "Apply FSM";
            this.applyFSMToolStripMenuItem1.Click += new System.EventHandler(this.applyFSMToolStripMenuItem1_Click);
            // 
            // toolStripDropDownButton3
            // 
            this.toolStripDropDownButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fighterModelToolStripMenuItem,
            this.itemModelToolStripMenuItem,
            this.toolStripSeparator3,
            this.hitboxesToolStripMenuItem,
            this.hurtboxesToolStripMenuItem,
            this.hitboxInterpolationToolStripMenuItem,
            this.hitboxInfoToolStripMenuItem,
            this.toolStripSeparator4,
            this.environmentCollisionToolStripMenuItem,
            this.groundedECBToolStripMenuItem,
            this.ledgeGrabBoxToolStripMenuItem,
            this.shieldBubbleToolStripMenuItem});
            this.toolStripDropDownButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton3.Image")));
            this.toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            this.toolStripDropDownButton3.Size = new System.Drawing.Size(45, 22);
            this.toolStripDropDownButton3.Text = "View";
            // 
            // fighterModelToolStripMenuItem
            // 
            this.fighterModelToolStripMenuItem.Checked = true;
            this.fighterModelToolStripMenuItem.CheckOnClick = true;
            this.fighterModelToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fighterModelToolStripMenuItem.Name = "fighterModelToolStripMenuItem";
            this.fighterModelToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.fighterModelToolStripMenuItem.Text = "Fighter Model";
            this.fighterModelToolStripMenuItem.CheckedChanged += new System.EventHandler(this.ToggleVisibility_Click);
            // 
            // itemModelToolStripMenuItem
            // 
            this.itemModelToolStripMenuItem.Checked = true;
            this.itemModelToolStripMenuItem.CheckOnClick = true;
            this.itemModelToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.itemModelToolStripMenuItem.Name = "itemModelToolStripMenuItem";
            this.itemModelToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.itemModelToolStripMenuItem.Text = "Item Model";
            this.itemModelToolStripMenuItem.CheckedChanged += new System.EventHandler(this.ToggleVisibility_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(188, 6);
            // 
            // hitboxesToolStripMenuItem
            // 
            this.hitboxesToolStripMenuItem.Checked = true;
            this.hitboxesToolStripMenuItem.CheckOnClick = true;
            this.hitboxesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hitboxesToolStripMenuItem.Name = "hitboxesToolStripMenuItem";
            this.hitboxesToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.hitboxesToolStripMenuItem.Text = "Hitboxes";
            this.hitboxesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.ToggleVisibility_Click);
            // 
            // hurtboxesToolStripMenuItem
            // 
            this.hurtboxesToolStripMenuItem.CheckOnClick = true;
            this.hurtboxesToolStripMenuItem.Name = "hurtboxesToolStripMenuItem";
            this.hurtboxesToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.hurtboxesToolStripMenuItem.Text = "Hurtboxes";
            this.hurtboxesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.ToggleVisibility_Click);
            // 
            // hitboxInterpolationToolStripMenuItem
            // 
            this.hitboxInterpolationToolStripMenuItem.CheckOnClick = true;
            this.hitboxInterpolationToolStripMenuItem.Name = "hitboxInterpolationToolStripMenuItem";
            this.hitboxInterpolationToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.hitboxInterpolationToolStripMenuItem.Text = "Hitbox Interpolation";
            this.hitboxInterpolationToolStripMenuItem.CheckedChanged += new System.EventHandler(this.ToggleVisibility_Click);
            // 
            // hitboxInfoToolStripMenuItem
            // 
            this.hitboxInfoToolStripMenuItem.CheckOnClick = true;
            this.hitboxInfoToolStripMenuItem.Name = "hitboxInfoToolStripMenuItem";
            this.hitboxInfoToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.hitboxInfoToolStripMenuItem.Text = "Hitbox Info";
            this.hitboxInfoToolStripMenuItem.CheckedChanged += new System.EventHandler(this.ToggleVisibility_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(188, 6);
            // 
            // environmentCollisionToolStripMenuItem
            // 
            this.environmentCollisionToolStripMenuItem.CheckOnClick = true;
            this.environmentCollisionToolStripMenuItem.Name = "environmentCollisionToolStripMenuItem";
            this.environmentCollisionToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.environmentCollisionToolStripMenuItem.Text = "Environment Collision";
            this.environmentCollisionToolStripMenuItem.CheckedChanged += new System.EventHandler(this.ToggleVisibility_Click);
            // 
            // groundedECBToolStripMenuItem
            // 
            this.groundedECBToolStripMenuItem.Checked = true;
            this.groundedECBToolStripMenuItem.CheckOnClick = true;
            this.groundedECBToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.groundedECBToolStripMenuItem.Name = "groundedECBToolStripMenuItem";
            this.groundedECBToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.groundedECBToolStripMenuItem.Text = "Grounded ECB";
            this.groundedECBToolStripMenuItem.CheckedChanged += new System.EventHandler(this.ToggleVisibility_Click);
            // 
            // ledgeGrabBoxToolStripMenuItem
            // 
            this.ledgeGrabBoxToolStripMenuItem.CheckOnClick = true;
            this.ledgeGrabBoxToolStripMenuItem.Name = "ledgeGrabBoxToolStripMenuItem";
            this.ledgeGrabBoxToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.ledgeGrabBoxToolStripMenuItem.Text = "Ledge Grab Box";
            this.ledgeGrabBoxToolStripMenuItem.CheckedChanged += new System.EventHandler(this.ToggleVisibility_Click);
            // 
            // shieldBubbleToolStripMenuItem
            // 
            this.shieldBubbleToolStripMenuItem.Checked = true;
            this.shieldBubbleToolStripMenuItem.CheckOnClick = true;
            this.shieldBubbleToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.shieldBubbleToolStripMenuItem.Name = "shieldBubbleToolStripMenuItem";
            this.shieldBubbleToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.shieldBubbleToolStripMenuItem.Text = "Shield Bubble";
            this.shieldBubbleToolStripMenuItem.CheckedChanged += new System.EventHandler(this.ToggleVisibility_Click);
            // 
            // toolStripDropDownButton4
            // 
            this.toolStripDropDownButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportTXTOnSaveToolStripMenuItem});
            this.toolStripDropDownButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton4.Image")));
            this.toolStripDropDownButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton4.Name = "toolStripDropDownButton4";
            this.toolStripDropDownButton4.Size = new System.Drawing.Size(62, 22);
            this.toolStripDropDownButton4.Text = "Options";
            // 
            // exportTXTOnSaveToolStripMenuItem
            // 
            this.exportTXTOnSaveToolStripMenuItem.CheckOnClick = true;
            this.exportTXTOnSaveToolStripMenuItem.Name = "exportTXTOnSaveToolStripMenuItem";
            this.exportTXTOnSaveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportTXTOnSaveToolStripMenuItem.Text = "Export TXT on Save";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(216, 6);
            // 
            // ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dockPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ScriptEditor";
            this.Text = "ScriptEditor";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem importModelFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem applyFSMToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem applyFSMToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem asMayaAnimToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asFigaTreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton3;
        private System.Windows.Forms.ToolStripMenuItem fighterModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem itemModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem hitboxesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hurtboxesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hitboxInterpolationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hitboxInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem environmentCollisionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem groundedECBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ledgeGrabBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shieldBubbleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAllAsTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton4;
        private System.Windows.Forms.ToolStripMenuItem exportTXTOnSaveToolStripMenuItem;
    }
}