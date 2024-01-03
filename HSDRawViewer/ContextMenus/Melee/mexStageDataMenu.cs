using HSDRaw.Melee.Ef;
using HSDRaw.Melee.Mn;
using HSDRaw.MEX;
using HSDRaw.MEX.Akaneia;
using HSDRaw.MEX.Stages;
using System;
using System.IO;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class MexMapDataMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(MEX_mexMapData) };
        
        public MexMapDataMenu() : base()
        {
            ToolStripMenuItem genPages = new ToolStripMenuItem("Generate SSS Pages");
            genPages.Click += (sender, args) =>
            {
                var path = Path.GetDirectoryName(MainForm.Instance.FilePath);
                var mxdtPath = Path.Combine(path + "/", "MxDt.dat");

                if (!File.Exists(mxdtPath))
                    return;

                var mxdt = new HSDRaw.HSDRawFile(mxdtPath).Roots[0].Data as MEX_Data;

                if (MainForm.SelectedDataNode.Accessor is MEX_mexMapData map_data &&
                    MainForm.Instance.GetSymbol("MnSelectStageDataTable") is SBM_MnSelectStageDataTable stage_table)
                {
                    var f = Tools.FileIO.SaveFile(ApplicationSettings.HSDFileFilter, "sss_pages.dat");
                    if(f != null)
                    {
                        var file = new HSDRaw.HSDRawFile();

                        file.Roots.Add(new HSDRaw.HSDRootNode()
                        {
                            Name = "sss_page",
                            Data = AK_StagePages.GenerateFromMex(mxdt, map_data, stage_table)
                        });

                        file.Save(f);
                    }
                }
            };
            Items.Add(genPages);


            ToolStripMenuItem excom = new ToolStripMenuItem("Extract Common Preview Model");
            excom.Click += (sender, args) =>
            {
                if (MainForm.Instance.GetSymbol("MnSelectStageDataTable") is SBM_MnSelectStageDataTable stage_table)
                {
                    var f = Tools.FileIO.SaveFile(ApplicationSettings.HSDFileFilter, "StagePreviewGrid_scene_models.dat");
                    if (f != null)
                    {
                        var file = new HSDRaw.HSDRawFile();

                        file.Roots.Add(new HSDRaw.HSDRootNode()
                        {
                            Name = "StagePreviewGrid_scene_models",
                            Data = AK_StagePages.ExtractPreviewCommonSceneModels(stage_table)
                        });

                        file.Save(f);
                    }
                }
            };
            Items.Add(excom);
        }
    }
}
