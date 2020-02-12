namespace HSDRaw.Melee.Pl
{
    public class SBM_PlayerModelLookupTables : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public int TableCounts { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int MOBJIndexCount { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int Unknown1 { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }
        public int Unknown2 { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }
    }

    public class SBM_LookupTable : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public int Count { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }
        
    }
}
