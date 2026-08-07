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
            collViewComboBox = new System.Windows.Forms.ToolStripComboBox();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
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
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripDropDownButton1, buttonSave, toolStripSeparator2, toolStripSeparator1, toolStripLabel1, buttonSelectCollision, buttonSelectTriangle, toolStripSeparator3, toolStripLabel2, collViewComboBox });
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
            // collViewComboBox
            // 
            collViewComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            collViewComboBox.Name = "collViewComboBox";
            collViewComboBox.Size = new System.Drawing.Size(121, 28);
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
    }
}