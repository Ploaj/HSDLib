namespace HSDRaw.MEX
{
    public class MEX_SSMLookup : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public int EntireFlag { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public byte GroupIndex { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }
        public byte Unknown1 { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }
        public byte Unknown2 { get => _s.GetByte(0x02); set => _s.SetByte(0x02, value); }
        public byte Unknown3 { get => _s.GetByte(0x03); set => _s.SetByte(0x03, value); }
    }
}
