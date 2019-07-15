using HSDRaw.GX;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace HSDRawViewer.Rendering
{
    /// <summary>
    /// Translates GX enums to opengl equivalent 
    /// </summary>
    public class GXTranslator
    {
        public static Vector4 toVector4(GXColor4 v)
        {
            return new Vector4(v.R, v.G, v.B, v.A);
        }

        public static Vector4 toVector4(GXVector4 v)
        {
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static Vector2 toVector2(GXVector2 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Vector3 toVector3(GXVector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static TextureMagFilter toMagFilter(GXTexFilter filter)
        {
            switch (filter)
            {
                case GXTexFilter.GX_NEAR:
                    return TextureMagFilter.Nearest;
                case GXTexFilter.GX_LINEAR:
                    return TextureMagFilter.Linear;
                default:
                    return TextureMagFilter.Nearest;
            }
        }

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
                case GXWrapMode.CLAMP: return TextureWrapMode.ClampToEdge;
                case GXWrapMode.MIRROR: return TextureWrapMode.MirroredRepeat;
                case GXWrapMode.REPEAT: return TextureWrapMode.Repeat;
            }
            return TextureWrapMode.Repeat;
        }

        public static AlphaFunction toAlphaFunction(GXCompareType type)
        {
            switch (type)
            {
                case GXCompareType.Always: return AlphaFunction.Always;
                case GXCompareType.Equal: return AlphaFunction.Equal;
                case GXCompareType.GEqual: return AlphaFunction.Gequal;
                case GXCompareType.Greater: return AlphaFunction.Greater;
                case GXCompareType.LEqual: return AlphaFunction.Lequal;
                case GXCompareType.Less: return AlphaFunction.Less;
                case GXCompareType.NEqual: return AlphaFunction.Notequal;
                case GXCompareType.Never: return AlphaFunction.Never;
                default:
                    return AlphaFunction.Never;
            }
        }

        public static BlendEquationMode toBlendMode(GXBlendMode mode)
        {
            switch (mode)
            {
                case GXBlendMode.GX_BLEND: return BlendEquationMode.FuncAdd;
                case GXBlendMode.GX_LOGIC: return BlendEquationMode.FuncSubtract;
                case GXBlendMode.GX_SUBTRACT: return BlendEquationMode.FuncSubtract;
                default:
                    return BlendEquationMode.FuncAdd;
            }
        }

        public static BlendingFactor toBlendingFactor(GXBlendFactor factor)
        {
            switch (factor)
            {
                case GXBlendFactor.GX_BL_ONE: return BlendingFactor.One;
                case GXBlendFactor.GX_BL_SRCALPHA: return BlendingFactor.SrcAlpha;
                case GXBlendFactor.GX_BL_SRCCLR: return BlendingFactor.SrcColor;
                case GXBlendFactor.GX_BL_ZERO: return BlendingFactor.Zero;
                case GXBlendFactor.GX_BL_INVSRCCLR: return BlendingFactor.OneMinusSrcColor;
                case GXBlendFactor.GX_BL_INVSRCALPHA: return BlendingFactor.OneMinusSrcAlpha;
                case GXBlendFactor.GX_BL_INVDSTALPHA: return BlendingFactor.OneMinusDstAlpha;
                default:
                    return BlendingFactor.SrcAlpha;
            }
        }
    }
}
