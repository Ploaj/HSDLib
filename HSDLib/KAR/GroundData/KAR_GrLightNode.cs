using System;
using HSDLib.Animation;
using HSDLib.Common;

namespace HSDLib.KAR
{
    public class KAR_GrLightGroup : IHSDNode
    {
        public KAR_GrLightNode GlobalLightGroup { get; set; }

        public KAR_GrLightNode LightGroup1 { get; set; } // player light?

        public KAR_GrLightNode LightGroup2 { get; set; }
    }

    public class KAR_GrLightNode : IHSDNode
    {
        public KAR_GrLight Light1 { get; set; }

        public KAR_GrLight Light2 { get; set; }

        public KAR_GrLight Light3 { get; set; }

        public KAR_GrLight Light4 { get; set; }
    }

    public class KAR_GrLight : IHSDNode
    {
        public HSD_LOBJ LightObject { get; set; }

        public KAR_LightAnimPointer AnimPointer { get; set; }
    }

    public class KAR_LightAnimPointer : IHSDNode
    {
        public HSD_AOBJ LightAnim { get; set; }

        public int Unknown { get; set; }
    }

}
