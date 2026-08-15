using HSDRaw.AirRide.Gr.Data;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
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

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KdAnimation Animation { get; set; } = new KdAnimation();

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

        [TypeConverter(typeof(ListConverter<RailSpeed>))]
        public List<RailSpeed> Speed { get; set; } = new List<RailSpeed>();

        [TypeConverter(typeof(ListConverter<RailDataIndex>))]
        public List<RailDataIndex> StopFriction { get; set; } = new List<RailDataIndex>();

        [TypeConverter(typeof(ListConverter<RailDataIndex>))]
        public List<RailDataIndex> Material { get; set; } = new List<RailDataIndex>();

        [TypeConverter(typeof(ListConverter<RailLeap>))]
        public List<RailLeap> Leap { get; set; } = new List<RailLeap>();

        public KdRail() { }

        public KdRail(KAR_grRailColl coll, HSD_Spline[] splines, HSD_AnimJoint[] animations)
        {
            if (coll.StartSplineIndex >= 0 && coll.StartSplineIndex < splines.Length)
                Spline1 = new KdSpline(splines[coll.StartSplineIndex]);

            if (coll.SplineLengthIndex >= 0 && coll.SplineLengthIndex < splines.Length)
                Spline2 = new KdSpline(splines[coll.SplineLengthIndex]);

            if (coll.SubAnimIndex >= 0 && coll.SubAnimIndex < animations.Length)
                Animation = new KdAnimation(animations[coll.SubAnimIndex]);

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
                Speed.AddRange(coll.Param.Speed.Array.Select(e => new RailSpeed(e)));
            if (coll.Param.DashCount > 0)
                StopFriction.AddRange(coll.Param.StopFriction.Array.Select(e => new RailDataIndex(e)));
            if (coll.Param.Dash2Count > 0)
                Material.AddRange(coll.Param.Material.Array.Select(e => new RailDataIndex(e)));
            if (coll.Param.LeapCount > 0)
                Leap.AddRange(coll.Param.Leap.Array.Select(e => new RailLeap(e)));
        }

        public KAR_grRailColl ToRailColl(List<KdSpline> railSplines, List<KdAnimation> animations)
        {
            int railIndex1 = railSplines.Count;
            int railIndex2 = railSplines.Count;

            railSplines.Add(Spline1);

            if (!Spline1.Equals(Spline2))
            {
                railIndex2 = railSplines.Count;
                railSplines.Add(Spline2);
            }

            int anim_index = animations.Count;
            animations.Add(Animation);

            return new KAR_grRailColl()
            {
                StartSplineIndex = railIndex1,
                SplineLengthIndex = railIndex2,
                SubAnimIndex = anim_index,
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
                    DataCount = Speed.Count,
                    Speed = Speed.Count > 0 ? new HSDRaw.HSDArrayAccessor<KAR_grRailDataParam>() { Array = Speed.Select(e => e.ToParam()).ToArray() } : null,
                    DashCount = StopFriction.Count,
                    StopFriction = StopFriction.Count > 0 ? new HSDRaw.HSDArrayAccessor<KAR_grRailDashParam>() { Array = StopFriction.Select(e => e.ToParam()).ToArray() } : null,
                    Dash2Count = Material.Count,
                    Material = Material.Count > 0 ? new HSDRaw.HSDArrayAccessor<KAR_grRailDashParam>() { Array = Material.Select(e => e.ToParam()).ToArray() } : null,
                    LeapCount = Leap.Count,
                    Leap = Leap.Count > 0 ? new HSDRaw.HSDArrayAccessor<KAR_grRailLeapParam>() { Array = Leap.Select(e => e.ToParam()).ToArray() } : null,
                }
            };
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class RailSpeed
        {
            public float Offset { get; set; }

            public float Speed1 { get; set; }

            public float Speed2 { get; set; } = 0.001f;

            public RailSpeed() { }

            public RailSpeed(KAR_grRailDataParam d)
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
        public class RailDataIndex
        {
            public float Offset { get; set; }

            public int Index { get; set; }

            public RailDataIndex() { }

            public RailDataIndex(KAR_grRailDashParam d)
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
            [DisplayName("Offset")]
            [Description("The offset (0-1) of the spline this data will start on.")]
            public float Offset { get; set; }

            [DisplayName("Left Rail")]
            [Description("The index of the left rail.")]
            public int LeftRailIndex { get; set; }

            [DisplayName("Right Rail")]
            [Description("The index of the right rail.")]
            public int RightRailIndex { get; set; }

            public RailLeap() { }

            public RailLeap(KAR_grRailLeapParam d)
            {
                Offset = d.Offset;
                LeftRailIndex = d.LeftRailIndex;
                RightRailIndex = d.RightRailIndex;
            }

            public KAR_grRailLeapParam ToParam()
            {
                return new KAR_grRailLeapParam()
                {
                    Offset = Offset,
                    LeftRailIndex = LeftRailIndex,
                    RightRailIndex = RightRailIndex,
                };
            }

            public override string ToString()
            {
                return $"{Offset}: L: {LeftRailIndex}, R: {RightRailIndex}";
            }
        }
    }
}
