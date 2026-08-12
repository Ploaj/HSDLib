using HSDRawViewer.GUI.Plugins.GrTool;
using HSDRawViewer.IO.AirRide.DataFormat;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrAnimationNode : GrNode
    {
        public override void OnSelect(GrRenderResource _render, GrDataResource _data)
        {
            if (Tag is not KdAnimation anim) return;

            _render.LoadRailAnimation(anim.Animation);
        }
    }
}
