
namespace HSDRawViewer.GUI.Plugins.Melee
{
    partial class NewCollDataEditor
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.lineArrayEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cbGroupList = new System.Windows.Forms.ToolStripComboBox();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.cbGroupList});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // lineArrayEditor
            // 
            this.lineArrayEditor.DisplayItemImages = false;
            this.lineArrayEditor.DisplayItemIndices = false;
            this.lineArrayEditor.Dock = System.Windows.Forms.DockStyle.Left;
            this.lineArrayEditor.EnablePropertyViewDescription = true;
            this.lineArrayEditor.ImageHeight = ((ushort)(24));
            this.lineArrayEditor.ImageWidth = ((ushort)(24));
            this.lineArrayEditor.ItemHeight = 13;
            this.lineArrayEditor.ItemIndexOffset = 0;
            this.lineArrayEditor.Location = new System.Drawing.Point(0, 25);
            this.lineArrayEditor.Name = "lineArrayEditor";
            this.lineArrayEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.lineArrayEditor.Size = new System.Drawing.Size(262, 425);
            this.lineArrayEditor.TabIndex = 1;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(46, 22);
            this.toolStripLabel1.Text = "Group: ";
            // 
            // cbGroupList
            // 
            this.cbGroupList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGroupList.Name = "cbGroupList";
            this.cbGroupList.Size = new System.Drawing.Size(121, 25);
            // 
            // NewCollDataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lineArrayEditor);
            this.Controls.Add(this.toolStrip1);
            this.Name = "NewCollDataEditor";
            this.TabText = "Collision Data Editor";
            this.Text = "Collision Data Editor";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private ArrayMemberEditor lineArrayEditor;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cbGroupList;
    }
}