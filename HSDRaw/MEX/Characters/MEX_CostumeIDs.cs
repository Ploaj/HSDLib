namespace HSDRaw.MEX
{
    public class MEX_CostumeIDs : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public byte CostumeCount { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public byte RedCostumeIndex { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }

        public byte BlueCostumeIndex { get => _s.GetByte(0x02); set => _s.SetByte(0x02, value); }

        public byte GreenCostumeIndex { get => _s.GetByte(0x03); set => _s.SetByte(0x03, value); }
    }
}
