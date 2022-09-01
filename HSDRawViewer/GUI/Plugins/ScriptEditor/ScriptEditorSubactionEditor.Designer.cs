namespace HSDRawViewer.GUI.Plugins.SubactionEditor
{
    partial class ScriptEditorSubactionEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptEditorSubactionEditor));
            this.subActionList = new System.Windows.Forms.ListBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.buttonAdd = new System.Windows.Forms.ToolStripButton();
            this.buttonRemove = new System.Windows.Forms.ToolStripButton();
            this.buttonUp = new System.Windows.Forms.ToolStripButton();
            this.buttonDown = new System.Windows.Forms.ToolStripButton();
            this.buttonCut = new System.Windows.Forms.ToolStripButton();
            this.buttonCopy = new System.Windows.Forms.ToolStripButton();
            this.buttonPaste = new System.Windows.Forms.ToolStripButton();
            this.buttonReplace = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cbDisplayType = new System.Windows.Forms.ToolStripComboBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.parametersBox = new System.Windows.Forms.GroupBox();
            this.cbActionType = new System.Windows.Forms.ComboBox();
            this.toolStrip2.SuspendLayout();
            this.parametersBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // subActionList
            // 
            this.subActionList.AllowDrop = true;
            this.subActionList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subActionList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.subActionList.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.subActionList.FormattingEnabled = true;
            this.subActionList.HorizontalExtent = 2000;
            this.subActionList.HorizontalScrollbar = true;
            this.subActionList.ItemHeight = 16;
            this.subActionList.Location = new System.Drawing.Point(0, 27);
            this.subActionList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.subActionList.Name = "subActionList";
            this.subActionList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.subActionList.Size = new System.Drawing.Size(300, 470);
            this.subActionList.TabIndex = 2;
            this.subActionList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.subActionList_DrawItem);
            this.subActionList.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.subActionList_MeasureItem);
            this.subActionList.SelectedIndexChanged += new System.EventHandler(this.subActionList_SelectedIndexChanged);
            this.subActionList.DragDrop += new System.Windows.Forms.DragEventHandler(this.subActionList_DragDrop);
            this.subActionList.DragOver += new System.Windows.Forms.DragEventHandler(this.subActionList_DragOver);
            this.subActionList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.subActionList_KeyDown);
            this.subActionList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.subActionList_MouseDown);
            this.subActionList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.subActionList_MouseUp);
            // 
            // toolStrip2
            // 
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonAdd,
            this.buttonRemove,
            this.buttonUp,
            this.buttonDown,
            this.buttonCut,
            this.buttonCopy,
            this.buttonPaste,
            this.buttonReplace,
            this.toolStripLabel1,
            this.cbDisplayType});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(559, 27);
            this.toolStrip2.TabIndex = 3;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // buttonAdd
            // 
            this.buttonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAdd.Image = global::HSDRawViewer.Properties.Resources.ts_add;
            this.buttonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(24, 24);
            this.buttonAdd.Text = "Add";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRemove.Image = global::HSDRawViewer.Properties.Resources.ts_subtract;
            this.buttonRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(24, 24);
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonUp.Image = global::HSDRawViewer.Properties.Resources.ts_up;
            this.buttonUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(24, 24);
            this.buttonUp.Text = "Move Up";
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDown.Image = global::HSDRawViewer.Properties.Resources.ts_down;
            this.buttonDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(24, 24);
            this.buttonDown.Text = "Move Down";
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonCut
            // 
            this.buttonCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonCut.Image = ((System.Drawing.Image)(resources.GetObject("buttonCut.Image")));
            this.buttonCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonCut.Name = "buttonCut";
            this.buttonCut.Size = new System.Drawing.Size(30, 24);
            this.buttonCut.Text = "Cut";
            this.buttonCut.Click += new System.EventHandler(this.buttonCut_Click);
            // 
            // buttonCopy
            // 
            this.buttonCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonCopy.Image = ((System.Drawing.Image)(resources.GetObject("buttonCopy.Image")));
            this.buttonCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(39, 24);
            this.buttonCopy.Text = "Copy";
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // buttonPaste
            // 
            this.buttonPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonPaste.Image = ((System.Drawing.Image)(resources.GetObject("buttonPaste.Image")));
            this.buttonPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPaste.Name = "buttonPaste";
            this.buttonPaste.Size = new System.Drawing.Size(39, 24);
            this.buttonPaste.Text = "Paste";
            this.buttonPaste.Click += new System.EventHandler(this.buttonPaste_Click);
            // 
            // buttonReplace
            // 
            this.buttonReplace.Image = global::HSDRawViewer.Properties.Resources.ico_replace;
            this.buttonReplace.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonReplace.Name = "buttonReplace";
            this.buttonReplace.Size = new System.Drawing.Size(72, 24);
            this.buttonReplace.Text = "Replace";
            this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(35, 24);
            this.toolStripLabel1.Text = "View:";
            // 
            // cbDisplayType
            // 
            this.cbDisplayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDisplayType.Name = "cbDisplayType";
            this.cbDisplayType.Size = new System.Drawing.Size(107, 27);
            this.cbDisplayType.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.HelpVisible = false;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 42);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid1.Size = new System.Drawing.Size(253, 425);
            this.propertyGrid1.TabIndex = 4;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(297, 27);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 470);
            this.splitter1.TabIndex = 5;
            this.splitter1.TabStop = false;
            // 
            // parametersBox
            // 
            this.parametersBox.Controls.Add(this.propertyGrid1);
            this.parametersBox.Controls.Add(this.cbActionType);
            this.parametersBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.parametersBox.Location = new System.Drawing.Point(300, 27);
            this.parametersBox.Name = "parametersBox";
            this.parametersBox.Size = new System.Drawing.Size(259, 470);
            this.parametersBox.TabIndex = 6;
            this.parametersBox.TabStop = false;
            this.parametersBox.Text = "Parameters";
            // 
            // cbActionType
            // 
            this.cbActionType.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbActionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbActionType.FormattingEnabled = true;
            this.cbActionType.Location = new System.Drawing.Point(3, 19);
            this.cbActionType.Name = "cbActionType";
            this.cbActionType.Size = new System.Drawing.Size(253, 23);
            this.cbActionType.TabIndex = 5;
            this.cbActionType.SelectedIndexChanged += new System.EventHandler(this.cbActionType_SelectedIndexChanged);
            // 
            // ScriptEditorSubactionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 497);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.subActionList);
            this.Controls.Add(this.parametersBox);
            this.Controls.Add(this.toolStrip2);
            this.Name = "ScriptEditorSubactionEditor";
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.parametersBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox subActionList;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton buttonAdd;
        private System.Windows.Forms.ToolStripButton buttonRemove;
        private System.Windows.Forms.ToolStripButton buttonUp;
        private System.Windows.Forms.ToolStripButton buttonDown;
        private System.Windows.Forms.ToolStripButton buttonCut;
        private System.Windows.Forms.ToolStripButton buttonCopy;
        private System.Windows.Forms.ToolStripButton buttonPaste;
        private System.Windows.Forms.ToolStripButton buttonReplace;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cbDisplayType;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.GroupBox parametersBox;
        private System.Windows.Forms.ComboBox cbActionType;
    }
}
