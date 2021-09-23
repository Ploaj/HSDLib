using HSDRaw.Common.Animation;

namespace HSDRaw.AirRide.Rd
{
    public class KAR_RdMotion : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        public HSD_FigaTree Anim { get => _s.GetReference<HSD_FigaTree>(0x00); set => _s.SetReference(0x00, value); }

        public KAR_RdScript Script { get => _s.GetReference<KAR_RdScript>(0x04); set => _s.SetReference(0x04, value); }
    }
}
