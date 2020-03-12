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

        //0x14 Unknown 0x0C??

        public HSDNullPointerArrayAccessor<HSD_Light> Lights { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_Light>>(0x18); set => _s.SetReference(0x18, value); }

        public bool LoopAnimation {
            get
            {
                var r = _s.GetReference<HSDAccessor>(0x28);
                if (r == null)
                    return true;
                return r._s.GetByte(0) == 1;
            }
            set
            {
                _s.GetCreateReference<HSDAccessor>(0x28)._s.SetData(new byte[] { (byte)(value ? 1 : 0), 0, 0, 0});
            }
        }
        /*
        0x1C Unknown
        */


        public SBM_Map_GOBJ_CollisionLink[] CollisionLinks
        {
            get
            {
                return _s.GetReference<HSDArrayAccessor<SBM_Map_GOBJ_CollisionLink>>(0x20)?.Array;
            }
            set
            {
                if(value == null || value.Length == 0)
                {
                    _s.SetInt32(0x24, value.Length);
                    _s.SetReference(0x20, null);
                }
                else
                {
                    _s.SetInt32(0x24, value.Length);
                    _s.GetReference<HSDArrayAccessor<SBM_Map_GOBJ_CollisionLink>>(0x20).Array = value;
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
