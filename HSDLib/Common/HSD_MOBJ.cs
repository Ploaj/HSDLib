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
        [FieldData(typeof(byte))]
        public byte AMB_R { get; set; }
        [FieldData(typeof(byte))]
        public byte AMB_G { get; set; }
        [FieldData(typeof(byte))]
        public byte AMB_B { get; set; }
        [FieldData(typeof(byte))]
        public byte AMB_A { get; set; }

        [FieldData(typeof(byte))]
        public byte DIF_R { get; set; }
        [FieldData(typeof(byte))]
        public byte DIF_G { get; set; }
        [FieldData(typeof(byte))]
        public byte DIF_B { get; set; }
        [FieldData(typeof(byte))]
        public byte DIF_A { get; set; }

        [FieldData(typeof(byte))]
        public byte SPC_R { get; set; }
        [FieldData(typeof(byte))]
        public byte SPC_G { get; set; }
        [FieldData(typeof(byte))]
        public byte SPC_B { get; set; }
        [FieldData(typeof(byte))]
        public byte SPC_A { get; set; }

        [FieldData(typeof(float))]
        public float Alpha { get; set; }

        [FieldData(typeof(float))]
        public float Shininess { get; set; }
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
