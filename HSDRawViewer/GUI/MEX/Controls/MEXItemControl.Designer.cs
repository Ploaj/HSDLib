namespace HSDRawViewer.GUI.MEX.Controls
{
    partial class MEXItemControl
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
            this.toolStrip5 = new System.Windows.Forms.ToolStrip();
            this.mexItemCloneButton = new System.Windows.Forms.ToolStripButton();
            this.itemExportButton = new System.Windows.Forms.ToolStripButton();
            this.cpyLogicToClipButton = new System.Windows.Forms.ToolStripButton();
            this.itemTabs = new System.Windows.Forms.TabControl();
            this.tabPageItemCommon = new System.Windows.Forms.TabPage();
            this.commonItemEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.tabPageItemFighter = new System.Windows.Forms.TabPage();
            this.fighterItemEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.tabPageItemPokemon = new System.Windows.Forms.TabPage();
            this.pokemonItemEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.tabPageItemStages = new System.Windows.Forms.TabPage();
            this.stageItemEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.tabPageMexItems = new System.Windows.Forms.TabPage();
            this.mexItemEditor = new HSDRawViewer.GUI.ArrayMemberEditor();
            this.toolStrip5.SuspendLayout();
            this.itemTabs.SuspendLayout();
            this.tabPageItemCommon.SuspendLayout();
            this.tabPageItemFighter.SuspendLayout();
            this.tabPageItemPokemon.SuspendLayout();
            this.tabPageItemStages.SuspendLayout();
            this.tabPageMexItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip5
            // 
            this.toolStrip5.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mexItemCloneButton,
            this.itemExportButton,
            this.cpyLogicToClipButton});
            this.toolStrip5.Location = new System.Drawing.Point(0, 0);
            this.toolStrip5.Name = "toolStrip5";
            this.toolStrip5.Size = new System.Drawing.Size(720, 25);
            this.toolStrip5.TabIndex = 1;
            this.toolStrip5.Text = "toolStrip5";
            // 
            // mexItemCloneButton
            // 
            this.mexItemCloneButton.Image = global::HSDRawViewer.Properties.Resources.ts_clone;
            this.mexItemCloneButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mexItemCloneButton.Name = "mexItemCloneButton";
            this.mexItemCloneButton.Size = new System.Drawing.Size(146, 22);
            this.mexItemCloneButton.Text = "Clone Selected to MEX";
            this.mexItemCloneButton.Click += new System.EventHandler(this.mexItemCloneButton_Click);
            // 
            // itemExportButton
            // 
            this.itemExportButton.Image = global::HSDRawViewer.Properties.Resources.ts_exportfile;
            this.itemExportButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemExportButton.Name = "itemExportButton";
            this.itemExportButton.Size = new System.Drawing.Size(95, 22);
            this.itemExportButton.Text = "Export YAML";
            this.itemExportButton.Click += new System.EventHandler(this.itemExportButton_Click);
            // 
            // cpyLogicToClipButton
            // 
            this.cpyLogicToClipButton.Image = global::HSDRawViewer.Properties.Resources.ts_clone;
            this.cpyLogicToClipButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cpyLogicToClipButton.Name = "cpyLogicToClipButton";
            this.cpyLogicToClipButton.Size = new System.Drawing.Size(156, 22);
            this.cpyLogicToClipButton.Text = "Copy Logic to Clipboard";
            this.cpyLogicToClipButton.Click += new System.EventHandler(this.cpyLogicToClipButton_Click);
            // 
            // itemTabs
            // 
            this.itemTabs.Controls.Add(this.tabPageItemCommon);
            this.itemTabs.Controls.Add(this.tabPageItemFighter);
            this.itemTabs.Controls.Add(this.tabPageItemPokemon);
            this.itemTabs.Controls.Add(this.tabPageItemStages);
            this.itemTabs.Controls.Add(this.tabPageMexItems);
            this.itemTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemTabs.Location = new System.Drawing.Point(0, 25);
            this.itemTabs.Name = "itemTabs";
            this.itemTabs.SelectedIndex = 0;
            this.itemTabs.Size = new System.Drawing.Size(720, 375);
            this.itemTabs.TabIndex = 3;
            // 
            // tabPageItemCommon
            // 
            this.tabPageItemCommon.Controls.Add(this.commonItemEditor);
            this.tabPageItemCommon.Location = new System.Drawing.Point(4, 22);
            this.tabPageItemCommon.Name = "tabPageItemCommon";
            this.tabPageItemCommon.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageItemCommon.Size = new System.Drawing.Size(712, 349);
            this.tabPageItemCommon.TabIndex = 0;
            this.tabPageItemCommon.Text = "Common";
            this.tabPageItemCommon.UseVisualStyleBackColor = true;
            // 
            // commonItemEditor
            // 
            this.commonItemEditor.CanAdd = false;
            this.commonItemEditor.CanClone = false;
            this.commonItemEditor.CanMove = false;
            this.commonItemEditor.CanRemove = false;
            this.commonItemEditor.DisplayItemIndices = true;
            this.commonItemEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commonItemEditor.EnablePropertyViewDescription = true;
            this.commonItemEditor.EnableToolStrip = false;
            this.commonItemEditor.ItemIndexOffset = 0;
            this.commonItemEditor.Location = new System.Drawing.Point(3, 3);
            this.commonItemEditor.Name = "commonItemEditor";
            this.commonItemEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.commonItemEditor.Size = new System.Drawing.Size(706, 343);
            this.commonItemEditor.TabIndex = 1;
            // 
            // tabPageItemFighter
            // 
            this.tabPageItemFighter.Controls.Add(this.fighterItemEditor);
            this.tabPageItemFighter.Location = new System.Drawing.Point(4, 22);
            this.tabPageItemFighter.Name = "tabPageItemFighter";
            this.tabPageItemFighter.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageItemFighter.Size = new System.Drawing.Size(712, 349);
            this.tabPageItemFighter.TabIndex = 1;
            this.tabPageItemFighter.Text = "Fighter";
            this.tabPageItemFighter.UseVisualStyleBackColor = true;
            // 
            // fighterItemEditor
            // 
            this.fighterItemEditor.CanAdd = false;
            this.fighterItemEditor.CanClone = false;
            this.fighterItemEditor.CanMove = false;
            this.fighterItemEditor.CanRemove = false;
            this.fighterItemEditor.DisplayItemIndices = true;
            this.fighterItemEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fighterItemEditor.EnablePropertyViewDescription = true;
            this.fighterItemEditor.EnableToolStrip = false;
            this.fighterItemEditor.ItemIndexOffset = 0;
            this.fighterItemEditor.Location = new System.Drawing.Point(3, 3);
            this.fighterItemEditor.Name = "fighterItemEditor";
            this.fighterItemEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.fighterItemEditor.Size = new System.Drawing.Size(706, 343);
            this.fighterItemEditor.TabIndex = 2;
            // 
            // tabPageItemPokemon
            // 
            this.tabPageItemPokemon.Controls.Add(this.pokemonItemEditor);
            this.tabPageItemPokemon.Location = new System.Drawing.Point(4, 22);
            this.tabPageItemPokemon.Name = "tabPageItemPokemon";
            this.tabPageItemPokemon.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageItemPokemon.Size = new System.Drawing.Size(712, 349);
            this.tabPageItemPokemon.TabIndex = 2;
            this.tabPageItemPokemon.Text = "Pokemon";
            this.tabPageItemPokemon.UseVisualStyleBackColor = true;
            // 
            // pokemonItemEditor
            // 
            this.pokemonItemEditor.CanAdd = false;
            this.pokemonItemEditor.CanClone = false;
            this.pokemonItemEditor.CanMove = false;
            this.pokemonItemEditor.CanRemove = false;
            this.pokemonItemEditor.DisplayItemIndices = true;
            this.pokemonItemEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pokemonItemEditor.EnablePropertyViewDescription = true;
            this.pokemonItemEditor.EnableToolStrip = false;
            this.pokemonItemEditor.ItemIndexOffset = 0;
            this.pokemonItemEditor.Location = new System.Drawing.Point(3, 3);
            this.pokemonItemEditor.Name = "pokemonItemEditor";
            this.pokemonItemEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.pokemonItemEditor.Size = new System.Drawing.Size(706, 343);
            this.pokemonItemEditor.TabIndex = 2;
            // 
            // tabPageItemStages
            // 
            this.tabPageItemStages.Controls.Add(this.stageItemEditor);
            this.tabPageItemStages.Location = new System.Drawing.Point(4, 22);
            this.tabPageItemStages.Name = "tabPageItemStages";
            this.tabPageItemStages.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageItemStages.Size = new System.Drawing.Size(712, 349);
            this.tabPageItemStages.TabIndex = 3;
            this.tabPageItemStages.Text = "Stages";
            this.tabPageItemStages.UseVisualStyleBackColor = true;
            // 
            // stageItemEditor
            // 
            this.stageItemEditor.CanAdd = false;
            this.stageItemEditor.CanClone = false;
            this.stageItemEditor.CanMove = false;
            this.stageItemEditor.CanRemove = false;
            this.stageItemEditor.DisplayItemIndices = true;
            this.stageItemEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stageItemEditor.EnablePropertyViewDescription = true;
            this.stageItemEditor.EnableToolStrip = false;
            this.stageItemEditor.ItemIndexOffset = 0;
            this.stageItemEditor.Location = new System.Drawing.Point(3, 3);
            this.stageItemEditor.Name = "stageItemEditor";
            this.stageItemEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.stageItemEditor.Size = new System.Drawing.Size(706, 343);
            this.stageItemEditor.TabIndex = 2;
            // 
            // tabPageMexItems
            // 
            this.tabPageMexItems.Controls.Add(this.mexItemEditor);
            this.tabPageMexItems.Location = new System.Drawing.Point(4, 22);
            this.tabPageMexItems.Name = "tabPageMexItems";
            this.tabPageMexItems.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMexItems.Size = new System.Drawing.Size(712, 349);
            this.tabPageMexItems.TabIndex = 4;
            this.tabPageMexItems.Text = "MEX Items";
            this.tabPageMexItems.UseVisualStyleBackColor = true;
            // 
            // mexItemEditor
            // 
            this.mexItemEditor.CanMove = false;
            this.mexItemEditor.DisplayItemIndices = true;
            this.mexItemEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mexItemEditor.EnablePropertyViewDescription = true;
            this.mexItemEditor.ItemIndexOffset = 0;
            this.mexItemEditor.Location = new System.Drawing.Point(3, 3);
            this.mexItemEditor.Name = "mexItemEditor";
            this.mexItemEditor.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.mexItemEditor.Size = new System.Drawing.Size(706, 343);
            this.mexItemEditor.TabIndex = 3;
            // 
            // MEXItemControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.itemTabs);
            this.Controls.Add(this.toolStrip5);
            this.Name = "MEXItemControl";
            this.Size = new System.Drawing.Size(720, 400);
            this.toolStrip5.ResumeLayout(false);
            this.toolStrip5.PerformLayout();
            this.itemTabs.ResumeLayout(false);
            this.tabPageItemCommon.ResumeLayout(false);
            this.tabPageItemFighter.ResumeLayout(false);
            this.tabPageItemPokemon.ResumeLayout(false);
            this.tabPageItemStages.ResumeLayout(false);
            this.tabPageMexItems.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip5;
        private System.Windows.Forms.ToolStripButton mexItemCloneButton;
        private System.Windows.Forms.ToolStripButton itemExportButton;
        private System.Windows.Forms.ToolStripButton cpyLogicToClipButton;
        private System.Windows.Forms.TabControl itemTabs;
        private System.Windows.Forms.TabPage tabPageItemCommon;
        private ArrayMemberEditor commonItemEditor;
        private System.Windows.Forms.TabPage tabPageItemFighter;
        private ArrayMemberEditor fighterItemEditor;
        private System.Windows.Forms.TabPage tabPageItemPokemon;
        private ArrayMemberEditor pokemonItemEditor;
        private System.Windows.Forms.TabPage tabPageItemStages;
        private ArrayMemberEditor stageItemEditor;
        private System.Windows.Forms.TabPage tabPageMexItems;
        private ArrayMemberEditor mexItemEditor;
    }
}
