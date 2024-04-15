using System;
using System.Collections.Generic;
using System.Text;

namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grItemNode : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public int x00 { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public KAR_grItemNodeTimingTable TimingTable { get => _s.GetReference<KAR_grItemNodeTimingTable>(0x04); set => _s.SetReference(0x04, value); }

        public KAR_grItemNodeCityTrial CityTrial { get => _s.GetReference<KAR_grItemNodeCityTrial>(0x08); set => _s.SetReference(0x0C, value); }

        public KAR_grItemGeneralSpawnTable AirRide { get => _s.GetReference<KAR_grItemGeneralSpawnTable>(0x0C); set => _s.SetReference(0x0C, value); }

        public KAR_grItemGeneralSpawnTable Coliseum { get => _s.GetReference<KAR_grItemGeneralSpawnTable>(0x10); set => _s.SetReference(0x10, value); }
    }
}
