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
            this.effectEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.SuspendLayout();
            // 
            // effectEditor
            // 
            this.effectEditor.DisplayItemIndices = true;
            this.effectEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.effectEditor.EnablePropertyViewDescription = true;
            this.effectEditor.ItemIndexOffset = 0;
            this.effectEditor.Location = new System.Drawing.Point(0, 0);
            this.effectEditor.Name = "effectEditor";
            this.effectEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.effectEditor.Size = new System.Drawing.Size(720, 400);
            this.effectEditor.TabIndex = 4;
            this.effectEditor.ArrayUpdated += new System.EventHandler(this.effectEditor_ArrayUpdated);
            this.effectEditor.OnItemRemove += new HSDRawViewer.GUI.ArrayMemberEditor.OnItemRemoveHandler(this.effectEditor_OnItemRemove);
            // 
            // MEXEffectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.effectEditor);
            this.Name = "MEXEffectControl";
            this.Size = new System.Drawing.Size(720, 400);
            this.ResumeLayout(false);

        }

        #endregion
        private ArrayMemberEditor effectEditor;
    }
}
