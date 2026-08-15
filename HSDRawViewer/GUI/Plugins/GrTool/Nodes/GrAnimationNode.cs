using HSDRaw.Common.Animation;
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


        public void SetAnimJoint(HSD_AnimJoint j)
        {
            if (Tag is not KdAnimation anim) return;

            anim.Animation = j;
        }
    }
}
