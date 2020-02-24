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
            this.ByteBox = new System.Windows.Forms.TextBox();
            this.Int16Box = new System.Windows.Forms.TextBox();
            this.Int32Box = new System.Windows.Forms.TextBox();
            this.floatBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.offsetBox = new System.Windows.Forms.MaskedTextBox();
            this.buttonByte = new System.Windows.Forms.Button();
            this.buttonInt16 = new System.Windows.Forms.Button();
            this.buttonInt32 = new System.Windows.Forms.Button();
            this.buttonFloat = new System.Windows.Forms.Button();
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
            this.hexbox.Location = new System.Drawing.Point(118, 19);
            this.hexbox.Name = "hexbox";
            this.hexbox.SelectionBackColor = System.Drawing.Color.MediumBlue;
            this.hexbox.SelectionForeColor = System.Drawing.Color.Yellow;
            this.hexbox.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexbox.Size = new System.Drawing.Size(634, 298);
            this.hexbox.StringViewVisible = true;
            this.hexbox.TabIndex = 2;
            this.hexbox.UseFixedBytesPerLine = true;
            this.hexbox.VScrollBarVisible = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ByteBox);
            this.groupBox1.Controls.Add(this.Int16Box);
            this.groupBox1.Controls.Add(this.Int32Box);
            this.groupBox1.Controls.Add(this.floatBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.offsetBox);
            this.groupBox1.Controls.Add(this.buttonByte);
            this.groupBox1.Controls.Add(this.buttonInt16);
            this.groupBox1.Controls.Add(this.buttonInt32);
            this.groupBox1.Controls.Add(this.buttonFloat);
            this.groupBox1.Controls.Add(this.hexbox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 224);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(756, 329);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Hex View";
            // 
            // ByteBox
            // 
            this.ByteBox.Location = new System.Drawing.Point(6, 289);
            this.ByteBox.MaxLength = 3;
            this.ByteBox.Name = "ByteBox";
            this.ByteBox.Size = new System.Drawing.Size(109, 20);
            this.ByteBox.TabIndex = 6;
            this.ByteBox.TextChanged += new System.EventHandler(this.ByteBox_TextChanged);
            // 
            // Int16Box
            // 
            this.Int16Box.Location = new System.Drawing.Point(6, 234);
            this.Int16Box.Name = "Int16Box";
            this.Int16Box.Size = new System.Drawing.Size(109, 20);
            this.Int16Box.TabIndex = 6;
            this.Int16Box.TextChanged += new System.EventHandler(this.Int16Box_TextChanged);
            // 
            // Int32Box
            // 
            this.Int32Box.Location = new System.Drawing.Point(6, 179);
            this.Int32Box.Name = "Int32Box";
            this.Int32Box.Size = new System.Drawing.Size(109, 20);
            this.Int32Box.TabIndex = 6;
            this.Int32Box.TextChanged += new System.EventHandler(this.Int32Box_TextChanged);
            // 
            // floatBox
            // 
            this.floatBox.Location = new System.Drawing.Point(6, 124);
            this.floatBox.Name = "floatBox";
            this.floatBox.Size = new System.Drawing.Size(109, 20);
            this.floatBox.TabIndex = 6;
            this.floatBox.TextChanged += new System.EventHandler(this.floatBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Memory Poke";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Offset:";
            // 
            // offsetBox
            // 
            this.offsetBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.offsetBox.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.offsetBox.Location = new System.Drawing.Point(6, 36);
            this.offsetBox.Name = "offsetBox";
            this.offsetBox.Size = new System.Drawing.Size(109, 22);
            this.offsetBox.TabIndex = 4;
            this.offsetBox.TextChanged += new System.EventHandler(this.offsetBox_TextChanged);
            this.offsetBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.offsetBox_KeyDown);
            // 
            // buttonByte
            // 
            this.buttonByte.Enabled = false;
            this.buttonByte.Location = new System.Drawing.Point(6, 260);
            this.buttonByte.Name = "buttonByte";
            this.buttonByte.Size = new System.Drawing.Size(109, 23);
            this.buttonByte.TabIndex = 3;
            this.buttonByte.Text = "Set Byte";
            this.buttonByte.UseVisualStyleBackColor = true;
            this.buttonByte.Click += new System.EventHandler(this.buttonByte_Click);
            // 
            // buttonInt16
            // 
            this.buttonInt16.Location = new System.Drawing.Point(6, 205);
            this.buttonInt16.Name = "buttonInt16";
            this.buttonInt16.Size = new System.Drawing.Size(109, 23);
            this.buttonInt16.TabIndex = 3;
            this.buttonInt16.Text = "Set Int16";
            this.buttonInt16.UseVisualStyleBackColor = true;
            this.buttonInt16.Click += new System.EventHandler(this.buttonInt16_Click);
            // 
            // buttonInt32
            // 
            this.buttonInt32.Location = new System.Drawing.Point(6, 150);
            this.buttonInt32.Name = "buttonInt32";
            this.buttonInt32.Size = new System.Drawing.Size(109, 23);
            this.buttonInt32.TabIndex = 3;
            this.buttonInt32.Text = "Set Int32";
            this.buttonInt32.UseVisualStyleBackColor = true;
            this.buttonInt32.Click += new System.EventHandler(this.buttonInt32_Click);
            // 
            // buttonFloat
            // 
            this.buttonFloat.Location = new System.Drawing.Point(6, 95);
            this.buttonFloat.Name = "buttonFloat";
            this.buttonFloat.Size = new System.Drawing.Size(109, 23);
            this.buttonFloat.TabIndex = 3;
            this.buttonFloat.Text = "Set Float";
            this.buttonFloat.UseVisualStyleBackColor = true;
            this.buttonFloat.Click += new System.EventHandler(this.buttonFloat_Click);
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
        private System.Windows.Forms.Button buttonFloat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MaskedTextBox offsetBox;
        private System.Windows.Forms.TextBox floatBox;
        private System.Windows.Forms.TextBox Int32Box;
        private System.Windows.Forms.Button buttonInt32;
        private System.Windows.Forms.TextBox Int16Box;
        private System.Windows.Forms.Button buttonInt16;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ByteBox;
        private System.Windows.Forms.Button buttonByte;
    }
}
