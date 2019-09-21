namespace HSDRaw.Common.Animation
{
    /// <summary>
    /// An extended header for a <see cref="HSD_FOBJ"/> that is a linked list 
    /// containing a starting frame and a data length
    /// </summary>
    public class HSD_FOBJDesc : HSDListAccessor<HSD_FOBJDesc>
    {
        public override int TrimmedSize => 0x14;

        public override HSD_FOBJDesc Next { get => _s.GetReference<HSD_FOBJDesc>(0x00); set => _s.SetReference(0x00, value); }

        public int DataLength { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int StartFrame { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public HSD_FOBJ FOBJ
        {
            get
            {
                HSD_FOBJ fobj = new HSD_FOBJ();
                fobj._s = _s.GetEmbededStruct(0x0C, 0x8);
                return fobj;
            }
            set
            {
                _s.SetEmbededStruct(0x0C, value._s);
                if (value != null)
                    DataLength = value.Buffer.Length;
                else
                    DataLength = 0;
            }
        }
    }
}
