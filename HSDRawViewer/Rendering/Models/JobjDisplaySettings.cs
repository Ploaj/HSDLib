using System.Collections.Generic;
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
    public enum LightRenderMode
    {
        Camera,
        Default,
        Custom,
    }

    /// <summary>
    /// 
    /// </summary>
    public class JobjDisplaySettings
    {
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

        [Category("1. Display"), DisplayName("Render Custom Lights"), Description("")]
        public bool RenderCustomLightPositions { get; set; } = false;


        [Category("2. Lighting"), DisplayName("Light Source"), Description("")]
        public LightRenderMode LightSource { get; set; } = LightRenderMode.Camera;

        [Browsable(false)]
        public RenderLObj[] _lights { get; internal set; }

        [Category("2. Lighting"), DisplayName("Custom Light 0"), Description(""), TypeConverter(typeof(ExpandableObjectConverter)), YamlDotNet.Serialization.YamlIgnore]
        public RenderLObj Light0 { get => _lights[0]; set => _lights[0] = value; }

        [Category("2. Lighting"), DisplayName("Custom Light 1"), Description(""), TypeConverter(typeof(ExpandableObjectConverter)), YamlDotNet.Serialization.YamlIgnore]
        public RenderLObj Light1 { get => _lights[1]; set => _lights[1] = value; }

        [Category("2. Lighting"), DisplayName("Custom Light 2"), Description(""), TypeConverter(typeof(ExpandableObjectConverter)), YamlDotNet.Serialization.YamlIgnore]
        public RenderLObj Light2 { get => _lights[2]; set => _lights[2] = value; }

        [Category("2. Lighting"), DisplayName("Custom Light 3"), Description(""), TypeConverter(typeof(ExpandableObjectConverter)), YamlDotNet.Serialization.YamlIgnore]
        public RenderLObj Light3 { get => _lights[3]; set => _lights[3] = value; }

        public JobjDisplaySettings()
        {
            _lights = new RenderLObj[]
            {
                new RenderLObj(),
                new RenderLObj(),
                new RenderLObj(),
                new RenderLObj(),
            };
        }
    }
}
