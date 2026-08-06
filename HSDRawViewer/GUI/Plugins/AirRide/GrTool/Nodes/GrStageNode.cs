namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public class GrStageNode : GrNode
    {
        public override bool HasTransform => false;

        public GrStageNode()
        {
            Text = "Stage";
            Checked = true;
        }
    }
}
