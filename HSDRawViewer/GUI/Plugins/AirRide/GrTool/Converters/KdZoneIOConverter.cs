using HSDRawViewer.IO.AirRide.DataFormat;
using IONET.Collada.Core.Geometry;
using IONET.Core;
using IONET.Core.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Converters
{
    public class KdZoneIOConverter
    {
        public static KdZone CreateBlankSize(float size)
        {
            var zone = new KdZone()
            {
                Name = "",
                Parent = -1,
                Type = 0,
                Flags = 0,
                LinkedZone = 0,

            };

            float h = size / 2.0f;

            zone.Vertices.AddRange( new List<List<float>>()
            {
                new List<float>() { -h, -h, -h }, // 0
                new List<float>() { h, -h, -h}, // 1
                new List<float>() { h,  h, -h }, // 2
                new List<float>() { -h, h, -h }, // 3

                new List<float>() { -h, -h,  h }, // 4
                new List<float>() { h, -h,  h }, // 5
                new List<float>() { h,  h,  h }, // 6
                new List<float>() { -h,  h,  h }, // 7
            });

            // Front (-Z)
            zone.Triangles.Add(new KdZoneTriangle(0, 0, new int[] { 0, 2, 1 }));
            zone.Triangles.Add(new KdZoneTriangle(0, 0, new int[] { 0, 3, 2 }));

            // Back (+Z)
            zone.Triangles.Add(new KdZoneTriangle(0, 0, new int[] { 4, 5, 6 }));
            zone.Triangles.Add(new KdZoneTriangle(0, 0, new int[] { 4, 6, 7 }));

            // Left (-X)
            zone.Triangles.Add(new KdZoneTriangle(0, 0, new int[] { 0, 7, 3 }));
            zone.Triangles.Add(new KdZoneTriangle(0, 0, new int[] { 0, 4, 7 }));

            // Right (+X)
            zone.Triangles.Add(new KdZoneTriangle(0, 0, new int[] { 1, 2, 6 }));
            zone.Triangles.Add(new KdZoneTriangle(0, 0, new int[] { 1, 6, 5 }));

            // Bottom (-Y)
            zone.Triangles.Add(new KdZoneTriangle(0, 0, new int[] { 0, 1, 5 }));
            zone.Triangles.Add(new KdZoneTriangle(0, 0, new int[] { 0, 5, 4 }));

            // Top (+Y)
            zone.Triangles.Add(new KdZoneTriangle(0, 0, new int[] { 3, 7, 6 }));
            zone.Triangles.Add(new KdZoneTriangle(0, 0, new int[] { 3, 6, 2 }));
            
            return zone;
        }

        public static IOScene ToIOScene(KdZone zone)
        {
            var scene = new IOScene();

            scene.Materials.Add(new IOMaterial()
            {
                Name = "ZoneMaterial",
                DiffuseColor = Vector4.One,
            });

            var model = new IOModel();
            scene.Models.Add(model);

            var mesh = new IOMesh();
            mesh.Name = "ZoneMesh";
            model.Meshes.Add(mesh);

            mesh.Vertices.AddRange(zone.Vertices.Select(e =>
            {
                if (e == null || e.Count < 3) return new IOVertex();
                return new IOVertex()
                {
                    Position = new System.Numerics.Vector3(e[0], e[1], e[2])
                };
            }));

            foreach (var g in zone.Triangles.GroupBy(e => e.Flags))
            {
                var mat_key = $"F{g.Key.ToString("X8")}";
                var mat = scene.Materials.Find(e => e.Name.Equals(mat_key));

                if (mat == null)
                {
                    mat = new IOMaterial()
                    {
                        Name = mat_key,
                        DiffuseColor = Vector4.One,
                    };
                    scene.Materials.Add(mat);
                }

                var poly = new IOPolygon()
                {
                    PrimitiveType = IOPrimitive.TRIANGLE,
                    MaterialName = mat.Name,
                };
                mesh.Polygons.Add(poly);

                foreach (var t in g)
                    poly.Indicies.AddRange(t.Indices);
            }

            return scene;
        }
        
        private static int TryParseMaterialFlag(string flag)
        {
            if (flag.StartsWith("F"))
            {
                if (int.TryParse(flag[1..],
                    NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture, 
                    out int f))
                {
                    return f;
                }
            }

            return 0;
        }

        private struct Plane
        {
            public Vector3 Normal;
            public float Distance;

            public bool SamePlane(Plane b)
            {
                float normalDot = MathF.Abs(
                    Vector3.Dot(Normal, b.Normal));

                return normalDot > 0.999f &&
                       MathF.Abs(Distance - b.Distance) < 0.001f;
            }
        }

        private class Triangle
        {
            public Plane Plane;

            public int Material;

            public int V1;

            public int V2;

            public int V3;

            public Triangle(IOMesh mesh, int mat, int v1, int v2, int v3)
            {
                Material = mat;
                Plane = GetPlane(mesh, v1, v2, v3);
                V1 = v1;
                V2 = v2;
                V3 = v3;
            }

        }

        private static Plane GetPlane(IOMesh mesh, int v1, int v2, int v3)
        {
            var a = mesh.Vertices[v1].Position;
            var b = mesh.Vertices[v2].Position;
            var c = mesh.Vertices[v3].Position;

            Vector3 normal = Vector3.Normalize(
                Vector3.Cross(b - a, c - a));

            return new Plane
            {
                Normal = normal,
                Distance = Vector3.Dot(normal, a)
            };
        }


        public static bool FromIOMesh(IOScene scene, IOMesh mesh, out KdZone zone, out string error)
        {
            // clear unneeded vertex information
            foreach (var v in mesh.Vertices)
            {
                v.UVs.Clear();
                v.Colors.Clear();
                v.Normal = Vector3.Zero;
            }
            mesh.Optimize();

            error = string.Empty;
            zone = null;

            // check vertices
            if (mesh.Vertices.Count != 8)
            {
                error = $"{mesh.Name} has incorrect vertex count: got {mesh.Vertices.Count} expected 8";
                return false;
            }

            // check faces
            int face_count = 0;
            foreach (var p in mesh.Polygons)
            {
                if (p.PrimitiveType != IOPrimitive.TRIANGLE) continue;
                face_count += p.Indicies.Count / 3;
            }

            if (face_count != 12)
            {
                error = $"{mesh.Name} has incorrect face count: got {face_count} expected 12";
                return false;
            }

            // gather triangles
            var triangles = new List<Triangle>();
            foreach (var p in mesh.Polygons)
            {
                if (p.PrimitiveType != IOPrimitive.TRIANGLE) continue;

                for (int i = 0; i + 3 < p.Indicies.Count; i += 3)
                {
                    triangles.Add(new Triangle(mesh, TryParseMaterialFlag(p.MaterialName), p.Indicies[i + 0], p.Indicies[i + 1], p.Indicies[i + 2]));
                }
            }

            // group triangles by plane
            zone = new KdZone();
            List<Plane> uniquePlanes = new List<Plane>();
            foreach (var t in triangles)
            {
                int index = uniquePlanes.FindIndex(e => e.SamePlane(t.Plane));

                if (index == -1)
                {
                    index = uniquePlanes.Count;
                    uniquePlanes.Add(t.Plane);
                }

                zone.Triangles.Add(new KdZoneTriangle()
                {
                    Flags = t.Material,
                    Indices = new int[] { t.V1, t.V2, t.V3, },
                });
            }

            //if (uniquePlanes.Count >= 6)
            //{
            //    error = $"Too many planes: expected {6} got {uniquePlanes.Count}";
            //    return false;
            //}

            zone.Vertices.AddRange(mesh.Vertices.Select(e => new List<float>() { e.Position.X, e.Position.Y, e.Position.Z }));

            return true;
        }

        public static bool FromIOScene(IOScene scene, out KdZone zone, out string error)
        {
            error = "error";
            zone = null;

            foreach (var model in scene.Models)
            {
                foreach (var mesh in model.Meshes)
                {
                    if (FromIOMesh(scene, mesh, out zone, out error))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
