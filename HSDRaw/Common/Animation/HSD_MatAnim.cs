namespace HSDRaw.Common.Animation
{
    public class HSD_MatAnim : HSDListAccessor<HSD_MatAnim>
    {
        public override int TrimmedSize => 0x10;

        public override HSD_MatAnim Next { get => _s.GetReference<HSD_MatAnim>(0x00); set => _s.SetReference(0x00, value); }
        
        public HSD_AOBJ AnimationObject { get => _s.GetReference<HSD_AOBJ>(0x04); set => _s.SetReference(0x04, value); }

        public HSD_TexAnim TextureAnimation { get => _s.GetReference<HSD_TexAnim>(0x08); set => _s.SetReference(0x08, value); }

        public bool RenderAnim { get => _s.GetInt32(0x0C) == 1;
            set {
                if (value) _s.SetInt32(0x0C, 1); else _s.SetInt32(0x0C, 0);
            }
        }
    }
}
