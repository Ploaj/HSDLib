
namespace HSDRawViewer.GUI.Plugins
{
    partial class ParticleEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParticleEditor));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.propertyGridEvent = new System.Windows.Forms.PropertyGrid();
            this.cbEventType = new System.Windows.Forms.ComboBox();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBoxPreview = new System.Windows.Forms.GroupBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonGenGenerator = new System.Windows.Forms.ToolStripButton();
            this.buttonKillAll = new System.Windows.Forms.ToolStripButton();
            this.buttonPause = new System.Windows.Forms.ToolStripButton();
            this.buttonStep = new System.Windows.Forms.ToolStripButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.propertyGridGenerator = new System.Windows.Forms.PropertyGrid();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.buttonSave = new System.Windows.Forms.ToolStripButton();
            this.ptclEventArrayEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.generateArrayEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.buttonRandom = new System.Windows.Forms.ToolStripButton();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxPreview.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.splitter4);
            this.groupBox1.Controls.Add(this.ptclEventArrayEditor);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(206, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 505);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Particle Events";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.propertyGridEvent);
            this.groupBox3.Controls.Add(this.cbEventType);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 322);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(194, 180);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Selected Event";
            // 
            // propertyGridEvent
            // 
            this.propertyGridEvent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridEvent.Location = new System.Drawing.Point(3, 37);
            this.propertyGridEvent.Name = "propertyGridEvent";
            this.propertyGridEvent.Size = new System.Drawing.Size(188, 140);
            this.propertyGridEvent.TabIndex = 5;
            this.propertyGridEvent.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridEvent_PropertyValueChanged);
            // 
            // cbEventType
            // 
            this.cbEventType.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbEventType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEventType.FormattingEnabled = true;
            this.cbEventType.Location = new System.Drawing.Point(3, 16);
            this.cbEventType.Name = "cbEventType";
            this.cbEventType.Size = new System.Drawing.Size(188, 21);
            this.cbEventType.TabIndex = 4;
            this.cbEventType.SelectedIndexChanged += new System.EventHandler(this.cbEventType_SelectedIndexChanged);
            // 
            // splitter4
            // 
            this.splitter4.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter4.Location = new System.Drawing.Point(3, 316);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(194, 6);
            this.splitter4.TabIndex = 6;
            this.splitter4.TabStop = false;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(406, 25);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(6, 505);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(200, 25);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(6, 505);
            this.splitter2.TabIndex = 3;
            this.splitter2.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.generateArrayEditor);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox2.Location = new System.Drawing.Point(0, 25);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 505);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Generators";
            // 
            // groupBoxPreview
            // 
            this.groupBoxPreview.Controls.Add(this.toolStrip1);
            this.groupBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxPreview.Location = new System.Drawing.Point(412, 279);
            this.groupBoxPreview.Name = "groupBoxPreview";
            this.groupBoxPreview.Size = new System.Drawing.Size(418, 251);
            this.groupBoxPreview.TabIndex = 5;
            this.groupBoxPreview.TabStop = false;
            this.groupBoxPreview.Text = "Preview";
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonGenGenerator,
            this.buttonKillAll,
            this.buttonPause,
            this.buttonStep,
            this.buttonRandom});
            this.toolStrip1.Location = new System.Drawing.Point(3, 16);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(412, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonGenGenerator
            // 
            this.buttonGenGenerator.ForeColor = System.Drawing.SystemColors.Control;
            this.buttonGenGenerator.Image = global::HSDRawViewer.Properties.Resources.ts_visible;
            this.buttonGenGenerator.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonGenGenerator.Name = "buttonGenGenerator";
            this.buttonGenGenerator.Size = new System.Drawing.Size(117, 22);
            this.buttonGenGenerator.Text = "Spawn Generator";
            this.buttonGenGenerator.Click += new System.EventHandler(this.buttonGenGenerator_Click);
            // 
            // buttonKillAll
            // 
            this.buttonKillAll.ForeColor = System.Drawing.SystemColors.Control;
            this.buttonKillAll.Image = global::HSDRawViewer.Properties.Resources.ts_stop;
            this.buttonKillAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonKillAll.Name = "buttonKillAll";
            this.buttonKillAll.Size = new System.Drawing.Size(51, 22);
            this.buttonKillAll.Text = "Stop";
            this.buttonKillAll.Click += new System.EventHandler(this.buttonKillAll_Click);
            // 
            // buttonPause
            // 
            this.buttonPause.CheckOnClick = true;
            this.buttonPause.ForeColor = System.Drawing.SystemColors.Control;
            this.buttonPause.Image = global::HSDRawViewer.Properties.Resources.ts_pause;
            this.buttonPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(58, 22);
            this.buttonPause.Text = "Pause";
            // 
            // buttonStep
            // 
            this.buttonStep.ForeColor = System.Drawing.SystemColors.Control;
            this.buttonStep.Image = global::HSDRawViewer.Properties.Resources.ts_add;
            this.buttonStep.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonStep.Name = "buttonStep";
            this.buttonStep.Size = new System.Drawing.Size(50, 22);
            this.buttonStep.Text = "Step";
            this.buttonStep.Click += new System.EventHandler(this.buttonStep_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.propertyGridGenerator);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox4.Location = new System.Drawing.Point(412, 25);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(418, 248);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Generator Params";
            // 
            // propertyGridGenerator
            // 
            this.propertyGridGenerator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridGenerator.HelpVisible = false;
            this.propertyGridGenerator.Location = new System.Drawing.Point(3, 16);
            this.propertyGridGenerator.Name = "propertyGridGenerator";
            this.propertyGridGenerator.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGridGenerator.Size = new System.Drawing.Size(412, 229);
            this.propertyGridGenerator.TabIndex = 7;
            this.propertyGridGenerator.ToolbarVisible = false;
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter3.Location = new System.Drawing.Point(412, 273);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(418, 6);
            this.splitter3.TabIndex = 6;
            this.splitter3.TabStop = false;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSave});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(830, 25);
            this.toolStrip2.TabIndex = 7;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // buttonSave
            // 
            this.buttonSave.Image = global::HSDRawViewer.Properties.Resources.ico_save;
            this.buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(100, 22);
            this.buttonSave.Text = "Save Changes";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // ptclEventArrayEditor
            // 
            this.ptclEventArrayEditor.DisplayItemImages = false;
            this.ptclEventArrayEditor.DisplayItemIndices = true;
            this.ptclEventArrayEditor.Dock = System.Windows.Forms.DockStyle.Top;
            this.ptclEventArrayEditor.EnablePropertyView = false;
            this.ptclEventArrayEditor.EnablePropertyViewDescription = true;
            this.ptclEventArrayEditor.ImageHeight = ((ushort)(24));
            this.ptclEventArrayEditor.ImageWidth = ((ushort)(24));
            this.ptclEventArrayEditor.InsertCloneAfterSelected = true;
            this.ptclEventArrayEditor.ItemHeight = 13;
            this.ptclEventArrayEditor.ItemIndexOffset = 0;
            this.ptclEventArrayEditor.Location = new System.Drawing.Point(3, 16);
            this.ptclEventArrayEditor.Name = "ptclEventArrayEditor";
            this.ptclEventArrayEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.ptclEventArrayEditor.Size = new System.Drawing.Size(194, 300);
            this.ptclEventArrayEditor.TabIndex = 3;
            this.ptclEventArrayEditor.SelectedObjectChanged += new System.EventHandler(this.ptclEventArrayEditor_SelectedObjectChanged);
            // 
            // generateArrayEditor
            // 
            this.generateArrayEditor.DisplayItemImages = false;
            this.generateArrayEditor.DisplayItemIndices = true;
            this.generateArrayEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generateArrayEditor.EnablePropertyView = false;
            this.generateArrayEditor.EnablePropertyViewDescription = true;
            this.generateArrayEditor.ImageHeight = ((ushort)(24));
            this.generateArrayEditor.ImageWidth = ((ushort)(24));
            this.generateArrayEditor.InsertCloneAfterSelected = false;
            this.generateArrayEditor.ItemHeight = 13;
            this.generateArrayEditor.ItemIndexOffset = 0;
            this.generateArrayEditor.Location = new System.Drawing.Point(3, 16);
            this.generateArrayEditor.Name = "generateArrayEditor";
            this.generateArrayEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.generateArrayEditor.Size = new System.Drawing.Size(194, 486);
            this.generateArrayEditor.TabIndex = 1;
            this.generateArrayEditor.ArrayUpdated += new System.EventHandler(this.generateArrayEditor_ArrayUpdated);
            // 
            // buttonRandom
            // 
            this.buttonRandom.CheckOnClick = true;
            this.buttonRandom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonRandom.ForeColor = System.Drawing.SystemColors.Control;
            this.buttonRandom.Image = ((System.Drawing.Image)(resources.GetObject("buttonRandom.Image")));
            this.buttonRandom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRandom.Name = "buttonRandom";
            this.buttonRandom.Size = new System.Drawing.Size(84, 22);
            this.buttonRandom.Text = "Random Seed";
            this.buttonRandom.CheckedChanged += new System.EventHandler(this.buttonRandom_CheckedChanged);
            // 
            // ParticleEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 530);
            this.Controls.Add(this.groupBoxPreview);
            this.Controls.Add(this.splitter3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.toolStrip2);
            this.Name = "ParticleEditor";
            this.TabText = "ParticleEditor";
            this.Text = "ParticleEditor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBoxPreview.ResumeLayout(false);
            this.groupBoxPreview.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.GroupBox groupBox2;
        private ArrayMemberEditor generateArrayEditor;
        private System.Windows.Forms.GroupBox groupBoxPreview;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.PropertyGrid propertyGridGenerator;
        private System.Windows.Forms.ToolStripButton buttonPause;
        private System.Windows.Forms.ToolStripButton buttonGenGenerator;
        private ArrayMemberEditor ptclEventArrayEditor;
        private System.Windows.Forms.ToolStripButton buttonStep;
        private System.Windows.Forms.ToolStripButton buttonKillAll;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.Splitter splitter4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cbEventType;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton buttonSave;
        private System.Windows.Forms.PropertyGrid propertyGridEvent;
        private System.Windows.Forms.ToolStripButton buttonRandom;
    }
}