using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using OpenTK.Mathematics;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrRangeSplineNode : GrDrawNode
    {
        public GrSplineNode Spline1 { get; set; }

        public GrSplineNode Spline2 { get; set; }

        private static Vector4 FlightColor = new Vector4(0xEC / (float)0xFF, 0x54 / (float)0xFF, 0x1E / (float)0xFF, 1f);

        private static Vector4 SwitchColor = new Vector4(0, 0x54 / (float)0xFF, 0x1E / (float)0xFF, 1f);

        private static Vector4 LeftColor = new Vector4(1f, 1f, 0, 1f);

        private static Vector4 RightColor = new Vector4(1f, 1f, 0, 1f);

        public GrRangeSplineNode(KdRangeSpline r)
        {
            Spline1 = new GrSplineNode(r.LeftSpline)
            {
                DisplayColor = LeftColor,
            };

            Spline2 = new GrSplineNode(r.RightSpline)
            {
                DisplayColor = RightColor,
            };

            Nodes.Add(Spline1);
            Nodes.Add(Spline2);
        }


        public override bool TryPickNode(PickInformation pick, LiveJObj joint, out float distance)
        {
            distance = float.PositiveInfinity;
            return false;
        }

        public override object PickData(PickInformation pick, LiveJObj joint)
        {
            return null;
        }

        public override void Draw(GrRenderResource render, object selected_object)
        {
            if (Tag is not KdRangeSpline s) return;
            if (s.Flags.HasFlag(KdRangeSplineFlags.FlightPath))
            {
                Spline1.DisplayColor = FlightColor;
                Spline2.DisplayColor = FlightColor;
            }
            else
            if (s.Flags.HasFlag(KdRangeSplineFlags.SwitchPath))
            {
                Spline1.DisplayColor = SwitchColor;
                Spline2.DisplayColor = SwitchColor;
            }
            else
            {
                Spline1.DisplayColor = LeftColor;
                Spline2.DisplayColor = RightColor;
            }
        }

        public override void DrawOverlay(GrRenderResource render, object selected_object)
        {
        }
    }
}
