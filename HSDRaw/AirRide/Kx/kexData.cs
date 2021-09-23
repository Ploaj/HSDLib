using System;
using System.Collections.Generic;
using System.Text;

namespace HSDRaw.AirRide.Kx
{
    public class kexData : HSDAccessor
    {
        public override int TrimmedSize => 0x30;

        // 0x00 MetaData

        public KEX_RiderTable RiderTable { get => _s.GetReference<KEX_RiderTable>(0x04); set => _s.SetReference(0x04, value); }

        public KEX_VehicleTable VehicleTable { get => _s.GetReference<KEX_VehicleTable>(0x08); set => _s.SetReference(0x08, value); }

        public KEX_StageTable StageTable { get => _s.GetReference<KEX_StageTable>(0x0C); set => _s.SetReference(0x0C, value); }

        public KEX_MusicTable MusicEntry { get => _s.GetReference<KEX_MusicTable>(0x10); set => _s.SetReference(0x10, value); }

        public KEX_RdFunctionTable FunctionTable { get => _s.GetReference<KEX_RdFunctionTable>(0x14); set => _s.SetReference(0x14, value); }


        public override void New()
        {
            base.New();
            VehicleTable = new KEX_VehicleTable();
            StageTable = new KEX_StageTable();
        }
    }
}
