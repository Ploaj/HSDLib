namespace HSDRawViewer.GUI.Plugins.Melee
{
    partial class SISMenuDataEditor
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
            panel1 = new System.Windows.Forms.Panel();
            textBox1 = new System.Windows.Forms.TextBox();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            arrayMemberEditor1 = new ArrayMemberEditor();
            splitter1 = new System.Windows.Forms.Splitter();
            tabPage2 = new System.Windows.Forms.TabPage();
            fontTable = new System.Windows.Forms.PictureBox();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            vS2015BlueTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)fontTable).BeginInit();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(4, 5);
            panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(767, 222);
            panel1.TabIndex = 0;
            panel1.Paint += panel1_Paint;
            // 
            // textBox1
            // 
            textBox1.Dock = System.Windows.Forms.DockStyle.Top;
            textBox1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            textBox1.Location = new System.Drawing.Point(4, 227);
            textBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(767, 156);
            textBox1.TabIndex = 3;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 27);
            tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(783, 670);
            tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(arrayMemberEditor1);
            tabPage1.Controls.Add(splitter1);
            tabPage1.Controls.Add(textBox1);
            tabPage1.Controls.Add(panel1);
            tabPage1.Location = new System.Drawing.Point(4, 29);
            tabPage1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tabPage1.Size = new System.Drawing.Size(775, 637);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Text Strings";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // arrayMemberEditor1
            // 
            arrayMemberEditor1.DisplayItemImages = false;
            arrayMemberEditor1.DisplayItemIndices = true;
            arrayMemberEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            arrayMemberEditor1.EnablePropertyView = false;
            arrayMemberEditor1.EnablePropertyViewDescription = false;
            arrayMemberEditor1.ImageHeight = (ushort)24;
            arrayMemberEditor1.ImageWidth = (ushort)24;
            arrayMemberEditor1.InsertCloneAfterSelected = false;
            arrayMemberEditor1.ItemHeight = 13;
            arrayMemberEditor1.ItemIndexOffset = 2;
            arrayMemberEditor1.Location = new System.Drawing.Point(4, 388);
            arrayMemberEditor1.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            arrayMemberEditor1.Name = "arrayMemberEditor1";
            arrayMemberEditor1.SelectionMode = System.Windows.Forms.SelectionMode.One;
            arrayMemberEditor1.Size = new System.Drawing.Size(767, 244);
            arrayMemberEditor1.TabIndex = 1;
            arrayMemberEditor1.SelectedObjectChanged += arrayMemberEditor1_SelectedObjectChanged;
            // 
            // splitter1
            // 
            splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            splitter1.Location = new System.Drawing.Point(4, 383);
            splitter1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            splitter1.Name = "splitter1";
            splitter1.Size = new System.Drawing.Size(767, 5);
            splitter1.TabIndex = 4;
            splitter1.TabStop = false;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(splitContainer1);
            tabPage2.Location = new System.Drawing.Point(4, 29);
            tabPage2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tabPage2.Size = new System.Drawing.Size(775, 637);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Extra Characters";
            tabPage2.UseVisualStyleBackColor = true;
            tabPage2.Click += tabPage2_Click;
            // 
            // fontTable
            // 
            fontTable.BackColor = System.Drawing.Color.Black;
            fontTable.Dock = System.Windows.Forms.DockStyle.Fill;
            fontTable.Location = new System.Drawing.Point(0, 0);
            fontTable.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            fontTable.Name = "fontTable";
            fontTable.Size = new System.Drawing.Size(513, 402);
            fontTable.TabIndex = 1;
            fontTable.TabStop = false;
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripButton1, toolStripButton2 });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(783, 27);
            toolStrip1.TabIndex = 5;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            toolStripButton1.Image = Properties.Resources.ico_save;
            toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new System.Drawing.Size(124, 24);
            toolStripButton1.Text = "Save Changes";
            toolStripButton1.Click += toolStripButton1_Click;
            // 
            // toolStripButton2
            // 
            toolStripButton2.Image = Properties.Resources.ico_known;
            toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButton2.Name = "toolStripButton2";
            toolStripButton2.Size = new System.Drawing.Size(65, 24);
            toolStripButton2.Text = "Help";
            toolStripButton2.Click += toolStripButton2_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(4, 5);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            splitContainer1.Size = new System.Drawing.Size(767, 627);
            splitContainer1.SplitterDistance = 513;
            splitContainer1.TabIndex = 2;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.Location = new System.Drawing.Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(fontTable);
            splitContainer2.Size = new System.Drawing.Size(513, 627);
            splitContainer2.SplitterDistance = 402;
            splitContainer2.TabIndex = 3;
            // 
            // SISMenuDataEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(783, 697);
            Controls.Add(tabControl1);
            Controls.Add(toolStrip1);
            DoubleBuffered = true;
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "SISMenuDataEditor";
            TabText = "SISMenuDataEditor";
            Text = "SISMenuDataEditor";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)fontTable).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private ArrayMemberEditor arrayMemberEditor1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.PictureBox fontTable;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme vS2015BlueTheme1;
        private System.Windows.Forms.SplitContainer splitContainer2;
    }
}