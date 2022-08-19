namespace HSDRawViewer.GUI.Controls.JObjEditor
{
    partial class JObjEditorNew
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JObjEditorNew));
            this.dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.importModelFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportModelToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.importSceneSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSceneSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.importToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.trimToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.fSMApplyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.displaySettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fogSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.showSelectionOutlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showBonesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showBoneOrientationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSplinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.renderModeBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.viewModeBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dockPanel1
            // 
            this.dockPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel1.Location = new System.Drawing.Point(0, 25);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Size = new System.Drawing.Size(719, 496);
            this.dockPanel1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton2,
            this.toolStripDropDownButton1,
            this.renderDropDown,
            this.toolStripLabel2,
            this.renderModeBox,
            this.toolStripLabel3,
            this.viewModeBox});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(719, 25);
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importModelFromFileToolStripMenuItem,
            this.exportModelToFileToolStripMenuItem,
            this.toolStripSeparator2,
            this.importSceneSettingsToolStripMenuItem,
            this.exportSceneSettingsToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(38, 22);
            this.toolStripDropDownButton2.Text = "File";
            // 
            // importModelFromFileToolStripMenuItem
            // 
            this.importModelFromFileToolStripMenuItem.Name = "importModelFromFileToolStripMenuItem";
            this.importModelFromFileToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.importModelFromFileToolStripMenuItem.Text = "Import Model From File";
            this.importModelFromFileToolStripMenuItem.Click += new System.EventHandler(this.importModelFromFileToolStripMenuItem_Click);
            // 
            // exportModelToFileToolStripMenuItem
            // 
            this.exportModelToFileToolStripMenuItem.Name = "exportModelToFileToolStripMenuItem";
            this.exportModelToFileToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.exportModelToFileToolStripMenuItem.Text = "Export Model To File";
            this.exportModelToFileToolStripMenuItem.Click += new System.EventHandler(this.exportModelToFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(196, 6);
            // 
            // importSceneSettingsToolStripMenuItem
            // 
            this.importSceneSettingsToolStripMenuItem.Name = "importSceneSettingsToolStripMenuItem";
            this.importSceneSettingsToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.importSceneSettingsToolStripMenuItem.Text = "Import Scene Settings";
            this.importSceneSettingsToolStripMenuItem.Click += new System.EventHandler(this.importSceneSettingsToolStripMenuItem_Click);
            // 
            // exportSceneSettingsToolStripMenuItem
            // 
            this.exportSceneSettingsToolStripMenuItem.Name = "exportSceneSettingsToolStripMenuItem";
            this.exportSceneSettingsToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.exportSceneSettingsToolStripMenuItem.Text = "Export Scene Settings";
            this.exportSceneSettingsToolStripMenuItem.Click += new System.EventHandler(this.exportSceneSettingsToolStripMenuItem_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem1,
            this.toolStripMenuItem1,
            this.toolStripSeparator9,
            this.createToolStripMenuItem,
            this.editToolStripMenuItem1,
            this.trimToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.toolStripSeparator7,
            this.fSMApplyToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(76, 22);
            this.toolStripDropDownButton1.Text = "Animation";
            // 
            // importToolStripMenuItem1
            // 
            this.importToolStripMenuItem1.Name = "importToolStripMenuItem1";
            this.importToolStripMenuItem1.Size = new System.Drawing.Size(131, 22);
            this.importToolStripMenuItem1.Text = "Import";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(131, 22);
            this.toolStripMenuItem1.Text = "Export";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItem2.Text = "As Maya ANIM";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItem3.Text = "As Figatree";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItem4.Text = "As AnimJoint";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItem5.Text = "As MOT";
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItem6.Text = "As XML";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(128, 6);
            // 
            // createToolStripMenuItem
            // 
            this.createToolStripMenuItem.Name = "createToolStripMenuItem";
            this.createToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.createToolStripMenuItem.Text = "Create";
            // 
            // editToolStripMenuItem1
            // 
            this.editToolStripMenuItem1.Name = "editToolStripMenuItem1";
            this.editToolStripMenuItem1.Size = new System.Drawing.Size(131, 22);
            this.editToolStripMenuItem1.Text = "Edit";
            // 
            // trimToolStripMenuItem
            // 
            this.trimToolStripMenuItem.Name = "trimToolStripMenuItem";
            this.trimToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.trimToolStripMenuItem.Text = "Trim";
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(128, 6);
            // 
            // fSMApplyToolStripMenuItem
            // 
            this.fSMApplyToolStripMenuItem.Name = "fSMApplyToolStripMenuItem";
            this.fSMApplyToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.fSMApplyToolStripMenuItem.Text = "FSM Apply";
            // 
            // renderDropDown
            // 
            this.renderDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.renderDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displaySettingsToolStripMenuItem,
            this.fogSettingsToolStripMenuItem,
            this.toolStripSeparator4,
            this.showSelectionOutlineToolStripMenuItem,
            this.showBonesToolStripMenuItem,
            this.showBoneOrientationToolStripMenuItem,
            this.showSplinesToolStripMenuItem});
            this.renderDropDown.Image = ((System.Drawing.Image)(resources.GetObject("renderDropDown.Image")));
            this.renderDropDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.renderDropDown.Name = "renderDropDown";
            this.renderDropDown.Size = new System.Drawing.Size(74, 22);
            this.renderDropDown.Text = "Rendering";
            // 
            // displaySettingsToolStripMenuItem
            // 
            this.displaySettingsToolStripMenuItem.Name = "displaySettingsToolStripMenuItem";
            this.displaySettingsToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.displaySettingsToolStripMenuItem.Text = "Light Settings";
            this.displaySettingsToolStripMenuItem.Click += new System.EventHandler(this.displaySettingsToolStripMenuItem_Click);
            // 
            // fogSettingsToolStripMenuItem
            // 
            this.fogSettingsToolStripMenuItem.Name = "fogSettingsToolStripMenuItem";
            this.fogSettingsToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.fogSettingsToolStripMenuItem.Text = "Fog Settings";
            this.fogSettingsToolStripMenuItem.Click += new System.EventHandler(this.fogSettingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(193, 6);
            // 
            // showSelectionOutlineToolStripMenuItem
            // 
            this.showSelectionOutlineToolStripMenuItem.Checked = true;
            this.showSelectionOutlineToolStripMenuItem.CheckOnClick = true;
            this.showSelectionOutlineToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showSelectionOutlineToolStripMenuItem.Name = "showSelectionOutlineToolStripMenuItem";
            this.showSelectionOutlineToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showSelectionOutlineToolStripMenuItem.Text = "Outline Selected Mesh";
            // 
            // showBonesToolStripMenuItem
            // 
            this.showBonesToolStripMenuItem.CheckOnClick = true;
            this.showBonesToolStripMenuItem.Name = "showBonesToolStripMenuItem";
            this.showBonesToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showBonesToolStripMenuItem.Text = "Show Bones";
            // 
            // showBoneOrientationToolStripMenuItem
            // 
            this.showBoneOrientationToolStripMenuItem.CheckOnClick = true;
            this.showBoneOrientationToolStripMenuItem.Name = "showBoneOrientationToolStripMenuItem";
            this.showBoneOrientationToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showBoneOrientationToolStripMenuItem.Text = "Show Bone Orientation";
            // 
            // showSplinesToolStripMenuItem
            // 
            this.showSplinesToolStripMenuItem.CheckOnClick = true;
            this.showSplinesToolStripMenuItem.Name = "showSplinesToolStripMenuItem";
            this.showSplinesToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.showSplinesToolStripMenuItem.Text = "Show Splines";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(81, 22);
            this.toolStripLabel2.Text = "Render Mode:";
            // 
            // renderModeBox
            // 
            this.renderModeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.renderModeBox.Name = "renderModeBox";
            this.renderModeBox.Size = new System.Drawing.Size(140, 25);
            this.renderModeBox.SelectedIndexChanged += new System.EventHandler(this.renderModeBox_SelectedIndexChanged);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(69, 22);
            this.toolStripLabel3.Text = "View Mode:";
            // 
            // viewModeBox
            // 
            this.viewModeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.viewModeBox.Items.AddRange(new object[] {
            "All",
            "Selected",
            "None"});
            this.viewModeBox.Name = "viewModeBox";
            this.viewModeBox.Size = new System.Drawing.Size(140, 25);
            this.viewModeBox.SelectedIndexChanged += new System.EventHandler(this.viewModeBox_SelectedIndexChanged);
            // 
            // JObjEditorNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dockPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "JObjEditorNew";
            this.Size = new System.Drawing.Size(719, 521);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem importModelFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportModelToFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem importSceneSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportSceneSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem trimToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem fSMApplyToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton renderDropDown;
        private System.Windows.Forms.ToolStripMenuItem displaySettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fogSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem showSelectionOutlineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showBonesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showBoneOrientationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSplinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox renderModeBox;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox viewModeBox;
    }
}
