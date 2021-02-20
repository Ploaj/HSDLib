using System.Drawing;

namespace HSDRaw.Common
{
    public enum FogType
    {
        None = 0x00,

        PerspectiveLinear = 0x02,
        PerspectiveExp = 0x04,
        PerspectiveExp2 = 0x05,
        PerspectiveRevExp = 0x06,
        PerspectiveRevExp2 = 0x07,

        OrthographicLinear = 0x0A,
        OrthographicExp = 0x0C,
        OrthographicExp2 = 0x0D,
        OrthographicRevExp = 0x0E,
        OrthographicRevExp2 = 0x0F,
    }

    public class HSD_FogAdjDesc : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSD_FogDesc Fog { get => _s.GetReference<HSD_FogDesc>(0x00); set => _s.SetReference(0x00, value); }

    }

    public class HSD_FogDesc : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public FogType Type { get => (FogType)_s.GetInt32(0x00); set => _s.SetInt32(0x00, (int)value); }

        public HSD_FogAdjDesc FogAdjDesc { get => _s.GetReference<HSD_FogAdjDesc>(0x04); set => _s.SetReference(0x04, value); }
        
        public float Start { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float End { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        
        public Color Color { get => _s.GetColorRGBA(0x10); set => _s.SetColorRGBA(0x10, value); }

    }
}
