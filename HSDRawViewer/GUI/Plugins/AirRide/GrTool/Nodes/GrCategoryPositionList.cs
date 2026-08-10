using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;
using OpenTK.Mathematics;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public class GrCategoryPositionList : GrCategoryNode<KdPositionList, GrCategoryPositionNode>
    {
        public Vector3 DisplayColorX = Vector3.UnitX;

        public Vector3 DisplayColorY = Vector3.UnitY;

        public Vector3 DisplayColorZ = Vector3.UnitZ;

        public GrCategoryPositionList(string name, ObservableList<KdPositionList> list) : base(name, list)
        {
        }

        protected override GrCategoryPositionNode CreateChild(KdPositionList m)
        {
            return new GrCategoryPositionNode("", m.Positions);
        }
    }
}
