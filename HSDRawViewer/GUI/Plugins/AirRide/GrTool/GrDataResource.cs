using HSDRaw;
using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool
{
    public class GrDataResource
    {
        private KdFile _data;

        public ObservableList<KdMesh> Meshes = new ObservableList<KdMesh>();

        public ObservableList<KdZone> Zones = new ObservableList<KdZone>();

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


        public KdCourseSplineSetup CourseSpline { get; } = new KdCourseSplineSetup();

        private void LoadList(LiveJObj joint, ObservableList<KdPositionList> list, HSDArrayAccessor<KAR_grPositionList> src)
        {
            if (src != null)
                foreach (var m in src.Array)
                    list.Add(new KdPositionList(joint, m));
        }

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
            }

            if (d.SplineNode != null)
            {
                if (d.SplineNode.SplineSetup != null)
                {
                    CourseSpline.Load(d.SplineNode.SplineSetup);
                }
            }
        }

        private HSDArrayAccessor<KAR_grPositionList> CreatePositionList(ObservableList<KdPositionList> list)
        {
            if (list.Count() == 0)
                return null;

            return new HSDRaw.HSDArrayAccessor<KAR_grPositionList>()
            {
                Array = list.Select(e => e.ToPositionList()).ToArray(),
            };
        }

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

            // save spline data
            d.SplineNode.SplineSetup = CourseSpline.Save();
        }
    }
}
