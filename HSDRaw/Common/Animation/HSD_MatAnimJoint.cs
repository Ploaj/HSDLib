namespace HSDRaw.Common.Animation
{
    public class HSD_MatAnimJoint : HSDTreeAccessor<HSD_MatAnimJoint>
    {
        public override int TrimmedSize => 0xC;

        public override HSD_MatAnimJoint Child { get => _s.GetReference<HSD_MatAnimJoint>(0x00); set => _s.SetReference(0x00, value); }

        public override HSD_MatAnimJoint Next { get => _s.GetReference<HSD_MatAnimJoint>(0x04); set => _s.SetReference(0x04, value); }

        public HSD_MatAnim MaterialAnimation { get => _s.GetReference<HSD_MatAnim>(0x08); set => _s.SetReference(0x08, value); }
    }
}
