using HSDRaw.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using HSDRawViewer.GUI.Plugins.Melee;
using HSDRaw.Melee.Gr;

namespace HSDRawViewer.Converters
{
    /// <summary>
    /// This special converter will take a model and a plane as input and generate a colldata for it
    /// </summary>
    public class ModeltoCollData
    {
        public class Line
        {
            public Vector2 P0;
            public Vector2 P1;

            public float Slope { get => (P1.Y - P0.Y) / (P1.X - P0.X); }

            public float Angle
            {
                get
                {
                    var angle = Math.Atan2((P1.Y - P0.Y), (P1.X - P0.X));

                    if (angle < 0)
                        angle += 360;

                    return (float)angle;
                }
            }

            public override bool Equals(object obj)
            {
                return obj is Line l && ((P1.Equals(l.P1) && P0.Equals(l.P0)) || (P1.Equals(l.P0) && P0.Equals(l.P1)));
            }

            public override int GetHashCode()
            {
                return P0.GetHashCode() | P1.GetHashCode();
            }

            public override string ToString()
            {
                return $"{P0}, {P1}";
            }

            public float AngleBetween(Line line)
            {
                return line.Angle - Angle;
            }

            public bool FindIntersection(Line line, out Vector2 intersection)
            {
                var p0_x = P0.X;
                var p0_y = P0.Y;
                var p1_x = P1.X;
                var p1_y = P1.Y;

                var p2_x = line.P0.X;
                var p2_y = line.P0.Y;
                var p3_x = line.P1.X;
                var p3_y = line.P1.Y;

                intersection = Vector2.Zero;

                float s1_x, s1_y, s2_x, s2_y;
                s1_x = p1_x - p0_x; s1_y = p1_y - p0_y;
                s2_x = p3_x - p2_x; s2_y = p3_y - p2_y;

                float s, t;
                s = (-s1_y * (p0_x - p2_x) + s1_x * (p0_y - p2_y)) / (-s2_x * s1_y + s1_x * s2_y);
                t = (s2_x * (p0_y - p2_y) - s2_y * (p0_x - p2_x)) / (-s2_x * s1_y + s1_x * s2_y);

                // Collision detected
                if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
                {
                    intersection = new Vector2(p0_x + (t * s1_x), p0_y + (t * s1_y));
                    return true;
                }
                // No collision
                return false; 
            }

            const float EPSILON = 0.001f;
            public bool IsPointOnLine(Vector2 point)
            {
                float a = (P1.Y - P0.Y) / (P1.X - P1.X);
                float b = P0.Y - a * P0.X;
                if (Math.Abs(point.Y - (a * point.X + b)) < EPSILON)
                {
                    return true;
                }

                return false;
            }
        }

