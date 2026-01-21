using HSDRaw.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    internal class KdSpline
    {
        public int Type { get; set; }

        public float Tension { get; set; }

        [JsonConverter(typeof(JsonInlineListConverter<float>))]
        public List<float> Points { get; set; }

        // Seg Poly?

        public KdSpline() { }

        public KdSpline(HSD_Spline s)
        {
            Points = s.Points.SelectMany(e=>new float[] { e.X, e.Y, e.Z }).ToList();
            Tension = s.Tension;
            Type = s.Type;


            //if (s.SegPolys != null)
            //{
            //    throw new NotSupportedException();
            //}
        }
    }
}
