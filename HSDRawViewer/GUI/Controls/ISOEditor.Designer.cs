namespace HSDRawViewer.GUI.Controls
{
    partial class ISOEditor
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.fileTree = new System.Windows.Forms.TreeView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.gameDataPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tbMakerCode = new System.Windows.Forms.TextBox();
            this.tbGameCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbGameName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bannerDataPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tbShortName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonExport = new System.Windows.Forms.Button();
            this.buttonImport = new System.Windows.Forms.Button();
            this.pictureIconBox = new System.Windows.Forms.PictureBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbShortMaker = new System.Windows.Forms.TextBox();
            this.tbLongName = new System.Windows.Forms.TextBox();
            this.tbLongMaker = new System.Windows.Forms.TextBox();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.folderContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.importFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.rootContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.importFileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gameDataPanel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.bannerDataPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureIconBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.folderContextMenu.SuspendLayout();
            this.fileContextMenu.SuspendLayout();
            this.rootContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.fileTree);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(361, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 445);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File System";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // fileTree
            // 
            this.fileTree.AllowDrop = true;
            this.fileTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileTree.Enabled = false;
            this.fileTree.HideSelection = false;
            this.fileTree.Indent = 12;
            this.fileTree.ItemHeight = 24;
            this.fileTree.LabelEdit = true;
            this.fileTree.Location = new System.Drawing.Point(3, 16);
            this.fileTree.Name = "fileTree";
            this.fileTree.Size = new System.Drawing.Size(270, 426);
            this.fileTree.TabIndex = 1;
            this.fileTree.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.fileTree_BeforeLabelEdit);
            this.fileTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.fileTree_AfterLabelEdit);
            this.fileTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.fileTree_NodeMouseClick);
            this.fileTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.fileTree_NodeMouseDoubleClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.gameDataPanel);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(358, 100);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Game Data";
            // 
            // gameDataPanel
            // 
            this.gameDataPanel.ColumnCount = 2;
            this.gameDataPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.55682F));
            this.gameDataPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.44318F));
            this.gameDataPanel.Controls.Add(this.tbMakerCode, 1, 2);
            this.gameDataPanel.Controls.Add(this.tbGameCode, 1, 1);
            this.gameDataPanel.Controls.Add(this.label1, 0, 0);
            this.gameDataPanel.Controls.Add(this.label2, 0, 1);
            this.gameDataPanel.Controls.Add(this.label3, 0, 2);
            this.gameDataPanel.Controls.Add(this.tbGameName, 1, 0);
            this.gameDataPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameDataPanel.Enabled = false;
            this.gameDataPanel.Location = new System.Drawing.Point(3, 16);
            this.gameDataPanel.Margin = new System.Windows.Forms.Padding(6);
            this.gameDataPanel.Name = "gameDataPanel";
            this.gameDataPanel.Padding = new System.Windows.Forms.Padding(4);
            this.gameDataPanel.RowCount = 3;
            this.gameDataPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.gameDataPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.gameDataPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.gameDataPanel.Size = new System.Drawing.Size(352, 81);
            this.gameDataPanel.TabIndex = 0;
            // 
            // tbMakerCode
            // 
            this.tbMakerCode.Location = new System.Drawing.Point(101, 55);
            this.tbMakerCode.MaxLength = 2;
            this.tbMakerCode.Name = "tbMakerCode";
            this.tbMakerCode.Size = new System.Drawing.Size(244, 20);
            this.tbMakerCode.TabIndex = 3;
            // 
            // tbGameCode
            // 
            this.tbGameCode.Location = new System.Drawing.Point(101, 31);
            this.tbGameCode.MaxLength = 4;
            this.tbGameCode.Name = "tbGameCode";
            this.tbGameCode.Size = new System.Drawing.Size(244, 20);
            this.tbGameCode.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Game Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Game Code:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Maker Code:";
            // 
            // tbGameName
            // 
            this.tbGameName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbGameName.Location = new System.Drawing.Point(101, 7);
            this.tbGameName.MaxLength = 991;
            this.tbGameName.Name = "tbGameName";
            this.tbGameName.Size = new System.Drawing.Size(244, 20);
            this.tbGameName.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.bannerDataPanel);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 103);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(358, 342);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Banner Data";
            // 
            // bannerDataPanel
            // 
            this.bannerDataPanel.ColumnCount = 2;
            this.bannerDataPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.55682F));
            this.bannerDataPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.44318F));
            this.bannerDataPanel.Controls.Add(this.tbShortName, 1, 2);
            this.bannerDataPanel.Controls.Add(this.label4, 0, 0);
            this.bannerDataPanel.Controls.Add(this.label5, 0, 1);
            this.bannerDataPanel.Controls.Add(this.label6, 0, 2);
            this.bannerDataPanel.Controls.Add(this.label7, 0, 3);
            this.bannerDataPanel.Controls.Add(this.label8, 0, 4);
            this.bannerDataPanel.Controls.Add(this.label9, 0, 5);
            this.bannerDataPanel.Controls.Add(this.label12, 0, 6);
            this.bannerDataPanel.Controls.Add(this.panel2, 1, 7);
            this.bannerDataPanel.Controls.Add(this.label11, 0, 7);
            this.bannerDataPanel.Controls.Add(this.tbShortMaker, 1, 3);
            this.bannerDataPanel.Controls.Add(this.tbLongName, 1, 4);
            this.bannerDataPanel.Controls.Add(this.tbLongMaker, 1, 5);
            this.bannerDataPanel.Controls.Add(this.tbDescription, 1, 6);
            this.bannerDataPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bannerDataPanel.Enabled = false;
            this.bannerDataPanel.Location = new System.Drawing.Point(3, 16);
            this.bannerDataPanel.Margin = new System.Windows.Forms.Padding(6);
            this.bannerDataPanel.Name = "bannerDataPanel";
            this.bannerDataPanel.Padding = new System.Windows.Forms.Padding(4);
            this.bannerDataPanel.RowCount = 8;
            this.bannerDataPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.bannerDataPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.bannerDataPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.bannerDataPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.bannerDataPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.bannerDataPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.bannerDataPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.bannerDataPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.bannerDataPanel.Size = new System.Drawing.Size(352, 323);
            this.bannerDataPanel.TabIndex = 1;
            // 
            // tbShortName
            // 
            this.tbShortName.Location = new System.Drawing.Point(101, 55);
            this.tbShortName.MaxLength = 31;
            this.tbShortName.Name = "tbShortName";
            this.tbShortName.Size = new System.Drawing.Size(244, 20);
            this.tbShortName.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Banner Found:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Language:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Short name:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 76);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Short maker:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 100);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Long name:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 124);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(66, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Long maker:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 148);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(63, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Description:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.buttonExport);
            this.panel2.Controls.Add(this.buttonImport);
            this.panel2.Controls.Add(this.pictureIconBox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(101, 199);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(244, 117);
            this.panel2.TabIndex = 4;
            // 
            // buttonExport
            // 
            this.buttonExport.Location = new System.Drawing.Point(105, 73);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(90, 23);
            this.buttonExport.TabIndex = 2;
            this.buttonExport.Text = "Export Image";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // buttonImport
            // 
            this.buttonImport.Location = new System.Drawing.Point(3, 73);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(90, 23);
            this.buttonImport.TabIndex = 2;
            this.buttonImport.Text = "Import Image";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // pictureIconBox
            // 
            this.pictureIconBox.Location = new System.Drawing.Point(3, 3);
            this.pictureIconBox.Name = "pictureIconBox";
            this.pictureIconBox.Size = new System.Drawing.Size(192, 64);
            this.pictureIconBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureIconBox.TabIndex = 1;
            this.pictureIconBox.TabStop = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 196);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(76, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Banner Image:";
            // 
            // tbShortMaker
            // 
            this.tbShortMaker.Location = new System.Drawing.Point(101, 79);
            this.tbShortMaker.MaxLength = 31;
            this.tbShortMaker.Name = "tbShortMaker";
            this.tbShortMaker.Size = new System.Drawing.Size(244, 20);
            this.tbShortMaker.TabIndex = 3;
            // 
            // tbLongName
            // 
            this.tbLongName.Location = new System.Drawing.Point(101, 103);
            this.tbLongName.MaxLength = 63;
            this.tbLongName.Name = "tbLongName";
            this.tbLongName.Size = new System.Drawing.Size(244, 20);
            this.tbLongName.TabIndex = 3;
            // 
            // tbLongMaker
            // 
            this.tbLongMaker.Location = new System.Drawing.Point(101, 127);
            this.tbLongMaker.MaxLength = 63;
            this.tbLongMaker.Name = "tbLongMaker";
            this.tbLongMaker.Size = new System.Drawing.Size(244, 20);
            this.tbLongMaker.TabIndex = 3;
            // 
            // tbDescription
            // 
            this.tbDescription.Location = new System.Drawing.Point(101, 151);
            this.tbDescription.MaxLength = 127;
            this.tbDescription.Multiline = true;
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(244, 42);
            this.tbDescription.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.splitter2);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(358, 445);
            this.panel1.TabIndex = 4;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(0, 100);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(358, 3);
            this.splitter2.TabIndex = 4;
            this.splitter2.TabStop = false;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(358, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 445);
            this.splitter1.TabIndex = 5;
            this.splitter1.TabStop = false;
            // 
            // folderContextMenu
            // 
            this.folderContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importFileToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.folderContextMenu.Name = "folderContextMenu";
            this.folderContextMenu.Size = new System.Drawing.Size(145, 70);
            // 
            // importFileToolStripMenuItem
            // 
            this.importFileToolStripMenuItem.Name = "importFileToolStripMenuItem";
            this.importFileToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.importFileToolStripMenuItem.Text = "Import File";
            this.importFileToolStripMenuItem.Click += new System.EventHandler(this.importFileToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.exportToolStripMenuItem.Text = "Export Folder";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.deleteToolStripMenuItem.Text = "Delete Folder";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // fileContextMenu
            // 
            this.fileContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem1,
            this.deleteToolStripMenuItem1});
            this.fileContextMenu.Name = "fileContextMenu";
            this.fileContextMenu.Size = new System.Drawing.Size(132, 70);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.importToolStripMenuItem.Text = "Import File";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem1
            // 
            this.exportToolStripMenuItem1.Name = "exportToolStripMenuItem1";
            this.exportToolStripMenuItem1.Size = new System.Drawing.Size(131, 22);
            this.exportToolStripMenuItem1.Text = "Export File";
            this.exportToolStripMenuItem1.Click += new System.EventHandler(this.exportToolStripMenuItem1_Click);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(131, 22);
            this.deleteToolStripMenuItem1.Text = "Delete File";
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
            // 
            // rootContextMenu
            // 
            this.rootContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importFileToolStripMenuItem1,
            this.exportToolStripMenuItem2});
            this.rootContextMenu.Name = "rootContextMenu";
            this.rootContextMenu.Size = new System.Drawing.Size(137, 48);
            // 
            // importFileToolStripMenuItem1
            // 
            this.importFileToolStripMenuItem1.Name = "importFileToolStripMenuItem1";
            this.importFileToolStripMenuItem1.Size = new System.Drawing.Size(136, 22);
            this.importFileToolStripMenuItem1.Text = "Import File";
            this.importFileToolStripMenuItem1.Click += new System.EventHandler(this.importFileToolStripMenuItem1_Click);
            // 
            // exportToolStripMenuItem2
            // 
            this.exportToolStripMenuItem2.Name = "exportToolStripMenuItem2";
            this.exportToolStripMenuItem2.Size = new System.Drawing.Size(136, 22);
            this.exportToolStripMenuItem2.Text = "Export Root";
            this.exportToolStripMenuItem2.Click += new System.EventHandler(this.exportToolStripMenuItem2_Click);
            // 
            // ISOEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Name = "ISOEditor";
            this.Size = new System.Drawing.Size(637, 445);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.gameDataPanel.ResumeLayout(false);
            this.gameDataPanel.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.bannerDataPanel.ResumeLayout(false);
            this.bannerDataPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureIconBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.folderContextMenu.ResumeLayout(false);
            this.fileContextMenu.ResumeLayout(false);
            this.rootContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TreeView fileTree;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.TableLayoutPanel gameDataPanel;
        private System.Windows.Forms.TextBox tbMakerCode;
        private System.Windows.Forms.TextBox tbGameCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbGameName;
        private System.Windows.Forms.TableLayoutPanel bannerDataPanel;
        private System.Windows.Forms.TextBox tbShortName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureIconBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbShortMaker;
        private System.Windows.Forms.TextBox tbLongName;
        private System.Windows.Forms.TextBox tbLongMaker;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.ContextMenuStrip folderContextMenu;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip fileContextMenu;
        private System.Windows.Forms.ContextMenuStrip rootContextMenu;
        private System.Windows.Forms.ToolStripMenuItem importFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem importFileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem2;
    }
}
