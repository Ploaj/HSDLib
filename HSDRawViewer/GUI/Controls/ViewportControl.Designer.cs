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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewportControl));
            this.panel1 = new OpenTK.GLControl(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 8, 16));
            this.animationGroup = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudPlaybackSpeed = new System.Windows.Forms.NumericUpDown();
            this.cbLoop = new System.Windows.Forms.CheckBox();
            this.nudMaxFrame = new System.Windows.Forms.NumericUpDown();
            this.buttonPrevFrame = new System.Windows.Forms.Button();
            this.buttonSeekStart = new System.Windows.Forms.Button();
            this.buttonSeekEnd = new System.Windows.Forms.Button();
            this.buttonNextFrame = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.nudFrame = new System.Windows.Forms.NumericUpDown();
            this.animationTrack = new System.Windows.Forms.CustomPaintTrackBar();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toggleGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toggleBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toggleCSPModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetCameraButton = new System.Windows.Forms.ToolStripButton();
            this.editCameraButton = new System.Windows.Forms.ToolStripButton();
            this.screenshotButton = new System.Windows.Forms.ToolStripButton();
            this.gridColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animationGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlaybackSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.animationTrack)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(404, 197);
            this.panel1.TabIndex = 0;
            this.panel1.VSync = false;
            this.panel1.Load += new System.EventHandler(this.panel1_Load);
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.panel1_KeyDown);
            this.panel1.MouseEnter += new System.EventHandler(this.panel1_MouseEnter);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // animationGroup
            // 
            this.animationGroup.Controls.Add(this.label1);
            this.animationGroup.Controls.Add(this.nudPlaybackSpeed);
            this.animationGroup.Controls.Add(this.cbLoop);
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
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(201, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Playback FPS:";
            // 
            // nudPlaybackSpeed
            // 
            this.nudPlaybackSpeed.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudPlaybackSpeed.Location = new System.Drawing.Point(115, 47);
            this.nudPlaybackSpeed.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.nudPlaybackSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPlaybackSpeed.Name = "nudPlaybackSpeed";
            this.nudPlaybackSpeed.Size = new System.Drawing.Size(80, 20);
            this.nudPlaybackSpeed.TabIndex = 9;
            this.nudPlaybackSpeed.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nudPlaybackSpeed.ValueChanged += new System.EventHandler(this.nudPlaybackSpeed_ValueChanged);
            // 
            // cbLoop
            // 
            this.cbLoop.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cbLoop.AutoSize = true;
            this.cbLoop.Checked = true;
            this.cbLoop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLoop.Location = new System.Drawing.Point(12, 47);
            this.cbLoop.Name = "cbLoop";
            this.cbLoop.Size = new System.Drawing.Size(97, 17);
            this.cbLoop.TabIndex = 8;
            this.cbLoop.Text = "Loop Playback";
            this.cbLoop.UseVisualStyleBackColor = true;
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
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.resetCameraButton,
            this.editCameraButton,
            this.screenshotButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(404, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleGridToolStripMenuItem,
            this.gridColorToolStripMenuItem,
            this.toolStripSeparator1,
            this.toggleBackgroundToolStripMenuItem,
            this.backgroundColorToolStripMenuItem,
            this.toolStripSeparator2,
            this.toggleCSPModeToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(45, 22);
            this.toolStripDropDownButton1.Text = "View";
            // 
            // toggleGridToolStripMenuItem
            // 
            this.toggleGridToolStripMenuItem.Name = "toggleGridToolStripMenuItem";
            this.toggleGridToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.toggleGridToolStripMenuItem.Text = "Toggle Grid";
            this.toggleGridToolStripMenuItem.Click += new System.EventHandler(this.toggleGridToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(186, 6);
            // 
            // toggleBackgroundToolStripMenuItem
            // 
            this.toggleBackgroundToolStripMenuItem.Name = "toggleBackgroundToolStripMenuItem";
            this.toggleBackgroundToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.toggleBackgroundToolStripMenuItem.Text = "Toggle Background";
            this.toggleBackgroundToolStripMenuItem.Click += new System.EventHandler(this.toggleBackgroundToolStripMenuItem_Click);
            // 
            // backgroundColorToolStripMenuItem
            // 
            this.backgroundColorToolStripMenuItem.Name = "backgroundColorToolStripMenuItem";
            this.backgroundColorToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.backgroundColorToolStripMenuItem.Text = "Set Background Color";
            this.backgroundColorToolStripMenuItem.Click += new System.EventHandler(this.backgroundColorToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(186, 6);
            // 
            // toggleCSPModeToolStripMenuItem
            // 
            this.toggleCSPModeToolStripMenuItem.Name = "toggleCSPModeToolStripMenuItem";
            this.toggleCSPModeToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.toggleCSPModeToolStripMenuItem.Text = "Toggle CSP Mode";
            this.toggleCSPModeToolStripMenuItem.Click += new System.EventHandler(this.toggleCSPModeToolStripMenuItem_Click);
            // 
            // resetCameraButton
            // 
            this.resetCameraButton.Image = ((System.Drawing.Image)(resources.GetObject("resetCameraButton.Image")));
            this.resetCameraButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.resetCameraButton.Name = "resetCameraButton";
            this.resetCameraButton.Size = new System.Drawing.Size(99, 22);
            this.resetCameraButton.Text = "Reset Camera";
            this.resetCameraButton.Click += new System.EventHandler(this.resetCameraButton_Click);
            // 
            // editCameraButton
            // 
            this.editCameraButton.Image = ((System.Drawing.Image)(resources.GetObject("editCameraButton.Image")));
            this.editCameraButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editCameraButton.Name = "editCameraButton";
            this.editCameraButton.Size = new System.Drawing.Size(113, 22);
            this.editCameraButton.Text = "Camera Settings";
            this.editCameraButton.Click += new System.EventHandler(this.editCameraButton_Click);
            // 
            // screenshotButton
            // 
            this.screenshotButton.Image = ((System.Drawing.Image)(resources.GetObject("screenshotButton.Image")));
            this.screenshotButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.screenshotButton.Name = "screenshotButton";
            this.screenshotButton.Size = new System.Drawing.Size(111, 22);
            this.screenshotButton.Text = "Take Screenshot";
            this.screenshotButton.Click += new System.EventHandler(this.screenshotButton_Click);
            // 
            // gridColorToolStripMenuItem
            // 
            this.gridColorToolStripMenuItem.Name = "gridColorToolStripMenuItem";
            this.gridColorToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.gridColorToolStripMenuItem.Text = "Set Grid Color";
            this.gridColorToolStripMenuItem.Click += new System.EventHandler(this.gridColorToolStripMenuItem_Click);
            // 
            // ViewportControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.animationGroup);
            this.Name = "ViewportControl";
            this.Size = new System.Drawing.Size(404, 341);
            this.animationGroup.ResumeLayout(false);
            this.animationGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlaybackSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.animationTrack)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.CheckBox cbLoop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudPlaybackSpeed;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton resetCameraButton;
        private System.Windows.Forms.ToolStripButton editCameraButton;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem toggleGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleBackgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton screenshotButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toggleCSPModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gridColorToolStripMenuItem;
    }
}
