namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grAreaPositionList : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        public HSDIntArray JointIndices { get => _s.GetReference<HSDIntArray>(0x00); set => _s.SetReference(0x00, value); }

        public HSDArrayAccessor<KAR_grAreaPositionData> AreaPosition { get => _s.GetReference<HSDArrayAccessor<KAR_grAreaPositionData>>(0x04); set => _s.SetReference(0x04, value);  }

        public int Count { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
    }

    public class KAR_grAreaPositionData : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public float X { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float Y { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float Z { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float DX { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float DY { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        public float DZ { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
    }
}
