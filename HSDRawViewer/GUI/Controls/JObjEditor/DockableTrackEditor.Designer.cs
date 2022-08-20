namespace HSDRawViewer.GUI.Controls.JObjEditor
{
    partial class DockableTrackEditor
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
            this.graphEditor1 = new HSDRawViewer.GUI.Controls.GraphEditor();
            this.SuspendLayout();
            // 
            // graphEditor1
            // 
            this.graphEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphEditor1.Location = new System.Drawing.Point(0, 0);
            this.graphEditor1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.graphEditor1.Name = "graphEditor1";
            this.graphEditor1.Size = new System.Drawing.Size(600, 435);
            this.graphEditor1.TabIndex = 0;
            // 
            // DockableTrackEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.graphEditor1);
            this.Name = "DockableTrackEditor";
            this.Size = new System.Drawing.Size(600, 435);
            this.ResumeLayout(false);

        }

        #endregion

        private GraphEditor graphEditor1;
    }
}
