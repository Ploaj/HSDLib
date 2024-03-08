using HSDRaw.AirRide.Gr.Data;
using HSDRaw.GX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HSDRaw.Tools.KAR
{
    public class Bucket
    {
        public static readonly int MAX_TRIANGLES = 500;

        private class BucketTriangle
        {
            public int Index;

            public GXVector3 v1;
            public GXVector3 v2;
            public GXVector3 v3;

            public GXVector3 this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: return v1;
                        case 1: return v2;
                        case 2: return v3;
                        default:
                            throw new Exception();
                    }
                }
            }

            public float MinX { get => Math.Min(v1.X, Math.Min(v2.X, v3.X)); }
            public float MinY { get => Math.Min(v1.Y, Math.Min(v2.Y, v3.Y)); }
            public float MinZ { get => Math.Min(v1.Z, Math.Min(v2.Z, v3.Z)); }
            public float MaxX { get => Math.Max(v1.X, Math.Max(v2.X, v3.X)); }
            public float MaxY { get => Math.Max(v1.Y, Math.Max(v2.Y, v3.Y)); }
            public float MaxZ { get => Math.Max(v1.Z, Math.Max(v2.Z, v3.Z)); }
        }

        private Bucket Left;

        private Bucket Right;

        private List<BucketTriangle> Triangles { get; set; } = new List<BucketTriangle>();

        public float MinX { get; internal set; } = float.MaxValue;
        public float MinY { get; internal set; } = float.MaxValue;
        public float MinZ { get; internal set; } = float.MaxValue;

        public float MaxX { get; internal set; } = float.MinValue;
        public float MaxY { get; internal set; } = float.MinValue;
        public float MaxZ { get; internal set; } = float.MinValue;

        private int depth = 0;

        /// <summary>
        /// Initialize Bucket Structure
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="minZ"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <param name="maxZ"></param>
        public Bucket(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
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
        public void AddTriangle(int index, GXVector3 v1, GXVector3 v2, GXVector3 v3)
        {
            Triangles.Add(new BucketTriangle() { Index = index, v1 = v1, v2 = v2, v3 = v3 });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        private void AddTriangle(BucketTriangle t)
        {
            // Check if the triangle intersects with the bounding box
            if (!IntersectsTriangle(t))
            {
                // Triangle does not intersect with this box, ignore it
                return;
            }

            // If this bucket has children, add the triangle to them instead
            if (Left != null && Right != null)
            {
                Left.AddTriangle(t);
                Right.AddTriangle(t);
            }
            else
            {
                // Add the triangle to this bucket
                Triangles.Add(t);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CheckSplit()
        {
            // Check if the number of triangles exceeds MAX_TRIANGLES
            if (Triangles.Count > MAX_TRIANGLES)
            {
                // Split the bucket if needed
                if (Left == null && Right == null)
                {
                    Split();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Split()
        {
            // Calculate the centroid of triangles in this bucket
            GXVector3 centroid = CalculateCentroid();

            // Determine the split dimension based on the maximum extent of the centroid
            float extentX = Math.Abs(centroid.X - (MinX + MaxX) / 2);
            float extentY = Math.Abs(centroid.Y - (MinY + MaxY) / 2);
            float extentZ = Math.Abs(centroid.Z - (MinZ + MaxZ) / 2);

            if (extentX >= extentY && extentX >= extentZ)
            {
                // Split along the x-axis
                float midX = (MinX + MaxX) / 2;
                Left = new Bucket(MinX, MinY, MinZ, midX, MaxY, MaxZ) { depth = depth + 1 };
                Right = new Bucket(midX, MinY, MinZ, MaxX, MaxY, MaxZ) { depth = depth + 1 };
            }
            else if (extentY >= extentZ)
            {
                // Split along the y-axis
                float midY = (MinY + MaxY) / 2;
                Left = new Bucket(MinX, MinY, MinZ, MaxX, midY, MaxZ) { depth = depth + 1 };
                Right = new Bucket(MinX, midY, MinZ, MaxX, MaxY, MaxZ) { depth = depth + 1 };
            }
            else
            {
                // Split along the z-axis
                float midZ = (MinZ + MaxZ) / 2;
                Left = new Bucket(MinX, MinY, MinZ, MaxX, MaxY, midZ) { depth = depth + 1 };
                Right = new Bucket(MinX, MinY, midZ, MaxX, MaxY, MaxZ) { depth = depth + 1 };
            }

            // Distribute triangles optimally between the two child buckets
            DistributeTriangles();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private GXVector3 CalculateCentroid()
        {
            // Calculate the centroid of triangles in this bucket
            GXVector3 sum = new GXVector3();

            foreach (var t in Triangles)
            {
                sum.X += (t.v1.X + t.v2.X + t.v3.X) / 3;
                sum.Y += (t.v1.Y + t.v2.Y + t.v3.Y) / 3;
                sum.Z += (t.v1.Z + t.v2.Z + t.v3.Z) / 3;
            }

            sum.X /= Triangles.Count;
            sum.Y /= Triangles.Count;
            sum.Z /= Triangles.Count;

            return sum;
        }

        private void DistributeTriangles()
        {
            // Distribute triangles optimally between the two child buckets
            foreach (var t in Triangles)
            {
                if (Left.DistanceToCentroid(t) < Right.DistanceToCentroid(t))
                {
                    Left.AddTriangle(t);
                }
                else
                {
                    Right.AddTriangle(t);
                }
            }

            // Clear the triangles from this bucket
            Triangles.Clear();

            // check split after all are added
            Left.CheckSplit();
            Right.CheckSplit();
        }

        private float DistanceToCentroid(BucketTriangle t)
        {
            // Calculate the distance from the centroid of this bucket to the centroid of the given triangle
            GXVector3 triangleCentroid = (t.v1 + t.v2 + t.v3) / 3;
            GXVector3 bucketCentroid = new GXVector3((MinX + MaxX) / 2, (MinY + MaxY) / 2, (MinZ + MaxZ) / 2);

            return GXVector3.Distance(bucketCentroid, triangleCentroid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool IntersectsTriangle(BucketTriangle t)
        {
            // Check if the triangle intersects with the bounding box

            float boxMinX = Math.Min(MinX, MaxX);
            float boxMaxX = Math.Max(MinX, MaxX);
            float boxMinY = Math.Min(MinY, MaxY);
            float boxMaxY = Math.Max(MinY, MaxY);
            float boxMinZ = Math.Min(MinZ, MaxZ);
            float boxMaxZ = Math.Max(MinZ, MaxZ);

            // Check if any of the triangle vertices is inside the bounding box
            if ((t.v1.X >= boxMinX && t.v1.X <= boxMaxX && t.v1.Y >= boxMinY && t.v1.Y <= boxMaxY && t.v1.Z >= boxMinZ && t.v1.Z <= boxMaxZ) ||
                (t.v2.X >= boxMinX && t.v2.X <= boxMaxX && t.v2.Y >= boxMinY && t.v2.Y <= boxMaxY && t.v2.Z >= boxMinZ && t.v2.Z <= boxMaxZ) ||
                (t.v3.X >= boxMinX && t.v3.X <= boxMaxX && t.v3.Y >= boxMinY && t.v3.Y <= boxMaxY && t.v3.Z >= boxMinZ && t.v3.Z <= boxMaxZ))
            {
                return true;
            }

            // Check if any of the triangle edges intersects with the bounding box
            if (EdgeIntersectsBox(t.v1, t.v2) || EdgeIntersectsBox(t.v2, t.v3) || EdgeIntersectsBox(t.v3, t.v1))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private bool EdgeIntersectsBox(GXVector3 v1, GXVector3 v2)
        {
            float boxMinX = Math.Min(MinX, MaxX);
            float boxMaxX = Math.Max(MinX, MaxX);
            float boxMinY = Math.Min(MinY, MaxY);
            float boxMaxY = Math.Max(MinY, MaxY);
            float boxMinZ = Math.Min(MinZ, MaxZ);
            float boxMaxZ = Math.Max(MinZ, MaxZ);

            // Check if the edge intersects with the bounding box

            float minX = Math.Min(v1.X, v2.X);
            float maxX = Math.Max(v1.X, v2.X);
            float minY = Math.Min(v1.Y, v2.Y);
            float maxY = Math.Max(v1.Y, v2.Y);
            float minZ = Math.Min(v1.Z, v2.Z);
            float maxZ = Math.Max(v1.Z, v2.Z);

            return !(maxX < boxMinX || minX > boxMaxX || maxY < boxMinY || minY > boxMaxY || maxZ < boxMinZ || minZ > boxMaxZ);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coll"></param>
        public static KAR_grCollisionTree GeneratePartitionNode(KAR_grCollisionNode coll)
        {
            var vertices = coll.Vertices;
            var triangles = coll.Triangles;

            Dictionary<int, ushort> triangleToRough = new Dictionary<int, ushort>();

            // create initial bucket
            var root = new Bucket(
                -5000, -5000, -5000,
                5000, 5000, 5000);

            // add all triangles
            // TODO: transform triangles by their joint
            for (int i = 0; i < triangles.Length; i++)
            {
                var t = triangles[i];

                if (t.Rough != 0)
                    triangleToRough.Add(i, (ushort)triangleToRough.Count);

                root.AddTriangle(i,
                    vertices[t.V1],
                    vertices[t.V2],
                    vertices[t.V3]);
            }
            root.CheckSplit();

            // TODO: add all zones

            // gather partition data
            List<KAR_grPartitionBucket> partBuckets = new List<KAR_grPartitionBucket>();
            List<ushort> collTris = new List<ushort>();
            List<ushort> roughTris = new List<ushort>();
            List<ushort> zones = new List<ushort>();

            // process queue

            void processBucket(Bucket b)
            {
                // create partition data
                var pt = new KAR_grPartitionBucket()
                {
                    Child1 = -1,
                    Child2 = -1,
                    CollTriangleStart = (ushort)collTris.Count,
                    RoughStart = (ushort)roughTris.Count,
                    ZoneIndexStart = (ushort)zones.Count,
                    MinX = b.MinX,
                    MinY = b.MinY,
                    MinZ = b.MinZ,
                    MaxX = b.MaxX,
                    MaxY = b.MaxY,
                    MaxZ = b.MaxZ,
                    Depth = (byte)b.depth,
                };
                partBuckets.Add(pt);

                // tris
                foreach (var tri in b.Triangles)
                {
                    var t = triangles[tri.Index];

                    // skip seg move
                    if (t.SegmentMove)
                        continue;

                    // add rough 
                    if (triangleToRough.ContainsKey(tri.Index))
                    {
                        roughTris.Add(triangleToRough[tri.Index]);
                    }

                    // TODO: add regardless of rough?
                    collTris.Add((ushort)tri.Index);
                }

                // TODO: zones

                // set counts
                pt.CollTriangleCount = (ushort)(collTris.Count - pt.CollTriangleStart);
                pt.RoughCount = (ushort)(roughTris.Count - pt.RoughStart);
                pt.ZoneIndexCount = (ushort)(zones.Count - pt.ZoneIndexStart);

                // process children
                if (b.Left != null && b.Right != null)
                {
                    pt.Child1 = (short)(partBuckets.Count);
                    processBucket(b.Left);

                    pt.Child2 = (short)(partBuckets.Count);
                    processBucket(b.Right);
                }
            };
            processBucket(root);

            // create partition node
            KAR_grCollisionTree partition = new KAR_grCollisionTree();

            // set buckets
            partition.Buckets = partBuckets.ToArray();

            // set collidable triangles
            partition.CollidableTriangleDataType = 5;
            partition.CollidableTriangles = collTris.ToArray();
            partition.CollidableTriangleCount = (ushort)partition.CollidableTriangles.Length;

            // set zones
            if (zones.Count > 0)
            {
                partition.ZoneIndexType = 5;
                partition.ZoneIndices = zones.ToArray();
                partition.ZoneIndexCount = (ushort)partition.ZoneIndices.Length;
            }

            // set rough triangles
            if (roughTris.Count > 0)
            {
                partition.RoughTriangleType = 5;
                partition.RoughIndices = roughTris.ToArray();
                partition.RoughIndexCount = (ushort)partition.RoughIndices.Length;
            }

            // process bit table
            partition.BitTableDataType = 3;
            partition._s.SetBuffer(0x54, new byte[(int)Math.Ceiling(collTris.Count / 8f)]);
            partition.BitTableCount = (ushort)collTris.Count;

            return partition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tris"></param>
        /// <param name="partBuckets"></param>
        /// <param name="partTris"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        //private KAR_grCollisionTree ProcessBuckets(
        //    List<KAR_CollisionTriangle> tris,
        //    List<GrZoneJoint> zonejoints,
        //    List<KAR_grPartitionBucket> partBuckets = null, 
        //    List<ushort> partTris = null,
        //    List<ushort> zones = null,
        //    List<ushort> roughTris = null,
        //    Dictionary<KAR_CollisionTriangle, int> triToRough = null,
        //    int depth = -1)
        //{
        //    if (partBuckets == null)
        //    {
        //        partBuckets = new List<KAR_grPartitionBucket>();
        //        partTris = new List<ushort>();
        //        zones = new List<ushort>();
        //        roughTris = new List<ushort>();
        //        triToRough = new Dictionary<KAR_CollisionTriangle, int>();
        //        for (int i = 0; i < tris.Count; i++)
        //            if (tris[i].Rough != 0)
        //                triToRough.Add(tris[i], triToRough.Count);
        //    }

        //    var bucketIndex = partBuckets.Count;
        //    KAR_grPartitionBucket partBucket = new KAR_grPartitionBucket()
        //    {
        //        MinX = MinX,
        //        MinY = MinY,
        //        MinZ = MinZ,
        //        MaxX = MaxX,
        //        MaxY = MaxY,
        //        MaxZ = MaxZ,
        //        Depth = (byte)depth
        //    };
        //    partBuckets.Add(partBucket);

        //    // if there are no children process the triangles in this bucket
        //    if (Children.Count == 0)
        //    {
        //        partBucket.BucketStart = -1;
        //        partBucket.BucketNext = -1;

        //        Debug.WriteLine(partBucket.BucketStart);
        //        Debug.WriteLine(partBucket.BucketNext);

        //        // process triangles
        //        partBucket.CollTriangleStart = (short)partTris.Count;
        //        partBucket.RoughStart = (short)roughTris.Count;
        //        partBucket.ZoneIndexStart = (short)zones.Count;
        //        foreach (var t in Triangles)
        //        {
        //            partTris.Add((ushort)tris.IndexOf(t.t));

        //            if (triToRough.ContainsKey(t.t))
        //                roughTris.Add((ushort)triToRough[t.t]);
        //        }
        //        // TODO: find zones in this area
        //        ushort zoneIndex = 0;
        //        foreach (var z in zonejoints)
        //        {
        //            foreach (var f in z.Faces)
        //            {
        //                if (IntersectsTriangle(f.p0, f.p1, f.p2))
        //                {
        //                    zones.Add(zoneIndex);
        //                    break;
        //                }
        //            }
        //            zoneIndex++;
        //        }
        //        partBucket.CollTriangleCount = (short)(partTris.Count - partBucket.CollTriangleStart);
        //        partBucket.RoughCount = (short)(roughTris.Count - partBucket.RoughStart);
        //        partBucket.ZoneIndexCount = (short)(zones.Count - partBucket.ZoneIndexStart);
        //    }
        //    else
        //    {
        //        // continue processing children
        //        foreach (var c in Children)
        //        {
        //            // set children bucket indices
        //            partBucket.BucketStart = (short)(bucketIndex + 1);
        //            partBucket.BucketNext = (short)(partBuckets.Count);

        //            c.ProcessBuckets(tris, zonejoints, partBuckets, partTris, zones, roughTris, triToRough, depth + 1);
        //        }

        //    }

        //    // only return the node if we are at the root (depth of 0)
        //    if (depth == 0)
        //    {
        //        KAR_grCollisionTree partition = new KAR_grCollisionTree();

        //        // set buckets
        //        partition.Buckets = partBuckets.ToArray();

        //        // set collidable triangles
        //        partition.CollidableTriangleDataType = 5;
        //        partition.CollidableTriangles = partTris.ToArray();
        //        partition.CollidableTriangleCount = (ushort)partition.CollidableTriangles.Length;

        //        // set zones
        //        if (zones.Count > 0)
        //        {
        //            partition.ZoneIndexType = 5;
        //            partition.ZoneIndices = zones.ToArray();
        //            partition.ZoneIndexCount = (ushort)partition.ZoneIndices.Length;
        //        }

        //        // set rough triangles
        //        if (roughTris.Count > 0)
        //        {
        //            partition.RoughTriangleType = 5;
        //            partition.RoughIndices = roughTris.ToArray();
        //            partition.RoughIndexCount = (ushort)partition.RoughIndices.Length;
        //        }

        //        // process bit table
        //        partition.BitTableDataType = 3;
        //        partition._s.SetBuffer(0x54, new byte[(int)Math.Ceiling(partTris.Count / 8f)]);
        //        partition.BitTableCount = (ushort)partTris.Count;

        //        return partition;
        //    }

        //    return null;
        //}
    }
}
