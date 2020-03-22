using HSDRaw.Common;
using HSDRaw.Common.Animation;

namespace HSDRaw.Melee.Gr
{
    public class SBM_Map_GOBJ : HSDAccessor
    {
        public override int TrimmedSize => 0x34;

        public HSD_JOBJ RootNode { get => _s.GetReference<HSD_JOBJ>(0); set => _s.SetReference(0x00, value); }

        public HSDNullPointerArrayAccessor<HSD_AnimJoint> JointAnimations { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_AnimJoint>>(0x04); set => _s.SetReference(0x04, value); }

        public HSDNullPointerArrayAccessor<HSD_MatAnimJoint> MaterialAnimations { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_MatAnimJoint>>(0x08); set => _s.SetReference(0x08, value); }

        public HSDNullPointerArrayAccessor<HSD_ShapeAnimJoint> ShapeAnimations { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_ShapeAnimJoint>>(0x0C); set => _s.SetReference(0x0C, value); }
        
        public HSD_Camera Camera { get => _s.GetReference<HSD_Camera>(0x10); set => _s.SetReference(0x10, value); }

        //0x14 Unknown 0x0C?? shape anim joint?

        public HSDNullPointerArrayAccessor<HSD_Light> Lights { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_Light>>(0x18); set => _s.SetReference(0x18, value); }

        public HSD_FogDesc Fog { get => _s.GetReference<HSD_FogDesc>(0x1C); set => _s.SetReference(0x1C, value); }
        
        public int CollisionLinkCount { get => _s.GetInt32(0x24); set => _s.SetInt32(0x24, value); }

        public HSDArrayAccessor<SBM_Map_GOBJ_CollisionLink> CollisionLinks
        {
            get
            {
                return _s.GetReference<HSDArrayAccessor<SBM_Map_GOBJ_CollisionLink>>(0x20);
            }
            set
            {
                if(value == null || value.Length == 0)
                {
                    CollisionLinkCount = 0;
                    _s.SetReference(0x20, null);
                }
                else
                {
                    CollisionLinkCount = value.Length;
                    _s.SetReference(0x20, value);
                }
            }
        }
        

        /*
		0x28 Unknown visibility?
		0x2C Unknown
		0x30 Unknown
        */
    }

    public class SBM_Map_GOBJ_CollisionLink : HSDAccessor
    {
        public override int TrimmedSize => 0x06;

        public short CollisionIndex { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }

        public short UnknownIndex { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public short JOBJIndex { get => _s.GetInt16(0x04); set => _s.SetInt16(0x04, value); }
    }
}
