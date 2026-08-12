using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public class GrCategoryConveyorSpline : GrCategoryNode<KdSpline, GrSplineNode>
    {
        public GrCategoryConveyorSpline(string name, ObservableList<KdSpline> list) : base(name, list)
        {
        }

        protected override GrSplineNode CreateChild(KdSpline m)
        {
            return new GrSplineNode(m);
        }
    }
}
