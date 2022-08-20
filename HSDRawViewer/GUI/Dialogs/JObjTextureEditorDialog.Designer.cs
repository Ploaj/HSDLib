namespace HSDRawViewer.GUI.Extra
{
    partial class JObjTextureEditorDialog
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.textureArrayEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.exporttoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.replaceTextureButton = new System.Windows.Forms.ToolStripButton();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.groupBox1.SuspendLayout();
            this.toolStrip4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.propertyGrid1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox1.Location = new System.Drawing.Point(471, 27);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(329, 423);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameters";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.HelpVisible = false;
            this.propertyGrid1.Location = new System.Drawing.Point(4, 19);
            this.propertyGrid1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid1.Size = new System.Drawing.Size(321, 401);
            this.propertyGrid1.TabIndex = 1;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // textureArrayEditor
            // 
            this.textureArrayEditor.AllowDrop = true;
            this.textureArrayEditor.DisplayItemImages = true;
            this.textureArrayEditor.DisplayItemIndices = true;
            this.textureArrayEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textureArrayEditor.EnablePropertyView = false;
            this.textureArrayEditor.EnablePropertyViewDescription = false;
            this.textureArrayEditor.EnableToolStrip = false;
            this.textureArrayEditor.ImageHeight = ((ushort)(64));
            this.textureArrayEditor.ImageWidth = ((ushort)(64));
            this.textureArrayEditor.InsertCloneAfterSelected = false;
            this.textureArrayEditor.ItemHeight = 64;
            this.textureArrayEditor.ItemIndexOffset = 0;
            this.textureArrayEditor.Location = new System.Drawing.Point(0, 27);
            this.textureArrayEditor.Margin = new System.Windows.Forms.Padding(5);
            this.textureArrayEditor.Name = "textureArrayEditor";
            this.textureArrayEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.textureArrayEditor.Size = new System.Drawing.Size(471, 423);
            this.textureArrayEditor.TabIndex = 4;
            this.textureArrayEditor.SelectedObjectChanged += new System.EventHandler(this.textureArrayEditor_SelectedObjectChanged);
            this.textureArrayEditor.DragDrop += new System.Windows.Forms.DragEventHandler(this.textureArrayEditor_DragDrop);
            this.textureArrayEditor.DragEnter += new System.Windows.Forms.DragEventHandler(this.textureArrayEditor_DragEnter);
            // 
            // toolStrip4
            // 
            this.toolStrip4.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exporttoolStripButton,
            this.replaceTextureButton});
            this.toolStrip4.Location = new System.Drawing.Point(0, 0);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.Size = new System.Drawing.Size(800, 27);
            this.toolStrip4.TabIndex = 5;
            this.toolStrip4.Text = "toolStrip4";
            // 
            // exporttoolStripButton
            // 
            this.exporttoolStripButton.Image = global::HSDRawViewer.Properties.Resources.ts_exportfile;
            this.exporttoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exporttoolStripButton.Name = "exporttoolStripButton";
            this.exporttoolStripButton.Size = new System.Drawing.Size(106, 24);
            this.exporttoolStripButton.Text = "Export Texture";
            this.exporttoolStripButton.Click += new System.EventHandler(this.exporttoolStripButton_Click);
            // 
            // replaceTextureButton
            // 
            this.replaceTextureButton.Image = global::HSDRawViewer.Properties.Resources.ico_replace;
            this.replaceTextureButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.replaceTextureButton.Name = "replaceTextureButton";
            this.replaceTextureButton.Size = new System.Drawing.Size(113, 24);
            this.replaceTextureButton.Text = "Replace Texture";
            this.replaceTextureButton.Click += new System.EventHandler(this.replaceTextureButton_Click);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(468, 27);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 423);
            this.splitter1.TabIndex = 7;
            this.splitter1.TabStop = false;
            // 
            // JObjTextureEditorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.textureArrayEditor);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip4);
            this.Name = "JObjTextureEditorDialog";
            this.Text = "JObjTextureEditor";
            this.groupBox1.ResumeLayout(false);
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private ArrayMemberEditor textureArrayEditor;
        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.ToolStripButton exporttoolStripButton;
        private System.Windows.Forms.ToolStripButton replaceTextureButton;
        private System.Windows.Forms.Splitter splitter1;
    }
}