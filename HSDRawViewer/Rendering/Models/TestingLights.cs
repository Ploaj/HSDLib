using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.Rendering.Models
{
    internal class TestingLights
    {
        public static RenderLObj[] Trophy1 { get; } = new RenderLObj[]
        {
            new RenderLObj()
            {
                Enabled = true,
                Type = HSDRaw.Common.LObjType.AMBIENT,
                _color = new OpenTK.Mathematics.Vector4(192, 192, 192, 255) / 255f,
            },
            new RenderLObj()
            {
                Enabled = true,
                Type = HSDRaw.Common.LObjType.INFINITE,
                _color = new OpenTK.Mathematics.Vector4(179, 179, 179, 255) / 255f,
                _position = new OpenTK.Mathematics.Vector3(7.5f, 12, 9),
            },
            new RenderLObj()
            {
                Enabled = true,
                Type = HSDRaw.Common.LObjType.INFINITE,
                _color = new OpenTK.Mathematics.Vector4(38, 38, 38, 255) / 255f,
                _position = new OpenTK.Mathematics.Vector3(-2, -12, 2),
            },
            new RenderLObj()
            {
                Enabled = true,
                Type = HSDRaw.Common.LObjType.INFINITE,
                _color = new OpenTK.Mathematics.Vector4(76, 76, 76, 255) / 255f,
                _position = new OpenTK.Mathematics.Vector3(-7.5f, 3, -9),
            },
        };
    }
}
