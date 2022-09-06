using HSDRaw.Common;
using HSDRawViewer.Rendering.Shaders;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.Rendering.Models
{
    public class RenderLObj
    {
        public bool Enabled { get; set; } = false;

        public LObjType Type { get; set; } = LObjType.AMBIENT;

        public Vector3 Position = Vector3.Zero;

        public Vector4 Color = Vector4.One;

        // public bool Attenuate = false;

        public void Bind(GXShader shader, int index)
        {
            // common
            shader.SetBoolToInt($"light[{index}].enabled", Enabled);
            shader.SetInt($"light[{index}].type", (int)Type);
            shader.SetVector3($"light[{index}].position", Position);
            shader.SetVector3($"light[{index}].color", Color.Xyz);
        }
    }
}
