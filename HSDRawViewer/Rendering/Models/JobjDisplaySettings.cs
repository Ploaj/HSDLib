using System.ComponentModel;

namespace HSDRawViewer.Rendering.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class JobjDisplaySettings
    {
        [Category("1. Display"), DisplayName("Show Bones"), Description("")]
        public bool RenderBones { get; set; } = true;

        [Category("1. Display"), DisplayName("Show Bone Orientation"), Description("")]
        public bool RenderOrientation { get; set; } = false;

        [Category("1. Display"), DisplayName("Show Objects"), Description("")]
        public bool RenderObjects { get; set; } = true;

        [Category("1. Display"), DisplayName("Outline Selected Object"), Description("")]
        public bool OutlineSelected { get; set; } = true;

        [Category("1. Display"), DisplayName("Render Splines"), Description("")]
        public bool RenderSplines { get; set; } = false;
    }
}
