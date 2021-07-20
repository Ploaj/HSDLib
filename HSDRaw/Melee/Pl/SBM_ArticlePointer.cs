using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Cmd;

namespace HSDRaw.Melee.Pl
{
    public class SBM_ArticlePointer : HSDAccessor
    {
        public SBM_Article[] Articles
        {
            get
            {
                SBM_Article[] a = new SBM_Article[_s.Length / 4];
                
                for(int i = 0; i < a.Length; i++)
                {
                    a[i] = _s.GetReference<SBM_Article>(4 * i);
                }

                return a;
            }
            set
            {
                _s.References.Clear();
                if(value == null)
                {
                    _s.Resize(4);
                }
                else
                {
                    _s.Resize(value.Length * 4);
                    for(int i = 0; i < value.Length; i++)
                    {
                        _s.SetReference(i * 4, value[i]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SBM_Article : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public SBM_ItemCommonAttr Parameters { get => _s.GetReference<SBM_ItemCommonAttr>(0x00); set => _s.SetReference(0x00, value); }

        public HSDAccessor ParametersExt { get => _s.GetReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }

        public SBM_HurtboxBank<SBM_ItemHurtbox> Hurtboxes { get => _s.GetReference<SBM_HurtboxBank<SBM_ItemHurtbox>>(0x08); set => _s.SetReference(0x08, value); }

        public HSDArrayAccessor<SBM_ItemState> ItemState { get => _s.GetReference<HSDArrayAccessor<SBM_ItemState>>(0x0C); set => _s.SetReference(0x0C, value); }

        public SBM_ItemModel Model { get => _s.GetReference<SBM_ItemModel>(0x10); set => _s.SetReference(0x10, value); }

        public HSDAccessor ItemDynamics { get => _s.GetReference<HSDAccessor>(0x14); set => _s.SetReference(0x14, value); }
    }

    public enum ItemHoldKind
    {
        Normal,
        OpenPalm,
        Grip,
        BentWrist
    }

    /// <summary>
    /// 
    /// </summary>
    public class SBM_ItemCommonAttr : HSDAccessor
    {
        public override int TrimmedSize => 0x84;

        public bool IsHeavy { get => (_s.GetByte(0x00) >> 7) == 1; set => _s.SetByte(0x00, (byte)((_s.GetByte(0x00) & 0x7F) | (value ? 1 : 0))); }

        public ItemHoldKind HoldKind { get => (ItemHoldKind)(_s.GetByte(0x00) & 0x7F); set => _s.SetByte(0x00, (byte)(((byte)value & 0x7F) | (_s.GetByte(0x00) & 0x80))); }
    
        public float ThrowSpeedMultiplier { get => _s.GetFloat(0x04); set => _s.SetFloat(0x04, value); }

        public float Unknown0x08 { get => _s.GetFloat(0x08); set => _s.SetFloat(0x08, value); }

        public float SpinSpeed { get => _s.GetFloat(0x0C); set => _s.SetFloat(0x0C, value); }

        public float FallAccel { get => _s.GetFloat(0x10); set => _s.SetFloat(0x10, value); }

        public float MaxFallSpeed { get => _s.GetFloat(0x14); set => _s.SetFloat(0x14, value); }

        public float Unknown0x18 { get => _s.GetFloat(0x18); set => _s.SetFloat(0x18, value); }

        public float Unknown0x1C { get => _s.GetFloat(0x1C); set => _s.SetFloat(0x1C, value); }

        public float Unknown0x20 { get => _s.GetFloat(0x20); set => _s.SetFloat(0x20, value); }

        public float Unknown0x24 { get => _s.GetFloat(0x24); set => _s.SetFloat(0x24, value); }

        public float Unknown0x28 { get => _s.GetFloat(0x28); set => _s.SetFloat(0x28, value); }

        public float Unknown0x2C { get => _s.GetFloat(0x2C); set => _s.SetFloat(0x2C, value); }

        public float Unknown0x30 { get => _s.GetFloat(0x30); set => _s.SetFloat(0x30, value); }

        public float Unknown0x34 { get => _s.GetFloat(0x34); set => _s.SetFloat(0x34, value); }

        public float Unknown0x38 { get => _s.GetFloat(0x38); set => _s.SetFloat(0x38, value); }

        public float Unknown0x3C { get => _s.GetFloat(0x3C); set => _s.SetFloat(0x3C, value); }

        public float ECB_Top { get => _s.GetFloat(0x40); set => _s.SetFloat(0x40, value); }

        public float ECB_Bottom { get => _s.GetFloat(0x44); set => _s.SetFloat(0x44, value); }

        public float ECB_Left { get => _s.GetFloat(0x48); set => _s.SetFloat(0x48, value); }

        public float ECB_Right { get => _s.GetFloat(0x4C); set => _s.SetFloat(0x4C, value); }

        public float Unknown0x50 { get => _s.GetFloat(0x50); set => _s.SetFloat(0x50, value); }

        public float Unknown0x54 { get => _s.GetFloat(0x54); set => _s.SetFloat(0x54, value); }

        public float Unknown0x58 { get => _s.GetFloat(0x58); set => _s.SetFloat(0x58, value); }

        public float Unknown0x5C { get => _s.GetFloat(0x5C); set => _s.SetFloat(0x5C, value); }

        public float ModelScale { get => _s.GetFloat(0x60); set => _s.SetFloat(0x60, value); }

        public int DestroyGFX { get => _s.GetInt32(0x64); set => _s.SetInt32(0x64, value); }

        public int DestroyGFX2 { get => _s.GetInt32(0x68); set => _s.SetInt32(0x68, value); }

        public int PickupSFX { get => _s.GetInt32(0x6C); set => _s.SetInt32(0x6C, value); }

        public int ThrowSFX { get => _s.GetInt32(0x70); set => _s.SetInt32(0x70, value); }

        public int DropSFX { get => _s.GetInt32(0x74); set => _s.SetInt32(0x74, value); }

        public int RandomHitSFX { get => _s.GetInt32(0x78); set => _s.SetInt32(0x78, value); }

        public int HitGroundSFX { get => _s.GetInt32(0x7C); set => _s.SetInt32(0x7C, value); }

        public int HitWallSFX { get => _s.GetInt32(0x80); set => _s.SetInt32(0x80, value); }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SBM_ItemModel : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSD_JOBJ RootModelJoint { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public int BoneCount { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int BoneAttachID { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public int BitField { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SBM_ItemState : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public HSD_AnimJoint AnimJoint { get => _s.GetReference<HSD_AnimJoint>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_MatAnimJoint MatAnimJoint { get => _s.GetReference<HSD_MatAnimJoint>(0x04); set => _s.SetReference(0x004, value); }

        public HSDAccessor Parameters { get => _s.GetReference<HSDAccessor>(0x08); set => _s.SetReference(0x08, value); }

        public SBM_ItemSubactionData SubactionScript { get => _s.GetReference<SBM_ItemSubactionData>(0x0C); set => _s.SetReference(0x0C, value); }
    }
}
