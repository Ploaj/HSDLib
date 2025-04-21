using HSDRaw;
using HSDRaw.AirRide.Gr.Data;
using HSDRaw.Common;
using HSDRaw.GX;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HSDRawViewer.Converters.AirRide
{
    public class KCLConv
    {
        public static float Scale = 0.01f;
        public static float YTrans = -150;

        public static KAR_grCollisionNode KCLtoKAR(string kclFile, out KAR_grCollisionTree tree)
        {
            KAR_grCollisionNode node = new();

            List<KAR_CollisionTriangle> tris = new();
            List<GXVector3> verts = new();

            using (FileStream f = new(kclFile, FileMode.Open))
            using (BinaryReaderExt r = new(f))
            {
                r.BigEndian = true;

                int posOffset = r.ReadInt32();
                int nrmOffset = r.ReadInt32();
                int triOffset = r.ReadInt32() + 0x10;
                int partOffste = r.ReadInt32();

                int triCount = (partOffste - triOffset) / 0x10;
                for (int i = 0; i < triCount; i++)
                {
                    r.Seek((uint)(triOffset + i * 0x10));

                    float length = r.ReadSingle();
                    ushort pi = r.ReadUInt16();
                    ushort di = r.ReadUInt16();
                    ushort n1 = r.ReadUInt16();
                    ushort n2 = r.ReadUInt16();
                    ushort n3 = r.ReadUInt16();
                    ushort fl = r.ReadUInt16();

                    r.Seek((uint)(posOffset + pi * 0xC));
                    Vector3 position = new(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());

                    r.Seek((uint)(nrmOffset + di * 0xC));
                    Vector3 direction = new(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());

                    r.Seek((uint)(nrmOffset + n1 * 0xC));
                    Vector3 normalA = new(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());

                    r.Seek((uint)(nrmOffset + n2 * 0xC));
                    Vector3 normalB = new(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());

                    r.Seek((uint)(nrmOffset + n3 * 0xC));
                    Vector3 normalC = new(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());

                    Vector3 crossA = Vector3.Cross(normalA, direction);
                    Vector3 crossB = Vector3.Cross(normalB, direction);
                    Vector3 vertex1 = position;
                    Vector3 vertex2 = position + crossB * (length / Vector3.Dot(crossB, normalC));
                    Vector3 vertex3 = position + crossA * (length / Vector3.Dot(crossA, normalC));

                    tris.Add(new KAR_CollisionTriangle()
                    {
                        Flags = KCCollFlag.Floor,
                        GrCommonIndex = 8,
                        V1 = verts.Count + 2,
                        V2 = verts.Count + 1,
                        V3 = verts.Count
                    });

                    // scale
                    vertex1 *= Scale;
                    vertex2 *= Scale;
                    vertex3 *= Scale;

                    vertex1.Y += YTrans;
                    vertex2.Y += YTrans;
                    vertex3.Y += YTrans;

                    verts.Add(new GXVector3() { X = vertex1.X, Y = vertex1.Y, Z = vertex1.Z });
                    verts.Add(new GXVector3() { X = vertex2.X, Y = vertex2.Y, Z = vertex2.Z });
                    verts.Add(new GXVector3() { X = vertex3.X, Y = vertex3.Y, Z = vertex3.Z });
                }
            }

            {
                float height = verts.Min(e => e.Y) - 10;

                Vector3 v1 = new(-10000, height, -10000);
                Vector3 v2 = new(10000, height, -10000);
                Vector3 v3 = new(10000, height, 10000);
                Vector3 v4 = new(-10000, height, 10000);

                tris.Add(new KAR_CollisionTriangle()
                {
                    Flags = KCCollFlag.Floor,
                    GrCommonIndex = 8,
                    V1 = verts.Count,
                    V2 = verts.Count + 1,
                    V3 = verts.Count + 2
                });

                verts.Add(new GXVector3() { X = v1.X, Y = v1.Y, Z = v1.Z });
                verts.Add(new GXVector3() { X = v2.X, Y = v2.Y, Z = v2.Z });
                verts.Add(new GXVector3() { X = v3.X, Y = v3.Y, Z = v3.Z });

                tris.Add(new KAR_CollisionTriangle()
                {
                    Flags = KCCollFlag.Floor,
                    GrCommonIndex = 8,
                    V1 = verts.Count,
                    V2 = verts.Count + 1,
                    V3 = verts.Count + 2
                });

                verts.Add(new GXVector3() { X = v1.X, Y = v1.Y, Z = v1.Z });
                verts.Add(new GXVector3() { X = v3.X, Y = v3.Y, Z = v3.Z });
                verts.Add(new GXVector3() { X = v4.X, Y = v4.Y, Z = v4.Z });
            }

            node.Triangles = tris.ToArray();
            node.Vertices = verts.ToArray();
            node.Joints = new KAR_CollisionJoint[]
            {
                new()
                {
                    VertexStart = 0,
                    VertexSize = verts.Count,
                    FaceStart = 0,
                    FaceSize = tris.Count
                }
            };

            tree = null; // new HSDRaw.Tools.KAR.GrCollisionTreeGenerator(node).Generate();

            return node;
        }

        public static HSD_Spline KMP_ExtractRouteSpline(string kmpFile)
        {
            List<HSD_Vector3> points = new();

            using (FileStream f = new(kmpFile, FileMode.Open))
            using (BinaryReaderExt r = new(f))
            {
                r.BigEndian = true;

                r.Seek(0x14);
                uint enpt = r.ReadUInt32();

                r.Seek(enpt + 0x4C);
                r.Skip(4);
                int count = r.ReadInt16();
                int unk = r.ReadInt16();

                for (int i = 0; i < count; i++)
                {
                    points.Add(new HSD_Vector3() { X = r.ReadSingle() * Scale, Y = r.ReadSingle() * Scale + YTrans, Z = r.ReadSingle() * Scale });
                    float range = r.ReadSingle();
                    r.Skip(4); // settings
                }
            }

            HSD_Spline spline = new();
            spline.Points = points.ToArray();
            spline.PointCount = (short)points.Count;

            float totalLength = 0;
            foreach (HSD_Vector3 e in points)
                totalLength += new Vector3(e.X, e.Y, e.Z).Length;

            float[] lengths = new float[points.Count];
            //float length = 0;
            for (int i = 0; i < lengths.Length; i++)
            {
                lengths[i] = i / (float)(lengths.Length - 1);
                //length += new Vector3(points[i].X, points[i].Y, points[i].Z).Length;
            }

            spline.TotalLength = totalLength;
            spline.Lengths = new HSDFloatArray() { Array = lengths };
            return spline;
        }
    }
}
