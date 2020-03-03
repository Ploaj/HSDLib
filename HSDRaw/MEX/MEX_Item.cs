﻿using System.ComponentModel;

namespace HSDRaw.MEX
{
    public class MEX_Item : HSDAccessor
    {
        public override int TrimmedSize => 0x3C;

        public HSDArrayAccessor<MEX_ItemStateInfo> ItemStates { get => _s.GetReference<HSDArrayAccessor<MEX_ItemStateInfo>>(0x00); set => _s.SetReference(0x00, value); }

        [TypeConverter(typeof(HexType))]
        public int OnSpawn { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        [TypeConverter(typeof(HexType))]
        public int OnDestroy { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        [TypeConverter(typeof(HexType))]
        public int OnPickup { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        [TypeConverter(typeof(HexType))]
        public int OnDrop { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        [TypeConverter(typeof(HexType))]
        public int OnThrow { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        [TypeConverter(typeof(HexType))]
        public int OnGiveDamage { get => _s.GetInt32(0x18); set => _s.SetInt32(0x18, value); }

        [TypeConverter(typeof(HexType))]
        public int OnTakeDamage { get => _s.GetInt32(0x1C); set => _s.SetInt32(0x1C, value); }

        [TypeConverter(typeof(HexType))]
        public int OnEnterAir { get => _s.GetInt32(0x20); set => _s.SetInt32(0x20, value); }

        [TypeConverter(typeof(HexType))]
        public int OnReflect { get => _s.GetInt32(0x24); set => _s.SetInt32(0x24, value); }

        [TypeConverter(typeof(HexType))]
        public int OnUnknown1 { get => _s.GetInt32(0x28); set => _s.SetInt32(0x28, value); }

        [TypeConverter(typeof(HexType))]
        public int OnUnknown2 { get => _s.GetInt32(0x2C); set => _s.SetInt32(0x2C, value); }

        [TypeConverter(typeof(HexType))]
        public int OnHitShieldBounce { get => _s.GetInt32(0x30); set => _s.SetInt32(0x30, value); }

        [TypeConverter(typeof(HexType))]
        public int OnHitShieldDetermineDestroy { get => _s.GetInt32(0x34); set => _s.SetInt32(0x34, value); }

        [TypeConverter(typeof(HexType))]
        public int OnUnknown3 { get => _s.GetInt32(0x38); set => _s.SetInt32(0x38, value); }
    }

    public class MEX_ItemStateInfo : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public int Unknown { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        [TypeConverter(typeof(HexType))]
        public int AnimationCallback { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        [TypeConverter(typeof(HexType))]
        public int PhysicsCallback { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        [TypeConverter(typeof(HexType))]
        public int CollisionCallback { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }
    }
}