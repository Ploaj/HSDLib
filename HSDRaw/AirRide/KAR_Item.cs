using HSDRaw.AirRide.Vc;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Cmd;
using System;
using System.Collections.Generic;
using System.Text;

namespace HSDRaw.AirRide
{
    public class KAR_Item : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public KAR_ItemCommonAttributes CommonAttributes { get => _s.GetReference<KAR_ItemCommonAttributes>(0x00); set => _s.SetReference(0x00, value); }

        public HSDAccessor UniqueAttributes { get => _s.GetReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }

        public KAR_ItemModel Model { get => _s.GetReference<KAR_ItemModel>(0x08); set => _s.SetReference(0x08, value); }

        public HSDArrayAccessor<KAR_ItemAnimation> AnimatonBanks { get => _s.GetReference<HSDArrayAccessor<KAR_ItemAnimation>>(0x0C); set => _s.SetReference(0x0C, value); }

        public KAR_ItemCollisions Collisions { get => _s.GetReference<KAR_ItemCollisions>(0x10); set => _s.SetReference(0x10, value); }

        public KAR_vcCollisionSphere BoundingSphere { get => _s.GetReference<KAR_vcCollisionSphere>(0x14); set => _s.SetReference(0x14, value); }

    }

    public class KAR_ItemCommonAttributes : HSDAccessor
    {
        public override int TrimmedSize => 0x2C;
    }

    public class KAR_ItemModel : HSDAccessor
    {
        public override int TrimmedSize => 0x28;

        public HSD_JOBJ RootJoint { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public int JointCount { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }

    public class KAR_ItemCollisions : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public HSDArrayAccessor<KAR_vcCollisionSphere> Spheres { get => _s.GetReference<HSDArrayAccessor<KAR_vcCollisionSphere>>(0x00); set => _s.SetReference(0x00, value); }

        public int Count { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }

    public class KAR_ItemAnimation : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSD_AnimJoint JointAnim { get => _s.GetReference<HSD_AnimJoint>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_MatAnimJoint MatAnim { get => _s.GetReference<HSD_MatAnimJoint>(0x04); set => _s.SetReference(0x04, value); }

        public KAR_ItScript AnimScript { get => _s.GetReference<KAR_ItScript>(0x08); set => _s.SetReference(0x08, value); }

        public float JointCount { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }
    }

    public class KAR_ItScript : SBM_FighterSubactionData
    {
    }
}
