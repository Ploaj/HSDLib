namespace HSDRaw.AirRide
{
    public class KAR_LODTableCollection : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int Count { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public HSDArrayAccessor<KAR_LODTable> Entries
        {
            get => _s.GetReference<HSDArrayAccessor<KAR_LODTable>>(0x04);
            set
            {
                if (value != null)
                    Count = value.Length;

                _s.SetReference(0x04, value);
            }
        }
    }

    public class KAR_LODTable : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int Count { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public HSDByteArray Entries
        {
            get => _s.GetReference<HSDByteArray>(0x04);
            set
            {
                if (value != null)
                    Count = value.Length;

                _s.SetReference(0x04, value);
            }
        }
    }
}
