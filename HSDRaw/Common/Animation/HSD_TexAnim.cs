using HSDRaw.GX;
using System;
using System.Collections.Generic;
using System.Text;

namespace HSDRaw.Common.Animation
{
    public class HSD_TexAnim : HSDListAccessor<HSD_TexAnim>
    {
        public override HSD_TexAnim Next { get => _s.GetReference<HSD_TexAnim>(0x00); set => _s.SetReference(0x00, value); }
        
        public GXTexMapID GXTexMapID { get => (GXTexMapID)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }

        public HSD_AOBJ AnimationObject { get => _s.GetReference<HSD_AOBJ>(0x08); set => _s.SetReference(0x08, value); }

        //public uint ImageArrayOffset { get; set; } 0x0C;

        //public uint TlutArrayOffset { get; set; } 0x10;

        public short ImageCount
        {
            get => _s.GetInt16(0x14);
        }

        public short TlutCount
        {
            get => _s.GetInt16(0x16);
        }
    }
}
