namespace HSDRawViewer.GUI
{
    partial class AJSplitDialog
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
            this.buttonAJ = new System.Windows.Forms.Button();
            this.buttonFolder = new System.Windows.Forms.Button();
            this.labelAJ = new System.Windows.Forms.Label();
            this.labelFolder = new System.Windows.Forms.Label();
            this.buttonExport = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonImport = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonAJ
            // 
            this.buttonAJ.Location = new System.Drawing.Point(3, 3);
            this.buttonAJ.Name = "buttonAJ";
            this.buttonAJ.Size = new System.Drawing.Size(75, 41);
            this.buttonAJ.TabIndex = 0;
            this.buttonAJ.Text = "Select Pl**AJ.dat";
            this.buttonAJ.UseVisualStyleBackColor = true;
            this.buttonAJ.Click += new System.EventHandler(this.buttonAJ_Click);
            // 
            // buttonFolder
            // 
            this.buttonFolder.Location = new System.Drawing.Point(3, 53);
            this.buttonFolder.Name = "buttonFolder";
            this.buttonFolder.Size = new System.Drawing.Size(75, 49);
            this.buttonFolder.TabIndex = 1;
            this.buttonFolder.Text = "Select Folder";
            this.buttonFolder.UseVisualStyleBackColor = true;
            this.buttonFolder.Click += new System.EventHandler(this.buttonFolder_Click);
            // 
            // labelAJ
            // 
            this.labelAJ.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelAJ.AutoSize = true;
            this.labelAJ.Location = new System.Drawing.Point(97, 18);
            this.labelAJ.Name = "labelAJ";
            this.labelAJ.Size = new System.Drawing.Size(35, 13);
            this.labelAJ.TabIndex = 2;
            this.labelAJ.Text = "label1";
            // 
            // labelFolder
            // 
            this.labelFolder.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelFolder.AutoSize = true;
            this.labelFolder.Location = new System.Drawing.Point(97, 73);
            this.labelFolder.Name = "labelFolder";
            this.labelFolder.Size = new System.Drawing.Size(35, 13);
            this.labelFolder.TabIndex = 3;
            this.labelFolder.Text = "label2";
            // 
            // buttonExport
            // 
            this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExport.Location = new System.Drawing.Point(13, 130);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(507, 23);
            this.buttonExport.TabIndex = 4;
            this.buttonExport.Text = "Export Animation To Folder";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 94F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.buttonAJ, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonFolder, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelAJ, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelFolder, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(508, 110);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // buttonImport
            // 
            this.buttonImport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonImport.Location = new System.Drawing.Point(15, 159);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(505, 23);
            this.buttonImport.TabIndex = 9;
            this.buttonImport.Text = "Inject Animations into Pl**AJ file";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // AJSplitDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 190);
            this.Controls.Add(this.buttonImport);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.buttonExport);
            this.Name = "AJSplitDialog";
            this.Text = "AJ Split";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAJ;
        private System.Windows.Forms.Button buttonFolder;
        private System.Windows.Forms.Label labelAJ;
        private System.Windows.Forms.Label labelFolder;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonImport;
    }
}