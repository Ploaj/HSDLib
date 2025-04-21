using HSDRaw;
using HSDRaw.AirRide.Gr;
using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools;
using HSDRawViewer.Tools.AirRide;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus.AirRide
{
    public class GrDataContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(KAR_grData) };

        public GrDataContextMenu() : base()
        {
            ToolStripMenuItem ImportCollModel = new("Import Collision Model");
            ImportCollModel.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grData data)
                {
                    string f = FileIO.OpenFile(IONET.IOManager.GetImportFileFilter(true, false), "coll_model.dae");
                    if (f != null)
                    {
                        // generate collision node
                        data.CollisionNode = KARCollisionImporter.GenerateCollisionNode(f);

                        // generate partition
                        HSDRaw.Common.HSD_JOBJ jobj = Converters.ModelImporter.ImportModelFromFile(f);
                        data.PartitionNode.Partition = SpatialPartitionOrganizer.GeneratePartition(new LiveJObj(jobj), data.CollisionNode);
                    }
                }
            };
            Items.Add(ImportCollModel);

            ToolStripMenuItem ExportCollModel = new("Export Collision Model");
            ExportCollModel.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grData data)
                {
                    string modelFileName = System.IO.Path.GetFileNameWithoutExtension(MainForm.Instance.FilePath) + "Model.dat";
                    string modelPath = FileIO.OpenFile(ApplicationSettings.HSDFileFilter, modelFileName);

                    if (modelPath != null)
                    {
                        string f = FileIO.SaveFile(IONET.IOManager.GetExportFileFilter(true, false), "coll_model.dae");
                        if (f != null)
                        {
                            KARCollisionExporter.ExportCollisionModel(f, modelPath, data);
                        }
                    }

                }
            };
            Items.Add(ExportCollModel);

            //ToolStripMenuItem ImportZoneModel = new ToolStripMenuItem("Import Zone Model");
            //ImportZoneModel.Click += (sender, args) =>
            //{
            //    if (MainForm.SelectedDataNode.Accessor is KAR_grData data)
            //    {
            //        var f = FileIO.OpenFile(IONET.IOManager.GetImportFileFilter(true, false), "coll_model.dae");
            //        if (f != null)
            //        {
            //            // generate collision node
            //            data.CollisionNode = KARCollisionImporter.GenerateCollisionNode(f);

            //            // generate partition
            //            var jobj = Converters.ModelImporter.ImportModelFromFile(f);
            //            data.PartitionNode.Partition = SpatialPartitionOrganizer.GeneratePartition(new LiveJObj(jobj), data.CollisionNode);
            //        }
            //    }
            //};
            //Items.Add(ImportZoneModel);

            ToolStripMenuItem ExportZoneModel = new("Export Zone Model");
            ExportZoneModel.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grData data)
                {
                    string modelFileName = System.IO.Path.GetFileNameWithoutExtension(MainForm.Instance.FilePath) + "Model.dat";
                    string modelPath = FileIO.OpenFile(ApplicationSettings.HSDFileFilter, modelFileName);

                    if (modelPath != null)
                    {
                        string f = FileIO.SaveFile(IONET.IOManager.GetExportFileFilter(true, false), "zone_model.dae");
                        if (f != null)
                        {
                            KARCollisionExporter.ExportZoneModel(f, modelPath, data);
                        }
                    }

                }
            };
            Items.Add(ExportZoneModel);


            Items.Add(new ToolStripSeparator());


            ToolStripMenuItem ExportAreaBones = new("Export Area Bones");
            ExportAreaBones.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grData data)
                {
                    string f = FileIO.SaveFile(IONET.IOManager.GetExportFileFilter(true, false), "area_pos.dae");
                    if (f != null)
                    {
                        KARPositionExporter.ExportAreaPositions(f, data.PositionNode.ItemAreaPos[0]);
                    }
                }
            };
            Items.Add(ExportAreaBones);



            ToolStripMenuItem test = new("Test Edit");
            test.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grData data)
                {
                    //data.PartitionNode.Partition = HSDRaw.Tools.KAR.Bucket.GeneratePartitionNode(data.CollisionNode);

                    // analyze splitting pattern

                    string f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter, System.IO.Path.GetFileName(MainForm.Instance.FilePath).Replace(".dat", "Model.dat"));
                    if (f != null)
                    {
                        HSDRawFile file = new(f);

                        foreach (HSDRootNode r in file.Roots)
                        {
                            if (r.Data is KAR_grModel m && m.MainModel != null && m.MainModel.RootNode != null)
                            {
                                LiveJObj jobj = new(m.MainModel.RootNode);
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


            ToolStripMenuItem clear = new("Clear Bones");
            clear.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grData data)
                {
                    KAR_CollisionJoint[] j = data.CollisionNode.Joints;
                    foreach (KAR_CollisionJoint joint in j)
                        joint.BoneID = 0;
                    data.CollisionNode.Joints = j;

                    KAR_ZoneCollisionJoint[] z = data.CollisionNode.ZoneJoints;
                    foreach (KAR_ZoneCollisionJoint joint in z)
                        joint.BoneID = 0;
                    data.CollisionNode.ZoneJoints = z;
                }
            };
            Items.Add(clear);


            ToolStripMenuItem ds = new("Remove Zone Type Flags");
            ds.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grData data)
                {
                    KAR_ZoneCollisionTriangle[] tri = data.CollisionNode.ZoneTriangles;
                    KAR_ZoneCollisionJoint[] joint = data.CollisionNode.ZoneJoints;

                    HashSet<int> flags = new();

                    int ji = 0;
                    foreach (KAR_ZoneCollisionJoint j in joint)
                    {
                        for (int i = j.ZoneFaceStart; i < j.ZoneFaceStart + j.ZoneFaceSize; i++)
                        {
                            KAR_ZoneCollisionTriangle t = tri[i];

                            if (!flags.Contains(t.Type) || j.x14 != -1)
                            {
                                flags.Add(t.Type);

                                System.Diagnostics.Debug.WriteLine($"Index: {ji} Type: {t.Type} x14: {j.x14} x18: {j.x18}");
                                if (j.x14_param != null)
                                {
                                    System.Diagnostics.Debug.WriteLine($"\t0x14 {j.x14_param._s.Length}");
                                }
                                if (j.x18_param != null)
                                {
                                    System.Diagnostics.Debug.WriteLine($"\t0x18 {j.x18_param._s.Length}");
                                }
                            }
                        }
                        ji++;
                    }

                    data.CollisionNode.ZoneTriangles = tri;
                    data.CollisionNode.ZoneJoints = joint;
                }
            };
            Items.Add(ds);


            //ToolStripMenuItem ds = new ToolStripMenuItem("Check Collision Angles");
            //ds.Click += (sender, args) =>
            //{
            //    if (MainForm.SelectedDataNode.Accessor is KAR_grData data)
            //    {
            //        var v = data.CollisionNode.Vertices;

            //        float floorMax = float.MinValue;
            //        float floorMin = float.MaxValue;

            //        float wallMax = float.MinValue;
            //        float wallMin = float.MaxValue;

            //        float ceilMax = float.MinValue;
            //        float ceilMin = float.MaxValue;

            //        foreach (var t in data.CollisionNode.Triangles)
            //        {
            //            var v1 = v[t.V3];
            //            var v2 = v[t.V2];
            //            var v3 = v[t.V1];
            //            var normal = Math3D.CalculateSurfaceNormal(
            //                new OpenTK.Mathematics.Vector3(v1.X, v1.Y, v1.Z),
            //                new OpenTK.Mathematics.Vector3(v2.X, v2.Y, v2.Z),
            //                new OpenTK.Mathematics.Vector3(v3.X, v3.Y, v3.Z));

            //            if (t.Flags == KCCollFlag.Floor)
            //            {
            //                floorMax = Math.Max(floorMax, normal.Y);
            //                floorMin = Math.Min(floorMin, normal.Y);
            //            }
            //            if (t.Flags == KCCollFlag.Wall)
            //            {
            //                wallMax = Math.Max(wallMax, normal.Y);
            //                wallMin = Math.Min(wallMin, normal.Y);
            //            }
            //            if (t.Flags == KCCollFlag.Ceiling)
            //            {
            //                ceilMax = Math.Max(ceilMax, normal.Y);
            //                ceilMin = Math.Min(ceilMin, normal.Y);
            //            }
            //            if (t.Flags.HasFlag(KCCollFlag.Unknown))
            //            {
            //                Console.WriteLine("Unknown Flag Detected");
            //            }
            //        }

            //        Console.WriteLine($"Floor| Max: {floorMax} Min: {floorMin}");
            //        Console.WriteLine($"Wall| Max: {wallMax} Min: {wallMin}");
            //        Console.WriteLine($"Ceiling| Max: {ceilMax} Min: {ceilMin}");
            //    }
            //};
            //Items.Add(ds);
        }

    }
}
