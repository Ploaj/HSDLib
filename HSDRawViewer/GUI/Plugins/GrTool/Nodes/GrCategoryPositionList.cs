using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;
using OpenTK.Mathematics;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrCategoryPositionList : GrCategoryNode<KdPositionList, GrCategoryPosition>
    {
        public Vector3 DisplayColorX = Vector3.UnitX;

        public Vector3 DisplayColorY = Vector3.UnitY;

        public Vector3 DisplayColorZ = Vector3.UnitZ;

        public GrCategoryPositionList(string name, ObservableList<KdPositionList> list) : base(name, list)
        {
        }

        protected override GrCategoryPosition CreateChild(KdPositionList m)
        {
            return new GrCategoryPosition("", m.Positions);
        }

        public override void BuildContextMenu(ContextMenuStrip menu, GrNode selected_node)
        {
            base.BuildContextMenu(menu, selected_node);

            if (selected_node != this) return;

            menu.Items.Add("New Position List", null, (s, e) => {
                list.Add(new KdPositionList()
                {

                });
            });
        }
    }
}
