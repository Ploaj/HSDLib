namespace HSDRaw.Common.Animation
{
    public class HSD_ShapeAnimJoint : HSDTreeAccessor<HSD_ShapeAnimJoint>
    {
        public override int TrimmedSize => 0xC;

        public override HSD_ShapeAnimJoint Child { get => _s.GetReference<HSD_ShapeAnimJoint>(0x00); set => _s.SetReference(0x00, value); }

        public override HSD_ShapeAnimJoint Next { get => _s.GetReference<HSD_ShapeAnimJoint>(0x04); set => _s.SetReference(0x04, value); }

        public HSD_ShapeAnim ShapeAnimation { get => _s.GetReference<HSD_ShapeAnim>(0x08); set => _s.SetReference(0x08, value); }

    }
}
