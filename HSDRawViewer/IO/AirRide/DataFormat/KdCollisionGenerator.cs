using HSDRaw;
using HSDRaw.AirRide.Gr.Data;
using HSDRaw.GX;
using System.Collections.Generic;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public class KdCollisionGenerator
    {
        private readonly List<GXVector3> verts = new();
        private readonly List<KAR_CollisionTriangle> triangles = new();
        private readonly List<KAR_CollisionJoint> joints = new();

        private readonly List<GXVector3> zverts = new();
        private readonly List<KAR_ZoneCollisionTriangle> ztriangles = new();
        private readonly List<KAR_ZoneCollisionJoint> zjoints = new();

        public void ParseMesh(KdMesh mesh)
        {
            int vertStart = verts.Count;
            int faceStart = triangles.Count;

            foreach (var v in mesh.Vertices)
                verts.Add(new GXVector3() { X = v[0], Y = v[1], Z = v[2] });

            for (int i = 0; i < mesh.Triangles.Count; i++)
            {
                var t = mesh.Triangles[i];
                var tri = new KAR_CollisionTriangle()
                {
                    V1 = t.Indices[2] + vertStart,
                    V2 = t.Indices[1] + vertStart,
                    V3 = t.Indices[0] + vertStart,
                };
                mesh.Materials[t.Material].SetMaterial(tri);
                triangles.Add(tri);
            }

            joints.Add(new KAR_CollisionJoint()
            {
                BoneID = mesh.Parent,
                FaceStart = faceStart,
                FaceSize = triangles.Count - faceStart,
                VertexStart = vertStart,
                VertexSize = verts.Count - vertStart,
            });
        }

        internal void ParseZone(KdZone zone)
        {
            int vertStart = zverts.Count;
            int faceStart = ztriangles.Count;

            foreach (var v in zone.Vertices)
                zverts.Add(new GXVector3() { X = v[0], Y = v[1], Z = v[2] });

            for (int i = 0; i < zone.Triangles.Count; i++)
            {
                var t = zone.Triangles[i];
                var tri = new KAR_ZoneCollisionTriangle()
                {
                    V1 = t.Indices[2] + vertStart,
                    V2 = t.Indices[1] + vertStart,
                    V3 = t.Indices[0] + vertStart,
                    Flags = zone.Flags,
                    Type = zone.Type,
                    PolyIndex = i / 2,
                    UnknownIndex = (byte)t.UnknownIndex,
                };
                ztriangles.Add(tri);
            }

            var new_zone = new KAR_ZoneCollisionJoint()
            {
                BoneID = zone.Parent,

                ZoneFaceStart = faceStart,
                ZoneFaceSize = ztriangles.Count - faceStart,
                ZoneVertexStart = vertStart,
                ZoneVertexSize = zverts.Count - vertStart,

                x14 = zone.Type1,
                x18 = zone.Type2,

                Mtx00 = zone.Matrix[0],
                Mtx01 = zone.Matrix[1],
                Mtx02 = zone.Matrix[2],
                Mtx10 = zone.Matrix[3],
                Mtx11 = zone.Matrix[4],
                Mtx12 = zone.Matrix[5],
                Mtx20 = zone.Matrix[6],
                Mtx21 = zone.Matrix[7],
                Mtx22 = zone.Matrix[8],
                Mtx30 = zone.Matrix[9],
                Mtx31 = zone.Matrix[10],
                Mtx32 = zone.Matrix[11],
            };

            if (zone.Type1Data != null)
                new_zone.x14_param = (HSDAccessor)zone.Type1Data;

            if (zone.Type2Data != null)
                new_zone.x18_param = (HSDAccessor)zone.Type2Data;

            zjoints.Add(new_zone);
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
