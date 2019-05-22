using HSDLib.Animation;
using HSDLib.MaterialAnimation;

namespace HSDLib.KAR
{
    public class KAR_GrModelMotion : IHSDNode
    {
        public HSD_AnimJoint AnimJoint { get; set; }
        
        public HSD_MatAnimJoint MatAnimJoint { get; set; }
    }
}
