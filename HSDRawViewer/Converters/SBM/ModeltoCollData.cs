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
using HSDRawViewer.Rendering;

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

            public override bool Equals(object obj)
            {
                return obj is Line l && ((P1.Equals(l.P1) && P0.Equals(l.P0)) || (P1.Equals(l.P0) && P0.Equals(l.P1)));
            }

            public override int GetHashCode()
            {
                return P0.GetHashCode() | P1.GetHashCode();
            }

            /*public bool Intersects(Line l)
            {
                var rayOrigin = P0;
                var rayDirection = (P1 - P0).Normalized();

                var v1 = rayOrigin - point1;
                var v2 = point2 - point1;
                var v3 = new Vector(-rayDirection.Y, rayDirection.X);
                
                var dot = v2 * v3;
                if (Math.Abs(dot) < 0.000001)
                    return null;

                var t1 = Vector.CrossProduct(v2, v1) / dot;
                var t2 = (v1 * v3) / dot;

                if (t1 >= 0.0 && (t2 >= 0.0 && t2 <= 1.0))
                    return t1;

                return false;
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Line> Polypath(IEnumerable<Line> lines)
        {
            foreach (var l in lines)
            {
                // poly test
                int count1 = 0;
                int count2 = 0;
                var origin = (l.P0 + l.P1) / 2;
                var r = (l.P0 - l.P1).Normalized();
                var ray1 = new Vector2(r.Y * -1, r.X);
                var ray2 = new Vector2(r.Y, r.X * -1);

                foreach (var t in lines)
                {
                    if (l != t)
                    {
                        if (GetRayToLineSegmentIntersection(origin, ray1, t.P0, t.P1) != -1)
                            count1++;

                        if (GetRayToLineSegmentIntersection(origin, ray2, t.P0, t.P1) != -1)
                            count2++;
                    }
                }

                Console.WriteLine(l.P0.ToString() + " " + l.P1.ToString() + " " + count1 + " " + count2);

                if (count1 % 2 == 0 || count2 % 2 == 0)
                    yield return l;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static List<Vector2> ModelToLines(HSD_JOBJ root)
        {
            // gather all lines on plane
            List<Vector2> connectedLines = new List<Vector2>();

            // get model as easier to access form
            ModelExporter mx = new ModelExporter(root, new ModelExportSettings() { Optimize = true }, new Dictionary<int, string>());
            var scene = mx.Scene;

            List<Line> lines = new List<Line>();

            // go through each triangle and check collision with plane
            // if it collides, get the intersection points as a line
            foreach (var m in scene.Models[0].Meshes)
            {
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

                        lines.Add(l1);
                        lines.Add(l2);
                        lines.Add(l3);
                    }
                }
                break;
            }

            //var path = Polypath(lines);

            Console.WriteLine(lines.Count + " " + lines.Distinct().Count());

            foreach (var l in (lines.Distinct()))
            {
                var origin = (l.P0 + l.P1) / 2;
                var r = (l.P1 - l.P0).Normalized() / 8;

                connectedLines.Add(l.P0);
                connectedLines.Add(l.P1);

                connectedLines.Add(origin);
                connectedLines.Add(origin + new Vector2(r.Y * -1, r.X));
            }

            // connect points
            /*Dictionary<Vector3, Line> pointToRight = new Dictionary<Vector3, Line>();
            Dictionary<Vector3, Line> pointToLeft = new Dictionary<Vector3, Line>();
            foreach (var l in lines)
            {
                if (!pointToRight.ContainsKey(l.P0))
                    pointToRight.Add(l.P0, l);

                if (!pointToLeft.ContainsKey(l.P1))
                    pointToLeft.Add(l.P1, l);
            }


            var current_line = lines[0];

            connectedLines.Add(current_line.P0);
            connectedLines.Add(current_line.P1);

            while (lines.Count > 0)
            {
                Console.WriteLine(lines.Count);

                lines.Remove(current_line);

                if (pointToRight.ContainsKey(current_line.P0))
                {
                    var nextLine = pointToRight[current_line.P0];
                    connectedLines.Add(nextLine.P1);
                    current_line = nextLine;
                }
                else
                if (pointToLeft.ContainsKey(current_line.P1))
                {
                    var nextLine = pointToRight[current_line.P1];
                    connectedLines.Add(nextLine.P0);
                    current_line = nextLine;
                }
                else
                {
                    current_line = lines[0];
                }
            }*/


            return connectedLines;
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

        private static bool TriangleIntersectsPlane(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 planePosition, Vector3 planeNormal, out Vector3 i0, out Vector3 i1)
        {
            List<Vector3> intersections = new List<Vector3>();
            Vector3 I;
            i0 = Vector3.Zero;
            i1 = Vector3.Zero;

            if (intersect3D_SegmentPlane(p0, p1, planePosition, planeNormal, out I) == 1)
                intersections.Add(I);
            if (intersect3D_SegmentPlane(p1, p2, planePosition, planeNormal, out I) == 1)
                intersections.Add(I);
            if (intersect3D_SegmentPlane(p0, p2, planePosition, planeNormal, out I) == 1)
                intersections.Add(I);

            if (intersections.Count == 2)
            {
                i0 = intersections[0];
                i1 = intersections[1];
                return true;
            }

            return false;
        }

        private static int intersect3D_SegmentPlane(Vector3 P0, Vector3 P1, Vector3 V0, Vector3 N0, out Vector3 I)
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
