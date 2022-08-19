namespace HSDRawViewer.GUI.Extra
{
    partial class MOBJEditor
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
            this.listTexture = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.cbEnablePP = new System.Windows.Forms.CheckBox();
            this.buttonDiffuse = new System.Windows.Forms.Button();
            this.buttonSpecular = new System.Windows.Forms.Button();
            this.buttonAmbient = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Textures = new System.Windows.Forms.TabPage();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.propertyTexture = new System.Windows.Forms.PropertyGrid();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tevPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.enableTEVCB = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonSaveTexture = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.buttonReplace = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.Colors = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbAlpha = new System.Windows.Forms.TextBox();
            this.tbShine = new System.Windows.Forms.TextBox();
            this.PixelProcessing = new System.Windows.Forms.TabPage();
            this.propertyPixel = new System.Windows.Forms.PropertyGrid();
            this.tabControl1.SuspendLayout();
            this.Textures.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.Colors.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.PixelProcessing.SuspendLayout();
            this.SuspendLayout();
            // 
            // listTexture
            // 
            this.listTexture.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listTexture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listTexture.FullRowSelect = true;
            this.listTexture.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listTexture.Location = new System.Drawing.Point(4, 28);
            this.listTexture.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listTexture.MultiSelect = false;
            this.listTexture.Name = "listTexture";
            this.listTexture.Size = new System.Drawing.Size(576, 566);
            this.listTexture.TabIndex = 0;
            this.listTexture.UseCompatibleStateImageBehavior = false;
            this.listTexture.View = System.Windows.Forms.View.Details;
            this.listTexture.SelectedIndexChanged += new System.EventHandler(this.listTexture_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 100;
            // 
            // cbEnablePP
            // 
            this.cbEnablePP.AutoSize = true;
            this.cbEnablePP.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbEnablePP.Location = new System.Drawing.Point(0, 0);
            this.cbEnablePP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbEnablePP.Name = "cbEnablePP";
            this.cbEnablePP.Size = new System.Drawing.Size(1084, 19);
            this.cbEnablePP.TabIndex = 2;
            this.cbEnablePP.Text = "Enable Pixel Processing";
            this.cbEnablePP.UseVisualStyleBackColor = true;
            this.cbEnablePP.CheckedChanged += new System.EventHandler(this.cbEnablePP_CheckedChanged);
            // 
            // buttonDiffuse
            // 
            this.buttonDiffuse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonDiffuse.Location = new System.Drawing.Point(113, 31);
            this.buttonDiffuse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonDiffuse.Name = "buttonDiffuse";
            this.buttonDiffuse.Size = new System.Drawing.Size(410, 22);
            this.buttonDiffuse.TabIndex = 3;
            this.buttonDiffuse.UseVisualStyleBackColor = true;
            this.buttonDiffuse.Click += new System.EventHandler(this.buttonDiffuse_Click);
            // 
            // buttonSpecular
            // 
            this.buttonSpecular.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSpecular.Location = new System.Drawing.Point(113, 59);
            this.buttonSpecular.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonSpecular.Name = "buttonSpecular";
            this.buttonSpecular.Size = new System.Drawing.Size(410, 22);
            this.buttonSpecular.TabIndex = 3;
            this.buttonSpecular.UseVisualStyleBackColor = true;
            this.buttonSpecular.Click += new System.EventHandler(this.buttonSpecular_Click);
            // 
            // buttonAmbient
            // 
            this.buttonAmbient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonAmbient.Location = new System.Drawing.Point(113, 3);
            this.buttonAmbient.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonAmbient.Name = "buttonAmbient";
            this.buttonAmbient.Size = new System.Drawing.Size(410, 22);
            this.buttonAmbient.TabIndex = 3;
            this.buttonAmbient.UseVisualStyleBackColor = true;
            this.buttonAmbient.Click += new System.EventHandler(this.buttonAmbient_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Textures);
            this.tabControl1.Controls.Add(this.Colors);
            this.tabControl1.Controls.Add(this.PixelProcessing);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1092, 625);
            this.tabControl1.TabIndex = 4;
            // 
            // Textures
            // 
            this.Textures.Controls.Add(this.splitter1);
            this.Textures.Controls.Add(this.listTexture);
            this.Textures.Controls.Add(this.tabControl2);
            this.Textures.Controls.Add(this.toolStrip1);
            this.Textures.Location = new System.Drawing.Point(4, 24);
            this.Textures.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Textures.Name = "Textures";
            this.Textures.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Textures.Size = new System.Drawing.Size(1084, 597);
            this.Textures.TabIndex = 0;
            this.Textures.Text = "Textures";
            this.Textures.UseVisualStyleBackColor = true;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(576, 28);
            this.splitter1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(4, 566);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Controls.Add(this.tabPage2);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Right;
            this.tabControl2.Location = new System.Drawing.Point(580, 28);
            this.tabControl2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(500, 566);
            this.tabControl2.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.propertyTexture);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage1.Size = new System.Drawing.Size(492, 538);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Properties";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // propertyTexture
            // 
            this.propertyTexture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyTexture.Location = new System.Drawing.Point(4, 3);
            this.propertyTexture.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.propertyTexture.Name = "propertyTexture";
            this.propertyTexture.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyTexture.Size = new System.Drawing.Size(484, 532);
            this.propertyTexture.TabIndex = 1;
            this.propertyTexture.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyTexture_PropertyValueChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tevPropertyGrid);
            this.tabPage2.Controls.Add(this.enableTEVCB);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage2.Size = new System.Drawing.Size(492, 538);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "TEV";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tevPropertyGrid
            // 
            this.tevPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tevPropertyGrid.Location = new System.Drawing.Point(4, 22);
            this.tevPropertyGrid.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tevPropertyGrid.Name = "tevPropertyGrid";
            this.tevPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.tevPropertyGrid.Size = new System.Drawing.Size(484, 513);
            this.tevPropertyGrid.TabIndex = 3;
            this.tevPropertyGrid.Visible = false;
            // 
            // enableTEVCB
            // 
            this.enableTEVCB.AutoSize = true;
            this.enableTEVCB.Dock = System.Windows.Forms.DockStyle.Top;
            this.enableTEVCB.Location = new System.Drawing.Point(4, 3);
            this.enableTEVCB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.enableTEVCB.Name = "enableTEVCB";
            this.enableTEVCB.Size = new System.Drawing.Size(484, 19);
            this.enableTEVCB.TabIndex = 4;
            this.enableTEVCB.Text = "Enable TEV";
            this.enableTEVCB.UseVisualStyleBackColor = true;
            this.enableTEVCB.CheckedChanged += new System.EventHandler(this.enableTEVCB_CheckedChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSaveTexture,
            this.toolStripButton1,
            this.buttonReplace,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4});
            this.toolStrip1.Location = new System.Drawing.Point(4, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1076, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonSaveTexture
            // 
            this.buttonSaveTexture.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.buttonSaveTexture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSaveTexture.Name = "buttonSaveTexture";
            this.buttonSaveTexture.Size = new System.Drawing.Size(61, 22);
            this.buttonSaveTexture.Text = "Export";
            this.buttonSaveTexture.Click += new System.EventHandler(this.buttonSaveTexture_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::HSDRawViewer.Properties.Resources.ts_add;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(63, 22);
            this.toolStripButton1.Text = "Import";
            this.toolStripButton1.Click += new System.EventHandler(this.importTexture_Click);
            // 
            // buttonReplace
            // 
            this.buttonReplace.Image = global::HSDRawViewer.Properties.Resources.ico_replace;
            this.buttonReplace.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonReplace.Name = "buttonReplace";
            this.buttonReplace.Size = new System.Drawing.Size(68, 22);
            this.buttonReplace.Text = "Replace";
            this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = global::HSDRawViewer.Properties.Resources.ts_subtract;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(70, 22);
            this.toolStripButton2.Text = "Remove";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = global::HSDRawViewer.Properties.Resources.ts_up;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "toolStripButton3";
            this.toolStripButton3.Click += new System.EventHandler(this.moveToolStripMenuItem_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = global::HSDRawViewer.Properties.Resources.ts_down;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton4.Text = "toolStripButton4";
            this.toolStripButton4.Click += new System.EventHandler(this.downToolStripMenuItem_Click);
            // 
            // Colors
            // 
            this.Colors.Controls.Add(this.tableLayoutPanel1);
            this.Colors.Location = new System.Drawing.Point(4, 24);
            this.Colors.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Colors.Name = "Colors";
            this.Colors.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Colors.Size = new System.Drawing.Size(1084, 597);
            this.Colors.TabIndex = 1;
            this.Colors.Text = "Colors";
            this.Colors.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.79646F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 79.20354F));
            this.tableLayoutPanel1.Controls.Add(this.buttonAmbient, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonSpecular, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonDiffuse, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.tbAlpha, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.tbShine, 1, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 7);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(527, 141);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Ambient Color:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 34);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Diffuse Color:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 62);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Specular Color:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(64, 90);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "Alpha:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 119);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Shininess:";
            // 
            // tbAlpha
            // 
            this.tbAlpha.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAlpha.Location = new System.Drawing.Point(113, 87);
            this.tbAlpha.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbAlpha.Name = "tbAlpha";
            this.tbAlpha.Size = new System.Drawing.Size(410, 23);
            this.tbAlpha.TabIndex = 5;
            this.tbAlpha.TextChanged += new System.EventHandler(this.tbAlpha_TextChanged);
            // 
            // tbShine
            // 
            this.tbShine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbShine.Location = new System.Drawing.Point(113, 115);
            this.tbShine.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbShine.Name = "tbShine";
            this.tbShine.Size = new System.Drawing.Size(410, 23);
            this.tbShine.TabIndex = 5;
            this.tbShine.TextChanged += new System.EventHandler(this.tbShine_TextChanged);
            // 
            // PixelProcessing
            // 
            this.PixelProcessing.Controls.Add(this.propertyPixel);
            this.PixelProcessing.Controls.Add(this.cbEnablePP);
            this.PixelProcessing.Location = new System.Drawing.Point(4, 24);
            this.PixelProcessing.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.PixelProcessing.Name = "PixelProcessing";
            this.PixelProcessing.Size = new System.Drawing.Size(1084, 597);
            this.PixelProcessing.TabIndex = 2;
            this.PixelProcessing.Text = "PixelProcessing";
            this.PixelProcessing.UseVisualStyleBackColor = true;
            // 
            // propertyPixel
            // 
            this.propertyPixel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyPixel.Location = new System.Drawing.Point(0, 19);
            this.propertyPixel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.propertyPixel.Name = "propertyPixel";
            this.propertyPixel.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyPixel.Size = new System.Drawing.Size(1084, 578);
            this.propertyPixel.TabIndex = 3;
            // 
            // MOBJEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MOBJEditor";
            this.Size = new System.Drawing.Size(1092, 625);
            this.tabControl1.ResumeLayout(false);
            this.Textures.ResumeLayout(false);
            this.Textures.PerformLayout();
            this.tabControl2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.Colors.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.PixelProcessing.ResumeLayout(false);
            this.PixelProcessing.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView listTexture;
        private System.Windows.Forms.CheckBox cbEnablePP;
        private System.Windows.Forms.Button buttonDiffuse;
        private System.Windows.Forms.Button buttonSpecular;
        private System.Windows.Forms.Button buttonAmbient;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Textures;
        private System.Windows.Forms.TabPage Colors;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage PixelProcessing;
        private System.Windows.Forms.TextBox tbAlpha;
        private System.Windows.Forms.TextBox tbShine;
        private System.Windows.Forms.PropertyGrid propertyTexture;
        private System.Windows.Forms.PropertyGrid propertyPixel;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton buttonReplace;
        private System.Windows.Forms.ToolStripButton buttonSaveTexture;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PropertyGrid tevPropertyGrid;
        private System.Windows.Forms.CheckBox enableTEVCB;
    }
}