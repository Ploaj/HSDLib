using HSDRaw.Common.Animation;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;
using System.ComponentModel;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrCategoryAnimation : GrCategoryNode<KdAnimation, GrAnimationNode>
    {
        public GrCategoryAnimation(string name, ObservableList<KdAnimation> list) : base(name, list)
        {
        }
    }
}
