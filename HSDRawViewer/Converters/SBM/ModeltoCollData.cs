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
        private class Line
        {
            public Vector3 P0;
            public Vector3 P1;

            public float Slope { get => (P1.Y - P0.Y) / (P1.X - P0.X); }

            public override bool Equals(object obj)
            {
                return obj is Line l && l.P0.Equals(P0) && l.P1.Equals(P1);
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
            Dictionary<Vector3, CollVertex> vertToVert = new Dictionary<Vector3, CollVertex>();
            
            // generate and connect lines
            List<CollLine> collLines = new List<CollLine>();
            Dictionary<Vector3, CollLine> leftPointToLine = new Dictionary<Vector3, CollLine>();
            Dictionary<Vector3, CollLine> rightPointToLine = new Dictionary<Vector3, CollLine>();
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
    }
}
