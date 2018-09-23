using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSDLib.GX;

namespace HSDLib.Common
{
    [Flags]
    public enum RENDER_MODE
    {
        DIFFUSE_MAT0 = (0 << 0),
        DIFFSE_MAT = (1 << 0),
        DIFFSE_VTX = (2 << 0),
        DIFFSE_BOTH = (3 << 0),
        DIFFUSE = (1<<2),
        SPECULAR = (1<<3),
        TEX0 = (1<<4),
        TEX1 = (1<<5),
        TEX2 = (1 << 6),
        TEX3 = (1 << 7),
        TEX4 = (1 << 8),
        TEX5 = (1 << 9),
        TEX6 = (1 << 10),
        TEX7 = (1 << 11),
        TOON = (1 << 12),
        ALPHA_COMPAT = (0<<13),
        ALPHA_MAT = (1<<13),
        ALPHA_VTX = (2<<13),
        ALPHA_BOTH = (3<<13),
        SHADOW = (1<<26),
        ZMODE_ALWAYS = (1<<27),
        DF_NONE = (1<<29),
        XLU = (1<<30),
        USER = (1<<31)
    }

    [Flags]
    public enum PIXEL_PROCESS_ENABLE
    {
        COLOR_UPDATE = (1<<0),
        ALPHA_UPDATE = (1<<1),
        DST_ALPHA = (1<<2),
        BEFORE_TEX = (1<<3),
        COMPARE = (1<<4),
        ZUPDATE = (1<<5),
        DITHER = (1<<6)
    }

    public class HSD_MOBJ : IHSDNode
    {
        [FieldData(typeof(uint))]
        public uint NameOffset { get; set; }

        [FieldData(typeof(RENDER_MODE))]
        public RENDER_MODE RenderFlags { get; set; }

        [FieldData(typeof(HSD_TOBJ))]
        public HSD_TOBJ Textures { get; set; }

        [FieldData(typeof(HSD_MCOBJ))]
        public HSD_MCOBJ MaterialColor { get; set; }

        [FieldData(typeof(uint))]
        public uint UnusedRenderOffset { get; set; }

        [FieldData(typeof(HSD_PixelProcessing))]
        public HSD_PixelProcessing PixelProcessing { get; set; }
    }

    public class HSD_MCOBJ : IHSDNode
    {
        public GXColor Ambient;
        public GXColor Diffuse;
        public GXColor Specular;
        public float Alpha;
        public float Shininess;

        public override void Open(HSDReader Reader)
        {
            Ambient = Reader.ReadType<GXColor>(new GXColor());
            Diffuse = Reader.ReadType<GXColor>(new GXColor());
            Specular = Reader.ReadType<GXColor>(new GXColor());
            Alpha = Reader.ReadSingle();
            Shininess = Reader.ReadSingle();
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(this);
            Writer.Write(Ambient.R); Writer.Write(Ambient.G); Writer.Write(Ambient.B); Writer.Write(Ambient.A);
            Writer.Write(Diffuse.R); Writer.Write(Diffuse.G); Writer.Write(Diffuse.B); Writer.Write(Diffuse.A);
            Writer.Write(Specular.R); Writer.Write(Specular.G); Writer.Write(Specular.B); Writer.Write(Specular.A);
            Writer.Write(Alpha);
            Writer.Write(Shininess);
        }
    }

    public struct GXColor
    {
        public byte R, G, B, A;
    }

    public class HSD_PixelProcessing : IHSDNode
    {
        [FieldData(typeof(byte))]
        public byte Flags { get; set; }
        [FieldData(typeof(byte))]
        public byte AlphaRef0 { get; set; }
        [FieldData(typeof(byte))]
        public byte AlphaRef1 { get; set; }
        [FieldData(typeof(byte))]
        public byte DestinationAlpha { get; set; }
        [FieldData(typeof(byte))]
        public GXBlendMode BlendMode { get; set; }
        [FieldData(typeof(byte))]
        public GXBlendFactor SrcFactor { get; set; }
        [FieldData(typeof(byte))]
        public GXBlendFactor DstFactor { get; set; }
        [FieldData(typeof(byte))]
        public GXLogicOp BlendOp { get; set; }
        [FieldData(typeof(byte))]
        public GXCompareType DepthFunction { get; set; }
        [FieldData(typeof(byte))]
        public GXCompareType AlphaComp0 { get; set; }
        [FieldData(typeof(byte))]
        public GXAlphaOp AlphaOp { get; set; }
        [FieldData(typeof(byte))]
        public GXCompareType AlphaComp1 { get; set; }
    }
}
