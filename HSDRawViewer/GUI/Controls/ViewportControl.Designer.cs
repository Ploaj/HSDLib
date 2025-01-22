using OpenTK;
using OpenTK.Graphics;
using OpenTK.WinForms;

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
            glControl = new GLControl(new GLControlSettings() { NumberOfSamples = 8 });
            animationGroup = new System.Windows.Forms.Panel();
            label2 = new System.Windows.Forms.Label();
            buttonStart = new System.Windows.Forms.Button();
            buttonRewind = new System.Windows.Forms.Button();
            buttonEnd = new System.Windows.Forms.Button();
            buttonFastForward = new System.Windows.Forms.Button();
            buttonPlayReverse = new System.Windows.Forms.Button();
            buttonPlayForward = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            nudPlaybackSpeed = new System.Windows.Forms.NumericUpDown();
            cbLoop = new System.Windows.Forms.CheckBox();
            nudMaxFrame = new System.Windows.Forms.NumericUpDown();
            nudFrame = new System.Windows.Forms.NumericUpDown();
            animationTrack = new Controls.PlaybackBar();
            splitter1 = new System.Windows.Forms.Splitter();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            toggleGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            gridColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toggleBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            backgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toggleCSPModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            cameraDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            resetToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            exportFrameAsPNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exportFrameToFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            asGIFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            screenshotButton = new System.Windows.Forms.ToolStripButton();
            animationGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudPlaybackSpeed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxFrame).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudFrame).BeginInit();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // glControl
            // 
            glControl.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            glControl.APIVersion = new System.Version(3, 3, 0, 0);
            glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            glControl.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            glControl.IsEventDriven = true;
            glControl.Location = new System.Drawing.Point(0, 27);
            glControl.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            glControl.Name = "glControl";
            glControl.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
            glControl.Size = new System.Drawing.Size(539, 348);
            glControl.TabIndex = 2;
            glControl.Text = "glControl1";
            glControl.Load += panel1_Load;
            glControl.Paint += panel1_Paint;
            glControl.KeyDown += panel1_KeyDown;
            glControl.MouseEnter += panel1_MouseEnter;
            glControl.Resize += panel1_Resize;
            // 
            // animationGroup
            // 
            animationGroup.BackColor = System.Drawing.Color.FromArgb(30, 30, 40);
            animationGroup.Controls.Add(label2);
            animationGroup.Controls.Add(buttonStart);
            animationGroup.Controls.Add(buttonRewind);
            animationGroup.Controls.Add(buttonEnd);
            animationGroup.Controls.Add(buttonFastForward);
            animationGroup.Controls.Add(buttonPlayReverse);
            animationGroup.Controls.Add(buttonPlayForward);
            animationGroup.Controls.Add(label1);
            animationGroup.Controls.Add(nudPlaybackSpeed);
            animationGroup.Controls.Add(cbLoop);
            animationGroup.Controls.Add(nudMaxFrame);
            animationGroup.Controls.Add(nudFrame);
            animationGroup.Controls.Add(animationTrack);
            animationGroup.Dock = System.Windows.Forms.DockStyle.Bottom;
            animationGroup.ForeColor = System.Drawing.SystemColors.Control;
            animationGroup.Location = new System.Drawing.Point(0, 380);
            animationGroup.Margin = new System.Windows.Forms.Padding(5);
            animationGroup.Name = "animationGroup";
            animationGroup.Padding = new System.Windows.Forms.Padding(5);
            animationGroup.Size = new System.Drawing.Size(539, 145);
            animationGroup.TabIndex = 1;
            animationGroup.Visible = false;
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label2.Location = new System.Drawing.Point(425, 55);
            label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(20, 28);
            label2.TabIndex = 18;
            label2.Text = "/";
            // 
            // buttonStart
            // 
            buttonStart.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonStart.BackColor = System.Drawing.Color.FromArgb(60, 60, 70);
            buttonStart.FlatAppearance.BorderSize = 0;
            buttonStart.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 40, 160);
            buttonStart.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 100, 200);
            buttonStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonStart.Image = Properties.Resources.pb_start;
            buttonStart.Location = new System.Drawing.Point(8, 91);
            buttonStart.Margin = new System.Windows.Forms.Padding(5);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new System.Drawing.Size(64, 45);
            buttonStart.TabIndex = 11;
            buttonStart.UseVisualStyleBackColor = false;
            buttonStart.Click += buttonSeekStart_Click;
            // 
            // buttonRewind
            // 
            buttonRewind.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonRewind.BackColor = System.Drawing.Color.FromArgb(60, 60, 70);
            buttonRewind.FlatAppearance.BorderSize = 0;
            buttonRewind.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 40, 160);
            buttonRewind.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 100, 200);
            buttonRewind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonRewind.Image = Properties.Resources.pb_rewind;
            buttonRewind.Location = new System.Drawing.Point(81, 91);
            buttonRewind.Margin = new System.Windows.Forms.Padding(5);
            buttonRewind.Name = "buttonRewind";
            buttonRewind.Size = new System.Drawing.Size(64, 45);
            buttonRewind.TabIndex = 12;
            buttonRewind.UseVisualStyleBackColor = false;
            buttonRewind.Click += buttonPrevFrame_Click;
            // 
            // buttonEnd
            // 
            buttonEnd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonEnd.BackColor = System.Drawing.Color.FromArgb(60, 60, 70);
            buttonEnd.FlatAppearance.BorderSize = 0;
            buttonEnd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 40, 160);
            buttonEnd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 100, 200);
            buttonEnd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonEnd.Image = Properties.Resources.pb_end;
            buttonEnd.Location = new System.Drawing.Point(467, 91);
            buttonEnd.Margin = new System.Windows.Forms.Padding(5);
            buttonEnd.Name = "buttonEnd";
            buttonEnd.Size = new System.Drawing.Size(64, 45);
            buttonEnd.TabIndex = 15;
            buttonEnd.UseVisualStyleBackColor = false;
            buttonEnd.Click += buttonSeekEnd_Click;
            // 
            // buttonFastForward
            // 
            buttonFastForward.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonFastForward.BackColor = System.Drawing.Color.FromArgb(60, 60, 70);
            buttonFastForward.FlatAppearance.BorderSize = 0;
            buttonFastForward.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 40, 160);
            buttonFastForward.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 100, 200);
            buttonFastForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonFastForward.Image = Properties.Resources.pb_fastforward;
            buttonFastForward.Location = new System.Drawing.Point(395, 91);
            buttonFastForward.Margin = new System.Windows.Forms.Padding(5);
            buttonFastForward.Name = "buttonFastForward";
            buttonFastForward.Size = new System.Drawing.Size(64, 45);
            buttonFastForward.TabIndex = 16;
            buttonFastForward.UseVisualStyleBackColor = false;
            buttonFastForward.Click += buttonNextFrame_Click;
            // 
            // buttonPlayReverse
            // 
            buttonPlayReverse.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonPlayReverse.BackColor = System.Drawing.Color.FromArgb(60, 60, 70);
            buttonPlayReverse.FlatAppearance.BorderSize = 0;
            buttonPlayReverse.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 40, 160);
            buttonPlayReverse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 100, 200);
            buttonPlayReverse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonPlayReverse.Image = Properties.Resources.pb_play_reverse;
            buttonPlayReverse.Location = new System.Drawing.Point(153, 91);
            buttonPlayReverse.Margin = new System.Windows.Forms.Padding(5);
            buttonPlayReverse.Name = "buttonPlayReverse";
            buttonPlayReverse.Size = new System.Drawing.Size(64, 45);
            buttonPlayReverse.TabIndex = 17;
            buttonPlayReverse.UseVisualStyleBackColor = false;
            buttonPlayReverse.Click += buttonPlayReverse_Click;
            // 
            // buttonPlayForward
            // 
            buttonPlayForward.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            buttonPlayForward.BackColor = System.Drawing.Color.FromArgb(60, 60, 70);
            buttonPlayForward.FlatAppearance.BorderSize = 0;
            buttonPlayForward.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 40, 160);
            buttonPlayForward.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 100, 200);
            buttonPlayForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonPlayForward.Image = Properties.Resources.pb_play;
            buttonPlayForward.Location = new System.Drawing.Point(225, 91);
            buttonPlayForward.Margin = new System.Windows.Forms.Padding(5);
            buttonPlayForward.Name = "buttonPlayForward";
            buttonPlayForward.Size = new System.Drawing.Size(161, 45);
            buttonPlayForward.TabIndex = 17;
            buttonPlayForward.UseVisualStyleBackColor = false;
            buttonPlayForward.Click += buttonPlay_Click;
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(8, 57);
            label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(97, 20);
            label1.TabIndex = 10;
            label1.Text = "Playback FPS:";
            // 
            // nudPlaybackSpeed
            // 
            nudPlaybackSpeed.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            nudPlaybackSpeed.Location = new System.Drawing.Point(107, 55);
            nudPlaybackSpeed.Margin = new System.Windows.Forms.Padding(5);
            nudPlaybackSpeed.Maximum = new decimal(new int[] { 120, 0, 0, 0 });
            nudPlaybackSpeed.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudPlaybackSpeed.Name = "nudPlaybackSpeed";
            nudPlaybackSpeed.Size = new System.Drawing.Size(65, 27);
            nudPlaybackSpeed.TabIndex = 9;
            nudPlaybackSpeed.Value = new decimal(new int[] { 60, 0, 0, 0 });
            nudPlaybackSpeed.ValueChanged += nudPlaybackSpeed_ValueChanged;
            // 
            // cbLoop
            // 
            cbLoop.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            cbLoop.AutoSize = true;
            cbLoop.Checked = true;
            cbLoop.CheckState = System.Windows.Forms.CheckState.Checked;
            cbLoop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cbLoop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            cbLoop.Location = new System.Drawing.Point(182, 59);
            cbLoop.Margin = new System.Windows.Forms.Padding(5);
            cbLoop.Name = "cbLoop";
            cbLoop.Size = new System.Drawing.Size(123, 24);
            cbLoop.TabIndex = 8;
            cbLoop.Text = "Loop Playback";
            cbLoop.UseVisualStyleBackColor = true;
            // 
            // nudMaxFrame
            // 
            nudMaxFrame.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            nudMaxFrame.Location = new System.Drawing.Point(450, 55);
            nudMaxFrame.Margin = new System.Windows.Forms.Padding(5);
            nudMaxFrame.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            nudMaxFrame.Name = "nudMaxFrame";
            nudMaxFrame.Size = new System.Drawing.Size(81, 27);
            nudMaxFrame.TabIndex = 7;
            nudMaxFrame.ValueChanged += nudMaxFrame_ValueChanged;
            // 
            // nudFrame
            // 
            nudFrame.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            nudFrame.Location = new System.Drawing.Point(338, 55);
            nudFrame.Margin = new System.Windows.Forms.Padding(5);
            nudFrame.Maximum = new decimal(new int[] { 99999, 0, 0, 0 });
            nudFrame.Name = "nudFrame";
            nudFrame.Size = new System.Drawing.Size(81, 27);
            nudFrame.TabIndex = 1;
            nudFrame.ValueChanged += nudFrame_ValueChanged;
            // 
            // animationTrack
            // 
            animationTrack.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            animationTrack.EndFrame = 1F;
            animationTrack.Frame = 0F;
            animationTrack.Location = new System.Drawing.Point(9, 12);
            animationTrack.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            animationTrack.Name = "animationTrack";
            animationTrack.Size = new System.Drawing.Size(522, 37);
            animationTrack.StartFrame = 0F;
            animationTrack.TabIndex = 0;
            animationTrack.ValueChanged += animationTrack_ValueChanged;
            animationTrack.KeyDown += animationTrack_KeyDown;
            // 
            // splitter1
            // 
            splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            splitter1.Location = new System.Drawing.Point(0, 375);
            splitter1.Margin = new System.Windows.Forms.Padding(5);
            splitter1.Name = "splitter1";
            splitter1.Size = new System.Drawing.Size(539, 5);
            splitter1.TabIndex = 2;
            splitter1.TabStop = false;
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripDropDownButton1, cameraDropDown, toolStripDropDownButton2, screenshotButton });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(539, 27);
            toolStrip1.TabIndex = 3;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { toggleGridToolStripMenuItem, gridColorToolStripMenuItem, toolStripSeparator1, toggleBackgroundToolStripMenuItem, backgroundColorToolStripMenuItem, toolStripSeparator2, toggleCSPModeToolStripMenuItem });
            toolStripDropDownButton1.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new System.Drawing.Size(55, 24);
            toolStripDropDownButton1.Text = "View";
            // 
            // toggleGridToolStripMenuItem
            // 
            toggleGridToolStripMenuItem.Name = "toggleGridToolStripMenuItem";
            toggleGridToolStripMenuItem.Size = new System.Drawing.Size(236, 26);
            toggleGridToolStripMenuItem.Text = "Toggle Grid";
            toggleGridToolStripMenuItem.Click += toggleGridToolStripMenuItem_Click;
            // 
            // gridColorToolStripMenuItem
            // 
            gridColorToolStripMenuItem.Name = "gridColorToolStripMenuItem";
            gridColorToolStripMenuItem.Size = new System.Drawing.Size(236, 26);
            gridColorToolStripMenuItem.Text = "Set Grid Color";
            gridColorToolStripMenuItem.Click += gridColorToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(233, 6);
            // 
            // toggleBackgroundToolStripMenuItem
            // 
            toggleBackgroundToolStripMenuItem.Name = "toggleBackgroundToolStripMenuItem";
            toggleBackgroundToolStripMenuItem.Size = new System.Drawing.Size(236, 26);
            toggleBackgroundToolStripMenuItem.Text = "Toggle Background";
            toggleBackgroundToolStripMenuItem.Click += toggleBackgroundToolStripMenuItem_Click;
            // 
            // backgroundColorToolStripMenuItem
            // 
            backgroundColorToolStripMenuItem.Name = "backgroundColorToolStripMenuItem";
            backgroundColorToolStripMenuItem.Size = new System.Drawing.Size(236, 26);
            backgroundColorToolStripMenuItem.Text = "Set Background Color";
            backgroundColorToolStripMenuItem.Click += backgroundColorToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(233, 6);
            // 
            // toggleCSPModeToolStripMenuItem
            // 
            toggleCSPModeToolStripMenuItem.Name = "toggleCSPModeToolStripMenuItem";
            toggleCSPModeToolStripMenuItem.Size = new System.Drawing.Size(236, 26);
            toggleCSPModeToolStripMenuItem.Text = "Toggle CSP Mode";
            toggleCSPModeToolStripMenuItem.Click += toggleCSPModeToolStripMenuItem_Click;
            // 
            // cameraDropDown
            // 
            cameraDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            cameraDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { resetToolStripMenuItem1, settingsToolStripMenuItem1 });
            cameraDropDown.Image = (System.Drawing.Image)resources.GetObject("cameraDropDown.Image");
            cameraDropDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            cameraDropDown.Name = "cameraDropDown";
            cameraDropDown.Size = new System.Drawing.Size(74, 24);
            cameraDropDown.Text = "Camera";
            // 
            // resetToolStripMenuItem1
            // 
            resetToolStripMenuItem1.Name = "resetToolStripMenuItem1";
            resetToolStripMenuItem1.Size = new System.Drawing.Size(145, 26);
            resetToolStripMenuItem1.Text = "Reset";
            resetToolStripMenuItem1.Click += resetCameraButton_Click;
            // 
            // settingsToolStripMenuItem1
            // 
            settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            settingsToolStripMenuItem1.Size = new System.Drawing.Size(145, 26);
            settingsToolStripMenuItem1.Text = "Settings";
            settingsToolStripMenuItem1.Click += editCameraButton_Click;
            // 
            // toolStripDropDownButton2
            // 
            toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { exportFrameAsPNGToolStripMenuItem, exportFrameToFolderToolStripMenuItem });
            toolStripDropDownButton2.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton2.Image");
            toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            toolStripDropDownButton2.Size = new System.Drawing.Size(66, 24);
            toolStripDropDownButton2.Text = "Export";
            // 
            // exportFrameAsPNGToolStripMenuItem
            // 
            exportFrameAsPNGToolStripMenuItem.Name = "exportFrameAsPNGToolStripMenuItem";
            exportFrameAsPNGToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            exportFrameAsPNGToolStripMenuItem.Text = "Current Frame";
            exportFrameAsPNGToolStripMenuItem.Click += exportFrameAsPNGToolStripMenuItem_Click;
            // 
            // exportFrameToFolderToolStripMenuItem
            // 
            exportFrameToFolderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { asGIFToolStripMenuItem, toFolderToolStripMenuItem });
            exportFrameToFolderToolStripMenuItem.Name = "exportFrameToFolderToolStripMenuItem";
            exportFrameToFolderToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            exportFrameToFolderToolStripMenuItem.Text = "Animation";
            // 
            // asGIFToolStripMenuItem
            // 
            asGIFToolStripMenuItem.Name = "asGIFToolStripMenuItem";
            asGIFToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            asGIFToolStripMenuItem.Text = "To GIF (.gif)";
            asGIFToolStripMenuItem.Click += asGIFToolStripMenuItem_Click;
            // 
            // toFolderToolStripMenuItem
            // 
            toFolderToolStripMenuItem.Name = "toFolderToolStripMenuItem";
            toFolderToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            toFolderToolStripMenuItem.Text = "To Folder (.png)";
            toFolderToolStripMenuItem.Click += exportFrameToFolderToolStripMenuItem_Click;
            // 
            // screenshotButton
            // 
            screenshotButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            screenshotButton.Image = (System.Drawing.Image)resources.GetObject("screenshotButton.Image");
            screenshotButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            screenshotButton.Name = "screenshotButton";
            screenshotButton.Size = new System.Drawing.Size(118, 24);
            screenshotButton.Text = "Take Screenshot";
            screenshotButton.Click += screenshotButton_Click;
            // 
            // ViewportControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(glControl);
            Controls.Add(toolStrip1);
            Controls.Add(splitter1);
            Controls.Add(animationGroup);
            Margin = new System.Windows.Forms.Padding(5);
            Name = "ViewportControl";
            Size = new System.Drawing.Size(539, 525);
            animationGroup.ResumeLayout(false);
            animationGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudPlaybackSpeed).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxFrame).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudFrame).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenTK.WinForms.GLControl glControl;
        private System.Windows.Forms.Panel animationGroup;
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem exportFrameAsPNGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportFrameToFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asGIFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toFolderToolStripMenuItem;
    }
}
