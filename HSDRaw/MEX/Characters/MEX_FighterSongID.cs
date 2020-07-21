namespace HSDRaw.MEX.Characters
{
    public class MEX_FighterSongID : HSDAccessor
    {
        public override int TrimmedSize => 0x03;

        public short SongID { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }

        public byte Unknown { get => _s.GetByte(0x02); set => _s.SetByte(0x02, value); }

    }
}
