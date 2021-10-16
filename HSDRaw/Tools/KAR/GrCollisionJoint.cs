using HSDRaw.AirRide.Gr.Data;
using HSDRaw.GX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HSDRaw.Tools.KAR
{
    public class GrCollision
    {
        public List<GrCollisionJoint> CollisionJoints { get; } = new List<GrCollisionJoint>();

        public List<GrZoneJoint> ZoneJoints { get; } = new List<GrZoneJoint>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public void LoadFromCollision(KAR_grCollisionNode node)
        {
            LoadCollisions(node);
            LoadZones(node);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void LoadCollisions(KAR_grCollisionNode node)
        {
            var vertices = node.Vertices;
            var faces = node.Triangles;

            foreach (var j in node.Joints)
            {
                // create collision joint
                var c = new GrCollisionJoint()
                {
                    JointIndex = j.BoneID,
                    Flags = j.Flags
                };

                // unknown data
                if (j._s.References.ContainsKey(0x18))
                    c.Unknown = j._s.References[0x18];

                // convert faces
                for (int i = j.FaceStart; i < j.FaceStart + j.FaceSize; i++)
                {
                    var face = faces[i];
                    c.Faces.Add(new GrCollisionFace()
                    {
                        Flags = face.Flags,
                        Material = face.Material,
                        p0 = vertices[face.V1],
                        p1 = vertices[face.V2],
                        p2 = vertices[face.V3],
                    });
                }

                // add to collection
                CollisionJoints.Add(c);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void LoadZones(KAR_grCollisionNode node)
        {
            var vertices = node.ZoneVertices;
            var faces = node.ZoneTriangles;

            foreach (var j in node.ZoneJoints)
            {
                // create collision joint
                var c = new GrZoneJoint()
                {
                    JointIndex = j.BoneID,
                };
                c.Mtx[0] = j.Mtx00;
                c.Mtx[1] = j.Mtx10;
                c.Mtx[2] = j.Mtx20;
                c.Mtx[3] = j.Mtx30;
                c.Mtx[4] = j.Mtx01;
                c.Mtx[5] = j.Mtx11;
                c.Mtx[6] = j.Mtx21;
                c.Mtx[7] = j.Mtx31;
                c.Mtx[8] = j.Mtx02;
                c.Mtx[9] = j.Mtx12;
                c.Mtx[10] = j.Mtx22;
                c.Mtx[11] = j.Mtx32;

                // unknown data
                if (j._s.References.ContainsKey(0x14))
                    c.Unknown = j._s.References[0x14];

                if (j._s.References.ContainsKey(0x18))
                    c.Params = j._s.References[0x18];

                // convert faces
                for (int i = j.ZoneFaceStart; i < j.ZoneFaceStart + j.ZoneFaceSize; i++)
                {
                    var face = faces[i];
                    c.Faces.Add(new GrZoneFace()
                    {
                        CollFlag = face.CollFlags,
                        TypeFlag = face.TypeFlags,
                        UnknownFlag = face.UnkFlags,
                        p0 = vertices[face.V1],
                        p1 = vertices[face.V2],
                        p2 = vertices[face.V3],
                    });
                }

                // add to collection
                ZoneJoints.Add(c);
            }
        }
    
        /// <summary>
        /// 
        /// </summary>
        public void ExportCollisions(out KAR_grCollisionNode collNode, out KAR_grCollisionTreeNode treeNode)
        {
            collNode = new KAR_grCollisionNode();
            treeNode = new KAR_grCollisionTreeNode();

            // collect triangles and optimize points
            KAR_CollisionJoint[] collJoints = new KAR_CollisionJoint[CollisionJoints.Count];

            // collect points and faces
            List<GXVector3> points = new List<GXVector3>();
            List<KAR_CollisionTriangle> faces = new List<KAR_CollisionTriangle>();

            // process collision joints
            var faceIndex = 0;
            var vertexIndex = 0;
            for (int i = 0; i < collJoints.Length; i++)
            {
                // convert collision joint
                var j = CollisionJoints[i];
                var joint = new KAR_CollisionJoint()
                {
                    BoneID = j.JointIndex,
                    Flags = j.Flags,
                    VertexStart = vertexIndex,
                    FaceStart = faceIndex,
                };
                if (j.Unknown != null)
                    joint._s.SetReferenceStruct(0x18, j.Unknown);
                collJoints[i] = joint;

                // add points and triangles
                Dictionary<GXVector3, int> pointToIndex = new Dictionary<GXVector3, int>();
                foreach (var tri in j.Faces)
                {
                    // generate and add face
                    var face = new KAR_CollisionTriangle()
                    {
                        Flags = tri.Flags,
                        Material = tri.Material,
                        V1 = GetPointIndex(tri.p0, ref pointToIndex, ref points),
                        V2 = GetPointIndex(tri.p1, ref pointToIndex, ref points),
                        V3 = GetPointIndex(tri.p2, ref pointToIndex, ref points),
                    };
                    faces.Add(face);
                }

                // update vertex and face sizes
                joint.FaceSize = j.Faces.Count;
                joint.VertexSize = pointToIndex.Count;

                // increment vertex and face size
                vertexIndex += joint.VertexSize;
                faceIndex += joint.FaceSize;
            }

            // generate partition buckets
            treeNode.Partition = GenerateBucketPartition(points, faces);

            collNode.Vertices = points.ToArray();
            collNode.VertexCount = points.Count;
            collNode.Triangles = faces.ToArray();
            collNode.TriangleCount = faces.Count;
            collNode.Joints = collJoints;
            collNode.JointCount = collJoints.Length;

            ProcessZones(collNode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collNode"></param>
        private void ProcessZones(KAR_grCollisionNode collNode)
        {
            // collect triangles and optimize points
            KAR_ZoneCollisionJoint[] joints = new KAR_ZoneCollisionJoint[ZoneJoints.Count];

            // collect points and faces
            List<GXVector3> points = new List<GXVector3>();
            List<KAR_ZoneCollisionTriangle> faces = new List<KAR_ZoneCollisionTriangle>();

            // process collision joints
            var faceIndex = 0;
            var vertexIndex = 0;
            for (int i = 0; i < joints.Length; i++)
            {
                // convert collision joint
                var j = ZoneJoints[i];
                var joint = new KAR_ZoneCollisionJoint()
                {
                    BoneID = j.JointIndex,
                    Mtx00 = j.Mtx[0],
                    Mtx10 = j.Mtx[1],
                    Mtx20 = j.Mtx[2],
                    Mtx30 = j.Mtx[3],
                    Mtx01 = j.Mtx[4],
                    Mtx11 = j.Mtx[5],
                    Mtx21 = j.Mtx[6],
                    Mtx31 = j.Mtx[7],
                    Mtx02 = j.Mtx[8],
                    Mtx12 = j.Mtx[9],
                    Mtx22 = j.Mtx[10],
                    Mtx32 = j.Mtx[11],
                    ZoneVertexStart = vertexIndex,
                    ZoneFaceStart = faceIndex,
                };
                if (j.Unknown != null)
                    joint._s.SetReferenceStruct(0x14, j.Unknown);
                if (j.Params != null)
                    joint._s.SetReferenceStruct(0x18, j.Params);
                joints[i] = joint;

                // add points and triangles
                Dictionary<GXVector3, int> pointToIndex = new Dictionary<GXVector3, int>();
                foreach (var tri in j.Faces)
                {
                    // generate and add face
                    var face = new KAR_ZoneCollisionTriangle()
                    {
                        CollFlags = tri.CollFlag,
                        TypeFlags = tri.TypeFlag,
                        UnkFlags = tri.UnknownFlag,
                        V1 = GetPointIndex(tri.p0, ref pointToIndex, ref points),
                        V2 = GetPointIndex(tri.p1, ref pointToIndex, ref points),
                        V3 = GetPointIndex(tri.p2, ref pointToIndex, ref points),
                    };
                    faces.Add(face);
                }

                // update vertex and face sizes
                joint.ZoneFaceSize = j.Faces.Count;
                joint.ZoneVertexSize = pointToIndex.Count;

                // increment vertex and face size
                vertexIndex += joint.ZoneVertexSize;
                faceIndex += joint.ZoneFaceSize;
            }

            // 
            collNode.ZoneVertices = points.ToArray();
            collNode.ZoneVertexCount = points.Count;
            collNode.ZoneTriangles = faces.ToArray();
            collNode.ZoneTriangleCount = faces.Count;
            collNode.ZoneJoints = joints;
            collNode.ZoneJointCount = joints.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coll"></param>
        /// <returns></returns>
        private KAR_grCollisionTree GenerateBucketPartition(List<GXVector3> v, List<KAR_CollisionTriangle> faces)
        {
            var bucketGen = new Bucket(
                v.Min(e => e.X) - 10,
                Math.Min(-1000, v.Min(e => e.Y) - 10),
                v.Min(e => e.Z) - 10,
                v.Max(e => e.X) + 10,
                Math.Max(1000, v.Min(e => e.Y) + 10),
                v.Max(e => e.Z) + 10);

            foreach (var t in faces)
                bucketGen.AddTriangle(t, v[t.V1], v[t.V2], v[t.V3]);

            return bucketGen.ProcessBuckets(faces, ZoneJoints);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="index"></param>
        /// <param name="pointToIndex"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        private static int GetPointIndex(GXVector3 p, ref Dictionary<GXVector3, int> pointToIndex, ref List<GXVector3> points)
        {
            if (!pointToIndex.ContainsKey(p))
            {
                pointToIndex.Add(p, points.Count);
                points.Add(p);
            }

            return pointToIndex[p];
        }
    }

    public class GrFace
    {
        public GXVector3 p0;
        public GXVector3 p1;
        public GXVector3 p2;
    }

    public class GrCollisionFace : GrFace
    {
        public int Flags;
        public int Material;
    }

    public class GrCollisionJoint
    {
        public int JointIndex;

        public int Flags;

        public HSDStruct Unknown;

        public List<GrCollisionFace> Faces = new List<GrCollisionFace>();
    }

    public class GrZoneFace : GrFace
    {
        public int CollFlag;
        public int TypeFlag;
        public int UnknownFlag;
    }

    public class GrZoneJoint
    {
        public int JointIndex;

        public float[] Mtx { get; } = new float[12];

        public HSDStruct Unknown;

        public HSDStruct Params;

        public List<GrZoneFace> Faces = new List<GrZoneFace>();

    }
}
