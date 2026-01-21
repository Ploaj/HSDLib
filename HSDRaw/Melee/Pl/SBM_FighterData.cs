using HSDRaw.Common;

namespace HSDRaw.Melee.Pl
{
    /// <summary>
    /// 
    /// </summary>
    public class SBM_FighterData : HSDAccessor
    {
        public override int TrimmedSize => 0x60;

        public SBM_CommonFighterAttributes Attributes { get => _s.GetReference<SBM_CommonFighterAttributes>(0x00); set => _s.SetReference(0x00, value); }

        public HSDAccessor Attributes2 { get => _s.GetReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }
        
        public SBM_PlayerModelLookupTables ModelLookupTables { get => _s.GetReference<SBM_PlayerModelLookupTables>(0x08); set => _s.SetReference(0x08, value); }

        public SBM_FighterActionTable FighterActionTable { get => _s.GetReference<SBM_FighterActionTable>(0x0C); set => _s.SetReference(0x0C, value); }

        public HSDArrayAccessor<SBM_DynamicBehavior> FighterActionDynamicBehaviors { get => _s.GetReference<HSDArrayAccessor<SBM_DynamicBehavior>>(0x10); set => _s.SetReference(0x10, value); }

        public SBM_FighterActionTable DemoActionTable { get => _s.GetReference<SBM_FighterActionTable>(0x14); set => _s.SetReference(0x14, value); }

        public HSDArrayAccessor<SBM_DynamicBehavior> DemoActionDynamicBehaviors { get => _s.GetReference<HSDArrayAccessor<SBM_DynamicBehavior>>(0x18); set => _s.SetReference(0x18, value); }

        public HSDFixedLengthPointerArrayAccessor<SBM_ModelPart> ModelPartAnimations { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<SBM_ModelPart>>(0x1C); set => _s.SetReference(0x1C, value); }
        
        public SBM_ShieldModelContainer ShieldPoseContainer { get => _s.GetReference<SBM_ShieldModelContainer>(0x20); set => _s.SetReference(0x20, value); }

        public HSDArrayAccessor<SBM_PlActionChance> IdleActionChances { get => _s.GetReference<HSDArrayAccessor<SBM_PlActionChance>>(0x24); set => _s.SetReference(0x24, value); }

        public HSDArrayAccessor<SBM_PlActionChance> WaitIdleActionChances { get => _s.GetReference<HSDArrayAccessor<SBM_PlActionChance>>(0x28); set => _s.SetReference(0x28, value); }

        public SBM_PhysicsGroup Physics { get => _s.GetReference<SBM_PhysicsGroup>(0x2C); set => _s.SetReference(0x2C, value); }

        public SBM_HurtboxBank<SBM_Hurtbox> Hurtboxes { get => _s.GetReference<SBM_HurtboxBank<SBM_Hurtbox>>(0x30); set => _s.SetReference(0x30, value); }

        public SBM_CenterBubble CenterBubble { get => _s.GetReference<SBM_CenterBubble>(0x34); set => _s.SetReference(0x34, value); }

        public HSDArrayAccessor<SBM_Pl_CoinCollisionSphere> CoinCollisionSpheres { get => _s.GetReference<HSDArrayAccessor<SBM_Pl_CoinCollisionSphere>>(0x38); set => _s.SetReference(0x38, value); }

        public SBM_PlCameraBox CameraBox { get => _s.GetReference<SBM_PlCameraBox>(0x3C); set => _s.SetReference(0x3C, value); }

        public SBM_PlItemPickupParam ItemPickupParams { get => _s.GetReference<SBM_PlItemPickupParam>(0x40); set => _s.SetReference(0x40, value); }

        public SBM_EnvironmentCollision EnvironmentCollision { get => _s.GetReference<SBM_EnvironmentCollision>(0x44); set => _s.SetReference(0x44, value); }

        public SBM_ArticlePointer Articles { get => _s.GetReference<SBM_ArticlePointer>(0x48); set => _s.SetReference(0x48, value); }

        public SBM_PlayerSFXTable CommonSoundEffectTable { get => _s.GetReference<SBM_PlayerSFXTable>(0x4C); set => _s.SetReference(0x4C, value); }

        public SBM_PlJostleBox JostleBox { get => _s.GetReference<SBM_PlJostleBox>(0x50); set => _s.SetReference(0x50, value); }

        public SBM_FighterBoneIDs FighterBoneTable { get => _s.GetReference<SBM_FighterBoneIDs>(0x54); set => _s.SetReference(0x54, value); }

        public SBM_FighterIK FighterIK { get => _s.GetReference<SBM_FighterIK>(0x58); set => _s.SetReference(0x58, value); }

        public HSD_JOBJ MetalModel { get => _s.GetReference<HSD_JOBJ>(0x5C); set => _s.SetReference(0x5C, value); }

    }

    public class SBM_Pl_CoinCollisionSphere : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        public int BoneIndex { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public float XOffset { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float YOffset { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float ZOffset { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float Size { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }
    }

    public class SBM_PlJostleBox : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public float Offset { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }
        public float Size { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
    }

    public class SBM_CenterBubble : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int BoneIndex { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public float Size { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SBM_DynamicBehavior : HSDAccessor
    {
        public override int TrimmedSize => 2;

        public byte Flags { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public byte BoneTableIndex { get => _s.GetByte(0x01); set => _s.SetByte(0x01, value); }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SBM_ShieldModelContainer : HSDAccessor
    {
        public override int TrimmedSize => 4;

        public HSD_JOBJ ShieldPose { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }
    }
}
