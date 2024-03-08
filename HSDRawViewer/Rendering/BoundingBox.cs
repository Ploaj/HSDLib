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

        public bool Intersects(Vector3 triangleP0, Vector3 triangleP1, Vector3 triangleP2)
        {
            // Create a Triangle object from the input points
            Triangle triangle = new Triangle { p0 = triangleP0, p1 = triangleP1, p2 = triangleP2 };

            // Test separating axes
            if (TestSeparatingAxis(triangle, Vector3.Cross(triangle.p1 - triangle.p0, triangle.p2 - triangle.p0)))
                return false;

            if (TestSeparatingAxis(triangle, triangle.p0 - triangle.p1))
                return false;

            if (TestSeparatingAxis(triangle, triangle.p1 - triangle.p2))
                return false;

            if (TestSeparatingAxis(triangle, triangle.p2 - triangle.p0))
                return false;

            if (TestSeparatingAxis(triangle, Vector3.UnitX))
                return false;

            if (TestSeparatingAxis(triangle, Vector3.UnitY))
                return false;

            if (TestSeparatingAxis(triangle, Vector3.UnitZ))
                return false;

            // No separating axis found, there is an intersection
            return true;
        }

        private bool TestSeparatingAxis(Triangle triangle, Vector3 axis)
        {
            // Project triangle onto the axis
            float[] triangleProjections = ProjectOntoAxis(triangle, axis);

            // Project bounding box onto the axis (considering orientation)
            float[] aabbProjections = ProjectOntoAxis(this, axis);

            // Check for overlap
            if (triangleProjections[1] < aabbProjections[0] || triangleProjections[0] > aabbProjections[1])
                return true;

            return false;
        }

        private float[] ProjectOntoAxis(Triangle triangle, Vector3 axis)
        {
            float dot0 = Vector3.Dot(triangle.p0, axis);
            float dot1 = Vector3.Dot(triangle.p1, axis);
            float dot2 = Vector3.Dot(triangle.p2, axis);

            return new float[] { Math.Min(dot0, Math.Min(dot1, dot2)), Math.Max(dot0, Math.Max(dot1, dot2)) };
        }

        private float[] ProjectOntoAxis(BoundingBox aabb, Vector3 axis)
        {
            Vector3[] boxVertices = GetBoxVertices(aabb);

            float dotMin = Vector3.Dot(boxVertices[0], axis);
            float dotMax = dotMin;

            for (int i = 1; i < boxVertices.Length; i++)
            {
                float dot = Vector3.Dot(boxVertices[i], axis);
                dotMin = Math.Min(dotMin, dot);
                dotMax = Math.Max(dotMax, dot);
            }

            return new float[] { dotMin, dotMax };
        }

        private Vector3[] GetBoxVertices(BoundingBox aabb)
        {
            Vector3[] vertices = new Vector3[8];

            Vector3 extents = aabb.Extents;
            Vector3 center = aabb.Center;

            // Calculate box vertices
            vertices[0] = center + new Vector3(-extents.X, -extents.Y, -extents.Z);
            vertices[1] = center + new Vector3(extents.X, -extents.Y, -extents.Z);
            vertices[2] = center + new Vector3(-extents.X, extents.Y, -extents.Z);
            vertices[3] = center + new Vector3(extents.X, extents.Y, -extents.Z);
            vertices[4] = center + new Vector3(-extents.X, -extents.Y, extents.Z);
            vertices[5] = center + new Vector3(extents.X, -extents.Y, extents.Z);
            vertices[6] = center + new Vector3(-extents.X, extents.Y, extents.Z);
            vertices[7] = center + new Vector3(extents.X, extents.Y, extents.Z);

            return vertices;
        }
    }
}
