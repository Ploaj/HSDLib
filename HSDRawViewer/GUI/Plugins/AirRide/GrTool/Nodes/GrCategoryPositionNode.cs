using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public class GrCategoryPositionNode : GrCategoryNode<KdPosition, GrPositionNode>
    {
        public GrCategoryPositionNode(string name, ObservableList<KdPosition> list) : base(name, list)
        {
            list.Refresh();
        }
    }
}
