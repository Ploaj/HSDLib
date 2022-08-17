namespace HSDRawViewer.GUI.Extra
{
    partial class ModelImportDialog
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.meshList = new System.Windows.Forms.ListView();
            this.selectAllMesh = new System.Windows.Forms.Button();
            this.meshProperty = new System.Windows.Forms.PropertyGrid();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.materialList = new System.Windows.Forms.ListView();
            this.selectAllMaterials = new System.Windows.Forms.Button();
            this.materialProperty = new System.Windows.Forms.PropertyGrid();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.boneTree = new System.Windows.Forms.TreeView();
            this.boneProperty = new System.Windows.Forms.PropertyGrid();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.mainProperty = new System.Windows.Forms.PropertyGrid();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(824, 514);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitter1);
            this.tabPage1.Controls.Add(this.meshList);
            this.tabPage1.Controls.Add(this.selectAllMesh);
            this.tabPage1.Controls.Add(this.meshProperty);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(816, 488);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Mesh List";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(365, 26);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 459);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // meshList
            // 
            this.meshList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.meshList.HideSelection = false;
            this.meshList.Location = new System.Drawing.Point(3, 26);
            this.meshList.Name = "meshList";
            this.meshList.Size = new System.Drawing.Size(365, 459);
            this.meshList.TabIndex = 3;
            this.meshList.UseCompatibleStateImageBehavior = false;
            this.meshList.View = System.Windows.Forms.View.List;
            // 
            // selectAllMesh
            // 
            this.selectAllMesh.Dock = System.Windows.Forms.DockStyle.Top;
            this.selectAllMesh.Location = new System.Drawing.Point(3, 3);
            this.selectAllMesh.Name = "selectAllMesh";
            this.selectAllMesh.Size = new System.Drawing.Size(365, 23);
            this.selectAllMesh.TabIndex = 5;
            this.selectAllMesh.Text = "Select All";
            this.selectAllMesh.UseVisualStyleBackColor = true;
            this.selectAllMesh.Click += new System.EventHandler(this.selectAllMesh_Click);
            // 
            // meshProperty
            // 
            this.meshProperty.Dock = System.Windows.Forms.DockStyle.Right;
            this.meshProperty.Location = new System.Drawing.Point(368, 3);
            this.meshProperty.Name = "meshProperty";
            this.meshProperty.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.meshProperty.Size = new System.Drawing.Size(445, 482);
            this.meshProperty.TabIndex = 2;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.splitter3);
            this.tabPage4.Controls.Add(this.materialList);
            this.tabPage4.Controls.Add(this.selectAllMaterials);
            this.tabPage4.Controls.Add(this.materialProperty);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(816, 488);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Materials";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter3.Location = new System.Drawing.Point(368, 23);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(3, 465);
            this.splitter3.TabIndex = 6;
            this.splitter3.TabStop = false;
            // 
            // materialList
            // 
            this.materialList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialList.HideSelection = false;
            this.materialList.Location = new System.Drawing.Point(0, 23);
            this.materialList.Name = "materialList";
            this.materialList.Size = new System.Drawing.Size(371, 465);
            this.materialList.TabIndex = 4;
            this.materialList.UseCompatibleStateImageBehavior = false;
            this.materialList.View = System.Windows.Forms.View.List;
            // 
            // selectAllMaterials
            // 
            this.selectAllMaterials.Dock = System.Windows.Forms.DockStyle.Top;
            this.selectAllMaterials.Location = new System.Drawing.Point(0, 0);
            this.selectAllMaterials.Name = "selectAllMaterials";
            this.selectAllMaterials.Size = new System.Drawing.Size(371, 23);
            this.selectAllMaterials.TabIndex = 7;
            this.selectAllMaterials.Text = "Select All";
            this.selectAllMaterials.UseVisualStyleBackColor = true;
            this.selectAllMaterials.Click += new System.EventHandler(this.selectAllMaterials_Click);
            // 
            // materialProperty
            // 
            this.materialProperty.Dock = System.Windows.Forms.DockStyle.Right;
            this.materialProperty.Location = new System.Drawing.Point(371, 0);
            this.materialProperty.Name = "materialProperty";
            this.materialProperty.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.materialProperty.Size = new System.Drawing.Size(445, 488);
            this.materialProperty.TabIndex = 5;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitter2);
            this.tabPage2.Controls.Add(this.boneTree);
            this.tabPage2.Controls.Add(this.boneProperty);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(816, 488);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Bone List";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter2.Location = new System.Drawing.Point(368, 3);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 482);
            this.splitter2.TabIndex = 2;
            this.splitter2.TabStop = false;
            // 
            // boneTree
            // 
            this.boneTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boneTree.Location = new System.Drawing.Point(3, 3);
            this.boneTree.Name = "boneTree";
            this.boneTree.Size = new System.Drawing.Size(368, 482);
            this.boneTree.TabIndex = 0;
            // 
            // boneProperty
            // 
            this.boneProperty.Dock = System.Windows.Forms.DockStyle.Right;
            this.boneProperty.Location = new System.Drawing.Point(371, 3);
            this.boneProperty.Name = "boneProperty";
            this.boneProperty.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.boneProperty.Size = new System.Drawing.Size(442, 482);
            this.boneProperty.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.mainProperty);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(816, 488);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Misc";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // mainProperty
            // 
            this.mainProperty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainProperty.Location = new System.Drawing.Point(0, 0);
            this.mainProperty.Name = "mainProperty";
            this.mainProperty.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.mainProperty.Size = new System.Drawing.Size(816, 488);
            this.mainProperty.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.Location = new System.Drawing.Point(0, 538);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(824, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Import Model";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(824, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importSettingsToolStripMenuItem,
            this.exportSettingsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exportSettingsToolStripMenuItem
            // 
            this.exportSettingsToolStripMenuItem.Name = "exportSettingsToolStripMenuItem";
            this.exportSettingsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportSettingsToolStripMenuItem.Text = "Export Settings";
            this.exportSettingsToolStripMenuItem.Click += new System.EventHandler(this.exportSettingsToolStripMenuItem_Click);
            // 
            // importSettingsToolStripMenuItem
            // 
            this.importSettingsToolStripMenuItem.Name = "importSettingsToolStripMenuItem";
            this.importSettingsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.importSettingsToolStripMenuItem.Text = "Import Settings";
            this.importSettingsToolStripMenuItem.Click += new System.EventHandler(this.importSettingsToolStripMenuItem_Click);
            // 
            // ModelImportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 561);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "ModelImportDialog";
            this.Text = "Model Import Dialog";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PropertyGrid meshProperty;
        private System.Windows.Forms.TreeView boneTree;
        private System.Windows.Forms.PropertyGrid boneProperty;
        private System.Windows.Forms.ListView meshList;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.PropertyGrid mainProperty;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.PropertyGrid materialProperty;
        private System.Windows.Forms.ListView materialList;
        private System.Windows.Forms.Button selectAllMesh;
        private System.Windows.Forms.Button selectAllMaterials;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importSettingsToolStripMenuItem;
    }
}