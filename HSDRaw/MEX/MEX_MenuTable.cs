using System.ComponentModel;

namespace HSDRaw.MEX
{
    public class MEX_MenuTable : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public MEX_IconData CSSIconData { get => _s.GetReference<MEX_IconData>(0x00); set => _s.SetReference(0x00, value); }

        public HSDArrayAccessor<MEX_StageIconData> SSSIconData { get => _s.GetReference<HSDArrayAccessor<MEX_StageIconData>>(0x04); set => _s.SetReference(0x04, value); }
    }

    public class MEX_StageIconData : HSDAccessor
    {
        public override int TrimmedSize => 0x1C;

        [Browsable(false)]
        public int RuntimeJOBJPointer { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        [Browsable(false)]
        public int Unknown { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
        
        public byte IconState { get => _s.GetByte(0x08); set => _s.SetByte(0x08, value); }
        public byte PreviewID { get => _s.GetByte(0x09); set => _s.SetByte(0x09, value); }
        public byte RandomID { get => _s.GetByte(0x0A); set => _s.SetByte(0x0A, value); }
        public byte InternalID { get => _s.GetByte(0x0B); set => _s.SetByte(0x0B, value); }

        public float CursorX { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        public float CursorY { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        public float OutlineX { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
        public float OutlineY { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }
    }
}
