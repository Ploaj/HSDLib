namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grItemGeneralSpawnTable : HSDAccessor
    {
        public override int TrimmedSize => 0xC;

        public HSDIntArray x00 { get => _s.GetReference<HSDIntArray>(0x00); set => _s.SetReference(0x00, value); }

        public HSDArrayAccessor<KAR_grItemGeneralSpawnItem> Items { get => _s.GetReference<HSDArrayAccessor<KAR_grItemGeneralSpawnItem>>(0x04); set => _s.SetReference(0x04, value); }

        public int Count { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
    }

    public class KAR_grItemGeneralSpawnItem : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public int ItemID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int Chance1 { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int Chance2 { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int Chance3 { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }
    }
}
