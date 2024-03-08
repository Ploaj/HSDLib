using HSDRaw.Common;
using System.Drawing;

namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grFogNode : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public KAR_grFogData FogData { get => _s.GetReference<KAR_grFogData>(0x00); set => _s.SetReference(0x00, value); }

        public KAR_TypeData FogTypes { get => _s.GetReference<KAR_TypeData>(0x04); set => _s.SetReference(0x04, value); }
    }

    public class KAR_grFogData : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSD_FogDesc Fog { get => _s.GetReference<HSD_FogDesc>(0x00); set => _s.SetReference(0x00, value); }

    }

    public class KAR_TypeData : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSDArrayAccessor<KAR_TypeDataEntry> FogData { get => _s.GetReference<HSDArrayAccessor<KAR_TypeDataEntry>>(0x00); set => _s.SetReference(0x00, value); }

        public int Count { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }

    public class KAR_TypeDataEntry : HSDAccessor
    {
        public override int TrimmedSize => 0x48;

        public int x00 { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public Color Color1 { get => _s.GetColorRGBA(0x04); set => _s.SetColorRGBA(0x04, value); }

        public float x04 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float x08 { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public Color Color2 { get => _s.GetColorRGBA(0x10); set => _s.SetColorRGBA(0x10, value); }

        public Color Color3 { get => _s.GetColorRGBA(0x14); set => _s.SetColorRGBA(0x14, value); }

        public float x18 { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        public float x1C { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }

        public Color Color4 { get => _s.GetColorRGBA(0x20); set => _s.SetColorRGBA(0x20, value); }

        public Color Color5 { get => _s.GetColorRGBA(0x24); set => _s.SetColorRGBA(0x24, value); }

        public float x28 { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }

        public float x2C { get => _s.GetFloat(0x2C); set => _s.SetFloat(0x2C, value); }

        public float x30 { get => _s.GetFloat(0x30); set => _s.SetFloat(0x30, value); }

        public int x34 { get => _s.GetInt32(0x34); set => _s.SetInt32(0x34, value); }

        public int x38 { get => _s.GetInt32(0x38); set => _s.SetInt32(0x38, value); }

        public int x3C { get => _s.GetInt32(0x3C); set => _s.SetInt32(0x3C, value); }

        public int x40 { get => _s.GetInt32(0x40); set => _s.SetInt32(0x40, value); }

        public byte Flag { get => _s.GetByte(0x44); set => _s.SetByte(0x44, value); }
    }
}
