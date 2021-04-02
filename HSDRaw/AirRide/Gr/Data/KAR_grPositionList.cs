namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grPositionList : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        public HSDIntArray JointIndices { get => _s.GetReference<HSDIntArray>(0x00); set => _s.SetReference(0x00, value); }

        public KAR_grPositionData[] PositionData
        {
            get
            {
                var spl = _s.GetReference<HSDArrayAccessor<KAR_grPositionData>>(0x04);
                return spl == null ? null : spl.Array;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    _s.SetReference(0x04, null);
                    Count = 0;
                }
                else
                {
                    _s.GetCreateReference<HSDArrayAccessor<KAR_grPositionData>>(0x04).Array = value;
                    Count = value.Length;
                }
            }
        }

        public int Count { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
    }

    public class KAR_grPositionData : HSDAccessor
    {
        public override int TrimmedSize => 0x24;

        public float X { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float Y { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float Z { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float x0C { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float x10 { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        public float x14 { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        public float x18 { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        public float x1C { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }

        public float x20 { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }
    }
}
