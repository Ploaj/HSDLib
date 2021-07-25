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

        public SBM_DynamicBehaviorIDs FighterActionDynamicBehaviors { get => _s.GetReference<SBM_DynamicBehaviorIDs>(0x10); set => _s.SetReference(0x10, value); }

        public SBM_FighterActionTable DemoActionTable { get => _s.GetReference<SBM_FighterActionTable>(0x14); set => _s.SetReference(0x14, value); }

        public SBM_DynamicBehaviorIDs DemoActionDynamicBehaviors { get => _s.GetReference<SBM_DynamicBehaviorIDs>(0x18); set => _s.SetReference(0x18, value); }

        public HSDFixedLengthPointerArrayAccessor<SBM_ModelPart> ModelPartAnimations { get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<SBM_ModelPart>>(0x1C); set => _s.SetReference(0x1C, value); }
        
        public SBM_ShieldModelContainer ShieldPoseContainer { get => _s.GetReference<SBM_ShieldModelContainer>(0x20); set => _s.SetReference(0x20, value); }

        public HSDArrayAccessor<SBM_ActionChance> IdleActionChances { get => _s.GetReference<HSDArrayAccessor<SBM_ActionChance>>(0x24); set => _s.SetReference(0x24, value); }

        public HSDArrayAccessor<SBM_ActionChance> WaitIdleActionChances { get => _s.GetReference<HSDArrayAccessor<SBM_ActionChance>>(0x28); set => _s.SetReference(0x28, value); }

        public SBM_PhysicsGroup Physics { get => _s.GetReference<SBM_PhysicsGroup>(0x2C); set => _s.SetReference(0x2C, value); }

        public SBM_HurtboxBank<SBM_Hurtbox> Hurtboxes { get => _s.GetReference<SBM_HurtboxBank<SBM_Hurtbox>>(0x30); set => _s.SetReference(0x30, value); }

        public SBM_CenterBubble CenterBubble { get => _s.GetReference<SBM_CenterBubble>(0x34); set => _s.SetReference(0x34, value); }

        public HSDAccessor Unknown0x38 { get => _s.GetReference<HSDAccessor>(0x38); set => _s.SetReference(0x38, value); }

        public SBM_Pl_CameraBox CameraBox { get => _s.GetReference<SBM_Pl_CameraBox>(0x3C); set => _s.SetReference(0x3C, value); }

        public HSDAccessor Unknown0x40 { get => _s.GetReference<HSDAccessor>(0x40); set => _s.SetReference(0x40, value); }

        public SBM_EnvironmentCollision EnvironmentCollision { get => _s.GetReference<SBM_EnvironmentCollision>(0x44); set => _s.SetReference(0x44, value); }

        public SBM_ArticlePointer Articles { get => _s.GetReference<SBM_ArticlePointer>(0x48); set => _s.SetReference(0x48, value); }

        public SBM_PlayerSFXTable CommonSoundEffectTable { get => _s.GetReference<SBM_PlayerSFXTable>(0x4C); set => _s.SetReference(0x4C, value); }

        public HSDAccessor Unknown0x50 { get => _s.GetReference<HSDAccessor>(0x50); set => _s.SetReference(0x50, value); }

        public SBM_FighterBoneIDs FighterBoneTable { get => _s.GetReference<SBM_FighterBoneIDs>(0x54); set => _s.SetReference(0x54, value); }

        public SBM_FighterIK FighterIK { get => _s.GetReference<SBM_FighterIK>(0x58); set => _s.SetReference(0x58, value); }

        public HSD_JOBJ MetalModel { get => _s.GetReference<HSD_JOBJ>(0x5C); set => _s.SetReference(0x5C, value); }

    }

    public class SBM_Pl_CameraBox : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public float YOffset { get => _s.GetFloat(0x00); set => _s.SetFloat(0x00, value); }

        public float ProjRight { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float ProjLeft { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float ProjTop { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        public float ProjBottom { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        public float x50_of_camera_box { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }
    }

    public class SBM_CenterBubble : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int BoneIndex { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public float Size { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }
    }

    public class SBM_ActionChance : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public int ActionID { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int Chance { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SBM_DynamicBehaviorIDs : HSDAccessor
    {
        public short[] Values
        {
            get
            {
                var e = new short[_s.Length / 2];

                for (int i = 0; i < e.Length; i++)
                    e[i] = _s.GetInt16(i * 2);

                return e;
            }
            set
            {
                if (value == null)
                {
                    _s.Resize(4);
                    _s.SetInt32(0, 0);
                }
                else
                {
                    var size = value.Length * 2;
                    if (size % 4 != 0)
                        size += 4 - (size % 4);

                    _s.Resize(size);
                    for (int i = 0; i < value.Length; i++)
                        _s.SetInt16(i * 2, value[i]);
                }
            }
        }
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
