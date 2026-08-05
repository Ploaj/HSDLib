using HSDRawViewer.GUI.Plugins.AirRide.GrTool.Converters;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;
using IONET;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public class GrCategoryZone : GrCategoryNode<KdZone, GrZoneNode>
    {
        public GrCategoryZone(string name, ObservableList<KdZone> list) : base(name, list)
        {
        }

        public override void BuildContextMenu(ContextMenuStrip menu)
        {
            menu.Items.Add("Import From Model", null, (s, e) => {
                var f = FileIO.OpenFile(IOManager.GetImportFileFilter());
                if (f == null) return;

                var scene = IOManager.LoadScene(f, new ImportSettings()
                {
                    Triangulate = true,
                });

                var zone = KdZoneIOConverter.FromIOScene(scene, out string error);

                if (zone != null)
                    list.Add(zone);
                else
                    MessageBox.Show(error, "Collision Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }
    }
}
