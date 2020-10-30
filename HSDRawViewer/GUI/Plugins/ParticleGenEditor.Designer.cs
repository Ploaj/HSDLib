namespace HSDRawViewer.GUI.Plugins
{
    partial class ParticleGenEditor
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
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.OpCodeArrayEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.particleArrayEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(208, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 456);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.OpCodeArrayEditor);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(211, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(427, 456);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Particle Track";
            // 
            // OpCodeArrayEditor
            // 
            this.OpCodeArrayEditor.DisplayItemImages = false;
            this.OpCodeArrayEditor.DisplayItemIndices = true;
            this.OpCodeArrayEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OpCodeArrayEditor.EnablePropertyView = false;
            this.OpCodeArrayEditor.EnablePropertyViewDescription = false;
            this.OpCodeArrayEditor.ImageHeight = ((ushort)(24));
            this.OpCodeArrayEditor.ImageWidth = ((ushort)(24));
            this.OpCodeArrayEditor.ItemHeight = 13;
            this.OpCodeArrayEditor.ItemIndexOffset = 0;
            this.OpCodeArrayEditor.Location = new System.Drawing.Point(3, 16);
            this.OpCodeArrayEditor.Name = "OpCodeArrayEditor";
            this.OpCodeArrayEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.OpCodeArrayEditor.Size = new System.Drawing.Size(421, 437);
            this.OpCodeArrayEditor.TabIndex = 0;
            this.OpCodeArrayEditor.DoubleClickedNode += new System.EventHandler(this.OpCodeArrayEditor_DoubleClickedNode);
            this.OpCodeArrayEditor.ArrayUpdated += new System.EventHandler(this.OpCodeArrayEditor_ArrayUpdated);
            // 
            // particleArrayEditor
            // 
            this.particleArrayEditor.DisplayItemImages = false;
            this.particleArrayEditor.DisplayItemIndices = true;
            this.particleArrayEditor.Dock = System.Windows.Forms.DockStyle.Left;
            this.particleArrayEditor.EnablePropertyViewDescription = true;
            this.particleArrayEditor.ImageHeight = ((ushort)(24));
            this.particleArrayEditor.ImageWidth = ((ushort)(24));
            this.particleArrayEditor.ItemHeight = 13;
            this.particleArrayEditor.ItemIndexOffset = 0;
            this.particleArrayEditor.Location = new System.Drawing.Point(0, 0);
            this.particleArrayEditor.Name = "particleArrayEditor";
            this.particleArrayEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.particleArrayEditor.Size = new System.Drawing.Size(208, 456);
            this.particleArrayEditor.TabIndex = 0;
            this.particleArrayEditor.SelectedObjectChanged += new System.EventHandler(this.particleArrayEditor_SelectedObjectChanged);
            // 
            // ParticleGenEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 456);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.particleArrayEditor);
            this.Name = "ParticleGenEditor";
            this.TabText = "Particle Generator Editor";
            this.Text = "Particle Generator Editor";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ArrayMemberEditor particleArrayEditor;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.GroupBox groupBox1;
        private ArrayMemberEditor OpCodeArrayEditor;
    }
}