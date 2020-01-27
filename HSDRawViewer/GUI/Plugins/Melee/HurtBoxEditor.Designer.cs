namespace HSDRawViewer.GUI.Plugins.Melee
{
    partial class HurtBoxEditor
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
            this.previewPanel = new System.Windows.Forms.GroupBox();
            this.buttonLoadModel = new System.Windows.Forms.Button();
            this.hurtboxPanel = new System.Windows.Forms.GroupBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.previewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // previewPanel
            // 
            this.previewPanel.Controls.Add(this.buttonLoadModel);
            this.previewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewPanel.Location = new System.Drawing.Point(203, 0);
            this.previewPanel.Name = "previewPanel";
            this.previewPanel.Size = new System.Drawing.Size(354, 325);
            this.previewPanel.TabIndex = 4;
            this.previewPanel.TabStop = false;
            this.previewPanel.Text = "Preview";
            // 
            // buttonLoadModel
            // 
            this.buttonLoadModel.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonLoadModel.Location = new System.Drawing.Point(3, 16);
            this.buttonLoadModel.Name = "buttonLoadModel";
            this.buttonLoadModel.Size = new System.Drawing.Size(348, 23);
            this.buttonLoadModel.TabIndex = 5;
            this.buttonLoadModel.Text = "Load Model";
            this.buttonLoadModel.UseVisualStyleBackColor = true;
            this.buttonLoadModel.Click += new System.EventHandler(this.buttonLoadModel_Click);
            // 
            // hurtboxPanel
            // 
            this.hurtboxPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.hurtboxPanel.Location = new System.Drawing.Point(0, 0);
            this.hurtboxPanel.Name = "hurtboxPanel";
            this.hurtboxPanel.Size = new System.Drawing.Size(200, 325);
            this.hurtboxPanel.TabIndex = 6;
            this.hurtboxPanel.TabStop = false;
            this.hurtboxPanel.Text = "Hurtboxes";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(200, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 325);
            this.splitter1.TabIndex = 7;
            this.splitter1.TabStop = false;
            // 
            // HurtBoxEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(557, 325);
            this.Controls.Add(this.previewPanel);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.hurtboxPanel);
            this.Name = "HurtBoxEditor";
            this.TabText = "HurtBoxEditor";
            this.Text = "HurtBoxEditor";
            this.previewPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox previewPanel;
        private System.Windows.Forms.Button buttonLoadModel;
        private System.Windows.Forms.GroupBox hurtboxPanel;
        private System.Windows.Forms.Splitter splitter1;
    }
}