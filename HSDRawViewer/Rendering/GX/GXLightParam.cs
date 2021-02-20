using System.ComponentModel;
using System.Drawing;
using YamlDotNet.Serialization;

namespace HSDRawViewer.Rendering.GX
{
    public class GXLightParam
    {
        [Category("1. Lighting Settings"), DisplayName("Use Camera Light"), Description("When true makes the light source emit from the camera's location")]
        public bool UseCameraLight { get; set; } = true;

        [Category("1. Lighting Settings"), DisplayName("Light X"), Description("X position of light in world when camera light is disabled")]
        public float LightX { get; set; } = 0;

        [Category("1. Lighting Settings"), DisplayName("Light Y"), Description("Y position of light in world when camera light is disabled")]
        public float LightY { get; set; } = 10;

        [Category("1. Lighting Settings"), DisplayName("Light Z"), Description("Z position of light in world when camera light is disabled")]
        public float LightZ { get; set; } = 50;

        [Category("2. Lighting Color"), DisplayName("Ambient Intensity"), Description("The intensity of the ambient lighting")]
        public float AmbientPower { get; set; } = 0.5f;

        [Category("2. Lighting Color"), DisplayName("Ambient Color"), Description("The color of the ambient light")]
        [YamlIgnore]
        public Color AmbientColor { get; set; } = Color.White;

        [Browsable(false)]
        public byte AmbientR { get => AmbientColor.R; set => AmbientColor = Color.FromArgb(AmbientColor.A, value, AmbientColor.G, AmbientColor.B); }
        [Browsable(false)]
        public byte AmbientG { get => AmbientColor.G; set => AmbientColor = Color.FromArgb(AmbientColor.A, AmbientColor.R, value, AmbientColor.B); }
        [Browsable(false)]
        public byte AmbientB { get => AmbientColor.B; set => AmbientColor = Color.FromArgb(AmbientColor.A, AmbientColor.R, AmbientColor.G, value); }

        [Category("2. Lighting Color"), DisplayName("Diffuse Intensity"), Description("The intensity of the diffuse lighting")]
        public float DiffusePower { get; set; } = 1;

        [Category("2. Lighting Color"), DisplayName("Diffuse Color"), Description("The color of the diffuse light")]
        [YamlIgnore]
        public Color DiffuseColor { get; set; } = Color.White;

        [Browsable(false)]
        public byte DiffuseR { get => DiffuseColor.R; set => DiffuseColor = Color.FromArgb(DiffuseColor.A, value, DiffuseColor.G, DiffuseColor.B); }
        [Browsable(false)]
        public byte DiffuseG { get => DiffuseColor.G; set => DiffuseColor = Color.FromArgb(DiffuseColor.A, DiffuseColor.R, value, DiffuseColor.B); }
        [Browsable(false)]
        public byte DiffuseB { get => DiffuseColor.B; set => DiffuseColor = Color.FromArgb(DiffuseColor.A, DiffuseColor.R, DiffuseColor.G, value); }

        [Category("3. Enhancements"), DisplayName("Use Per Pixel Lighting"), Description("Calculates lighting per pixel for a smoother look. Set to false for gamecube style.")]
        public bool UsePerPixelLighting { get; set; } = true;

        [Category("3. Enhancements"), DisplayName("Adjust Saturation"), Description("")]
        public float Saturation { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shader"></param>
        public void Bind(Shader shader)
        {
            shader.SetFloat("saturate", Saturation);

            shader.SetBoolToInt("perPixelLighting", UsePerPixelLighting);

            shader.SetBoolToInt("light.useCamera", UseCameraLight);

            shader.SetVector3("light.position", LightX, LightY, LightZ);

            shader.SetColor("light.ambient", AmbientColor, 1);
            shader.SetFloat("light.ambientPower", AmbientPower);

            shader.SetColor("light.diffuse", DiffuseColor, 1);
            shader.SetFloat("light.diffusePower", DiffusePower);

            //shader.SetColor("light.specular", settings.SpecularColor, 1);
            //shader.SetFloat("light.specularPower", settings.SpecularPower);
        }
    }
}
