using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.GUI.PropertyGrid;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public class KdRail
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KdSpline Spline1 { get; set; } = new KdSpline();

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KdSpline Spline2 { get; set; } = new KdSpline();

        public int AnimationIndex { get; set; }

        public int NextRail1 { get; set; }

        public int NextRail2 { get; set; }

        public int NextRail3 { get; set; }

        public int PreviousRail { get; set; }

        public int Unused20 { get; set; }

        public int Unused24 { get; set; }

        public int CityRailIndex { get; set; }

        public int Unknown2C { get; set; }


        public int Unknown30 { get; set; }

        public bool Unknown30_1 { get; set; }


        public int Param00 { get; set; }

        public int ParamFlag { get; set; }

        public float Param08 { get; set; }

        public int ParamAltRail1 { get; set; }

        public int ParamAltRail2 { get; set; }

        [TypeConverter(typeof(ListConverter<RailData>))]
        public List<RailData> Data { get; set; } = new List<RailData>();

        [TypeConverter(typeof(ListConverter<RailDash>))]
        public List<RailDash> Dash1 { get; set; } = new List<RailDash>();

        [TypeConverter(typeof(ListConverter<RailDash>))]
        public List<RailDash> Dash2 { get; set; } = new List<RailDash>();

        [TypeConverter(typeof(ListConverter<RailLeap>))]
        public List<RailLeap> Leap { get; set; } = new List<RailLeap>();

        public KdRail() { }

        public KdRail(KAR_grRailColl coll, KAR_grSplineNode splineNode)
        {
            var splines = splineNode.RailSpline1.Splines.Array;

            if (coll.StartSplineIndex > 0 && coll.StartSplineIndex < splines.Length)
                Spline1 = new KdSpline(splines[coll.StartSplineIndex]);

            if (coll.SplineLengthIndex > 0 && coll.SplineLengthIndex < splines.Length)
                Spline2 = new KdSpline(splines[coll.SplineLengthIndex]);

            AnimationIndex = coll.SubAnimIndex;
            NextRail1 = coll.x10;
            NextRail2 = coll.x14;
            NextRail3 = coll.x18;
            PreviousRail = coll.x1C;
            Unused20 = coll.x20;
            Unused24 = coll.x24;
            CityRailIndex = coll.x28;
            Unknown2C = coll.x2C;
            Unknown30 = coll.x30 >> 1;
            Unknown30_1 = (coll.x30 & 0x1) != 0;

            Param00 = coll.Param.x00;
            ParamFlag = coll.Param.Flags;
            Param08 = coll.Param.x08;
            ParamAltRail1 = coll.Param.AltRail1;
            ParamAltRail2 = coll.Param.AltRail2;
            if (coll.Param.DataCount > 0)
                Data.AddRange(coll.Param.Data.Array.Select(e => new RailData(e)));
            if (coll.Param.DashCount > 0)
                Dash1.AddRange(coll.Param.Dash.Array.Select(e => new RailDash(e)));
            if (coll.Param.Dash2Count > 0)
                Dash2.AddRange(coll.Param.Dash2.Array.Select(e => new RailDash(e)));
            if (coll.Param.LeapCount > 0)
                Leap.AddRange(coll.Param.Leap.Array.Select(e => new RailLeap(e)));
        }

        public KAR_grRailColl ToRailColl(List<KdSpline> railSplines)
        {
            int railIndex1 = railSplines.Count;
            int railIndex2 = railSplines.Count;

            railSplines.Add(Spline1);

            if (!Spline1.Equals(Spline2))
            {
                railIndex2 = railSplines.Count;
                railSplines.Add(Spline2);
            }

            return new KAR_grRailColl()
            {
                StartSplineIndex = railIndex1,
                SplineLengthIndex = railIndex2,
                SubAnimIndex = AnimationIndex,
                x10 = NextRail1,
                x14 = NextRail2,
                x18 = NextRail3,
                x1C = PreviousRail,
                x20 = Unused20,
                x24 = Unused24,
                x28 = CityRailIndex,
                x2C = Unknown2C,
                x30 = (Unknown30 << 1) | (Unknown30_1 ? 1 : 0),

                Param = new KAR_grRailParam()
                {
                    x00 = Param00,
                    Flags = ParamFlag,
                    x08 = Param08,
                    AltRail1 = ParamAltRail1,
                    AltRail2 = ParamAltRail2,
                    DataCount = Data.Count,
                    Data = Data.Count > 0 ? new HSDRaw.HSDArrayAccessor<KAR_grRailDataParam>() { Array = Data.Select(e => e.ToParam()).ToArray() } : null,
                    DashCount = Dash1.Count,
                    Dash = Dash1.Count > 0 ? new HSDRaw.HSDArrayAccessor<KAR_grRailDashParam>() { Array = Dash1.Select(e => e.ToParam()).ToArray() } : null,
                    Dash2Count = Dash2.Count,
                    Dash2 = Dash2.Count > 0 ? new HSDRaw.HSDArrayAccessor<KAR_grRailDashParam>() { Array = Dash2.Select(e => e.ToParam()).ToArray() } : null,
                    LeapCount = Leap.Count,
                    Leap = Leap.Count > 0 ? new HSDRaw.HSDArrayAccessor<KAR_grRailLeapParam>() { Array = Leap.Select(e => e.ToParam()).ToArray() } : null,
                }
            };
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class RailData
        {
            public float Offset { get; set; }

            public float Speed1 { get; set; }

            public float Speed2 { get; set; } = 0.001f;

            public RailData() { }

            public RailData(KAR_grRailDataParam d)
            {
                Offset = d.Offset;
                Speed1 = d.Speed1;
                Speed2 = d.Speed2;
            }

            public KAR_grRailDataParam ToParam()
            {
                return new KAR_grRailDataParam()
                {
                    Offset = Offset,
                    Speed1 = Speed1,
                    Speed2 = Speed2,
                };
            }

            public override string ToString()
            {
                return $"{Offset}: {Speed1}, {Speed2}";
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class RailDash
        {
            public float Offset { get; set; }

            public int Index { get; set; }

            public RailDash() { }

            public RailDash(KAR_grRailDashParam d)
            {
                Offset = d.Offset;
                Index = d.Index;
            }

            public KAR_grRailDashParam ToParam()
            {
                return new KAR_grRailDashParam()
                {
                    Offset = Offset,
                    Index = Index,
                };
            }

            public override string ToString()
            {
                return $"{Offset}: {Index}";
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class RailLeap
        {
            public float Offset { get; set; }

            public int Index1 { get; set; }

            public int Index2 { get; set; }

            public RailLeap() { }

            public RailLeap(KAR_grRailLeapParam d)
            {
                Offset = d.Offset;
                Index1 = d.RailIndex1;
                Index2 = d.RailIndex2;
            }

            public KAR_grRailLeapParam ToParam()
            {
                return new KAR_grRailLeapParam()
                {
                    Offset = Offset,
                    RailIndex1 = Index1,
                    RailIndex2 = Index2,
                };
            }

            public override string ToString()
            {
                return $"{Offset}: {Index1}, {Index2}";
            }
        }
    }
}
