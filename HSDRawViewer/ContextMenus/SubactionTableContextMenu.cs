using HSDRaw;
using HSDRaw.Melee.Pl;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
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
            
        }
    }
}
