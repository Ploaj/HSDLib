namespace HSDRawViewer.GUI.MEX
{
    partial class MexDataEditor
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
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.tabPageExternal = new System.Windows.Forms.TabPage();
            this.tabPageFighter = new System.Windows.Forms.TabPage();
            this.tabPageStage = new System.Windows.Forms.TabPage();
            this.tabPageItem = new System.Windows.Forms.TabPage();
            this.tabPageEffects = new System.Windows.Forms.TabPage();
            this.tabPageCSS = new System.Windows.Forms.TabPage();
            this.tabPageMusic = new System.Windows.Forms.TabPage();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.tabPageSound = new System.Windows.Forms.TabPage();
            this.toolStrip6 = new System.Windows.Forms.ToolStrip();
            this.saveAllChangesButton = new System.Windows.Forms.ToolStripButton();
            this.mainTabControl.SuspendLayout();
            this.tabPageMusic.SuspendLayout();
            this.toolStrip6.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.tabPageExternal);
            this.mainTabControl.Controls.Add(this.tabPageFighter);
            this.mainTabControl.Controls.Add(this.tabPageStage);
            this.mainTabControl.Controls.Add(this.tabPageItem);
            this.mainTabControl.Controls.Add(this.tabPageEffects);
            this.mainTabControl.Controls.Add(this.tabPageCSS);
            this.mainTabControl.Controls.Add(this.tabPageMusic);
            this.mainTabControl.Controls.Add(this.tabPageSound);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 25);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(799, 308);
            this.mainTabControl.TabIndex = 0;
            // 
            // tabPageExternal
            // 
            this.tabPageExternal.Location = new System.Drawing.Point(4, 22);
            this.tabPageExternal.Name = "tabPageExternal";
            this.tabPageExternal.Size = new System.Drawing.Size(791, 282);
            this.tabPageExternal.TabIndex = 9;
            this.tabPageExternal.Text = "External Files";
            this.tabPageExternal.UseVisualStyleBackColor = true;
            // 
            // tabPageFighter
            // 
            this.tabPageFighter.Location = new System.Drawing.Point(4, 22);
            this.tabPageFighter.Name = "tabPageFighter";
            this.tabPageFighter.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFighter.Size = new System.Drawing.Size(791, 282);
            this.tabPageFighter.TabIndex = 0;
            this.tabPageFighter.Text = "Fighters";
            this.tabPageFighter.UseVisualStyleBackColor = true;
            // 
            // tabPageStage
            // 
            this.tabPageStage.Location = new System.Drawing.Point(4, 22);
            this.tabPageStage.Name = "tabPageStage";
            this.tabPageStage.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageStage.Size = new System.Drawing.Size(791, 282);
            this.tabPageStage.TabIndex = 6;
            this.tabPageStage.Text = "Stages";
            this.tabPageStage.UseVisualStyleBackColor = true;
            // 
            // tabPageItem
            // 
            this.tabPageItem.Location = new System.Drawing.Point(4, 22);
            this.tabPageItem.Name = "tabPageItem";
            this.tabPageItem.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageItem.Size = new System.Drawing.Size(791, 282);
            this.tabPageItem.TabIndex = 5;
            this.tabPageItem.Text = "Items";
            this.tabPageItem.UseVisualStyleBackColor = true;
            // 
            // tabPageEffects
            // 
            this.tabPageEffects.Location = new System.Drawing.Point(4, 22);
            this.tabPageEffects.Name = "tabPageEffects";
            this.tabPageEffects.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEffects.Size = new System.Drawing.Size(791, 282);
            this.tabPageEffects.TabIndex = 3;
            this.tabPageEffects.Text = "Effects";
            this.tabPageEffects.UseVisualStyleBackColor = true;
            // 
            // tabPageCSS
            // 
            this.tabPageCSS.Location = new System.Drawing.Point(4, 22);
            this.tabPageCSS.Name = "tabPageCSS";
            this.tabPageCSS.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCSS.Size = new System.Drawing.Size(791, 282);
            this.tabPageCSS.TabIndex = 2;
            this.tabPageCSS.Text = "Select Menu";
            this.tabPageCSS.UseVisualStyleBackColor = true;
            // 
            // tabPageMusic
            // 
            this.tabPageMusic.Controls.Add(this.splitter1);
            this.tabPageMusic.Location = new System.Drawing.Point(4, 22);
            this.tabPageMusic.Name = "tabPageMusic";
            this.tabPageMusic.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMusic.Size = new System.Drawing.Size(791, 282);
            this.tabPageMusic.TabIndex = 4;
            this.tabPageMusic.Text = "Music";
            this.tabPageMusic.UseVisualStyleBackColor = true;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(785, 3);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 276);
            this.splitter1.TabIndex = 6;
            this.splitter1.TabStop = false;
            // 
            // tabPageSound
            // 
            this.tabPageSound.Location = new System.Drawing.Point(4, 22);
            this.tabPageSound.Name = "tabPageSound";
            this.tabPageSound.Size = new System.Drawing.Size(791, 282);
            this.tabPageSound.TabIndex = 7;
            this.tabPageSound.Text = "Sound";
            this.tabPageSound.UseVisualStyleBackColor = true;
            // 
            // toolStrip6
            // 
            this.toolStrip6.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAllChangesButton});
            this.toolStrip6.Location = new System.Drawing.Point(0, 0);
            this.toolStrip6.Name = "toolStrip6";
            this.toolStrip6.Size = new System.Drawing.Size(799, 25);
            this.toolStrip6.TabIndex = 9;
            this.toolStrip6.Text = "toolStrip6";
            // 
            // saveAllChangesButton
            // 
            this.saveAllChangesButton.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.saveAllChangesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveAllChangesButton.Name = "saveAllChangesButton";
            this.saveAllChangesButton.Size = new System.Drawing.Size(117, 22);
            this.saveAllChangesButton.Text = "Save All Changes";
            this.saveAllChangesButton.Click += new System.EventHandler(this.saveAllChangesButton_Click);
            // 
            // MexDataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 333);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.toolStrip6);
            this.Name = "MexDataEditor";
            this.TabText = "MexDataEditor";
            this.Text = "MexDataEditor";
            this.mainTabControl.ResumeLayout(false);
            this.tabPageMusic.ResumeLayout(false);
            this.toolStrip6.ResumeLayout(false);
            this.toolStrip6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage tabPageFighter;
        private System.Windows.Forms.TabPage tabPageEffects;
        private System.Windows.Forms.TabPage tabPageCSS;
        private System.Windows.Forms.TabPage tabPageMusic;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.TabPage tabPageItem;
        private System.Windows.Forms.ToolStrip toolStrip6;
        private System.Windows.Forms.ToolStripButton saveAllChangesButton;
        private System.Windows.Forms.TabPage tabPageStage;
        private System.Windows.Forms.TabPage tabPageSound;
        private System.Windows.Forms.TabPage tabPageExternal;
    }
}