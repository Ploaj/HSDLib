using HSDRawViewer.Converters.Melee;
using IONET;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.Converters.SBM
{
    public class NewModeltoCollData
    {
        /// <summary>
        /// 
        /// </summary>
        public struct Triangle
        {
            public Vector3 P1, P2, P3;
            public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
            {
                P1 = p1; P2 = p2; P3 = p3;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="planeNormal"></param>
        /// <param name="planePoint"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <returns></returns>
        public static Vector3? IntersectPlane(Vector3 planeNormal, float planeD, Vector3 lineStart, Vector3 lineEnd)
        {
            // Compute signed distances of the points from the plane
            float distanceStart = planeNormal.X * lineStart.X + planeNormal.Y * lineStart.Y + planeNormal.Z * lineStart.Z + planeD;
            float distanceEnd = planeNormal.X * lineEnd.X + planeNormal.Y * lineEnd.Y + planeNormal.Z * lineEnd.Z + planeD;

            // If both points are on the same side of the plane, no intersection
            if (distanceStart * distanceEnd > 0)
                return null;

            // If one point is exactly on the plane, return it as the intersection
            if (distanceStart == 0)
                return lineStart;
            if (distanceEnd == 0)
                return lineEnd;

            // Calculate the interpolation factor (t) for the intersection point
            float t = distanceStart / (distanceStart - distanceEnd);

            // Interpolate between the points to find the intersection point
            return new Vector3(
                lineStart.X + t * (lineEnd.X - lineStart.X),
                lineStart.Y + t * (lineEnd.Y - lineStart.Y),
                lineStart.Z + t * (lineEnd.Z - lineStart.Z)
            );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static float DotProduct(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="triangles"></param>
        /// <param name="planeNormal"></param>
        /// <param name="planePoint"></param>
        /// <returns></returns>
        private static List<(Vector3, Vector3)> FindIntersectionLines(List<Triangle> triangles, Vector3 planeNormal, float planeD)
        {
            List<(Vector3, Vector3)> intersectionLines = new();

            foreach (Triangle triangle in triangles)
            {
                // Check all three edges of the triangle
                Vector3? intersect1 = IntersectPlane(planeNormal, planeD, triangle.P1, triangle.P2);
                Vector3? intersect2 = IntersectPlane(planeNormal, planeD, triangle.P2, triangle.P3);
                Vector3? intersect3 = IntersectPlane(planeNormal, planeD, triangle.P3, triangle.P1);

                List<Vector3> intersections = new();

                if (intersect1.HasValue) intersections.Add(intersect1.Value);
                if (intersect2.HasValue) intersections.Add(intersect2.Value);
                if (intersect3.HasValue) intersections.Add(intersect3.Value);

                if (intersections.Count == 2)
                {
                    intersectionLines.Add((intersections[0], intersections[1]));
                }
            }

            return intersectionLines;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public static void Convert(string filePath, string ssfFile)
        {
            IONET.Core.IOScene scene = IOManager.LoadScene(filePath, new ImportSettings() { });

            // gather all triangles
            List<Triangle> triangles = new();
            foreach (IONET.Core.Model.IOModel v in scene.Models)
            {
                foreach (IONET.Core.Model.IOMesh m in v.Meshes)
                {
                    m.MakeTriangles();

                    foreach (IONET.Core.Model.IOPolygon p in m.Polygons)
                    {
                        for (int i = 0; i < p.Indicies.Count; i += 3)
                        {
                            IONET.Core.Model.IOVertex v1 = m.Vertices[p.Indicies[i]];
                            IONET.Core.Model.IOVertex v2 = m.Vertices[p.Indicies[i + 1]];
                            IONET.Core.Model.IOVertex v3 = m.Vertices[p.Indicies[i + 2]];
                            triangles.Add(new Triangle(
                                new Vector3(v1.Position.X, v1.Position.Y, v1.Position.Z),
                                new Vector3(v2.Position.X, v2.Position.Y, v2.Position.Z),
                                new Vector3(v3.Position.X, v3.Position.Y, v3.Position.Z)));
                        }
                    }

                }
            }

            //Vector3 planeNormal = new Vector3(0, 0, 1); // z = 0 plane (XY plane)
            //Vector3 planePoint = new Vector3(0, 0, 0);  // Origin

            //List<Triangle> triangles = new List<Triangle>
            //{
            //    new Triangle(new Vector3(1, 1, 1), new Vector3(-1, 1, -1), new Vector3(0, -1, 1)),
            //    new Triangle(new Vector3(-1, -1, 2), new Vector3(2, 2, -2), new Vector3(1, 0, 1)),
            //    // Add more triangles
            //};

            //List<(Vector3, Vector3)> intersectionLines = FindIntersectionLines(triangles, planeNormal, 0);

            //foreach (var line in intersectionLines)
            //{
            //    System.Diagnostics.Debug.WriteLine($"Line from ({line.Item1.X}, {line.Item1.Y}, {line.Item1.Z}) to ({line.Item2.X}, {line.Item2.Y}, {line.Item2.Z})");
            //}

            List<(Vector3, Vector3)> lines = FindIntersectionLines(triangles, -Vector3.UnitZ, 0);

            SSFGroup group = new()
            {
                Name = "TestGroup"
            };
            foreach ((Vector3, Vector3) line in lines)
            {
                group.Lines.Add(new SSFLine()
                {
                    Vertex1 = group.Vertices.Count,
                    Vertex2 = group.Vertices.Count + 1,
                    Material = "Basic",
                });
                group.Vertices.Add(new SSFVertex() { X = line.Item1.X, Y = line.Item1.Y });
                group.Vertices.Add(new SSFVertex() { X = line.Item2.X, Y = line.Item2.Y });
            }
            Fuse(group);

            SSF ssf = new();
            ssf.Groups.Add(group);
            ssf.Save(ssfFile);
        }

        private static void Fuse(SSFGroup g)
        {
            List<Vector2> verts = new();
            Dictionary<int, int> vertexToPoint = new();

            int i = 0;
            foreach (SSFVertex v in g.Vertices)
            {
                Vector2 point = new(v.X, v.Y);
                if (!verts.Contains(point))
                {
                    vertexToPoint.Add(i, verts.Count);
                    verts.Add(point);
                }
                else
                {
                    vertexToPoint.Add(i, verts.IndexOf(point));
                }
                i++;
            }

            foreach (SSFLine l in g.Lines)
            {
                l.Vertex1 = vertexToPoint[l.Vertex1];
                l.Vertex2 = vertexToPoint[l.Vertex2];
            }

            System.Diagnostics.Debug.WriteLine(g.Vertices.Count + "->" + verts.Count);

            g.Vertices = verts.Select(e => new SSFVertex() { X = e.X, Y = e.Y }).ToList();
        }
    }
}
