using HSDRaw.GX;

namespace HSDRaw.Common.Animation
{
    public class HSD_TexAnim : HSDListAccessor<HSD_TexAnim>
    {
        public override HSD_TexAnim Next { get => _s.GetReference<HSD_TexAnim>(0x00); set => _s.SetReference(0x00, value); }
        
        public GXTexMapID GXTexMapID { get => (GXTexMapID)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }

        public HSD_AOBJ AnimationObject { get => _s.GetReference<HSD_AOBJ>(0x08); set => _s.SetReference(0x08, value); }

        public HSDArrayAccessor<HSD_TexBuffer> ImageBuffers
        {
            get => _s.GetReference<HSDArrayAccessor<HSD_TexBuffer>>(0x0C);
            set
            {
                ImageCount = (short)value.Length;
                _s.SetReference(0x0C, value);
            }
        }
        
        public HSDArrayAccessor<HSD_TlutBuffer> TlutBuffers
        {
            get => _s.GetReference<HSDArrayAccessor<HSD_TlutBuffer>>(0x10);
            set
            {
                TlutCount = (short)value.Length;
                _s.SetReference(0x10, value);
            }
        }

        public short ImageCount
        {
            get => _s.GetInt16(0x14);
            internal set => _s.SetInt16(0x14, value);
        }

        public short TlutCount
        {
            get => _s.GetInt16(0x16);
            internal set => _s.SetInt16(0x16, value);
        }
    }

    public class HSD_TexBuffer : HSDAccessor
    {
        public override int TrimmedSize => 4;

        public HSD_Image Data { get => _s.GetReference<HSD_Image>(0); set => _s.SetReference(0, value); }
    }

    public class HSD_TlutBuffer : HSDAccessor
    {
        public override int TrimmedSize => 4;

        public HSD_Tlut Data { get => _s.GetReference<HSD_Tlut>(0); set => _s.SetReference(0, value); }
    }
}
