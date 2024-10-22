using HSDRaw.Tools.Melee;
using System.ComponentModel;

namespace HSDRaw.Melee
{

    public class SBM_CPUCommand : HSDAccessor
    {
        public string Script
        {
            get
            {
                return CPUCommands.Decompile(_s.GetData());
            }
        }
    }

    public class SBM_CPUChance : HSDAccessor
    {
        public override int TrimmedSize => 0x24;

        [Description("Index of script to use inside of CPU table")]
        public int ScriptIndex { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        [Description("Prediction Distance for range to trigger")]
        public int Range { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        [Description("Offset from Fighter Position")]
        public float X1 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        [Description("Offset from Fighter Position")]
        public float X2 { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        [Description("Offset from Fighter Position")]
        public float Y1 { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        [Description("Offset from Fighter Position")]
        public float Y2 { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        [Description("Chance this script will be picked")]
        public float Chance { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        [Description("The interval, in frames, at which this event or action can occur.")]
        public int FrameInterval { get => _s.GetInt32(0x1C); set => _s.SetInt32(0x1C, value); }

        [Description("Minimum CPU Level for this script to occur")]
        public int MinimumCPULevel { get => _s.GetInt32(0x20); set => _s.SetInt32(0x20, value); }
    }

    public class SBM_PlCoCPUTable : HSDAccessor
    {
        public override int TrimmedSize => 0x30;

        /// <summary>
        /// 
        /// </summary>
        public HSDFixedLengthPointerArrayAccessor<SBM_CPUCommand> Scripts 
        { 
            get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<SBM_CPUCommand>>(0x00);
            set => _s.SetReference(0x00, value);
        }
        /// <summary>
        /// General (used during downed and cliff states as well)
        /// </summary>
        public HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<SBM_CPUChance>> GeneralAttacks
        { 
            get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<SBM_CPUChance>>>(0x04);
            set => _s.SetReference(0x04, value);
        }
        /// <summary>
        /// Attack Airborne
        /// </summary>
        public HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<SBM_CPUChance>> AirborneAttacks
        {
            get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<SBM_CPUChance>>>(0x08);
            set => _s.SetReference(0x08, value);
        }
        /// <summary>
        /// Attack From Distance
        /// </summary>
        public HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<SBM_CPUChance>> ProjectileAttacks
        {
            get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<SBM_CPUChance>>>(0x0C);
            set => _s.SetReference(0x0C, value);
        }
        /// <summary>
        /// Grab
        /// </summary>
        public HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<SBM_CPUChance>> GrabAttacks
        {
            get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<SBM_CPUChance>>>(0x10);
            set => _s.SetReference(0x10, value);
        }
        /// <summary>
        /// Used when 0x04 is not selected and we have a target item
        /// </summary>
        public HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<SBM_CPUChance>> ItemAttacks
        {
            get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<SBM_CPUChance>>>(0x14);
            set => _s.SetReference(0x14, value);
        }
        /// <summary>
        /// Grounded Battering Item?
        /// </summary>
        public HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<SBM_CPUChance>> BatteringItemAttacks
        {
            get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<SBM_CPUChance>>>(0x18);
            set => _s.SetReference(0x18, value);
        }
        /// <summary>
        /// Off stage attack
        /// </summary>
        public HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<SBM_CPUChance>> OffStageAttacks
        {
            get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSDArrayAccessor<SBM_CPUChance>>>(0x1C);
            set => _s.SetReference(0x1C, value);
        }
        /// <summary>
        /// Used when checking the distance between the cpu and their target
        /// </summary>
        public HSDFloatArray TargetRadius
        {
            get => _s.GetReference<HSDFloatArray>(0x20);
            set => _s.SetReference(0x20, value);
        }
        /// <summary>
        /// Item ID Order = 0x18 (ITEM_FAN), 0x17 (ITEM_LIPSSTICK), 0x16 (ITEM_STARROD), 0xC (ITEM_BEAMSWORD), 0xB (ITEM_HOMERUNBAT), 0xD (ITEM_PARASOL)
        /// </summary>
        public HSDFloatArray SpecialItemRanges
        {
            get => _s.GetReference<HSDFloatArray>(0x24);
            set => _s.SetReference(0x24, value);
        }
    }
}
