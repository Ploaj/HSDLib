namespace HSDRaw.Melee.Ty
{
    public class SBM_tyDisplayModelEntry : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public int TrophyID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }
        public byte Trophy2DFileIndex { get => _s.GetByte(0x04); set => _s.SetByte(0x04, value); }
        public byte Trophy2DImageIndex { get => _s.GetByte(0x05); set => _s.SetByte(0x05, value); }
        public float OffsetX { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
        public float OffsetY { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
    }

    public class SBM_tyInitModelEntry : HSDAccessor
    {
        public override int TrimmedSize => 0x24;

        public int TrophyID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }
        public int TrophyType { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
        public float OffsetX { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }
        public float OffsetY { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
        public float OffsetZ { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
        public float ModelScale { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }
        public float StandScale { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }
        public float YRotation { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }
        public byte x20 { get => _s.GetByte(0x20); set => _s.SetByte(0x20, value); }
        public byte x21 { get => _s.GetByte(0x21); set => _s.SetByte(0x21, value); }
    }

    public class SBM_tyModelSortEntry : HSDAccessor
    {
        public override int TrimmedSize => 0xC;

        public short TrophyID { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }
        public short x02 { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }
        public short x04 { get => _s.GetInt16(0x04); set => _s.SetInt16(0x04, value); }
        public short x06 { get => _s.GetInt16(0x06); set => _s.SetInt16(0x06, value); }
        public short x08 { get => _s.GetInt16(0x08); set => _s.SetInt16(0x08, value); }
        public short x0A { get => _s.GetInt16(0x0A); set => _s.SetInt16(0x0A, value); }

    }
}
