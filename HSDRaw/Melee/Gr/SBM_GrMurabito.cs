using HSDRaw.Common;
using HSDRaw.Common.Animation;
using System;

namespace HSDRaw.Melee.Gr
{
    [Flags]
    public enum MurabitoFlag
    {
        HUMAN = (1 << 0),
        SLEEP = (1 << 1),
        BLANCA = (1 << 2),
        NOEMOTE = (1 << 3),
        ELDER = (1 << 4),
    }

    public class SBM_GrMurabito : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSD_JOBJ Joint { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_MatAnimJoint MatAnim { get => _s.GetReference<HSD_MatAnimJoint>(0x04); set => _s.SetReference(0x04, value); }

        public MurabitoFlag Flags { get => (MurabitoFlag)_s.GetInt16(0x08); set => _s.SetInt16(0x08, (short)value); }

        public ushort SpawnChance { get => _s.GetUInt16(0x0A); set => _s.SetUInt16(0x0A, value); }

        public ushort FriendSpawnChance { get => _s.GetUInt16(0x0C); set => _s.SetUInt16(0x0C, value); }

        public ushort FriendId { get => _s.GetUInt16(0x0E); set => _s.SetUInt16(0x0E, value); }
    }
}
