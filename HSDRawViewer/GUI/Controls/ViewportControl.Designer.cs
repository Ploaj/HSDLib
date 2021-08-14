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
            this.panel1 = new GLControl(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 8, 16));
            this.animationGroup = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudPlaybackSpeed = new System.Windows.Forms.NumericUpDown();
            this.cbLoop = new System.Windows.Forms.CheckBox();
            this.nudMaxFrame = new System.Windows.Forms.NumericUpDown();
            this.nudFrame = new System.Windows.Forms.NumericUpDown();
            this.animationTrack = new HSDRawViewer.GUI.Controls.PlaybackBar();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toggleGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toggleBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toggleCSPModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.resetToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.screenshotButton = new System.Windows.Forms.ToolStripButton();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonRewind = new System.Windows.Forms.Button();
            this.buttonEnd = new System.Windows.Forms.Button();
            this.buttonFastForward = new System.Windows.Forms.Button();
            this.buttonPlayReverse = new System.Windows.Forms.Button();
            this.buttonPlayForward = new System.Windows.Forms.Button();
            this.animationGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlaybackSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrame)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(404, 172);
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
            this.animationGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.animationGroup.Controls.Add(this.buttonStart);
            this.animationGroup.Controls.Add(this.buttonRewind);
            this.animationGroup.Controls.Add(this.buttonEnd);
            this.animationGroup.Controls.Add(this.buttonFastForward);
            this.animationGroup.Controls.Add(this.buttonPlayReverse);
            this.animationGroup.Controls.Add(this.buttonPlayForward);
            this.animationGroup.Controls.Add(this.label1);
            this.animationGroup.Controls.Add(this.nudPlaybackSpeed);
            this.animationGroup.Controls.Add(this.cbLoop);
            this.animationGroup.Controls.Add(this.nudMaxFrame);
            this.animationGroup.Controls.Add(this.nudFrame);
            this.animationGroup.Controls.Add(this.animationTrack);
            this.animationGroup.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.animationGroup.ForeColor = System.Drawing.SystemColors.Control;
            this.animationGroup.Location = new System.Drawing.Point(0, 200);
            this.animationGroup.Name = "animationGroup";
            this.animationGroup.Size = new System.Drawing.Size(404, 141);
            this.animationGroup.TabIndex = 1;
            this.animationGroup.TabStop = false;
            this.animationGroup.Text = "Animation Track";
            this.animationGroup.Visible = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Playback FPS:";
            // 
            // nudPlaybackSpeed
            // 
            this.nudPlaybackSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudPlaybackSpeed.Location = new System.Drawing.Point(89, 69);
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
            this.nudPlaybackSpeed.Size = new System.Drawing.Size(49, 20);
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
            this.cbLoop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLoop.AutoSize = true;
            this.cbLoop.Checked = true;
            this.cbLoop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLoop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbLoop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbLoop.Location = new System.Drawing.Point(304, 70);
            this.cbLoop.Name = "cbLoop";
            this.cbLoop.Size = new System.Drawing.Size(94, 17);
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
            this.animationTrack.EndFrame = 1F;
            this.animationTrack.Frame = 0F;
            this.animationTrack.Location = new System.Drawing.Point(6, 19);
            this.animationTrack.Name = "animationTrack";
            this.animationTrack.Size = new System.Drawing.Size(325, 45);
            this.animationTrack.StartFrame = 0F;
            this.animationTrack.TabIndex = 0;
            this.animationTrack.ValueChanged += new System.EventHandler(this.animationTrack_ValueChanged);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 197);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(404, 3);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.cameraDropDown,
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
            // gridColorToolStripMenuItem
            // 
            this.gridColorToolStripMenuItem.Name = "gridColorToolStripMenuItem";
            this.gridColorToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.gridColorToolStripMenuItem.Text = "Set Grid Color";
            this.gridColorToolStripMenuItem.Click += new System.EventHandler(this.gridColorToolStripMenuItem_Click);
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
            // cameraDropDown
            // 
            this.cameraDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cameraDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolStripMenuItem1,
            this.settingsToolStripMenuItem1});
            this.cameraDropDown.Image = ((System.Drawing.Image)(resources.GetObject("cameraDropDown.Image")));
            this.cameraDropDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cameraDropDown.Name = "cameraDropDown";
            this.cameraDropDown.Size = new System.Drawing.Size(61, 22);
            this.cameraDropDown.Text = "Camera";
            // 
            // resetToolStripMenuItem1
            // 
            this.resetToolStripMenuItem1.Name = "resetToolStripMenuItem1";
            this.resetToolStripMenuItem1.Size = new System.Drawing.Size(116, 22);
            this.resetToolStripMenuItem1.Text = "Reset";
            this.resetToolStripMenuItem1.Click += new System.EventHandler(this.resetCameraButton_Click);
            // 
            // settingsToolStripMenuItem1
            // 
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            this.settingsToolStripMenuItem1.Size = new System.Drawing.Size(116, 22);
            this.settingsToolStripMenuItem1.Text = "Settings";
            this.settingsToolStripMenuItem1.Click += new System.EventHandler(this.editCameraButton_Click);
            // 
            // screenshotButton
            // 
            this.screenshotButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.screenshotButton.Image = ((System.Drawing.Image)(resources.GetObject("screenshotButton.Image")));
            this.screenshotButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.screenshotButton.Name = "screenshotButton";
            this.screenshotButton.Size = new System.Drawing.Size(95, 22);
            this.screenshotButton.Text = "Take Screenshot";
            this.screenshotButton.Click += new System.EventHandler(this.screenshotButton_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.buttonStart.FlatAppearance.BorderSize = 0;
            this.buttonStart.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(160)))));
            this.buttonStart.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(200)))));
            this.buttonStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStart.Image = global::HSDRawViewer.Properties.Resources.pb_start;
            this.buttonStart.Location = new System.Drawing.Point(6, 95);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(48, 40);
            this.buttonStart.TabIndex = 11;
            this.buttonStart.UseVisualStyleBackColor = false;
            this.buttonStart.Click += new System.EventHandler(this.buttonSeekStart_Click);
            // 
            // buttonRewind
            // 
            this.buttonRewind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRewind.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.buttonRewind.FlatAppearance.BorderSize = 0;
            this.buttonRewind.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(160)))));
            this.buttonRewind.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(200)))));
            this.buttonRewind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRewind.Image = global::HSDRawViewer.Properties.Resources.pb_rewind;
            this.buttonRewind.Location = new System.Drawing.Point(61, 95);
            this.buttonRewind.Name = "buttonRewind";
            this.buttonRewind.Size = new System.Drawing.Size(48, 40);
            this.buttonRewind.TabIndex = 12;
            this.buttonRewind.UseVisualStyleBackColor = false;
            this.buttonRewind.Click += new System.EventHandler(this.buttonPrevFrame_Click);
            // 
            // buttonEnd
            // 
            this.buttonEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEnd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.buttonEnd.FlatAppearance.BorderSize = 0;
            this.buttonEnd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(160)))));
            this.buttonEnd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(200)))));
            this.buttonEnd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEnd.Image = global::HSDRawViewer.Properties.Resources.pb_end;
            this.buttonEnd.Location = new System.Drawing.Point(350, 95);
            this.buttonEnd.Name = "buttonEnd";
            this.buttonEnd.Size = new System.Drawing.Size(48, 40);
            this.buttonEnd.TabIndex = 15;
            this.buttonEnd.UseVisualStyleBackColor = false;
            this.buttonEnd.Click += new System.EventHandler(this.buttonSeekEnd_Click);
            // 
            // buttonFastForward
            // 
            this.buttonFastForward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFastForward.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.buttonFastForward.FlatAppearance.BorderSize = 0;
            this.buttonFastForward.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(160)))));
            this.buttonFastForward.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(200)))));
            this.buttonFastForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFastForward.Image = global::HSDRawViewer.Properties.Resources.pb_fastforward;
            this.buttonFastForward.Location = new System.Drawing.Point(296, 95);
            this.buttonFastForward.Name = "buttonFastForward";
            this.buttonFastForward.Size = new System.Drawing.Size(48, 40);
            this.buttonFastForward.TabIndex = 16;
            this.buttonFastForward.UseVisualStyleBackColor = false;
            this.buttonFastForward.Click += new System.EventHandler(this.buttonNextFrame_Click);
            // 
            // buttonPlayReverse
            // 
            this.buttonPlayReverse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPlayReverse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.buttonPlayReverse.FlatAppearance.BorderSize = 0;
            this.buttonPlayReverse.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(160)))));
            this.buttonPlayReverse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(200)))));
            this.buttonPlayReverse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPlayReverse.Image = global::HSDRawViewer.Properties.Resources.pb_play_reverse;
            this.buttonPlayReverse.Location = new System.Drawing.Point(115, 95);
            this.buttonPlayReverse.Name = "buttonPlayReverse";
            this.buttonPlayReverse.Size = new System.Drawing.Size(48, 40);
            this.buttonPlayReverse.TabIndex = 17;
            this.buttonPlayReverse.UseVisualStyleBackColor = false;
            this.buttonPlayReverse.Click += new System.EventHandler(this.buttonPlayReverse_Click);
            // 
            // buttonPlayForward
            // 
            this.buttonPlayForward.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPlayForward.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.buttonPlayForward.FlatAppearance.BorderSize = 0;
            this.buttonPlayForward.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(40)))), ((int)(((byte)(160)))));
            this.buttonPlayForward.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(200)))));
            this.buttonPlayForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPlayForward.Image = global::HSDRawViewer.Properties.Resources.pb_play;
            this.buttonPlayForward.Location = new System.Drawing.Point(169, 95);
            this.buttonPlayForward.Name = "buttonPlayForward";
            this.buttonPlayForward.Size = new System.Drawing.Size(121, 40);
            this.buttonPlayForward.TabIndex = 17;
            this.buttonPlayForward.UseVisualStyleBackColor = false;
            this.buttonPlayForward.Click += new System.EventHandler(this.buttonPlay_Click);
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
        private Controls.PlaybackBar animationTrack;
        private System.Windows.Forms.NumericUpDown nudMaxFrame;
        private System.Windows.Forms.CheckBox cbLoop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudPlaybackSpeed;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem toggleGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleBackgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton screenshotButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toggleCSPModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gridColorToolStripMenuItem;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonRewind;
        private System.Windows.Forms.Button buttonEnd;
        private System.Windows.Forms.Button buttonFastForward;
        private System.Windows.Forms.Button buttonPlayForward;
        private System.Windows.Forms.Button buttonPlayReverse;
        private System.Windows.Forms.ToolStripDropDownButton cameraDropDown;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem1;
    }
}
