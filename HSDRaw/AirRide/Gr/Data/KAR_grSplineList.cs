using HSDRaw.Common;

namespace HSDRaw.AirRide.Gr.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class KAR_grSplineList : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public HSDFixedLengthPointerArrayAccessor<HSD_Spline> Splines
        {
            get
            {
                return _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_Spline>>(0x00);
            }
            set
            {
                _s.SetReference(0x00, value);
            }
        }

        public int Count { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }
}
