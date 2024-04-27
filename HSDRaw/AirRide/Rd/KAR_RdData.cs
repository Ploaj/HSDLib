using System;
using System.Collections.Generic;
using System.Text;

namespace HSDRaw.AirRide.Rd
{
    public class KAR_RdData : HSDAccessor
    {
        public override int TrimmedSize => 0x40;

        public HSDAccessor Attributes { get => _s.GetReference<HSDAccessor>(0x00); set => _s.SetReference(0x00, value); }

        public KAR_RdModel ModelData { get => _s.GetReference<KAR_RdModel>(0x04); set => _s.SetReference(0x04, value); }

        public KAR_RdUnknownTable0x08 x08 { get => _s.GetReference<KAR_RdUnknownTable0x08>(0x08); set => _s.SetReference(0x08, value); }

        public HSDArrayAccessor<KAR_RdMotionTable> MotionTables { get => _s.GetReference<HSDArrayAccessor<KAR_RdMotionTable>>(0x0C); set => _s.SetReference(0x0C, value); }

        public KAR_UnkCollisionGroup x10 { get => _s.GetReference<KAR_UnkCollisionGroup>(0x10); set => _s.SetReference(0x10, value); }

        public KAR_RdUnknownTable0x14 x14 { get => _s.GetReference<KAR_RdUnknownTable0x14>(0x14); set => _s.SetReference(0x14, value); }

        public KAR_UnkCollision x18 { get => _s.GetReference<KAR_UnkCollision>(0x18); set => _s.SetReference(0x18, value); }

        public HSDAccessor Attribute2 { get => _s.GetReference<HSDAccessor>(0x1C); set => _s.SetReference(0x1C, value); }

        public KAR_RdWeaponBank Weapons { get => _s.GetReference<KAR_RdWeaponBank>(0x20); set => _s.SetReference(0x20, value); }

        public HSDFixedLengthPointerArrayAccessor<KAR_RdCap> CapData { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<KAR_RdCap>>(0x24); set => _s.SetReference(0x24, value); }

    }

    public class KAR_RdWeaponBank : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public int Count { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public HSDArrayAccessor<KAR_RdWeapon> Weapons { get => _s.GetReference<HSDArrayAccessor<KAR_RdWeapon>>(0x04); set => _s.SetReference(0x04, value); }
    }

    public class KAR_RdWeapon : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int ID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public KAR_Weapon WeaponItem { get => _s.GetReference<KAR_Weapon>(0x04); set => _s.SetReference(0x04, value); }
    }

    public class KAR_RdMotionTable : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int Count { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public HSDArrayAccessor<KAR_RdMotion> Entries { get => _s.GetReference<HSDArrayAccessor<KAR_RdMotion>>(0x04); set => _s.SetReference(0x04, value); }

    }

    public class KAR_RdUnknownTable0x08 : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public HSDByteArray Entries { get => _s.GetReference<HSDByteArray>(0x00); set => _s.SetReference(0x00, value); }

        public int Count { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

    }

    public class KAR_RdUnknownTable0x14 : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public float x00 { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float x04 { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float x08 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float x0C { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

    }

}
