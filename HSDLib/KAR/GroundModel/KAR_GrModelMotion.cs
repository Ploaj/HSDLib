using HSDLib.Animation;
using HSDLib.MaterialAnimation;

namespace HSDLib.KAR
{
    public class KAR_GrModelMotion : IHSDNode
    {
        [FieldData(typeof(HSD_AnimJoint))]
        public HSD_AnimJoint AnimJoint { get; set; }

        [FieldData(typeof(HSD_MatAnimJoint))]
        public HSD_MatAnimJoint MatAnimJoint { get; set; }
    }
}
