using HSDRaw.Common;

namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grFGMNode : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSDFixedLengthPointerArrayAccessor<KAR_grFGMNodeEntry> Entry1 { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<KAR_grFGMNodeEntry>>(0x00); set => _s.SetReference(0x00, value); }

        public int Count1 { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public HSDFixedLengthPointerArrayAccessor<KAR_grFGMNodeEntry> Entry2 { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<KAR_grFGMNodeEntry>>(0x08); set => _s.SetReference(0x08, value); }

        public int Count2 { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

    }

    public class KAR_grFGMNodeEntry : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public HSDArrayAccessor<KAR_grFGMNodeSound> Entries { get => _s.GetReference<HSDArrayAccessor<KAR_grFGMNodeSound>>(0x00); set => _s.SetReference(0x00, value); }

        public int EntryCount { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int x08 { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int Type { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public float Distance { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        public HSD_Vector3 Position
        {
            get
            {
                if (Type != 1)
                    return null;

                return _s.GetReference<HSD_Vector3>(0x14);
            }
            set
            {
                Type = 1;
                _s.SetReference(0x14, value);
            }
        }

        public HSD_Spline Spline
        {
            get
            {
                if (Type != 1)
                    return null;

                return _s.GetReference<HSD_Spline>(0x14);
            }
            set
            {
                Type = 2;
                _s.SetReference(0x14, value);
            }
        }
    }

    public class KAR_grFGMNodeSound : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public ushort SFXBank { get => _s.GetUInt16(0x00); set => _s.SetUInt16(0x00, value); }

        public ushort SFXId { get => _s.GetUInt16(0x02); set => _s.SetUInt16(0x02, value); }

        public uint Flags { get => (uint)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }

    }
}
