using HSDRaw.Common;
using HSDRaw.MEX.Sounds;

namespace HSDRaw.MEX
{
    public class MEX_BGMStruct : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSDFixedLengthPointerArrayAccessor<HSD_String> BackgroundMusicStrings { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_String>>(0x00); set => _s.SetReference(0x00, value); }

        public HSDArrayAccessor<MEX_PlaylistItem> MenuPlaylist { get => _s.GetReference<HSDArrayAccessor<MEX_PlaylistItem>>(0x04); set => _s.SetReference(0x04, value); }

        public int MenuPlayListCount { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
    }
}
