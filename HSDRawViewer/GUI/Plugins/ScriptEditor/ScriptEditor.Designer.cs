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
            dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            importModelFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            exportAllAsTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            asMayaAnimToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            asFigaTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            editAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            applyFSMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            applyFSMToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            fighterModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            bonesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            itemModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            hitboxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            hurtboxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            hitboxInterpolationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            hitboxInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            environmentCollisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            groundedECBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ledgeGrabBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            shieldBubbleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripDropDownButton4 = new System.Windows.Forms.ToolStripDropDownButton();
            exportTXTOnSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // dockPanel1
            // 
            dockPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            dockPanel1.Location = new System.Drawing.Point(0, 27);
            dockPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            dockPanel1.Name = "dockPanel1";
            dockPanel1.Size = new System.Drawing.Size(914, 573);
            dockPanel1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripDropDownButton2, toolStripDropDownButton1, toolStripDropDownButton3, toolStripDropDownButton4 });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(914, 27);
            toolStrip1.TabIndex = 9;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton2
            // 
            toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { importModelFromFileToolStripMenuItem, toolStripSeparator5, exportAllAsTextToolStripMenuItem });
            toolStripDropDownButton2.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton2.Image");
            toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            toolStripDropDownButton2.Size = new System.Drawing.Size(46, 24);
            toolStripDropDownButton2.Text = "File";
            // 
            // importModelFromFileToolStripMenuItem
            // 
            importModelFromFileToolStripMenuItem.Name = "importModelFromFileToolStripMenuItem";
            importModelFromFileToolStripMenuItem.Size = new System.Drawing.Size(274, 26);
            importModelFromFileToolStripMenuItem.Text = "Load Model and Animation";
            importModelFromFileToolStripMenuItem.Click += loadModelAndAnimationToolStripMenuItem_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(271, 6);
            // 
            // exportAllAsTextToolStripMenuItem
            // 
            exportAllAsTextToolStripMenuItem.Name = "exportAllAsTextToolStripMenuItem";
            exportAllAsTextToolStripMenuItem.Size = new System.Drawing.Size(274, 26);
            exportAllAsTextToolStripMenuItem.Text = "Export All As Text";
            exportAllAsTextToolStripMenuItem.Click += exportAllAsTextToolStripMenuItem_Click;
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { importToolStripMenuItem, exportToolStripMenuItem, toolStripSeparator2, editToolStripMenuItem, editAnimationToolStripMenuItem, toolStripSeparator1, applyFSMToolStripMenuItem, applyFSMToolStripMenuItem1 });
            toolStripDropDownButton1.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new System.Drawing.Size(92, 24);
            toolStripDropDownButton1.Text = "Animation";
            // 
            // importToolStripMenuItem
            // 
            importToolStripMenuItem.Name = "importToolStripMenuItem";
            importToolStripMenuItem.Size = new System.Drawing.Size(214, 26);
            importToolStripMenuItem.Text = "Import";
            importToolStripMenuItem.Click += importToolStripMenuItem_Click;
            // 
            // exportToolStripMenuItem
            // 
            exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { asMayaAnimToolStripMenuItem, asFigaTreeToolStripMenuItem });
            exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            exportToolStripMenuItem.Size = new System.Drawing.Size(214, 26);
            exportToolStripMenuItem.Text = "Export";
            // 
            // asMayaAnimToolStripMenuItem
            // 
            asMayaAnimToolStripMenuItem.Name = "asMayaAnimToolStripMenuItem";
            asMayaAnimToolStripMenuItem.Size = new System.Drawing.Size(183, 26);
            asMayaAnimToolStripMenuItem.Text = "As MayaAnim";
            asMayaAnimToolStripMenuItem.Click += asMayaAnimToolStripMenuItem_Click;
            // 
            // asFigaTreeToolStripMenuItem
            // 
            asFigaTreeToolStripMenuItem.Name = "asFigaTreeToolStripMenuItem";
            asFigaTreeToolStripMenuItem.Size = new System.Drawing.Size(183, 26);
            asFigaTreeToolStripMenuItem.Text = "As FigaTree";
            asFigaTreeToolStripMenuItem.Click += asFigaTreeToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(211, 6);
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new System.Drawing.Size(214, 26);
            editToolStripMenuItem.Text = "Edit Keys";
            editToolStripMenuItem.Click += editToolStripMenuItem_Click;
            // 
            // editAnimationToolStripMenuItem
            // 
            editAnimationToolStripMenuItem.Name = "editAnimationToolStripMenuItem";
            editAnimationToolStripMenuItem.Size = new System.Drawing.Size(214, 26);
            editAnimationToolStripMenuItem.Text = "Rebake Animation";
            editAnimationToolStripMenuItem.Click += editAnimationToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(211, 6);
            // 
            // applyFSMToolStripMenuItem
            // 
            applyFSMToolStripMenuItem.Name = "applyFSMToolStripMenuItem";
            applyFSMToolStripMenuItem.Size = new System.Drawing.Size(214, 26);
            applyFSMToolStripMenuItem.Text = "Edit FSM";
            applyFSMToolStripMenuItem.Click += applyFSMToolStripMenuItem_Click;
            // 
            // applyFSMToolStripMenuItem1
            // 
            applyFSMToolStripMenuItem1.Name = "applyFSMToolStripMenuItem1";
            applyFSMToolStripMenuItem1.Size = new System.Drawing.Size(214, 26);
            applyFSMToolStripMenuItem1.Text = "Apply FSM";
            applyFSMToolStripMenuItem1.Click += applyFSMToolStripMenuItem1_Click;
            // 
            // toolStripDropDownButton3
            // 
            toolStripDropDownButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { fighterModelToolStripMenuItem, bonesToolStripMenuItem, itemModelToolStripMenuItem, toolStripSeparator3, hitboxesToolStripMenuItem, hurtboxesToolStripMenuItem, hitboxInterpolationToolStripMenuItem, hitboxInfoToolStripMenuItem, toolStripSeparator4, environmentCollisionToolStripMenuItem, groundedECBToolStripMenuItem, ledgeGrabBoxToolStripMenuItem, shieldBubbleToolStripMenuItem });
            toolStripDropDownButton3.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton3.Image");
            toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            toolStripDropDownButton3.Size = new System.Drawing.Size(55, 24);
            toolStripDropDownButton3.Text = "View";
            // 
            // fighterModelToolStripMenuItem
            // 
            fighterModelToolStripMenuItem.Checked = true;
            fighterModelToolStripMenuItem.CheckOnClick = true;
            fighterModelToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            fighterModelToolStripMenuItem.Name = "fighterModelToolStripMenuItem";
            fighterModelToolStripMenuItem.Size = new System.Drawing.Size(236, 26);
            fighterModelToolStripMenuItem.Text = "Fighter Model";
            fighterModelToolStripMenuItem.CheckedChanged += UpdateRenderCheckboxes;
            // 
            // bonesToolStripMenuItem
            // 
            bonesToolStripMenuItem.CheckOnClick = true;
            bonesToolStripMenuItem.Name = "bonesToolStripMenuItem";
            bonesToolStripMenuItem.Size = new System.Drawing.Size(236, 26);
            bonesToolStripMenuItem.Text = "Bones";
            bonesToolStripMenuItem.CheckedChanged += UpdateRenderCheckboxes;
            // 
            // itemModelToolStripMenuItem
            // 
            itemModelToolStripMenuItem.Checked = true;
            itemModelToolStripMenuItem.CheckOnClick = true;
            itemModelToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            itemModelToolStripMenuItem.Name = "itemModelToolStripMenuItem";
            itemModelToolStripMenuItem.Size = new System.Drawing.Size(236, 26);
            itemModelToolStripMenuItem.Text = "Item Model";
            itemModelToolStripMenuItem.CheckedChanged += UpdateRenderCheckboxes;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(233, 6);
            // 
            // hitboxesToolStripMenuItem
            // 
            hitboxesToolStripMenuItem.Checked = true;
            hitboxesToolStripMenuItem.CheckOnClick = true;
            hitboxesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            hitboxesToolStripMenuItem.Name = "hitboxesToolStripMenuItem";
            hitboxesToolStripMenuItem.Size = new System.Drawing.Size(236, 26);
            hitboxesToolStripMenuItem.Text = "Hitboxes";
            hitboxesToolStripMenuItem.CheckedChanged += UpdateRenderCheckboxes;
            // 
            // hurtboxesToolStripMenuItem
            // 
            hurtboxesToolStripMenuItem.CheckOnClick = true;
            hurtboxesToolStripMenuItem.Name = "hurtboxesToolStripMenuItem";
            hurtboxesToolStripMenuItem.Size = new System.Drawing.Size(236, 26);
            hurtboxesToolStripMenuItem.Text = "Hurtboxes";
            hurtboxesToolStripMenuItem.CheckedChanged += UpdateRenderCheckboxes;
            // 
            // hitboxInterpolationToolStripMenuItem
            // 
            hitboxInterpolationToolStripMenuItem.CheckOnClick = true;
            hitboxInterpolationToolStripMenuItem.Name = "hitboxInterpolationToolStripMenuItem";
            hitboxInterpolationToolStripMenuItem.Size = new System.Drawing.Size(236, 26);
            hitboxInterpolationToolStripMenuItem.Text = "Hitbox Interpolation";
            hitboxInterpolationToolStripMenuItem.CheckedChanged += UpdateRenderCheckboxes;
            // 
            // hitboxInfoToolStripMenuItem
            // 
            hitboxInfoToolStripMenuItem.CheckOnClick = true;
            hitboxInfoToolStripMenuItem.Name = "hitboxInfoToolStripMenuItem";
            hitboxInfoToolStripMenuItem.Size = new System.Drawing.Size(236, 26);
            hitboxInfoToolStripMenuItem.Text = "Hitbox Info";
            hitboxInfoToolStripMenuItem.CheckedChanged += UpdateRenderCheckboxes;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(233, 6);
            // 
            // environmentCollisionToolStripMenuItem
            // 
            environmentCollisionToolStripMenuItem.CheckOnClick = true;
            environmentCollisionToolStripMenuItem.Name = "environmentCollisionToolStripMenuItem";
            environmentCollisionToolStripMenuItem.Size = new System.Drawing.Size(236, 26);
            environmentCollisionToolStripMenuItem.Text = "Environment Collision";
            environmentCollisionToolStripMenuItem.CheckedChanged += UpdateRenderCheckboxes;
            // 
            // groundedECBToolStripMenuItem
            // 
            groundedECBToolStripMenuItem.Checked = true;
            groundedECBToolStripMenuItem.CheckOnClick = true;
            groundedECBToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            groundedECBToolStripMenuItem.Name = "groundedECBToolStripMenuItem";
            groundedECBToolStripMenuItem.Size = new System.Drawing.Size(236, 26);
            groundedECBToolStripMenuItem.Text = "Grounded ECB";
            groundedECBToolStripMenuItem.CheckedChanged += UpdateRenderCheckboxes;
            // 
            // ledgeGrabBoxToolStripMenuItem
            // 
            ledgeGrabBoxToolStripMenuItem.CheckOnClick = true;
            ledgeGrabBoxToolStripMenuItem.Name = "ledgeGrabBoxToolStripMenuItem";
            ledgeGrabBoxToolStripMenuItem.Size = new System.Drawing.Size(236, 26);
            ledgeGrabBoxToolStripMenuItem.Text = "Ledge Grab Box";
            ledgeGrabBoxToolStripMenuItem.CheckedChanged += UpdateRenderCheckboxes;
            // 
            // shieldBubbleToolStripMenuItem
            // 
            shieldBubbleToolStripMenuItem.Checked = true;
            shieldBubbleToolStripMenuItem.CheckOnClick = true;
            shieldBubbleToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            shieldBubbleToolStripMenuItem.Name = "shieldBubbleToolStripMenuItem";
            shieldBubbleToolStripMenuItem.Size = new System.Drawing.Size(236, 26);
            shieldBubbleToolStripMenuItem.Text = "Shield Bubble";
            shieldBubbleToolStripMenuItem.CheckedChanged += UpdateRenderCheckboxes;
            // 
            // toolStripDropDownButton4
            // 
            toolStripDropDownButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { exportTXTOnSaveToolStripMenuItem });
            toolStripDropDownButton4.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton4.Image");
            toolStripDropDownButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton4.Name = "toolStripDropDownButton4";
            toolStripDropDownButton4.Size = new System.Drawing.Size(75, 24);
            toolStripDropDownButton4.Text = "Options";
            // 
            // exportTXTOnSaveToolStripMenuItem
            // 
            exportTXTOnSaveToolStripMenuItem.CheckOnClick = true;
            exportTXTOnSaveToolStripMenuItem.Name = "exportTXTOnSaveToolStripMenuItem";
            exportTXTOnSaveToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            exportTXTOnSaveToolStripMenuItem.Text = "Export TXT on Save";
            // 
            // ScriptEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(914, 600);
            Controls.Add(dockPanel1);
            Controls.Add(toolStrip1);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "ScriptEditor";
            Text = "ScriptEditor";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem bonesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editAnimationToolStripMenuItem;
    }
}