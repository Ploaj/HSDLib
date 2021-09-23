namespace HSDRaw.AirRide.Vc
{
    public class KAR_vcDataCommon : HSDAccessor
    {
        public override int TrimmedSize => 0x44;

        public HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<KAR_vcCommonSoundEntry>> VehicleSoundTable
        {
            get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<KAR_vcCommonSoundEntry>>>(0x10);
            set => _s.SetReference(0x10, value);
        }
    }

    public class KAR_vcCommonSoundEntry : HSDAccessor
    {
        public override int TrimmedSize => 0x94;

        public int SFXx00 { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int SFXx04 { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int SFXx08 { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int SFXx0C { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public int SFXx10 { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public int SFXx14 { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        public int SFXx18 { get => _s.GetInt32(0x18); set => _s.SetInt32(0x18, value); }

        public int SFXx1C { get => _s.GetInt32(0x1C); set => _s.SetInt32(0x1C, value); }

        public int SFXx20 { get => _s.GetInt32(0x20); set => _s.SetInt32(0x20, value); }

        public int SFXx24 { get => _s.GetInt32(0x24); set => _s.SetInt32(0x24, value); }

        public int SFXx28 { get => _s.GetInt32(0x28); set => _s.SetInt32(0x28, value); }

        public int SFXx2C { get => _s.GetInt32(0x2C); set => _s.SetInt32(0x2C, value); }

        public int SFXx30 { get => _s.GetInt32(0x30); set => _s.SetInt32(0x30, value); }

    }
}
