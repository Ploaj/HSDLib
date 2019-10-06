using HSDRaw.Melee.Gr;
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

                    if(sd.ShowDialog() == DialogResult.OK)
                    {
                        Converters.ConvSVG.CollDataToSVG(sd.FileName, MainForm.SelectedDataNode.Accessor as SBM_Coll_Data);
                    }
                }
            };
            MenuItems.Add(Export);
        }
    }
}
