using HSDRaw;
using HSDRaw.Melee.Pl;
using HSDRawViewer.GUI;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
#if DEBUG
    public class SubactionTableRename
    {
        public string Name { get; set; }
    }
#endif
    public class SubactionTableContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_FighterActionTable) };

        public SubactionTableContextMenu() : base()
        {
            MenuItem Export = new MenuItem("Import Subaction Data From File");
            Export.Click += (sender, args) =>
            {
                var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

                if(f != null && MainForm.SelectedDataNode.Accessor is SBM_FighterActionTable table)
                {
                    var dataToImport = new SBM_FighterActionTable();

                    dataToImport._s = new HSDRawFile(f).Roots[0].Data._s;

                    if(dataToImport.Count == table.Count)
                    {
                        var importTable = dataToImport.Commands;
                        var newTable = table.Commands;
                        for (int i = 0; i < table.Count; i++)
                            newTable[i].SubAction = importTable[i].SubAction;
                        table.Commands = newTable;
                    }
                }
            };
            MenuItems.Add(Export);

#if DEBUG

            MenuItem rename = new MenuItem("Rename Symbols");
            rename.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_FighterActionTable table)
                {
                    var prop = new SubactionTableRename();
                    using (PropertyDialog d = new PropertyDialog("Fighter Symbol Rename", prop))
                    {
                        if (d.ShowDialog() == DialogResult.OK)
                        {
                            var tables = table.Commands;

                            foreach (var c in tables)
                            {
                                if (c.SymbolName != null && !string.IsNullOrEmpty(c.SymbolName.Value))
                                {
                                    var sym = c.SymbolName.Value;

                                    var newsym = System.Text.RegularExpressions.Regex.Replace(sym, @"(?=Ply)(.)*(?=5K)", prop.Name); ;

                                    c.SymbolName.Value = newsym;
                                }
                            }

                            table.Commands = tables;
                        }
                    }
                }
            };
            MenuItems.Add(rename);
#endif

        }
    }
}
