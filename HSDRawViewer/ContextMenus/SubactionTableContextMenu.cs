using HSDRaw;
using HSDRaw.Melee.Pl;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class SubactionTableContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_SubActionTable) };

        public SubactionTableContextMenu() : base()
        {
            MenuItem Export = new MenuItem("Import Subaction Data From File");
            Export.Click += (sender, args) =>
            {
                var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

                if(f != null && MainForm.SelectedDataNode.Accessor is SBM_SubActionTable table)
                {
                    var dataToImport = new SBM_SubActionTable();

                    dataToImport._s = new HSDRawFile(f).Roots[0].Data._s;

                    if(dataToImport.Count == table.Count)
                    {
                        var importTable = dataToImport.Subactions;
                        var newTable = table.Subactions;
                        for (int i = 0; i < table.Count; i++)
                            newTable[i].SubAction = importTable[i].SubAction;
                        table.Subactions = newTable;
                    }
                }
            };
            MenuItems.Add(Export);
            
        }
    }
}
