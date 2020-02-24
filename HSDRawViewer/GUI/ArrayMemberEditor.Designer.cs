namespace HSDRawViewer.GUI
{
    partial class ArrayMemberEditor
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
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.elementList = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonAdd = new System.Windows.Forms.ToolStripButton();
            this.buttonRemove = new System.Windows.Forms.ToolStripButton();
            this.buttonUp = new System.Windows.Forms.ToolStripButton();
            this.buttonDown = new System.Windows.Forms.ToolStripButton();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 245);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid.Size = new System.Drawing.Size(208, 87);
            this.propertyGrid.TabIndex = 7;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // elementList
            // 
            this.elementList.Dock = System.Windows.Forms.DockStyle.Top;
            this.elementList.FormattingEnabled = true;
            this.elementList.Location = new System.Drawing.Point(0, 25);
            this.elementList.Name = "elementList";
            this.elementList.Size = new System.Drawing.Size(208, 212);
            this.elementList.TabIndex = 4;
            this.elementList.SelectedValueChanged += new System.EventHandler(this.elementList_SelectedValueChanged);
            this.elementList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.elementList_MouseDoubleClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonAdd,
            this.buttonRemove,
            this.buttonUp,
            this.buttonDown});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(208, 25);
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
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
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
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
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 237);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(208, 8);
            this.splitter1.TabIndex = 9;
            this.splitter1.TabStop = false;
            // 
            // ArrayMemberEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.elementList);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ArrayMemberEditor";
            this.Size = new System.Drawing.Size(208, 332);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.ListBox elementList;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonAdd;
        private System.Windows.Forms.ToolStripButton buttonRemove;
        private System.Windows.Forms.ToolStripButton buttonUp;
        private System.Windows.Forms.ToolStripButton buttonDown;
        private System.Windows.Forms.Splitter splitter1;
    }
}
