using HSDRaw.Common;
using HSDRaw.Common.Animation;

namespace HSDRaw.AirRide.Em
{
    public class KAR_emData : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public KAR_emEntry EnemyEntry { get => _s.GetReference<KAR_emEntry>(0x00); set => _s.SetReference(0x00, value); }

        public KAR_emEntry EnemyEntryAlt { get => _s.GetReference<KAR_emEntry>(0x04); set => _s.SetReference(0x04, value); }

        public KAR_emEntry EnemyEntryAlt2 { get => _s.GetReference<KAR_emEntry>(0x08); set => _s.SetReference(0x08, value); }

        public KAR_emEntry EnemyEntryAlt3 { get => _s.GetReference<KAR_emEntry>(0x0C); set => _s.SetReference(0x0C, value); }

    }

    public class KAR_emEntry : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public KAR_emAttributes CommonAttributes { get => _s.GetReference<KAR_emAttributes>(0x00); set => _s.SetReference(0x00, value); }

        public HSDAccessor UniqueAttributes { get => _s.GetReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }

        public KAR_emModel Model { get => _s.GetReference<KAR_emModel>(0x08); set => _s.SetReference(0x08, value); }

        public HSDArrayAccessor<KAR_emAnim> AnimBank { get => _s.GetReference<HSDArrayAccessor<KAR_emAnim>>(0x0C); set => _s.SetReference(0x0C, value); }

        public KAR_UnkCollisionGroup CollisionGroup { get => _s.GetReference<KAR_UnkCollisionGroup>(0x10); set => _s.SetReference(0x10, value); }

        public KAR_UnkCollision CollisionSphere { get => _s.GetReference<KAR_UnkCollision>(0x14); set => _s.SetReference(0x14, value); }
    }

    public class KAR_emAttributes : HSDAccessor
    {
        public override int TrimmedSize => 0xA4;


    }

    public class KAR_emModel : HSDAccessor
    {
        public override int TrimmedSize => 0x28;

        public HSD_JOBJ RootJoint { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public int BoneCount { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int x08 { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int x0C { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public int x10 { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        public KAR_LODTableCollection HighLOD { get => _s.GetReference<KAR_LODTableCollection>(0x14); set => _s.SetReference(0x14, value); }

        public KAR_LODTableCollection MidLOD { get => _s.GetReference<KAR_LODTableCollection>(0x18); set => _s.SetReference(0x18, value); }

        public KAR_LODTableCollection LowLOD { get => _s.GetReference<KAR_LODTableCollection>(0x1C); set => _s.SetReference(0x1C, value); }

        public int x20 { get => _s.GetInt32(0x20); set => _s.SetInt32(0x20, value); }

        public int x24 { get => _s.GetInt32(0x24); set => _s.SetInt32(0x24, value); }
    }

    public class KAR_emAnim : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSD_AnimJoint JointAnim { get => _s.GetReference<HSD_AnimJoint>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_MatAnimJoint MatAnim { get => _s.GetReference<HSD_MatAnimJoint>(0x04); set => _s.SetReference(0x04, value); }

        
    }

}
