using OpenTK;
using OpenTK.Graphics;

namespace HSDRawViewer.GUI
{
    partial class ViewportControl
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
            this.panel1 = new OpenTK.GLControl(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 8, 16));
            this.animationGroup = new System.Windows.Forms.GroupBox();
            this.nudMaxFrame = new System.Windows.Forms.NumericUpDown();
            this.buttonPrevFrame = new System.Windows.Forms.Button();
            this.buttonSeekStart = new System.Windows.Forms.Button();
            this.buttonSeekEnd = new System.Windows.Forms.Button();
            this.buttonNextFrame = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.nudFrame = new System.Windows.Forms.NumericUpDown();
            this.animationTrack = new System.Windows.Forms.CustomPaintTrackBar();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.animationGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.animationTrack)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(404, 222);
            this.panel1.TabIndex = 0;
            this.panel1.VSync = false;
            this.panel1.Load += new System.EventHandler(this.panel1_Load);
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.panel1_KeyDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // animationGroup
            // 
            this.animationGroup.Controls.Add(this.nudMaxFrame);
            this.animationGroup.Controls.Add(this.buttonPrevFrame);
            this.animationGroup.Controls.Add(this.buttonSeekStart);
            this.animationGroup.Controls.Add(this.buttonSeekEnd);
            this.animationGroup.Controls.Add(this.buttonNextFrame);
            this.animationGroup.Controls.Add(this.buttonPlay);
            this.animationGroup.Controls.Add(this.nudFrame);
            this.animationGroup.Controls.Add(this.animationTrack);
            this.animationGroup.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.animationGroup.Location = new System.Drawing.Point(0, 225);
            this.animationGroup.Name = "animationGroup";
            this.animationGroup.Size = new System.Drawing.Size(404, 116);
            this.animationGroup.TabIndex = 1;
            this.animationGroup.TabStop = false;
            this.animationGroup.Text = "Animation Track";
            this.animationGroup.Visible = false;
            // 
            // nudMaxFrame
            // 
            this.nudMaxFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudMaxFrame.Enabled = false;
            this.nudMaxFrame.Location = new System.Drawing.Point(337, 44);
            this.nudMaxFrame.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nudMaxFrame.Name = "nudMaxFrame";
            this.nudMaxFrame.Size = new System.Drawing.Size(61, 20);
            this.nudMaxFrame.TabIndex = 7;
            // 
            // buttonPrevFrame
            // 
            this.buttonPrevFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPrevFrame.Location = new System.Drawing.Point(52, 70);
            this.buttonPrevFrame.Name = "buttonPrevFrame";
            this.buttonPrevFrame.Size = new System.Drawing.Size(40, 36);
            this.buttonPrevFrame.TabIndex = 6;
            this.buttonPrevFrame.Text = "<";
            this.buttonPrevFrame.UseVisualStyleBackColor = true;
            this.buttonPrevFrame.Click += new System.EventHandler(this.buttonPrevFrame_Click);
            // 
            // buttonSeekStart
            // 
            this.buttonSeekStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSeekStart.Location = new System.Drawing.Point(6, 70);
            this.buttonSeekStart.Name = "buttonSeekStart";
            this.buttonSeekStart.Size = new System.Drawing.Size(40, 36);
            this.buttonSeekStart.TabIndex = 5;
            this.buttonSeekStart.Text = "<<";
            this.buttonSeekStart.UseVisualStyleBackColor = true;
            this.buttonSeekStart.Click += new System.EventHandler(this.buttonSeekStart_Click);
            // 
            // buttonSeekEnd
            // 
            this.buttonSeekEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSeekEnd.Location = new System.Drawing.Point(358, 70);
            this.buttonSeekEnd.Name = "buttonSeekEnd";
            this.buttonSeekEnd.Size = new System.Drawing.Size(40, 36);
            this.buttonSeekEnd.TabIndex = 4;
            this.buttonSeekEnd.Text = ">>";
            this.buttonSeekEnd.UseVisualStyleBackColor = true;
            this.buttonSeekEnd.Click += new System.EventHandler(this.buttonSeekEnd_Click);
            // 
            // buttonNextFrame
            // 
            this.buttonNextFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNextFrame.Location = new System.Drawing.Point(312, 70);
            this.buttonNextFrame.Name = "buttonNextFrame";
            this.buttonNextFrame.Size = new System.Drawing.Size(40, 36);
            this.buttonNextFrame.TabIndex = 3;
            this.buttonNextFrame.Text = ">";
            this.buttonNextFrame.UseVisualStyleBackColor = true;
            this.buttonNextFrame.Click += new System.EventHandler(this.buttonNextFrame_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPlay.Location = new System.Drawing.Point(98, 70);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(208, 36);
            this.buttonPlay.TabIndex = 2;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // nudFrame
            // 
            this.nudFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudFrame.Location = new System.Drawing.Point(337, 18);
            this.nudFrame.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudFrame.Name = "nudFrame";
            this.nudFrame.Size = new System.Drawing.Size(61, 20);
            this.nudFrame.TabIndex = 1;
            this.nudFrame.ValueChanged += new System.EventHandler(this.nudFrame_ValueChanged);
            // 
            // animationTrack
            // 
            this.animationTrack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationTrack.Location = new System.Drawing.Point(6, 19);
            this.animationTrack.Maximum = 1;
            this.animationTrack.Name = "animationTrack";
            this.animationTrack.Size = new System.Drawing.Size(325, 45);
            this.animationTrack.TabIndex = 0;
            this.animationTrack.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.animationTrack.ValueChanged += new System.EventHandler(this.animationTrack_ValueChanged);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 222);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(404, 3);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // ViewportControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.animationGroup);
            this.Name = "ViewportControl";
            this.Size = new System.Drawing.Size(404, 341);
            this.animationGroup.ResumeLayout(false);
            this.animationGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.animationTrack)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private OpenTK.GLControl panel1;
        private System.Windows.Forms.GroupBox animationGroup;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.NumericUpDown nudFrame;
        private System.Windows.Forms.CustomPaintTrackBar animationTrack;
        private System.Windows.Forms.Button buttonPrevFrame;
        private System.Windows.Forms.Button buttonSeekStart;
        private System.Windows.Forms.Button buttonSeekEnd;
        private System.Windows.Forms.Button buttonNextFrame;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.NumericUpDown nudMaxFrame;
    }
}
