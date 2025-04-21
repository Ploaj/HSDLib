using HSDRaw.AirRide.Gr.Data;
using IONET;
using IONET.Core;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus.AirRide
{
    public class GrPositionListContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(KAR_grPositionList) };


        public GrPositionListContextMenu() : base()
        {
            ToolStripMenuItem import = new("Import Model");
            import.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grPositionList data)
                {
                    string f = Tools.FileIO.SaveFile(IOManager.GetImportFileFilter(true, false), "model.dae");
                    if (f != null)
                    {
                        IOScene scene = IOManager.LoadScene(f, new ImportSettings());
                        data._s = ToPositionList(scene.Models[0].Skeleton.RootBones[0])._s;
                    }
                }
            };
            Items.Add(import);

            ToolStripMenuItem export = new("Export Model");
            export.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grPositionList data)
                {
                    string f = Tools.FileIO.SaveFile(IOManager.GetExportFileFilter(true, false), "model.dae");
                    if (f != null)
                    {
                        IOModel model = new();
                        model.Skeleton = new IOSkeleton();
                        model.Skeleton.RootBones.Add(ToBone(data));

                        IOScene scene = new();
                        scene.Models.Add(model);
                        IOManager.ExportScene(scene, f);
                    }
                }
            };
            Items.Add(export);
        }

        private static IOBone ToBone(KAR_grPositionList data)
        {
            IOBone root = new()
            {
                Name = "Origin"
            };

            if (data.PositionData != null)
            {
                int i = 0;
                foreach (KAR_grPositionData p in data.PositionData.Array)
                {
                    Quaternion rot = new Matrix3(p.M11, p.M12, p.M13,
                        p.M21, p.M22, p.M23,
                        0, 0, 1).ExtractRotation();

                    IOBone bone = new()
                    {
                        Name = $"POS_{i++.ToString("D3")}",
                        Translation = new System.Numerics.Vector3(p.X, p.Y, p.Z),
                        Rotation = new System.Numerics.Quaternion(rot.X, rot.Y, rot.Z, rot.W)
                    };

                    root.AddChild(bone);
                }
            }

            return root;
        }

        private static KAR_grPositionList ToPositionList(IOBone root)
        {
            List<KAR_grPositionData> data = new();

            foreach (IOBone bone in root.Children)
            {
                System.Numerics.Quaternion br = bone.Rotation;
                Matrix3 rot = Matrix3.CreateFromQuaternion(new Quaternion(br.X, br.Y, br.Z, br.W));
                data.Add(new KAR_grPositionData()
                {
                    X = bone.TranslationX,
                    Y = bone.TranslationY,
                    Z = bone.TranslationZ,
                    M11 = rot.M11,
                    M12 = rot.M12,
                    M13 = rot.M13,
                    M21 = rot.M21,
                    M22 = rot.M22,
                    M23 = rot.M23,
                });
            }

            return new KAR_grPositionList()
            {
                PositionData = new HSDRaw.HSDArrayAccessor<KAR_grPositionData>()
                {
                    Array = data.ToArray()
                },
                Count = data.Count
            };
        }
    }
}
