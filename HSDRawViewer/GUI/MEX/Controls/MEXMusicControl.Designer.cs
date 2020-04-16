namespace HSDRawViewer.GUI.MEX.Controls
{
    partial class MEXMusicControl
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
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.saveMusicButton = new System.Windows.Forms.ToolStripButton();
            this.createHPSButton = new System.Windows.Forms.ToolStripButton();
            this.musicDSPPlayer = new HSDRawViewer.GUI.Extra.DSPViewer();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.musicListEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.menuPlaylistEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip2.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.tabPage8.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveMusicButton,
            this.createHPSButton});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(720, 25);
            this.toolStrip2.TabIndex = 4;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // saveMusicButton
            // 
            this.saveMusicButton.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.saveMusicButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveMusicButton.Name = "saveMusicButton";
            this.saveMusicButton.Size = new System.Drawing.Size(135, 22);
            this.saveMusicButton.Text = "Save Music Changes";
            this.saveMusicButton.Click += new System.EventHandler(this.saveMusicButton_Click);
            // 
            // createHPSButton
            // 
            this.createHPSButton.Image = global::HSDRawViewer.Properties.Resources.ts_importfile;
            this.createHPSButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.createHPSButton.Name = "createHPSButton";
            this.createHPSButton.Size = new System.Drawing.Size(151, 22);
            this.createHPSButton.Text = "Create HPS From File(s)";
            this.createHPSButton.Click += new System.EventHandler(this.createHPSButton_Click);
            // 
            // musicDSPPlayer
            // 
            this.musicDSPPlayer.Dock = System.Windows.Forms.DockStyle.Right;
            this.musicDSPPlayer.DSP = null;
            this.musicDSPPlayer.Location = new System.Drawing.Point(351, 25);
            this.musicDSPPlayer.Name = "musicDSPPlayer";
            this.musicDSPPlayer.ReplaceButtonVisible = false;
            this.musicDSPPlayer.Size = new System.Drawing.Size(369, 375);
            this.musicDSPPlayer.TabIndex = 6;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage7);
            this.tabControl2.Controls.Add(this.tabPage8);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 25);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(351, 375);
            this.tabControl2.TabIndex = 8;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.musicListEditor);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(343, 349);
            this.tabPage7.TabIndex = 0;
            this.tabPage7.Text = "Music Files";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // musicListEditor
            // 
            this.musicListEditor.DisplayItemIndices = true;
            this.musicListEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.musicListEditor.EnablePropertyViewDescription = true;
            this.musicListEditor.ItemIndexOffset = 0;
            this.musicListEditor.Location = new System.Drawing.Point(3, 3);
            this.musicListEditor.Name = "musicListEditor";
            this.musicListEditor.Size = new System.Drawing.Size(337, 343);
            this.musicListEditor.TabIndex = 4;
            this.musicListEditor.DoubleClickedNode += new System.EventHandler(this.musicListEditor_DoubleClickedNode);
            this.musicListEditor.ArrayUpdated += new System.EventHandler(this.musicListEditor_ArrayUpdated);
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.menuPlaylistEditor);
            this.tabPage8.Location = new System.Drawing.Point(4, 22);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(343, 349);
            this.tabPage8.TabIndex = 1;
            this.tabPage8.Text = "Menu Playlist";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // menuPlaylistEditor
            // 
            this.menuPlaylistEditor.DisplayItemIndices = true;
            this.menuPlaylistEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuPlaylistEditor.EnablePropertyViewDescription = true;
            this.menuPlaylistEditor.ItemIndexOffset = 0;
            this.menuPlaylistEditor.Location = new System.Drawing.Point(3, 3);
            this.menuPlaylistEditor.Name = "menuPlaylistEditor";
            this.menuPlaylistEditor.Size = new System.Drawing.Size(337, 343);
            this.menuPlaylistEditor.TabIndex = 0;
            this.menuPlaylistEditor.DoubleClickedNode += new System.EventHandler(this.menuPlaylistEditor_DoubleClickedNode);
            // 
            // MEXMusicControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.musicDSPPlayer);
            this.Controls.Add(this.toolStrip2);
            this.Name = "MEXMusicControl";
            this.Size = new System.Drawing.Size(720, 400);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tabControl2.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.tabPage8.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton saveMusicButton;
        private System.Windows.Forms.ToolStripButton createHPSButton;
        private Extra.DSPViewer musicDSPPlayer;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage7;
        private ArrayMemberEditor musicListEditor;
        private System.Windows.Forms.TabPage tabPage8;
        private ArrayMemberEditor menuPlaylistEditor;
    }
}
