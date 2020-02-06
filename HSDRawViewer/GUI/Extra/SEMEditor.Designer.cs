namespace HSDRawViewer.GUI.Extra
{
    partial class SEMEditor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSEMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSEMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.entryList = new System.Windows.Forms.ListBox();
            this.soundList = new System.Windows.Forms.ListBox();
            this.entryBox = new System.Windows.Forms.GroupBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonAddEntry = new System.Windows.Forms.ToolStripButton();
            this.buttonDeleteEntry = new System.Windows.Forms.ToolStripButton();
            this.soundBox = new System.Windows.Forms.GroupBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.buttonAddSound = new System.Windows.Forms.ToolStripButton();
            this.buttonRemoveSound = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.scriptBox = new System.Windows.Forms.RichTextBox();
            this.buttonSaveScript = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.menuStrip1.SuspendLayout();
            this.entryBox.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.soundBox.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(926, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSEMToolStripMenuItem,
            this.exportSEMToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openSEMToolStripMenuItem
            // 
            this.openSEMToolStripMenuItem.Name = "openSEMToolStripMenuItem";
            this.openSEMToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.openSEMToolStripMenuItem.Text = "Open SEM";
            this.openSEMToolStripMenuItem.Click += new System.EventHandler(this.openSEMToolStripMenuItem_Click);
            // 
            // exportSEMToolStripMenuItem
            // 
            this.exportSEMToolStripMenuItem.Name = "exportSEMToolStripMenuItem";
            this.exportSEMToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.exportSEMToolStripMenuItem.Text = "Export SEM";
            this.exportSEMToolStripMenuItem.Click += new System.EventHandler(this.exportSEMToolStripMenuItem_Click);
            // 
            // entryList
            // 
            this.entryList.AllowDrop = true;
            this.entryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entryList.FormattingEnabled = true;
            this.entryList.Location = new System.Drawing.Point(3, 41);
            this.entryList.Name = "entryList";
            this.entryList.Size = new System.Drawing.Size(159, 281);
            this.entryList.TabIndex = 1;
            this.entryList.SelectedIndexChanged += new System.EventHandler(this.entryList_SelectedIndexChanged);
            this.entryList.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox_DragDrop);
            this.entryList.DragOver += new System.Windows.Forms.DragEventHandler(this.listBox_DragOver);
            this.entryList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseDown);
            // 
            // soundList
            // 
            this.soundList.AllowDrop = true;
            this.soundList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.soundList.FormattingEnabled = true;
            this.soundList.Location = new System.Drawing.Point(3, 41);
            this.soundList.Name = "soundList";
            this.soundList.Size = new System.Drawing.Size(194, 281);
            this.soundList.TabIndex = 3;
            this.soundList.SelectedIndexChanged += new System.EventHandler(this.soundList_SelectedIndexChanged);
            this.soundList.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox_DragDrop);
            this.soundList.DragOver += new System.Windows.Forms.DragEventHandler(this.listBox_DragOver);
            this.soundList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseDown);
            // 
            // entryBox
            // 
            this.entryBox.Controls.Add(this.entryList);
            this.entryBox.Controls.Add(this.toolStrip1);
            this.entryBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.entryBox.Location = new System.Drawing.Point(0, 24);
            this.entryBox.Name = "entryBox";
            this.entryBox.Size = new System.Drawing.Size(165, 325);
            this.entryBox.TabIndex = 4;
            this.entryBox.TabStop = false;
            this.entryBox.Text = "Entries";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonAddEntry,
            this.buttonDeleteEntry});
            this.toolStrip1.Location = new System.Drawing.Point(3, 16);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(159, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonAddEntry
            // 
            this.buttonAddEntry.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAddEntry.Image = global::HSDRawViewer.Properties.Resources.ts_add;
            this.buttonAddEntry.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAddEntry.Name = "buttonAddEntry";
            this.buttonAddEntry.Size = new System.Drawing.Size(23, 22);
            this.buttonAddEntry.Text = "Add Entry";
            this.buttonAddEntry.Click += new System.EventHandler(this.buttonAddEntry_Click);
            // 
            // buttonDeleteEntry
            // 
            this.buttonDeleteEntry.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDeleteEntry.Image = global::HSDRawViewer.Properties.Resources.ts_subtract;
            this.buttonDeleteEntry.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDeleteEntry.Name = "buttonDeleteEntry";
            this.buttonDeleteEntry.Size = new System.Drawing.Size(23, 22);
            this.buttonDeleteEntry.Text = "Delete Entry";
            this.buttonDeleteEntry.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // soundBox
            // 
            this.soundBox.Controls.Add(this.soundList);
            this.soundBox.Controls.Add(this.toolStrip2);
            this.soundBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.soundBox.Location = new System.Drawing.Point(175, 24);
            this.soundBox.Name = "soundBox";
            this.soundBox.Size = new System.Drawing.Size(200, 325);
            this.soundBox.TabIndex = 5;
            this.soundBox.TabStop = false;
            this.soundBox.Text = "Sounds";
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonAddSound,
            this.buttonRemoveSound});
            this.toolStrip2.Location = new System.Drawing.Point(3, 16);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(194, 25);
            this.toolStrip2.TabIndex = 4;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // buttonAddSound
            // 
            this.buttonAddSound.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAddSound.Image = global::HSDRawViewer.Properties.Resources.ts_add;
            this.buttonAddSound.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAddSound.Name = "buttonAddSound";
            this.buttonAddSound.Size = new System.Drawing.Size(23, 22);
            this.buttonAddSound.Text = "Add Sound";
            this.buttonAddSound.Click += new System.EventHandler(this.buttonAddSound_Click);
            // 
            // buttonRemoveSound
            // 
            this.buttonRemoveSound.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRemoveSound.Image = global::HSDRawViewer.Properties.Resources.ts_subtract;
            this.buttonRemoveSound.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRemoveSound.Name = "buttonRemoveSound";
            this.buttonRemoveSound.Size = new System.Drawing.Size(23, 22);
            this.buttonRemoveSound.Text = "Remove Sound";
            this.buttonRemoveSound.Click += new System.EventHandler(this.buttonRemoveSound_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.splitter3);
            this.groupBox1.Controls.Add(this.scriptBox);
            this.groupBox1.Controls.Add(this.buttonSaveScript);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(375, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(551, 325);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data";
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter3.Location = new System.Drawing.Point(3, 148);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(545, 3);
            this.splitter3.TabIndex = 1;
            this.splitter3.TabStop = false;
            // 
            // scriptBox
            // 
            this.scriptBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.scriptBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scriptBox.Location = new System.Drawing.Point(3, 39);
            this.scriptBox.Name = "scriptBox";
            this.scriptBox.Size = new System.Drawing.Size(545, 109);
            this.scriptBox.TabIndex = 0;
            this.scriptBox.Text = "";
            // 
            // buttonSaveScript
            // 
            this.buttonSaveScript.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonSaveScript.Location = new System.Drawing.Point(3, 16);
            this.buttonSaveScript.Name = "buttonSaveScript";
            this.buttonSaveScript.Size = new System.Drawing.Size(545, 23);
            this.buttonSaveScript.TabIndex = 2;
            this.buttonSaveScript.Text = "Save";
            this.buttonSaveScript.UseVisualStyleBackColor = true;
            this.buttonSaveScript.Click += new System.EventHandler(this.buttonSaveScript_Click);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(165, 24);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(10, 325);
            this.splitter1.TabIndex = 7;
            this.splitter1.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(375, 24);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 325);
            this.splitter2.TabIndex = 8;
            this.splitter2.TabStop = false;
            // 
            // SEMEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 349);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.soundBox);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.entryBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SEMEditor";
            this.Text = "SEM Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.entryBox.ResumeLayout(false);
            this.entryBox.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.soundBox.ResumeLayout(false);
            this.soundBox.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSEMToolStripMenuItem;
        private System.Windows.Forms.ListBox entryList;
        private System.Windows.Forms.ListBox soundList;
        private System.Windows.Forms.GroupBox entryBox;
        private System.Windows.Forms.GroupBox soundBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.ToolStripMenuItem exportSEMToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.RichTextBox scriptBox;
        private System.Windows.Forms.ToolStripButton buttonAddEntry;
        private System.Windows.Forms.ToolStripButton buttonDeleteEntry;
        private System.Windows.Forms.ToolStripButton buttonAddSound;
        private System.Windows.Forms.ToolStripButton buttonRemoveSound;
        private System.Windows.Forms.Button buttonSaveScript;
    }
}