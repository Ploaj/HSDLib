namespace HSDRawViewer.GUI
{
    partial class SubactionEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubactionEditor));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.subActionList = new System.Windows.Forms.ListBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.buttonAdd = new System.Windows.Forms.ToolStripButton();
            this.buttonRemove = new System.Windows.Forms.ToolStripButton();
            this.buttonUp = new System.Windows.Forms.ToolStripButton();
            this.buttonDown = new System.Windows.Forms.ToolStripButton();
            this.buttonEdit = new System.Windows.Forms.ToolStripButton();
            this.buttonCut = new System.Windows.Forms.ToolStripButton();
            this.buttonCopy = new System.Windows.Forms.ToolStripButton();
            this.buttonPaste = new System.Windows.Forms.ToolStripButton();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonGoto = new System.Windows.Forms.Button();
            this.referenceLabel = new System.Windows.Forms.Label();
            this.cbReference = new System.Windows.Forms.ComboBox();
            this.previewBox = new System.Windows.Forms.GroupBox();
            this.actionList = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.editDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.clearAllActionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadPlayerFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.renderHitboxInterpolationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(197, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 439);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.splitter2);
            this.groupBox1.Controls.Add(this.subActionList);
            this.groupBox1.Controls.Add(this.toolStrip2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.previewBox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(200, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(495, 439);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Subaction";
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(3, 113);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(489, 3);
            this.splitter2.TabIndex = 8;
            this.splitter2.TabStop = false;
            // 
            // subActionList
            // 
            this.subActionList.AllowDrop = true;
            this.subActionList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subActionList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.subActionList.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.subActionList.FormattingEnabled = true;
            this.subActionList.HorizontalExtent = 1000;
            this.subActionList.HorizontalScrollbar = true;
            this.subActionList.ItemHeight = 16;
            this.subActionList.Location = new System.Drawing.Point(3, 70);
            this.subActionList.Name = "subActionList";
            this.subActionList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.subActionList.Size = new System.Drawing.Size(489, 46);
            this.subActionList.TabIndex = 0;
            this.subActionList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.subActionList_DrawItem);
            this.subActionList.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.subActionList_MeasureItem);
            this.subActionList.SelectedIndexChanged += new System.EventHandler(this.subActionList_SelectedIndexChanged);
            this.subActionList.DoubleClick += new System.EventHandler(this.subActionList_DoubleClick);
            this.subActionList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.subActionList_KeyDown);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonAdd,
            this.buttonRemove,
            this.buttonUp,
            this.buttonDown,
            this.buttonEdit,
            this.buttonCut,
            this.buttonCopy,
            this.buttonPaste});
            this.toolStrip2.Location = new System.Drawing.Point(3, 45);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(489, 25);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // buttonAdd
            // 
            this.buttonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAdd.Image = global::HSDRawViewer.Properties.Resources.ts_add;
            this.buttonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(23, 22);
            this.buttonAdd.Text = "Add";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRemove.Image = global::HSDRawViewer.Properties.Resources.ts_subtract;
            this.buttonRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(23, 22);
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonUp.Image = global::HSDRawViewer.Properties.Resources.ts_up;
            this.buttonUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(23, 22);
            this.buttonUp.Text = "Move Up";
            this.buttonUp.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDown.Image = global::HSDRawViewer.Properties.Resources.ts_down;
            this.buttonDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(23, 22);
            this.buttonDown.Text = "Move Down";
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonEdit.Image = ((System.Drawing.Image)(resources.GetObject("buttonEdit.Image")));
            this.buttonEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(31, 22);
            this.buttonEdit.Text = "Edit";
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonCut
            // 
            this.buttonCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonCut.Image = ((System.Drawing.Image)(resources.GetObject("buttonCut.Image")));
            this.buttonCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonCut.Name = "buttonCut";
            this.buttonCut.Size = new System.Drawing.Size(30, 22);
            this.buttonCut.Text = "Cut";
            this.buttonCut.Click += new System.EventHandler(this.buttonCut_Click);
            // 
            // buttonCopy
            // 
            this.buttonCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonCopy.Image = ((System.Drawing.Image)(resources.GetObject("buttonCopy.Image")));
            this.buttonCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(39, 22);
            this.buttonCopy.Text = "Copy";
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // buttonPaste
            // 
            this.buttonPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonPaste.Image = ((System.Drawing.Image)(resources.GetObject("buttonPaste.Image")));
            this.buttonPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPaste.Name = "buttonPaste";
            this.buttonPaste.Size = new System.Drawing.Size(39, 22);
            this.buttonPaste.Text = "Paste";
            this.buttonPaste.Click += new System.EventHandler(this.buttonPaste_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.buttonGoto);
            this.panel1.Controls.Add(this.referenceLabel);
            this.panel1.Controls.Add(this.cbReference);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(489, 29);
            this.panel1.TabIndex = 6;
            // 
            // buttonGoto
            // 
            this.buttonGoto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGoto.Location = new System.Drawing.Point(411, 3);
            this.buttonGoto.Name = "buttonGoto";
            this.buttonGoto.Size = new System.Drawing.Size(75, 23);
            this.buttonGoto.TabIndex = 5;
            this.buttonGoto.Text = "Goto";
            this.buttonGoto.UseVisualStyleBackColor = true;
            this.buttonGoto.Click += new System.EventHandler(this.buttonGoto_Click);
            // 
            // referenceLabel
            // 
            this.referenceLabel.AutoSize = true;
            this.referenceLabel.Location = new System.Drawing.Point(3, 8);
            this.referenceLabel.Name = "referenceLabel";
            this.referenceLabel.Size = new System.Drawing.Size(81, 13);
            this.referenceLabel.TabIndex = 3;
            this.referenceLabel.Text = "Referenced By:";
            // 
            // cbReference
            // 
            this.cbReference.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbReference.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbReference.FormattingEnabled = true;
            this.cbReference.Location = new System.Drawing.Point(90, 4);
            this.cbReference.Name = "cbReference";
            this.cbReference.Size = new System.Drawing.Size(315, 21);
            this.cbReference.TabIndex = 4;
            // 
            // previewBox
            // 
            this.previewBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.previewBox.Location = new System.Drawing.Point(3, 116);
            this.previewBox.Name = "previewBox";
            this.previewBox.Size = new System.Drawing.Size(489, 320);
            this.previewBox.TabIndex = 7;
            this.previewBox.TabStop = false;
            this.previewBox.Text = "Preview";
            this.previewBox.Visible = false;
            // 
            // actionList
            // 
            this.actionList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionList.FormattingEnabled = true;
            this.actionList.Location = new System.Drawing.Point(3, 41);
            this.actionList.Name = "actionList";
            this.actionList.Size = new System.Drawing.Size(191, 395);
            this.actionList.TabIndex = 5;
            this.actionList.SelectedIndexChanged += new System.EventHandler(this.actionList_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.actionList);
            this.groupBox2.Controls.Add(this.toolStrip1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(197, 439);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Action List";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editDropDown,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(3, 16);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(191, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // editDropDown
            // 
            this.editDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.editDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPlayerFilesToolStripMenuItem,
            this.renderHitboxInterpolationToolStripMenuItem,
            this.clearAllActionsToolStripMenuItem});
            this.editDropDown.Image = ((System.Drawing.Image)(resources.GetObject("editDropDown.Image")));
            this.editDropDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editDropDown.Name = "editDropDown";
            this.editDropDown.Size = new System.Drawing.Size(29, 22);
            this.editDropDown.Text = "Edit";
            // 
            // clearAllActionsToolStripMenuItem
            // 
            this.clearAllActionsToolStripMenuItem.Name = "clearAllActionsToolStripMenuItem";
            this.clearAllActionsToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.clearAllActionsToolStripMenuItem.Text = "Clear All Action\'s Scripts";
            this.clearAllActionsToolStripMenuItem.Click += new System.EventHandler(this.clearAllActionsToolStripMenuItem_Click);
            // 
            // loadPlayerFilesToolStripMenuItem
            // 
            this.loadPlayerFilesToolStripMenuItem.Name = "loadPlayerFilesToolStripMenuItem";
            this.loadPlayerFilesToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.loadPlayerFilesToolStripMenuItem.Text = "Load Player Files";
            this.loadPlayerFilesToolStripMenuItem.Click += new System.EventHandler(this.loadPlayerFilesToolStripMenuItem_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(133, 22);
            this.toolStripButton1.Text = "Create New Subroutine";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // renderHitboxInterpolationToolStripMenuItem
            // 
            this.renderHitboxInterpolationToolStripMenuItem.CheckOnClick = true;
            this.renderHitboxInterpolationToolStripMenuItem.Name = "renderHitboxInterpolationToolStripMenuItem";
            this.renderHitboxInterpolationToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.renderHitboxInterpolationToolStripMenuItem.Text = "Render Hitbox Interpolation";
            // 
            // SubactionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 439);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.groupBox2);
            this.Name = "SubactionEditor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox subActionList;
        private System.Windows.Forms.ListBox actionList;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton buttonAdd;
        private System.Windows.Forms.ToolStripButton buttonRemove;
        private System.Windows.Forms.ToolStripButton buttonEdit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label referenceLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonGoto;
        private System.Windows.Forms.ComboBox cbReference;
        private System.Windows.Forms.ToolStripButton buttonUp;
        private System.Windows.Forms.ToolStripButton buttonDown;
        private System.Windows.Forms.ToolStripButton buttonCopy;
        private System.Windows.Forms.ToolStripButton buttonPaste;
        private System.Windows.Forms.ToolStripButton buttonCut;
        private System.Windows.Forms.ToolStripDropDownButton editDropDown;
        private System.Windows.Forms.ToolStripMenuItem clearAllActionsToolStripMenuItem;
        private System.Windows.Forms.GroupBox previewBox;
        private System.Windows.Forms.ToolStripMenuItem loadPlayerFilesToolStripMenuItem;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.ToolStripMenuItem renderHitboxInterpolationToolStripMenuItem;
    }
}
