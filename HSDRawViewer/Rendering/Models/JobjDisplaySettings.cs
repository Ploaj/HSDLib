using System.ComponentModel;

namespace HSDRawViewer.Rendering.Models
{
    public enum ObjectRenderMode
    {
        All,
        Visible,
        Selected,
        None,
    }

    /// <summary>
    /// 
    /// </summary>
    public class JobjDisplaySettings
    {
        [Category("1. Display"), DisplayName("Use Camera Light"), Description("Casts directional light from camera position.")]
        public bool UseCameraLight { get; set; } = true;

        [Category("1. Display"), DisplayName("Object Render Mode"), Description("")]
        public ObjectRenderMode RenderObjects { get; set; } = ObjectRenderMode.Visible;

        [Category("1. Display"), DisplayName("Show Bones"), Description("")]
        public bool RenderBones { get; set; } = true;

        [Category("1. Display"), DisplayName("Show Bone Orientation"), Description("")]
        public bool RenderOrientation { get; set; } = false;

        [Category("1. Display"), DisplayName("Outline Selected Object"), Description("")]
        public bool OutlineSelected { get; set; } = true;

        [Category("1. Display"), DisplayName("Render Splines"), Description("")]
        public bool RenderSplines { get; set; } = false;
    }
}
