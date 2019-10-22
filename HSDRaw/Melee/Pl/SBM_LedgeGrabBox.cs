namespace HSDRaw.Melee.Pl
{
    public class SBM_LedgeGrabBox : HSDAccessor
    {
        public override int TrimmedSize => 0x1C;

        public short Unknown1 { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }
        public short Unknown2 { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }
        public short Unknown3 { get => _s.GetInt16(0x04); set => _s.SetInt16(0x04, value); }
        public short Unknown4 { get => _s.GetInt16(0x06); set => _s.SetInt16(0x06, value); }
        public short Unknown5 { get => _s.GetInt16(0x08); set => _s.SetInt16(0x08, value); }
        public short Unknown6 { get => _s.GetInt16(0x0A); set => _s.SetInt16(0x0A, value); }

        public float HorizontalOffset { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        public float HorizontalScale { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        public float VerticalOffset { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
        public float VerticalScale { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }
    }
}
