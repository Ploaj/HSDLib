using HSDRaw.GX;
using System.Drawing;

namespace HSDRaw.Common
{
    public class HSD_TOBJ_TEV : HSDAccessor
    {
        public override int TrimmedSize { get; } = 0x20;

        public TevColorOp color_op { get => (TevColorOp)_s.GetByte(0x00); set => _s.SetByte(0x00, (byte)value); }

        public TevAlphaOp alpha_op { get => (TevAlphaOp)_s.GetByte(0x01); set => _s.SetByte(0x01, (byte)value); }

        public TevBias color_bias { get => (TevBias)_s.GetByte(0x02); set => _s.SetByte(0x02, (byte)value); }

        public TevBias alpha_bias { get => (TevBias)_s.GetByte(0x03); set => _s.SetByte(0x03, (byte)value); }

        public TevScale color_scale { get => (TevScale)_s.GetByte(0x04); set => _s.SetByte(0x04, (byte)value); }

        public TevScale alpha_scale { get => (TevScale)_s.GetByte(0x05); set => _s.SetByte(0x05, (byte)value); }

        public byte color_clamp { get => _s.GetByte(0x06); set => _s.SetByte(0x06, value); }

        public byte color_a { get => _s.GetByte(0x07); set => _s.SetByte(0x07, value); }

        public byte color_b { get => _s.GetByte(0x08); set => _s.SetByte(0x08, value); }

        public byte color_c { get => _s.GetByte(0x09); set => _s.SetByte(0x09, value); }

        public byte color_d { get => _s.GetByte(0x0A); set => _s.SetByte(0x0A, value); }

        public byte alpha_a { get => _s.GetByte(0x0B); set => _s.SetByte(0x0B, value); }

        public byte alpha_b { get => _s.GetByte(0x0C); set => _s.SetByte(0x0C, value); }

        public byte alpha_c { get => _s.GetByte(0x0D); set => _s.SetByte(0x0D, value); }

        public byte alpha_d { get => _s.GetByte(0x0E); set => _s.SetByte(0x0E, value); }

        public byte ConstantAlpha { get => _s.GetByte(0x13); set => _s.SetByte(0x13, value); }
        public Color ConstantColor { get => _s.GetColorRGB(0x10); set => _s.SetColorRGB(0x10, value); }

        public byte tev0Alpha { get => _s.GetByte(0x17); set => _s.SetByte(0x17, value); }
        public Color tev0 { get => _s.GetColorRGB(0x14); set => _s.SetColorRGB(0x14, value); }

        public byte tev1Alpha { get => _s.GetByte(0x1B); set => _s.SetByte(0x1B, value); }
        public Color tev1 { get => _s.GetColorRGB(0x18); set => _s.SetColorRGB(0x18, value); }

        public int active { get => _s.GetInt32(0x1C); set => _s.SetInt32(0x1C, value); }
    }
}
