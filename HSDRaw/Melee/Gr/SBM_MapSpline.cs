using HSDRaw.Common;
using HSDRaw.Common.Animation;

namespace HSDRaw.Melee.Gr
{
    public class SBM_MapSpline : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public SBM_SplineEntry SplineEntry { get => _s.GetReference<SBM_SplineEntry>(0x00); set => _s.SetReference(0x00, value); }

        public uint Flag { get => (uint)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }
    }

    public class SBM_SplineEntry : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSD_AOBJ AOBJ { get => _s.GetReference<HSD_AOBJ>(0x04); set => _s.SetReference(0x04, value); }

        public HSDNullPointerArrayAccessor<SBM_SplineDesc> SplineDesc { get => _s.GetReference<HSDNullPointerArrayAccessor<SBM_SplineDesc>>(0x08); set => _s.SetReference(0x08, value); }
    }

    public class SBM_SplineDesc : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public float FrameCount { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public HSD_FOBJDesc FOBJDesc { get => _s.GetReference<HSD_FOBJDesc>(0x08); set => _s.SetReference(0x08, value); }

        public HSD_JOBJ SplineDesc { get => _s.GetReference<HSD_JOBJ>(0x0C); set => _s.SetReference(0x0C, value); }
    }
}
