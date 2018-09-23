namespace Modlee
{
    partial class MaterialEditor
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonDIF = new System.Windows.Forms.Button();
            this.buttonAMB = new System.Windows.Forms.Button();
            this.buttonSPC = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(150, 144);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // buttonDIF
            // 
            this.buttonDIF.Location = new System.Drawing.Point(159, 3);
            this.buttonDIF.Name = "buttonDIF";
            this.buttonDIF.Size = new System.Drawing.Size(29, 29);
            this.buttonDIF.TabIndex = 1;
            this.buttonDIF.UseVisualStyleBackColor = true;
            // 
            // buttonAMB
            // 
            this.buttonAMB.Location = new System.Drawing.Point(194, 3);
            this.buttonAMB.Name = "buttonAMB";
            this.buttonAMB.Size = new System.Drawing.Size(29, 29);
            this.buttonAMB.TabIndex = 1;
            this.buttonAMB.UseVisualStyleBackColor = true;
            // 
            // buttonSPC
            // 
            this.buttonSPC.Location = new System.Drawing.Point(229, 3);
            this.buttonSPC.Name = "buttonSPC";
            this.buttonSPC.Size = new System.Drawing.Size(29, 29);
            this.buttonSPC.TabIndex = 1;
            this.buttonSPC.UseVisualStyleBackColor = true;
            // 
            // MaterialEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonSPC);
            this.Controls.Add(this.buttonAMB);
            this.Controls.Add(this.buttonDIF);
            this.Controls.Add(this.pictureBox1);
            this.Name = "MaterialEditor";
            this.Size = new System.Drawing.Size(335, 314);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonDIF;
        private System.Windows.Forms.Button buttonAMB;
        private System.Windows.Forms.Button buttonSPC;
    }
}
