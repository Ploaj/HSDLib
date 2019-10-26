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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MOBJEditor));
            this.listTexture = new System.Windows.Forms.ListView();
            this.cbEnablePP = new System.Windows.Forms.CheckBox();
            this.buttonDiffuse = new System.Windows.Forms.Button();
            this.buttonSpecular = new System.Windows.Forms.Button();
            this.buttonAmbient = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Textures = new System.Windows.Forms.TabPage();
            this.Colors = new System.Windows.Forms.TabPage();
            this.PixelProcessing = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbAlpha = new System.Windows.Forms.TextBox();
            this.tbShine = new System.Windows.Forms.TextBox();
            this.propertyPixel = new System.Windows.Forms.PropertyGrid();
            this.propertyTexture = new System.Windows.Forms.PropertyGrid();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.moveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.Textures.SuspendLayout();
            this.Colors.SuspendLayout();
            this.PixelProcessing.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listTexture
            // 
            this.listTexture.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listTexture.Dock = System.Windows.Forms.DockStyle.Top;
            this.listTexture.FullRowSelect = true;
            this.listTexture.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listTexture.Location = new System.Drawing.Point(3, 28);
            this.listTexture.MultiSelect = false;
            this.listTexture.Name = "listTexture";
            this.listTexture.Size = new System.Drawing.Size(458, 164);
            this.listTexture.TabIndex = 0;
            this.listTexture.UseCompatibleStateImageBehavior = false;
            this.listTexture.View = System.Windows.Forms.View.Details;
            this.listTexture.SelectedIndexChanged += new System.EventHandler(this.listTexture_SelectedIndexChanged);
            // 
            // cbEnablePP
            // 
            this.cbEnablePP.AutoSize = true;
            this.cbEnablePP.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbEnablePP.Location = new System.Drawing.Point(0, 0);
            this.cbEnablePP.Name = "cbEnablePP";
            this.cbEnablePP.Size = new System.Drawing.Size(464, 17);
            this.cbEnablePP.TabIndex = 2;
            this.cbEnablePP.Text = "Enable Pixel Processing";
            this.cbEnablePP.UseVisualStyleBackColor = true;
            this.cbEnablePP.CheckedChanged += new System.EventHandler(this.cbEnablePP_CheckedChanged);
            // 
            // buttonDiffuse
            // 
            this.buttonDiffuse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonDiffuse.Location = new System.Drawing.Point(97, 27);
            this.buttonDiffuse.Name = "buttonDiffuse";
            this.buttonDiffuse.Size = new System.Drawing.Size(352, 18);
            this.buttonDiffuse.TabIndex = 3;
            this.buttonDiffuse.UseVisualStyleBackColor = true;
            this.buttonDiffuse.Click += new System.EventHandler(this.buttonDiffuse_Click);
            // 
            // buttonSpecular
            // 
            this.buttonSpecular.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonSpecular.Location = new System.Drawing.Point(97, 51);
            this.buttonSpecular.Name = "buttonSpecular";
            this.buttonSpecular.Size = new System.Drawing.Size(352, 18);
            this.buttonSpecular.TabIndex = 3;
            this.buttonSpecular.UseVisualStyleBackColor = true;
            this.buttonSpecular.Click += new System.EventHandler(this.buttonSpecular_Click);
            // 
            // buttonAmbient
            // 
            this.buttonAmbient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonAmbient.Location = new System.Drawing.Point(97, 3);
            this.buttonAmbient.Name = "buttonAmbient";
            this.buttonAmbient.Size = new System.Drawing.Size(352, 18);
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
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(472, 427);
            this.tabControl1.TabIndex = 4;
            // 
            // Textures
            // 
            this.Textures.Controls.Add(this.splitter1);
            this.Textures.Controls.Add(this.propertyTexture);
            this.Textures.Controls.Add(this.listTexture);
            this.Textures.Controls.Add(this.toolStrip1);
            this.Textures.Location = new System.Drawing.Point(4, 22);
            this.Textures.Name = "Textures";
            this.Textures.Padding = new System.Windows.Forms.Padding(3);
            this.Textures.Size = new System.Drawing.Size(464, 401);
            this.Textures.TabIndex = 0;
            this.Textures.Text = "Textures";
            this.Textures.UseVisualStyleBackColor = true;
            // 
            // Colors
            // 
            this.Colors.Controls.Add(this.tableLayoutPanel1);
            this.Colors.Location = new System.Drawing.Point(4, 22);
            this.Colors.Name = "Colors";
            this.Colors.Padding = new System.Windows.Forms.Padding(3);
            this.Colors.Size = new System.Drawing.Size(464, 401);
            this.Colors.TabIndex = 1;
            this.Colors.Text = "Colors";
            this.Colors.UseVisualStyleBackColor = true;
            // 
            // PixelProcessing
            // 
            this.PixelProcessing.Controls.Add(this.propertyPixel);
            this.PixelProcessing.Controls.Add(this.cbEnablePP);
            this.PixelProcessing.Location = new System.Drawing.Point(4, 22);
            this.PixelProcessing.Name = "PixelProcessing";
            this.PixelProcessing.Size = new System.Drawing.Size(464, 401);
            this.PixelProcessing.TabIndex = 2;
            this.PixelProcessing.Text = "PixelProcessing";
            this.PixelProcessing.UseVisualStyleBackColor = true;
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(452, 122);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Ambient Color:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Diffuse Color:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Specular Color:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Shininess:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(54, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Alpha:";
            // 
            // tbAlpha
            // 
            this.tbAlpha.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAlpha.Location = new System.Drawing.Point(97, 75);
            this.tbAlpha.Name = "tbAlpha";
            this.tbAlpha.Size = new System.Drawing.Size(352, 20);
            this.tbAlpha.TabIndex = 5;
            this.tbAlpha.TextChanged += new System.EventHandler(this.tbAlpha_TextChanged);
            // 
            // tbShine
            // 
            this.tbShine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbShine.Location = new System.Drawing.Point(97, 99);
            this.tbShine.Name = "tbShine";
            this.tbShine.Size = new System.Drawing.Size(352, 20);
            this.tbShine.TabIndex = 5;
            this.tbShine.TextChanged += new System.EventHandler(this.tbShine_TextChanged);
            // 
            // propertyPixel
            // 
            this.propertyPixel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyPixel.Location = new System.Drawing.Point(0, 17);
            this.propertyPixel.Name = "propertyPixel";
            this.propertyPixel.Size = new System.Drawing.Size(464, 384);
            this.propertyPixel.TabIndex = 3;
            // 
            // propertyTexture
            // 
            this.propertyTexture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyTexture.Location = new System.Drawing.Point(3, 192);
            this.propertyTexture.Name = "propertyTexture";
            this.propertyTexture.Size = new System.Drawing.Size(458, 206);
            this.propertyTexture.TabIndex = 1;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 100;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(3, 192);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(458, 3);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripDropDownButton1});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(458, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Enabled = false;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(47, 22);
            this.toolStripButton1.Text = "Import";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(54, 22);
            this.toolStripButton2.Text = "Remove";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveToolStripMenuItem,
            this.downToolStripMenuItem});
            this.toolStripDropDownButton1.Enabled = false;
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(50, 22);
            this.toolStripDropDownButton1.Text = "Move";
            // 
            // moveToolStripMenuItem
            // 
            this.moveToolStripMenuItem.Name = "moveToolStripMenuItem";
            this.moveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.moveToolStripMenuItem.Text = "Up";
            this.moveToolStripMenuItem.Click += new System.EventHandler(this.moveToolStripMenuItem_Click);
            // 
            // downToolStripMenuItem
            // 
            this.downToolStripMenuItem.Name = "downToolStripMenuItem";
            this.downToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.downToolStripMenuItem.Text = "Down";
            this.downToolStripMenuItem.Click += new System.EventHandler(this.downToolStripMenuItem_Click);
            // 
            // MOBJEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 427);
            this.Controls.Add(this.tabControl1);
            this.Name = "MOBJEditor";
            this.Text = "MOBJ Editor";
            this.tabControl1.ResumeLayout(false);
            this.Textures.ResumeLayout(false);
            this.Textures.PerformLayout();
            this.Colors.ResumeLayout(false);
            this.PixelProcessing.ResumeLayout(false);
            this.PixelProcessing.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
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
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem moveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downToolStripMenuItem;
    }
}