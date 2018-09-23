using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSDLib.GX;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace HALSysDATViewer.Rendering
{
    public class GXTranslator
    {

        public static PrimitiveType toPrimitiveType(GXPrimitiveType pt)
        {
            switch (pt)
            {
                case GXPrimitiveType.Quads: return PrimitiveType.Quads;
                case GXPrimitiveType.Triangles: return PrimitiveType.Triangles;
                case GXPrimitiveType.TriangleStrip: return PrimitiveType.TriangleStrip;
                case GXPrimitiveType.TriangleFan: return PrimitiveType.TriangleFan;
                case GXPrimitiveType.Lines: return PrimitiveType.Lines;
                case GXPrimitiveType.LineStrip: return PrimitiveType.LineStrip;
                case GXPrimitiveType.Points: return PrimitiveType.Points;
                default:
                    return PrimitiveType.Triangles;
            }
        }

        public static TextureWrapMode toWrapMode(GXWrapMode wm)
        {
            switch (wm)
            {
                case GXWrapMode.CLAMP: return TextureWrapMode.Clamp;
                case GXWrapMode.MIRROR: return TextureWrapMode.MirroredRepeat;
                case GXWrapMode.REPEAT: return TextureWrapMode.Repeat;
            }
            return TextureWrapMode.Repeat;
        }

    }
}
