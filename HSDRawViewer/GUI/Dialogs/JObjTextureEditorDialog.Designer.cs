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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JObjTextureEditorDialog));
            groupBox1 = new System.Windows.Forms.GroupBox();
            propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            textureArrayEditor = new ArrayMemberEditor();
            toolStrip4 = new System.Windows.Forms.ToolStrip();
            exporttoolStripButton = new System.Windows.Forms.ToolStripButton();
            replaceTextureButton = new System.Windows.Forms.ToolStripButton();
            toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            splitter1 = new System.Windows.Forms.Splitter();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            groupBox1.SuspendLayout();
            toolStrip4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.Controls.Add(propertyGrid1);
            groupBox1.Location = new System.Drawing.Point(549, 329);
            groupBox1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            groupBox1.Size = new System.Drawing.Size(351, 268);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "Parameters";
            // 
            // propertyGrid1
            // 
            propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            propertyGrid1.HelpVisible = false;
            propertyGrid1.Location = new System.Drawing.Point(5, 24);
            propertyGrid1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            propertyGrid1.Size = new System.Drawing.Size(341, 240);
            propertyGrid1.TabIndex = 1;
            propertyGrid1.ToolbarVisible = false;
            // 
            // textureArrayEditor
            // 
            textureArrayEditor.AllowDrop = true;
            textureArrayEditor.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            textureArrayEditor.DisplayItemImages = true;
            textureArrayEditor.DisplayItemIndices = true;
            textureArrayEditor.EnablePropertyView = false;
            textureArrayEditor.EnablePropertyViewDescription = false;
            textureArrayEditor.EnableToolStrip = false;
            textureArrayEditor.ImageHeight = (ushort)64;
            textureArrayEditor.ImageWidth = (ushort)64;
            textureArrayEditor.InsertCloneAfterSelected = false;
            textureArrayEditor.ItemHeight = 64;
            textureArrayEditor.ItemIndexOffset = 0;
            textureArrayEditor.Location = new System.Drawing.Point(0, 27);
            textureArrayEditor.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            textureArrayEditor.Name = "textureArrayEditor";
            textureArrayEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            textureArrayEditor.Size = new System.Drawing.Size(538, 573);
            textureArrayEditor.TabIndex = 4;
            textureArrayEditor.SelectedObjectChanged += textureArrayEditor_SelectedObjectChanged;
            textureArrayEditor.DragDrop += textureArrayEditor_DragDrop;
            textureArrayEditor.DragEnter += textureArrayEditor_DragEnter;
            // 
            // toolStrip4
            // 
            toolStrip4.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { exporttoolStripButton, replaceTextureButton, toolStripButton1, toolStripButton2 });
            toolStrip4.Location = new System.Drawing.Point(0, 0);
            toolStrip4.Name = "toolStrip4";
            toolStrip4.Size = new System.Drawing.Size(914, 27);
            toolStrip4.TabIndex = 5;
            toolStrip4.Text = "toolStrip4";
            // 
            // exporttoolStripButton
            // 
            exporttoolStripButton.Image = Properties.Resources.ts_exportfile;
            exporttoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            exporttoolStripButton.Name = "exporttoolStripButton";
            exporttoolStripButton.Size = new System.Drawing.Size(128, 24);
            exporttoolStripButton.Text = "Export Texture";
            exporttoolStripButton.Click += exporttoolStripButton_Click;
            // 
            // replaceTextureButton
            // 
            replaceTextureButton.Image = Properties.Resources.ico_replace;
            replaceTextureButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            replaceTextureButton.Name = "replaceTextureButton";
            replaceTextureButton.Size = new System.Drawing.Size(138, 24);
            replaceTextureButton.Text = "Replace Texture";
            replaceTextureButton.Click += replaceTextureButton_Click;
            // 
            // toolStripButton1
            // 
            toolStripButton1.Image = Properties.Resources.ts_exportfile;
            toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new System.Drawing.Size(156, 24);
            toolStripButton1.Text = "Export All Textures";
            toolStripButton1.Click += toolStripButton1_Click;
            // 
            // toolStripButton2
            // 
            toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButton2.Image = (System.Drawing.Image)resources.GetObject("toolStripButton2.Image");
            toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripButton2.Name = "toolStripButton2";
            toolStripButton2.Size = new System.Drawing.Size(91, 24);
            toolStripButton2.Text = "Edit Texture";
            toolStripButton2.Click += toolStripButton2_Click;
            // 
            // splitter1
            // 
            splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            splitter1.Location = new System.Drawing.Point(911, 27);
            splitter1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            splitter1.Name = "splitter1";
            splitter1.Size = new System.Drawing.Size(3, 573);
            splitter1.TabIndex = 7;
            splitter1.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pictureBox1.Location = new System.Drawing.Point(547, 30);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(355, 292);
            pictureBox1.TabIndex = 8;
            pictureBox1.TabStop = false;
            // 
            // JObjTextureEditorDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(914, 600);
            Controls.Add(pictureBox1);
            Controls.Add(splitter1);
            Controls.Add(textureArrayEditor);
            Controls.Add(groupBox1);
            Controls.Add(toolStrip4);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "JObjTextureEditorDialog";
            Text = "JObjTextureEditor";
            groupBox1.ResumeLayout(false);
            toolStrip4.ResumeLayout(false);
            toolStrip4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private ArrayMemberEditor textureArrayEditor;
        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.ToolStripButton exporttoolStripButton;
        private System.Windows.Forms.ToolStripButton replaceTextureButton;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}