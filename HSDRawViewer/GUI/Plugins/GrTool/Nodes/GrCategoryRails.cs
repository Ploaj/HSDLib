using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrCategoryRails : GrCategoryNode<KdRail, GrRailNode>
    {
        public GrCategoryRails(string name, ObservableList<KdRail> list) : base(name, list)
        {

        }

        public override void BuildContextMenu(ContextMenuStrip menu, GrNode selectedNode)
        {
            base.BuildContextMenu(menu, selectedNode);

            if (selectedNode != this) return;

            menu.Items.Add("Add New...", null, (s, e) =>
            {
                var rail = new KdRail()
                {
                    NextRail1 = -1,
                    NextRail2 = -1,
                    NextRail3 = -1,
                    PreviousRail = -1,
                    Unused20 = -1,
                    Unused24 = -1,
                    CityRailIndex = -1,
                    Unknown2C = -1,
                    Unknown30 = 0,
                    Unknown30_1 = false,
                    Param00 = 0,
                    ParamFlag = 0,
                    Param08 = 3,
                    ParamAltRail1 = -1,
                    ParamAltRail2 = -1,
                };
                rail.Speed.Add(new KdRail.RailSpeed()
                {
                    Offset = 0,
                    Speed1 = 0,
                    Speed2 = 0.03f,
                });
                rail.StopFriction.Add(new KdRail.RailDataIndex()
                {
                    Offset = 0,
                    Index = -1,
                });
                rail.Material.Add(new KdRail.RailDataIndex()
                {
                    Offset = 0,
                    Index = 26,
                });

                list.Add(rail);
            });
        }

        protected override GrRailNode CreateChild(KdRail m)
        {
            return new GrRailNode(m);
        }
    }
}
