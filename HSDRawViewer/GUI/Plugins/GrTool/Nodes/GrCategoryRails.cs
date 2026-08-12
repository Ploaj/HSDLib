using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrCategoryRails : GrCategoryNode<KdRail, GrRailNode>
    {
        public GrCategoryRails(string name, ObservableList<KdRail> list) : base(name, list)
        {

        }

        protected override GrRailNode CreateChild(KdRail m)
        {
            return new GrRailNode(m);
        }
    }
}
