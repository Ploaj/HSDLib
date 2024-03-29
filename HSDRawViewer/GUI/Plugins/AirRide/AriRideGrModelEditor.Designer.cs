﻿namespace HSDRawViewer.GUI.Plugins.AirRide
{
    partial class AriRideGrModelEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AriRideGrModelEditor));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.recalBoundingButton = new System.Windows.Forms.ToolStripButton();
            this.genCollButton = new System.Windows.Forms.ToolStripButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.arrayMemberEditor1 = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.recalBoundingButton,
            this.genCollButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(941, 31);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Checked = true;
            this.toolStripButton1.CheckOnClick = true;
            this.toolStripButton1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(136, 28);
            this.toolStripButton1.Text = "Draw View Ranges";
            // 
            // recalBoundingButton
            // 
            this.recalBoundingButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.recalBoundingButton.Image = ((System.Drawing.Image)(resources.GetObject("recalBoundingButton.Image")));
            this.recalBoundingButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.recalBoundingButton.Name = "recalBoundingButton";
            this.recalBoundingButton.Size = new System.Drawing.Size(204, 28);
            this.recalBoundingButton.Text = "Recalculate Model Bounding";
            this.recalBoundingButton.Click += new System.EventHandler(this.recalBoundingButton_Click);
            // 
            // genCollButton
            // 
            this.genCollButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.genCollButton.Image = ((System.Drawing.Image)(resources.GetObject("genCollButton.Image")));
            this.genCollButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.genCollButton.Name = "genCollButton";
            this.genCollButton.Size = new System.Drawing.Size(263, 28);
            this.genCollButton.Text = "Generate Collision and Partition Node";
            this.genCollButton.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 31);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(941, 559);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Size = new System.Drawing.Size(933, 530);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Model";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.arrayMemberEditor1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Size = new System.Drawing.Size(933, 530);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Bounding";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // arrayMemberEditor1
            // 
            this.arrayMemberEditor1.DisplayItemImages = false;
            this.arrayMemberEditor1.DisplayItemIndices = false;
            this.arrayMemberEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arrayMemberEditor1.EnablePropertyViewDescription = true;
            this.arrayMemberEditor1.ImageHeight = ((ushort)(24));
            this.arrayMemberEditor1.ImageWidth = ((ushort)(24));
            this.arrayMemberEditor1.ItemHeight = 13;
            this.arrayMemberEditor1.ItemIndexOffset = 0;
            this.arrayMemberEditor1.Location = new System.Drawing.Point(4, 4);
            this.arrayMemberEditor1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.arrayMemberEditor1.Name = "arrayMemberEditor1";
            this.arrayMemberEditor1.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.arrayMemberEditor1.Size = new System.Drawing.Size(925, 522);
            this.arrayMemberEditor1.TabIndex = 0;
            this.arrayMemberEditor1.SelectedObjectChanged += new System.EventHandler(this.arrayMemberEditor1_SelectedObjectChanged);
            // 
            // AriRideGrModelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(941, 590);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "AriRideGrModelEditor";
            this.TabText = "AriRideGrModelEditor";
            this.Text = "AriRideGrModelEditor";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private ArrayMemberEditor arrayMemberEditor1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton recalBoundingButton;
        private System.Windows.Forms.ToolStripButton genCollButton;
    }
}