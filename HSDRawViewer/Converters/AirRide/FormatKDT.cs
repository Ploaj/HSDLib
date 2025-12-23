using HSDRaw.AirRide.Gr.Data;
using HSDRaw.GX;
using HSDRawViewer.Tools;
using HSDRawViewer.Tools.Animation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HSDRawViewer.Converters.AirRide
{
    public static class FormatKDT
    {
        public enum KdType
        {
            NONE,
            FLOOR,
            CEILING,
            WALL,
            UNKNOWN,
        }

        public enum KdConveyor
        {
            NONE,
            FORWARD,
            BACKWARD,
            LEFT,
            RIGHT,
        }

        public class KdMaterial
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("type")]
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public KdType Type { get; set; }

            [JsonPropertyName("cmn")]
            public byte CommonType { get; set; }

            [JsonPropertyName("fric")]
            public byte Friction { get; set; }

            [JsonPropertyName("r1")]
            public byte Restitution { get; set; }

            [JsonPropertyName("r2")]
            public byte Restitution2 { get; set; }

            [JsonPropertyName("seg")]
            public bool SegmentMove { get; set; }

            [JsonPropertyName("conv")]
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public KdConveyor ConveyorVertical { get; set; }

            [JsonPropertyName("conh")]
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public KdConveyor ConveyorHorizontal { get; set; }

            public KCCollFlag GetRealFlag()
            {
                switch (Type)
                {
                    case KdType.FLOOR: return KCCollFlag.Floor;
                    case KdType.CEILING: return KCCollFlag.Ceiling;
                    case KdType.WALL: return KCCollFlag.Wall;
                    case KdType.UNKNOWN: return KCCollFlag.Unknown;
                    default: return KCCollFlag.None;
                }
            }

            public KCConveyorDirection GetConveyorFlag()
            {
                KCConveyorDirection dir = (KCConveyorDirection)0;

                switch (ConveyorHorizontal)
                {
                    case KdConveyor.LEFT: dir |= KCConveyorDirection.DirLeft; break;
                    case KdConveyor.RIGHT: dir |= KCConveyorDirection.DirRight; break;
                }

                switch (ConveyorVertical)
                {
                    case KdConveyor.FORWARD: dir |= KCConveyorDirection.DirFront; break;
                    case KdConveyor.BACKWARD: dir |= KCConveyorDirection.DirBack; break;
                }

                return dir;
            }

            public void SetRealFlag(KCCollFlag f)
            {
                switch (f)
                {
                    case KCCollFlag.Floor: Type = KdType.FLOOR; break;
                    case KCCollFlag.Ceiling: Type = KdType.CEILING; break;
                    case KCCollFlag.Wall: Type = KdType.WALL; break;
                    case KCCollFlag.Unknown: Type = KdType.UNKNOWN; break;
                    default: Type = KdType.NONE; break;
                }
            }

            public void SetConveyorFlag(KCConveyorDirection dir)
            {
                if (dir.HasFlag(KCConveyorDirection.DirBack))
                    ConveyorVertical = KdConveyor.BACKWARD;
                else
                if (dir.HasFlag(KCConveyorDirection.DirFront))
                    ConveyorVertical = KdConveyor.FORWARD;
                else
                    ConveyorVertical = KdConveyor.NONE;

                if (dir.HasFlag(KCConveyorDirection.DirLeft))
                    ConveyorHorizontal = KdConveyor.LEFT;
                else
                if (dir.HasFlag(KCConveyorDirection.DirRight))
                    ConveyorHorizontal = KdConveyor.RIGHT;
                else
                    ConveyorHorizontal = KdConveyor.NONE;
            }

            public static KdMaterial FromTriangle(KAR_CollisionTriangle t)
            {
                var m = new KdMaterial()
                {
                    CommonType = t.GrCommonIndex,
                    Friction = t.Rough,
                    Restitution = t.StageNodeReflectIndex,
                    Restitution2 = t.StageNodeForceReflectIndex,
                    SegmentMove = t.SegmentMove,
                };
                m.SetRealFlag(t.Flags);
                m.SetConveyorFlag(t.ConveyorDirection);
                return m;
            }

            public void SetMaterial(KAR_CollisionTriangle v)
            {
                v.Flags = GetRealFlag();
                v.GrCommonIndex = CommonType;
                v.Rough = Friction;
                v.StageNodeReflectIndex = Restitution;
                v.StageNodeForceReflectIndex = Restitution2;
                v.ConveyorDirection = GetConveyorFlag();
                v.SegmentMove = SegmentMove;
            }
        }

        public class KdTriangle
        {
            [JsonPropertyName("v")]
            public int[] Indices { get; set; }

            [JsonPropertyName("mat")]
            public int Material { get; set; }
        }

        public class KdMesh
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("parent")]
            public string Parent { get; set; }

            [JsonPropertyName("vertices")]
            public List<List<float>> Vertices { get; set; }

            [JsonPropertyName("triangles")]
            public List<KdTriangle> Triangles { get; set; }

            [JsonPropertyName("materials")]
            public List<KdMaterial> Materials { get; set; }
        }

        private static JsonSerializerOptions _settings = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
        };


        public static void ReadKDT(string filePath, out KAR_grCollisionNode node, out KAR_grCollisionTree tree)
        {
            string jsonString = File.ReadAllText(filePath);
            KdMesh[] meshes = JsonSerializer.Deserialize<KdMesh[]>(jsonString, _settings);

            // todo load joint map
            JointMap map = new JointMap();

            // dump data into node
            var gen = new KdCollisionGenerator();
            foreach (var m in meshes)
                gen.ParseMesh(m, map);

            // TODO: zones

            // generate collision node
            node = gen.GenerateNode();

            // generate partition
            tree = SpatialPartitionOrganizer.GeneratePartition(null, node);
        }

        public static void WriteKDT(string filepath, KAR_grCollisionNode node)
        {
            List<KdMesh> meshes = new List<KdMesh>();
            var vertices = node.Vertices;
            var triangles = node.Triangles;

            int ji = 0;
            foreach (var n in node.Joints)
            {
                KdMesh m = new KdMesh()
                {
                    Name = $"CollMesh_{ji++.ToString("D3")}",
                    Parent = $"Joint_{n.BoneID}",
                    Vertices = new List<List<float>>(),
                    Triangles = new List<KdTriangle>(),
                    Materials = new List<KdMaterial>(),
                };
                meshes.Add(m);

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

                // TODO: zones
            }

            File.WriteAllText(filepath, JsonSerializer.Serialize(meshes, _settings));
        }

        private class KdCollisionGenerator
        {
            private readonly List<GXVector3> verts = new();
            private readonly List<KAR_CollisionTriangle> triangles = new();
            private readonly List<KAR_CollisionJoint> joints = new();

            private readonly List<GXVector3> zverts = new();
            private readonly List<KAR_ZoneCollisionTriangle> ztriangles = new();
            private readonly List<KAR_ZoneCollisionJoint> zjoints = new();

            public void ParseMesh(KdMesh mesh, JointMap map)
            {
                int vertStart = verts.Count;
                int faceStart = triangles.Count;

                foreach (var v in mesh.Vertices)
                    verts.Add(new GXVector3() { X = v[0], Y = v[1], Z = v[2] });

                for (int i = 0; i < mesh.Triangles.Count; i ++)
                {
                    var t = mesh.Triangles[i];
                    var tri = new KAR_CollisionTriangle()
                    {
                        V1 = t.Indices[2] - vertStart,
                        V2 = t.Indices[1] - vertStart,
                        V3 = t.Indices[0] - vertStart,
                    };
                    mesh.Materials[t.Material].SetMaterial(tri);
                    triangles.Add(tri);
                }

                int parent_index = map.IndexOf(mesh.Parent);
                if (parent_index < 0)
                    parent_index = 0;

                joints.Add(new KAR_CollisionJoint()
                {
                    BoneID = parent_index,
                    FaceStart = faceStart,
                    FaceSize = triangles.Count - faceStart,
                    VertexStart = vertStart,
                    VertexSize = verts.Count - vertStart,
                });
            }

            public KAR_grCollisionNode GenerateNode()
            {
                // create new collision node
                return new KAR_grCollisionNode()
                {
                    Vertices = verts.ToArray(),
                    Triangles = triangles.ToArray(),
                    Joints = joints.ToArray(),

                    ZoneVertices = zverts.Count > 0 ? zverts.ToArray() : null,
                    ZoneTriangles = ztriangles.Count > 0 ? ztriangles.ToArray() : null,
                    ZoneJoints = zjoints.Count > 0 ? zjoints.ToArray() : null,
                };
            }
        }

    }
}
