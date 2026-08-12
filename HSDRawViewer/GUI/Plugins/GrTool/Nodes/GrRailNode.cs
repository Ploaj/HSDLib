using HSDRawViewer.IO.AirRide.DataFormat;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrRailNode : GrNode
    {
        public GrSplineNode Spline1 { get; set; }

        public GrSplineNode Spline2 { get; set; }

        public GrRailNode(KdRail r)
        {
            Spline1 = new GrSplineNode(r.Spline1)
            {
                DisplayColor = new OpenTK.Mathematics.Vector4(0.31372549019f, 0.31372549019f, 0, 1f),
            };
            Spline2 = new GrSplineNode(r.Spline2)
            {
                DisplayColor = new OpenTK.Mathematics.Vector4(0.31372549019f, 0.31372549019f, 0, 1f),
            };

            Nodes.Add(Spline1);
            Nodes.Add(Spline2);
        }
    }
}
