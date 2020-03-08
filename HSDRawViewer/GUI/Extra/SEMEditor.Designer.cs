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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SEMEditor));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSEMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSEMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smStdatToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mxDtdatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.entryList = new System.Windows.Forms.ListBox();
            this.soundList = new System.Windows.Forms.ListBox();
            this.entryBox = new System.Windows.Forms.GroupBox();
            this.splitter5 = new System.Windows.Forms.Splitter();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonAddEntry = new System.Windows.Forms.ToolStripButton();
            this.buttonDeleteEntry = new System.Windows.Forms.ToolStripButton();
            this.soundBox = new System.Windows.Forms.GroupBox();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.scriptGroup = new System.Windows.Forms.GroupBox();
            this.scriptBox = new System.Windows.Forms.RichTextBox();
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.copyScriptButton = new System.Windows.Forms.ToolStripButton();
            this.pasteScriptButton = new System.Windows.Forms.ToolStripButton();
            this.buttonSaveScript = new System.Windows.Forms.Button();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.buttonAddSound = new System.Windows.Forms.ToolStripButton();
            this.buttonRemoveSound = new System.Windows.Forms.ToolStripButton();
            this.buttonMoveUp = new System.Windows.Forms.ToolStripButton();
            this.buttonMoveDown = new System.Windows.Forms.ToolStripButton();
            this.renameButton = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.soundBankList = new System.Windows.Forms.ListBox();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.buttonSoundBankAdd = new System.Windows.Forms.ToolStripButton();
            this.buttonSoundBankDelete = new System.Windows.Forms.ToolStripButton();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.exportScriptButton = new System.Windows.Forms.ToolStripButton();
            this.dspViewer1 = new HSDRawViewer.GUI.Extra.DSPViewer();
            this.menuStrip1.SuspendLayout();
            this.entryBox.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.soundBox.SuspendLayout();
            this.scriptGroup.SuspendLayout();
            this.toolStrip4.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.smStdatToolStripMenuItem1,
            this.mxDtdatToolStripMenuItem});
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
            // smStdatToolStripMenuItem1
            // 
            this.smStdatToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.smStdatToolStripMenuItem1.Enabled = false;
            this.smStdatToolStripMenuItem1.Name = "smStdatToolStripMenuItem1";
            this.smStdatToolStripMenuItem1.Size = new System.Drawing.Size(66, 20);
            this.smStdatToolStripMenuItem1.Text = "SmSt.dat";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.ImportToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportSmStdatToolStripMenuItem_Click);
            // 
            // mxDtdatToolStripMenuItem
            // 
            this.mxDtdatToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem1});
            this.mxDtdatToolStripMenuItem.Enabled = false;
            this.mxDtdatToolStripMenuItem.Name = "mxDtdatToolStripMenuItem";
            this.mxDtdatToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.mxDtdatToolStripMenuItem.Text = "MxDt.dat";
            // 
            // importToolStripMenuItem1
            // 
            this.importToolStripMenuItem1.Name = "importToolStripMenuItem1";
            this.importToolStripMenuItem1.Size = new System.Drawing.Size(110, 22);
            this.importToolStripMenuItem1.Text = "Import";
            this.importToolStripMenuItem1.Click += new System.EventHandler(this.importToolStripMenuItem1_Click);
            // 
            // entryList
            // 
            this.entryList.AllowDrop = true;
            this.entryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entryList.FormattingEnabled = true;
            this.entryList.Location = new System.Drawing.Point(3, 41);
            this.entryList.Name = "entryList";
            this.entryList.Size = new System.Drawing.Size(159, 185);
            this.entryList.TabIndex = 1;
            this.entryList.SelectedIndexChanged += new System.EventHandler(this.entryList_SelectedIndexChanged);
            this.entryList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.entryList_KeyDown);
            // 
            // soundList
            // 
            this.soundList.AllowDrop = true;
            this.soundList.Dock = System.Windows.Forms.DockStyle.Left;
            this.soundList.FormattingEnabled = true;
            this.soundList.Location = new System.Drawing.Point(3, 41);
            this.soundList.Name = "soundList";
            this.soundList.Size = new System.Drawing.Size(230, 134);
            this.soundList.TabIndex = 3;
            this.soundList.SelectedIndexChanged += new System.EventHandler(this.soundList_SelectedIndexChanged);
            this.soundList.DataSourceChanged += new System.EventHandler(this.soundList_DataSourceChanged);
            this.soundList.DoubleClick += new System.EventHandler(this.soundList_DoubleClick);
            this.soundList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.soundList_KeyDown);
            // 
            // entryBox
            // 
            this.entryBox.Controls.Add(this.entryList);
            this.entryBox.Controls.Add(this.splitter5);
            this.entryBox.Controls.Add(this.propertyGrid1);
            this.entryBox.Controls.Add(this.toolStrip1);
            this.entryBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.entryBox.Location = new System.Drawing.Point(0, 24);
            this.entryBox.Name = "entryBox";
            this.entryBox.Size = new System.Drawing.Size(165, 449);
            this.entryBox.TabIndex = 4;
            this.entryBox.TabStop = false;
            this.entryBox.Text = "Entries";
            // 
            // splitter5
            // 
            this.splitter5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter5.Location = new System.Drawing.Point(3, 226);
            this.splitter5.Name = "splitter5";
            this.splitter5.Size = new System.Drawing.Size(159, 3);
            this.splitter5.TabIndex = 4;
            this.splitter5.TabStop = false;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 229);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(159, 217);
            this.propertyGrid1.TabIndex = 3;
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
            this.soundBox.Controls.Add(this.splitter4);
            this.soundBox.Controls.Add(this.scriptGroup);
            this.soundBox.Controls.Add(this.soundList);
            this.soundBox.Controls.Add(this.toolStrip2);
            this.soundBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.soundBox.Location = new System.Drawing.Point(168, 24);
            this.soundBox.Name = "soundBox";
            this.soundBox.Size = new System.Drawing.Size(758, 178);
            this.soundBox.TabIndex = 5;
            this.soundBox.TabStop = false;
            this.soundBox.Text = "Sound Script";
            // 
            // splitter4
            // 
            this.splitter4.Location = new System.Drawing.Point(233, 41);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(3, 134);
            this.splitter4.TabIndex = 6;
            this.splitter4.TabStop = false;
            // 
            // scriptGroup
            // 
            this.scriptGroup.Controls.Add(this.scriptBox);
            this.scriptGroup.Controls.Add(this.toolStrip4);
            this.scriptGroup.Controls.Add(this.buttonSaveScript);
            this.scriptGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scriptGroup.Location = new System.Drawing.Point(233, 41);
            this.scriptGroup.Name = "scriptGroup";
            this.scriptGroup.Size = new System.Drawing.Size(522, 134);
            this.scriptGroup.TabIndex = 5;
            this.scriptGroup.TabStop = false;
            this.scriptGroup.Text = "Script";
            // 
            // scriptBox
            // 
            this.scriptBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scriptBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scriptBox.Location = new System.Drawing.Point(3, 64);
            this.scriptBox.Name = "scriptBox";
            this.scriptBox.Size = new System.Drawing.Size(516, 67);
            this.scriptBox.TabIndex = 0;
            this.scriptBox.Text = "";
            // 
            // toolStrip4
            // 
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyScriptButton,
            this.pasteScriptButton});
            this.toolStrip4.Location = new System.Drawing.Point(3, 39);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.Size = new System.Drawing.Size(516, 25);
            this.toolStrip4.TabIndex = 3;
            this.toolStrip4.Text = "toolStrip4";
            // 
            // copyScriptButton
            // 
            this.copyScriptButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.copyScriptButton.Image = ((System.Drawing.Image)(resources.GetObject("copyScriptButton.Image")));
            this.copyScriptButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyScriptButton.Name = "copyScriptButton";
            this.copyScriptButton.Size = new System.Drawing.Size(39, 22);
            this.copyScriptButton.Text = "Copy";
            this.copyScriptButton.Click += new System.EventHandler(this.copyScriptButton_Click);
            // 
            // pasteScriptButton
            // 
            this.pasteScriptButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.pasteScriptButton.Image = ((System.Drawing.Image)(resources.GetObject("pasteScriptButton.Image")));
            this.pasteScriptButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteScriptButton.Name = "pasteScriptButton";
            this.pasteScriptButton.Size = new System.Drawing.Size(39, 22);
            this.pasteScriptButton.Text = "Paste";
            this.pasteScriptButton.Click += new System.EventHandler(this.pasteScriptButton_Click);
            // 
            // buttonSaveScript
            // 
            this.buttonSaveScript.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonSaveScript.Location = new System.Drawing.Point(3, 16);
            this.buttonSaveScript.Name = "buttonSaveScript";
            this.buttonSaveScript.Size = new System.Drawing.Size(516, 23);
            this.buttonSaveScript.TabIndex = 2;
            this.buttonSaveScript.Text = "Save Script Changes";
            this.buttonSaveScript.UseVisualStyleBackColor = true;
            this.buttonSaveScript.Click += new System.EventHandler(this.buttonSaveScript_Click);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonAddSound,
            this.buttonRemoveSound,
            this.buttonMoveUp,
            this.buttonMoveDown,
            this.renameButton,
            this.exportScriptButton});
            this.toolStrip2.Location = new System.Drawing.Point(3, 16);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(752, 25);
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
            // buttonMoveUp
            // 
            this.buttonMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonMoveUp.Image = global::HSDRawViewer.Properties.Resources.ts_up;
            this.buttonMoveUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(23, 22);
            this.buttonMoveUp.Text = "toolStripButton1";
            this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonMoveDown.Image = global::HSDRawViewer.Properties.Resources.ts_down;
            this.buttonMoveDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(23, 22);
            this.buttonMoveDown.Text = "toolStripButton1";
            this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
            // 
            // renameButton
            // 
            this.renameButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.renameButton.Enabled = false;
            this.renameButton.Image = ((System.Drawing.Image)(resources.GetObject("renameButton.Image")));
            this.renameButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.renameButton.Name = "renameButton";
            this.renameButton.Size = new System.Drawing.Size(57, 22);
            this.renameButton.Text = "Rename ";
            this.renameButton.Click += new System.EventHandler(this.renameButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dspViewer1);
            this.groupBox1.Controls.Add(this.splitter3);
            this.groupBox1.Controls.Add(this.soundBankList);
            this.groupBox1.Controls.Add(this.toolStrip3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(168, 212);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(758, 261);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sound Bank";
            // 
            // splitter3
            // 
            this.splitter3.Location = new System.Drawing.Point(233, 41);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(3, 217);
            this.splitter3.TabIndex = 1;
            this.splitter3.TabStop = false;
            // 
            // soundBankList
            // 
            this.soundBankList.AllowDrop = true;
            this.soundBankList.Dock = System.Windows.Forms.DockStyle.Left;
            this.soundBankList.FormattingEnabled = true;
            this.soundBankList.Location = new System.Drawing.Point(3, 41);
            this.soundBankList.Name = "soundBankList";
            this.soundBankList.Size = new System.Drawing.Size(230, 217);
            this.soundBankList.TabIndex = 4;
            this.soundBankList.SelectedIndexChanged += new System.EventHandler(this.soundBankList_SelectedIndexChanged);
            this.soundBankList.DataSourceChanged += new System.EventHandler(this.soundBankList_DataSourceChanged);
            this.soundBankList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.soundBankList_MouseDoubleClick);
            // 
            // toolStrip3
            // 
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSoundBankAdd,
            this.buttonSoundBankDelete});
            this.toolStrip3.Location = new System.Drawing.Point(3, 16);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(752, 25);
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
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(168, 202);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(758, 10);
            this.splitter1.TabIndex = 7;
            this.splitter1.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(165, 24);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 449);
            this.splitter2.TabIndex = 8;
            this.splitter2.TabStop = false;
            // 
            // exportScriptButton
            // 
            this.exportScriptButton.Image = global::HSDRawViewer.Properties.Resources.ts_exportfile;
            this.exportScriptButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exportScriptButton.Name = "exportScriptButton";
            this.exportScriptButton.Size = new System.Drawing.Size(94, 22);
            this.exportScriptButton.Text = "Export Script";
            this.exportScriptButton.Click += new System.EventHandler(this.exportScriptButton_Click);
            // 
            // dspViewer1
            // 
            this.dspViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dspViewer1.DSP = null;
            this.dspViewer1.Location = new System.Drawing.Point(236, 41);
            this.dspViewer1.Name = "dspViewer1";
            this.dspViewer1.ReplaceButtonVisible = true;
            this.dspViewer1.Size = new System.Drawing.Size(519, 217);
            this.dspViewer1.TabIndex = 3;
            // 
            // SEMEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 473);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.soundBox);
            this.Controls.Add(this.splitter2);
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
            this.scriptGroup.ResumeLayout(false);
            this.scriptGroup.PerformLayout();
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
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
        private System.Windows.Forms.RichTextBox scriptBox;
        private System.Windows.Forms.ToolStripButton buttonAddEntry;
        private System.Windows.Forms.ToolStripButton buttonDeleteEntry;
        private System.Windows.Forms.ToolStripButton buttonAddSound;
        private System.Windows.Forms.ToolStripButton buttonRemoveSound;
        private System.Windows.Forms.Button buttonSaveScript;
        private DSPViewer dspViewer1;
        private System.Windows.Forms.Splitter splitter4;
        private System.Windows.Forms.GroupBox scriptGroup;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.ListBox soundBankList;
        private System.Windows.Forms.ToolStripButton buttonMoveUp;
        private System.Windows.Forms.ToolStripButton buttonMoveDown;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton buttonSoundBankAdd;
        private System.Windows.Forms.ToolStripButton buttonSoundBankDelete;
        private System.Windows.Forms.ToolStripButton renameButton;
        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.ToolStripButton copyScriptButton;
        private System.Windows.Forms.ToolStripButton pasteScriptButton;
        private System.Windows.Forms.ToolStripMenuItem smStdatToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mxDtdatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem1;
        private System.Windows.Forms.Splitter splitter5;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ToolStripButton exportScriptButton;
    }
}