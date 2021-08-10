namespace HSDRawViewer.GUI.Extra
{
    partial class FSMTool
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.animationPlaybackControl1 = new HSDRawViewer.GUI.Controls.AnimationPlaybackControl();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(775, 256);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Preview";
            // 
            // animationPlaybackControl1
            // 
            this.animationPlaybackControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationPlaybackControl1.CanEditStartAndEndTime = false;
            this.animationPlaybackControl1.EndFrame = 100F;
            this.animationPlaybackControl1.Frame = 0F;
            this.animationPlaybackControl1.FSMEnabled = true;
            this.animationPlaybackControl1.Location = new System.Drawing.Point(12, 275);
            this.animationPlaybackControl1.Name = "animationPlaybackControl1";
            this.animationPlaybackControl1.PlaybackSpeed = 1F;
            this.animationPlaybackControl1.Size = new System.Drawing.Size(775, 188);
            this.animationPlaybackControl1.StartFrame = 0F;
            this.animationPlaybackControl1.TabIndex = 1;
            // 
            // FSMTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 465);
            this.Controls.Add(this.animationPlaybackControl1);
            this.Controls.Add(this.groupBox1);
            this.Name = "FSMTool";
            this.Text = "FSMTool";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private Controls.AnimationPlaybackControl animationPlaybackControl1;
    }
}