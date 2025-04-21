using HSDRaw.Melee.Gr;
using HSDRawViewer.Converters.Melee;
using HSDRawViewer.Converters.SBM;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class CollDataContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_Coll_Data) };

        public CollDataContextMenu() : base()
        {
            ToolStripMenuItem Export = new("Export As SVG");
            Export.Click += (sender, args) =>
            {
                using SaveFileDialog sd = new();
                sd.Filter = "Scalable Vector Graphics (.svg)|*.svg";

                if (sd.ShowDialog() == DialogResult.OK)
                {
                    Converters.ConvSVG.CollDataToSVG(sd.FileName, MainForm.SelectedDataNode.Accessor as SBM_Coll_Data);
                }
            };
            Items.Add(Export);


            ToolStripMenuItem ImportSSF = new("Import SSF");
            ImportSSF.Click += (sender, args) =>
            {
                using OpenFileDialog sd = new();
                sd.Filter = "Smash Stage File (.ssf)|*.ssf";

                if (sd.ShowDialog() == DialogResult.OK)
                    SSFConverter.ImportCollDataFromSSF(MainForm.SelectedDataNode.Accessor as SBM_Coll_Data, SSF.Open(sd.FileName));
            };
            Items.Add(ImportSSF);

            ToolStripMenuItem ExportSSF = new("Export SSF");
            ExportSSF.Click += (sender, args) =>
            {
                SSFConverter.ExportCollDataToSSF(MainForm.SelectedDataNode.Accessor as SBM_Coll_Data);
            };
            Items.Add(ExportSSF);




            ToolStripMenuItem ImportCOLL = new("Import COLL");
            ImportCOLL.Click += (sender, args) =>
            {
                using OpenFileDialog sd = new();
                sd.Filter = "Brawl collision format (.coll)|*.coll";

                if (sd.ShowDialog() == DialogResult.OK)
                {
                    SBM_Coll_Data c = MainForm.SelectedDataNode.Accessor as SBM_Coll_Data;
                    CollImporter.ImportColl(sd.FileName, c);
                }
            };
            Items.Add(ImportCOLL);
        }
    }
}
