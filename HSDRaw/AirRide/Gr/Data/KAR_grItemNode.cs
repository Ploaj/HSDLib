using System;
using System.Collections.Generic;
using System.Text;

namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grItemNode : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public int x00 { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }
    }
}
