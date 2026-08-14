using HSDRawViewer.GUI.Plugins.GrTool.Converters;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;
using IONET;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrCategoryCollision : GrCategoryNode<KdMesh, GrCollisionNode>
    {
        public GrCategoryCollision(string name, ObservableList<KdMesh> list) : base(name, list)
        {
        }

        public override void BuildContextMenu(ContextMenuStrip menu, GrNode selected_node)
        {
            if (selected_node != this) return;

            menu.Items.Add("Import New Collision...", null, (s, e) => {
                var f = FileIO.OpenFile(IOManager.GetImportFileFilter());
                if (f == null) return;

                var scene = IOManager.LoadScene(f, new ImportSettings()
                {
                    Triangulate = true,
                });

                var m = KdMeshIOConverter.FromIOScene(scene);

                list.Add(m);
            });

            menu.Items.Add("Export All...", null, (s, e) => {
                var f = FileIO.SaveFile(IOManager.GetExportFileFilter());
                if (f == null) return;
                IOManager.ExportScene(KdMeshIOConverter.ToIOScene(list), f, new ExportSettings());
            });
        }
    }
}
