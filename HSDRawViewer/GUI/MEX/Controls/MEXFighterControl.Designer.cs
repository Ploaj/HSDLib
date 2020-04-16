namespace HSDRawViewer.GUI.MEX.Controls
{
    partial class MEXFighterControl
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.saveFightersButton = new System.Windows.Forms.ToolStripButton();
            this.exportFighter = new System.Windows.Forms.ToolStripButton();
            this.importFighter = new System.Windows.Forms.ToolStripButton();
            this.cloneButton = new System.Windows.Forms.ToolStripButton();
            this.deleteFighter = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.fighterList = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.fighterPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.functionPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.buttonCopyMoveLogic = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveFightersButton,
            this.exportFighter,
            this.importFighter,
            this.cloneButton,
            this.deleteFighter});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(720, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // saveFightersButton
            // 
            this.saveFightersButton.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.saveFightersButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveFightersButton.Name = "saveFightersButton";
            this.saveFightersButton.Size = new System.Drawing.Size(140, 22);
            this.saveFightersButton.Text = "Save Fighter Changes";
            this.saveFightersButton.Click += new System.EventHandler(this.saveFightersButton_Click);
            // 
            // exportFighter
            // 
            this.exportFighter.Image = global::HSDRawViewer.Properties.Resources.ts_exportfile;
            this.exportFighter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exportFighter.Name = "exportFighter";
            this.exportFighter.Size = new System.Drawing.Size(101, 22);
            this.exportFighter.Text = "Export Fighter";
            this.exportFighter.Click += new System.EventHandler(this.exportFighter_Click);
            // 
            // importFighter
            // 
            this.importFighter.Image = global::HSDRawViewer.Properties.Resources.ts_importfile;
            this.importFighter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.importFighter.Name = "importFighter";
            this.importFighter.Size = new System.Drawing.Size(103, 22);
            this.importFighter.Text = "Import Fighter";
            this.importFighter.Click += new System.EventHandler(this.importFighter_Click);
            // 
            // cloneButton
            // 
            this.cloneButton.Image = global::HSDRawViewer.Properties.Resources.ts_clone;
            this.cloneButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cloneButton.Name = "cloneButton";
            this.cloneButton.Size = new System.Drawing.Size(98, 22);
            this.cloneButton.Text = "Clone Fighter";
            this.cloneButton.Click += new System.EventHandler(this.cloneButton_Click);
            // 
            // deleteFighter
            // 
            this.deleteFighter.Image = global::HSDRawViewer.Properties.Resources.ts_subtract;
            this.deleteFighter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteFighter.Name = "deleteFighter";
            this.deleteFighter.Size = new System.Drawing.Size(100, 22);
            this.deleteFighter.Text = "Delete Fighter";
            this.deleteFighter.Click += new System.EventHandler(this.deleteFighter_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.fighterList);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(0, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 455);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Fighters";
            // 
            // fighterList
            // 
            this.fighterList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fighterList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.fighterList.FormattingEnabled = true;
            this.fighterList.Location = new System.Drawing.Point(3, 16);
            this.fighterList.Name = "fighterList";
            this.fighterList.Size = new System.Drawing.Size(194, 436);
            this.fighterList.TabIndex = 0;
            this.fighterList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.fighterList_DrawItem);
            this.fighterList.SelectedIndexChanged += new System.EventHandler(this.fighterList_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(200, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(520, 455);
            this.tabControl1.TabIndex = 7;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.fighterPropertyGrid);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(512, 429);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Properties";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // fighterPropertyGrid
            // 
            this.fighterPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fighterPropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.fighterPropertyGrid.Name = "fighterPropertyGrid";
            this.fighterPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.fighterPropertyGrid.Size = new System.Drawing.Size(506, 423);
            this.fighterPropertyGrid.TabIndex = 0;
            this.fighterPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.fighterPropertyGrid_PropertyValueChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.functionPropertyGrid);
            this.tabPage4.Controls.Add(this.buttonCopyMoveLogic);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(512, 429);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Functions";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // functionPropertyGrid
            // 
            this.functionPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.functionPropertyGrid.Location = new System.Drawing.Point(3, 26);
            this.functionPropertyGrid.Name = "functionPropertyGrid";
            this.functionPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.functionPropertyGrid.Size = new System.Drawing.Size(506, 400);
            this.functionPropertyGrid.TabIndex = 1;
            this.functionPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid2_PropertyValueChanged);
            // 
            // buttonCopyMoveLogic
            // 
            this.buttonCopyMoveLogic.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonCopyMoveLogic.Location = new System.Drawing.Point(3, 3);
            this.buttonCopyMoveLogic.Name = "buttonCopyMoveLogic";
            this.buttonCopyMoveLogic.Size = new System.Drawing.Size(506, 23);
            this.buttonCopyMoveLogic.TabIndex = 2;
            this.buttonCopyMoveLogic.Text = "Copy Move Logic to Clipboard";
            this.buttonCopyMoveLogic.UseVisualStyleBackColor = true;
            this.buttonCopyMoveLogic.Click += new System.EventHandler(this.buttonCopyMoveLogic_Click);
            // 
            // MEXFighterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MEXFighterControl";
            this.Size = new System.Drawing.Size(720, 480);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton saveFightersButton;
        private System.Windows.Forms.ToolStripButton exportFighter;
        private System.Windows.Forms.ToolStripButton importFighter;
        private System.Windows.Forms.ToolStripButton cloneButton;
        private System.Windows.Forms.ToolStripButton deleteFighter;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox fighterList;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.PropertyGrid fighterPropertyGrid;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.PropertyGrid functionPropertyGrid;
        private System.Windows.Forms.Button buttonCopyMoveLogic;
    }
}
