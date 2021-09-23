namespace HSDRaw.AirRide
{
    public class KAR_stData : HSDAccessor
    {
        public override int TrimmedSize => 0x04;
        public HSDArrayAccessor<KAR_stKind> StageKinds { get => _s.GetReference<HSDArrayAccessor<KAR_stKind>>(0x00); set => _s.SetReference(0x00, value); }

    }

    public class KAR_stKind : HSDAccessor
    {
        public override int TrimmedSize => 0x58;

        public int GroundId { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int BGMId { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int UraBGMId { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int BGMChance { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public int DefBGMId { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public int RenderCam { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        public int Max_Player { get => _s.GetInt32(0x18); set => _s.SetInt32(0x18, value); }

        public int StartId { get => _s.GetInt32(0x1C); set => _s.SetInt32(0x1C, value); }

        public int EnemyId { get => _s.GetInt32(0x20); set => _s.SetInt32(0x20, value); }

        public int ItemId { get => _s.GetInt32(0x24); set => _s.SetInt32(0x24, value); }

        public int itemAreaId { get => _s.GetInt32(0x28); set => _s.SetInt32(0x28, value); }

        public int VehicleId { get => _s.GetInt32(0x2C); set => _s.SetInt32(0x2C, value); }

        public int x30 { get => _s.GetInt32(0x30); set => _s.SetInt32(0x30, value); }

        public int x34 { get => _s.GetInt32(0x34); set => _s.SetInt32(0x34, value); }

        public int hasItem { get => _s.GetInt32(0x38); set => _s.SetInt32(0x38, value); }

        public int hasYakumono { get => _s.GetInt32(0x3C); set => _s.SetInt32(0x3C, value); }

        public HSDShortArray UnknownArray { get => _s.GetReference<HSDShortArray>(0x40); set => _s.SetReference(0x40, value); }

        public int x44 { get => _s.GetInt32(0x44); set => _s.SetInt32(0x44, value); }

        public int x48 { get => _s.GetInt32(0x48); set => _s.SetInt32(0x48, value); }

        public int x4C { get => _s.GetInt32(0x4C); set => _s.SetInt32(0x4C, value); }

        public int x50 { get => _s.GetInt32(0x50); set => _s.SetInt32(0x50, value); }

        public int x54 { get => _s.GetInt32(0x54); set => _s.SetInt32(0x54, value); }
    }
}
