using HSDRaw.Common;

namespace HSDRaw.AirRide.Db
{
    public class KAR_smSoundTestFGMGroupTable : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public int SSMIndex { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int x04 { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int x08 { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public string Name { get => _s.GetString(0x0C); set => _s.SetString(0x0C, value); }

        public int SoundCount { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public HSDFixedLengthPointerArrayAccessor<HSD_String> SoundNames { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_String>>(0x14); set => _s.SetReference(0x14, value); }
    }
}
