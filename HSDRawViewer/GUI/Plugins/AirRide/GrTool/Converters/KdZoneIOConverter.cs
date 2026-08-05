using HSDRawViewer.IO.AirRide.DataFormat;
using IONET.Core;
using IONET.Core.Model;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Converters
{
    public class KdZoneIOConverter
    {
        public static IOScene ToIOScene(KdZone zone)
        {
            var scene = new IOScene();

            //foreach (var mat in m.Materials)
            //{
            //    var iomat = new IOMaterial()
            //    {
            //        Name = mat.ToString(),
            //    };

            //    switch (mat.Type)
            //    {
            //        case KdType.CEILING: iomat.DiffuseColor = new System.Numerics.Vector4(1, 0, 0, 1); break;
            //        case KdType.FLOOR: iomat.DiffuseColor = new System.Numerics.Vector4(0, 1, 0, 1); break;
            //        case KdType.WALL: iomat.DiffuseColor = new System.Numerics.Vector4(0, 0, 1, 1); break;
            //    }

            //    scene.Materials.Add(iomat);
            //}
            //var material_names = m.Materials.Select(e => e.ToString()).ToArray();

            //var model = new IOModel();
            //scene.Models.Add(model);

            //var mesh = new IOMesh();
            //mesh.Name = "CollisionMesh";
            //model.Meshes.Add(mesh);

            //mesh.Vertices.AddRange(m.Vertices.Select(e =>
            //{
            //    if (e == null || e.Count < 3) return new IOVertex();
            //    return new IOVertex()
            //    {
            //        Position = new System.Numerics.Vector3(e[0], e[1], e[2])
            //    };
            //}));

            //foreach (var g in m.Triangles.GroupBy(e => e.Material))
            //{
            //    var poly = new IOPolygon()
            //    {
            //        PrimitiveType = IOPrimitive.TRIANGLE,
            //        MaterialName = material_names[g.Key],
            //    };
            //    mesh.Polygons.Add(poly);

            //    foreach (var t in g)
            //        poly.Indicies.AddRange(t.Indices);
            //}

            return scene;
        }

        public static KdZone FromIOScene(IOScene scene, out string error)
        {
            error = "";
            KdZone zone = new KdZone();

            //Dictionary<string, int> material_lookup = new Dictionary<string, int>();
            //foreach (var iomat in scene.Materials)
            //{
            //    material_lookup.Add(iomat.Name, m.Materials.Count);
            //    m.Materials.Add(KdMaterial.Parse(iomat.Name));
            //}

            //foreach (var model in scene.Models)
            //{
            //    foreach (var mesh in model.Meshes)
            //    {
            //        var offset = m.Vertices.Count;
            //        foreach (var p in mesh.Polygons)
            //        {
            //            int material_index = material_lookup[p.MaterialName];

            //            for (int i = 0; i < p.Indicies.Count; i += 3)
            //            {
            //                m.Triangles.Add(new KdTriangle()
            //                {
            //                    Material = material_index,
            //                    Indices = new int[] {
            //                        offset + p.Indicies[i],
            //                        offset + p.Indicies[i + 1],
            //                        offset + p.Indicies[i + 2],
            //                    },
            //                });
            //            }
            //        }
            //        m.Vertices.AddRange(mesh.Vertices.Select(e => new List<float>() { e.Position.X, e.Position.Y, e.Position.Z }));
            //    }
            //}

            return zone;
        }
    }
}
