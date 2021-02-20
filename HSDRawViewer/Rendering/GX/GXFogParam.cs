using System.ComponentModel;
using System.Drawing;

namespace HSDRawViewer.Rendering.GX
{
    public class GXFogParam
    {
        [Category("1. Fog Settings"), DisplayName("Enable Fog"), Description("Enable Fog Rendering")]
        public bool FogEnabled { get => Type == 1; set => Type = value ? 1 : 0; }

        private int Type { get; set; } = 0;

        [Category("1. Fog Settings"), DisplayName("Fog Color"), Description("The color of the fog")]
        public Color Color { get; set; } = Color.White;

        [Category("1. Fog Settings"), DisplayName("Start"), Description("The starting distance of the fog")]
        public float Start { get; set; } = 1000;

        [Category("1. Fog Settings"), DisplayName("End"), Description("The end distance of the fog")]
        public float End { get; set; } = 3000;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shader"></param>
        public void Bind(Shader shader)
        {
            shader.SetInt("fog.type", Type);
            shader.SetFloat("fog.start", Start);
            shader.SetFloat("fog.end", End);
            shader.SetColor("fog.color", Color, Color.A);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fog"></param>
        public void LoadFromHSD(HSDRaw.Common.HSD_FogDesc fog)
        {
            Type = (int)fog.Type;
            Start = fog.Start;
            End = fog.End;
            Color = fog.Color;
        }
    }
}
