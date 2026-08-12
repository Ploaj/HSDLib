using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrCategoryRangeSpline : GrCategoryNode<KdRangeSpline, GrRangeSplineNode>
    {
        public GrCategoryRangeSpline(string name, ObservableList<KdRangeSpline> list) : base(name, list)
        {
        }

        protected override GrRangeSplineNode CreateChild(KdRangeSpline m)
        {
            return new GrRangeSplineNode(m);
        }
    }
}
