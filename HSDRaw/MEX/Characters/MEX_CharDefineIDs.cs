namespace HSDRaw.MEX
{
    public enum SubCharacterBehavior
    {
        SpawnSimultaneously,
        SpawnSeparately,
        SpawnNormally = 0xFF
    }

    public class MEX_CharDefineIDs : HSDAccessor
    {
        public override int TrimmedSize => 0x03; // Probably the only place this isn't gonna be aligned

        public byte InternalID { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }
        public byte SubCharacterInternalID { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }
        public SubCharacterBehavior SubCharacterBehavior { get => (SubCharacterBehavior)_s.GetByte(0x02); set => _s.SetByte(0x02, (byte)value); }
    }
}
