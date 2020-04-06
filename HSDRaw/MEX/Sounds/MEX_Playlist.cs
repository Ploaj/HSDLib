namespace HSDRaw.MEX.Sounds
{
    public class MEX_Playlist : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int MenuPlayListCount { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public HSDArrayAccessor<MEX_PlaylistItem> MenuPlaylist { get => _s.GetReference<HSDArrayAccessor<MEX_PlaylistItem>>(0x04); set => _s.SetReference(0x04, value); }
    }

    public class MEX_PlaylistItem : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public ushort HPSID { get => (ushort)_s.GetInt16(0x00); set => _s.SetInt16(0x00, (short)value); }

        public short ChanceToPlay { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }
    }
}
