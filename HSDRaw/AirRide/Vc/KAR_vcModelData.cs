using HSDRaw.Common;

namespace HSDRaw.AirRide.Vc
{
    public class KAR_vcModelData : HSDAccessor
    {
        public override int TrimmedSize => 0x2C;

        public HSD_JOBJ MainModelRoot { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public int Unknown1 { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public byte BoneCount { get => _s.GetByte(0x08); set => _s.SetByte(0x08, value); }

        public byte Unknown2 { get => _s.GetByte(0x09); set => _s.SetByte(0x09, value); }
        public byte Unknown3 { get => _s.GetByte(0x0A); set => _s.SetByte(0x0A, value); }
        public byte Unknown4 { get => _s.GetByte(0x0B); set => _s.SetByte(0x0B, value); }

        public int Unknown5 { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        // TODO: 6 unknown model structs

        public HSD_JOBJ ShadowModelRoot { get => _s.GetReference<HSD_JOBJ>(0x28); set => _s.SetReference(0x28, value); }
    }
}
