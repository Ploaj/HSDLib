using HSDRaw.Common;

namespace HSDRaw.AirRide.Gr
{
    public class KAR_grMainModel : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public HSD_JOBJ RootNode { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }
        
        public int Unk1 { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int Unk2 { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int Unk3 { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public KAR_grModelBounding ModelBounding { get => _s.GetReference<KAR_grModelBounding>(0x10); set => _s.SetReference(0x10, value); }
    }
}
