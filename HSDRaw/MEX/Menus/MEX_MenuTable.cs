namespace HSDRaw.MEX.Menus
{
    public class MEX_MenuTable : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public MEX_MenuParameters Parameters { get => _s.GetReference<MEX_MenuParameters>(0x00); set => _s.SetReference(0x00, value); }

        public MEX_IconData CSSIconData { get => _s.GetReference<MEX_IconData>(0x04); set => _s.SetReference(0x04, value); }

        public HSDArrayAccessor<MEX_StageIconData> SSSIconData { get => _s.GetReference<HSDArrayAccessor<MEX_StageIconData>>(0x08); set => _s.SetReference(0x08, value); }

        public SSSBitfield SSSBitField { get => _s.GetReference<SSSBitfield>(0x0C); set => _s.SetReference(0x0C, value); }
    }

    public class SSSBitfield : HSDByteArray
    {
        public void SetField(int index, bool value)
        {
            int byteIndex = index / 8;
            int bitIndex = index % 8;

            var temp = Array;
            if (value)
                temp[byteIndex] |= (byte)(1 << bitIndex);
            else
                temp[byteIndex] &= (byte)~(1 << bitIndex);
            Array = temp;
        }

        public bool GetField(int index)
        {
            int byteIndex = index / 8;
            int bitIndex = index % 8;

            return ((Array[byteIndex] >> bitIndex) & 0x1) != 0;
        }
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
