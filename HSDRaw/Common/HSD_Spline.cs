namespace HSDRaw.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class HSD_Spline : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public short Type { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }

        public short PointCount { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public float Tension { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public HSDArrayAccessor<HSD_Vector3> Points
        {
            get => _s.GetReference<HSDArrayAccessor<HSD_Vector3>>(0x08);
            set => _s.SetReference(0x08, value);
        }

        public float TotalLength { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public HSDArrayAccessor<HSD_Float> Lengths
        {
            get => _s.GetReference<HSDArrayAccessor<HSD_Float>>(0x10);
            set => _s.SetReference(0x10, value);
        }

        //TODO: 0x14 has a pointer to some unknown structure
    }
}
