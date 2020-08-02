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
            this.createLineFromSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editLineMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.addToSelectedGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbSelectType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Groups = new System.Windows.Forms.TabPage();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.optionsDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.calculateRangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipDirectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guessCollisionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addGroupButton = new System.Windows.Forms.ToolStripButton();
            this.showAllCheckBox = new System.Windows.Forms.ToolStripButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listLines = new System.Windows.Forms.ListBox();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.cbRenderModes = new System.Windows.Forms.ToolStripComboBox();
            this.deleteGroupButton = new System.Windows.Forms.ToolStripButton();
            this.toolBox.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.Groups.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolBox
            // 
            this.toolBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolBox.Controls.Add(this.toolStrip1);
            this.toolBox.Controls.Add(this.propertyGrid1);
            this.toolBox.Location = new System.Drawing.Point(0, 199);
            this.toolBox.Name = "toolBox";
            this.toolBox.Size = new System.Drawing.Size(276, 287);
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
            this.newLineToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.newLineToolStripMenuItem.Text = "Line/Point";
            this.newLineToolStripMenuItem.Click += new System.EventHandler(this.newLineToolStripMenuItem_Click);
            // 
            // editVertexMenu
            // 
            this.editVertexMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.editVertexMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fuseSelectedToolStripMenuItem,
            this.splitSelectedToolStripMenuItem,
            this.removeSelectedToolStripMenuItem,
            this.createLineFromSelectedToolStripMenuItem});
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
            this.fuseSelectedToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.fuseSelectedToolStripMenuItem.Text = "Fuse Selected";
            this.fuseSelectedToolStripMenuItem.Click += new System.EventHandler(this.fuseSelectedToolStripMenuItem_Click);
            // 
            // splitSelectedToolStripMenuItem
            // 
            this.splitSelectedToolStripMenuItem.Name = "splitSelectedToolStripMenuItem";
            this.splitSelectedToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.splitSelectedToolStripMenuItem.Text = "Split Selected";
            this.splitSelectedToolStripMenuItem.Click += new System.EventHandler(this.splitSelectedToolStripMenuItem_Click);
            // 
            // removeSelectedToolStripMenuItem
            // 
            this.removeSelectedToolStripMenuItem.Name = "removeSelectedToolStripMenuItem";
            this.removeSelectedToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.removeSelectedToolStripMenuItem.Text = "Remove Selected";
            this.removeSelectedToolStripMenuItem.Click += new System.EventHandler(this.removeSelectedToolStripMenuItem_Click);
            // 
            // createLineFromSelectedToolStripMenuItem
            // 
            this.createLineFromSelectedToolStripMenuItem.Name = "createLineFromSelectedToolStripMenuItem";
            this.createLineFromSelectedToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.createLineFromSelectedToolStripMenuItem.Text = "Create Line from Selected";
            this.createLineFromSelectedToolStripMenuItem.Click += new System.EventHandler(this.createLineFromSelectedToolStripMenuItem_Click);
            // 
            // editLineMenu
            // 
            this.editLineMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.editLineMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToSelectedGroupToolStripMenuItem,
            this.splitLineToolStripMenuItem});
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
            // splitLineToolStripMenuItem
            // 
            this.splitLineToolStripMenuItem.Name = "splitLineToolStripMenuItem";
            this.splitLineToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.splitLineToolStripMenuItem.Text = "Split Line";
            this.splitLineToolStripMenuItem.Click += new System.EventHandler(this.splitLineToolStripMenuItem_Click);
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
            this.cbSelectType.Location = new System.Drawing.Point(79, 172);
            this.cbSelectType.Name = "cbSelectType";
            this.cbSelectType.Size = new System.Drawing.Size(187, 21);
            this.cbSelectType.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 175);
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
            this.propertyGrid1.Location = new System.Drawing.Point(6, 44);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid1.Size = new System.Drawing.Size(264, 243);
            this.propertyGrid1.TabIndex = 4;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 33);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(259, 82);
            this.listBox1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.Groups);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 28);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(267, 141);
            this.tabControl1.TabIndex = 9;
            // 
            // Groups
            // 
            this.Groups.Controls.Add(this.toolStrip2);
            this.Groups.Controls.Add(this.listBox1);
            this.Groups.Location = new System.Drawing.Point(4, 22);
            this.Groups.Name = "Groups";
            this.Groups.Padding = new System.Windows.Forms.Padding(3);
            this.Groups.Size = new System.Drawing.Size(259, 115);
            this.Groups.TabIndex = 0;
            this.Groups.Text = "Groups";
            this.Groups.UseVisualStyleBackColor = true;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsDropDown,
            this.addGroupButton,
            this.deleteGroupButton,
            this.showAllCheckBox});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(253, 25);
            this.toolStrip2.TabIndex = 10;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // optionsDropDown
            // 
            this.optionsDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.optionsDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.calculateRangeToolStripMenuItem,
            this.flipDirectionToolStripMenuItem,
            this.guessCollisionsToolStripMenuItem});
            this.optionsDropDown.Image = ((System.Drawing.Image)(resources.GetObject("optionsDropDown.Image")));
            this.optionsDropDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.optionsDropDown.Name = "optionsDropDown";
            this.optionsDropDown.Size = new System.Drawing.Size(62, 22);
            this.optionsDropDown.Text = "Options";
            // 
            // calculateRangeToolStripMenuItem
            // 
            this.calculateRangeToolStripMenuItem.Name = "calculateRangeToolStripMenuItem";
            this.calculateRangeToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.calculateRangeToolStripMenuItem.Text = "Calculate Range";
            this.calculateRangeToolStripMenuItem.Click += new System.EventHandler(this.buttonCalculateRange_Click);
            // 
            // flipDirectionToolStripMenuItem
            // 
            this.flipDirectionToolStripMenuItem.Name = "flipDirectionToolStripMenuItem";
            this.flipDirectionToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.flipDirectionToolStripMenuItem.Text = "Flip Direction";
            this.flipDirectionToolStripMenuItem.Click += new System.EventHandler(this.flipDirectionToolStripMenuItem_Click);
            // 
            // guessCollisionsToolStripMenuItem
            // 
            this.guessCollisionsToolStripMenuItem.Name = "guessCollisionsToolStripMenuItem";
            this.guessCollisionsToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.guessCollisionsToolStripMenuItem.Text = "Guess Collisions";
            this.guessCollisionsToolStripMenuItem.Click += new System.EventHandler(this.guessCollisionsToolStripMenuItem_Click);
            // 
            // addGroupButton
            // 
            this.addGroupButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addGroupButton.Image = global::HSDRawViewer.Properties.Resources.ts_add;
            this.addGroupButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addGroupButton.Name = "addGroupButton";
            this.addGroupButton.Size = new System.Drawing.Size(23, 22);
            this.addGroupButton.Text = "Add Group";
            this.addGroupButton.Click += new System.EventHandler(this.buttonAddGroup_Click);
            // 
            // showAllCheckBox
            // 
            this.showAllCheckBox.Checked = true;
            this.showAllCheckBox.CheckOnClick = true;
            this.showAllCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showAllCheckBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.showAllCheckBox.Image = ((System.Drawing.Image)(resources.GetObject("showAllCheckBox.Image")));
            this.showAllCheckBox.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showAllCheckBox.Name = "showAllCheckBox";
            this.showAllCheckBox.Size = new System.Drawing.Size(57, 22);
            this.showAllCheckBox.Text = "Show All";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listLines);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(259, 115);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Lines";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listLines
            // 
            this.listLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listLines.FormattingEnabled = true;
            this.listLines.Location = new System.Drawing.Point(3, 3);
            this.listLines.Name = "listLines";
            this.listLines.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listLines.Size = new System.Drawing.Size(253, 109);
            this.listLines.TabIndex = 0;
            // 
            // toolStrip3
            // 
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveButton,
            this.cbRenderModes});
            this.toolStrip3.Location = new System.Drawing.Point(0, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(276, 25);
            this.toolStrip3.TabIndex = 12;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // saveButton
            // 
            this.saveButton.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(100, 22);
            this.saveButton.Text = "Save Changes";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cbRenderModes
            // 
            this.cbRenderModes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRenderModes.Items.AddRange(new object[] {
            "Material Type",
            "Collision Type",
            "Dropdown"});
            this.cbRenderModes.Name = "cbRenderModes";
            this.cbRenderModes.Size = new System.Drawing.Size(121, 25);
            // 
            // deleteGroupButton
            // 
            this.deleteGroupButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteGroupButton.Image = global::HSDRawViewer.Properties.Resources.ts_subtract;
            this.deleteGroupButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteGroupButton.Name = "deleteGroupButton";
            this.deleteGroupButton.Size = new System.Drawing.Size(23, 22);
            this.deleteGroupButton.Text = "Delete Group";
            this.deleteGroupButton.Click += new System.EventHandler(this.buttonDeleteGroup_Click);
            // 
            // CollDataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 498);
            this.Controls.Add(this.toolStrip3);
            this.Controls.Add(this.cbSelectType);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.toolBox);
            this.Name = "CollDataEditor";
            this.toolBox.ResumeLayout(false);
            this.toolBox.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.Groups.ResumeLayout(false);
            this.Groups.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox toolBox;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbSelectType;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
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
        private System.Windows.Forms.ToolStripMenuItem splitLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createLineFromSelectedToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Groups;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListBox listLines;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripDropDownButton optionsDropDown;
        private System.Windows.Forms.ToolStripButton addGroupButton;
        private System.Windows.Forms.ToolStripButton showAllCheckBox;
        private System.Windows.Forms.ToolStripMenuItem calculateRangeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flipDirectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem guessCollisionsToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton saveButton;
        private System.Windows.Forms.ToolStripComboBox cbRenderModes;
        private System.Windows.Forms.ToolStripButton deleteGroupButton;
    }
}
