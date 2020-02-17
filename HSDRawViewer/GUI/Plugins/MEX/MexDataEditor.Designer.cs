namespace HSDRawViewer.GUI.Plugins.MEX
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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.propertyGrid2 = new System.Windows.Forms.PropertyGrid();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.fighterList = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.saveFightersButton = new System.Windows.Forms.ToolStripButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.saveSSMButton = new System.Windows.Forms.ToolStripButton();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.saveEffectButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonLoadPlSl = new System.Windows.Forms.Button();
            this.buttonSaveCSS = new System.Windows.Forms.ToolStripButton();
            this.ssmEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.effectEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.cssIconEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.mainTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.toolStrip4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.tabPage1);
            this.mainTabControl.Controls.Add(this.tabPage2);
            this.mainTabControl.Controls.Add(this.tabPage6);
            this.mainTabControl.Controls.Add(this.tabPage5);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 0);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(799, 333);
            this.mainTabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tabControl1);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.toolStrip1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(791, 307);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Fighters";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(203, 28);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(585, 276);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.propertyGrid1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(577, 250);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Properties";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid1.Size = new System.Drawing.Size(571, 244);
            this.propertyGrid1.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.propertyGrid2);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(577, 250);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Functions";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // propertyGrid2
            // 
            this.propertyGrid2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid2.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid2.Name = "propertyGrid2";
            this.propertyGrid2.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid2.Size = new System.Drawing.Size(571, 244);
            this.propertyGrid2.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.fighterList);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(3, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 276);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Fighters";
            // 
            // fighterList
            // 
            this.fighterList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fighterList.FormattingEnabled = true;
            this.fighterList.Location = new System.Drawing.Point(3, 16);
            this.fighterList.Name = "fighterList";
            this.fighterList.Size = new System.Drawing.Size(194, 257);
            this.fighterList.TabIndex = 0;
            this.fighterList.SelectedIndexChanged += new System.EventHandler(this.fighterList_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveFightersButton});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(785, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // saveFightersButton
            // 
            this.saveFightersButton.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.saveFightersButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveFightersButton.Name = "saveFightersButton";
            this.saveFightersButton.Size = new System.Drawing.Size(140, 22);
            this.saveFightersButton.Text = "Save Fighter Changes";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ssmEditor);
            this.tabPage2.Controls.Add(this.toolStrip2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(791, 307);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Sounds";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveSSMButton});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(785, 25);
            this.toolStrip2.TabIndex = 3;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // saveSSMButton
            // 
            this.saveSSMButton.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.saveSSMButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveSSMButton.Name = "saveSSMButton";
            this.saveSSMButton.Size = new System.Drawing.Size(126, 22);
            this.saveSSMButton.Text = "Save SSM Changes";
            this.saveSSMButton.Click += new System.EventHandler(this.saveSSMButton_Click);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.effectEditor);
            this.tabPage6.Controls.Add(this.toolStrip3);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(791, 307);
            this.tabPage6.TabIndex = 3;
            this.tabPage6.Text = "Effects";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.groupBox2);
            this.tabPage5.Controls.Add(this.toolStrip4);
            this.tabPage5.Controls.Add(this.cssIconEditor);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(791, 307);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "CSS";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // toolStrip3
            // 
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveEffectButton});
            this.toolStrip3.Location = new System.Drawing.Point(0, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(791, 25);
            this.toolStrip3.TabIndex = 2;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // saveEffectButton
            // 
            this.saveEffectButton.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.saveEffectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveEffectButton.Name = "saveEffectButton";
            this.saveEffectButton.Size = new System.Drawing.Size(133, 22);
            this.saveEffectButton.Text = "Save Effect Changes";
            this.saveEffectButton.Click += new System.EventHandler(this.saveEffectButton_Click);
            // 
            // toolStrip4
            // 
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSaveCSS});
            this.toolStrip4.Location = new System.Drawing.Point(208, 0);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.Size = new System.Drawing.Size(583, 25);
            this.toolStrip4.TabIndex = 1;
            this.toolStrip4.Text = "toolStrip4";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonLoadPlSl);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(208, 25);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(583, 282);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Preview";
            // 
            // buttonLoadPlSl
            // 
            this.buttonLoadPlSl.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonLoadPlSl.Location = new System.Drawing.Point(3, 16);
            this.buttonLoadPlSl.Name = "buttonLoadPlSl";
            this.buttonLoadPlSl.Size = new System.Drawing.Size(577, 23);
            this.buttonLoadPlSl.TabIndex = 0;
            this.buttonLoadPlSl.Text = "Load MnSlChr";
            this.buttonLoadPlSl.UseVisualStyleBackColor = true;
            this.buttonLoadPlSl.Click += new System.EventHandler(this.buttonLoadPlSl_Click);
            // 
            // buttonSaveCSS
            // 
            this.buttonSaveCSS.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.buttonSaveCSS.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSaveCSS.Name = "buttonSaveCSS";
            this.buttonSaveCSS.Size = new System.Drawing.Size(127, 22);
            this.buttonSaveCSS.Text = "Save CSS Icon Data";
            this.buttonSaveCSS.Click += new System.EventHandler(this.buttonSaveCSS_Click);
            // 
            // ssmEditor
            // 
            this.ssmEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ssmEditor.Location = new System.Drawing.Point(3, 28);
            this.ssmEditor.Name = "ssmEditor";
            this.ssmEditor.Size = new System.Drawing.Size(785, 276);
            this.ssmEditor.TabIndex = 6;
            // 
            // effectEditor
            // 
            this.effectEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.effectEditor.Location = new System.Drawing.Point(0, 25);
            this.effectEditor.Name = "effectEditor";
            this.effectEditor.Size = new System.Drawing.Size(791, 282);
            this.effectEditor.TabIndex = 1;
            // 
            // cssIconEditor
            // 
            this.cssIconEditor.Dock = System.Windows.Forms.DockStyle.Left;
            this.cssIconEditor.Location = new System.Drawing.Point(0, 0);
            this.cssIconEditor.Name = "cssIconEditor";
            this.cssIconEditor.Size = new System.Drawing.Size(208, 307);
            this.cssIconEditor.TabIndex = 0;
            // 
            // MexDataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 333);
            this.Controls.Add(this.mainTabControl);
            this.Name = "MexDataEditor";
            this.TabText = "MexDataEditor";
            this.Text = "MexDataEditor";
            this.mainTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox fighterList;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.PropertyGrid propertyGrid2;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton saveFightersButton;
        private System.Windows.Forms.ToolStripButton saveSSMButton;
        private ArrayMemberEditor ssmEditor;
        private ArrayMemberEditor effectEditor;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton saveEffectButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private ArrayMemberEditor cssIconEditor;
        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.Button buttonLoadPlSl;
        private System.Windows.Forms.ToolStripButton buttonSaveCSS;
    }
}