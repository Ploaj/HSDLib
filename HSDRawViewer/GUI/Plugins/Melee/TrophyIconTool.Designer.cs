namespace HSDRawViewer.GUI.Plugins.Melee
{
    partial class TrophyIconTool
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
            arrayEditor = new ArrayMemberEditor();
            previewBox = new System.Windows.Forms.GroupBox();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            saveButton = new System.Windows.Forms.ToolStripButton();
            importButton = new System.Windows.Forms.ToolStripButton();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // arrayEditor
            // 
            arrayEditor.CanAdd = false;
            arrayEditor.CanClone = false;
            arrayEditor.DisplayItemImages = true;
            arrayEditor.DisplayItemIndices = true;
            arrayEditor.Dock = System.Windows.Forms.DockStyle.Left;
            arrayEditor.EnablePropertyViewDescription = true;
            arrayEditor.ImageHeight = (ushort)64;
            arrayEditor.ImageWidth = (ushort)64;
            arrayEditor.InsertCloneAfterSelected = false;
            arrayEditor.ItemHeight = 13;
            arrayEditor.ItemIndexOffset = 0;
            arrayEditor.Location = new System.Drawing.Point(0, 25);
            arrayEditor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            arrayEditor.Name = "arrayEditor";
            arrayEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            arrayEditor.Size = new System.Drawing.Size(243, 425);
            arrayEditor.TabIndex = 2;
            // 
            // previewBox
            // 
            previewBox.Dock = System.Windows.Forms.DockStyle.Fill;
            previewBox.Location = new System.Drawing.Point(243, 25);
            previewBox.Name = "previewBox";
            previewBox.Size = new System.Drawing.Size(557, 425);
            previewBox.TabIndex = 3;
            previewBox.TabStop = false;
            previewBox.Text = "Preview";
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { saveButton, importButton });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(800, 25);
            toolStrip1.TabIndex = 4;
            toolStrip1.Text = "toolStrip1";
            // 
            // saveButton
            // 
            saveButton.Image = Properties.Resources.ico_save;
            saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(100, 22);
            saveButton.Text = "Save Changes";
            saveButton.Click += saveButton_Click;
            // 
            // importButton
            // 
            importButton.Image = Properties.Resources.ts_importfile;
            importButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            importButton.Name = "importButton";
            importButton.Size = new System.Drawing.Size(89, 22);
            importButton.Text = "Import Icon";
            importButton.Click += importButton_Click;
            // 
            // TrophyIconTool
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(previewBox);
            Controls.Add(arrayEditor);
            Controls.Add(toolStrip1);
            Name = "TrophyIconTool";
            Text = "TrophyIconTool";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ArrayMemberEditor arrayEditor;
        private System.Windows.Forms.GroupBox previewBox;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton saveButton;
        private System.Windows.Forms.ToolStripButton importButton;
    }
}