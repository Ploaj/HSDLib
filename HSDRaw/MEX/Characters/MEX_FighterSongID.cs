namespace HSDRaw.MEX.Characters
{
    public class MEX_FighterSongID : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public short SongID1 { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }

        public short SongID2 { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

    }
}
