using HSDRawViewer.Rendering.Models;
using OpenTK.Mathematics;
namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public interface IGrRotate
    {
        public bool CanRotate(object selected_object);

        public Matrix4 GetRotation(object selected_object, LiveJObj joint);

        public void SetRotation(object selected_object, LiveJObj joint, Quaternion value);
    }
}
