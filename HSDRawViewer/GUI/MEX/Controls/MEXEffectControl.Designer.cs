namespace HSDRawViewer.GUI.MEX.Controls
{
    partial class MEXEffectControl
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
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.saveEffectButton = new System.Windows.Forms.ToolStripButton();
            this.effectEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip3
            // 
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveEffectButton});
            this.toolStrip3.Location = new System.Drawing.Point(0, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(720, 25);
            this.toolStrip3.TabIndex = 3;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // saveEffectButton
            // 
            this.saveEffectButton.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.saveEffectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveEffectButton.Name = "saveEffectButton";
            this.saveEffectButton.Size = new System.Drawing.Size(133, 22);
            this.saveEffectButton.Text = "Save Effect Changes";
            this.saveEffectButton.Click += new System.EventHandler(this.saveEffectButton_Click);
            // 
            // effectEditor
            // 
            this.effectEditor.DisplayItemIndices = true;
            this.effectEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.effectEditor.EnablePropertyViewDescription = true;
            this.effectEditor.ItemIndexOffset = 0;
            this.effectEditor.Location = new System.Drawing.Point(0, 25);
            this.effectEditor.Name = "effectEditor";
            this.effectEditor.Size = new System.Drawing.Size(720, 375);
            this.effectEditor.TabIndex = 4;
            this.effectEditor.ArrayUpdated += new System.EventHandler(this.effectEditor_ArrayUpdated);
            this.effectEditor.OnItemRemove += new HSDRawViewer.GUI.ArrayMemberEditor.OnItemRemoveHandler(this.effectEditor_OnItemRemove);
            // 
            // MEXEffectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.effectEditor);
            this.Controls.Add(this.toolStrip3);
            this.Name = "MEXEffectControl";
            this.Size = new System.Drawing.Size(720, 400);
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton saveEffectButton;
        private ArrayMemberEditor effectEditor;
    }
}
