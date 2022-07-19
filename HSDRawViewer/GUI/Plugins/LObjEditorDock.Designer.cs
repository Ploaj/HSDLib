namespace HSDRawViewer.GUI.Plugins
{
    partial class LObjEditorDock
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
            this.lObjEditor1 = new HSDRawViewer.GUI.Controls.LObjEditor();
            this.SuspendLayout();
            // 
            // lObjEditor1
            // 
            this.lObjEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lObjEditor1.Location = new System.Drawing.Point(0, 0);
            this.lObjEditor1.Name = "lObjEditor1";
            this.lObjEditor1.Size = new System.Drawing.Size(684, 431);
            this.lObjEditor1.TabIndex = 0;
            // 
            // LObjEditorDock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 431);
            this.Controls.Add(this.lObjEditor1);
            this.Name = "LObjEditorDock";
            this.Text = "LObjEditorDock";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.LObjEditor lObjEditor1;
    }
}