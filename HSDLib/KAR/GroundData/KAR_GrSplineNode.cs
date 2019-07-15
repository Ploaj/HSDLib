namespace HSDLib.KAR
{
    public class KAR_GrSplineNode : IHSDNode
    {
        public KAR_GrCourseSplineSetup CourseSplineSetup { get; set; }

        public KAR_GrSplineSetup RangeSplineSetup { get; set; }
        public KAR_GrSplineSetup GravitySplineSetup { get; set; }

        public KAR_GrFlowSetup AirFlowSetup { get; set; }

        public KAR_GrSplineList ConveyerSplines { get; set; }
        public KAR_GrSplineList UnknownGetSplineDataAll { get; set; }
        public KAR_GrSplineList RailSplines { get; set; }
    }

}
