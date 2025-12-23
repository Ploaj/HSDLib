using OpenTK.Mathematics;
using System;

namespace HSDRawViewer.Rendering
{
    public class BoundingBox
    {
        public Vector3 Min { get; internal set; }

        public Vector3 Max { get; internal set; }

        public Vector3 Center => (Max + Min) / 2;

        public Vector3 Extents
        {
            get { return (Max - Min) * 0.5f; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public BoundingBox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        private class Triangle
        {
            public Vector3 p0;
            public Vector3 p1;
            public Vector3 p2;
        }

        public bool Intersects(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            const float EPSILON = 1e-6f;

            // Move triangle into AABB local space (AABB centered at origin)
            Vector3 c = Center;
            Vector3 v0 = p0 - c;
            Vector3 v1 = p1 - c;
            Vector3 v2 = p2 - c;

            Vector3 e0 = v1 - v0;
            Vector3 e1 = v2 - v1;
            Vector3 e2 = v0 - v2;

            Vector3 ext = Extents;

            float min, max, rad, p0p, p1p, p2p;

            // ============================================================
            // 1) 9 edge × axis tests
            // ============================================================

            // --- Edge 0 ---
            // AXIS = (0, -e0.Z, e0.Y)
            p0p = e0.Z * v0.Y - e0.Y * v0.Z;
            p2p = e0.Z * v2.Y - e0.Y * v2.Z;
            min = MathF.Min(p0p, p2p);
            max = MathF.Max(p0p, p2p);
            rad = MathF.Abs(e0.Z) * ext.Y + MathF.Abs(e0.Y) * ext.Z;
            if (min > rad + EPSILON || max < -rad - EPSILON) return false;

            // AXIS = (e0.Z, 0, -e0.X)
            p0p = e0.Z * v0.X - e0.X * v0.Z;
            p2p = e0.Z * v2.X - e0.X * v2.Z;
            min = MathF.Min(p0p, p2p);
            max = MathF.Max(p0p, p2p);
            rad = MathF.Abs(e0.Z) * ext.X + MathF.Abs(e0.X) * ext.Z;
            if (min > rad + EPSILON || max < -rad - EPSILON) return false;

            // AXIS = (-e0.Y, e0.X, 0)
            p1p = e0.Y * v1.X - e0.X * v1.Y;
            p2p = e0.Y * v2.X - e0.X * v2.Y;
            min = MathF.Min(p1p, p2p);
            max = MathF.Max(p1p, p2p);
            rad = MathF.Abs(e0.Y) * ext.X + MathF.Abs(e0.X) * ext.Y;
            if (min > rad + EPSILON || max < -rad - EPSILON) return false;

            // --- Edge 1 ---
            p0p = e1.Z * v0.Y - e1.Y * v0.Z;
            p2p = e1.Z * v2.Y - e1.Y * v2.Z;
            min = MathF.Min(p0p, p2p);
            max = MathF.Max(p0p, p2p);
            rad = MathF.Abs(e1.Z) * ext.Y + MathF.Abs(e1.Y) * ext.Z;
            if (min > rad + EPSILON || max < -rad - EPSILON) return false;

            p0p = e1.Z * v0.X - e1.X * v0.Z;
            p2p = e1.Z * v2.X - e1.X * v2.Z;
            min = MathF.Min(p0p, p2p);
            max = MathF.Max(p0p, p2p);
            rad = MathF.Abs(e1.Z) * ext.X + MathF.Abs(e1.X) * ext.Z;
            if (min > rad + EPSILON || max < -rad - EPSILON) return false;

            p0p = e1.Y * v0.X - e1.X * v0.Y;
            p1p = e1.Y * v1.X - e1.X * v1.Y;
            min = MathF.Min(p0p, p1p);
            max = MathF.Max(p0p, p1p);
            rad = MathF.Abs(e1.Y) * ext.X + MathF.Abs(e1.X) * ext.Y;
            if (min > rad + EPSILON || max < -rad - EPSILON) return false;

            // --- Edge 2 ---
            p0p = e2.Z * v0.Y - e2.Y * v0.Z;
            p1p = e2.Z * v1.Y - e2.Y * v1.Z;
            min = MathF.Min(p0p, p1p);
            max = MathF.Max(p0p, p1p);
            rad = MathF.Abs(e2.Z) * ext.Y + MathF.Abs(e2.Y) * ext.Z;
            if (min > rad + EPSILON || max < -rad - EPSILON) return false;

            p0p = e2.Z * v0.X - e2.X * v0.Z;
            p1p = e2.Z * v1.X - e2.X * v1.Z;
            min = MathF.Min(p0p, p1p);
            max = MathF.Max(p0p, p1p);
            rad = MathF.Abs(e2.Z) * ext.X + MathF.Abs(e2.X) * ext.Z;
            if (min > rad + EPSILON || max < -rad - EPSILON) return false;

            p1p = e2.Y * v1.X - e2.X * v1.Y;
            p2p = e2.Y * v2.X - e2.X * v2.Y;
            min = MathF.Min(p1p, p2p);
            max = MathF.Max(p1p, p2p);
            rad = MathF.Abs(e2.Y) * ext.X + MathF.Abs(e2.X) * ext.Y;
            if (min > rad + EPSILON || max < -rad - EPSILON) return false;

            // ============================================================
            // 2) Test overlap in X, Y, Z (AABB axes)
            // ============================================================

            min = MathF.Min(v0.X, MathF.Min(v1.X, v2.X));
            max = MathF.Max(v0.X, MathF.Max(v1.X, v2.X));
            if (min > ext.X + EPSILON || max < -ext.X - EPSILON) return false;

            min = MathF.Min(v0.Y, MathF.Min(v1.Y, v2.Y));
            max = MathF.Max(v0.Y, MathF.Max(v1.Y, v2.Y));
            if (min > ext.Y + EPSILON || max < -ext.Y - EPSILON) return false;

            min = MathF.Min(v0.Z, MathF.Min(v1.Z, v2.Z));
            max = MathF.Max(v0.Z, MathF.Max(v1.Z, v2.Z));
            if (min > ext.Z + EPSILON || max < -ext.Z - EPSILON) return false;

            // ============================================================
            // 3) Triangle normal test
            // ============================================================

            Vector3 normal = Vector3.Cross(e0, e1);

            Vector3 vmin, vmax;

            vmin.X = normal.X > 0 ? -ext.X - v0.X : ext.X - v0.X;
            vmax.X = normal.X > 0 ? ext.X - v0.X : -ext.X - v0.X;

            vmin.Y = normal.Y > 0 ? -ext.Y - v0.Y : ext.Y - v0.Y;
            vmax.Y = normal.Y > 0 ? ext.Y - v0.Y : -ext.Y - v0.Y;

            vmin.Z = normal.Z > 0 ? -ext.Z - v0.Z : ext.Z - v0.Z;
            vmax.Z = normal.Z > 0 ? ext.Z - v0.Z : -ext.Z - v0.Z;

            if (Vector3.Dot(normal, vmin) > EPSILON) return false;
            if (Vector3.Dot(normal, vmax) < -EPSILON) return false;

            return true;
        }

        //private bool TestSeparatingAxis(Triangle triangle, Vector3 axis)
        //{
        //    // Project triangle onto the axis
        //    float[] triangleProjections = ProjectOntoAxis(triangle, axis);

        //    // Project bounding box onto the axis (considering orientation)
        //    float[] aabbProjections = ProjectOntoAxis(this, axis);

        //    // Check for overlap
        //    if (triangleProjections[1] < aabbProjections[0] || triangleProjections[0] > aabbProjections[1])
        //        return true;

        //    return false;
        //}

        //private float[] ProjectOntoAxis(Triangle triangle, Vector3 axis)
        //{
        //    float dot0 = Vector3.Dot(triangle.p0, axis);
        //    float dot1 = Vector3.Dot(triangle.p1, axis);
        //    float dot2 = Vector3.Dot(triangle.p2, axis);

        //    return new float[] { Math.Min(dot0, Math.Min(dot1, dot2)), Math.Max(dot0, Math.Max(dot1, dot2)) };
        //}

        //private float[] ProjectOntoAxis(BoundingBox aabb, Vector3 axis)
        //{
        //    Vector3[] boxVertices = GetBoxVertices(aabb);

        //    float dotMin = Vector3.Dot(boxVertices[0], axis);
        //    float dotMax = dotMin;

        //    for (int i = 1; i < boxVertices.Length; i++)
        //    {
        //        float dot = Vector3.Dot(boxVertices[i], axis);
        //        dotMin = Math.Min(dotMin, dot);
        //        dotMax = Math.Max(dotMax, dot);
        //    }

        //    return new float[] { dotMin, dotMax };
        //}

        //private Vector3[] GetBoxVertices(BoundingBox aabb)
        //{
        //    Vector3[] vertices = new Vector3[8];

        //    Vector3 extents = aabb.Extents;
        //    Vector3 center = aabb.Center;

        //    // Calculate box vertices
        //    vertices[0] = center + new Vector3(-extents.X, -extents.Y, -extents.Z);
        //    vertices[1] = center + new Vector3(extents.X, -extents.Y, -extents.Z);
        //    vertices[2] = center + new Vector3(-extents.X, extents.Y, -extents.Z);
        //    vertices[3] = center + new Vector3(extents.X, extents.Y, -extents.Z);
        //    vertices[4] = center + new Vector3(-extents.X, -extents.Y, extents.Z);
        //    vertices[5] = center + new Vector3(extents.X, -extents.Y, extents.Z);
        //    vertices[6] = center + new Vector3(-extents.X, extents.Y, extents.Z);
        //    vertices[7] = center + new Vector3(extents.X, extents.Y, extents.Z);

        //    return vertices;
        //}
    }
}
