using HSDRaw.Common.Animation;

namespace HSDRaw.AirRide.Gr
{
    public class KAR_grModelMotion : HSDAccessor
    {
        public override int TrimmedSize => 8;

        public HSD_AnimJoint AnimJoint { get => _s.GetReference<HSD_AnimJoint>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_MatAnimJoint MatAnimJoint { get => _s.GetReference<HSD_MatAnimJoint>(0x04); set => _s.SetReference(0x04, value); }
    }
}
