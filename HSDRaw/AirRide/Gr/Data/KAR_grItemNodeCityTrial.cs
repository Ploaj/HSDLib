namespace HSDRaw.AirRide.Gr.Data
{

    public class KAR_grItemNodeCityTrial : HSDAccessor
    {
        public override int TrimmedSize => 0x28;

        public KAR_grItemNodeCityTrialBoxSpawnTable BoxSpawnTable { get => _s.GetReference<KAR_grItemNodeCityTrialBoxSpawnTable>(0x00); set => _s.SetReference(0x00, value); }

        public HSDArrayAccessor<KAR_grItemNodeCityTrialCommonSpawnTable> ItemSpawnChanceTable { get => _s.GetReference<HSDArrayAccessor<KAR_grItemNodeCityTrialCommonSpawnTable>>(0x04); set => _s.SetReference(0x04, value); }
        
        public int ItemSpawnChanceTableCount { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
        
        public HSDArrayAccessor<KAR_grItemNodeCityTrialPlayerTable> PlayerTable { get => _s.GetReference<HSDArrayAccessor<KAR_grItemNodeCityTrialPlayerTable>>(0x0C); set => _s.SetReference(0x0C, value); }

        // x10 TODO: I don't know what this structure is for

        public int x10Count { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        public HSDArrayAccessor<KAR_grItemNodeCityTrialUnknownTable> UnknownTable { get => _s.GetReference<HSDArrayAccessor<KAR_grItemNodeCityTrialUnknownTable>>(0x18); set => _s.SetReference(0x18, value); }

        public int UnknownCount { get => _s.GetInt32(0x1C); set => _s.SetInt32(0x1C, value); }

        public HSDArrayAccessor<KAR_grItemNodeCityTrialSpecialTiming> SpecialTimingTable { get => _s.GetReference<HSDArrayAccessor<KAR_grItemNodeCityTrialSpecialTiming>>(0x20); set => _s.SetReference(0x20, value); }

        public int SpecialTimingCount { get => _s.GetInt32(0x24); set => _s.SetInt32(0x24, value); }

    }

    public class KAR_grItemNodeCityTrialBoxSpawnTable : HSDAccessor
    {
        public override int TrimmedSize => 0x28;

        public byte BlueSmallChance { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public byte BlueMediumChance { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }

        public byte BlueLargeChance { get => _s.GetByte(0x02); set => _s.SetByte(0x02, value); }

        public byte GreenSmallChance { get => _s.GetByte(0x03); set => _s.SetByte(0x03, value); }

        public byte GreenMediumChance { get => _s.GetByte(0x04); set => _s.SetByte(0x04, value); }

        public byte GreenLargeChance { get => _s.GetByte(0x05); set => _s.SetByte(0x05, value); }

        public byte RedSmallChance { get => _s.GetByte(0x06); set => _s.SetByte(0x06, value); }

        public byte RedMediumChance { get => _s.GetByte(0x07); set => _s.SetByte(0x07, value); }

        public byte RedLargeChance { get => _s.GetByte(0x08); set => _s.SetByte(0x08, value); }

        // there are 3 rhythm tables for item or box spawn
        // the order is blue, green, red
        // there are 9 bytes per entry and 01 is spawn box, FF is reset, and 00 is item
    }

    public class KAR_grItemNodeCityTrialCommonSpawnTable : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public int ItemID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }


        public short ChanceType0 { get => _s.GetInt16(0x04); set => _s.SetInt16(0x04, value); }

        public short ChanceType1 { get => _s.GetInt16(0x06); set => _s.SetInt16(0x06, value); }

        public short ChanceType2 { get => _s.GetInt16(0x08); set => _s.SetInt16(0x08, value); }

        public short ChanceType3 { get => _s.GetInt16(0x0A); set => _s.SetInt16(0x0A, value); }

        public short ChanceType4 { get => _s.GetInt16(0x0C); set => _s.SetInt16(0x0C, value); }

        public short ChanceType5 { get => _s.GetInt16(0x0E); set => _s.SetInt16(0x0E, value); }

        public short ChanceType6 { get => _s.GetInt16(0x10); set => _s.SetInt16(0x10, value); }

        public short ChanceType7 { get => _s.GetInt16(0x12); set => _s.SetInt16(0x12, value); }


        public short BoxSpawnChance1 { get => _s.GetInt16(0x14); set => _s.SetInt16(0x14, value); }

        public short BoxSpawnChance2 { get => _s.GetInt16(0x16); set => _s.SetInt16(0x16, value); }

    }

    public class KAR_grItemNodeCityTrialPlayerTable : HSDAccessor
    {
        public override int TrimmedSize => 0x0E;

        public byte Chance1 { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public byte P2ChanceMin { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }

        public byte P2ChanceMax { get => _s.GetByte(0x02); set => _s.SetByte(0x02, value); }

        public byte P3ChanceMin { get => _s.GetByte(0x03); set => _s.SetByte(0x03, value); }

        public byte P3ChanceMax { get => _s.GetByte(0x04); set => _s.SetByte(0x04, value); }

        public byte P4ChanceMin { get => _s.GetByte(0x05); set => _s.SetByte(0x05, value); }

        public byte P4ChanceMax { get => _s.GetByte(0x06); set => _s.SetByte(0x06, value); }


        public byte Chance2 { get => _s.GetByte(0x07); set => _s.SetByte(0x07, value); }

        public byte P2ChanceMin2 { get => _s.GetByte(0x08); set => _s.SetByte(0x08, value); }

        public byte P2ChanceMax2 { get => _s.GetByte(0x09); set => _s.SetByte(0x09, value); }

        public byte P3ChanceMin2 { get => _s.GetByte(0x0A); set => _s.SetByte(0x0A, value); }

        public byte P3ChanceMax2 { get => _s.GetByte(0x0B); set => _s.SetByte(0x0B, value); }

        public byte P4ChanceMin2 { get => _s.GetByte(0x0C); set => _s.SetByte(0x0C, value); }

        public byte P4ChanceMax2 { get => _s.GetByte(0x0D); set => _s.SetByte(0x0D, value); }


    }

    public class KAR_grItemNodeCityTrialUnknownTable : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public int ItemID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public short ChanceType0 { get => _s.GetInt16(0x04); set => _s.SetInt16(0x04, value); }

        public short ChanceType1 { get => _s.GetInt16(0x06); set => _s.SetInt16(0x06, value); }

        public short ChanceType2 { get => _s.GetInt16(0x08); set => _s.SetInt16(0x08, value); }

        public short ChanceType3 { get => _s.GetInt16(0x0A); set => _s.SetInt16(0x0A, value); }

        public short ChanceType4 { get => _s.GetInt16(0x0C); set => _s.SetInt16(0x0C, value); }

        public short ChanceType5 { get => _s.GetInt16(0x0E); set => _s.SetInt16(0x0E, value); }

    }

    public class KAR_grItemNodeCityTrialSpecialTiming : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        /// <summary>
        /// 
        /// </summary>
        public byte EventID { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        /// <summary>
        /// Maximum number of items that are allowed to be spawned
        /// </summary>
        public int MaxItemCount { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        /// <summary>
        /// Minimum wait time before spawning new item
        /// </summary>
        public int IntervalMin { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        /// <summary>
        /// Maximum wait time before spawning new item
        /// </summary>
        public int IntervalMax { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }


    }
}
