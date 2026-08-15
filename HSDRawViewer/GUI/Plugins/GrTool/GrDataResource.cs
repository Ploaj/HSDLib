using HSDRaw;
using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.GUI.Plugins.GrTool
{
    public class GrDataResource
    {
        private KdFile _data;

        /// <summary>
        /// Collisions
        /// </summary>

        public ObservableList<KdMesh> Meshes = new ObservableList<KdMesh>();

        public ObservableList<KdZone> Zones = new ObservableList<KdZone>();

        /// <summary>
        /// Positions
        /// </summary>

        public ObservableList<KdPositionList> StartPositions = new ObservableList<KdPositionList>();

        public ObservableList<KdPositionList> EnemyPositions = new ObservableList<KdPositionList>();

        public ObservableList<KdPositionList> GravityPositions = new ObservableList<KdPositionList>();

        public ObservableList<KdPositionList> AirFlowPositions = new ObservableList<KdPositionList>();

        public ObservableList<KdPositionList> ConveyorPositions = new ObservableList<KdPositionList>();

        public ObservableList<KdPositionList> ItemPositions = new ObservableList<KdPositionList>();

        public ObservableList<KdPositionList> EventPositions = new ObservableList<KdPositionList>();

        public ObservableList<KdPositionList> VehiclePositions = new ObservableList<KdPositionList>();

        public ObservableList<KdPositionList> GlobalDeadPositions = new ObservableList<KdPositionList>();

        public ObservableList<KdPositionList> LocalDeadPositions = new ObservableList<KdPositionList>();

        public ObservableList<KdPositionList> YakumonoPositions = new ObservableList<KdPositionList>();

        public ObservableList<KdPositionAreaList> ItemAreaPositions = new ObservableList<KdPositionAreaList>();

        public ObservableList<KdPositionAreaList> VehicleAreaPositions = new ObservableList<KdPositionAreaList>();

        /// <summary>
        /// Splines
        /// </summary>

        public KdCourseSplineSetup CourseSpline { get; } = new KdCourseSplineSetup();

        public ObservableList<KdRangeSpline> RangeSplines = new ObservableList<KdRangeSpline>();

        public ObservableList<KdSpline> ConveyorSplines = new ObservableList<KdSpline>();

        public ObservableList<KdSpline> HeavySplines = new ObservableList<KdSpline>();

        /// <summary>
        /// Animations
        /// </summary>

        public ObservableList<KdAnimation> SuperJumpAnimations = new ObservableList<KdAnimation>();

        public ObservableList<KdAnimation> LeapAnimations = new ObservableList<KdAnimation>();

        public ObservableList<KdAnimation> x0CAnimations = new ObservableList<KdAnimation>();

        public ObservableList<KdAnimation> x10Animations = new ObservableList<KdAnimation>();

        public ObservableList<KdAnimation> EventAnimations = new ObservableList<KdAnimation>();

        /// <summary>
        /// Rails
        /// </summary>

        public ObservableList<KdRail> Rails = new ObservableList<KdRail>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        /// <param name="list"></param>
        /// <param name="src"></param>

        private void LoadList(LiveJObj joint, ObservableList<KdPositionList> list, HSDArrayAccessor<KAR_grPositionList> src)
        {
            if (src != null)
                foreach (var m in src.Array)
                    list.Add(new KdPositionList(joint, m));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        public void Load(KAR_grData d)
        {
            var kd = new KdFile(d);
            _data = kd;

            foreach (var m in kd.Collisions)
                Meshes.Add(m);

            foreach (var m in kd.Zones)
                Zones.Add(m);

            if (d.PositionNode != null)
            {
                var positionJoint = d.PositionNode.PositionJoint;
                LiveJObj joint = null;
                if (positionJoint != null)
                    joint = new LiveJObj(positionJoint);

                LoadList(joint, StartPositions, d.PositionNode.Startpos);
                LoadList(joint, EnemyPositions, d.PositionNode.Enemypos);
                LoadList(joint, GravityPositions, d.PositionNode.Gravitypos);
                LoadList(joint, AirFlowPositions, d.PositionNode.Airflowpos);
                LoadList(joint, ConveyorPositions, d.PositionNode.Conveyorpos);
                LoadList(joint, ItemPositions, d.PositionNode.ItemPos);
                LoadList(joint, EventPositions, d.PositionNode.Eventpos);
                LoadList(joint, VehiclePositions, d.PositionNode.Vehiclepos);
                LoadList(joint, GlobalDeadPositions, d.PositionNode.GlobalDeadPos);
                LoadList(joint, LocalDeadPositions, d.PositionNode.LocalDeadPos);
                LoadList(joint, YakumonoPositions, d.PositionNode.Yakumonopos);

                if (d.PositionNode.VehicleAreapos != null)
                {
                    foreach (var e in d.PositionNode.VehicleAreapos.Array)
                    {
                        VehicleAreaPositions.Add(new KdPositionAreaList(joint, e));
                    }
                }

                if (d.PositionNode.ItemAreaPos != null)
                {
                    foreach (var e in d.PositionNode.ItemAreaPos.Array)
                    {
                        ItemAreaPositions.Add(new KdPositionAreaList(joint, e));
                    }
                }

                if (d.RailCollNode != null && 
                    d.SplineNode != null && 
                    d.SplineNode.RailSpline1 != null &&
                    d.SubAnimNode != null &&
                    d.SubAnimNode.Rail != null)
                {
                    var src_rails = d.RailCollNode.RailColl.Array;
                    var splineNode = d.SplineNode.RailSpline1.Splines.Array;
                    var railAnimations = d.SubAnimNode.Rail.Animations.Array;

                    foreach (var t in src_rails)
                        Rails.Add(new KdRail(t, splineNode, railAnimations));
                }

            }

            if (d.SubAnimNode != null)
            {
                if (d.SubAnimNode.SuperJump != null)
                    foreach (var a in d.SubAnimNode.SuperJump.Animations.Array)
                        SuperJumpAnimations.Add(new KdAnimation(a));

                if (d.SubAnimNode.Leap != null)
                    foreach (var a in d.SubAnimNode.Leap.Animations.Array)
                        LeapAnimations.Add(new KdAnimation(a));

                if (d.SubAnimNode.x0C != null)
                    foreach (var a in d.SubAnimNode.x0C.Animations.Array)
                        x0CAnimations.Add(new KdAnimation(a));

                if (d.SubAnimNode.x10 != null)
                    foreach (var a in d.SubAnimNode.x10.Animations.Array)
                        x10Animations.Add(new KdAnimation(a));

                if (d.SubAnimNode.EventAnim != null)
                    foreach (var a in d.SubAnimNode.EventAnim.Animations.Array)
                        EventAnimations.Add(new KdAnimation(a));
            }

            if (d.SplineNode != null)
            {
                if (d.SplineNode.SplineSetup != null)
                {
                    CourseSpline.Load(d.SplineNode.SplineSetup);
                }

                if (d.SplineNode.RangeSplineSetup != null)
                {
                    foreach (var v in d.SplineNode.RangeSplineSetup.Splines)
                    {
                        RangeSplines.Add(new KdRangeSpline(v));
                    }
                }

                if (d.SplineNode.ConveyorSpline != null)
                {
                    foreach (var v in d.SplineNode.ConveyorSpline.SplineList.Splines.Array)
                    {
                        ConveyorSplines.Add(new KdSpline(v));
                    }
                }

                if (d.SplineNode.HeavySplines != null)
                {
                    foreach (var v in d.SplineNode.HeavySplines.Splines.Array)
                    {
                        HeavySplines.Add(new KdSpline(v));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private HSDArrayAccessor<KAR_grPositionList> CreatePositionList(ObservableList<KdPositionList> list)
        {
            if (list.Count() == 0)
                return null;

            return new HSDArrayAccessor<KAR_grPositionList>()
            {
                Array = list.Select(e => e.ToPositionList()).ToArray(),
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animations"></param>
        /// <returns></returns>
        private KAR_grSubAnim GenerateSubanim(IEnumerable<KdAnimation> animations)
        {
            int count = animations.Count();
            if (count > 0)
            {
                return new KAR_grSubAnim()
                {
                    Animations = new HSDFixedLengthPointerArrayAccessor<HSDRaw.Common.Animation.HSD_AnimJoint>
                    {
                        Array = animations.Select(e => e.Animation).ToArray()
                    },
                    Count = count
                };
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="d"></param>
        internal void Save(LiveJObj jobj, KAR_grData d)
        {
            _data.Collisions = Meshes.ToList();
            _data.Zones = Zones.ToList();
            _data.ImportIntoNode(jobj, d);

            d.PositionNode.Startpos = CreatePositionList(StartPositions);
            d.PositionNode.Enemypos = CreatePositionList(EnemyPositions);
            d.PositionNode.Gravitypos = CreatePositionList(GravityPositions);
            d.PositionNode.Airflowpos = CreatePositionList(AirFlowPositions);
            d.PositionNode.Conveyorpos = CreatePositionList(ConveyorPositions);
            d.PositionNode.ItemPos = CreatePositionList(ItemPositions);
            d.PositionNode.Eventpos = CreatePositionList(EventPositions);
            d.PositionNode.Vehiclepos = CreatePositionList(VehiclePositions);
            d.PositionNode.GlobalDeadPos = CreatePositionList(GlobalDeadPositions);
            d.PositionNode.LocalDeadPos = CreatePositionList(LocalDeadPositions);
            d.PositionNode.Yakumonopos = CreatePositionList(YakumonoPositions);

            if (ItemAreaPositions.Count() > 0)
            {
                d.PositionNode.ItemAreaPos = new HSDArrayAccessor<KAR_grAreaPositionList>()
                {
                    Array = ItemAreaPositions.Select(e=>e.ToPositionList()).ToArray()
                };
            }
            else
            {
                d.PositionNode.ItemAreaPos = null;
            }

            if (VehicleAreaPositions.Count() > 0)
            {
                d.PositionNode.VehicleAreapos = new HSDArrayAccessor<KAR_grAreaPositionList>()
                {
                    Array = VehicleAreaPositions.Select(e => e.ToPositionList()).ToArray()
                };
            }
            else
            {
                d.PositionNode.VehicleAreapos = null;
            }


            d.SubAnimNode = new KAR_grSubAnimNode();
            d.SubAnimNode.SuperJump = GenerateSubanim(SuperJumpAnimations);
            d.SubAnimNode.Leap = GenerateSubanim(LeapAnimations);
            d.SubAnimNode.x0C = GenerateSubanim(x0CAnimations);
            d.SubAnimNode.x10 = GenerateSubanim(x10Animations);
            d.SubAnimNode.EventAnim = GenerateSubanim(EventAnimations);


            if (SuperJumpAnimations.Count > 0)
            {
                d.SubAnimNode.SuperJump = new KAR_grSubAnim()
                {
                    Animations = new HSDFixedLengthPointerArrayAccessor<HSDRaw.Common.Animation.HSD_AnimJoint>
                    {
                        Array = SuperJumpAnimations.Select(e=>e.Animation).ToArray()
                    },
                    Count = SuperJumpAnimations.Count
                };
            }

            // Splines

            d.SplineNode.SplineSetup = CourseSpline.Save();

            if (RangeSplines.Count > 0)
            {
                var data = RangeSplines.Select(e => e.ToRangeSpline()).ToArray();
                d.SplineNode.RangeSplineSetup = new KAR_grRangeSplineSetup()
                {
                    Splines = data,
                    Count = RangeSplines.Count
                };
            }
            else
            {
                d.SplineNode.RangeSplineSetup = null;
            }

            if (ConveyorSplines.Count > 0)
            {
                var data = ConveyorSplines.Select(e => e.ToHsdSpline()).ToArray();
                d.SplineNode.ConveyorSpline = new KAR_grConveyorPath()
                {
                    SplineList = new KAR_grSplineList()
                    {
                        Count = data.Length,
                        Splines = new HSDFixedLengthPointerArrayAccessor<HSDRaw.Common.HSD_Spline>()
                        {
                            Array = data
                        }
                    }
                };
            }
            else
            {
                d.SplineNode.ConveyorSpline = null;
            }

            if (HeavySplines.Count > 0)
            {
                var data = HeavySplines.Select(e => e.ToHsdSpline()).ToArray();
                d.SplineNode.HeavySplines = new KAR_grSplineList()
                {
                    Count = data.Length,
                    Splines = new HSDFixedLengthPointerArrayAccessor<HSDRaw.Common.HSD_Spline>()
                    {
                        Array = data
                    }
                };
            }
            else
            {
                d.SplineNode.HeavySplines = null;
            }

            // Rails

            if (Rails.Count > 0)
            {
                List<KdSpline> splines = new List<KdSpline>();
                List<KdAnimation> anims = new List<KdAnimation>();
                d.RailCollNode = new KAR_grRailCollNode()
                {
                    Count = Rails.Count,
                    RailColl = new HSDFixedLengthPointerArrayAccessor<KAR_grRailColl>() { Array = Rails.Select(e => e.ToRailColl(splines, anims)).ToArray() }
                };

                d.SplineNode.RailSpline1 = new KAR_grSplineList()
                {
                    Count = splines.Count,
                    Splines = new HSDFixedLengthPointerArrayAccessor<HSDRaw.Common.HSD_Spline>() { Array = splines.Select(e => e.ToHsdSpline()).ToArray() }
                };

                d.SubAnimNode.Rail = GenerateSubanim(anims);
            }
            else
            {
                d.RailCollNode = null;
            }

            // TODO: ensure bit table has enough entries for all splines...
            var railpoints = Rails.Sum(e => e.Spline1.Points.Count + e.Spline2.Points.Count);

            int bitTableCount = (ushort)Math.Max(railpoints, d.PartitionNode.Partition.BitTableCount);
            int bitTableSize = (int)Math.Ceiling(bitTableCount / 8f);

            d.PartitionNode.Partition.BitTableCount = (ushort)bitTableCount;
            d.PartitionNode.Partition._s.SetBuffer(0x54, new byte[bitTableSize]);
        }
    }
}
