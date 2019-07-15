using System;
using HSDLib.Animation;
using HSDLib.MaterialAnimation;

namespace HSDLib.Common
{
    public class HSD_SOBJ : IHSDNode
    {
        public HSD_PointerArray<HSD_JOBJDesc> JOBJDescs { get; set; }

        public HSD_PointerArray<HSD_Camera> Camera { get; set; }
        
        public HSD_PointerArray<HSD_Light> Lights { get; set; }

        public int Fog { get; set; }
    }

    public class HSD_JOBJDesc : IHSDNode
    {
        public HSD_JOBJ RootJoint { get; set; }

        public HSD_PointerArray<HSD_AnimJoint> SkelAnimations { get; set; }

        public HSD_PointerArray<HSD_MatAnimJoint> MatAnimations { get; set; }

        public int Unknown { get; set; }
    }
}
