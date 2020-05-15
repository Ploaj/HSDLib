using HSDRaw.Melee.Gr;
using HSDRawViewer.Converters.Melee;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class CollDataContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_Coll_Data) };

        public CollDataContextMenu() : base()
        {
            MenuItem Export = new MenuItem("Export As SVG");
            Export.Click += (sender, args) =>
            {
                using (SaveFileDialog sd = new SaveFileDialog())
                {
                    sd.Filter = "Scalable Vector Graphics (.svg)|*.svg";

                    if (sd.ShowDialog() == DialogResult.OK)
                    {
                        Converters.ConvSVG.CollDataToSVG(sd.FileName, MainForm.SelectedDataNode.Accessor as SBM_Coll_Data);
                    }
                }
            };
            MenuItems.Add(Export);


            MenuItem ImportSSF = new MenuItem("Import From SSF");
            ImportSSF.Click += (sender, args) =>
            {
                using (OpenFileDialog sd = new OpenFileDialog())
                {
                    sd.Filter = "Smash Stage File (.ssf)|*.ssf";

                    if (sd.ShowDialog() == DialogResult.OK)
                    {
                        Converters.Melee.SSFConverter.ImportCollDataFromSSF(MainForm.SelectedDataNode.Accessor as SBM_Coll_Data, SSF.Open(sd.FileName));
                    }
                }
            };
            MenuItems.Add(ImportSSF);
        }
    }
}
