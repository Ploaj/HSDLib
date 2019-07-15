using HSDRaw.Common;
using HSDRaw.Common.Animation;

namespace HSDRaw.Melee.Gr
{
    public class SBM_Model_Group : HSDAccessor
    {
        public override int TrimmedSize => 0x34;

        public HSD_JOBJ RootNode { get => _s.GetReference<HSD_JOBJ>(0); set => _s.SetReference(0x00, value); }

        public HSDNullPointerArrayAccessor<HSD_AnimJoint> JointAnimations { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_AnimJoint>>(0x04); set => _s.SetReference(0x04, value); }

        public HSDNullPointerArrayAccessor<HSD_MatAnimJoint> MaterialAnimations { get => _s.GetReference<HSDNullPointerArrayAccessor<HSD_MatAnimJoint>>(0x08); set => _s.SetReference(0x08, value); }

        //0x0C Unknown null pointer list for unknown anim

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
		0x20 Unknown
		0x24 Unknown
		0x28 Unknown visibility?
		0x2C Unknown
		0x30 Unknown
        */
    }
}
