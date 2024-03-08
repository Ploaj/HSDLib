using HSDRaw.AirRide.Gr.Data;
using IONET;
using IONET.Core;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus.AirRide
{
    public class GrPositionListContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(KAR_grPositionList) };


        public GrPositionListContextMenu() : base()
        {
            ToolStripMenuItem import = new ToolStripMenuItem("Import Model");
            import.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grPositionList data)
                {
                    var f = Tools.FileIO.SaveFile(IOManager.GetImportFileFilter(true, false), "model.dae");
                    if (f != null)
                    {
                        var scene = IOManager.LoadScene(f, new ImportSettings());
                        data._s = ToPositionList(scene.Models[0].Skeleton.RootBones[0])._s;
                    }
                }
            };
            Items.Add(import);

            ToolStripMenuItem export = new ToolStripMenuItem("Export Model");
            export.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is KAR_grPositionList data)
                {
                    var f = Tools.FileIO.SaveFile(IOManager.GetExportFileFilter(true, false), "model.dae");
                    if (f != null)
                    {
                        IOModel model = new IOModel();
                        model.Skeleton = new IOSkeleton();
                        model.Skeleton.RootBones.Add(ToBone(data));

                        IOScene scene = new IOScene();
                        scene.Models.Add(model);
                        IOManager.ExportScene(scene, f);
                    }
                }
            };
            Items.Add(export);
        }

        private static IOBone ToBone(KAR_grPositionList data)
        {
            IOBone root = new IOBone()
            {
                Name = "Origin"
            };

            if (data.PositionData != null)
            {
                int i = 0;
                foreach (var p in data.PositionData.Array)
                {
                    var rot = new Matrix3(p.M11, p.M12, p.M13,
                        p.M21, p.M22, p.M23,
                        0, 0, 1).ExtractRotation();

                    IOBone bone = new IOBone()
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
            List<KAR_grPositionData> data = new List<KAR_grPositionData>();

            foreach (var bone in root.Children)
            {
                var br = bone.Rotation;
                var rot = Matrix3.CreateFromQuaternion(new Quaternion(br.X, br.Y, br.Z, br.W));
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
