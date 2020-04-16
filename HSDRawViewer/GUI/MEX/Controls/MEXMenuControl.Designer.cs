namespace HSDRawViewer.GUI.MEX.Controls
{
    partial class MEXMenuControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MEXMenuControl));
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.buttonSaveCSS = new System.Windows.Forms.ToolStripButton();
            this.buttonImportMnSlcChr = new System.Windows.Forms.ToolStripButton();
            this.buttonImportMnSlMap = new System.Windows.Forms.ToolStripButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mnslmapToolStrip = new System.Windows.Forms.ToolStrip();
            this.regenerateAnimationButton = new System.Windows.Forms.ToolStripButton();
            this.importStageIconButton = new System.Windows.Forms.ToolStripButton();
            this.makeNameTagButton = new System.Windows.Forms.ToolStripButton();
            this.loadHSDCamButton = new System.Windows.Forms.ToolStripButton();
            this.mnslchrToolStrip = new System.Windows.Forms.ToolStrip();
            this.importIconButton = new System.Windows.Forms.ToolStripButton();
            this.removeIconButton = new System.Windows.Forms.ToolStripButton();
            this.cssIconTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.cssIconEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.addedIconEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.sssEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.mnslmapToolStrip.SuspendLayout();
            this.mnslchrToolStrip.SuspendLayout();
            this.cssIconTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage9.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip4
            // 
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSaveCSS,
            this.buttonImportMnSlcChr,
            this.buttonImportMnSlMap});
            this.toolStrip4.Location = new System.Drawing.Point(0, 0);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.Size = new System.Drawing.Size(720, 25);
            this.toolStrip4.TabIndex = 2;
            this.toolStrip4.Text = "toolStrip4";
            // 
            // buttonSaveCSS
            // 
            this.buttonSaveCSS.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.buttonSaveCSS.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSaveCSS.Name = "buttonSaveCSS";
            this.buttonSaveCSS.Size = new System.Drawing.Size(123, 22);
            this.buttonSaveCSS.Text = "Save CSS Changes";
            this.buttonSaveCSS.Click += new System.EventHandler(this.buttonSaveCSS_Click);
            // 
            // buttonImportMnSlcChr
            // 
            this.buttonImportMnSlcChr.Image = global::HSDRawViewer.Properties.Resources.ts_importfile;
            this.buttonImportMnSlcChr.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonImportMnSlcChr.Name = "buttonImportMnSlcChr";
            this.buttonImportMnSlcChr.Size = new System.Drawing.Size(112, 22);
            this.buttonImportMnSlcChr.Text = "Import MnSlChr";
            this.buttonImportMnSlcChr.Click += new System.EventHandler(this.buttonImportMnSlcChr_Click);
            // 
            // buttonImportMnSlMap
            // 
            this.buttonImportMnSlMap.Image = global::HSDRawViewer.Properties.Resources.ts_importfile;
            this.buttonImportMnSlMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonImportMnSlMap.Name = "buttonImportMnSlMap";
            this.buttonImportMnSlMap.Size = new System.Drawing.Size(117, 22);
            this.buttonImportMnSlMap.Text = "Import MnSlMap";
            this.buttonImportMnSlMap.Click += new System.EventHandler(this.buttonImportMnSlMap_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mnslmapToolStrip);
            this.groupBox2.Controls.Add(this.mnslchrToolStrip);
            this.groupBox2.Controls.Add(this.cssIconTabControl);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 25);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(720, 375);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Preview";
            // 
            // mnslmapToolStrip
            // 
            this.mnslmapToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.regenerateAnimationButton,
            this.importStageIconButton,
            this.makeNameTagButton,
            this.loadHSDCamButton});
            this.mnslmapToolStrip.Location = new System.Drawing.Point(203, 16);
            this.mnslmapToolStrip.Name = "mnslmapToolStrip";
            this.mnslmapToolStrip.Size = new System.Drawing.Size(514, 25);
            this.mnslmapToolStrip.TabIndex = 3;
            this.mnslmapToolStrip.Text = "toolStrip8";
            this.mnslmapToolStrip.Visible = false;
            // 
            // regenerateAnimationButton
            // 
            this.regenerateAnimationButton.Image = global::HSDRawViewer.Properties.Resources.ico_replace;
            this.regenerateAnimationButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.regenerateAnimationButton.Name = "regenerateAnimationButton";
            this.regenerateAnimationButton.Size = new System.Drawing.Size(145, 22);
            this.regenerateAnimationButton.Text = "Regenerate Animation";
            this.regenerateAnimationButton.Click += new System.EventHandler(this.regenerateAnimationButton_Click);
            // 
            // importStageIconButton
            // 
            this.importStageIconButton.Image = global::HSDRawViewer.Properties.Resources.ts_importfile;
            this.importStageIconButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.importStageIconButton.Name = "importStageIconButton";
            this.importStageIconButton.Size = new System.Drawing.Size(125, 22);
            this.importStageIconButton.Text = "Import Icon Image";
            this.importStageIconButton.Click += new System.EventHandler(this.importStageIconButton_Click);
            // 
            // makeNameTagButton
            // 
            this.makeNameTagButton.Image = global::HSDRawViewer.Properties.Resources.ts_add;
            this.makeNameTagButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.makeNameTagButton.Name = "makeNameTagButton";
            this.makeNameTagButton.Size = new System.Drawing.Size(130, 22);
            this.makeNameTagButton.Text = "Generate Name Tag";
            this.makeNameTagButton.Click += new System.EventHandler(this.makeNameTagButton_Click);
            // 
            // loadHSDCamButton
            // 
            this.loadHSDCamButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.loadHSDCamButton.Image = ((System.Drawing.Image)(resources.GetObject("loadHSDCamButton.Image")));
            this.loadHSDCamButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loadHSDCamButton.Name = "loadHSDCamButton";
            this.loadHSDCamButton.Size = new System.Drawing.Size(23, 22);
            this.loadHSDCamButton.Text = "Load HSD Camera";
            this.loadHSDCamButton.Click += new System.EventHandler(this.loadHSDCamButton_Click);
            // 
            // mnslchrToolStrip
            // 
            this.mnslchrToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importIconButton,
            this.removeIconButton});
            this.mnslchrToolStrip.Location = new System.Drawing.Point(203, 16);
            this.mnslchrToolStrip.Name = "mnslchrToolStrip";
            this.mnslchrToolStrip.Size = new System.Drawing.Size(514, 25);
            this.mnslchrToolStrip.TabIndex = 1;
            this.mnslchrToolStrip.Text = "toolStrip7";
            this.mnslchrToolStrip.Visible = false;
            // 
            // importIconButton
            // 
            this.importIconButton.Image = global::HSDRawViewer.Properties.Resources.ts_importfile;
            this.importIconButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.importIconButton.Name = "importIconButton";
            this.importIconButton.Size = new System.Drawing.Size(89, 22);
            this.importIconButton.Text = "Import Icon";
            this.importIconButton.Click += new System.EventHandler(this.importIconButton_Click);
            // 
            // removeIconButton
            // 
            this.removeIconButton.Image = global::HSDRawViewer.Properties.Resources.ts_subtract;
            this.removeIconButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeIconButton.Name = "removeIconButton";
            this.removeIconButton.Size = new System.Drawing.Size(96, 22);
            this.removeIconButton.Text = "Remove Icon";
            this.removeIconButton.Click += new System.EventHandler(this.removeIconButton_Click);
            // 
            // cssIconTabControl
            // 
            this.cssIconTabControl.Controls.Add(this.tabPage1);
            this.cssIconTabControl.Controls.Add(this.tabPage2);
            this.cssIconTabControl.Controls.Add(this.tabPage9);
            this.cssIconTabControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.cssIconTabControl.Location = new System.Drawing.Point(3, 16);
            this.cssIconTabControl.Name = "cssIconTabControl";
            this.cssIconTabControl.SelectedIndex = 0;
            this.cssIconTabControl.Size = new System.Drawing.Size(200, 356);
            this.cssIconTabControl.TabIndex = 2;
            this.cssIconTabControl.SelectedIndexChanged += new System.EventHandler(this.cssIconTabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cssIconEditor);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(192, 330);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "CSS";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // cssIconEditor
            // 
            this.cssIconEditor.DisplayItemIndices = false;
            this.cssIconEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cssIconEditor.EnablePropertyViewDescription = true;
            this.cssIconEditor.ItemIndexOffset = 0;
            this.cssIconEditor.Location = new System.Drawing.Point(3, 3);
            this.cssIconEditor.Name = "cssIconEditor";
            this.cssIconEditor.Size = new System.Drawing.Size(186, 324);
            this.cssIconEditor.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.addedIconEditor);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(192, 330);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "CSS Added";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // addedIconEditor
            // 
            this.addedIconEditor.CanAdd = false;
            this.addedIconEditor.CanClone = false;
            this.addedIconEditor.CanMove = false;
            this.addedIconEditor.CanRemove = false;
            this.addedIconEditor.DisplayItemIndices = true;
            this.addedIconEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addedIconEditor.EnablePropertyViewDescription = true;
            this.addedIconEditor.EnableToolStrip = false;
            this.addedIconEditor.ItemIndexOffset = 0;
            this.addedIconEditor.Location = new System.Drawing.Point(3, 3);
            this.addedIconEditor.Name = "addedIconEditor";
            this.addedIconEditor.Size = new System.Drawing.Size(186, 324);
            this.addedIconEditor.TabIndex = 0;
            this.addedIconEditor.Visible = false;
            this.addedIconEditor.SelectedObjectChanged += new System.EventHandler(this.addedIconEditor_SelectedObjectChanged);
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.sssEditor);
            this.tabPage9.Location = new System.Drawing.Point(4, 22);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Size = new System.Drawing.Size(192, 330);
            this.tabPage9.TabIndex = 2;
            this.tabPage9.Text = "SSS";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // sssEditor
            // 
            this.sssEditor.DisplayItemIndices = true;
            this.sssEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sssEditor.EnablePropertyViewDescription = true;
            this.sssEditor.ItemIndexOffset = 0;
            this.sssEditor.Location = new System.Drawing.Point(0, 0);
            this.sssEditor.Name = "sssEditor";
            this.sssEditor.Size = new System.Drawing.Size(192, 330);
            this.sssEditor.TabIndex = 0;
            this.sssEditor.Visible = false;
            this.sssEditor.SelectedObjectChanged += new System.EventHandler(this.sssEditor_SelectedObjectChanged);
            // 
            // MEXMenuControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.toolStrip4);
            this.Name = "MEXMenuControl";
            this.Size = new System.Drawing.Size(720, 400);
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.mnslmapToolStrip.ResumeLayout(false);
            this.mnslmapToolStrip.PerformLayout();
            this.mnslchrToolStrip.ResumeLayout(false);
            this.mnslchrToolStrip.PerformLayout();
            this.cssIconTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage9.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.ToolStripButton buttonSaveCSS;
        private System.Windows.Forms.ToolStripButton buttonImportMnSlcChr;
        private System.Windows.Forms.ToolStripButton buttonImportMnSlMap;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStrip mnslmapToolStrip;
        private System.Windows.Forms.ToolStripButton regenerateAnimationButton;
        private System.Windows.Forms.ToolStripButton importStageIconButton;
        private System.Windows.Forms.ToolStripButton makeNameTagButton;
        private System.Windows.Forms.ToolStripButton loadHSDCamButton;
        private System.Windows.Forms.ToolStrip mnslchrToolStrip;
        private System.Windows.Forms.ToolStripButton importIconButton;
        private System.Windows.Forms.ToolStripButton removeIconButton;
        private System.Windows.Forms.TabControl cssIconTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private ArrayMemberEditor cssIconEditor;
        private System.Windows.Forms.TabPage tabPage2;
        private ArrayMemberEditor addedIconEditor;
        private System.Windows.Forms.TabPage tabPage9;
        private ArrayMemberEditor sssEditor;
    }
}
