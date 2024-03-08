using HSDRaw;
using HSDRaw.AirRide.Gr;
using HSDRaw.AirRide.Gr.Data;
using HSDRaw.GX;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools;
using HSDRawViewer.Tools.AirRide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus.AirRide
{
    public class GrDataContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(KAR_grData) };

        public GrDataContextMenu() : base()
        {
            ToolStripMenuItem genPages = new ToolStripMenuItem("Export Collision Model");
            genPages.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grData data)
                {
                    var f = Tools.FileIO.SaveFile(IONET.IOManager.GetExportFileFilter(true, false), "coll_model.dae");
                    if (f != null)
                    {
                        KARCollisionExporter.ExportCollisionModel(f, data);
                    }
                }
            };
            Items.Add(genPages);


            ToolStripMenuItem test = new ToolStripMenuItem("Test Edit");
            test.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grData data)
                {
                    //data.PartitionNode.Partition = HSDRaw.Tools.KAR.Bucket.GeneratePartitionNode(data.CollisionNode);

                    // analyze splitting pattern

                    var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter, System.IO.Path.GetFileName(MainForm.Instance.FilePath).Replace(".dat", "Model.dat"));
                    if (f != null)
                    {
                        var file = new HSDRawFile(f);

                        foreach (var r in file.Roots)
                        {
                            if (r.Data is KAR_grModel m && m.MainModel != null && m.MainModel.RootNode != null)
                            {
                                var jobj = new LiveJObj(m.MainModel.RootNode);
                                data.PartitionNode.Partition = SpatialPartitionOrganizer.GeneratePartition(jobj, data.CollisionNode);
                            }
                        }
                    }

                    //var tri = data.CollisionNode.Triangles;

                    //var indices = tri
                    //    .Select((value, index) => new { Value = value, Index = index })
                    //    .Where(item => item.Value.Rough != 0)
                    //    .Select(item => item.Index)
                    //    .ToArray();

                    //var ct = data.PartitionNode.Partition.CollidableTriangles;
                    //foreach (var b in data.PartitionNode.Partition.Buckets)
                    //{
                    //    for (int i = b.RoughStart; i < b.RoughStart + b.RoughCount; i++)
                    //    {
                    //        var index = indices[data.PartitionNode.Partition.RoughIndices[i]];

                    //        bool found = false;
                    //        for (int j = b.CollTriangleStart; j < b.CollTriangleStart + b.CollTriangleCount; j++)
                    //        {
                    //            if (ct[j] == index)
                    //            {
                    //                found = true;
                    //                break;
                    //            }
                    //        }

                    //        if (!found)
                    //            Console.WriteLine("!found " + tri[index].Rough);
                    //        else
                    //            Console.WriteLine("found " + tri[index].Rough);

                    //    }
                    //}
                }
            };
            Items.Add(test);


            ToolStripMenuItem clear = new ToolStripMenuItem("Clear Bones");
            clear.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grData data)
                {
                    var j = data.CollisionNode.Joints;
                    foreach (var joint in j)
                        joint.BoneID = 0;
                    data.CollisionNode.Joints = j;

                    var z = data.CollisionNode.ZoneJoints;
                    foreach (var joint in z)
                        joint.BoneID = 0;
                    data.CollisionNode.ZoneJoints = z;
                }
            };
            Items.Add(clear);

            ToolStripMenuItem coll = new ToolStripMenuItem("Import Collision");
            coll.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grData data)
                {
                    var f = FileIO.OpenFile(IONET.IOManager.GetImportFileFilter(true, false), "coll_model.dae");
                    if (f != null)
                    {
                        var jobj = Converters.ModelImporter.ImportModelFromFile(f);
                        data.CollisionNode = GenerateCollisionNode(f);
                        data.PartitionNode.Partition = SpatialPartitionOrganizer.GeneratePartition(new LiveJObj(jobj), data.CollisionNode);
                    }
                }
            };
            Items.Add(coll);



            ToolStripMenuItem ds = new ToolStripMenuItem("Check Collision Angles");
            ds.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grData data)
                {
                    var v = data.CollisionNode.Vertices;

                    float floorMax = float.MinValue;
                    float floorMin = float.MaxValue;

                    float wallMax = float.MinValue;
                    float wallMin = float.MaxValue;

                    float ceilMax = float.MinValue;
                    float ceilMin = float.MaxValue;

                    foreach (var t in data.CollisionNode.Triangles)
                    {
                        var v1 = v[t.V3];
                        var v2 = v[t.V2];
                        var v3 = v[t.V1];
                        var normal = Math3D.CalculateSurfaceNormal(
                            new OpenTK.Mathematics.Vector3(v1.X, v1.Y, v1.Z),
                            new OpenTK.Mathematics.Vector3(v2.X, v2.Y, v2.Z),
                            new OpenTK.Mathematics.Vector3(v3.X, v3.Y, v3.Z));

                        if (t.Flags == KCCollFlag.Floor)
                        {
                            floorMax = Math.Max(floorMax, normal.Y);
                            floorMin = Math.Min(floorMin, normal.Y);
                        }
                        if (t.Flags == KCCollFlag.Wall)
                        {
                            wallMax = Math.Max(wallMax, normal.Y);
                            wallMin = Math.Min(wallMin, normal.Y);
                        }
                        if (t.Flags == KCCollFlag.Ceiling)
                        {
                            ceilMax = Math.Max(ceilMax, normal.Y);
                            ceilMin = Math.Min(ceilMin, normal.Y);
                        }
                        if (t.Flags.HasFlag(KCCollFlag.Unknown))
                        {
                            Console.WriteLine("Unknown Flag Detected");
                        }
                    }

                    Console.WriteLine($"Floor| Max: {floorMax} Min: {floorMin}");
                    Console.WriteLine($"Wall| Max: {wallMax} Min: {wallMin}");
                    Console.WriteLine($"Ceiling| Max: {ceilMax} Min: {ceilMin}");
                }
            };
            Items.Add(ds);
        }

        private static KAR_grCollisionNode GenerateCollisionNode(string filepath)
        {
            var scene = IONET.IOManager.LoadScene(filepath, new IONET.ImportSettings());

            List<GXVector3> points = new List<GXVector3>();
            List<KAR_CollisionTriangle> triangles = new List<KAR_CollisionTriangle>();

            // calculate mesh bounding
            int meshIndex = 0;
            foreach (var m in scene.Models[0].Meshes)
            {
                m.MakeTriangles();

                int pointStart = points.Count;

                foreach (var v in m.Vertices)
                {
                    points.Add(new GXVector3() { X = v.Position.X, Y = v.Position.Y, Z = v.Position.Z });
                }

                foreach (var p in m.Polygons)
                {
                    for (int i = 0; i < p.Indicies.Count; i += 3)
                    {
                        //var v1 = m.Vertices[p.Indicies[i + 2]];
                        //var v2 = m.Vertices[p.Indicies[i + 1]];
                        //var v3 = m.Vertices[p.Indicies[i]];
                        //var normal = Math3D.CalculateSurfaceNormal(
                        //    new OpenTK.Mathematics.Vector3(v1.Position.X, v1.Position.Y, v1.Position.Z),
                        //    new OpenTK.Mathematics.Vector3(v2.Position.X, v2.Position.Y, v2.Position.Z),
                        //    new OpenTK.Mathematics.Vector3(v3.Position.X, v3.Position.Y, v3.Position.Z));

                        //if (normal.Y > 0.8f)
                        //{
                        //    flag = KCCollFlag.Floor;
                        //} 
                        //else if (normal.Y < -0.8)
                        //{
                        //    flag = KCCollFlag.Ceiling;
                        //}
                        //else
                        //{
                        //    flag = KCCollFlag.Wall;
                        //}

                        KCCollFlag flag = KCCollFlag.Floor;

                        if (p.MaterialName.ToLower().Contains("wall"))
                        {
                            flag = KCCollFlag.Wall;
                        }
                        else
                        if (p.MaterialName.ToLower().Contains("ceiling"))
                        {
                            flag = KCCollFlag.Ceiling;
                        }

                        triangles.Add(new KAR_CollisionTriangle()
                        {
                            V1 = p.Indicies[i + 2] + pointStart,
                            V2 = p.Indicies[i + 1] + pointStart,
                            V3 = p.Indicies[i + 0] + pointStart,
                            Flags = flag,
                            //SegmentMove = true,
                        });
                    }
                }

                meshIndex++;
            }

            KAR_grCollisionNode collision = new KAR_grCollisionNode();

            collision.Vertices = points.ToArray();
            collision.Triangles = triangles.ToArray();

            collision.Joints = new KAR_CollisionJoint[]
            {
                new KAR_CollisionJoint()
                {
                    BoneID = 0,
                    FaceStart = 0,
                    FaceSize = triangles.Count,
                    VertexStart = 0,
                    VertexSize = points.Count
                }
            };

            return collision;
        }
    }
}
