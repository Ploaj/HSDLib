using HSDRaw.Common;
using HSDRaw.Common.Animation;

namespace HSDRaw.AirRide.Rd
{
    public class KAR_RdCap : HSDAccessor
    {

        public HSD_JOBJ Model { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_MatAnimJoint MatAnim { get => _s.GetReference<HSD_MatAnimJoint>(0x04); set => _s.SetReference(0x04, value); }

        public KAR_LODTableCollection HighPolyTable { get => _s.GetReference<KAR_LODTableCollection>(0x0C); set => _s.SetReference(0x0C, value); }

        public KAR_LODTableCollection MidPolyTable { get => _s.GetReference<KAR_LODTableCollection>(0x10); set => _s.SetReference(0x10, value); }

        public KAR_LODTableCollection LowPolyTable { get => _s.GetReference<KAR_LODTableCollection>(0x14); set => _s.SetReference(0x14, value); }


        // 0x18 onward is unique but usually animations

    }
}
