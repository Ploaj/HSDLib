namespace HSDRawViewer.GUI.Plugins
{
    partial class JOBJEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JOBJEditor));
            this.listDOBJ = new System.Windows.Forms.CheckedListBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.treeJOBJ = new System.Windows.Forms.TreeView();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.vertexColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.previewBox = new System.Windows.Forms.GroupBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.importModelFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportModelToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.mainRender = new System.Windows.Forms.ToolStripButton();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.addDummyDOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listDOBJ
            // 
            this.listDOBJ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listDOBJ.Location = new System.Drawing.Point(3, 28);
            this.listDOBJ.Name = "listDOBJ";
            this.listDOBJ.Size = new System.Drawing.Size(316, 100);
            this.listDOBJ.TabIndex = 0;
            this.listDOBJ.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ListDOBJ_ItemCheck);
            this.listDOBJ.BindingContextChanged += new System.EventHandler(this.listDOBJ_BindingContextChanged);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(11, 19);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(600, 237);
            this.propertyGrid1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(330, 157);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.treeJOBJ);
            this.tabPage1.Controls.Add(this.toolStrip3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(322, 131);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "JOBJs";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // treeJOBJ
            // 
            this.treeJOBJ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeJOBJ.Location = new System.Drawing.Point(3, 28);
            this.treeJOBJ.Name = "treeJOBJ";
            this.treeJOBJ.Size = new System.Drawing.Size(316, 100);
            this.treeJOBJ.TabIndex = 0;
            // 
            // toolStrip3
            // 
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip3.Location = new System.Drawing.Point(3, 3);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(316, 25);
            this.toolStrip3.TabIndex = 1;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Checked = true;
            this.toolStripButton1.CheckOnClick = true;
            this.toolStripButton1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(83, 22);
            this.toolStripButton1.Text = "Render Bones";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listDOBJ);
            this.tabPage2.Controls.Add(this.toolStrip2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(322, 131);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "DOBJs";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton3,
            this.toolStripLabel1,
            this.toolStripComboBox1,
            this.toolStripDropDownButton1});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(316, 25);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(47, 22);
            this.toolStripLabel1.Text = "Render:";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox1.Items.AddRange(new object[] {
            "All",
            "Selected",
            "None"});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 25);
            this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vertexColorsToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(49, 22);
            this.toolStripDropDownButton1.Text = "Show";
            // 
            // vertexColorsToolStripMenuItem
            // 
            this.vertexColorsToolStripMenuItem.Checked = true;
            this.vertexColorsToolStripMenuItem.CheckOnClick = true;
            this.vertexColorsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.vertexColorsToolStripMenuItem.Name = "vertexColorsToolStripMenuItem";
            this.vertexColorsToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.vertexColorsToolStripMenuItem.Text = "Vertex Colors";
            this.vertexColorsToolStripMenuItem.Click += new System.EventHandler(this.vertexColorsToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.propertyGrid1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 185);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(620, 262);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // previewBox
            // 
            this.previewBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewBox.Location = new System.Drawing.Point(330, 25);
            this.previewBox.Margin = new System.Windows.Forms.Padding(5);
            this.previewBox.Name = "previewBox";
            this.previewBox.Size = new System.Drawing.Size(290, 157);
            this.previewBox.TabIndex = 4;
            this.previewBox.TabStop = false;
            this.previewBox.Text = "Preview";
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(330, 25);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 157);
            this.splitter2.TabIndex = 6;
            this.splitter2.TabStop = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton2,
            this.toolStripButton2,
            this.mainRender});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(620, 25);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importModelFromFileToolStripMenuItem,
            this.exportModelToFileToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(38, 22);
            this.toolStripDropDownButton2.Text = "File";
            // 
            // importModelFromFileToolStripMenuItem
            // 
            this.importModelFromFileToolStripMenuItem.Name = "importModelFromFileToolStripMenuItem";
            this.importModelFromFileToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.importModelFromFileToolStripMenuItem.Text = "Import Model From File";
            this.importModelFromFileToolStripMenuItem.Click += new System.EventHandler(this.importModelFromFileToolStripMenuItem_Click);
            // 
            // exportModelToFileToolStripMenuItem
            // 
            this.exportModelToFileToolStripMenuItem.Name = "exportModelToFileToolStripMenuItem";
            this.exportModelToFileToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.exportModelToFileToolStripMenuItem.Text = "Export Model To File";
            this.exportModelToFileToolStripMenuItem.Click += new System.EventHandler(this.exportModelToFileToolStripMenuItem_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(77, 22);
            this.toolStripButton2.Text = "Edit Material";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // mainRender
            // 
            this.mainRender.CheckOnClick = true;
            this.mainRender.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mainRender.Image = ((System.Drawing.Image)(resources.GetObject("mainRender.Image")));
            this.mainRender.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mainRender.Name = "mainRender";
            this.mainRender.Size = new System.Drawing.Size(111, 22);
            this.mainRender.Text = "Render In Viewport";
            this.mainRender.CheckStateChanged += new System.EventHandler(this.mainRender_CheckStateChanged);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 182);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(620, 3);
            this.splitter1.TabIndex = 8;
            this.splitter1.TabStop = false;
            // 
            // toolStripDropDownButton3
            // 
            this.toolStripDropDownButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addDummyDOBJToolStripMenuItem});
            this.toolStripDropDownButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton3.Image")));
            this.toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            this.toolStripDropDownButton3.Size = new System.Drawing.Size(62, 22);
            this.toolStripDropDownButton3.Text = "Options";
            // 
            // addDummyDOBJToolStripMenuItem
            // 
            this.addDummyDOBJToolStripMenuItem.Name = "addDummyDOBJToolStripMenuItem";
            this.addDummyDOBJToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.addDummyDOBJToolStripMenuItem.Text = "Add Dummy DOBJ";
            this.addDummyDOBJToolStripMenuItem.Click += new System.EventHandler(this.addDummyDOBJToolStripMenuItem_Click);
            // 
            // JOBJEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 447);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.previewBox);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.groupBox1);
            this.Name = "JOBJEditor";
            this.TabText = "JOBJEditor";
            this.Text = "JOBJEditor";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox listDOBJ;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView treeJOBJ;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem vertexColorsToolStripMenuItem;
        private System.Windows.Forms.GroupBox previewBox;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem importModelFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportModelToFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripButton mainRender;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton3;
        private System.Windows.Forms.ToolStripMenuItem addDummyDOBJToolStripMenuItem;
    }
}