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
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.generateRangeSplinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
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
            this.toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.recalculateCollisionFlagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolStripButton1,
            this.toolStripLabel1,
            this.modeComboBox,
            this.toolStripDropDownButton1,
            this.toolStripTextBox1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1025, 28);
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
            this.exportOBJToolStripMenuItem,
            this.toolStripSeparator1,
            this.generateRangeSplinesToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(46, 25);
            this.toolStripDropDownButton2.Text = "File";
            // 
            // importFromKCLToolStripMenuItem
            // 
            this.importFromKCLToolStripMenuItem.Name = "importFromKCLToolStripMenuItem";
            this.importFromKCLToolStripMenuItem.Size = new System.Drawing.Size(249, 26);
            this.importFromKCLToolStripMenuItem.Text = "Import From KCL";
            this.importFromKCLToolStripMenuItem.Click += new System.EventHandler(this.importFromKCLToolStripMenuItem_Click);
            // 
            // importFromKMPToolStripMenuItem
            // 
            this.importFromKMPToolStripMenuItem.Name = "importFromKMPToolStripMenuItem";
            this.importFromKMPToolStripMenuItem.Size = new System.Drawing.Size(249, 26);
            this.importFromKMPToolStripMenuItem.Text = "Import From KMP";
            this.importFromKMPToolStripMenuItem.Click += new System.EventHandler(this.importFromKMPToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(246, 6);
            // 
            // importFromOBJToolStripMenuItem
            // 
            this.importFromOBJToolStripMenuItem.Name = "importFromOBJToolStripMenuItem";
            this.importFromOBJToolStripMenuItem.Size = new System.Drawing.Size(249, 26);
            this.importFromOBJToolStripMenuItem.Text = "Import Collision OBJ";
            this.importFromOBJToolStripMenuItem.Click += new System.EventHandler(this.importFromOBJToolStripMenuItem_Click);
            // 
            // exportOBJToolStripMenuItem
            // 
            this.exportOBJToolStripMenuItem.Name = "exportOBJToolStripMenuItem";
            this.exportOBJToolStripMenuItem.Size = new System.Drawing.Size(249, 26);
            this.exportOBJToolStripMenuItem.Text = "Export Collision OBJ";
            this.exportOBJToolStripMenuItem.Click += new System.EventHandler(this.exportOBJToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(246, 6);
            // 
            // generateRangeSplinesToolStripMenuItem
            // 
            this.generateRangeSplinesToolStripMenuItem.Name = "generateRangeSplinesToolStripMenuItem";
            this.generateRangeSplinesToolStripMenuItem.Size = new System.Drawing.Size(249, 26);
            this.generateRangeSplinesToolStripMenuItem.Text = "Generate Range Splines";
            this.generateRangeSplinesToolStripMenuItem.Click += new System.EventHandler(this.generateRangeSplinesToolStripMenuItem_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(44, 25);
            this.toolStripButton1.Text = "Save";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(51, 25);
            this.toolStripLabel1.Text = "Mode:";
            // 
            // modeComboBox
            // 
            this.modeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(160, 28);
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
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(70, 25);
            this.toolStripDropDownButton1.Text = "Render";
            // 
            // renderCollisionsToolStripMenuItem
            // 
            this.renderCollisionsToolStripMenuItem.Checked = true;
            this.renderCollisionsToolStripMenuItem.CheckOnClick = true;
            this.renderCollisionsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.renderCollisionsToolStripMenuItem.Name = "renderCollisionsToolStripMenuItem";
            this.renderCollisionsToolStripMenuItem.Size = new System.Drawing.Size(155, 26);
            this.renderCollisionsToolStripMenuItem.Text = "Collisions";
            // 
            // zonesToolStripMenuItem
            // 
            this.zonesToolStripMenuItem.Checked = true;
            this.zonesToolStripMenuItem.CheckOnClick = true;
            this.zonesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.zonesToolStripMenuItem.Name = "zonesToolStripMenuItem";
            this.zonesToolStripMenuItem.Size = new System.Drawing.Size(155, 26);
            this.zonesToolStripMenuItem.Text = "Zones";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 28);
            this.toolStripTextBox1.TextChanged += new System.EventHandler(this.toolStripTextBox1_TextChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 28);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1025, 596);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(1017, 567);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.arrayMemberEditor1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(304, 559);
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
            this.arrayMemberEditor1.Location = new System.Drawing.Point(4, 19);
            this.arrayMemberEditor1.Margin = new System.Windows.Forms.Padding(5);
            this.arrayMemberEditor1.Name = "arrayMemberEditor1";
            this.arrayMemberEditor1.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.arrayMemberEditor1.Size = new System.Drawing.Size(296, 536);
            this.arrayMemberEditor1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(1273, 702);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // toolStripDropDownButton3
            // 
            this.toolStripDropDownButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recalculateCollisionFlagsToolStripMenuItem});
            this.toolStripDropDownButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton3.Image")));
            this.toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            this.toolStripDropDownButton3.Size = new System.Drawing.Size(49, 25);
            this.toolStripDropDownButton3.Text = "Edit";
            // 
            // recalculateCollisionFlagsToolStripMenuItem
            // 
            this.recalculateCollisionFlagsToolStripMenuItem.Name = "recalculateCollisionFlagsToolStripMenuItem";
            this.recalculateCollisionFlagsToolStripMenuItem.Size = new System.Drawing.Size(267, 26);
            this.recalculateCollisionFlagsToolStripMenuItem.Text = "Recalculate Collision Flags";
            this.recalculateCollisionFlagsToolStripMenuItem.Click += new System.EventHandler(this.recalculateCollisionFlagsToolStripMenuItem_Click);
            // 
            // AirRideGrDataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1025, 624);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(4);
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
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem importFromKMPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateRangeSplinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importFromOBJToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exportOBJToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton3;
        private System.Windows.Forms.ToolStripMenuItem recalculateCollisionFlagsToolStripMenuItem;
    }
}