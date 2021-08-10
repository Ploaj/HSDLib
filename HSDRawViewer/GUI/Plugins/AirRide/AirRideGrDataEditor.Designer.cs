namespace HSDRawViewer.GUI.Plugins.AirRide
{
    partial class AirRideGrDataEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AirRideGrDataEditor));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.importFromKCLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importFromKMPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.importFromOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.recalculateCollisionFlagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsSaveButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.modeComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.renderCollisionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zonesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.arrayMemberEditor1 = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.generateRangeSplinesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton2,
            this.toolStripDropDownButton3,
            this.tsSaveButton,
            this.toolStripLabel1,
            this.modeComboBox,
            this.toolStripDropDownButton1,
            this.toolStripTextBox1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(769, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importFromKCLToolStripMenuItem,
            this.importFromKMPToolStripMenuItem,
            this.toolStripSeparator2,
            this.importFromOBJToolStripMenuItem,
            this.exportOBJToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(38, 22);
            this.toolStripDropDownButton2.Text = "File";
            // 
            // importFromKCLToolStripMenuItem
            // 
            this.importFromKCLToolStripMenuItem.Name = "importFromKCLToolStripMenuItem";
            this.importFromKCLToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.importFromKCLToolStripMenuItem.Text = "Import From KCL";
            this.importFromKCLToolStripMenuItem.Click += new System.EventHandler(this.importFromKCLToolStripMenuItem_Click);
            // 
            // importFromKMPToolStripMenuItem
            // 
            this.importFromKMPToolStripMenuItem.Name = "importFromKMPToolStripMenuItem";
            this.importFromKMPToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.importFromKMPToolStripMenuItem.Text = "Import From KMP";
            this.importFromKMPToolStripMenuItem.Click += new System.EventHandler(this.importFromKMPToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(179, 6);
            // 
            // importFromOBJToolStripMenuItem
            // 
            this.importFromOBJToolStripMenuItem.Name = "importFromOBJToolStripMenuItem";
            this.importFromOBJToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.importFromOBJToolStripMenuItem.Text = "Import Collision OBJ";
            this.importFromOBJToolStripMenuItem.Click += new System.EventHandler(this.importFromOBJToolStripMenuItem_Click);
            // 
            // exportOBJToolStripMenuItem
            // 
            this.exportOBJToolStripMenuItem.Name = "exportOBJToolStripMenuItem";
            this.exportOBJToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.exportOBJToolStripMenuItem.Text = "Export Collision OBJ";
            this.exportOBJToolStripMenuItem.Click += new System.EventHandler(this.exportOBJToolStripMenuItem_Click);
            // 
            // toolStripDropDownButton3
            // 
            this.toolStripDropDownButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recalculateCollisionFlagsToolStripMenuItem,
            this.toolStripSeparator3,
            this.generateRangeSplinesToolStripMenuItem1});
            this.toolStripDropDownButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton3.Image")));
            this.toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            this.toolStripDropDownButton3.Size = new System.Drawing.Size(71, 22);
            this.toolStripDropDownButton3.Text = "Collisions";
            // 
            // recalculateCollisionFlagsToolStripMenuItem
            // 
            this.recalculateCollisionFlagsToolStripMenuItem.Name = "recalculateCollisionFlagsToolStripMenuItem";
            this.recalculateCollisionFlagsToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.recalculateCollisionFlagsToolStripMenuItem.Text = "Recalculate Collision Flags";
            this.recalculateCollisionFlagsToolStripMenuItem.Click += new System.EventHandler(this.recalculateCollisionFlagsToolStripMenuItem_Click);
            // 
            // tsSaveButton
            // 
            this.tsSaveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsSaveButton.Image = ((System.Drawing.Image)(resources.GetObject("tsSaveButton.Image")));
            this.tsSaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsSaveButton.Name = "tsSaveButton";
            this.tsSaveButton.Size = new System.Drawing.Size(35, 22);
            this.tsSaveButton.Text = "Save";
            this.tsSaveButton.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(41, 22);
            this.toolStripLabel1.Text = "Mode:";
            // 
            // modeComboBox
            // 
            this.modeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(121, 25);
            this.modeComboBox.SelectedIndexChanged += new System.EventHandler(this.modeComboBox_SelectedIndexChanged);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renderCollisionsToolStripMenuItem,
            this.zonesToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(57, 22);
            this.toolStripDropDownButton1.Text = "Render";
            // 
            // renderCollisionsToolStripMenuItem
            // 
            this.renderCollisionsToolStripMenuItem.Checked = true;
            this.renderCollisionsToolStripMenuItem.CheckOnClick = true;
            this.renderCollisionsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.renderCollisionsToolStripMenuItem.Name = "renderCollisionsToolStripMenuItem";
            this.renderCollisionsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.renderCollisionsToolStripMenuItem.Text = "Collisions";
            // 
            // zonesToolStripMenuItem
            // 
            this.zonesToolStripMenuItem.Checked = true;
            this.zonesToolStripMenuItem.CheckOnClick = true;
            this.zonesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.zonesToolStripMenuItem.Name = "zonesToolStripMenuItem";
            this.zonesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.zonesToolStripMenuItem.Text = "Zones";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(76, 25);
            this.toolStripTextBox1.TextChanged += new System.EventHandler(this.toolStripTextBox1_TextChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(769, 482);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage1.Size = new System.Drawing.Size(761, 456);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Data Viewer";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.arrayMemberEditor1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(228, 450);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Editor:";
            // 
            // arrayMemberEditor1
            // 
            this.arrayMemberEditor1.DisplayItemImages = false;
            this.arrayMemberEditor1.DisplayItemIndices = false;
            this.arrayMemberEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arrayMemberEditor1.EnablePropertyViewDescription = true;
            this.arrayMemberEditor1.ImageHeight = ((ushort)(24));
            this.arrayMemberEditor1.ImageWidth = ((ushort)(24));
            this.arrayMemberEditor1.ItemHeight = 13;
            this.arrayMemberEditor1.ItemIndexOffset = 0;
            this.arrayMemberEditor1.Location = new System.Drawing.Point(3, 16);
            this.arrayMemberEditor1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.arrayMemberEditor1.Name = "arrayMemberEditor1";
            this.arrayMemberEditor1.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.arrayMemberEditor1.Size = new System.Drawing.Size(222, 431);
            this.arrayMemberEditor1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage2.Size = new System.Drawing.Size(761, 456);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(210, 6);
            // 
            // generateRangeSplinesToolStripMenuItem1
            // 
            this.generateRangeSplinesToolStripMenuItem1.Name = "generateRangeSplinesToolStripMenuItem1";
            this.generateRangeSplinesToolStripMenuItem1.Size = new System.Drawing.Size(213, 22);
            this.generateRangeSplinesToolStripMenuItem1.Text = "Generate Range Splines";
            this.generateRangeSplinesToolStripMenuItem1.Click += new System.EventHandler(this.generateRangeSplinesToolStripMenuItem_Click);
            // 
            // AirRideGrDataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 507);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "AirRideGrDataEditor";
            this.TabText = "AirRideGrDataEditor";
            this.Text = "AirRideGrDataEditor";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem renderCollisionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zonesToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripComboBox modeComboBox;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private ArrayMemberEditor arrayMemberEditor1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem importFromKCLToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton tsSaveButton;
        private System.Windows.Forms.ToolStripMenuItem importFromKMPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importFromOBJToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exportOBJToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton3;
        private System.Windows.Forms.ToolStripMenuItem recalculateCollisionFlagsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem generateRangeSplinesToolStripMenuItem1;
    }
}