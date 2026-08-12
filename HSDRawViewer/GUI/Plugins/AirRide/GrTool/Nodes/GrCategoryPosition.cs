using HSDRawViewer.GUI.Plugins.AirRide.GrTool.Converters;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;
using IONET;
using IONET.Core;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using System;
using System.Numerics;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public class GrCategoryPosition : GrCategoryNode<KdPosition, GrPositionNode>
    {
        public GrCategoryPosition(string name, ObservableList<KdPosition> list) : base(name, list)
        {
            list.Refresh();
        }

        public override void BuildContextMenu(ContextMenuStrip menu)
        {
            menu.Items.Add("New Position", null, (s, e) => {
                list.Add(new KdPosition()
                {
                    Position = new KdVector(0, 0, 0),
                    Forward = new KdVector(0, 0, 1),
                    Up = new KdVector(0, 1, 0),
                });
            });

            menu.Items.Add(new ToolStripSeparator());

            menu.Items.Add("Import and Replace", null, (s, e) => {

                var filePath = FileIO.OpenFile(IOManager.GetImportFileFilter(), "");
                if (filePath == null) return;

                var scene = IOManager.LoadScene(filePath, new ImportSettings());

                foreach (var m in scene.Models)
                {
                    if (m.Skeleton == null) return;

                    foreach (var b in m.Skeleton.BreathFirstOrder())
                    {
                        if (b.Name.StartsWith("P") && int.TryParse(b.Name[1..], out int index))
                        {
                            while (index >= list.Count)
                            {
                                list.Add(new KdPosition()
                                {
                                    Position = new KdVector(0, 0, 0),
                                    Forward = new KdVector(0, 0, 1),
                                    Up = new KdVector(0, 1, 0),
                                });
                            }

                            KdPositionConverter.FromIOBone(b).CopyTo(list[index]);
                        }
                    }
                }
            });

            menu.Items.Add("Export All", null, (s, e) => {

                var filePath = FileIO.SaveFile(IOManager.GetExportFileFilter(), Text + ".dae");
                if (filePath == null) return;

                var scene = new IOScene();
                var model = new IOModel();
                scene.Models.Add(model);

                model.Skeleton = new IOSkeleton();
                var root = new IOBone()
                {
                    Name = Text,
                    Scale = Vector3.One
                };
                model.Skeleton.RootBones.Add(root);

                int i = 0;
                foreach (KdPosition t in list)
                {
                    root.AddChild(KdPositionConverter.GenerateIOBone($"P{i:D3}", t));
                    i++;
                }

                IOManager.ExportScene(scene, filePath);
            });

            menu.Items.Add(new ToolStripSeparator());

            menu.Items.Add("Delete", null, (s, e) => {
                OnDeleteNode?.Invoke(this);
            });
        }
    }
}
