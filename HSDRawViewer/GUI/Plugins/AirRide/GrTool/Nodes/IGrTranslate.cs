using OpenTK.Mathematics;
namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public interface IGrTranslate
    {
        public bool CanTranslate(object selected_object);

        public Vector3 GetTranslate(object selected_object);

        public void SetTranslate(object selected_object, Vector3 value);
    }
}
