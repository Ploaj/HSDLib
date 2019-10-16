namespace HSDRawViewer.GUI.Plugins
{
    partial class JOBJEditor
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
            this.listJOBJ = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listJOBJ
            // 
            this.listJOBJ.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listJOBJ.FormattingEnabled = true;
            this.listJOBJ.Location = new System.Drawing.Point(13, 13);
            this.listJOBJ.Name = "listJOBJ";
            this.listJOBJ.Size = new System.Drawing.Size(259, 160);
            this.listJOBJ.TabIndex = 0;
            // 
            // JOBJEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.listJOBJ);
            this.Name = "JOBJEditor";
            this.TabText = "JOBJEditor";
            this.Text = "JOBJEditor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listJOBJ;
    }
}