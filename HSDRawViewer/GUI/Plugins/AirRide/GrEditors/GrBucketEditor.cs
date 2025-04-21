using HSDRaw.AirRide.Gr.Data;
using HSDRaw.GX;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Drawing;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrEditors
{
    public class GrBucketEditor : IGrEditor
    {
        public KAR_grPartitionBucket[] _items { get; set; }

        private readonly ushort[] trilookup;

        private readonly GXVector3[] _vertices;
        private readonly KAR_CollisionTriangle[] _triangles;
        private readonly KAR_CollisionJoint[] _joints;

        private readonly GXVector3[] _zvertices;
        private readonly KAR_ZoneCollisionTriangle[] _ztriangles;
        private readonly KAR_ZoneCollisionJoint[] _zjoints;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public GrBucketEditor(KAR_grData data)
        {
            trilookup = data.PartitionNode.Partition.CollidableTriangles;
            _vertices = data.CollisionNode.Vertices;
            _triangles = data.CollisionNode.Triangles;
            _joints = data.CollisionNode.Joints;

            _zvertices = data.CollisionNode.ZoneVertices;
            _ztriangles = data.CollisionNode.ZoneTriangles;
            _zjoints = data.CollisionNode.ZoneJoints;

            _items = data.PartitionNode.Partition.Buckets;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selected"></param>
        public void Render(Camera cam, LiveJObj model, object selected)
        {
            // render buckets
            //foreach (var b in _items)
            //{
            //    if (b == null)
            //        continue;

            //    if (b.Child1 != -1)
            //        continue;

            //    DrawShape.DrawBox(Color.White, b.MinX, b.MinY, b.MinZ, b.MaxX, b.MaxY, b.MaxZ);
            //}

            GL.Enable(EnableCap.DepthTest);
            foreach (KAR_CollisionJoint j in _joints)
            {
                GL.MatrixMode(MatrixMode.Modelview);
                GL.PushMatrix();
                if (model != null)
                {
                    Matrix4 trans = model.GetJObjAtIndex(j.BoneID).WorldTransform;
                    GL.MultMatrix(ref trans);
                }

                GL.Begin(PrimitiveType.Triangles);
                for (int i = j.FaceStart; i < j.FaceStart + j.FaceSize; i++)
                {
                    KAR_CollisionTriangle tri = _triangles[i];

                    GXVector3 v1 = _vertices[tri.V3];
                    GXVector3 v2 = _vertices[tri.V2];
                    GXVector3 v3 = _vertices[tri.V1];

                    //GL.Color4(new Vector4(CalculateSurfaceNormal(v1, v2, v3), 0.5f));
                    if (tri.Flags.HasFlag(KCCollFlag.Wall))
                    {
                        GL.Color3(0f, 0f, 1f);
                    }
                    else
                    if (tri.Flags.HasFlag(KCCollFlag.Ceiling))
                    {
                        GL.Color3(1f, 0f, 0f);
                    }
                    else
                    if (tri.Flags.HasFlag(KCCollFlag.Floor))
                    {
                        GL.Color3(0f, 1f, 0f);
                    }
                    else
                    {
                        GL.Color3(0f, 0f, 0f);
                    }

                    GL.Vertex3(v1.X, v1.Y, v1.Z);
                    GL.Vertex3(v2.X, v2.Y, v2.Z);
                    GL.Vertex3(v3.X, v3.Y, v3.Z);
                }
                GL.End();

                GL.Begin(PrimitiveType.Lines);
                for (int i = j.FaceStart; i < j.FaceStart + j.FaceSize; i++)
                {
                    KAR_CollisionTriangle tri = _triangles[i];

                    GXVector3 v1 = _vertices[tri.V3];
                    GXVector3 v2 = _vertices[tri.V2];
                    GXVector3 v3 = _vertices[tri.V1];

                    GL.Color3(1f, 1f, 1f);

                    GL.Vertex3(v1.X, v1.Y, v1.Z);
                    GL.Vertex3(v2.X, v2.Y, v2.Z);

                    GL.Vertex3(v2.X, v2.Y, v2.Z);
                    GL.Vertex3(v3.X, v3.Y, v3.Z);

                    GL.Vertex3(v3.X, v3.Y, v3.Z);
                    GL.Vertex3(v1.X, v1.Y, v1.Z);
                }
                GL.End();

                GL.PopMatrix();
            }


            //foreach (var j in _zjoints)
            //if (selected is KAR_ZoneCollisionJoint j)
            //{
            //    GL.MatrixMode(MatrixMode.Modelview);
            //    GL.PushMatrix();
            //    if (model != null)
            //    {
            //        Matrix4 trans = model.GetJObjAtIndex(j.BoneID).WorldTransform;
            //        GL.MultMatrix(ref trans);
            //    }

            //    GL.Begin(PrimitiveType.Triangles);
            //    for (int i = j.ZoneFaceStart; i < j.ZoneFaceStart + j.ZoneFaceSize; i++)
            //    {
            //        var tri = _ztriangles[i];

            //        var v1 = _zvertices[tri.V3];
            //        var v2 = _zvertices[tri.V2];
            //        var v3 = _zvertices[tri.V1];

            //        GL.Color4(new Vector4(CalculateSurfaceNormal(v1, v2, v3), 0.5f));

            //        GL.Vertex3(v1.X, v1.Y, v1.Z);
            //        GL.Vertex3(v2.X, v2.Y, v2.Z);
            //        GL.Vertex3(v3.X, v3.Y, v3.Z);
            //    }
            //    GL.End();

            //    GL.PopMatrix();
            //}

            GL.Clear(ClearBufferMask.DepthBufferBit);

            // render buckets
            foreach (KAR_grPartitionBucket b in _items)
            {
                if (b == null)
                    continue;

                if (b == (KAR_grPartitionBucket)selected)
                {
                    GL.Enable(EnableCap.DepthTest);
                    GL.Begin(PrimitiveType.Triangles);
                    for (int i = b.CollTriangleStart; i < b.CollTriangleStart + b.CollTriangleCount; i++)
                    {
                        ushort index = trilookup[i];
                        KAR_CollisionTriangle tri = _triangles[index];

                        GXVector3 v1 = _vertices[tri.V3];
                        GXVector3 v2 = _vertices[tri.V2];
                        GXVector3 v3 = _vertices[tri.V1];

                        GL.Color3(CalculateSurfaceNormal(v1, v2, v3));

                        GL.Vertex3(v1.X, v1.Y, v1.Z);
                        GL.Vertex3(v2.X, v2.Y, v2.Z);
                        GL.Vertex3(v3.X, v3.Y, v3.Z);
                    }
                    GL.End();

                    GL.Disable(EnableCap.DepthTest);
                    DrawShape.DrawBox(Color.Yellow, b.MinX, b.MinY, b.MinZ, b.MaxX, b.MaxY, b.MaxZ);
                }
            }
        }

        private static Vector3 CalculateSurfaceNormal(
            GXVector3 v1,
            GXVector3 v2,
            GXVector3 v3)
        {
            return Math3D.CalculateSurfaceNormal(
                new Vector3(v1.X, v1.Y, v1.Z),
                new Vector3(v2.X, v2.Y, v2.Z),
                new Vector3(v3.X, v3.Y, v3.Z));
        }

        public void Update()
        {

        }
    }
}
