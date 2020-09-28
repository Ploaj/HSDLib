namespace HSDRawViewer.GUI.MEX.Controls
{
    partial class MEXStockIconControl
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.arrayMemberEditor1 = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.arrayMemberEditor3 = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.replaceIcon = new System.Windows.Forms.ToolStripButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.arrayMemberEditor2 = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(644, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // saveButton
            // 
            this.saveButton.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(51, 22);
            this.saveButton.Text = "Save";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // arrayMemberEditor1
            // 
            this.arrayMemberEditor1.DisplayItemImages = true;
            this.arrayMemberEditor1.DisplayItemIndices = true;
            this.arrayMemberEditor1.Dock = System.Windows.Forms.DockStyle.Left;
            this.arrayMemberEditor1.EnablePropertyView = false;
            this.arrayMemberEditor1.EnablePropertyViewDescription = false;
            this.arrayMemberEditor1.ImageHeight = ((ushort)(24));
            this.arrayMemberEditor1.ImageWidth = ((ushort)(24));
            this.arrayMemberEditor1.ItemHeight = 24;
            this.arrayMemberEditor1.ItemIndexOffset = 0;
            this.arrayMemberEditor1.Location = new System.Drawing.Point(3, 3);
            this.arrayMemberEditor1.Name = "arrayMemberEditor1";
            this.arrayMemberEditor1.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.arrayMemberEditor1.Size = new System.Drawing.Size(233, 388);
            this.arrayMemberEditor1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(644, 420);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.arrayMemberEditor3);
            this.tabPage1.Controls.Add(this.toolStrip2);
            this.tabPage1.Controls.Add(this.arrayMemberEditor1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(636, 394);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Fighters";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // arrayMemberEditor3
            // 
            this.arrayMemberEditor3.DisplayItemImages = true;
            this.arrayMemberEditor3.DisplayItemIndices = true;
            this.arrayMemberEditor3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arrayMemberEditor3.EnablePropertyView = false;
            this.arrayMemberEditor3.EnablePropertyViewDescription = false;
            this.arrayMemberEditor3.ImageHeight = ((ushort)(24));
            this.arrayMemberEditor3.ImageWidth = ((ushort)(24));
            this.arrayMemberEditor3.ItemHeight = 24;
            this.arrayMemberEditor3.ItemIndexOffset = 0;
            this.arrayMemberEditor3.Location = new System.Drawing.Point(236, 28);
            this.arrayMemberEditor3.Name = "arrayMemberEditor3";
            this.arrayMemberEditor3.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.arrayMemberEditor3.Size = new System.Drawing.Size(397, 363);
            this.arrayMemberEditor3.TabIndex = 2;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replaceIcon});
            this.toolStrip2.Location = new System.Drawing.Point(236, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(397, 25);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // replaceIcon
            // 
            this.replaceIcon.Image = global::HSDRawViewer.Properties.Resources.ico_replace;
            this.replaceIcon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.replaceIcon.Name = "replaceIcon";
            this.replaceIcon.Size = new System.Drawing.Size(94, 22);
            this.replaceIcon.Text = "Replace Icon";
            this.replaceIcon.Click += new System.EventHandler(this.replaceIcon_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.arrayMemberEditor2);
            this.tabPage2.Controls.Add(this.toolStrip3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(636, 394);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Reserved";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // arrayMemberEditor2
            // 
            this.arrayMemberEditor2.DisplayItemImages = true;
            this.arrayMemberEditor2.DisplayItemIndices = true;
            this.arrayMemberEditor2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arrayMemberEditor2.EnablePropertyView = false;
            this.arrayMemberEditor2.EnablePropertyViewDescription = false;
            this.arrayMemberEditor2.ImageHeight = ((ushort)(24));
            this.arrayMemberEditor2.ImageWidth = ((ushort)(24));
            this.arrayMemberEditor2.ItemHeight = 24;
            this.arrayMemberEditor2.ItemIndexOffset = 0;
            this.arrayMemberEditor2.Location = new System.Drawing.Point(3, 28);
            this.arrayMemberEditor2.Name = "arrayMemberEditor2";
            this.arrayMemberEditor2.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.arrayMemberEditor2.Size = new System.Drawing.Size(630, 363);
            this.arrayMemberEditor2.TabIndex = 1;
            // 
            // toolStrip3
            // 
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip3.Location = new System.Drawing.Point(3, 3);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(630, 25);
            this.toolStrip3.TabIndex = 2;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::HSDRawViewer.Properties.Resources.ico_replace;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(94, 22);
            this.toolStripButton1.Text = "Replace Icon";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // MEXStockIconControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MEXStockIconControl";
            this.Size = new System.Drawing.Size(644, 445);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ArrayMemberEditor arrayMemberEditor1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton saveButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private ArrayMemberEditor arrayMemberEditor2;
        private ArrayMemberEditor arrayMemberEditor3;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton replaceIcon;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}
