using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrEditors
{
    public interface IGrEditor
    {
        public void Update();

        public void Render(Camera cam, LiveJObj model, object selected);
    }

}