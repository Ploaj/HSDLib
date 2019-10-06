using HSDRaw.Common;

namespace HSDRaw.AirRide.Gr
{
    public class KAR_grSkyBoxModel : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSD_JOBJ JOBJRoot { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public KAR_grModelMotion MotionJoint { get => _s.GetReference<KAR_grModelMotion>(0x04); set => _s.SetReference(0x04, value); }
    }
}
