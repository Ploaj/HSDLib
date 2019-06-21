using HSDLib.KAR;
using System;
using System.Collections.Generic;
using OpenTK;
using System.IO;

namespace HALSysDATViewer.Tools
{
    public class CollisionTriangles
    {
        public Vector3 p1, p2, p3;
        // flag to be determined
    }

    public class CollisionBucket
    {
        // maximum number of triangles a bucket can hold
        private static int MAX_SIZE = 30;

        public Vector3 Min = Vector3.Zero, Max = Vector3.Zero;

        private List<CollisionBucket> SubBuckets = new List<CollisionBucket>();

        private List<CollisionTriangles> Triangles = new List<CollisionTriangles>();

        public CollisionBucket()
        {

        }

        public void AddTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var tri = new CollisionTriangles();
            tri.p1 = p1;
            tri.p2 = p2;
            tri.p3 = p3;
            Triangles.Add(tri);
            /*if (Triangles.Count >= MAX_SIZE)
                Spill();
            else*/
            {
                CalculateSize();
            }
        }

        private void Spill()
        {

        }

        private void CalculateSize()
        {
            Min = new Vector3(float.MaxValue);
            Max = new Vector3(float.MinValue);

            foreach(var v in Triangles)
            {
                var vert = v.p1;
                {
                    Max.X = Math.Max(Max.X, vert.X);
                    Max.Y = Math.Max(Max.Y, vert.Y);
                    Max.Z = Math.Max(Max.Z, vert.Z);
                    Min.X = Math.Min(Min.X, vert.X);
                    Min.Y = Math.Min(Min.Y, vert.Y);
                    Min.Z = Math.Min(Min.Z, vert.Z);
                }
                vert = v.p2;
                {
                    Max.X = Math.Max(Max.X, vert.X);
                    Max.Y = Math.Max(Max.Y, vert.Y);
                    Max.Z = Math.Max(Max.Z, vert.Z);
                    Min.X = Math.Min(Min.X, vert.X);
                    Min.Y = Math.Min(Min.Y, vert.Y);
                    Min.Z = Math.Min(Min.Z, vert.Z);
                }
                vert = v.p3;
                {
                    Max.X = Math.Max(Max.X, vert.X);
                    Max.Y = Math.Max(Max.Y, vert.Y);
                    Max.Z = Math.Max(Max.Z, vert.Z);
                    Min.X = Math.Min(Min.X, vert.X);
                    Min.Y = Math.Min(Min.Y, vert.Y);
                    Min.Z = Math.Min(Min.Z, vert.Z);
                }
            }
        }

        public static void Export(string FilePath, KAR_GrData data)
        {
            using (StreamWriter w = new StreamWriter(new FileStream(FilePath, FileMode.Create)))
            {
                foreach(var v in data.CollisionNode.Vertices)
                {
                    w.WriteLine($"v {v.X} {v.Y} {v.Z}");
                }
                foreach (var v in data.CollisionNode.Faces)
                {
                    w.WriteLine($"f {v.V1 + 1} {v.V2 + 1} {v.V3 + 1}");
                }
            }
        }

        public static void Import(string OBJPath, KAR_GrData data)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> faces = new List<Vector3>();

            using (StreamReader r = new StreamReader(new FileStream(OBJPath, FileMode.Open)))
            {
                while (!r.EndOfStream)
                {
                    var args = r.ReadLine().Split(' ');

                    if(args[0] == "v")
                    {
                        vertices.Add(new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3])));
                    }
                    if (args[0] == "f")
                    {
                        faces.Add(new Vector3(int.Parse(args[3].Split('/')[0])-1, int.Parse(args[2].Split('/')[0]) - 1, int.Parse(args[1].Split('/')[0]) - 1));
                    }
                }
            }
            
            /*data.PositionNode.StartPositions.Matrices[0] = new HSDLib.Common.HSD_Matrix3x3()
            {
                M00 = 1, M01 = 0, M02 = 0,
                M10 = 0, M11 = 1, M12 = 0,
                M20 = 0, M21 = 0, M22 = 1
            };*/

            CollisionBucket bucket = new CollisionBucket();

            var collision = new KAR_GrCollisionNode();

            foreach(var v in vertices)
            {
                Console.WriteLine($"v {v.X} {v.Y} {v.Z}");
                collision.Vertices.Add(new HSDLib.Helpers.GXVector3(v.X, v.Y, v.Z));
            }

            foreach (var f in faces)
            {
                bucket.AddTriangle(vertices[(int)f.X], vertices[(int)f.Y], vertices[(int)f.Z]);
                collision.Faces.Add(new KAR_CollisionTriangle()
                {
                    V1 = (int)f.X,
                    V2 = (int)f.Y,
                    V3 = (int)f.Z,
                    Color = 81
                });
            }
            
            var joint = data.CollisionNode.Joints[0];
            joint.FaceSize = faces.Count;
            joint.VertexSize = vertices.Count;
            collision.Joints.Add(joint);
            data.CollisionNode = collision;


            var part = new KAR_GrPartitionNode();

            part.Setup = new KAR_GrPartitionSetup();
            part.Setup.CollidableTriangles = new List<short>();
            part.Setup.CollidableTrianglesBits = new List<bool>();
            part.Setup.Partitions = new List<KAR_GrPartition>();
            for (int i = 0; i < faces.Count; i++)
            {
                part.Setup.CollidableTriangles.Add((short)i);
                part.Setup.CollidableTrianglesBits.Add(false);
            }
            {
                KAR_GrPartition partition = new KAR_GrPartition();
                /*partition.ChildStartIndex = -1;
                partition.ChildEndIndex = -1;
                partition.CollisionFaceStart = 0;
                partition.CollisionFaceCount = (short)faces.Count;
                partition.Depth = 0;
                partition.MaxX = bucket.Max.X;
                partition.MaxY = bucket.Max.Y;
                partition.MaxZ = bucket.Max.Z;
                partition.MinX = bucket.Min.X;
                partition.MinY = bucket.Min.Y;
                partition.MinZ = bucket.Min.Z;
                part.Setup.Partitions.Add(partition);*/

                Console.WriteLine(bucket.Max.ToString() + " " + bucket.Min.ToString());
                partition = data.PartitionNode.Setup.Partitions[0];
                partition.ChildStartIndex = -1;
                partition.ChildEndIndex = -1;
                partition.CollisionFaceCount = (short)faces.Count;
                //Console.WriteLine(partition.MinX + " " + partition.MinY + " " + partition.MinZ);
                //Console.WriteLine(partition.MaxX + " " + partition.MaxY + " " + partition.MaxZ);
                partition.MaxX = bucket.Max.X;
                partition.MaxY = bucket.Max.Y;
                partition.MaxZ = bucket.Max.Z;
                partition.MinX = bucket.Min.X;
                partition.MinY = bucket.Min.Y;
                partition.MinZ = bucket.Min.Z;
                data.PartitionNode.Setup.Partitions.Clear();
                data.PartitionNode.Setup.Partitions.Add(partition);
            }

        }
    }
}
