using HSDRaw.AirRide.Gr.Data;
using HSDRaw.GX;
using HSDRawViewer.Rendering;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.Converters.AirRide
{
    public class BucketGen
    {
        public static readonly int MAX_TRIANGLES = 500;

        public static KAR_grCollisionTree GenerateBucketPartition(KAR_grCollisionNode coll)
        {
            var v = coll.Vertices;
            var bucketGen = new BucketGen(
                v.Min(e => e.X) - 10,
                Math.Min(-1000, v.Min(e => e.Y) - 10),
                v.Min(e => e.Z) - 10,
                v.Max(e => e.X) + 10,
                Math.Max(1000, v.Min(e => e.Y) + 10),
                v.Max(e => e.Z) + 10);

            var tris = coll.Triangles.ToList();

            foreach (var t in tris)
                bucketGen.AddTriangle(t, v[t.V1], v[t.V2], v[t.V3]);

            return bucketGen.ProcessBuckets(tris);
        }


        private class BucketTriangle
        {
            public KAR_CollisionTriangle t;

            public GXVector3 v1;
            public GXVector3 v2;
            public GXVector3 v3;

            public GXVector3[] Vertices { get => new GXVector3[] { v1, v2, v3 }; }

            public float MinX { get => Math.Min(v1.X, Math.Min(v2.X, v3.X)); }
            public float MinY { get => Math.Min(v1.Y, Math.Min(v2.Y, v3.Y)); }
            public float MinZ { get => Math.Min(v1.Z, Math.Min(v2.Z, v3.Z)); }
            public float MaxX { get => Math.Max(v1.X, Math.Max(v2.X, v3.X)); }
            public float MaxY { get => Math.Max(v1.Y, Math.Max(v2.Y, v3.Y)); }
            public float MaxZ { get => Math.Max(v1.Z, Math.Max(v2.Z, v3.Z)); }
        }

        public List<BucketGen> Children { get; internal set; } = new List<BucketGen>();

        private List<BucketTriangle> Triangles { get; set; } = new List<BucketTriangle>();


        public float MinX { get; internal set; } = float.MaxValue;
        public float MinY { get; internal set; } = float.MaxValue;
        public float MinZ { get; internal set; } = float.MaxValue;

        public float MaxX { get; internal set; } = float.MinValue;
        public float MaxY { get; internal set; } = float.MinValue;
        public float MaxZ { get; internal set; } = float.MinValue;

        private int depth = 0;

        public BucketGen(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            MinX = minX;
            MinY = minY;
            MinZ = minZ;
            MaxX = maxX;
            MaxY = maxY;
            MaxZ = maxZ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tri"></param>
        public void AddTriangle(KAR_CollisionTriangle t, GXVector3 v1, GXVector3 v2, GXVector3 v3)
        {
            AddTriangle(new BucketTriangle() { t = t, v1 = v1, v2 = v2, v3 = v3 });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tri"></param>
        private void AddTriangle(BucketTriangle t)
        {
            // make sure at least one point is inside bucket
            if (!ContainsTriangle(t))
                return;

            // add triangle to bucket 
            if(Children.Count == 0)
            {
                Triangles.Add(t);

                // spill bucket when it overflows
                if (Triangles.Count > MAX_TRIANGLES && depth < 20)
                    Spill();
            }
            else
            {
                foreach (var v in Children)
                    v.AddTriangle(t);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ContainsTriangle(BucketTriangle tri)
        {
            return 
                ContainsPoint(tri.v1) || 
                ContainsPoint(tri.v2) || 
                ContainsPoint(tri.v3) ||
                ContainsTriangle(
                new Vector3(tri.v1.X, tri.v1.Y, tri.v1.Z),
                new Vector3(tri.v2.X, tri.v2.Y, tri.v2.Z),
                new Vector3(tri.v3.X, tri.v3.Y, tri.v3.Z));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        private bool ContainsTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            var B1 = new Vector3(MinX, MinY, MinZ);
            var B2 = new Vector3(MaxX, MaxY, MaxZ);

            Vector3 hit = Vector3.Zero;

            return PickInformation.CheckLineBox(B1, B2, v1, v2, ref hit) ||
                PickInformation.CheckLineBox(B1, B2, v2, v3, ref hit) ||
                PickInformation.CheckLineBox(B1, B2, v1, v3, ref hit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool ContainsPoint(GXVector3 p)
        {
            return 
                p.X >= MinX && p.X <= MaxX &&
                p.Y >= MinY && p.Y <= MaxY &&
                p.Z >= MinZ && p.Z <= MaxZ;
        }


        /// <summary>
        /// 
        /// </summary>
        private void Spill()
        {
            var mid_x = (MaxX + MinX) / 2;
            BucketGen bx1 = new BucketGen(MinX, MinY, MinZ, mid_x, MaxY, MaxZ) { depth = depth + 1 };
            BucketGen bx2 = new BucketGen(mid_x, MinY, MinZ, MaxX, MaxY, MaxZ) { depth = depth + 1 };

            var mid_y = (MaxY + MinY) / 2;
            BucketGen by1 = new BucketGen(MinX, MinY, MinZ, MaxX, mid_y, MaxZ) { depth = depth + 1 };
            BucketGen by2 = new BucketGen(MinX, mid_y, MinZ, MaxX, MaxY, MaxZ) { depth = depth + 1 };

            var mid_z = (MaxZ + MinZ) / 2;
            BucketGen bz1 = new BucketGen(MinX, MinY, MinZ, MaxX, MaxY, mid_z) { depth = depth + 1 };
            BucketGen bz2 = new BucketGen(MinX, MinY, mid_z, MaxX, MaxY, MaxZ) { depth = depth + 1 };

            Console.WriteLine("Start Spilling");
            foreach (var t in Triangles)
            {
                bx1.AddTriangle(t);
                bx2.AddTriangle(t);
                by1.AddTriangle(t);
                by2.AddTriangle(t);
                bz1.AddTriangle(t);
                bz2.AddTriangle(t);
            }
            Console.WriteLine("End Spilling");

            var split_x = Math.Abs(bx2.Triangles.Count - bx1.Triangles.Count);
            var split_y = Math.Abs(by2.Triangles.Count - by1.Triangles.Count);
            var split_z = Math.Abs(bz2.Triangles.Count - bz1.Triangles.Count);

            Triangles.Clear();

            // find smallest bucket split and use that
            if (split_x <= split_y && split_x <= split_z)
            {
                // x split
                Children.Add(bx1);
                Children.Add(bx2);
            }
            else if (split_y < split_z)
            {
                // y split
                Children.Add(by1);
                Children.Add(by2);
            }
            else
            {
                // z split
                Children.Add(bz1);
                Children.Add(bz2);
            }
        }

        public KAR_grCollisionTree ProcessBuckets(List<KAR_CollisionTriangle> tris, List<KAR_grPartitionBucket> partBuckets = null, List<ushort> partTris = null, int depth = 0)
        {
            if (partBuckets == null)
            {
                partBuckets = new List<KAR_grPartitionBucket>();
                partTris = new List<ushort>();
            }

            var bucketIndex = partBuckets.Count;
            KAR_grPartitionBucket partBucket = new KAR_grPartitionBucket()
            {
                MinX = MinX,
                MinY = MinY,
                MinZ = MinZ,
                MaxX = MaxX,
                MaxY = MaxY,
                MaxZ = MaxZ,
                Depth = (byte)depth
            };
            partBuckets.Add(partBucket);

            if (Children.Count == 0)
            {
                partBucket.BucketStart = -1;
                partBucket.BucketCount = -1;

                Console.WriteLine(partBucket.BucketStart);
                Console.WriteLine(partBucket.BucketCount);

                // process triangles
                partBucket.CollTriangleStart = (short)partTris.Count;
                foreach (var t in Triangles)
                    partTris.Add((ushort)tris.IndexOf(t.t));
                partBucket.CollTriangleCount = (short)(partTris.Count - partBucket.CollTriangleStart);

            }
            else
            {
                foreach (var c in Children)
                {
                    // set children bucket indices
                    partBucket.BucketStart = (short)(bucketIndex + 1);
                    partBucket.BucketCount = (short)(partBuckets.Count);

                    c.ProcessBuckets(tris, partBuckets, partTris, depth + 1);
                }

            }

            
            if(depth == 0)
            {
                KAR_grCollisionTree partition = new KAR_grCollisionTree();

                partition.Buckets = partBuckets.ToArray();

                partition.CollidableTriangleDataType = 5;
                partition.CollidableTriangles = partTris.ToArray();

                partition.BitTableDataType = 3;
                partition._s.SetBuffer(0x54, new byte[(int)Math.Ceiling(partTris.Count / 8f)]);
                partition.BitTableCount = (ushort)partTris.Count;

                return partition;
            }

            return null;
        }
    }
}
