using HSDRaw.Common.Animation;

namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grSubAnimNode : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public KAR_grSubAnim x00 { get => _s.GetReference<KAR_grSubAnim>(0x00); set => _s.SetReference(0x00, value); }

        public KAR_grSubAnim x04 { get => _s.GetReference<KAR_grSubAnim>(0x04); set => _s.SetReference(0x04, value); }

        public KAR_grSubAnim Rail { get => _s.GetReference<KAR_grSubAnim>(0x08); set => _s.SetReference(0x08, value); }

        public KAR_grSubAnim x0C { get => _s.GetReference<KAR_grSubAnim>(0x0C); set => _s.SetReference(0x0C, value); }

        public KAR_grSubAnim x10 { get => _s.GetReference<KAR_grSubAnim>(0x10); set => _s.SetReference(0x10, value); }

        public KAR_grSubAnim x14 { get => _s.GetReference<KAR_grSubAnim>(0x14); set => _s.SetReference(0x14, value); }
    }

    public class KAR_grSubAnim : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSDFixedLengthPointerArrayAccessor<HSD_AnimJoint> Animations { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_AnimJoint>>(0x00); set => _s.SetReference(0x00, value); }

        public int Count { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }
}