        public static List<Vector2> PolyTrace(IEnumerable<Line> lines)
        {
            var pointToPoint = new Dictionary<Vector2, HashSet<Vector2>>();

            var startPoint = new Vector2(float.MinValue, 0);

            foreach (var line in lines)
            {
                if (!pointToPoint.ContainsKey(line.P0))
                    pointToPoint.Add(line.P0, new HashSet<Vector2>());

                if (!pointToPoint.ContainsKey(line.P1))
                    pointToPoint.Add(line.P1, new HashSet<Vector2>());

                pointToPoint[line.P0].Add(line.P1);
                pointToPoint[line.P1].Add(line.P0);

                if (line.P0.X > startPoint.X)
                    startPoint = line.P0;

                if (line.P1.X > startPoint.X)
                    startPoint = line.P1;
            }

            var poly = new List<Vector2>();

            var point = startPoint;
            var prevPoint = startPoint + Vector2.UnitX;

            while(poly.Count == 0 || point != startPoint)
            {
                Vector2 nextPoint = point;
                var smallestAngle = double.MaxValue;

                foreach(var connected in pointToPoint[point])
                {
                    var a = (prevPoint - point).Normalized();
                    var b = (connected - point).Normalized();
                    var angle = Math.Atan2(a.X * b.Y - a.Y * b.X, a.X * b.X + a.Y * b.Y);

                    if (angle <= 0)
                        angle += Math.PI * 2;

                    if(angle < smallestAngle)
                    {
                        smallestAngle = angle;
                        nextPoint = connected;
                    }
                    poly.Add(point);
                    poly.Add(connected);

                }

                poly.Add(point);
                poly.Add(nextPoint);

                prevPoint = point;
                point = nextPoint;
            }

            return poly;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static List<Vector2> ModelToLines(HSD_JOBJ root)
        {
            // get model as easier to access form
            ModelExporter mx = new ModelExporter(root, new ModelExportSettings() { Optimize = true }, new Tools.JointMap());
            var scene = mx.Scene;

            HashSet<Line> lines = new HashSet<Line>();
            List<Vector2> polygon = new List<Vector2>();

            // go through each triangle and check collision with plane
            // if it collides, get the intersection points as a line
            //foreach (var m in scene.Models[0].Meshes)
            {
                var m = scene.Models[0].Meshes[1];
                // for each polygon
                foreach (var p in m.Polygons)
                {
                    if (p.PrimitiveType != IONET.Core.Model.IOPrimitive.TRIANGLE)
                        continue;

                    for (int i = 0; i < p.Indicies.Count; i += 3)
                    {
                        var v1 = IOVertexToTKVector(m.Vertices[p.Indicies[i]]);
                        var v2 = IOVertexToTKVector(m.Vertices[p.Indicies[i + 1]]);
                        var v3 = IOVertexToTKVector(m.Vertices[p.Indicies[i + 2]]);

                        var l1 = new Line() { P0 = v1, P1 = v2 };
                        var l2 = new Line() { P0 = v2, P1 = v3 };
                        var l3 = new Line() { P0 = v1, P1 = v3 };

                        if(!lines.Contains(l1))
                            lines.Add(l1);
                        if (!lines.Contains(l2))
                            lines.Add(l2);
                        if (!lines.Contains(l3))
                            lines.Add(l3);
                    }
                }

                // break overlapping lines
                var lineList = new List<Line>();

                foreach (var line in lines)
                {
                    List<Vector2> intersections = new List<Vector2>();

                    // check if this line intersects any other line
                    foreach(var l in lines)
                    {
                        if(line != l && !line.IsPointOnLine(l.P0) && !line.IsPointOnLine(l.P1) &&
                            line.FindIntersection(l, out Vector2 intersection) &&
                            intersection != line.P0 &&
                            intersection != line.P1 &&
                            !intersections.Contains(intersection))
                            intersections.Add(intersection);
                    }

                    if (intersections.Count == 0)
                        lineList.Add(line);
                    else
                    {
                        intersections.Add(line.P1);
                        Vector2 prev = line.P0;
                        foreach(var l in intersections.OrderBy(e => Vector2.DistanceSquared(line.P0, e)))
                        {
                            lineList.Add(new Line() { P0 = prev, P1 = l });
                            prev = l;
                        }
                    }
                }

                //polygon.AddRange(PolyTrace(lineList));

                foreach(var line in lineList)
                {
                    polygon.Add(line.P0);
                    polygon.Add(line.P1);
                }
            }



            // merge all polygons into one

            // outside clockwise
            // inside counter clockwise


            return polygon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private static Vector2 IOVertexToTKVector(IONET.Core.Model.IOVertex vertex)
        {
            return new Vector2(vertex.Position.Z, vertex.Position.Y);
        }

        public static SBM_Coll_Data JOBJtoCollData(HSD_JOBJ root)
        {
            List<Line> Lines = new List<Line>();

            // TODO;
            /*var scene = ModelExporter.JOBJtoScene(root);

            // go through each triangle and check collision with plane
            // if it collides, get the intersection points as a line
            if (scene.HasMeshes)
                foreach (var m in scene.Meshes)
                {
                    if (m.HasFaces)
                        foreach (var f in m.Faces)
                        {
                            if (f.IndexCount != 3)
                                throw new NotSupportedException("Only Triangles are supported for coll generation");

                            // triangle
                            var v1 = AssimpToTK(m.Vertices[f.Indices[0]]);
                            var v2 = AssimpToTK(m.Vertices[f.Indices[1]]);
                            var v3 = AssimpToTK(m.Vertices[f.Indices[2]]);

                            // triangle plane collision line
                            Vector3 i0, i1;
                            if (TriangleIntersectsPlane(v1, v2, v3, Vector3.Zero, Vector3.UnitZ, out i0, out i1)
                                && i1 != i0)
                            {
                                var line = new Line() { P0 = i0, P1 = i1 };
                                if(!Lines.Contains(line))
                                    Lines.Add(line);
                            }
                        }
                }*/


            CollLineGroup lineGroup = new CollLineGroup();

            // gather unique vertices
            Dictionary<Vector2, CollVertex> vertToVert = new Dictionary<Vector2, CollVertex>();

            // generate and connect lines
            List<CollLine> collLines = new List<CollLine>();
            Dictionary<Vector2, CollLine> leftPointToLine = new Dictionary<Vector2, CollLine>();
            Dictionary<Vector2, CollLine> rightPointToLine = new Dictionary<Vector2, CollLine>();
            foreach (var l in Lines)
            {
                if (!vertToVert.ContainsKey(l.P0))
                    vertToVert.Add(l.P0, new CollVertex(l.P0.X, l.P0.Y));

                if (!vertToVert.ContainsKey(l.P1))
                    vertToVert.Add(l.P1, new CollVertex(l.P1.X, l.P1.Y));

                // join lines with same slope
                /*if (leftPointToLine.ContainsKey(l.P1) && l.Slope == leftPointToLine[l.P1].Slope)
               {
                   var line = leftPointToLine[l.P1];
                   var oldp = new Vector3(line.v1.X, line.v1.Y, 0);
                   rightPointToLine.Remove(oldp);
                   if (!rightPointToLine.ContainsKey(l.P0))
                       rightPointToLine.Add(l.P0, line);
                   line.v1 = vertToVert[l.P0];
                   continue;
               }
                if (rightPointToLine.ContainsKey(l.P0) && l.Slope == rightPointToLine[l.P0].Slope)
               {
                   var line = rightPointToLine[l.P0];
                   var oldp = new Vector3(line.v2.X, line.v2.Y, 0);
                   leftPointToLine.Remove(oldp);
                   if (!leftPointToLine.ContainsKey(l.P1))
                       leftPointToLine.Add(l.P1, line);
                   line.v2 = vertToVert[l.P1];
                   continue;
               }*/

                CollLine cl = new CollLine();
                cl.Group = lineGroup;
                cl.Material = HSDRaw.Melee.Gr.CollMaterial.Basic;
                cl.CollisionFlag = CollPhysics.Top;
                cl.v1 = vertToVert[l.P0];
                cl.v2 = vertToVert[l.P1];

                // TODO: generate wall flag based on normal?

                if (!leftPointToLine.ContainsKey(l.P0))
                    leftPointToLine.Add(l.P0, cl);
                if (!rightPointToLine.ContainsKey(l.P1))
                    rightPointToLine.Add(l.P1, cl);
                collLines.Add(cl);
            }

            List<CollLineGroup> collLineGroups = new List<CollLineGroup>();
            collLineGroups.Add(lineGroup);

            lineGroup.CalcuateRange(vertToVert.Values.ToArray());

            var cd = new SBM_Coll_Data();

            CollDataBuilder.GenerateCollData(collLines, collLineGroups, cd);

            return cd;
        }

        private static void OutputSVG(List<Line> Lines)
        {
            svg svg = new svg();

            var groups = new List<svgShape>();

            svgGroup g = new svgGroup();
            List<svgShape> lines = new List<svgShape>();
            float scale = 20;
            foreach (var l in Lines)
                lines.Add(new svgLine() { x1 = l.P0.X * scale, y1 = l.P0.Y * scale, x2 = l.P1.X * scale, y2 = l.P1.Y * scale });
            g.shapes = lines.ToArray();
            groups.Add(g);

            svg.groups = groups.ToArray();

            using (var settings = new XmlTextWriter(new FileStream("test.svg", FileMode.Create), Encoding.UTF8))
            {
                settings.Indentation = 4;
                settings.Formatting = Formatting.Indented;
                //settings.Namespaces = false;
                //settings.Settings.OmitXmlDeclaration = true;

                XmlSerializer serializer = new XmlSerializer(typeof(svg));
                serializer.Serialize(settings, svg);
            }
        }

        private static int LinePlaneIntersection(Vector3 P0, Vector3 P1, Vector3 V0, Vector3 N0, out Vector3 I)
        {
            I = Vector3.Zero;
            Vector3 u = P1 - P0;
            Vector3 w = P0 - V0;

            float D = Vector3.Dot(N0, u);
            float N = -Vector3.Dot(N0, w);

            float SMALL_NUM = 0.00001f;
            if (Math.Abs(D) < SMALL_NUM)
            {           // segment is parallel to plane
                if (N == 0)                      // segment lies in plane
                    return 2;
                else
                    return 0;                    // no intersection
            }
            // they are not parallel
            // compute intersect param
            float sI = N / D;
            if (sI < 0 || sI > 1)
                return 0;                        // no intersection

            I = P0 + sI * u;                  // compute segment intersect point
            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rayOrigin"></param>
        /// <param name="rayDirection"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static double GetRayToLineSegmentIntersection(Vector2 rayOrigin, Vector2 rayDirection, Vector2 point1, Vector2 point2)
        {
            rayDirection.Normalize();

            Vector2 v1 = rayOrigin - point1;
            Vector2 v2 = point2 - point1;
            Vector2 v3 = new Vector2(-rayDirection.Y, rayDirection.X);

            float dot = Vector2.Dot(v2, v3);
            if (Math.Abs(dot) < 0.000001)
                return -1.0f;

            float t1 = CrossProduct(v2, v1) / dot;
            float t2 = Vector2.Dot(v1, v3) / dot;

            if (t1 >= 0.0 && (t2 >= 0.0 && t2 <= 1.0))
                return t1;

            return -1.0f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private static float CrossProduct(Vector2 v1, Vector2 v2)
        {
            return (v1.X * v2.Y) - (v1.Y * v2.X);
        }
    }
}
