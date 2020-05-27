namespace HSDRaw.MEX
{
    public class MEX_EffectData : HSDAccessor
    {
        public override int TrimmedSize => 0x20;

        public HSDArrayAccessor<MEX_EffectFiles> EffectFiles { get => _s.GetReference<HSDArrayAccessor<MEX_EffectFiles>>(0x00); set => _s.SetReference(0x00, value); }

        public HSDAccessor EffectRuntime { get => _s.GetReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }

        public HSDAccessor RuntimeArray1 { get => _s.GetReference<HSDAccessor>(0x08); set => _s.SetReference(0x08, value); }

        public HSDAccessor RuntimeArray2 { get => _s.GetReference<HSDAccessor>(0x0C); set => _s.SetReference(0x0C, value); }

        public HSDAccessor RuntimeArray3 { get => _s.GetReference<HSDAccessor>(0x10); set => _s.SetReference(0x10, value); }

        public HSDAccessor RuntimeArray4 { get => _s.GetReference<HSDAccessor>(0x14); set => _s.SetReference(0x14, value); }

        public HSDAccessor RuntimeArray5 { get => _s.GetReference<HSDAccessor>(0x18); set => _s.SetReference(0x18, value); }

        public HSDAccessor RuntimeArray6 { get => _s.GetReference<HSDAccessor>(0x1C); set => _s.SetReference(0x1C, value); }
    }
}
