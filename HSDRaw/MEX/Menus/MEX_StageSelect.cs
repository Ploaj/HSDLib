using HSDRaw.Common.Animation;
using HSDRaw.MEX.Stages;

namespace HSDRaw.MEX.Menus
{
    public class MEX_PageStruct : HSDAccessor
    {
        public override int TrimmedSize => 0x20;

        public int LiveJOBJ { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int PageCount { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        public int CurrentPage { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
        
        // 0x0C reserved

        public HSDFixedLengthPointerArrayAccessor<HSD_AnimJoint> Anims
        {
            get => _s.GetReference<HSDFixedLengthPointerArrayAccessor<HSD_AnimJoint>>(0x10);
            set => _s.SetReference(0x10, value);
        }
    }

}
