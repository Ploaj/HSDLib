using HSDRawViewer.GUI.Plugins.GrTool;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrModelNode : GrDrawNode
    {
        public class Settings
        {
            public bool DisplayBones { get; set; } = false;

            public bool DisplayBoneNames { get; set; } = false;
        }

        public Settings settings = new Settings();

        public GrModelNode() 
        { 
            Text = "Model";
            Checked = true;
            Tag = settings;
        }

        public override bool TryPickNode(PickInformation pick, LiveJObj joint, out float distance)
        {
            distance = float.PositiveInfinity;
            return false;
        }

        public override object PickData(PickInformation pick, LiveJObj joint)
        {
            return null;
        }

        public override void Draw(GrRenderResource render, object selected_object)
        {
            render.RenderModel = Visible;
            render.RenderBoneLabels = settings.DisplayBoneNames;
            render.RenderBones = settings.DisplayBones;
        }

        public override void DrawOverlay(GrRenderResource render, object selected_object)
        {

        }
    }
}
