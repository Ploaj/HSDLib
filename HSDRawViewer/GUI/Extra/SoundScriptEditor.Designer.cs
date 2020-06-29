namespace HSDRawViewer.GUI.Extra
{
    partial class SoundScriptEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoundScriptEditor));
            this.soundBox = new System.Windows.Forms.GroupBox();
            this.scriptGroup = new System.Windows.Forms.GroupBox();
            this.scriptBox = new System.Windows.Forms.RichTextBox();
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.copyScriptButton = new System.Windows.Forms.ToolStripButton();
            this.pasteScriptButton = new System.Windows.Forms.ToolStripButton();
            this.buttonSaveScript = new System.Windows.Forms.Button();
            this.scriptArrayEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.soundBox.SuspendLayout();
            this.scriptGroup.SuspendLayout();
            this.toolStrip4.SuspendLayout();
            this.SuspendLayout();
            // 
            // soundBox
            // 
            this.soundBox.Controls.Add(this.scriptGroup);
            this.soundBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.soundBox.Location = new System.Drawing.Point(0, 0);
            this.soundBox.Name = "soundBox";
            this.soundBox.Size = new System.Drawing.Size(743, 220);
            this.soundBox.TabIndex = 6;
            this.soundBox.TabStop = false;
            this.soundBox.Text = "Sound Script";
            // 
            // scriptGroup
            // 
            this.scriptGroup.Controls.Add(this.scriptBox);
            this.scriptGroup.Controls.Add(this.toolStrip4);
            this.scriptGroup.Controls.Add(this.buttonSaveScript);
            this.scriptGroup.Controls.Add(this.scriptArrayEditor);
            this.scriptGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scriptGroup.Location = new System.Drawing.Point(3, 16);
            this.scriptGroup.Name = "scriptGroup";
            this.scriptGroup.Size = new System.Drawing.Size(737, 201);
            this.scriptGroup.TabIndex = 5;
            this.scriptGroup.TabStop = false;
            this.scriptGroup.Text = "Scripts";
            // 
            // scriptBox
            // 
            this.scriptBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scriptBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scriptBox.Location = new System.Drawing.Point(211, 64);
            this.scriptBox.Name = "scriptBox";
            this.scriptBox.Size = new System.Drawing.Size(523, 134);
            this.scriptBox.TabIndex = 0;
            this.scriptBox.Text = "";
            // 
            // toolStrip4
            // 
            this.toolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyScriptButton,
            this.pasteScriptButton});
            this.toolStrip4.Location = new System.Drawing.Point(211, 39);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.Size = new System.Drawing.Size(523, 25);
            this.toolStrip4.TabIndex = 3;
            this.toolStrip4.Text = "toolStrip4";
            // 
            // copyScriptButton
            // 
            this.copyScriptButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.copyScriptButton.Image = ((System.Drawing.Image)(resources.GetObject("copyScriptButton.Image")));
            this.copyScriptButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyScriptButton.Name = "copyScriptButton";
            this.copyScriptButton.Size = new System.Drawing.Size(39, 22);
            this.copyScriptButton.Text = "Copy";
            this.copyScriptButton.Click += new System.EventHandler(this.copyScriptButton_Click);
            // 
            // pasteScriptButton
            // 
            this.pasteScriptButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.pasteScriptButton.Image = ((System.Drawing.Image)(resources.GetObject("pasteScriptButton.Image")));
            this.pasteScriptButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteScriptButton.Name = "pasteScriptButton";
            this.pasteScriptButton.Size = new System.Drawing.Size(39, 22);
            this.pasteScriptButton.Text = "Paste";
            this.pasteScriptButton.Click += new System.EventHandler(this.pasteScriptButton_Click);
            // 
            // buttonSaveScript
            // 
            this.buttonSaveScript.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonSaveScript.Location = new System.Drawing.Point(211, 16);
            this.buttonSaveScript.Name = "buttonSaveScript";
            this.buttonSaveScript.Size = new System.Drawing.Size(523, 23);
            this.buttonSaveScript.TabIndex = 2;
            this.buttonSaveScript.Text = "Save Script Changes";
            this.buttonSaveScript.UseVisualStyleBackColor = true;
            this.buttonSaveScript.Click += new System.EventHandler(this.buttonSaveScript_Click);
            // 
            // scriptArrayEditor
            // 
            this.scriptArrayEditor.DisplayItemIndices = true;
            this.scriptArrayEditor.Dock = System.Windows.Forms.DockStyle.Left;
            this.scriptArrayEditor.EnablePropertyView = false;
            this.scriptArrayEditor.EnablePropertyViewDescription = false;
            this.scriptArrayEditor.ItemIndexOffset = 0;
            this.scriptArrayEditor.Location = new System.Drawing.Point(3, 16);
            this.scriptArrayEditor.Name = "scriptArrayEditor";
            this.scriptArrayEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.scriptArrayEditor.Size = new System.Drawing.Size(208, 182);
            this.scriptArrayEditor.TabIndex = 4;
            this.scriptArrayEditor.SelectedObjectChanged += new System.EventHandler(this.scriptArrayEditor_SelectedObjectChanged);
            this.scriptArrayEditor.DoubleClickedNode += new System.EventHandler(this.soundList_DoubleClick);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 220);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(743, 3);
            this.splitter1.TabIndex = 7;
            this.splitter1.TabStop = false;
            // 
            // SoundScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.soundBox);
            this.Name = "SoundScriptEditor";
            this.Size = new System.Drawing.Size(743, 470);
            this.soundBox.ResumeLayout(false);
            this.scriptGroup.ResumeLayout(false);
            this.scriptGroup.PerformLayout();
            this.toolStrip4.ResumeLayout(false);
            this.toolStrip4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox soundBox;
        private System.Windows.Forms.GroupBox scriptGroup;
        private System.Windows.Forms.RichTextBox scriptBox;
        private System.Windows.Forms.ToolStrip toolStrip4;
        private System.Windows.Forms.ToolStripButton copyScriptButton;
        private System.Windows.Forms.ToolStripButton pasteScriptButton;
        private System.Windows.Forms.Button buttonSaveScript;
        private ArrayMemberEditor scriptArrayEditor;
        private System.Windows.Forms.Splitter splitter1;
    }
}
