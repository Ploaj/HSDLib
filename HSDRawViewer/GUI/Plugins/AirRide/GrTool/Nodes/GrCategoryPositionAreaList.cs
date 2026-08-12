using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;
using OpenTK.Mathematics;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public class GrCategoryPositionAreaList : GrCategoryNode<KdPositionAreaList, GrCategoryPositionArea>
    {
        public Vector3 DisplayColor = Vector3.One;

        public GrCategoryPositionAreaList(string name, ObservableList<KdPositionAreaList> list) : base(name, list)
        {
        }

        protected override GrCategoryPositionArea CreateChild(KdPositionAreaList m)
        {
            return new GrCategoryPositionArea("", m.Positions);
        }

        public override void BuildContextMenu(ContextMenuStrip menu)
        {
            menu.Items.Add("New Position Area List", null, (s, e) => {
                list.Add(new KdPositionAreaList()
                {

                });
            });
        }
    }
}
