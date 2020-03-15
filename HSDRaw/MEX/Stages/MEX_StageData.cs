namespace HSDRaw.MEX.Stages
{
    public class MEX_StageData : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public HSDArrayAccessor<MEX_StageIDTable> StageIDTable { get => _s.GetReference<HSDArrayAccessor<MEX_StageIDTable>>(0x00); set => _s.SetReference(0x00, value); }

        public HSDArrayAccessor<MEX_StageReverb> ReverbTable { get => _s.GetReference<HSDArrayAccessor<MEX_StageReverb>>(0x04); set => _s.SetReference(0x04, value); }

    }

    public class MEX_StageIDTable : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        public int StageID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int Unknown1 { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int Unknown2 { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
    }

    public class MEX_StageReverb : HSDAccessor
    {
        public override int TrimmedSize => 0x03;

        public byte SSMID { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public byte Reverb { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public byte Unknown { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }
    }
}
