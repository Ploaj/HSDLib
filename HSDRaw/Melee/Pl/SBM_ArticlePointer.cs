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

        public HSDAccessor Parameters { get => _s.GetReference<HSDAccessor>(0x00); set => _s.SetReference(0x00, value); }

        public HSDAccessor ParametersExt { get => _s.GetReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }

        public SBM_HurtboxBank<SBM_ItemHurtbox> Hurtboxes { get => _s.GetReference<SBM_HurtboxBank<SBM_ItemHurtbox>>(0x08); set => _s.SetReference(0x08, value); }

        public HSDArrayAccessor<SBM_ItemState> ItemState { get => _s.GetReference<HSDArrayAccessor<SBM_ItemState>>(0x0C); set => _s.SetReference(0x0C, value); }

        public SBM_ItemModel Model { get => _s.GetReference<SBM_ItemModel>(0x10); set => _s.SetReference(0x10, value); }

        public ItemDynamics ItemDynamics { get => _s.GetReference<ItemDynamics>(0x14); set => _s.SetReference(0x14, value); }
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
