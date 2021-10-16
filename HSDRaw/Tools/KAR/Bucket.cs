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

        public List<Bucket> Children { get; internal set; } = new List<Bucket>();

        private List<BucketTriangle> Triangles { get; set; } = new List<BucketTriangle>();


        public float MinX { get; internal set; } = float.MaxValue;
        public float MinY { get; internal set; } = float.MaxValue;
        public float MinZ { get; internal set; } = float.MaxValue;

        public float MaxX { get; internal set; } = float.MinValue;
        public float MaxY { get; internal set; } = float.MinValue;
        public float MaxZ { get; internal set; } = float.MinValue;

        private int depth = 0;

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
            if (Children.Count == 0)
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
                ContainsTriangle(tri.v1, tri.v2, tri.v3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        private bool ContainsTriangle(GXVector3 v1, GXVector3 v2, GXVector3 v3)
        {
            var B1 = new GXVector3(MinX, MinY, MinZ);
            var B2 = new GXVector3(MaxX, MaxY, MaxZ);

            GXVector3 hit = new GXVector3(0, 0, 0);

            return CheckLineBox(B1, B2, v1, v2, ref hit) ||
                CheckLineBox(B1, B2, v2, v3, ref hit) ||
                CheckLineBox(B1, B2, v1, v3, ref hit);
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
        /// <param name="B1"></param>
        /// <param name="B2"></param>
        /// <param name="L1"></param>
        /// <param name="L2"></param>
        /// <param name="Hit"></param>
        /// <returns></returns>
        public static bool CheckLineBox(GXVector3 B1, GXVector3 B2, GXVector3 L1, GXVector3 L2, ref GXVector3 Hit)
        {
            if (L2.X < B1.X && L1.X < B1.X) return false;
            if (L2.X > B2.X && L1.X > B2.X) return false;
            if (L2.Y < B1.Y && L1.Y < B1.Y) return false;
            if (L2.Y > B2.Y && L1.Y > B2.Y) return false;
            if (L2.Z < B1.Z && L1.Z < B1.Z) return false;
            if (L2.Z > B2.Z && L1.Z > B2.Z) return false;
            if (L1.X > B1.X && L1.X < B2.X &&
                L1.Y > B1.Y && L1.Y < B2.Y &&
                L1.Z > B1.Z && L1.Z < B2.Z)
            {
                Hit = L1;
                return true;
            }
            if ((GetIntersection(L1.X - B1.X, L2.X - B1.X, L1, L2, ref Hit) && InBox(Hit, B1, B2, 1))
              || (GetIntersection(L1.Y - B1.Y, L2.Y - B1.Y, L1, L2, ref Hit) && InBox(Hit, B1, B2, 2))
              || (GetIntersection(L1.Z - B1.Z, L2.Z - B1.Z, L1, L2, ref Hit) && InBox(Hit, B1, B2, 3))
              || (GetIntersection(L1.X - B2.X, L2.X - B2.X, L1, L2, ref Hit) && InBox(Hit, B1, B2, 1))
              || (GetIntersection(L1.Y - B2.Y, L2.Y - B2.Y, L1, L2, ref Hit) && InBox(Hit, B1, B2, 2))
              || (GetIntersection(L1.Z - B2.Z, L2.Z - B2.Z, L1, L2, ref Hit) && InBox(Hit, B1, B2, 3)))
                return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fDst1"></param>
        /// <param name="fDst2"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="Hit"></param>
        /// <returns></returns>
        public static bool GetIntersection(float fDst1, float fDst2, GXVector3 P1, GXVector3 P2, ref GXVector3 Hit)
        {
            if ((fDst1 * fDst2) >= 0.0f) return false;
            if (fDst1 == fDst2) return false;
            //Hit = P1 + (P2 - P1) * (-fDst1 / (fDst2 - fDst1));

            Hit.X = P1.X + (P2.X - P1.X) * (-fDst1 / (fDst2 - fDst1));
            Hit.Y = P1.Y + (P2.Y - P1.Y) * (-fDst1 / (fDst2 - fDst1));
            Hit.Z = P1.Z + (P2.Z - P1.Z) * (-fDst1 / (fDst2 - fDst1));

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Hit"></param>
        /// <param name="B1"></param>
        /// <param name="B2"></param>
        /// <param name="Axis"></param>
        /// <returns></returns>
        public static bool InBox(GXVector3 Hit, GXVector3 B1, GXVector3 B2, int Axis)
        {
            if (Axis == 1 && Hit.Z > B1.Z && Hit.Z < B2.Z && Hit.Y > B1.Y && Hit.Y < B2.Y) return true;
            if (Axis == 2 && Hit.Z > B1.Z && Hit.Z < B2.Z && Hit.X > B1.X && Hit.X < B2.X) return true;
            if (Axis == 3 && Hit.X > B1.X && Hit.X < B2.X && Hit.Y > B1.Y && Hit.Y < B2.Y) return true;
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        private void Spill()
        {
            var mid_x = (MaxX + MinX) / 2;
            Bucket bx1 = new Bucket(MinX, MinY, MinZ, mid_x, MaxY, MaxZ) { depth = depth + 1 };
            Bucket bx2 = new Bucket(mid_x, MinY, MinZ, MaxX, MaxY, MaxZ) { depth = depth + 1 };

            var mid_y = (MaxY + MinY) / 2;
            Bucket by1 = new Bucket(MinX, MinY, MinZ, MaxX, mid_y, MaxZ) { depth = depth + 1 };
            Bucket by2 = new Bucket(MinX, mid_y, MinZ, MaxX, MaxY, MaxZ) { depth = depth + 1 };

            var mid_z = (MaxZ + MinZ) / 2;
            Bucket bz1 = new Bucket(MinX, MinY, MinZ, MaxX, MaxY, mid_z) { depth = depth + 1 };
            Bucket bz2 = new Bucket(MinX, MinY, mid_z, MaxX, MaxY, MaxZ) { depth = depth + 1 };

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tris"></param>
        /// <param name="partBuckets"></param>
        /// <param name="partTris"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public KAR_grCollisionTree ProcessBuckets(
            List<KAR_CollisionTriangle> tris,
            List<GrZoneJoint> zonejoints,
            List<KAR_grPartitionBucket> partBuckets = null, 
            List<ushort> partTris = null,
            List<ushort> zones = null,
            List<ushort> roughTris = null,
            Dictionary<KAR_CollisionTriangle, int> triToRough = null,
            int depth = 0)
        {
            if (partBuckets == null)
            {
                partBuckets = new List<KAR_grPartitionBucket>();
                partTris = new List<ushort>();
                zones = new List<ushort>();
                roughTris = new List<ushort>();
                triToRough = new Dictionary<KAR_CollisionTriangle, int>();
                for (int i = 0; i < tris.Count; i++)
                    if ((tris[i].Material & 0x3) != 0)
                        triToRough.Add(tris[i], triToRough.Count);
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

            // if there are no children process the triangles in this bucket
            if (Children.Count == 0)
            {
                partBucket.BucketStart = -1;
                partBucket.BucketCount = -1;

                Debug.WriteLine(partBucket.BucketStart);
                Debug.WriteLine(partBucket.BucketCount);

                // process triangles
                partBucket.CollTriangleStart = (short)partTris.Count;
                partBucket.RoughStart = (short)roughTris.Count;
                partBucket.ZoneIndexStart = (short)zones.Count;
                foreach (var t in Triangles)
                {
                    partTris.Add((ushort)tris.IndexOf(t.t));

                    if (triToRough.ContainsKey(t.t))
                        roughTris.Add((ushort)triToRough[t.t]);
                }
                // TODO: find zones in this area
                ushort zoneIndex = 0;
                foreach (var z in zonejoints)
                {
                    foreach (var f in z.Faces)
                    {
                        if (ContainsTriangle(f.p0, f.p1, f.p2))
                        {
                            zones.Add(zoneIndex);
                            break;
                        }
                    }
                    zoneIndex++;
                }
                partBucket.CollTriangleCount = (short)(partTris.Count - partBucket.CollTriangleStart);
                partBucket.RoughCount = (short)(roughTris.Count - partBucket.RoughStart);
                partBucket.ZoneIndexCount = (short)(zones.Count - partBucket.ZoneIndexStart);
            }
            else
            {
                // continue processing children
                foreach (var c in Children)
                {
                    // set children bucket indices
                    partBucket.BucketStart = (short)(bucketIndex + 1);
                    partBucket.BucketCount = (short)(partBuckets.Count);

                    c.ProcessBuckets(tris, zonejoints, partBuckets, partTris, zones, roughTris, triToRough, depth + 1);
                }

            }

            // only return the node if we are at the root (depth of 0)
            if (depth == 0)
            {
                KAR_grCollisionTree partition = new KAR_grCollisionTree();

                // set buckets
                partition.Buckets = partBuckets.ToArray();

                // set collidable triangles
                partition.CollidableTriangleDataType = 5;
                partition.CollidableTriangles = partTris.ToArray();
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
                partition._s.SetBuffer(0x54, new byte[(int)Math.Ceiling(partTris.Count / 8f)]);
                partition.BitTableCount = (ushort)partTris.Count;

                return partition;
            }

            return null;
        }
    }
}
