using HSDRaw.AirRide.Gr.Data;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    internal class KdSplineData
    {
        public List<KdSpline> Course { get; set; }

        public List<KdSpline> Rail { get; set; }

        public List<KdSpline> Rail2 { get; set; }

        public KdSplineData() { }

        public KdSplineData(KAR_grSplineNode node)
        {
            Course = node.SplineSetup.CourseSplineList.Splines.Array.Select(e => new KdSpline(e)).ToList();

            if (node.RailSpline1 != null)
                Rail = node.RailSpline1.Splines.Array.Select(e => new KdSpline(e)).ToList();

            if (node.RailSpline2 != null)
                Rail2 = node.RailSpline2.Splines.Array.Select(e => new KdSpline(e)).ToList();
        }

        public KAR_grSplineNode ToSplineNode()
        {
            // TODO:
            return new KAR_grSplineNode()
            {

            };
        }
    }
}
