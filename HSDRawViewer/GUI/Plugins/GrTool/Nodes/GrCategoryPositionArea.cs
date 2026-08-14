using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Tools;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrCategoryPositionArea : GrCategoryNode<KdPositionArea, GrPositionAreaNode>
    {
        public GrCategoryPositionArea(string name, ObservableList<KdPositionArea> list) : base(name, list)
        {
            list.Refresh();
        }

        public override void BuildContextMenu(ContextMenuStrip menu, GrNode selected_node)
        {
            base.BuildContextMenu(menu, selected_node);

            if (selected_node != this) return;

            menu.Items.Add("New Position Area", null, (s, e) => {
                list.Add(new KdPositionArea()
                {
                    P1 = new KdVector(-10, -10, -10),
                    P2 = new KdVector(10, 10, 10),
                    Forward = new KdVector(0, 0, -1),
                });
            });

            menu.Items.Add(new ToolStripSeparator());

            //menu.Items.Add("Import and Replace", null, (s, e) => {

            //    //var filePath = FileIO.OpenFile(IOManager.GetImportFileFilter(), "");
            //    //if (filePath == null) return;

            //    //var scene = IOManager.LoadScene(filePath, new ImportSettings());

            //    //foreach (var m in scene.Models)
            //    //{
            //    //    if (m.Skeleton == null) return;

            //    //    foreach (var b in m.Skeleton.BreathFirstOrder())
            //    //    {
            //    //        if (b.Name.StartsWith("P") && int.TryParse(b.Name[1..], out int index))
            //    //        {
            //    //            while (index >= list.Count)
            //    //            {
            //    //                list.Add(new KdPosition()
            //    //                {
            //    //                    Position = new KdVector(0, 0, 0),
            //    //                    Forward = new KdVector(0, 0, 1),
            //    //                    Up = new KdVector(0, 1, 0),
            //    //                });
            //    //            }

            //    //            KdPositionConverter.FromIOBone(b).CopyTo(list[index]);
            //    //        }
            //    //    }
            //    //}
            //});

            //menu.Items.Add("Export All", null, (s, e) => {

            //    var filePath = FileIO.SaveFile(IOManager.GetExportFileFilter(), Text + ".dae");
            //    if (filePath == null) return;

            //    var scene = new IOScene();
            //    var model = new IOModel();
            //    scene.Models.Add(model);

            //    model.Skeleton = new IOSkeleton();
            //    var root = new IOBone()
            //    {
            //        Name = Text,
            //        Scale = Vector3.One
            //    };
            //    model.Skeleton.RootBones.Add(root);

            //    int i = 0;
            //    foreach (KdPositionArea t in list)
            //    {
            //        var bm = KdPositionConverter.GenerateIOBoneMesh($"P{i:D3}", t);
            //        root.AddChild(bm.Item1);
            //        model.Meshes.Add(bm.Item2);
            //        i++;
            //    }

            //    IOManager.ExportScene(scene, filePath);
            //});
        }

    }
}
