namespace HSDRaw.Common.Animation
{
    public class HSD_ShapeAnim : HSDListAccessor<HSD_ShapeAnim>
    {
        public override int TrimmedSize => 0x8;

        public override HSD_ShapeAnim Next { get => _s.GetReference<HSD_ShapeAnim>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_AOBJDesc AnimationObject { get => _s.GetReference<HSD_AOBJDesc>(0x04); set => _s.SetReference(0x04, value); }

    }
}
