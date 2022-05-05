using System.Drawing;

namespace HSDRaw.Melee.Pl
{
    public class SBM_AfterImageDesc : HSDAccessor
    {
        public override int TrimmedSize => 0x20;

        public float x0 { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float x4 { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public byte AlphaStart { get => _s.GetByte(0x08); set => _s.SetByte(0x08, value); }

        public byte AlphaEnd { get => _s.GetByte(0x09); set => _s.SetByte(0x09, value); }

        public Color InCol { get => _s.GetColorRGB(0x0A); set => _s.SetColorRGB(0x0A, value); }

        public Color OutCol { get => _s.GetColorRGB(0x0E); set => _s.SetColorRGB(0x0E, value); }

        public int Bone { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        public float Bottom { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        public float Top { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }
    }
}
