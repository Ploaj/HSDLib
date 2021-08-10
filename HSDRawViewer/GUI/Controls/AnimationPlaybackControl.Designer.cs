
namespace HSDRawViewer.GUI.Controls
{
    partial class AnimationPlaybackControl
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
            this.panelPlayback = new HSDRawViewer.GUI.Controls.DrawingPanel();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonRewind = new System.Windows.Forms.Button();
            this.buttonPlayReverse = new System.Windows.Forms.Button();
            this.buttonEnd = new System.Windows.Forms.Button();
            this.buttonFastForward = new System.Windows.Forms.Button();
            this.buttonPlayForward = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tbStartTime = new System.Windows.Forms.TextBox();
            this.tbEndTime = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // panelPlayback
            // 
            this.panelPlayback.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPlayback.Location = new System.Drawing.Point(3, 30);
            this.panelPlayback.Name = "panelPlayback";
            this.panelPlayback.Size = new System.Drawing.Size(366, 60);
            this.panelPlayback.TabIndex = 0;
            this.panelPlayback.Paint += new System.Windows.Forms.PaintEventHandler(this.panelPlayback_Paint);
            this.panelPlayback.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelPlayback_MouseDown);
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(80)))), ((int)(((byte)(180)))));
            this.buttonStart.FlatAppearance.BorderSize = 0;
            this.buttonStart.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(160)))));
            this.buttonStart.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(200)))));
            this.buttonStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStart.Image = global::HSDRawViewer.Properties.Resources.pb_start;
            this.buttonStart.Location = new System.Drawing.Point(3, 96);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(56, 48);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.UseVisualStyleBackColor = false;
            // 
            // buttonRewind
            // 
            this.buttonRewind.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonRewind.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(80)))), ((int)(((byte)(180)))));
            this.buttonRewind.FlatAppearance.BorderSize = 0;
            this.buttonRewind.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(160)))));
            this.buttonRewind.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(200)))));
            this.buttonRewind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRewind.Image = global::HSDRawViewer.Properties.Resources.pb_rewind;
            this.buttonRewind.Location = new System.Drawing.Point(65, 96);
            this.buttonRewind.Name = "buttonRewind";
            this.buttonRewind.Size = new System.Drawing.Size(56, 48);
            this.buttonRewind.TabIndex = 1;
            this.buttonRewind.UseVisualStyleBackColor = false;
            // 
            // buttonPlayReverse
            // 
            this.buttonPlayReverse.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonPlayReverse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(80)))), ((int)(((byte)(180)))));
            this.buttonPlayReverse.FlatAppearance.BorderSize = 0;
            this.buttonPlayReverse.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(160)))));
            this.buttonPlayReverse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(200)))));
            this.buttonPlayReverse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPlayReverse.Image = global::HSDRawViewer.Properties.Resources.pb_play_reverse;
            this.buttonPlayReverse.Location = new System.Drawing.Point(127, 96);
            this.buttonPlayReverse.Name = "buttonPlayReverse";
            this.buttonPlayReverse.Size = new System.Drawing.Size(56, 48);
            this.buttonPlayReverse.TabIndex = 1;
            this.buttonPlayReverse.UseVisualStyleBackColor = false;
            this.buttonPlayReverse.Click += new System.EventHandler(this.buttonPlayReverse_Click);
            // 
            // buttonEnd
            // 
            this.buttonEnd.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonEnd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(80)))), ((int)(((byte)(180)))));
            this.buttonEnd.FlatAppearance.BorderSize = 0;
            this.buttonEnd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(160)))));
            this.buttonEnd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(200)))));
            this.buttonEnd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEnd.Image = global::HSDRawViewer.Properties.Resources.pb_end;
            this.buttonEnd.Location = new System.Drawing.Point(313, 96);
            this.buttonEnd.Name = "buttonEnd";
            this.buttonEnd.Size = new System.Drawing.Size(56, 48);
            this.buttonEnd.TabIndex = 1;
            this.buttonEnd.UseVisualStyleBackColor = false;
            // 
            // buttonFastForward
            // 
            this.buttonFastForward.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonFastForward.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(80)))), ((int)(((byte)(180)))));
            this.buttonFastForward.FlatAppearance.BorderSize = 0;
            this.buttonFastForward.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(160)))));
            this.buttonFastForward.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(200)))));
            this.buttonFastForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFastForward.Image = global::HSDRawViewer.Properties.Resources.pb_fastforward;
            this.buttonFastForward.Location = new System.Drawing.Point(251, 96);
            this.buttonFastForward.Name = "buttonFastForward";
            this.buttonFastForward.Size = new System.Drawing.Size(56, 48);
            this.buttonFastForward.TabIndex = 1;
            this.buttonFastForward.UseVisualStyleBackColor = false;
            // 
            // buttonPlayForward
            // 
            this.buttonPlayForward.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonPlayForward.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(80)))), ((int)(((byte)(180)))));
            this.buttonPlayForward.FlatAppearance.BorderSize = 0;
            this.buttonPlayForward.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(160)))));
            this.buttonPlayForward.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(200)))));
            this.buttonPlayForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPlayForward.Image = global::HSDRawViewer.Properties.Resources.pb_play;
            this.buttonPlayForward.Location = new System.Drawing.Point(189, 96);
            this.buttonPlayForward.Name = "buttonPlayForward";
            this.buttonPlayForward.Size = new System.Drawing.Size(56, 48);
            this.buttonPlayForward.TabIndex = 1;
            this.buttonPlayForward.UseVisualStyleBackColor = false;
            this.buttonPlayForward.Click += new System.EventHandler(this.buttonPlayForward_Click);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(80)))), ((int)(((byte)(180)))));
            this.button1.BackgroundImage = global::HSDRawViewer.Properties.Resources.pb_loop;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(160)))));
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(200)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(3, 150);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(32, 32);
            this.button1.TabIndex = 1;
            this.button1.UseVisualStyleBackColor = false;
            // 
            // tbStartTime
            // 
            this.tbStartTime.Enabled = false;
            this.tbStartTime.Location = new System.Drawing.Point(3, 4);
            this.tbStartTime.Name = "tbStartTime";
            this.tbStartTime.Size = new System.Drawing.Size(100, 20);
            this.tbStartTime.TabIndex = 2;
            // 
            // tbEndTime
            // 
            this.tbEndTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbEndTime.Enabled = false;
            this.tbEndTime.Location = new System.Drawing.Point(269, 4);
            this.tbEndTime.Name = "tbEndTime";
            this.tbEndTime.Size = new System.Drawing.Size(100, 20);
            this.tbEndTime.TabIndex = 2;
            // 
            // AnimationPlaybackControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbEndTime);
            this.Controls.Add(this.tbStartTime);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonRewind);
            this.Controls.Add(this.buttonPlayReverse);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonEnd);
            this.Controls.Add(this.buttonFastForward);
            this.Controls.Add(this.buttonPlayForward);
            this.Controls.Add(this.panelPlayback);
            this.Name = "AnimationPlaybackControl";
            this.Size = new System.Drawing.Size(373, 188);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DrawingPanel panelPlayback;
        private System.Windows.Forms.Button buttonPlayForward;
        private System.Windows.Forms.Button buttonPlayReverse;
        private System.Windows.Forms.Button buttonFastForward;
        private System.Windows.Forms.Button buttonEnd;
        private System.Windows.Forms.Button buttonRewind;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbStartTime;
        private System.Windows.Forms.TextBox tbEndTime;
    }
}
