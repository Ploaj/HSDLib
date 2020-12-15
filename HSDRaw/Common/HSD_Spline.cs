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

        public HSDArrayAccessor<HSD_SegPoly> SegPolys
        {
            get => _s.GetReference<HSDArrayAccessor<HSD_SegPoly>>(0x14);
            set => _s.SetReference(0x14, value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HSD_SegPoly : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public float Value1 { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }
        public float Value2 { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
        public float Value3 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
        public float Value4 { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        public float Value5 { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
    }
}
