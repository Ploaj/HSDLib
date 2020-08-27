namespace HSDRaw.MEX
{
    public class MEX_EffectData : HSDAccessor
    {
        public override int TrimmedSize => 0x24;

        public HSDArrayAccessor<MEX_EffectFiles> EffectFiles { get => _s.GetReference<HSDArrayAccessor<MEX_EffectFiles>>(0x00); set => _s.SetReference(0x00, value); }

        public HSDAccessor RuntimeUnk1 { get => _s.GetReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }

        public HSDAccessor RuntimeUnk3 { get => _s.GetReference<HSDAccessor>(0x08); set => _s.SetReference(0x08, value); }

        public HSDAccessor RuntimeTexGrNum { get => _s.GetReference<HSDAccessor>(0x0C); set => _s.SetReference(0x0C, value); }

        public HSDAccessor RuntimeTexGrData { get => _s.GetReference<HSDAccessor>(0x10); set => _s.SetReference(0x10, value); }

        public HSDAccessor RuntimeUnk4 { get => _s.GetReference<HSDAccessor>(0x14); set => _s.SetReference(0x14, value); }

        public HSDAccessor RuntimePtclLast { get => _s.GetReference<HSDAccessor>(0x18); set => _s.SetReference(0x18, value); }

        public HSDAccessor RuntimePtclData { get => _s.GetReference<HSDAccessor>(0x1C); set => _s.SetReference(0x1C, value); }

        public HSDAccessor RuntimeLookup { get => _s.GetReference<HSDAccessor>(0x20); set => _s.SetReference(0x20, value); }
    }
}
