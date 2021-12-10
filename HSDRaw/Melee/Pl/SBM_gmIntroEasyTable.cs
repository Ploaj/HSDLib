namespace HSDRaw.Melee.Pl
{
    public class SBM_gmIntroEasyTable : HSDAccessor
    {
        public override int TrimmedSize => 0x24;

        public float XOffset { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float YOffset { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float XScale { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float YScale { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float ZScale { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        public float x14 { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        public float XMultiOff { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        public float YMultiOff { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }

        public float x20 { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }

        public override void New()
        {
            base.New();
            XScale = 1;
            YScale = 1;
            ZScale = 1;
        }
    }
}
