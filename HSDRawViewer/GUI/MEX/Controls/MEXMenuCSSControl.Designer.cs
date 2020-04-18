namespace HSDRawViewer.GUI.MEX.Controls
{
    partial class MEXMenuCSSControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MEXMenuCSSControl));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.editMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.enableSnapAlignmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cspDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cspSelectBox = new System.Windows.Forms.ToolStripComboBox();
            this.cssIconEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editMenu,
            this.cspDropDown,
            this.cspSelectBox});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(300, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // editMenu
            // 
            this.editMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableSnapAlignmentToolStripMenuItem,
            this.replaceIconToolStripMenuItem});
            this.editMenu.Image = ((System.Drawing.Image)(resources.GetObject("editMenu.Image")));
            this.editMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editMenu.Name = "editMenu";
            this.editMenu.Size = new System.Drawing.Size(40, 22);
            this.editMenu.Text = "Edit";
            // 
            // enableSnapAlignmentToolStripMenuItem
            // 
            this.enableSnapAlignmentToolStripMenuItem.Checked = true;
            this.enableSnapAlignmentToolStripMenuItem.CheckOnClick = true;
            this.enableSnapAlignmentToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableSnapAlignmentToolStripMenuItem.Name = "enableSnapAlignmentToolStripMenuItem";
            this.enableSnapAlignmentToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.enableSnapAlignmentToolStripMenuItem.Text = "Enable Snap Alignment";
            // 
            // replaceIconToolStripMenuItem
            // 
            this.replaceIconToolStripMenuItem.Image = global::HSDRawViewer.Properties.Resources.ico_replace;
            this.replaceIconToolStripMenuItem.Name = "replaceIconToolStripMenuItem";
            this.replaceIconToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.replaceIconToolStripMenuItem.Text = "Import Icon";
            this.replaceIconToolStripMenuItem.Click += new System.EventHandler(this.replaceIconToolStripMenuItem_Click);
            // 
            // cspDropDown
            // 
            this.cspDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cspDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replaceToolStripMenuItem,
            this.addToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.cspDropDown.Image = ((System.Drawing.Image)(resources.GetObject("cspDropDown.Image")));
            this.cspDropDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cspDropDown.Name = "cspDropDown";
            this.cspDropDown.Size = new System.Drawing.Size(41, 22);
            this.cspDropDown.Text = "CSP";
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Image = global::HSDRawViewer.Properties.Resources.ico_replace;
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.replaceToolStripMenuItem.Text = "Replace";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Image = global::HSDRawViewer.Properties.Resources.ts_add;
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::HSDRawViewer.Properties.Resources.ts_subtract;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // cspSelectBox
            // 
            this.cspSelectBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cspSelectBox.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.cspSelectBox.Name = "cspSelectBox";
            this.cspSelectBox.Size = new System.Drawing.Size(121, 25);
            this.cspSelectBox.SelectedIndexChanged += new System.EventHandler(this.cspSelectBox_SelectedIndexChanged);
            // 
            // cssIconEditor
            // 
            this.cssIconEditor.CanAdd = false;
            this.cssIconEditor.CanMove = false;
            this.cssIconEditor.DisplayItemIndices = false;
            this.cssIconEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cssIconEditor.EnablePropertyViewDescription = true;
            this.cssIconEditor.ItemIndexOffset = 0;
            this.cssIconEditor.Location = new System.Drawing.Point(0, 25);
            this.cssIconEditor.Name = "cssIconEditor";
            this.cssIconEditor.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.cssIconEditor.Size = new System.Drawing.Size(300, 375);
            this.cssIconEditor.TabIndex = 1;
            this.cssIconEditor.SelectedObjectChanged += new System.EventHandler(this.cssIconEditor_SelectedObjectChanged);
            this.cssIconEditor.ArrayUpdated += new System.EventHandler(this.cssIconEditor_ArrayUpdated);
            // 
            // MEXMenuCSSControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cssIconEditor);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MEXMenuCSSControl";
            this.Size = new System.Drawing.Size(300, 400);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public ArrayMemberEditor cssIconEditor;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton editMenu;
        private System.Windows.Forms.ToolStripComboBox cspSelectBox;
        private System.Windows.Forms.ToolStripMenuItem enableSnapAlignmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceIconToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton cspDropDown;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    }
}
