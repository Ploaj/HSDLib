namespace HSDRawViewer.GUI.MEX.Controls
{
    partial class MEXStageControl
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
            this.toolStrip7 = new System.Windows.Forms.ToolStrip();
            this.importStageButton = new System.Windows.Forms.ToolStripButton();
            this.exportStageButton = new System.Windows.Forms.ToolStripButton();
            this.removeStageButton = new System.Windows.Forms.ToolStripButton();
            this.mapGOBJCopyButton = new System.Windows.Forms.ToolStripButton();
            this.stageTabControl = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.stageEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.stageIDEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip7.SuspendLayout();
            this.stageTabControl.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip7
            // 
            this.toolStrip7.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importStageButton,
            this.exportStageButton,
            this.removeStageButton,
            this.mapGOBJCopyButton});
            this.toolStrip7.Location = new System.Drawing.Point(0, 0);
            this.toolStrip7.Name = "toolStrip7";
            this.toolStrip7.Size = new System.Drawing.Size(720, 25);
            this.toolStrip7.TabIndex = 1;
            this.toolStrip7.Text = "toolStrip7";
            // 
            // importStageButton
            // 
            this.importStageButton.Image = global::HSDRawViewer.Properties.Resources.ts_importfile;
            this.importStageButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.importStageButton.Name = "importStageButton";
            this.importStageButton.Size = new System.Drawing.Size(95, 22);
            this.importStageButton.Text = "Import Stage";
            this.importStageButton.Click += new System.EventHandler(this.importStageButton_Click);
            // 
            // exportStageButton
            // 
            this.exportStageButton.Image = global::HSDRawViewer.Properties.Resources.ts_exportfile;
            this.exportStageButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exportStageButton.Name = "exportStageButton";
            this.exportStageButton.Size = new System.Drawing.Size(93, 22);
            this.exportStageButton.Text = "Export Stage";
            this.exportStageButton.Click += new System.EventHandler(this.exportStageButton_Click);
            // 
            // removeStageButton
            // 
            this.removeStageButton.Image = global::HSDRawViewer.Properties.Resources.ts_subtract;
            this.removeStageButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeStageButton.Name = "removeStageButton";
            this.removeStageButton.Size = new System.Drawing.Size(102, 22);
            this.removeStageButton.Text = "Remove Stage";
            this.removeStageButton.Click += new System.EventHandler(this.removeStageButton_Click);
            // 
            // mapGOBJCopyButton
            // 
            this.mapGOBJCopyButton.Image = global::HSDRawViewer.Properties.Resources.ts_clone;
            this.mapGOBJCopyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mapGOBJCopyButton.Name = "mapGOBJCopyButton";
            this.mapGOBJCopyButton.Size = new System.Drawing.Size(187, 22);
            this.mapGOBJCopyButton.Text = "Copy Map GOBJs to Clipboard";
            this.mapGOBJCopyButton.Click += new System.EventHandler(this.mapGOBJCopyButton_Click);
            // 
            // stageTabControl
            // 
            this.stageTabControl.Controls.Add(this.tabPage5);
            this.stageTabControl.Controls.Add(this.tabPage6);
            this.stageTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stageTabControl.Location = new System.Drawing.Point(0, 25);
            this.stageTabControl.Name = "stageTabControl";
            this.stageTabControl.SelectedIndex = 0;
            this.stageTabControl.Size = new System.Drawing.Size(720, 375);
            this.stageTabControl.TabIndex = 3;
            this.stageTabControl.SelectedIndexChanged += new System.EventHandler(this.stageTabControl_SelectedIndexChanged);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.stageEditor);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(712, 349);
            this.tabPage5.TabIndex = 0;
            this.tabPage5.Text = "Stages";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // stageEditor
            // 
            this.stageEditor.CanRemove = false;
            this.stageEditor.DisplayItemIndices = true;
            this.stageEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stageEditor.EnablePropertyViewDescription = true;
            this.stageEditor.ItemIndexOffset = 0;
            this.stageEditor.Location = new System.Drawing.Point(3, 3);
            this.stageEditor.Name = "stageEditor";
            this.stageEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.stageEditor.Size = new System.Drawing.Size(706, 343);
            this.stageEditor.TabIndex = 1;
            this.stageEditor.ArrayUpdated += new System.EventHandler(this.stageEditor_ArrayUpdated);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.stageIDEditor);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(712, 349);
            this.tabPage6.TabIndex = 1;
            this.tabPage6.Text = "StageIDs";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // stageIDEditor
            // 
            this.stageIDEditor.DisplayItemIndices = true;
            this.stageIDEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stageIDEditor.EnablePropertyViewDescription = true;
            this.stageIDEditor.ItemIndexOffset = 0;
            this.stageIDEditor.Location = new System.Drawing.Point(3, 3);
            this.stageIDEditor.Name = "stageIDEditor";
            this.stageIDEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.stageIDEditor.Size = new System.Drawing.Size(706, 343);
            this.stageIDEditor.TabIndex = 0;
            this.stageIDEditor.ArrayUpdated += new System.EventHandler(this.stageIDEditor_ArrayUpdated);
            // 
            // MEXStageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stageTabControl);
            this.Controls.Add(this.toolStrip7);
            this.Name = "MEXStageControl";
            this.Size = new System.Drawing.Size(720, 400);
            this.toolStrip7.ResumeLayout(false);
            this.toolStrip7.PerformLayout();
            this.stageTabControl.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip7;
        private System.Windows.Forms.ToolStripButton mapGOBJCopyButton;
        private System.Windows.Forms.TabControl stageTabControl;
        private System.Windows.Forms.TabPage tabPage5;
        private ArrayMemberEditor stageEditor;
        private System.Windows.Forms.TabPage tabPage6;
        private ArrayMemberEditor stageIDEditor;
        private System.Windows.Forms.ToolStripButton exportStageButton;
        private System.Windows.Forms.ToolStripButton importStageButton;
        private System.Windows.Forms.ToolStripButton removeStageButton;
    }
}
