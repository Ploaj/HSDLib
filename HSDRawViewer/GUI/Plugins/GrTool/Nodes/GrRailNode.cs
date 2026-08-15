using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrRailNode : GrNode
    {
        public GrSplineNode Spline1 { get; set; }

        public GrSplineNode Spline2 { get; set; }

        public GrAnimationNode Animation { get; set; }

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
            Animation = new GrAnimationNode()
            {
                Text = "Animation",
                Tag = r.Animation,
            };

            Nodes.Add(Spline1);
            Nodes.Add(Spline2);
            Nodes.Add(Animation);
        }

        public class AnimationGenSettings
        {
            public int FrameCount { get; set; } = 299;
        }

        public override void BuildContextMenu(ContextMenuStrip menu, GrNode selected_node)
        {
            base.BuildContextMenu(menu, selected_node);

            if (selected_node != this) return;

            menu.Items.Add("Generate Animation", null, (s, e) =>
            {
                var settings = new AnimationGenSettings();
                using (var p = new PropertyDialog("Spline Animation Settings", settings))
                {
                    if (p.ShowDialog() != DialogResult.OK)
                        return;

                    var anim = SplineTools.GenerateAnimJoint(Spline1.GetSpline(), settings.FrameCount);
                    Animation.SetAnimJoint(anim);
                    TreeView.SelectedNode = this;
                }
            });
        }
    }
}
