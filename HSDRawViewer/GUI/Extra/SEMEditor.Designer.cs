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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.entryBox = new System.Windows.Forms.GroupBox();
            this.entryList = new System.Windows.Forms.ListBox();
            this.splitter5 = new System.Windows.Forms.Splitter();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.importSPKG = new System.Windows.Forms.ToolStripButton();
            this.exportPackage = new System.Windows.Forms.ToolStripButton();
            this.buttonAddEntry = new System.Windows.Forms.ToolStripButton();
            this.buttonDeleteEntry = new System.Windows.Forms.ToolStripButton();
            this.replacePackageButton = new System.Windows.Forms.ToolStripButton();
            this.entryBox.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(205, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 488);
            this.splitter2.TabIndex = 10;
            this.splitter2.TabStop = false;
            // 
            // entryBox
            // 
            this.entryBox.Controls.Add(this.entryList);
            this.entryBox.Controls.Add(this.splitter5);
            this.entryBox.Controls.Add(this.toolStrip1);
            this.entryBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.entryBox.Location = new System.Drawing.Point(0, 0);
            this.entryBox.Name = "entryBox";
            this.entryBox.Size = new System.Drawing.Size(205, 488);
            this.entryBox.TabIndex = 9;
            this.entryBox.TabStop = false;
            this.entryBox.Text = "Entries";
            // 
            // entryList
            // 
            this.entryList.AllowDrop = true;
            this.entryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entryList.FormattingEnabled = true;
            this.entryList.Location = new System.Drawing.Point(3, 41);
            this.entryList.Name = "entryList";
            this.entryList.Size = new System.Drawing.Size(199, 441);
            this.entryList.TabIndex = 1;
            this.entryList.SelectedIndexChanged += new System.EventHandler(this.entryList_SelectedIndexChanged);
            this.entryList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.entryList_KeyDown);
            // 
            // splitter5
            // 
            this.splitter5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter5.Location = new System.Drawing.Point(3, 482);
            this.splitter5.Name = "splitter5";
            this.splitter5.Size = new System.Drawing.Size(199, 3);
            this.splitter5.TabIndex = 4;
            this.splitter5.TabStop = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importSPKG,
            this.exportPackage,
            this.buttonAddEntry,
            this.buttonDeleteEntry,
            this.replacePackageButton});
            this.toolStrip1.Location = new System.Drawing.Point(3, 16);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(199, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // importSPKG
            // 
            this.importSPKG.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.importSPKG.Image = global::HSDRawViewer.Properties.Resources.ts_importfile;
            this.importSPKG.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.importSPKG.Name = "importSPKG";
            this.importSPKG.Size = new System.Drawing.Size(23, 22);
            this.importSPKG.Text = "Import SPKG File";
            this.importSPKG.Click += new System.EventHandler(this.importSPKG_Click);
            // 
            // exportPackage
            // 
            this.exportPackage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.exportPackage.Image = global::HSDRawViewer.Properties.Resources.ts_exportfile;
            this.exportPackage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exportPackage.Name = "exportPackage";
            this.exportPackage.Size = new System.Drawing.Size(23, 22);
            this.exportPackage.Text = "Export SPKG";
            this.exportPackage.Click += new System.EventHandler(this.exportPackage_Click);
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
            // replacePackageButton
            // 
            this.replacePackageButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.replacePackageButton.Image = global::HSDRawViewer.Properties.Resources.ico_replace;
            this.replacePackageButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.replacePackageButton.Name = "replacePackageButton";
            this.replacePackageButton.Size = new System.Drawing.Size(23, 22);
            this.replacePackageButton.Text = "Replace Package";
            this.replacePackageButton.Click += new System.EventHandler(this.replacePackageButton_Click);
            // 
            // SEMEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.entryBox);
            this.Name = "SEMEditor";
            this.Size = new System.Drawing.Size(760, 488);
            this.entryBox.ResumeLayout(false);
            this.entryBox.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.GroupBox entryBox;
        private System.Windows.Forms.ListBox entryList;
        private System.Windows.Forms.Splitter splitter5;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton importSPKG;
        private System.Windows.Forms.ToolStripButton exportPackage;
        private System.Windows.Forms.ToolStripButton buttonAddEntry;
        private System.Windows.Forms.ToolStripButton buttonDeleteEntry;
        private System.Windows.Forms.ToolStripButton replacePackageButton;
    }
}
