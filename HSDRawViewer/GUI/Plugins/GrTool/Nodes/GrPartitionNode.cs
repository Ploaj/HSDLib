using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.GUI.Plugins.AirRide.GrTool;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using IONET.Collada.Core.Geometry;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrPartitionNode : GrDrawNode
    {
        public class DebugTriangle
        {
            public int Joint;
            public bool Cached = false;

            public Vector3 p1;
            public Vector3 p2;
            public Vector3 p3;
        }

        public class DebugInfo
        {
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public KAR_grPartitionBucket _bucket { get; set; }

            public HashSet<int> zones { get; } = new HashSet<int>();

            public List<DebugTriangle> triangles { get; } = new List<DebugTriangle>();

            public int TriangleCount { get; set; }

            public int RoughCount { get; set; }
        }

        private DebugInfo _debug;

        public GrPartitionNode(KAR_grData d)
        {
            var buckets = d.PartitionNode.Partition.Buckets;
            var zones = d.PartitionNode.Partition.ZoneIndices;
            var tris = d.PartitionNode.Partition.CollidableTriangles;

            var tri = new Dictionary<int, DebugTriangle>();
            var js = d.CollisionNode.Joints;
            var fs = d.CollisionNode.Triangles;
            var vs = d.CollisionNode.Vertices.Select(e => new Vector3(e.X, e.Y, e.Z)).ToArray();

            foreach (var j in js)
            {
                var bone = j.BoneID;

                for (int i = j.FaceStart; i < j.FaceStart + j.FaceSize; i++)
                {
                    var f = fs[i];

                    if (!tri.ContainsKey(i))
                    {
                        tri.Add(i, new DebugTriangle()
                        {
                            Joint = bone,
                            p1 = vs[f.V1],
                            p2 = vs[f.V2],
                            p3 = vs[f.V3],
                        });
                    }
                }
            }

            Setup(0, buckets[0], buckets, zones, tris, tri);
        }

        public GrPartitionNode(int index, KAR_grPartitionBucket bucket, KAR_grPartitionBucket[] buckets, ushort[] zones, ushort[] ti, Dictionary<int, DebugTriangle> tris)
        {
            Setup(0, bucket, buckets, zones, ti, tris);
        }

        private void Setup(int index, KAR_grPartitionBucket bucket, KAR_grPartitionBucket[] buckets, ushort[] zones, ushort[] ti, Dictionary<int, DebugTriangle> tris) 
        {
            Text = $"{index:D3}_{bucket.CollTriangleCount}";
            _debug = new DebugInfo()
            {
                _bucket = bucket,
                TriangleCount = bucket.CollTriangleCount,
                RoughCount = bucket.RoughCount,

            };
            Tag = _debug;

            for (int i = bucket.CollTriangleStart; i < bucket.CollTriangleStart + bucket.CollTriangleCount; i++)
            {
                if (tris.ContainsKey(ti[i]))
                {
                    _debug.triangles.Add(tris[ti[i]]);
                }
            }
;
            for (int i = bucket.ZoneIndexStart; i < bucket.ZoneIndexStart + bucket.ZoneIndexCount; i++)
            {
                _debug.zones.Add(zones[i]);
            }

            if (bucket.Child1 != -1)
            {
                Nodes.Add(new GrPartitionNode(bucket.Child1, buckets[bucket.Child1], buckets, zones, ti, tris));
            }

            if (bucket.Child2 != -1)
            {
                Nodes.Add(new GrPartitionNode(bucket.Child2, buckets[bucket.Child2], buckets, zones, ti, tris));
            }
        }

        public bool ContainsZone(int zone)
        {
            return _debug.zones.Contains(zone);
        }

        public override bool TryPickNode(PickInformation pick, LiveJObj joint, out float distance)
        {
            distance = float.PositiveInfinity;
            return false;
        }

        public override object PickData(PickInformation pick, LiveJObj joint)
        {
            return null;
        }

        public override void Draw(GrRenderResource render, object selected_object)
        {
            if (selected_object == _debug)
            {
                DrawShape.DrawBox(Color.White, _debug._bucket.MinX, _debug._bucket.MinY, _debug._bucket.MinZ, _debug._bucket.MaxX, _debug._bucket.MaxY, _debug._bucket.MaxZ);
            }

            if (!IsSelected) return;

            

            GL.Color4(1f, 1f, 1f, 0.5f);
            GL.Begin(PrimitiveType.Triangles);
            foreach (var t in _debug.triangles)
            {
                if (!t.Cached)
                {
                    var trans = render.Joints.GetJObjAtIndex(t.Joint).WorldTransform;
                    t.p1 = Vector3.TransformPosition(t.p1, trans);
                    t.p2 = Vector3.TransformPosition(t.p2, trans);
                    t.p3 = Vector3.TransformPosition(t.p3, trans);
                    t.Cached = true;
                }
                GL.Vertex3(t.p3);
                GL.Vertex3(t.p2);
                GL.Vertex3(t.p1);
            }
            GL.End();

            GL.Color4(0, 0, 0, 0.5f);
            GL.Begin(PrimitiveType.Lines);
            foreach (var t in _debug.triangles)
            {
                GL.Vertex3(t.p1);
                GL.Vertex3(t.p2);

                GL.Vertex3(t.p2);
                GL.Vertex3(t.p3);

                GL.Vertex3(t.p3);
                GL.Vertex3(t.p1);
            }
            GL.End();
        }

        public override void DrawOverlay(GrRenderResource render, object selected_object)
        {
        }
    }
}
