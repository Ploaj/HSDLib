using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public abstract class GrDrawNode : GrNode
    {
        public abstract bool TryPickNode(PickInformation pick, LiveJObj joint, out float distance);

        public abstract object PickData(PickInformation pick, LiveJObj joint);

        public abstract void Draw(GrRenderResource render, object selected_object);

        public abstract void DrawOverlay(GrRenderResource render, object selected_object);
    }
}
