namespace HSDRaw.MEX
{
    public class MEX_EffectData : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSDArrayAccessor<MEX_EffectFiles> EffectFiles { get => _s.GetReference<HSDArrayAccessor<MEX_EffectFiles>>(0x00); set => _s.SetReference(0x00, value); }

        public HSDAccessor EffectRuntime { get => _s.GetReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }
    }
}
