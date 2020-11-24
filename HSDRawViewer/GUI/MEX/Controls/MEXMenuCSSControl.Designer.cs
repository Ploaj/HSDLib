namespace HSDRawViewer.GUI.MEX.Controls
{
    partial class MEXMenuCSSControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MEXMenuCSSControl));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonEditUI = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.buttonEditAnimation = new System.Windows.Forms.Button();
            this.buttonExportIcon = new System.Windows.Forms.Button();
            this.buttonImportIcon = new System.Windows.Forms.Button();
            this.iconPreviewBox = new System.Windows.Forms.PictureBox();
            this.enableSnapAlignmentToolStripMenuItem = new System.Windows.Forms.ToolStripButton();
            this.buttonReplaceCSP = new System.Windows.Forms.Button();
            this.cssIconEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.cspArrayEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.buttonExportCSP = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconPreviewBox)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonEditUI,
            this.enableSnapAlignmentToolStripMenuItem});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(474, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonEditUI
            // 
            this.buttonEditUI.CheckOnClick = true;
            this.buttonEditUI.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonEditUI.Image = ((System.Drawing.Image)(resources.GetObject("buttonEditUI.Image")));
            this.buttonEditUI.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonEditUI.Name = "buttonEditUI";
            this.buttonEditUI.Size = new System.Drawing.Size(45, 22);
            this.buttonEditUI.Text = "Edit UI";
            this.buttonEditUI.CheckedChanged += new System.EventHandler(this.buttonEditUI_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonExportCSP);
            this.groupBox1.Controls.Add(this.buttonReplaceCSP);
            this.groupBox1.Controls.Add(this.cspArrayEditor);
            this.groupBox1.Controls.Add(this.buttonExportIcon);
            this.groupBox1.Controls.Add(this.buttonImportIcon);
            this.groupBox1.Controls.Add(this.iconPreviewBox);
            this.groupBox1.Controls.Add(this.buttonEditAnimation);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox1.Location = new System.Drawing.Point(239, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(235, 407);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CSS Slot UI";
            this.groupBox1.Visible = false;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(236, 25);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 407);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // buttonEditAnimation
            // 
            this.buttonEditAnimation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEditAnimation.Location = new System.Drawing.Point(7, 20);
            this.buttonEditAnimation.Name = "buttonEditAnimation";
            this.buttonEditAnimation.Size = new System.Drawing.Size(222, 23);
            this.buttonEditAnimation.TabIndex = 0;
            this.buttonEditAnimation.Text = "Edit Animation";
            this.buttonEditAnimation.UseVisualStyleBackColor = true;
            this.buttonEditAnimation.Click += new System.EventHandler(this.editAnimationToolStripMenuItem_Click);
            // 
            // buttonExportIcon
            // 
            this.buttonExportIcon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExportIcon.Image = global::HSDRawViewer.Properties.Resources.ts_exportfile;
            this.buttonExportIcon.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonExportIcon.Location = new System.Drawing.Point(7, 177);
            this.buttonExportIcon.Name = "buttonExportIcon";
            this.buttonExportIcon.Size = new System.Drawing.Size(222, 23);
            this.buttonExportIcon.TabIndex = 6;
            this.buttonExportIcon.Text = "Export Icon";
            this.buttonExportIcon.UseVisualStyleBackColor = true;
            this.buttonExportIcon.Click += new System.EventHandler(this.buttonExportIcon_Click);
            // 
            // buttonImportIcon
            // 
            this.buttonImportIcon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonImportIcon.Image = global::HSDRawViewer.Properties.Resources.ico_replace;
            this.buttonImportIcon.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonImportIcon.Location = new System.Drawing.Point(7, 148);
            this.buttonImportIcon.Name = "buttonImportIcon";
            this.buttonImportIcon.Size = new System.Drawing.Size(222, 23);
            this.buttonImportIcon.TabIndex = 5;
            this.buttonImportIcon.Text = "Import Icon";
            this.buttonImportIcon.UseVisualStyleBackColor = true;
            this.buttonImportIcon.Click += new System.EventHandler(this.replaceIconToolStripMenuItem_Click);
            // 
            // iconPreviewBox
            // 
            this.iconPreviewBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.iconPreviewBox.Location = new System.Drawing.Point(7, 49);
            this.iconPreviewBox.Name = "iconPreviewBox";
            this.iconPreviewBox.Size = new System.Drawing.Size(222, 93);
            this.iconPreviewBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.iconPreviewBox.TabIndex = 4;
            this.iconPreviewBox.TabStop = false;
            // 
            // enableSnapAlignmentToolStripMenuItem
            // 
            this.enableSnapAlignmentToolStripMenuItem.Checked = true;
            this.enableSnapAlignmentToolStripMenuItem.CheckOnClick = true;
            this.enableSnapAlignmentToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableSnapAlignmentToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.enableSnapAlignmentToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("enableSnapAlignmentToolStripMenuItem.Image")));
            this.enableSnapAlignmentToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.enableSnapAlignmentToolStripMenuItem.Name = "enableSnapAlignmentToolStripMenuItem";
            this.enableSnapAlignmentToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
            this.enableSnapAlignmentToolStripMenuItem.Text = "Enable Snap Align";
            // 
            // buttonReplaceCSP
            // 
            this.buttonReplaceCSP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReplaceCSP.Image = global::HSDRawViewer.Properties.Resources.ico_replace;
            this.buttonReplaceCSP.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonReplaceCSP.Location = new System.Drawing.Point(7, 353);
            this.buttonReplaceCSP.Name = "buttonReplaceCSP";
            this.buttonReplaceCSP.Size = new System.Drawing.Size(222, 23);
            this.buttonReplaceCSP.TabIndex = 8;
            this.buttonReplaceCSP.Text = "Replace Selected CSP Image";
            this.buttonReplaceCSP.UseVisualStyleBackColor = true;
            this.buttonReplaceCSP.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // cssIconEditor
            // 
            this.cssIconEditor.CanAdd = false;
            this.cssIconEditor.CanMove = false;
            this.cssIconEditor.DisplayItemImages = false;
            this.cssIconEditor.DisplayItemIndices = false;
            this.cssIconEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cssIconEditor.EnablePropertyViewDescription = true;
            this.cssIconEditor.ImageHeight = ((ushort)(24));
            this.cssIconEditor.ImageWidth = ((ushort)(24));
            this.cssIconEditor.ItemHeight = 13;
            this.cssIconEditor.ItemIndexOffset = 0;
            this.cssIconEditor.Location = new System.Drawing.Point(0, 25);
            this.cssIconEditor.Name = "cssIconEditor";
            this.cssIconEditor.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.cssIconEditor.Size = new System.Drawing.Size(236, 407);
            this.cssIconEditor.TabIndex = 1;
            this.cssIconEditor.SelectedObjectChanged += new System.EventHandler(this.cssIconEditor_SelectedObjectChanged);
            this.cssIconEditor.ArrayUpdated += new System.EventHandler(this.cssIconEditor_ArrayUpdated);
            // 
            // cspArrayEditor
            // 
            this.cspArrayEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cspArrayEditor.DisplayItemImages = true;
            this.cspArrayEditor.DisplayItemIndices = true;
            this.cspArrayEditor.EnablePropertyView = false;
            this.cspArrayEditor.EnablePropertyViewDescription = false;
            this.cspArrayEditor.ImageHeight = ((ushort)(94));
            this.cspArrayEditor.ImageWidth = ((ushort)(68));
            this.cspArrayEditor.ItemHeight = 94;
            this.cspArrayEditor.ItemIndexOffset = 0;
            this.cspArrayEditor.Location = new System.Drawing.Point(7, 206);
            this.cspArrayEditor.Name = "cspArrayEditor";
            this.cspArrayEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.cspArrayEditor.Size = new System.Drawing.Size(222, 141);
            this.cspArrayEditor.TabIndex = 7;
            this.cspArrayEditor.SelectedObjectChanged += new System.EventHandler(this.cspArrayEditor_SelectedObjectChanged);
            // 
            // buttonExportCSP
            // 
            this.buttonExportCSP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExportCSP.Image = global::HSDRawViewer.Properties.Resources.ts_exportfile;
            this.buttonExportCSP.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonExportCSP.Location = new System.Drawing.Point(7, 381);
            this.buttonExportCSP.Name = "buttonExportCSP";
            this.buttonExportCSP.Size = new System.Drawing.Size(222, 23);
            this.buttonExportCSP.TabIndex = 9;
            this.buttonExportCSP.Text = "Export Selected CSP Image";
            this.buttonExportCSP.UseVisualStyleBackColor = true;
            this.buttonExportCSP.Click += new System.EventHandler(this.buttonExportCSP_Click);
            // 
            // MEXMenuCSSControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cssIconEditor);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MEXMenuCSSControl";
            this.Size = new System.Drawing.Size(474, 432);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.iconPreviewBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public ArrayMemberEditor cssIconEditor;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonEditUI;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Button buttonEditAnimation;
        private System.Windows.Forms.Button buttonExportIcon;
        private System.Windows.Forms.Button buttonImportIcon;
        private System.Windows.Forms.PictureBox iconPreviewBox;
        private System.Windows.Forms.ToolStripButton enableSnapAlignmentToolStripMenuItem;
        private ArrayMemberEditor cspArrayEditor;
        private System.Windows.Forms.Button buttonReplaceCSP;
        private System.Windows.Forms.Button buttonExportCSP;
    }
}
