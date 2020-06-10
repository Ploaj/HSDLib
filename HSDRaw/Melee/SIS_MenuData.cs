namespace HSDRaw.Melee
{
    public class SBM_SISData : HSDAccessor
    {
        public HSDAccessor ImageData { get => _s.GetReference<HSDAccessor>(0x00); set => _s.SetReference(0x00, value); }

        public HSDAccessor CharacterSpacingParams { get => _s.GetReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }

        // everything else is op codes for drawing

            /// <summary>
            /// No need to align the text data
            /// </summary>
        public override void SetStructFlags()
        {
            for (int i = 8; i < _s.Length; i += 4)
                if (_s.GetReference<HSDAccessor>(i) != null)
                    _s.GetReference<HSDAccessor>(i)._s.Align = false;

            base.SetStructFlags();
        }
    }
}
