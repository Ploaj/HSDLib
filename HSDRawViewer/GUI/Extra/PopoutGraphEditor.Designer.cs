namespace HSDRawViewer.GUI.Extra
{
    partial class PopoutGraphEditor
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
            this.graphEditor = new HSDRawViewer.GUI.Controls.GraphEditor();
            this.SuspendLayout();
            // 
            // graphEditor
            // 
            this.graphEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphEditor.Location = new System.Drawing.Point(0, 0);
            this.graphEditor.Name = "graphEditor";
            this.graphEditor.Size = new System.Drawing.Size(851, 397);
            this.graphEditor.TabIndex = 0;
            // 
            // PopoutGraphEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(851, 397);
            this.Controls.Add(this.graphEditor);
            this.Name = "PopoutGraphEditor";
            this.Text = "PopoutGraphEditor";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.GraphEditor graphEditor;
    }
}