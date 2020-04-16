namespace HSDRaw.MEX.Menus
{
    public class MEX_MenuTable : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public MEX_IconData CSSIconData { get => _s.GetReference<MEX_IconData>(0x00); set => _s.SetReference(0x00, value); }

        public HSDArrayAccessor<MEX_StageIconData> SSSIconData { get => _s.GetReference<HSDArrayAccessor<MEX_StageIconData>>(0x04); set => _s.SetReference(0x04, value); }
    }
}
