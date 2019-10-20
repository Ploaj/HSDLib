namespace HSDRawViewer.GUI.Plugins
{
    partial class CollDataEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollDataEditor));
            this.toolBox = new System.Windows.Forms.GroupBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.newLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editVertexMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.fuseSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbSelectType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.buttonAddGroup = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonCalculateRange = new System.Windows.Forms.Button();
            this.cbShowAllGroups = new System.Windows.Forms.CheckBox();
            this.cbRenderModes = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.editLineMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.addToSelectedGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolBox.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolBox
            // 
            this.toolBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolBox.Controls.Add(this.toolStrip1);
            this.toolBox.Controls.Add(this.cbSelectType);
            this.toolBox.Controls.Add(this.label1);
            this.toolBox.Controls.Add(this.propertyGrid1);
            this.toolBox.Location = new System.Drawing.Point(0, 154);
            this.toolBox.Name = "toolBox";
            this.toolBox.Size = new System.Drawing.Size(276, 209);
            this.toolBox.TabIndex = 1;
            this.toolBox.TabStop = false;
            this.toolBox.Text = "Selected";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteButton,
            this.toolStripDropDownButton1,
            this.editVertexMenu,
            this.editLineMenu});
            this.toolStrip1.Location = new System.Drawing.Point(3, 16);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(270, 25);
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // deleteButton
            // 
            this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.deleteButton.Image = ((System.Drawing.Image)(resources.GetObject("deleteButton.Image")));
            this.deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(44, 22);
            this.deleteButton.Text = "Delete";
            this.deleteButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newLineToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(42, 22);
            this.toolStripDropDownButton1.Text = "Add";
            // 
            // newLineToolStripMenuItem
            // 
            this.newLineToolStripMenuItem.Name = "newLineToolStripMenuItem";
            this.newLineToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newLineToolStripMenuItem.Text = "Line/Point";
            this.newLineToolStripMenuItem.Click += new System.EventHandler(this.newLineToolStripMenuItem_Click);
            // 
            // editVertexMenu
            // 
            this.editVertexMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.editVertexMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fuseSelectedToolStripMenuItem,
            this.splitSelectedToolStripMenuItem,
            this.removeSelectedToolStripMenuItem});
            this.editVertexMenu.Enabled = false;
            this.editVertexMenu.Image = ((System.Drawing.Image)(resources.GetObject("editVertexMenu.Image")));
            this.editVertexMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editVertexMenu.Name = "editVertexMenu";
            this.editVertexMenu.Size = new System.Drawing.Size(75, 22);
            this.editVertexMenu.Text = "Edit Vertex";
            // 
            // fuseSelectedToolStripMenuItem
            // 
            this.fuseSelectedToolStripMenuItem.Name = "fuseSelectedToolStripMenuItem";
            this.fuseSelectedToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.fuseSelectedToolStripMenuItem.Text = "Fuse Selected";
            this.fuseSelectedToolStripMenuItem.Click += new System.EventHandler(this.fuseSelectedToolStripMenuItem_Click);
            // 
            // splitSelectedToolStripMenuItem
            // 
            this.splitSelectedToolStripMenuItem.Name = "splitSelectedToolStripMenuItem";
            this.splitSelectedToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.splitSelectedToolStripMenuItem.Text = "Split Selected";
            this.splitSelectedToolStripMenuItem.Click += new System.EventHandler(this.splitSelectedToolStripMenuItem_Click);
            // 
            // removeSelectedToolStripMenuItem
            // 
            this.removeSelectedToolStripMenuItem.Name = "removeSelectedToolStripMenuItem";
            this.removeSelectedToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.removeSelectedToolStripMenuItem.Text = "Remove Selected";
            this.removeSelectedToolStripMenuItem.Click += new System.EventHandler(this.removeSelectedToolStripMenuItem_Click);
            // 
            // cbSelectType
            // 
            this.cbSelectType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSelectType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSelectType.FormattingEnabled = true;
            this.cbSelectType.Items.AddRange(new object[] {
            "Point",
            "Line"});
            this.cbSelectType.Location = new System.Drawing.Point(79, 48);
            this.cbSelectType.Name = "cbSelectType";
            this.cbSelectType.Size = new System.Drawing.Size(194, 21);
            this.cbSelectType.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Select Mode:";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(6, 75);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGrid1.Size = new System.Drawing.Size(264, 134);
            this.propertyGrid1.TabIndex = 4;
            // 
            // buttonAddGroup
            // 
            this.buttonAddGroup.Location = new System.Drawing.Point(6, 19);
            this.buttonAddGroup.Name = "buttonAddGroup";
            this.buttonAddGroup.Size = new System.Drawing.Size(37, 23);
            this.buttonAddGroup.TabIndex = 5;
            this.buttonAddGroup.Text = "+";
            this.buttonAddGroup.UseVisualStyleBackColor = true;
            this.buttonAddGroup.Click += new System.EventHandler(this.buttonAddGroup_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(6, 4);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(6, 48);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(264, 56);
            this.listBox1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.buttonCalculateRange);
            this.groupBox1.Controls.Add(this.cbShowAllGroups);
            this.groupBox1.Controls.Add(this.buttonAddGroup);
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.Location = new System.Drawing.Point(0, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 115);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Groups";
            // 
            // buttonCalculateRange
            // 
            this.buttonCalculateRange.Location = new System.Drawing.Point(49, 19);
            this.buttonCalculateRange.Name = "buttonCalculateRange";
            this.buttonCalculateRange.Size = new System.Drawing.Size(95, 23);
            this.buttonCalculateRange.TabIndex = 6;
            this.buttonCalculateRange.Text = "Calculate Range";
            this.buttonCalculateRange.UseVisualStyleBackColor = true;
            this.buttonCalculateRange.Click += new System.EventHandler(this.buttonCalculateRange_Click);
            // 
            // cbShowAllGroups
            // 
            this.cbShowAllGroups.AutoSize = true;
            this.cbShowAllGroups.Checked = true;
            this.cbShowAllGroups.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowAllGroups.Location = new System.Drawing.Point(150, 23);
            this.cbShowAllGroups.Name = "cbShowAllGroups";
            this.cbShowAllGroups.Size = new System.Drawing.Size(67, 17);
            this.cbShowAllGroups.TabIndex = 8;
            this.cbShowAllGroups.Text = "Show All";
            this.cbShowAllGroups.UseVisualStyleBackColor = true;
            // 
            // cbRenderModes
            // 
            this.cbRenderModes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbRenderModes.FormattingEnabled = true;
            this.cbRenderModes.Items.AddRange(new object[] {
            "Material Type",
            "Collision Type",
            "Dropdown"});
            this.cbRenderModes.Location = new System.Drawing.Point(141, 6);
            this.cbRenderModes.Name = "cbRenderModes";
            this.cbRenderModes.Size = new System.Drawing.Size(126, 21);
            this.cbRenderModes.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(87, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Render: ";
            // 
            // editLineMenu
            // 
            this.editLineMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.editLineMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToSelectedGroupToolStripMenuItem});
            this.editLineMenu.Enabled = false;
            this.editLineMenu.Image = ((System.Drawing.Image)(resources.GetObject("editLineMenu.Image")));
            this.editLineMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editLineMenu.Name = "editLineMenu";
            this.editLineMenu.Size = new System.Drawing.Size(65, 22);
            this.editLineMenu.Text = "Edit Line";
            // 
            // addToSelectedGroupToolStripMenuItem
            // 
            this.addToSelectedGroupToolStripMenuItem.Name = "addToSelectedGroupToolStripMenuItem";
            this.addToSelectedGroupToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.addToSelectedGroupToolStripMenuItem.Text = "Add to Selected Group";
            this.addToSelectedGroupToolStripMenuItem.Click += new System.EventHandler(this.addToSelectedGroupToolStripMenuItem_Click);
            // 
            // CollDataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 375);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbRenderModes);
            this.Controls.Add(this.toolBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.saveButton);
            this.Name = "CollDataEditor";
            this.toolBox.ResumeLayout(false);
            this.toolBox.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox toolBox;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbSelectType;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Button buttonAddGroup;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonCalculateRange;
        private System.Windows.Forms.CheckBox cbShowAllGroups;
        private System.Windows.Forms.ComboBox cbRenderModes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem newLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton editVertexMenu;
        private System.Windows.Forms.ToolStripMenuItem fuseSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem splitSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton editLineMenu;
        private System.Windows.Forms.ToolStripMenuItem addToSelectedGroupToolStripMenuItem;
    }
}
