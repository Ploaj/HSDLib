using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using MeleeLib.GCX;

namespace Modlee
{
    public class GXtoGL
    {
        public static float GetGLWrapMode(GXWrapMode i)
        {
            switch (i)
            {
                case GXWrapMode.CLAMP: return (float)TextureWrapMode.Clamp;
                case GXWrapMode.REPEAT: return (float)TextureWrapMode.Repeat;
                case GXWrapMode.MIRROR: return (float)TextureWrapMode.MirroredRepeat;
                default: return (float)TextureWrapMode.Clamp;
            }
        }
    }
}
