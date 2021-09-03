using HSDRaw.Common;

namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grSplineNode : HSDAccessor
    {
        public override int TrimmedSize => 0x1C;

        public KAR_grSplineSetup SplineSetup { get => _s.GetReference<KAR_grSplineSetup>(0x00); set => _s.SetReference(0x00, value); }

        public KAR_grRangeSplineSetup RangeSplineSetup { get => _s.GetReference<KAR_grRangeSplineSetup>(0x04); set => _s.SetReference(0x04, value); }

        // 08 ??

        // 0C ??

        // 10 grCheck grDesert
        public KAR_grConveyorPath ConveyorSpline { get => _s.GetReference<KAR_grConveyorPath>(0x10); set => _s.SetReference(0x10, value); }

        // 14
        public KAR_grSplineList RailSpline1 { get => _s.GetReference<KAR_grSplineList>(0x14); set => _s.SetReference(0x14, value); }

        // 18 grDesert
        public KAR_grSplineList RailSpline2 { get => _s.GetReference<KAR_grSplineList>(0x18); set => _s.SetReference(0x18, value); }
    }

    public class KAR_grConveyorPath : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public KAR_grSplineList SplineList { get => _s.GetReference<KAR_grSplineList>(0x00); set => _s.SetReference(0x00, value); }

    }

    public class KAR_grSplineSetup : HSDAccessor
    {
        public override int TrimmedSize => 0x38;

        public KAR_grSplineList CourseSplineList { get => _s.GetReference<KAR_grSplineList>(0x00); set => _s.SetReference(0x00, value); }

        public bool Loop { get => _s.GetByte(0x10) == 1; set => _s.SetByte(0x10, (byte)(value ? 1 : 0)); }
    }


    public class KAR_grRangeSplineSetup : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public KAR_grRangeSpline[] Splines
        {
            get
            {
                var spl = _s.GetReference<HSDArrayAccessor<KAR_grRangeSpline>>(0x00);
                return spl == null ? null : spl.Array;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    _s.SetReference(0x00, null);
                    Count = 0;
                }
                else
                {
                    _s.GetCreateReference<HSDArrayAccessor<KAR_grRangeSpline>>(0x00).Array = value;
                    Count = value.Length;
                }
            }
        }

        public int Count { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }

    public class KAR_grRangeSpline : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public HSD_Spline LeftSpline { get => _s.GetReference<HSD_Spline>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_Spline RightSpline { get => _s.GetReference<HSD_Spline>(0x04); set => _s.SetReference(0x04, value); }

        public int x08 { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int x0C { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public int x10 { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public int x14 { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }
    }
}
