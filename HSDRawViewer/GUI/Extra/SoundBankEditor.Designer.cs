namespace HSDRawViewer.GUI.Extra
{
    partial class SoundBankEditor
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dspViewer1 = new HSDRawViewer.GUI.Extra.DSPViewer();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.soundArrayEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.buttonSoundBankAdd = new System.Windows.Forms.ToolStripButton();
            this.buttonSoundBankDelete = new System.Windows.Forms.ToolStripButton();
            this.groupBox1.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dspViewer1);
            this.groupBox1.Controls.Add(this.splitter3);
            this.groupBox1.Controls.Add(this.soundArrayEditor);
            this.groupBox1.Controls.Add(this.toolStrip3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(702, 524);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sound Bank";
            // 
            // dspViewer1
            // 
            this.dspViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dspViewer1.DSP = null;
            this.dspViewer1.Location = new System.Drawing.Point(214, 41);
            this.dspViewer1.Name = "dspViewer1";
            this.dspViewer1.ReplaceButtonVisible = true;
            this.dspViewer1.Size = new System.Drawing.Size(485, 480);
            this.dspViewer1.TabIndex = 3;
            // 
            // splitter3
            // 
            this.splitter3.Location = new System.Drawing.Point(211, 41);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(3, 480);
            this.splitter3.TabIndex = 1;
            this.splitter3.TabStop = false;
            // 
            // soundArrayEditor
            // 
            this.soundArrayEditor.DisplayItemIndices = true;
            this.soundArrayEditor.Dock = System.Windows.Forms.DockStyle.Left;
            this.soundArrayEditor.EnablePropertyView = false;
            this.soundArrayEditor.EnablePropertyViewDescription = false;
            this.soundArrayEditor.EnableToolStrip = false;
            this.soundArrayEditor.ItemIndexOffset = 0;
            this.soundArrayEditor.Location = new System.Drawing.Point(3, 41);
            this.soundArrayEditor.Name = "soundArrayEditor";
            this.soundArrayEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.soundArrayEditor.Size = new System.Drawing.Size(208, 480);
            this.soundArrayEditor.TabIndex = 6;
            this.soundArrayEditor.SelectedObjectChanged += new System.EventHandler(this.soundBankList_SelectedIndexChanged);
            this.soundArrayEditor.DoubleClickedNode += new System.EventHandler(this.soundBankList_MouseDoubleClick);
            // 
            // toolStrip3
            // 
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSoundBankAdd,
            this.buttonSoundBankDelete});
            this.toolStrip3.Location = new System.Drawing.Point(3, 16);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(696, 25);
            this.toolStrip3.TabIndex = 5;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // buttonSoundBankAdd
            // 
            this.buttonSoundBankAdd.Image = global::HSDRawViewer.Properties.Resources.ts_add;
            this.buttonSoundBankAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSoundBankAdd.Name = "buttonSoundBankAdd";
            this.buttonSoundBankAdd.Size = new System.Drawing.Size(113, 22);
            this.buttonSoundBankAdd.Text = "Add New Sound";
            this.buttonSoundBankAdd.Click += new System.EventHandler(this.buttonSoundBankAdd_Click);
            // 
            // buttonSoundBankDelete
            // 
            this.buttonSoundBankDelete.Image = global::HSDRawViewer.Properties.Resources.ts_subtract;
            this.buttonSoundBankDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSoundBankDelete.Name = "buttonSoundBankDelete";
            this.buttonSoundBankDelete.Size = new System.Drawing.Size(144, 22);
            this.buttonSoundBankDelete.Text = "Delete Selected Sound";
            this.buttonSoundBankDelete.Click += new System.EventHandler(this.buttonSoundBankDelete_Click);
            // 
            // SoundBankEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "SoundBankEditor";
            this.Size = new System.Drawing.Size(702, 524);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private DSPViewer dspViewer1;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton buttonSoundBankAdd;
        private System.Windows.Forms.ToolStripButton buttonSoundBankDelete;
        private ArrayMemberEditor soundArrayEditor;
    }
}
