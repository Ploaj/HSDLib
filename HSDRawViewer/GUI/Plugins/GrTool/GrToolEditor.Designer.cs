namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool
{
    partial class GrToolEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GrToolEditor));
            dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            loadModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            buttonSave = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            buttonSelectCollision = new System.Windows.Forms.ToolStripButton();
            buttonSelectTriangle = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            collViewComboBox = new System.Windows.Forms.ToolStripComboBox();
            toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            buttonXRay = new System.Windows.Forms.ToolStripButton();
            wireframeButton = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            railAnimationButton = new System.Windows.Forms.ToolStripDropDownButton();
            loopAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            advanceFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            rewindFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            seekStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            seekEndToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            loadStarModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            drawArcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // dockPanel
            // 
            dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            dockPanel.Location = new System.Drawing.Point(0, 28);
            dockPanel.Name = "dockPanel";
            dockPanel.Size = new System.Drawing.Size(1062, 645);
            dockPanel.TabIndex = 0;
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripDropDownButton1, buttonSave, toolStripSeparator2, toolStripSeparator1, toolStripLabel1, buttonSelectCollision, buttonSelectTriangle, toolStripSeparator3, toolStripLabel2, collViewComboBox, toolStripButton1, buttonXRay, wireframeButton, toolStripSeparator4, railAnimationButton });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(1062, 28);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { loadModelToolStripMenuItem });
            toolStripDropDownButton1.Image = Properties.Resources.ts_importfile;
            toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new System.Drawing.Size(66, 25);
            toolStripDropDownButton1.Text = "File";
            // 
            // loadModelToolStripMenuItem
            // 
            loadModelToolStripMenuItem.Name = "loadModelToolStripMenuItem";
            loadModelToolStripMenuItem.Size = new System.Drawing.Size(172, 26);
            loadModelToolStripMenuItem.Text = "Load Model";
            loadModelToolStripMenuItem.Click += loadModelToolStripMenuItem_Click;
            // 
            // buttonSave
            // 
            buttonSave.Image = Properties.Resources.ico_save;
            buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new System.Drawing.Size(64, 25);
            buttonSave.Text = "Save";
            buttonSave.ToolTipText = "Save";
            buttonSave.Click += saveToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new System.Drawing.Size(116, 25);
            toolStripLabel1.Text = "Selection Mode:";
            // 
            // buttonSelectCollision
            // 
            buttonSelectCollision.Checked = true;
            buttonSelectCollision.CheckOnClick = true;
            buttonSelectCollision.CheckState = System.Windows.Forms.CheckState.Checked;
            buttonSelectCollision.Image = Properties.Resources.ts_solid;
            buttonSelectCollision.ImageTransparentColor = System.Drawing.Color.Magenta;
            buttonSelectCollision.Name = "buttonSelectCollision";
            buttonSelectCollision.Size = new System.Drawing.Size(70, 25);
            buttonSelectCollision.Text = "Node";
            // 
            // buttonSelectTriangle
            // 
            buttonSelectTriangle.CheckOnClick = true;
            buttonSelectTriangle.Image = Properties.Resources.ts_play;
            buttonSelectTriangle.ImageTransparentColor = System.Drawing.Color.Magenta;
            buttonSelectTriangle.Name = "buttonSelectTriangle";
            buttonSelectTriangle.Size = new System.Drawing.Size(65, 25);
            buttonSelectTriangle.Text = "Data";
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new System.Drawing.Size(122, 25);
            toolStripLabel2.Text = "Collision Display:";
            // 
            // collViewComboBox
            // 
            collViewComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            collViewComboBox.Name = "collViewComboBox";
            collViewComboBox.Size = new System.Drawing.Size(121, 28);
            // 
            // toolStripButton1
            // 
            toolStripButton1.Image = (System.Drawing.Image)resources.GetObject("toolStripButton1.Image");
            toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new System.Drawing.Size(139, 25);
            toolStripButton1.Text = "Display Settings";
            toolStripButton1.Click += toolStripButton1_Click;
            // 
            // buttonXRay
            // 
            buttonXRay.Checked = true;
            buttonXRay.CheckOnClick = true;
            buttonXRay.CheckState = System.Windows.Forms.CheckState.Checked;
            buttonXRay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            buttonXRay.Image = Properties.Resources.ts_xray;
            buttonXRay.ImageTransparentColor = System.Drawing.Color.Magenta;
            buttonXRay.Name = "buttonXRay";
            buttonXRay.Size = new System.Drawing.Size(29, 25);
            buttonXRay.Text = "X - Ray";
            // 
            // wireframeButton
            // 
            wireframeButton.Checked = true;
            wireframeButton.CheckOnClick = true;
            wireframeButton.CheckState = System.Windows.Forms.CheckState.Checked;
            wireframeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            wireframeButton.Image = Properties.Resources.ts_wireframe;
            wireframeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            wireframeButton.Name = "wireframeButton";
            wireframeButton.Size = new System.Drawing.Size(29, 25);
            wireframeButton.Text = "Show Wireframe";
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(6, 28);
            // 
            // railAnimationButton
            // 
            railAnimationButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { loopAnimationToolStripMenuItem, pauseToolStripMenuItem, toolStripSeparator6, advanceFrameToolStripMenuItem, rewindFrameToolStripMenuItem, seekStartToolStripMenuItem, seekEndToolStripMenuItem, toolStripSeparator5, loadStarModelToolStripMenuItem, drawArcToolStripMenuItem });
            railAnimationButton.Image = Properties.Resources.ts_play;
            railAnimationButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            railAnimationButton.Name = "railAnimationButton";
            railAnimationButton.Size = new System.Drawing.Size(141, 25);
            railAnimationButton.Text = "Rail Animation";
            // 
            // loopAnimationToolStripMenuItem
            // 
            loopAnimationToolStripMenuItem.Checked = true;
            loopAnimationToolStripMenuItem.CheckOnClick = true;
            loopAnimationToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            loopAnimationToolStripMenuItem.Name = "loopAnimationToolStripMenuItem";
            loopAnimationToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            loopAnimationToolStripMenuItem.Text = "Loop Animation";
            loopAnimationToolStripMenuItem.Click += loopAnimationToolStripMenuItem_Click;
            // 
            // pauseToolStripMenuItem
            // 
            pauseToolStripMenuItem.Checked = true;
            pauseToolStripMenuItem.CheckOnClick = true;
            pauseToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            pauseToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            pauseToolStripMenuItem.Text = "Play";
            pauseToolStripMenuItem.Click += pauseToolStripMenuItem_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new System.Drawing.Size(221, 6);
            // 
            // advanceFrameToolStripMenuItem
            // 
            advanceFrameToolStripMenuItem.Name = "advanceFrameToolStripMenuItem";
            advanceFrameToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            advanceFrameToolStripMenuItem.Text = "Advance Frame";
            advanceFrameToolStripMenuItem.Click += advanceFrameToolStripMenuItem_Click;
            // 
            // rewindFrameToolStripMenuItem
            // 
            rewindFrameToolStripMenuItem.Name = "rewindFrameToolStripMenuItem";
            rewindFrameToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            rewindFrameToolStripMenuItem.Text = "Rewind Frame";
            rewindFrameToolStripMenuItem.Click += rewindFrameToolStripMenuItem_Click;
            // 
            // seekStartToolStripMenuItem
            // 
            seekStartToolStripMenuItem.Name = "seekStartToolStripMenuItem";
            seekStartToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            seekStartToolStripMenuItem.Text = "Seek Start";
            seekStartToolStripMenuItem.Click += seekStartToolStripMenuItem_Click;
            // 
            // seekEndToolStripMenuItem
            // 
            seekEndToolStripMenuItem.Name = "seekEndToolStripMenuItem";
            seekEndToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            seekEndToolStripMenuItem.Text = "Seek End";
            seekEndToolStripMenuItem.Click += seekEndToolStripMenuItem_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(221, 6);
            // 
            // loadStarModelToolStripMenuItem
            // 
            loadStarModelToolStripMenuItem.Name = "loadStarModelToolStripMenuItem";
            loadStarModelToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            loadStarModelToolStripMenuItem.Text = "Load Star Model";
            loadStarModelToolStripMenuItem.Click += loadStarModelToolStripMenuItem_Click;
            // 
            // drawArcToolStripMenuItem
            // 
            drawArcToolStripMenuItem.Checked = true;
            drawArcToolStripMenuItem.CheckOnClick = true;
            drawArcToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            drawArcToolStripMenuItem.Name = "drawArcToolStripMenuItem";
            drawArcToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            drawArcToolStripMenuItem.Text = "Draw Arc";
            drawArcToolStripMenuItem.Click += drawArcToolStripMenuItem_Click;
            // 
            // GrToolEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1062, 673);
            Controls.Add(dockPanel);
            Controls.Add(toolStrip1);
            Name = "GrToolEditor";
            Text = "GrToolEditor";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonSave;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem loadModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton buttonSelectCollision;
        private System.Windows.Forms.ToolStripButton buttonSelectTriangle;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox collViewComboBox;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton buttonXRay;
        private System.Windows.Forms.ToolStripButton wireframeButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripDropDownButton railAnimationButton;
        private System.Windows.Forms.ToolStripMenuItem loopAnimationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advanceFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rewindFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadStarModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem seekStartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem seekEndToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem drawArcToolStripMenuItem;
    }
}