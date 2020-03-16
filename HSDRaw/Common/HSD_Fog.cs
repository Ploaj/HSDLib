using System.Drawing;

namespace HSDRaw.Common
{
    public class HSD_FogAdjDesc : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

    }

    public class HSD_FogDesc : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public int Type { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public HSD_FogAdjDesc FogAdjDesc { get => _s.GetReference<HSD_FogAdjDesc>(0x04); set => _s.SetReference(0x04, value); }
        
        public float Start { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float End { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        
        public Color Color { get => _s.GetColorRGBA(0x10); set => _s.SetColorRGBA(0x10, value); }

    }
}
