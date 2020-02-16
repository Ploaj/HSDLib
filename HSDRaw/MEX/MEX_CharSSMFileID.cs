namespace HSDRaw.MEX
{
    public class MEX_CharSSMFileID : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public byte SSMID { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public int Unknown { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int BitField1 { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int BitField2 { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }
    }
}
