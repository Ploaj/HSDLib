using System;
using System.Windows.Forms;
using HSDRaw.Melee.Pl;
using HSDRawViewer.GUI.Dialog;

namespace HSDRawViewer.ContextMenus.Melee
{
    /// <summary>
    /// 
    /// </summary>
    public class FighterDataContextMenu : CommonContextMenu
    {
#if DEBUG

        public class RenameProperty
        {
            public string OldName { get; set; }
            public string NewName { get; set; }
        }

#endif
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_FighterData) };

        public FighterDataContextMenu() : base()
        {
            ToolStripMenuItem addFromFile = new ToolStripMenuItem("Add Article Folder");
            addFromFile.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_FighterData root)
                {
                    if(root.Articles == null)
                        root.Articles = new SBM_ArticlePointer();
                }
            };
            Items.Add(addFromFile);

#if DEBUG
            ToolStripMenuItem renameAnimSymbol = new ToolStripMenuItem("Rename Anim Symbol");
            renameAnimSymbol.Click += (sender, args) =>
            {
                var rn = new RenameProperty();

                using (PropertyDialog d = new PropertyDialog("Rename Symbol", rn))
                {
                    bool inlcudeVictoryAnim = MessageBox.Show("Include Victory Anim Symbols?", "Victory Symbols", MessageBoxButtons.YesNoCancel) == DialogResult.Yes;

                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        if (MainForm.SelectedDataNode.Accessor is SBM_FighterData root)
                        {
                            var sa = root.FighterActionTable.Commands;
                            foreach (var s in sa)
                                if (s.SymbolName != null)
                                    s.SymbolName.Value = s.SymbolName.Value.Replace(rn.OldName, rn.NewName);

                            if (inlcudeVictoryAnim)
                            {
                                var vc = root.DemoActionTable.Commands;

                                foreach (var s in vc)
                                    if (s.SymbolName != null)
                                        s.SymbolName.Value = s.SymbolName.Value.Replace(rn.OldName, rn.NewName);
                            }
                        }
                    }
                }
            };
            Items.Add(renameAnimSymbol);
#endif
        }
    }
}
