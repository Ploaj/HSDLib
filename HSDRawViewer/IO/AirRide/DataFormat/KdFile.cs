using HSDRaw;
using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.Converters;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    internal class KdFile
    {
        private readonly string CollisionFile = "_collision.json";
        private readonly string PositionFile = "_positions.json";
        private readonly string SplineFile = "_splines.json";

        public List<KdBone> Bones { get; set; } = new List<KdBone>();

        public List<KdMesh> Collisions { get; set; } = new List<KdMesh>();

        public List<KdZone> Zones { get; set; } = new List<KdZone>();

        // Lights

        // Scene Params?

        // Spline
        public KdSplineData Splines { get; set; }

        // Position
        public List<KdPositionList> Positions { get; set; } = new List<KdPositionList>();

        [JsonPropertyName("area_positions")]
        public List<KdPositionAreaList> PositionsArea { get; set; } = new List<KdPositionAreaList>();

        // Enemy

        // Item

        // Fog

        // Rail Collision

        // Audio

        // YakumonoNode

        // Replay

        // Respawn

        // Stadium

        public KdFile() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public KdFile(KAR_grData data)
        {
            FromCollisionNode(data.CollisionNode);
            FromPositionNode(data.PositionNode);
            Splines = new KdSplineData(data.SplineNode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public void ImportIntoNode(KAR_grData node)
        {
            if (Collisions != null && Collisions.Count > 0)
            {
                if (ToCollisionNode(out KAR_grCollisionNode coll, out KAR_grCollisionTree tree))
                {
                    node.CollisionNode = coll;
                    node.PartitionNode.Partition = tree;
                }
            }

            if ((Positions != null && Positions.Count > 0) ||
                (PositionsArea != null && PositionsArea.Count > 0))
            {
                node.PositionNode = ToPositionNode();
            }

            if (Splines != null)
            {
                node.SplineNode = Splines.ToSplineNode();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coll"></param>
        private void FromCollisionNode(KAR_grCollisionNode coll)
        {
            {
                Collisions.Clear();
                var vertices = coll.Vertices;
                var triangles = coll.Triangles;

                int ji = 0;
                foreach (var n in coll.Joints)
                {
                    KdMesh m = new KdMesh()
                    {
                        Name = $"CollMesh_{ji++.ToString("D3")}",
                        Parent = n.BoneID,
                        Vertices = new List<List<float>>(),
                        Triangles = new List<KdTriangle>(),
                        Materials = new List<KdMaterial>(),
                    };
                    Collisions.Add(m);

                    for (int i = n.VertexStart; i < n.VertexStart + n.VertexSize; i++)
                        m.Vertices.Add(new List<float>() { vertices[i].X, vertices[i].Y, vertices[i].Z });

                    Dictionary<ulong, int> hashToMaterial = new Dictionary<ulong, int>();

                    for (int i = n.FaceStart; i < n.FaceStart + n.FaceSize; i++)
                    {
                        var tri = triangles[i];

                        // generate material if it doesn't exist
                        int a = tri._s.GetInt32(0x0C);
                        int b = tri._s.GetInt32(0x10);
                        ulong hash = ((ulong)(uint)a << 32) | (uint)b;
                        if (!hashToMaterial.ContainsKey(hash))
                        {
                            hashToMaterial.Add(hash, m.Materials.Count);
                            m.Materials.Add(KdMaterial.FromTriangle(tri));
                        }

                        // extract material
                        m.Triangles.Add(new KdTriangle()
                        {
                            Indices = new int[]
                            {
                            tri.V3 - n.VertexStart,
                            tri.V2 - n.VertexStart,
                            tri.V1 - n.VertexStart,
                            },
                            Material = hashToMaterial[hash],
                        });
                    }
                }
            }
            {
                Zones.Clear();
                var vertices = coll.ZoneVertices;
                var triangles = coll.ZoneTriangles;

                int ji = 0;
                foreach (var n in coll.ZoneJoints)
                {
                    KdZone m = new KdZone()
                    {
                        Name = $"Zone_{ji++.ToString("D3")}",
                        Parent = n.BoneID,
                        Vertices = new List<List<float>>(),
                        Triangles = new List<KdZoneTriangle>(),
                    };
                    Zones.Add(m);

                    for (int i = n.ZoneVertexStart; i < n.ZoneVertexStart + n.ZoneVertexSize; i++)
                        m.Vertices.Add(new List<float>() { vertices[i].X, vertices[i].Y, vertices[i].Z });

                    for (int i = n.ZoneFaceStart; i < n.ZoneFaceStart + n.ZoneFaceSize; i++)
                    {
                        var tri = triangles[i];
                        m.Triangles.Add(new KdZoneTriangle()
                        {
                            Indices = new int[]
                            {
                                tri.V3 - n.ZoneVertexStart,
                                tri.V2 - n.ZoneVertexStart,
                                tri.V1 - n.ZoneVertexStart,
                            },
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="tree"></param>
        private bool ToCollisionNode(out KAR_grCollisionNode node, out KAR_grCollisionTree tree)
        {
            // dump data into node
            var gen = new KdCollisionGenerator();
            foreach (var m in Collisions)
                gen.ParseMesh(m);

            // TODO: zones

            // generate collision node
            node = gen.GenerateNode();

            // get bone
            if (Bones == null || Bones.Count == 0)
            {
                node = null;
                tree = null;
                return false;
            }

            // generate partition
            tree = SpatialPartitionOrganizer.GeneratePartition(Bones.Select(e=>e.ToMatrix()).ToArray(), node);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="root"></param>
        /// <param name="list"></param>
        private void AddPositionList(KdPositionKind kind, LiveJObj root, HSDArrayAccessor<KAR_grPositionList> n)
        {
            if (n == null || n.Length == 0)
                return;

            var list = n.Array;
            for (int i = 0; i < list.Length; i++)
                Positions.Add(new KdPositionList(kind, i, root, list[i]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="root"></param>
        /// <param name="list"></param>
        private void AddPositionAreaList(KdAreaPositionKind kind, LiveJObj root, HSDArrayAccessor<KAR_grAreaPositionList> n)
        {
            if (n == null || n.Length == 0)
                return;

            var list = n.Array;
            for (int i = 0; i < list.Length; i++)
                PositionsArea.Add(new KdPositionAreaList(kind, i, root, list[i]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        public void FromPositionNode(KAR_grPositionNode pos)
        {
            var root = pos.PositionJoint != null ? new LiveJObj(pos.PositionJoint) : null;
            AddPositionList(KdPositionKind.START,       root, pos.Startpos);
            AddPositionList(KdPositionKind.ENEMY,       root, pos.Enemypos);
            AddPositionList(KdPositionKind.GRAVITY,     root, pos.Gravitypos);
            AddPositionList(KdPositionKind.AIRFLOW,     root, pos.Airflowpos);
            AddPositionList(KdPositionKind.CONVEYOR,    root, pos.Conveyorpos);
            AddPositionList(KdPositionKind.ITEM,        root, pos.ItemPos);
            AddPositionList(KdPositionKind.EVENT,       root, pos.Eventpos);
            AddPositionList(KdPositionKind.VEHICLE,     root, pos.Vehiclepos);
            AddPositionList(KdPositionKind.GLOBAL_DEAD, root, pos.GlobalDeadPos);
            AddPositionList(KdPositionKind.LOCAL_DEAD,  root, pos.LocalDeadPos);
            AddPositionList(KdPositionKind.YAKUMONO,    root, pos.Yakumonopos);

            AddPositionAreaList(KdAreaPositionKind.ITEM_AREA,       root, pos.ItemAreaPos);
            AddPositionAreaList(KdAreaPositionKind.VEHICLE_AREA,    root, pos.VehicleAreapos);
        }

        private HSDArrayAccessor<KAR_grPositionList> SetPositionInList(KdPositionList list, HSDArrayAccessor<KAR_grPositionList> arr)
        {
            if (arr == null)
                arr = new HSDArrayAccessor<KAR_grPositionList>();

            arr.Set(list.Slot, list.ToPositionList());

            return arr;
        }

        private HSDArrayAccessor<KAR_grAreaPositionList> SetPositionInList(KdPositionAreaList list, HSDArrayAccessor<KAR_grAreaPositionList> arr)
        {
            if (arr == null)
                arr = new HSDArrayAccessor<KAR_grAreaPositionList>();

            arr.Set(list.Slot, list.ToPositionList());

            return arr;
        }

        private KAR_grPositionNode ToPositionNode()
        {
            var node = new KAR_grPositionNode()
            {
                PositionJoint = new HSDRaw.Common.HSD_JOBJ()
            };

            foreach (var p in Positions)
            {
                switch (p.Kind)
                {
                    case KdPositionKind.START:          node.Startpos       = SetPositionInList(p, node.Startpos); break;
                    case KdPositionKind.ENEMY:          node.Enemypos       = SetPositionInList(p, node.Enemypos); break;
                    case KdPositionKind.GRAVITY:        node.Gravitypos     = SetPositionInList(p, node.Gravitypos); break;
                    case KdPositionKind.AIRFLOW:        node.Airflowpos     = SetPositionInList(p, node.Airflowpos); break;
                    case KdPositionKind.CONVEYOR:       node.Conveyorpos    = SetPositionInList(p, node.Conveyorpos); break;
                    case KdPositionKind.ITEM:           node.ItemPos        = SetPositionInList(p, node.ItemPos); break;
                    case KdPositionKind.EVENT:          node.Eventpos       = SetPositionInList(p, node.Eventpos); break;
                    case KdPositionKind.VEHICLE:        node.Vehiclepos     = SetPositionInList(p, node.Vehiclepos); break;
                    case KdPositionKind.GLOBAL_DEAD:    node.GlobalDeadPos  = SetPositionInList(p, node.GlobalDeadPos); break;
                    case KdPositionKind.LOCAL_DEAD:     node.LocalDeadPos   = SetPositionInList(p, node.LocalDeadPos); break;
                    case KdPositionKind.YAKUMONO:       node.Yakumonopos    = SetPositionInList(p, node.Yakumonopos); break;
                }
            }

            foreach (var p in PositionsArea)
            {
                switch (p.Kind)
                {
                    case KdAreaPositionKind.ITEM_AREA:      node.ItemAreaPos = SetPositionInList(p, node.ItemAreaPos); break;
                    case KdAreaPositionKind.VEHICLE_AREA:   node.VehicleAreapos = SetPositionInList(p, node.VehicleAreapos); break;
                }
            }

            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static KdFile OpenKdFile(string filePath)
        {
            return JsonHelper.Import<KdFile>(filePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        public void Save(string filename)
        {
            if (Collisions.Count > 0)
                JsonHelper.Export($"{filename}{CollisionFile}", new KdFile() { Collisions = Collisions, Zones = Zones, Bones = Bones });

            if (Positions.Count > 0)
                JsonHelper.Export($"{filename}{PositionFile}", new KdFile() { Positions = Positions, PositionsArea = PositionsArea });

            if (Splines != null)
                JsonHelper.Export($"{filename}{SplineFile}", new KdFile() { Splines = Splines });
        }
    }
}
