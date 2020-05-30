namespace HSDRaw.Melee
{
    public class SBM_SISData : HSDAccessor
    {
        public HSDAccessor ImageData { get => _s.GetReference<HSDAccessor>(0x00); set => _s.SetReference(0x00, value); }

        public HSDAccessor CharacterSpacingParams { get => _s.GetReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }

        // everything else is op codes for drawing
    }
}
