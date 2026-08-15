using System.ComponentModel;

namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grEventYakuExt : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        [Description("This replaces the model and animation data in City's Yakumono Entry #5. Idk what it is.")]
        public HSDAccessor YakumonoEntry5Data { get => _s.GetReference<HSDAccessor>(0x00); set => _s.SetReference(0x00, value); }

        public int x04 { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        [Description("Additional yakumono entries for events.")]
        public HSDFixedLengthPointerArrayAccessor<KAR_YakumonoDesc> YakumonoEntries { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<KAR_YakumonoDesc>>(0x08); set => _s.SetReference(0x08, value); }

        [Description("Number of additional yakumono event entries.")]
        public int YakumonoCount { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public int x10 { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public int x14 { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }
    }
}
