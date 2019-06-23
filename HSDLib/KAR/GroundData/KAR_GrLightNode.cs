using System;
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
        public HSD_Light Light1 { get; set; }

        public HSD_Light Light2 { get; set; }

        public HSD_Light Light3 { get; set; }

        public HSD_Light Light4 { get; set; }
    }

}
