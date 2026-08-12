using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;
using OpenTK.Mathematics;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrCategorySpline : GrCategoryNode<KdSpline, GrSplineNode>
    {
        public Vector4 SplineColor = Vector4.One;

        public GrCategorySpline(string name, ObservableList<KdSpline> list) : base(name, list)
        {
        }

        protected override GrSplineNode CreateChild(KdSpline m)
        {
            return new GrSplineNode(m)
            {
                DisplayColor = SplineColor,
            };
        }
    }
}
