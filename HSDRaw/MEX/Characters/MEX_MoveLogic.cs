using System;
using System.ComponentModel;

namespace HSDRaw.MEX
{
    [Flags]
    public enum FIGHTER_ASC_FLAGS : uint
    {
        FIGHTER_FASTFALL_PRESERVE = 0x1,
        FIGHTER_GFX_PRESERVE = 0x2,
        FIGHTER_HITSTATUS_COLANIM_PRESERVE = 0x4,
        FIGHTER_HIT_NOUPDATE = 0x8,
        FIGHTER_MODEL_NOUPDATE = 0x10,
        FIGHTER_ANIMVEL_NOUPDATE = 0x20,
        FIGHTER_UNK_0x40 = 0x40,
        FIGHTER_MATANIM_NOUPDATE = 0x80,
        FIGHTER_THROW_EXCEPTION_NOUPDATE = 0x100,
        FIGHTER_SFX_PRESERVE = 0x200,
        FIGHTER_PARASOL_NOUPDATE = 0x400,
        FIGHTER_RUMBLE_NOUPDATE = 0x800,
        FIGHTER_COLANIM_NOUPDATE = 0x1000,
        FIGHTER_ACCESSORY_PRESERVE = 0x2000,
        FIGHTER_CMD_UPDATE = 0x4000,
        FIGHTER_NAMETAGVIS_NOUPDATE = 0x8000,
        FIGHTER_PART_HITSTATUS_COLANIM_PRESERVE = 0x10000,
        FIGHTER_SWORDTRAIL_PRESERVE = 0x20000,
        FIGHTER_ITEMVIS_NOUPDATE = 0x40000,
        FIGHTER_SKIP_UNK_0x2222 = 0x80000,
        FIGHTER_PHYS_UNKUPDATE = 0x100000,
        FIGHTER_FREEZESTATE = 0x200000,
        FIGHTER_MODELPART_VIS_NOUPDATE = 0x400000,
        FIGHTER_METALB_NOUPDATE = 0x800000,
        FIGHTER_UNK_0x1000000 = 0x1000000,
        FIGHTER_ATTACKCOUNT_NOUPDATE = 0x2000000,
        FIGHTER_MODEL_FLAG_NOUPDATE = 0x4000000,
        FIGHTER_UNK_0x2227 = 0x8000000,
        FIGHTER_HITSTUN_FLAG_NOUPDATE = 0x10000000,
        FIGHTER_ANIM_NOUPDATE = 0x20000000,
        FIGHTER_UNK_0x40000000 = 0x40000000,
        FIGHTER_UNK_0x80000000 = 0x80000000,
    }

    public class MEX_MoveLogic : HSDAccessor
    {
        public override int TrimmedSize => 0x20;

        public int AnimationID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        [TypeConverter(typeof(HexType))]
        public uint StateFlags { get => (uint)_s.GetInt32(0x04); set => _s.SetInt32(0x04, (int)value); }

        public byte AttackID { get => _s.GetByte(0x08); set => _s.SetByte(0x08, value); }

        public byte BitFlags { get => _s.GetByte(0x09); set => _s.SetByte(0x09, value); }

        [TypeConverter(typeof(HexType))]
        public uint AnimationCallBack { get => (uint)_s.GetInt32(0x0C); set => _s.SetInt32(0x0C, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint IASACallBack { get => (uint)_s.GetInt32(0x10); set => _s.SetInt32(0x10, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint PhysicsCallback { get => (uint)_s.GetInt32(0x14); set => _s.SetInt32(0x14, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint CollisionCallback { get => (uint)_s.GetInt32(0x18); set => _s.SetInt32(0x18, (int)value); }

        [TypeConverter(typeof(HexType))]
        public uint CameraCallback { get => (uint)_s.GetInt32(0x1C); set => _s.SetInt32(0x1C, (int)value); }

        public override string ToString()
        {
            return $"{AnimationID} {AttackID} {StateFlags.ToString("X8")}";
        }
    }
}
