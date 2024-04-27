using HSDRaw.Common.Animation;
using HSDRaw.Melee.Cmd;

namespace HSDRaw.AirRide
{
    public class KAR_Weapon : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public KAR_ItemWeaponCommonAttributes CommonAttributes { get => _s.GetReference<KAR_ItemWeaponCommonAttributes>(0x00); set => _s.SetReference(0x00, value); }

        public HSDAccessor UniqueAttributes { get => _s.GetReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }

        public KAR_ItemModel Model { get => _s.GetReference<KAR_ItemModel>(0x08); set => _s.SetReference(0x08, value); }

        public HSDArrayAccessor<KAR_WeaponAnimation> ActionStates { get => _s.GetReference<HSDArrayAccessor<KAR_WeaponAnimation>>(0x0C); set => _s.SetReference(0x0C, value); }

        public KAR_ItemCollisions Collisions { get => _s.GetReference<KAR_ItemCollisions>(0x10); set => _s.SetReference(0x10, value); }

        public KAR_WeaponUnknown Unknown { get => _s.GetReference<KAR_WeaponUnknown>(0x14); set => _s.SetReference(0x14, value); }

    }

    public class KAR_ItemWeaponCommonAttributes : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public float x00 { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float x04 { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float x08 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public int x0C { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }
    }

    public class KAR_WeaponUnknown : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public float x00 { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float x04 { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float x08 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float x0C { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
    }

    public class KAR_WeaponAnimation : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSD_AnimJoint JointAnim { get => _s.GetReference<HSD_AnimJoint>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_MatAnimJoint MatAnim { get => _s.GetReference<HSD_MatAnimJoint>(0x04); set => _s.SetReference(0x04, value); }

        public KAR_WpScript AnimScript { get => _s.GetReference<KAR_WpScript>(0x08); set => _s.SetReference(0x08, value); }

        public int Flags { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }
    }

    public class KAR_WpScript : SBM_FighterSubactionData
    {
    }
}
