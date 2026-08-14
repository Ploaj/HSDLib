using HSDRawViewer.GUI.Plugins.GrTool.Converters;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;
using IONET;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrCategoryZone : GrCategoryNode<KdZone, GrZoneNode>
    {
        public GrCategoryZone(string name, ObservableList<KdZone> list) : base(name, list)
        {
        }

        public override void BuildContextMenu(ContextMenuStrip menu, GrNode selected_node)
        {
            if (selected_node != this) return;

            menu.Items.Add("Add New", null, (s, e) => {
                list.Add(KdZoneIOConverter.CreateBlankSize(40f));
            });

            menu.Items.Add(new ToolStripSeparator());

            menu.Items.Add("Import From Model", null, (s, e) => {
                var f = FileIO.OpenFile(IOManager.GetImportFileFilter());
                if (f == null) return;

                var scene = IOManager.LoadScene(f, new ImportSettings()
                {
                    Triangulate = true,
                });

                if (KdZoneIOConverter.FromIOScene(scene, out KdZone zone, out string error))
                {
                    list.Add(zone);
                }
                else
                {
                    MessageBox.Show(
                        error, 
                        "Collision Import Error", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                }
            });
        }
    }
}
