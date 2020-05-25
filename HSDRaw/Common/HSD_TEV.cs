using HSDRaw.GX;
using System;
using System.Drawing;

namespace HSDRaw.Common
{
    public enum TOBJ_TEV_CC
    {
        GX_CC_TEXC = 8,
        GX_CC_TEXA = 9,
        GX_CC_ONE = 12,
        GX_CC_HALF = 13,
        GX_CC_ZERO = 15,
        KONST_RGB = (0x01 << 7 | 0),
        KONST_RRR = (0x01 << 7 | 1),
        KONST_GGG = (0x01 << 7 | 2),
        KONST_BBB = (0x01 << 7 | 3),
        KONST_AAA = (0x01 << 7 | 4),
        TEX0_RGB = (0x01 << 7 | 5),
        TEX0_AAA = (0x01 << 7 | 6),
        TEX1_RGB = (0x01 << 7 | 7),
        TEX1_AAA = (0x01 << 7 | 8)
    }

    public enum TOBJ_TEV_CA
    {
        GX_CC_TEXC = 8,
        GX_CC_TEXA = 9,
        GX_CC_ONE = 12,
        GX_CC_HALF = 13,
        GX_CC_ZERO = 15,
        KONST_R = (0x01 << 6 | 0),
        KONST_G = (0x01 << 6 | 1),
        KONST_B = (0x01 << 6 | 2),
        KONST_A = (0x01 << 6 | 3),
        TEX0_A = (0x01 << 6 | 4),
        TEX1_A = (0x01 << 6 | 5)
    }

    [Flags]
    public enum TOBJ_TEVREG_ACTIVE
    {
        KONST_R = (0x01 << 0),
        KONST_G = (0x01 << 1),
        KONST_B = (0x01 << 2),
        KONST_A = (0x01 << 3),
        KONST =
            (KONST_R | KONST_G
                | KONST_B | KONST_A),
        TEV0_R = (0x01 << 4),
        TEV0_G = (0x01 << 5),
        TEV0_B = (0x01 << 6),
        TEV0_A = (0x01 << 7),
        TEV0 =
            (TEV0_R | TEV0_G
                | TEV0_B | TEV0_A),
        TEV1_R = (0x01 << 8),
        TEV1_G = (0x01 << 9),
        TEV1_B = (0x01 << 10),
        TEV1_A = (0x01 << 11),
        TEV1 =
            (TEV1_R | TEV1_G
                | TEV1_B | TEV1_A),
        COLOR_TEV = (0x01 << 30),
        ALPHA_TEV = (0x01 << 31)
    }

    public class HSD_TOBJ_TEV : HSDAccessor
    {
        public override int TrimmedSize { get; } = 0x20;

        public TevColorOp color_op { get => (TevColorOp)_s.GetByte(0x00); set => _s.SetByte(0x00, (byte)value); }

        public TevAlphaOp alpha_op { get => (TevAlphaOp)_s.GetByte(0x01); set => _s.SetByte(0x01, (byte)value); }

        public TevBias color_bias { get => (TevBias)_s.GetByte(0x02); set => _s.SetByte(0x02, (byte)value); }

        public TevBias alpha_bias { get => (TevBias)_s.GetByte(0x03); set => _s.SetByte(0x03, (byte)value); }

        public TevScale color_scale { get => (TevScale)_s.GetByte(0x04); set => _s.SetByte(0x04, (byte)value); }

        public TevScale alpha_scale { get => (TevScale)_s.GetByte(0x05); set => _s.SetByte(0x05, (byte)value); }

        public bool color_clamp { get => _s.GetByte(0x06) == 1; set => _s.SetByte(0x06, (byte)(value ? 1 : 0)); }

        public bool alpha_clamp { get => _s.GetByte(0x07) == 1; set => _s.SetByte(0x07, (byte)(value ? 1 : 0)); }

        private byte color_a { get => _s.GetByte(0x08); set => _s.SetByte(0x08, value); }

        private byte color_b { get => _s.GetByte(0x09); set => _s.SetByte(0x09, value); }

        private byte color_c { get => _s.GetByte(0x0A); set => _s.SetByte(0x0A, value); }

        private byte color_d { get => _s.GetByte(0x0B); set => _s.SetByte(0x0B, value); }

        private byte alpha_a { get => _s.GetByte(0x0C); set => _s.SetByte(0x0C, value); }

        private byte alpha_b { get => _s.GetByte(0x0D); set => _s.SetByte(0x0D, value); }

        private byte alpha_c { get => _s.GetByte(0x0E); set => _s.SetByte(0x0E, value); }

        private byte alpha_d { get => _s.GetByte(0x0F); set => _s.SetByte(0x0F, value); }
        
        public TOBJ_TEV_CC color_a_in { get => (TOBJ_TEV_CC)color_a; set => color_a = (byte)value; }
        public TOBJ_TEV_CC color_b_in { get => (TOBJ_TEV_CC)color_b; set => color_b = (byte)value; }
        public TOBJ_TEV_CC color_c_in { get => (TOBJ_TEV_CC)color_c; set => color_c = (byte)value; }
        public TOBJ_TEV_CC color_d_in { get => (TOBJ_TEV_CC)color_d; set => color_d = (byte)value; }

        public TOBJ_TEV_CA alpha_a_in { get => (TOBJ_TEV_CA)alpha_a; set => alpha_a = (byte)value; }
        public TOBJ_TEV_CA alpha_b_in { get => (TOBJ_TEV_CA)alpha_b; set => alpha_b = (byte)value; }
        public TOBJ_TEV_CA alpha_c_in { get => (TOBJ_TEV_CA)alpha_c; set => alpha_c = (byte)value; }
        public TOBJ_TEV_CA alpha_d_in { get => (TOBJ_TEV_CA)alpha_d; set => alpha_d = (byte)value; }

        public byte constantAlpha { get => _s.GetByte(0x13); set => _s.SetByte(0x13, value); }
        public Color constant { get => _s.GetColorRGB(0x10); set => _s.SetColorRGB(0x10, value); }

        public byte tev0Alpha { get => _s.GetByte(0x17); set => _s.SetByte(0x17, value); }
        public Color tev0 { get => _s.GetColorRGB(0x14); set => _s.SetColorRGB(0x14, value); }

        public byte tev1Alpha { get => _s.GetByte(0x1B); set => _s.SetByte(0x1B, value); }
        public Color tev1 { get => _s.GetColorRGB(0x18); set => _s.SetColorRGB(0x18, value); }
        
        public TOBJ_TEVREG_ACTIVE active { get => (TOBJ_TEVREG_ACTIVE)_s.GetInt32(0x1C); set => _s.SetInt32(0x1C, (int)value); }
    }
}
