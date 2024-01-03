using HSDRaw;
using HSDRaw.MEX.Akaneia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus.Melee
{
    public class Stage_IDs : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public int count { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public HSDUShortArray array { get => _s.GetReference<HSDUShortArray>(0x04); set => _s.SetReference(0x04, value); }
    }

    public class AkaneiaPagesContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(AK_StagePages) };

        public AkaneiaPagesContextMenu() : base()
        {
            ToolStripMenuItem genPages = new ToolStripMenuItem("Generate SSS Pages");
            genPages.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is AK_StagePages pages)
                {
                    var f = Tools.FileIO.SaveFile(ApplicationSettings.HSDFileFilter, "MxPtSSSRn.dat");
                    if (f != null)
                    {
                        var file = System.IO.File.Exists(f) ? new HSDRaw.HSDRawFile(f) : new HSDRaw.HSDRawFile();

                        var ids = pages.Pages.Array.SelectMany(e => e.Icons.Array.Select(e => (ushort)e.ExternalID)).Distinct().Where(e => e != 0).ToArray();

                        Stage_IDs data = new Stage_IDs();
                        data.count = ids.Length;
                        data.array = new HSDUShortArray();
                        data.array.Array = ids;

                        var stage_id_node = file.Roots.Find(e => e.Name.Equals("stage_ids"));

                        if (stage_id_node == null)
                        {
                            stage_id_node = new HSDRaw.HSDRootNode()
                            {
                                Name = "stage_ids"
                            };
                            file.Roots.Add(stage_id_node);
                        }
                        stage_id_node.Data = data;

                        file.Save(f);
                    }
                }
            };
            Items.Add(genPages);
        }
    }
}
