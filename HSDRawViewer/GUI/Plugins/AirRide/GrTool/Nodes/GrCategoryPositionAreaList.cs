using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;
using OpenTK.Mathematics;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public class GrCategoryPositionAreaList : GrCategoryNode<KdPositionAreaList, GrCategoryPositionAreaNode>
    {
        public Vector3 DisplayColor = Vector3.One;

        public GrCategoryPositionAreaList(string name, ObservableList<KdPositionAreaList> list) : base(name, list)
        {
        }

        protected override GrCategoryPositionAreaNode CreateChild(KdPositionAreaList m)
        {
            return new GrCategoryPositionAreaNode("", m.Positions);
        }
    }
}
