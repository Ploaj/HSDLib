using HSDRaw.Common;

namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grLightGroup : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        public KAR_grLightNode GlobalLightGroup { get => _s.GetReference<KAR_grLightNode>(0x0); set => _s.SetReference(0x0, value); }

        public KAR_grLightNode LightGroup1 { get => _s.GetReference<KAR_grLightNode>(0x4); set => _s.SetReference(0x4, value); } // player light?

        public KAR_grLightNode LightGroup2 { get => _s.GetReference<KAR_grLightNode>(0x8); set => _s.SetReference(0x8, value); }
    }

    public class KAR_grLightNode : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSD_Light Light1 { get => _s.GetReference<HSD_Light>(0x0); set => _s.SetReference(0x0, value); }

        public HSD_Light Light2 { get => _s.GetReference<HSD_Light>(0x4); set => _s.SetReference(0x4, value); }

        public HSD_Light Light3 { get => _s.GetReference<HSD_Light>(0x8); set => _s.SetReference(0x8, value); }

        public HSD_Light Light4 { get => _s.GetReference<HSD_Light>(0xc); set => _s.SetReference(0xc, value); }
    }
}
