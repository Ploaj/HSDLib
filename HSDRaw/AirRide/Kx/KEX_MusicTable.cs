using HSDRaw.Common;

namespace HSDRaw.AirRide.Kx
{
    public class KEX_MusicTable : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public int MusicCount { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public HSDArrayAccessor<KEX_MusicEntry> FilePath { get => _s.GetReference<HSDArrayAccessor<KEX_MusicEntry>>(0x04); set => _s.SetReference(0x04, value); }

        public KEX_Playlist MenuPlaylist { get => _s.GetReference<KEX_Playlist>(0x08); set => _s.SetReference(0x08, value); }

    }

    public class KEX_MusicEntry : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public int ID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int x04 { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public string FilePath { get => _s.GetString(0x08); set => _s.SetString(0x08, value); }

        public int x0C { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }
    }

    public class KEX_Playlist : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public int EntryCount { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public HSDArrayAccessor<KEX_PlaylistEntry> Entries { get => _s.GetReference<HSDArrayAccessor<KEX_PlaylistEntry>>(0x04); set => _s.SetReference(0x04, value); }

    }

    public class KEX_PlaylistEntry : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public byte PlayChance { get => _s.GetByte(0x00); set => _s.SetByte(0x00, (byte)(value > 100 ? 100 : value)); }

        public int BGMIndex { get => _s.GetInt32(0x00) & 0xFFFFFF; set => _s.SetInt32(0x00, (value & 0xFFFFFF) | ((PlayChance & 0xFF) << 24)); }
    }
}
