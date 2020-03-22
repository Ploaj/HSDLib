using HSDRaw.GX;
using System;
using System.Drawing;

namespace HSDRaw.Common
{
    [Flags]
    public enum RENDER_MODE
    {
        CONSTANT = (1 << 0),
        VERTEX = (1 << 1),
        BOTH = CONSTANT | VERTEX,
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
        DF_ALL = (1 << 28),
        DF_NONE = (1 << 29),
        XLU = (1 << 30),
        USER = (1 << 31),
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

    public enum MATERIAL_ANIM_TYPE
    {
        HSD_A_M_AMBIENT_R = 1,  /* アンビエントカラー */
        HSD_A_M_AMBIENT_G = 2,  /* アンビエントカラー */
        HSD_A_M_AMBIENT_B = 3,  /* アンビエントカラー */
        HSD_A_M_DIFFUSE_R = 4,  /* ディフューズカラー */
        HSD_A_M_DIFFUSE_G = 5,  /* ディフューズカラー */
        HSD_A_M_DIFFUSE_B = 6,  /* ディフューズカラー */
        HSD_A_M_SPECULAR_R = 7,     /* スペキュラカラー */
        HSD_A_M_SPECULAR_G = 8, /* スペキュラカラー */
        HSD_A_M_SPECULAR_B = 9,     /* スペキュラカラー */
        HSD_A_M_ALPHA = 10, /* トランスペアレンシー */
                            /* PEシェーダを使用したアニメーション */
        HSD_A_M_PE_REF0 = 11,   /*  */
        HSD_A_M_PE_REF1 = 12,   /*  */
        HSD_A_M_PE_DSTALPHA = 13,	/*  */
    }

    /// <summary>
    /// Material Object
    /// Contains material rendering information such as textures, colors, and pixel processing
    /// </summary>
    public class HSD_MOBJ : HSDAccessor
    {
        public override int TrimmedSize { get; } = 0x18; // 0x38

        //public uint NameOffset { get; set; }

        public RENDER_MODE RenderFlags { get => (RENDER_MODE)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }

        public HSD_TOBJ Textures { get => _s.GetReference<HSD_TOBJ>(0x08); set => _s.SetReference(0x08, value); }

        public HSD_Material Material { get => _s.GetReference<HSD_Material>(0x0C); set => _s.SetReference(0x0C, value); }

        //public uint UnusedRenderOffset { get; set; }

        public HSD_PEDesc PEDesc { get => _s.GetReference<HSD_PEDesc>(0x14); set => _s.SetReference(0x14, value); }
    }

    /// <summary>
    /// Material Color Object
    /// Contains Ambient, Diffuse, and Specular Color as well as alpha and shinyness of a material
    /// </summary>
    public class HSD_Material : HSDAccessor
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

        public Color DiffuseColor { get => Color.FromArgb(DIF_A, DIF_R, DIF_G, DIF_B); set
            {
                DIF_A = value.A;
                DIF_R = value.R;
                DIF_G = value.G;
                DIF_B = value.B;
            }
        }

        public Color SpecularColor
        {
            get => Color.FromArgb(SPC_A, SPC_R, SPC_G, SPC_B); set
            {
                SPC_A = value.A;
                SPC_R = value.R;
                SPC_G = value.G;
                SPC_B = value.B;
            }
        }

        public Color AmbientColor
        {
            get => Color.FromArgb(AMB_A, AMB_R, AMB_G, AMB_B); set
            {
                AMB_A = value.A;
                AMB_R = value.R;
                AMB_G = value.G;
                AMB_B = value.B;
            }
        }

        public float Alpha { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float Shininess { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
    }

    public class HSD_PEDesc : HSDAccessor
    {
        public override int TrimmedSize { get; } = 0xC;

        public PIXEL_PROCESS_ENABLE Flags { get => (PIXEL_PROCESS_ENABLE)_s.GetByte(0x00); set => _s.SetByte(0x00, (byte)value); }

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
