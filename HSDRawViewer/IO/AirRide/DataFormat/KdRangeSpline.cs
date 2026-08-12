using HSDRaw.AirRide.Gr.Data;
using System;
using System.ComponentModel;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    [Flags]
    public enum KdRangeSplineFlags : uint
    {
        FlightPath = 0x80000000,
        SwitchPath = 0x40000000,
    }


    public class KdRangeSpline
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KdSpline LeftSpline { get; set; } = new KdSpline();

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KdSpline RightSpline { get; set; } = new KdSpline();

        public int x08 { get; set; }

        public int x0C { get; set; }

        public int x10 { get; set; }

        public KdRangeSplineFlags Flags { get; set; }

        public KdRangeSpline(KAR_grRangeSpline range)
        {
            LeftSpline = new KdSpline(range.LeftSpline);
            RightSpline = new KdSpline(range.RightSpline);
            x08 = range.x08;
            x0C = range.x0C;
            x10 = range.x10;
            Flags = (KdRangeSplineFlags)range.Flags;
        }

        public KAR_grRangeSpline ToRangeSpline()
        {
            return new KAR_grRangeSpline()
            {
                LeftSpline = LeftSpline.ToHsdSpline(),
                RightSpline = RightSpline.ToHsdSpline(),
                x08 = x08,
                x0C = x0C,
                x10 = x10,
                Flags = (uint)Flags,
            };
        }
    }
}
