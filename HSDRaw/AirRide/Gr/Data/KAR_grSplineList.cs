using HSDRaw.Common;

namespace HSDRaw.AirRide.Gr.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class KAR_grSplineList : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public HSD_Spline[] Splines
        {
            get
            {
                var spl = _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_Spline>>(0x00);
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
                    _s.GetCreateReference<HSDFixedLengthPointerArrayAccessor<HSD_Spline>>(0x00).Array = value;
                    Count = value.Length;
                }
            }
        }

        public int Count { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }
}
