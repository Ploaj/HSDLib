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

        public bool color_a_set { get => (color_a & 0x80) != 0; set => color_a = (byte)((color_a & 0x7F) | (value ? 0x80 : 0x00)); }
        public bool color_b_set { get => (color_b & 0x80) != 0; set => color_b = (byte)((color_b & 0x7F) | (value ? 0x80 : 0x00)); }
        public bool color_c_set { get => (color_c & 0x80) != 0; set => color_c = (byte)((color_c & 0x7F) | (value ? 0x80 : 0x00)); }
        public bool color_d_set { get => (color_d & 0x80) != 0; set => color_d = (byte)((color_d & 0x7F) | (value ? 0x80 : 0x00)); }
        public TevColorIn color_a_in { get => (TevColorIn)(color_a & 0x7F); set => color_a = (byte)((color_a & 0x80) | ((byte)value & 0x7F)); }
        public TevColorIn color_b_in { get => (TevColorIn)(color_b & 0x7F); set => color_b = (byte)((color_b & 0x80) | ((byte)value & 0x7F)); }
        public TevColorIn color_c_in { get => (TevColorIn)(color_c & 0x7F); set => color_c = (byte)((color_c & 0x80) | ((byte)value & 0x7F)); }
        public TevColorIn color_d_in { get => (TevColorIn)(color_d & 0x7F); set => color_d = (byte)((color_d & 0x80) | ((byte)value & 0x7F)); }

        public bool alpha_a_set { get => (alpha_a & 0x80) != 0; set => alpha_a = (byte)((alpha_a & 0x7F) | (value ? 0x80 : 0x00)); }
        public bool alpha_b_set { get => (alpha_b & 0x80) != 0; set => alpha_b = (byte)((alpha_b & 0x7F) | (value ? 0x80 : 0x00)); }
        public bool alpha_c_set { get => (alpha_c & 0x80) != 0; set => alpha_c = (byte)((alpha_c & 0x7F) | (value ? 0x80 : 0x00)); }
        public bool alpha_d_set { get => (alpha_d & 0x80) != 0; set => alpha_d = (byte)((alpha_d & 0x7F) | (value ? 0x80 : 0x00)); }
        public TevColorIn alpha_a_in { get => (TevColorIn)(alpha_a & 0x7F); set => alpha_a = (byte)((alpha_a & 0x80) | ((byte)value & 0x7F)); }
        public TevColorIn alpha_b_in { get => (TevColorIn)(alpha_b & 0x7F); set => alpha_b = (byte)((alpha_b & 0x80) | ((byte)value & 0x7F)); }
        public TevColorIn alpha_c_in { get => (TevColorIn)(alpha_c & 0x7F); set => alpha_c = (byte)((alpha_c & 0x80) | ((byte)value & 0x7F)); }
        public TevColorIn alpha_d_in { get => (TevColorIn)(alpha_d & 0x7F); set => alpha_d = (byte)((alpha_d & 0x80) | ((byte)value & 0x7F)); }

        public byte constantAlpha { get => _s.GetByte(0x13); set => _s.SetByte(0x13, value); }
        public Color constant { get => _s.GetColorRGB(0x10); set => _s.SetColorRGB(0x10, value); }

        public byte tev0Alpha { get => _s.GetByte(0x17); set => _s.SetByte(0x17, value); }
        public Color tev0 { get => _s.GetColorRGB(0x14); set => _s.SetColorRGB(0x14, value); }

        public byte tev1Alpha { get => _s.GetByte(0x1B); set => _s.SetByte(0x1B, value); }
        public Color tev1 { get => _s.GetColorRGB(0x18); set => _s.SetColorRGB(0x18, value); }
        
        public int active { get => _s.GetInt32(0x1C); set => _s.SetInt32(0x1C, value); }
    }
}
