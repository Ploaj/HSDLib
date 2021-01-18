namespace HSDRaw.MEX.Menus
{
    public class MEX_MenuTable : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        public MEX_MenuParameters Parameters { get => _s.GetReference<MEX_MenuParameters>(0x00); set => _s.SetReference(0x00, value); }

        public MEX_IconData CSSIconData { get => _s.GetReference<MEX_IconData>(0x04); set => _s.SetReference(0x04, value); }

        public HSDArrayAccessor<MEX_StageIconData> SSSIconData { get => _s.GetReference<HSDArrayAccessor<MEX_StageIconData>>(0x08); set => _s.SetReference(0x08, value); }
    }

    public class MEX_MenuParameters : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public float CSSHandScale { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float StageSelectCursorStartX { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float StageSelectCursorStartY { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float StageSelectCursorStartZ { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
    }
}
