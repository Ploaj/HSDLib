using HSDRaw.Common;

namespace HSDRaw.Melee.Pl
{
    /// <summary>
    /// 
    /// </summary>
    public class SBM_PlayerData : HSDAccessor
    {
        public override int TrimmedSize => 0x60;

        public SBM_CommonFighterAttributes Attributes { get => _s.GetReference<SBM_CommonFighterAttributes>(0x00); set => _s.SetReference(0x00, value); }

        public HSDAccessor Attributes2 { get => _s.GetReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }
        
        public SBM_PlayerModelLookupTables ModelLookupTables { get => _s.GetReference<SBM_PlayerModelLookupTables>(0x08); set => _s.SetReference(0x08, value); }

        public SBM_SubActionTable SubActionTable { get => _s.GetReference<SBM_SubActionTable>(0x0C); set => _s.SetReference(0x0C, value); }

        public SBM_SubactionShorts SubActionShorts { get => _s.GetReference<SBM_SubactionShorts>(0x10); set => _s.SetReference(0x10, value); }

        public SBM_SubActionTable WinSubAction { get => _s.GetReference<SBM_SubActionTable>(0x14); set => _s.SetReference(0x14, value); }

        public SBM_SubactionShorts WinSubActionShort { get => _s.GetReference<SBM_SubactionShorts>(0x18); set => _s.SetReference(0x18, value); }

        public SBM_ModelPartTable ModelPartAnimations { get => _s.GetReference<SBM_ModelPartTable>(0x1C); set => _s.SetReference(0x1C, value); }
        
        public SBM_ShieldModelContainer ShieldPoseContainer { get => _s.GetReference<SBM_ShieldModelContainer>(0x20); set => _s.SetReference(0x20, value); }

        public HSDAccessor Unknown0x24 { get => _s.GetReference<HSDAccessor>(0x24); set => _s.SetReference(0x24, value); }

        public HSDAccessor Unknown0x28 { get => _s.GetReference<HSDAccessor>(0x28); set => _s.SetReference(0x28, value); }

        public SBM_PhysicsGroup Physics { get => _s.GetReference<SBM_PhysicsGroup>(0x2C); set => _s.SetReference(0x2C, value); }

        public SBM_HurtboxBank<SBM_Hurtbox> Hurtboxes { get => _s.GetReference<SBM_HurtboxBank<SBM_Hurtbox>>(0x30); set => _s.SetReference(0x30, value); }

        public HSDAccessor Unknown0x34 { get => _s.GetReference<HSDAccessor>(0x34); set => _s.SetReference(0x34, value); }

        public HSDAccessor Unknown0x38 { get => _s.GetReference<HSDAccessor>(0x38); set => _s.SetReference(0x38, value); }

        public HSDAccessor Unknown0x3C { get => _s.GetReference<HSDAccessor>(0x3C); set => _s.SetReference(0x3C, value); }

        public HSDAccessor Unknown0x40 { get => _s.GetReference<HSDAccessor>(0x40); set => _s.SetReference(0x40, value); }

        public SBM_LedgeGrabBox LedgeGrabBox { get => _s.GetReference<SBM_LedgeGrabBox>(0x44); set => _s.SetReference(0x44, value); }

        public SBM_ArticlePointer Articles { get => _s.GetReference<SBM_ArticlePointer>(0x48); set => _s.SetReference(0x48, value); }

        public SBM_PlayerSFXTable CommonSoundEffectTable { get => _s.GetReference<SBM_PlayerSFXTable>(0x4C); set => _s.SetReference(0x4C, value); }

        public HSDAccessor Unknown0x50 { get => _s.GetReference<HSDAccessor>(0x50); set => _s.SetReference(0x50, value); }

        public HSDAccessor Unknown0x54 { get => _s.GetReference<HSDAccessor>(0x54); set => _s.SetReference(0x54, value); }

        public HSDAccessor Unknown0x58 { get => _s.GetReference<HSDAccessor>(0x58); set => _s.SetReference(0x58, value); }

        public HSD_JOBJ ShadowModel { get => _s.GetReference<HSD_JOBJ>(0x5C); set => _s.SetReference(0x5C, value); }

    }

    /// <summary>
    /// 
    /// </summary>
    public class SBM_SubactionShorts : HSDAccessor
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
