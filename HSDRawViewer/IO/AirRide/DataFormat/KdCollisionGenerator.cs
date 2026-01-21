using HSDRaw.AirRide.Gr.Data;
using HSDRaw.GX;
using System.Collections.Generic;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    internal class KdCollisionGenerator
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
