namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grEventDataAllCity1 : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public KAR_grEventParamNode EventNode { get => _s.GetReference<KAR_grEventParamNode>(0x00); set => _s.SetReference(0x00, value); }

        public KAR_grEventYakuExt YakuExt { get => _s.GetReference<KAR_grEventYakuExt>(0x04); set => _s.SetReference(0x04, value); }
    }
}
