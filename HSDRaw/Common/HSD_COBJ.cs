namespace HSDRaw.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class HSD_Camera : HSDAccessor
    {
        public override int TrimmedSize => 0x3C;

        public string ClassName { get => _s.GetString(0x00); set => _s.SetString(0x00, value); }
        public short Flags { get => _s.GetInt16(0x04); set => _s.SetInt16(0x04, value); }
        public short ProjectionType { get => _s.GetInt16(0x06); set => _s.SetInt16(0x06, value); }
        public short ViewportLeft { get => _s.GetInt16(0x08); set => _s.SetInt16(0x08, value); }
        public short ViewportRight { get => _s.GetInt16(0x0A); set => _s.SetInt16(0x0A, value); }
        public short ViewportTop { get => _s.GetInt16(0x0C); set => _s.SetInt16(0x0C, value); }
        public short ViewportBottom { get => _s.GetInt16(0x0E); set => _s.SetInt16(0x0E, value); }
        public int ProjWidth { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }
        public int ProjHeight { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }
        public HSD_WOBJ eye { get => _s.GetReference<HSD_WOBJ>(0x18); set => _s.SetReference(0x18, value); }
        public HSD_WOBJ target { get => _s.GetReference<HSD_WOBJ>(0x1C); set => _s.SetReference(0x1C, value); }
        public float Roll { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }
        public int Unknown3 { get => _s.GetInt32(0x24); set => _s.SetInt32(0x24, value); }
        public float NearClip { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }
        public float FarClip { get => _s.GetFloat(0x2C); set => _s.SetFloat(0x2C, value); }
        public HSD_Camera SelfReference { get => _s.GetReference<HSD_Camera>(0x30); set => _s.SetReference(0x30, value); }
        public float FieldOfView { get => _s.GetFloat(0x34); set => _s.SetFloat(0x34, value); }
    }
    
}
