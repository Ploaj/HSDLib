using HSDLib.Common;
using System.Collections.Generic;

namespace HSDLib.KAR
{
    public class KAR_GrSplineNode : IHSDNode
    {
        public int CourseSplineSetup { get; set; }

        public KAR_GrSplineSetup RangeSplineSetup { get; set; }
        public KAR_GrSplineSetup GravitySplineSetup { get; set; }

        public KAR_GrFlowSetup AirFlowSetup { get; set; }
        public KAR_GrSplineList ConveyerFlowSetup { get; set; }

        public KAR_GrSplineList UnknownGetSplineDataAll { get; set; }

        public KAR_GrSplineList RailSplineSetup { get; set; }
    }

}
