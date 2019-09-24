using HSDRaw.GX;
using System;

namespace HSDRaw.Common
{
    [Flags]
    public enum RENDER_MODE
    {
        DIFFUSE_MAT0 = (0 << 0),
        DIFFSE_MAT = (1 << 0),
        DIFFSE_VTX = (2 << 0),
        DIFFSE_BOTH = (3 << 0),
        DIFFUSE = (1 << 2),
        SPECULAR = (1 << 3),
        TEX0 = (1 << 4),
        TEX1 = (1 << 5),
        TEX2 = (1 << 6),
        TEX3 = (1 << 7),
        TEX4 = (1 << 8),
        TEX5 = (1 << 9),
        TEX6 = (1 << 10),
        TEX7 = (1 << 11),
        TOON = (1 << 12),
        ALPHA_COMPAT = (0 << 13),
        ALPHA_MAT = (1 << 13),
        ALPHA_VTX = (2 << 13),
        ALPHA_BOTH = (3 << 13),
        SHADOW = (1 << 26),
        ZMODE_ALWAYS = (1 << 27),
        DF_NONE = (1 << 29),
        XLU = (1 << 30),
        USER = (1 << 31)
    }

    [Flags]
    public enum PIXEL_PROCESS_ENABLE
    {
        COLOR_UPDATE = (1 << 0),
        ALPHA_UPDATE = (1 << 1),
        DST_ALPHA = (1 << 2),
        BEFORE_TEX = (1 << 3),
        COMPARE = (1 << 4),
        ZUPDATE = (1 << 5),
        DITHER = (1 << 6)
    }

    /// <summary>
    /// Material Object
    /// Contains material rendering information such as textures, colors, and pixel processing
    /// </summary>
    public class HSD_MOBJ : HSDAccessor
    {
        public override int TrimmedSize { get; } = 0x18;

        //public uint NameOffset { get; set; }

        public RENDER_MODE RenderFlags { get => (RENDER_MODE)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }

        public HSD_TOBJ Textures { get => _s.GetReference<HSD_TOBJ>(0x08); set => _s.SetReference(0x08, value); }

        public HSD_MCOBJ MaterialColor { get => _s.GetReference<HSD_MCOBJ>(0x0C); set => _s.SetReference(0x0C, value); }

        //public uint UnusedRenderOffset { get; set; }

        public HSD_PixelProcessing PixelProcessing { get => _s.GetReference<HSD_PixelProcessing>(0x14); set => _s.SetReference(0x14, value); }
    }

    /// <summary>
    /// Material Color Object
    /// Contains Ambient, Diffuse, and Specular Color as well as alpha and shinyness of a material
    /// </summary>
    public class HSD_MCOBJ : HSDAccessor
    {
        public override int TrimmedSize { get; } = 0x14;

        public byte AMB_R { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }
        public byte AMB_G { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }
        public byte AMB_B { get => _s.GetByte(0x02); set => _s.SetByte(0x02, value); }
        public byte AMB_A { get => _s.GetByte(0x03); set => _s.SetByte(0x03, value); }
        
        public byte DIF_R { get => _s.GetByte(0x04); set => _s.SetByte(0x04, value); }
        public byte DIF_G { get => _s.GetByte(0x05); set => _s.SetByte(0x05, value); }
        public byte DIF_B { get => _s.GetByte(0x06); set => _s.SetByte(0x06, value); }
        public byte DIF_A { get => _s.GetByte(0x07); set => _s.SetByte(0x07, value); }
        
        public byte SPC_R { get => _s.GetByte(0x08); set => _s.SetByte(0x08, value); }
        public byte SPC_G { get => _s.GetByte(0x09); set => _s.SetByte(0x09, value); }
        public byte SPC_B { get => _s.GetByte(0x0A); set => _s.SetByte(0x0A, value); }
        public byte SPC_A { get => _s.GetByte(0x0B); set => _s.SetByte(0x0B, value); }

        public uint AmbientColorABGR { get => (uint)_s.GetInt32(0x00); set => _s.SetInt32(0x00, (int)value); }
        public uint DiffuseColorABGR { get => (uint)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }
        public uint SpecularColorABGR { get => (uint)_s.GetInt32(0x08); set => _s.SetInt32(0x08, (int)value); }
        
        public float Alpha { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float Shininess { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
    }

    public class HSD_PixelProcessing : HSDAccessor
    {
        public override int TrimmedSize { get; } = 0xC;

        public byte Flags { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public byte AlphaRef0 { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }

        public byte AlphaRef1 { get => _s.GetByte(0x02); set => _s.SetByte(0x02, value); }

        public byte DestinationAlpha { get => _s.GetByte(0x03); set => _s.SetByte(0x03, value); }

        public GXBlendMode BlendMode { get => (GXBlendMode)_s.GetByte(0x04); set => _s.SetByte(0x04, (byte)value); }

        public GXBlendFactor SrcFactor { get => (GXBlendFactor)_s.GetByte(0x05); set => _s.SetByte(0x05, (byte)value); }

        public GXBlendFactor DstFactor { get => (GXBlendFactor)_s.GetByte(0x06); set => _s.SetByte(0x06, (byte)value); }

        public GXLogicOp BlendOp { get => (GXLogicOp)_s.GetByte(0x07); set => _s.SetByte(0x07, (byte)value); }

        public GXCompareType DepthFunction { get => (GXCompareType)_s.GetByte(0x08); set => _s.SetByte(0x08, (byte)value); }

        public GXCompareType AlphaComp0 { get => (GXCompareType)_s.GetByte(0x09); set => _s.SetByte(0x09, (byte)value); }

        public GXAlphaOp AlphaOp { get => (GXAlphaOp)_s.GetByte(0x0A); set => _s.SetByte(0x0A, (byte)value); }

        public GXCompareType AlphaComp1 { get => (GXCompareType)_s.GetByte(0x0B); set => _s.SetByte(0x0B, (byte)value); }
    }
}
