using System.ComponentModel;

namespace HSDRaw.MEX.Menus
{
    public class MEX_StageIconData : HSDAccessor
    {
        public override int TrimmedSize => 0x20;

        [Browsable(false)]
        public int RuntimeJOBJPointer { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public bool RandomEnabled 
        { 
            get => (_s.GetByte(0x04) & 0x40) != 0; 
            set => _s.SetByte(0x04, (byte)((_s.GetByte(0x04) & ~0x40) | (value ? 0x40 : 0x00))); 
        }

        [Browsable(false)]
        public byte IconState { get => _s.GetByte(0x08); set => _s.SetByte(0x08, value); }

        public byte PreviewModelID { get => _s.GetByte(0x09); set => _s.SetByte(0x09, value); }

        public byte RandomStageSelectID { get => _s.GetByte(0x0A); set => _s.SetByte(0x0A, value); }

        private byte _oldExternalID { get => _s.GetByte(0x0B); set => _s.SetByte(0x0B, value); }
        public int ExternalID { get => _s.GetInt32(0x1C); set { _s.SetInt32(0x1C, value); } }

        public float CursorWidth { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        public float CursorHeight { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        public float OutlineWidth { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
        public float OutlineHeight { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        public override string ToString()
        {
            return "ID: " + ExternalID;
        }
    }
}
