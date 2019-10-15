namespace HSDRawViewer.GUI.Plugins
{
    partial class CollDataEditor
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
            this.viewBox = new System.Windows.Forms.GroupBox();
            this.toolBox = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // viewBox
            // 
            this.viewBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewBox.Location = new System.Drawing.Point(200, 0);
            this.viewBox.Name = "viewBox";
            this.viewBox.Size = new System.Drawing.Size(309, 375);
            this.viewBox.TabIndex = 0;
            this.viewBox.TabStop = false;
            this.viewBox.Text = "Render";
            // 
            // toolBox
            // 
            this.toolBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolBox.Location = new System.Drawing.Point(0, 0);
            this.toolBox.Name = "toolBox";
            this.toolBox.Size = new System.Drawing.Size(200, 375);
            this.toolBox.TabIndex = 1;
            this.toolBox.TabStop = false;
            this.toolBox.Text = "Options";
            // 
            // CollDataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 375);
            this.Controls.Add(this.viewBox);
            this.Controls.Add(this.toolBox);
            this.Name = "CollDataEditor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox viewBox;
        private System.Windows.Forms.GroupBox toolBox;
    }
}
