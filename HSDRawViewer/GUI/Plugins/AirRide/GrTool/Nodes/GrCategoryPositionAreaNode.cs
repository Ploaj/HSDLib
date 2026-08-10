using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public class GrCategoryPositionAreaNode : GrCategoryNode<KdPositionArea, GrPositionAreaNode>
    {
        public GrCategoryPositionAreaNode(string name, ObservableList<KdPositionArea> list) : base(name, list)
        {
            list.Refresh();
        }
    }
}
