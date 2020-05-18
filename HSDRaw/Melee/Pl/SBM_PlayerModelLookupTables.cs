namespace HSDRaw.Melee.Pl
{
    public class SBM_PlayerModelLookupTables : HSDAccessor
    {
        public override int TrimmedSize => 0x18;
        
        public int TableCounts { get => _s.GetInt32(0x00); internal set => _s.SetInt32(0x00, value); }

        /*public HSDNullPointerArrayAccessor<SBM_LookupTable> LookupTables {
            get => _s.GetReference<HSDNullPointerArrayAccessor<SBM_LookupTable>>(0x04);
            set
            {
                if (value == null || value.Length == 0)
                {
                    _s.SetReference(0x04, null);
                    TableCounts = 0;
                } else
                {
                    _s.SetReference(0x04, value);
                    TableCounts = value.Length;
                }
            }
        }*/
    
        public int MaterialIndexCount { get => _s.GetInt32(0x08); internal set => _s.SetInt32(0x08, value); }
        
        public byte ItemHoldBone { get => _s.GetByte(0x10); set => _s.SetByte(0x10, value); }
        public byte ShieldBone { get => _s.GetByte(0x11); set => _s.SetByte(0x11, value); }
        public byte TopOfHeadBone { get => _s.GetByte(0x12); set => _s.SetByte(0x12, value); }
        public byte LeftFootBone { get => _s.GetByte(0x13); set => _s.SetByte(0x13, value); }
        public byte RightFootBone { get => _s.GetByte(0x14); set => _s.SetByte(0x14, value); }
    }

    public class SBM_LookupTable : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public int Count { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        /*public SBM_LookupEntry[] LookupEntries
        {
            get => _s.GetNullPointerArray<SBM_LookupEntry>(0x04);
            set
            {
                if (value == null || value.Length == 0)
                {
                    _s.SetReference(0x04, null);
                    Count = 0;
                }
                else
                {
                    _s.SetNullPointerArray(0x04, value);
                    Count = value.Length;
                }
            }
        }*/
    }

    public class SBM_LookupEntry : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public int Count { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public byte[] Entries
        {
            get => _s.GetCreateReference<HSDAccessor>(0x4)._s.GetSubData(0, Count);
            set
            {
                if (value == null || value.Length == 0)
                {
                    _s.SetReference(0x04, null);
                    Count = 0;
                }
                else
                {
                    _s.GetCreateReference<HSDAccessor>(0x4)._s.SetData(value);
                    Count = value.Length;
                }
            }
        }
    }
}
