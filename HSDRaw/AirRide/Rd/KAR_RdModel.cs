using HSDRaw.Common;
using HSDRaw.Common.Animation;

namespace HSDRaw.AirRide.Rd
{
    public class KAR_RdModel : HSDAccessor
    {
        public override int TrimmedSize => 0x30;

        public HSD_JOBJ Model { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_MatAnimJoint MaterialAnim { get => _s.GetReference<HSD_MatAnimJoint>(0x04); set => _s.SetReference(0x04, value); }

        public byte BoneCount { get => _s.GetByte(0x08); set => _s.SetByte(0x08, value); }

        public byte x09 { get => _s.GetByte(0x09); set => _s.SetByte(0x09, value); }

        public byte x0A { get => _s.GetByte(0x0A); set => _s.SetByte(0x0A, value); }

        public int LODTableCount { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public HSDArrayAccessor<KAR_LODTableCollection> HighPolyTable { get => _s.GetReference< HSDArrayAccessor<KAR_LODTableCollection>>(0x10); set => _s.SetReference(0x10, value); }

        public HSDArrayAccessor<KAR_LODTableCollection> MidPolyTable { get => _s.GetReference< HSDArrayAccessor<KAR_LODTableCollection>>(0x14); set => _s.SetReference(0x14, value); }

        public HSDArrayAccessor<KAR_LODTableCollection> LowPolyTable { get => _s.GetReference< HSDArrayAccessor<KAR_LODTableCollection>>(0x18); set => _s.SetReference(0x18, value); }

        public HSDArrayAccessor<KAR_RdTextureTable> TextureTable { get => _s.GetReference< HSDArrayAccessor<KAR_RdTextureTable>>(0x1C); set => _s.SetReference(0x1C, value); }

        public int TextureTableCount { get => _s.GetInt32(0x20); set => _s.SetInt32(0x20, value); }

        public byte x24 { get => _s.GetByte(0x24); set => _s.SetByte(0x24, value); }
        public byte x25 { get => _s.GetByte(0x25); set => _s.SetByte(0x25, value); }
        public byte x26 { get => _s.GetByte(0x26); set => _s.SetByte(0x26, value); }
        public byte x27 { get => _s.GetByte(0x27); set => _s.SetByte(0x27, value); }

        public byte x28 { get => _s.GetByte(0x28); set => _s.SetByte(0x28, value); }
        public byte x29 { get => _s.GetByte(0x29); set => _s.SetByte(0x29, value); }
        public byte x2A { get => _s.GetByte(0x2A); set => _s.SetByte(0x2A, value); }
        public byte x2B { get => _s.GetByte(0x2B); set => _s.SetByte(0x2B, value); }
    }

    public class KAR_RdTextureTable : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int Count { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public HSDArrayAccessor<KAR_RdTextureEntry> Entries { get => _s.GetReference<HSDArrayAccessor<KAR_RdTextureEntry>>(0x04); set => _s.SetReference(0x04, value); }
    }

    public class KAR_RdTextureEntry : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int Count { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public HSDShortArray Entries { get => _s.GetReference<HSDShortArray>(0x04); set => _s.SetReference(0x04, value); }

    }
}
