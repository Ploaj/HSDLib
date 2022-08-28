namespace HSDRawViewer.GUI.Plugins.SubactionEditor
{
    partial class ScriptEditorActionList
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
            this.actionArrayEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.subroutineArrayEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.referencePanel = new System.Windows.Forms.Panel();
            this.buttonGoto = new System.Windows.Forms.Button();
            this.referenceLabel = new System.Windows.Forms.Label();
            this.cbReference = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.referencePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // actionArrayEditor
            // 
            this.actionArrayEditor.AskBeforeDelete = true;
            this.actionArrayEditor.DisplayItemImages = false;
            this.actionArrayEditor.DisplayItemIndices = true;
            this.actionArrayEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionArrayEditor.EnablePropertyViewDescription = true;
            this.actionArrayEditor.ImageHeight = ((ushort)(24));
            this.actionArrayEditor.ImageWidth = ((ushort)(24));
            this.actionArrayEditor.InsertCloneAfterSelected = false;
            this.actionArrayEditor.ItemHeight = 13;
            this.actionArrayEditor.ItemIndexOffset = 0;
            this.actionArrayEditor.Location = new System.Drawing.Point(3, 3);
            this.actionArrayEditor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.actionArrayEditor.Name = "actionArrayEditor";
            this.actionArrayEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.actionArrayEditor.Size = new System.Drawing.Size(275, 440);
            this.actionArrayEditor.TabIndex = 0;
            this.actionArrayEditor.SelectedObjectChanged += new System.EventHandler(this.actionArrayEditor_SelectedObjectChanged);
            this.actionArrayEditor.OnItemAdded += new HSDRawViewer.GUI.ArrayMemberEditor.OnItemAddedHandler(this.actionArrayEditor_OnItemAdded);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 32);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(289, 474);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.actionArrayEditor);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(281, 446);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Actions";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.subroutineArrayEditor);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(281, 446);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Subroutines";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // subroutineArrayEditor
            // 
            this.subroutineArrayEditor.CanMove = false;
            this.subroutineArrayEditor.DisplayItemImages = false;
            this.subroutineArrayEditor.DisplayItemIndices = true;
            this.subroutineArrayEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subroutineArrayEditor.EnablePropertyView = false;
            this.subroutineArrayEditor.EnablePropertyViewDescription = true;
            this.subroutineArrayEditor.ImageHeight = ((ushort)(24));
            this.subroutineArrayEditor.ImageWidth = ((ushort)(24));
            this.subroutineArrayEditor.InsertCloneAfterSelected = false;
            this.subroutineArrayEditor.ItemHeight = 13;
            this.subroutineArrayEditor.ItemIndexOffset = 0;
            this.subroutineArrayEditor.Location = new System.Drawing.Point(3, 3);
            this.subroutineArrayEditor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.subroutineArrayEditor.Name = "subroutineArrayEditor";
            this.subroutineArrayEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.subroutineArrayEditor.Size = new System.Drawing.Size(275, 440);
            this.subroutineArrayEditor.TabIndex = 0;
            this.subroutineArrayEditor.SelectedObjectChanged += new System.EventHandler(this.subroutineArrayEditor_SelectedObjectChanged);
            this.subroutineArrayEditor.OnItemRemove += new HSDRawViewer.GUI.ArrayMemberEditor.OnItemRemoveHandler(this.subroutineArrayEditor_OnItemRemove);
            // 
            // referencePanel
            // 
            this.referencePanel.AutoSize = true;
            this.referencePanel.Controls.Add(this.buttonGoto);
            this.referencePanel.Controls.Add(this.referenceLabel);
            this.referencePanel.Controls.Add(this.cbReference);
            this.referencePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.referencePanel.Location = new System.Drawing.Point(0, 0);
            this.referencePanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.referencePanel.Name = "referencePanel";
            this.referencePanel.Size = new System.Drawing.Size(289, 32);
            this.referencePanel.TabIndex = 7;
            // 
            // buttonGoto
            // 
            this.buttonGoto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGoto.Location = new System.Drawing.Point(210, 6);
            this.buttonGoto.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonGoto.Name = "buttonGoto";
            this.buttonGoto.Size = new System.Drawing.Size(74, 23);
            this.buttonGoto.TabIndex = 5;
            this.buttonGoto.Text = "Goto";
            this.buttonGoto.UseVisualStyleBackColor = true;
            this.buttonGoto.Click += new System.EventHandler(this.buttonGoto_Click);
            // 
            // referenceLabel
            // 
            this.referenceLabel.AutoSize = true;
            this.referenceLabel.Location = new System.Drawing.Point(4, 9);
            this.referenceLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.referenceLabel.Name = "referenceLabel";
            this.referenceLabel.Size = new System.Drawing.Size(85, 15);
            this.referenceLabel.TabIndex = 3;
            this.referenceLabel.Text = "Referenced By:";
            // 
            // cbReference
            // 
            this.cbReference.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbReference.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbReference.FormattingEnabled = true;
            this.cbReference.Location = new System.Drawing.Point(97, 6);
            this.cbReference.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbReference.Name = "cbReference";
            this.cbReference.Size = new System.Drawing.Size(105, 23);
            this.cbReference.TabIndex = 4;
            // 
            // ScriptEditorActionList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 506);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.referencePanel);
            this.Name = "ScriptEditorActionList";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.referencePanel.ResumeLayout(false);
            this.referencePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ArrayMemberEditor actionArrayEditor;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private ArrayMemberEditor subroutineArrayEditor;
        private System.Windows.Forms.Panel referencePanel;
        private System.Windows.Forms.Button buttonGoto;
        private System.Windows.Forms.Label referenceLabel;
        private System.Windows.Forms.ComboBox cbReference;
    }
}
