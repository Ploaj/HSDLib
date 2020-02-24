using HSDRaw.Common;

namespace HSDRaw.MEX
{
    public class MEX_BGMStruct : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSDFixedLengthPointerArrayAccessor<HSD_String> BackgroundMusicStrings { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_String>>(0x00); set => _s.SetReference(0x00, value); }

        public HSDArrayAccessor<MEX_MenuPlaylistItem> MenuPlaylist { get => _s.GetReference<HSDArrayAccessor<MEX_MenuPlaylistItem>>(0x04); set => _s.SetReference(0x04, value); }

        public int MenuPlayListCount { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
    }

    public class MEX_MenuPlaylistItem : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public ushort HPSID { get => (ushort)_s.GetInt16(0x00); set => _s.SetInt16(0x00, (short)value); }

        public short ChanceToPlay { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }
    }
}
