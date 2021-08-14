using Be.Windows.Forms;
using System.ComponentModel.Design;

namespace HSDRawViewer.GUI
{
    partial class PropertyView
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
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.hexbox = new Be.Windows.Forms.HexBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonRemovePointer = new System.Windows.Forms.Button();
            this.buttonSetPointer = new System.Windows.Forms.Button();
            this.propertyGrid2 = new System.Windows.Forms.PropertyGrid();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.structSize = new System.Windows.Forms.MaskedTextBox();
            this.offsetBox = new System.Windows.Forms.MaskedTextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Top;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid1.Size = new System.Drawing.Size(756, 221);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 221);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(756, 3);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // hexbox
            // 
            this.hexbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.hexbox.BackColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.hexbox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexbox.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.hexbox.GroupSeparatorVisible = true;
            this.hexbox.InfoForeColor = System.Drawing.Color.LightGray;
            this.hexbox.LineInfoVisible = true;
            this.hexbox.Location = new System.Drawing.Point(158, 19);
            this.hexbox.Name = "hexbox";
            this.hexbox.SelectionBackColor = System.Drawing.Color.MediumBlue;
            this.hexbox.SelectionForeColor = System.Drawing.Color.Yellow;
            this.hexbox.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexbox.Size = new System.Drawing.Size(594, 298);
            this.hexbox.StringViewVisible = true;
            this.hexbox.TabIndex = 2;
            this.hexbox.UseFixedBytesPerLine = true;
            this.hexbox.VScrollBarVisible = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonRemovePointer);
            this.groupBox1.Controls.Add(this.buttonSetPointer);
            this.groupBox1.Controls.Add(this.propertyGrid2);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.structSize);
            this.groupBox1.Controls.Add(this.offsetBox);
            this.groupBox1.Controls.Add(this.hexbox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 224);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(756, 329);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Hex View";
            // 
            // buttonRemovePointer
            // 
            this.buttonRemovePointer.Location = new System.Drawing.Point(77, 104);
            this.buttonRemovePointer.Name = "buttonRemovePointer";
            this.buttonRemovePointer.Size = new System.Drawing.Size(75, 45);
            this.buttonRemovePointer.TabIndex = 8;
            this.buttonRemovePointer.Text = "Remove Pointer";
            this.buttonRemovePointer.UseVisualStyleBackColor = true;
            this.buttonRemovePointer.Click += new System.EventHandler(this.buttonRemovePointer_Click);
            // 
            // buttonSetPointer
            // 
            this.buttonSetPointer.Location = new System.Drawing.Point(6, 104);
            this.buttonSetPointer.Name = "buttonSetPointer";
            this.buttonSetPointer.Size = new System.Drawing.Size(65, 45);
            this.buttonSetPointer.TabIndex = 7;
            this.buttonSetPointer.Text = "Set Pointer";
            this.buttonSetPointer.UseVisualStyleBackColor = true;
            this.buttonSetPointer.Click += new System.EventHandler(this.buttonSetPointer_Click);
            // 
            // propertyGrid2
            // 
            this.propertyGrid2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.propertyGrid2.HelpVisible = false;
            this.propertyGrid2.Location = new System.Drawing.Point(6, 168);
            this.propertyGrid2.Name = "propertyGrid2";
            this.propertyGrid2.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid2.Size = new System.Drawing.Size(146, 149);
            this.propertyGrid2.TabIndex = 6;
            this.propertyGrid2.ToolbarVisible = false;
            this.propertyGrid2.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid2_PropertyValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Memory Poke";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Struct Size:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Selected Offset:";
            // 
            // structSize
            // 
            this.structSize.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.structSize.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.structSize.Location = new System.Drawing.Point(6, 31);
            this.structSize.Name = "structSize";
            this.structSize.Size = new System.Drawing.Size(146, 22);
            this.structSize.TabIndex = 4;
            // 
            // offsetBox
            // 
            this.offsetBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.offsetBox.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.offsetBox.Location = new System.Drawing.Point(6, 76);
            this.offsetBox.Name = "offsetBox";
            this.offsetBox.Size = new System.Drawing.Size(146, 22);
            this.offsetBox.TabIndex = 4;
            this.offsetBox.TextChanged += new System.EventHandler(this.offsetBox_TextChanged);
            this.offsetBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.offsetBox_KeyDown);
            // 
            // PropertyView
            // 
            this.ClientSize = new System.Drawing.Size(756, 553);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.propertyGrid1);
            this.Name = "PropertyView";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Splitter splitter1;
        private HexBox hexbox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MaskedTextBox offsetBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PropertyGrid propertyGrid2;
        private System.Windows.Forms.Button buttonRemovePointer;
        private System.Windows.Forms.Button buttonSetPointer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MaskedTextBox structSize;
    }
}
