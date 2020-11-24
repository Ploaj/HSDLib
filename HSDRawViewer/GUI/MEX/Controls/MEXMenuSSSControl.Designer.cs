namespace HSDRawViewer.GUI.MEX.Controls
{
    partial class MEXMenuSSSControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MEXMenuSSSControl));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonEditUI = new System.Windows.Forms.ToolStripButton();
            this.buttonEditAnimation = new System.Windows.Forms.Button();
            this.iconPreviewBox = new System.Windows.Forms.PictureBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.namePreviewBox = new System.Windows.Forms.PictureBox();
            this.buttonImportIcon = new System.Windows.Forms.Button();
            this.buttonGenerateName = new System.Windows.Forms.Button();
            this.buttonExportIcon = new System.Windows.Forms.Button();
            this.buttonImportName = new System.Windows.Forms.Button();
            this.sssEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconPreviewBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.namePreviewBox)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonEditUI});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(376, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonImportName);
            this.groupBox1.Controls.Add(this.buttonGenerateName);
            this.groupBox1.Controls.Add(this.buttonExportIcon);
            this.groupBox1.Controls.Add(this.buttonImportIcon);
            this.groupBox1.Controls.Add(this.namePreviewBox);
            this.groupBox1.Controls.Add(this.iconPreviewBox);
            this.groupBox1.Controls.Add(this.buttonEditAnimation);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox1.Location = new System.Drawing.Point(189, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(187, 375);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stage UI Elements";
            this.groupBox1.Visible = false;
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
            // buttonEditAnimation
            // 
            this.buttonEditAnimation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEditAnimation.Location = new System.Drawing.Point(7, 20);
            this.buttonEditAnimation.Name = "buttonEditAnimation";
            this.buttonEditAnimation.Size = new System.Drawing.Size(174, 23);
            this.buttonEditAnimation.TabIndex = 0;
            this.buttonEditAnimation.Text = "Edit Animation";
            this.buttonEditAnimation.UseVisualStyleBackColor = true;
            this.buttonEditAnimation.Click += new System.EventHandler(this.regenerateAnimationToolStripMenuItem_Click);
            // 
            // iconPreviewBox
            // 
            this.iconPreviewBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.iconPreviewBox.Location = new System.Drawing.Point(7, 49);
            this.iconPreviewBox.Name = "iconPreviewBox";
            this.iconPreviewBox.Size = new System.Drawing.Size(174, 93);
            this.iconPreviewBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.iconPreviewBox.TabIndex = 1;
            this.iconPreviewBox.TabStop = false;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(186, 25);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 375);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // namePreviewBox
            // 
            this.namePreviewBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.namePreviewBox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.namePreviewBox.Location = new System.Drawing.Point(7, 206);
            this.namePreviewBox.Name = "namePreviewBox";
            this.namePreviewBox.Size = new System.Drawing.Size(174, 106);
            this.namePreviewBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.namePreviewBox.TabIndex = 2;
            this.namePreviewBox.TabStop = false;
            // 
            // buttonImportIcon
            // 
            this.buttonImportIcon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonImportIcon.Image = global::HSDRawViewer.Properties.Resources.ico_replace;
            this.buttonImportIcon.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonImportIcon.Location = new System.Drawing.Point(7, 148);
            this.buttonImportIcon.Name = "buttonImportIcon";
            this.buttonImportIcon.Size = new System.Drawing.Size(174, 23);
            this.buttonImportIcon.TabIndex = 3;
            this.buttonImportIcon.Text = "Import Icon";
            this.buttonImportIcon.UseVisualStyleBackColor = true;
            this.buttonImportIcon.Click += new System.EventHandler(this.importNewIconImageToolStripMenuItem_Click);
            // 
            // buttonGenerateName
            // 
            this.buttonGenerateName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGenerateName.Location = new System.Drawing.Point(6, 318);
            this.buttonGenerateName.Name = "buttonGenerateName";
            this.buttonGenerateName.Size = new System.Drawing.Size(175, 23);
            this.buttonGenerateName.TabIndex = 4;
            this.buttonGenerateName.Text = "Generate Name Tag";
            this.buttonGenerateName.UseVisualStyleBackColor = true;
            this.buttonGenerateName.Click += new System.EventHandler(this.generateNameTagToolStripMenuItem_Click);
            // 
            // buttonExportIcon
            // 
            this.buttonExportIcon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExportIcon.Image = global::HSDRawViewer.Properties.Resources.ts_exportfile;
            this.buttonExportIcon.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonExportIcon.Location = new System.Drawing.Point(7, 177);
            this.buttonExportIcon.Name = "buttonExportIcon";
            this.buttonExportIcon.Size = new System.Drawing.Size(174, 23);
            this.buttonExportIcon.TabIndex = 3;
            this.buttonExportIcon.Text = "Export Icon";
            this.buttonExportIcon.UseVisualStyleBackColor = true;
            this.buttonExportIcon.Click += new System.EventHandler(this.buttonExportIcon_Click);
            // 
            // buttonImportName
            // 
            this.buttonImportName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonImportName.Location = new System.Drawing.Point(7, 347);
            this.buttonImportName.Name = "buttonImportName";
            this.buttonImportName.Size = new System.Drawing.Size(175, 23);
            this.buttonImportName.TabIndex = 5;
            this.buttonImportName.Text = "Import Custom Name Tag";
            this.buttonImportName.UseVisualStyleBackColor = true;
            this.buttonImportName.Click += new System.EventHandler(this.buttonImportName_Click);
            // 
            // sssEditor
            // 
            this.sssEditor.DisplayItemImages = false;
            this.sssEditor.DisplayItemIndices = true;
            this.sssEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sssEditor.EnablePropertyViewDescription = true;
            this.sssEditor.ImageHeight = ((ushort)(24));
            this.sssEditor.ImageWidth = ((ushort)(24));
            this.sssEditor.ItemHeight = 13;
            this.sssEditor.ItemIndexOffset = 0;
            this.sssEditor.Location = new System.Drawing.Point(0, 25);
            this.sssEditor.Name = "sssEditor";
            this.sssEditor.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.sssEditor.Size = new System.Drawing.Size(186, 375);
            this.sssEditor.TabIndex = 1;
            this.sssEditor.SelectedObjectChanged += new System.EventHandler(this.sssEditor_SelectedObjectChanged);
            // 
            // MEXMenuSSSControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sssEditor);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MEXMenuSSSControl";
            this.Size = new System.Drawing.Size(376, 400);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.iconPreviewBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.namePreviewBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        public ArrayMemberEditor sssEditor;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolStripButton buttonEditUI;
        private System.Windows.Forms.Button buttonEditAnimation;
        private System.Windows.Forms.PictureBox iconPreviewBox;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.PictureBox namePreviewBox;
        private System.Windows.Forms.Button buttonGenerateName;
        private System.Windows.Forms.Button buttonImportIcon;
        private System.Windows.Forms.Button buttonImportName;
        private System.Windows.Forms.Button buttonExportIcon;
    }
}
