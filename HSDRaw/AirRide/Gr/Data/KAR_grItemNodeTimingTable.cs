using System;
using System.Collections.Generic;
using System.Text;

namespace HSDRaw.AirRide.Gr.Data
{


    public class KAR_grItemNodeTimingTable : HSDAccessor
    {
        public override int TrimmedSize => 0x1C;

        public int x00 { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public HSDArrayAccessor<KAR_grItemNodeTimingEntry> TimingTable { get => _s.GetReference<HSDArrayAccessor<KAR_grItemNodeTimingEntry>>(0x04); set => _s.SetReference(0x04, value); }

        public int TimingTableCount { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public HSDArrayAccessor<KAR_grItemNodeTimingPositionLookup> PositionTable { get => _s.GetReference<HSDArrayAccessor<KAR_grItemNodeTimingPositionLookup>> (0x0C); set => _s.SetReference(0x0C, value); }

        public int PositionCount { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public HSDArrayAccessor<KAR_grItemNodeTimingPositionLookup> AreaTable { get => _s.GetReference<HSDArrayAccessor<KAR_grItemNodeTimingPositionLookup>>(0x14); set => _s.SetReference(0x14, value); }

        public int AreaCount { get => _s.GetInt32(0x18); set => _s.SetInt32(0x18, value); }
    }

    public class KAR_grItemNodeTimingEntry : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        /// <summary>
        /// Timing 0-1.1 of the match progression
        /// For example, 0.3 means 30% of the match time has passed
        /// </summary>
        public float Timing { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        /// <summary>
        /// Maximum number of items that are allowed to be spawned
        /// </summary>
        public int MaxItemCount { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        /// <summary>
        /// Minimum wait time before spawning new item
        /// </summary>
        public int IntervalMin { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        /// <summary>
        /// Maximum wait time before spawning new item
        /// </summary>
        public int IntervalMax { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }
    }

    public class KAR_grItemNodeTimingPositionLookup : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        /// <summary>
        /// Index in respecitive Position table to use
        /// </summary>
        public int Index { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        /// <summary>
        /// Selection based flags
        /// </summary>
        public byte SelectionFlags { get => _s.GetByte(0x04); set => _s.SetByte(0x04, value); }

        /// <summary>
        /// Spawn based flags
        /// </summary>
        public int SpawnFlags { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        /// <summary>
        /// Weighted chance that this location will be selected
        /// </summary>
        public int Chance { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }
    }
}
