using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRawViewer.Converters;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class JOBJContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_JOBJ) };

        public JOBJContextMenu() : base()
        {
            ToolStripMenuItem Import = new ToolStripMenuItem("Import Model From File");
            Import.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_JOBJ root)
                {
                    MainForm.SelectedDataNode.Collapse();
                    ModelImporter.ReplaceModelFromFile(root);
                }
            };
            Items.Add(Import);

            ToolStripMenuItem ImportSheet = new ToolStripMenuItem("Import Model Info Sheet From File");
            ImportSheet.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_JOBJ root)
                {
                    MainForm.SelectedDataNode.Collapse();
                    var f = Tools.FileIO.OpenFile("JSON (*.json)|*.json");
                    if (f != null)
                    {
                        ModelInfoSheet infoSheet = ModelInfoSheet.Import(f);
                        infoSheet.updateJobj(root);
                    }
                }
            };
            Items.Add(ImportSheet);

            ToolStripMenuItem GenerateMatAnimJoint = new ToolStripMenuItem("Generate and Export MatAnimJoint Structure");
            GenerateMatAnimJoint.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_JOBJ root)
                {
                    var file = Tools.FileIO.SaveFile(ApplicationSettings.HSDFileFilter, "matanim_joint.dat");
                    if(file != null)
                    {
                        HSDRawFile f = new HSDRawFile();
                        HSDRootNode r = new HSDRootNode();
                        r.Name = "matanim_joint";
                        r.Data = GenerateMatAnimJointFromJOBJ(root);
                        f.Roots.Add(r);
                        f.Save(file);
                    }
                }
            };
            Items.Add(GenerateMatAnimJoint);

            ToolStripMenuItem addChild = new ToolStripMenuItem("Add Child");
            Items.Add(addChild);

            ToolStripMenuItem createJOBJ = new ToolStripMenuItem("From Scratch");
            createJOBJ.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_JOBJ root)
                {
                    root.AddChild(new HSD_JOBJ()
                    {
                        SX = 1,
                        SY = 1,
                        SZ = 1,
                        Flags = JOBJ_FLAG.CLASSICAL_SCALING | JOBJ_FLAG.ROOT_XLU
                    });
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            addChild.DropDownItems.Add(createJOBJ);


            ToolStripMenuItem createJOBJFromFile = new ToolStripMenuItem("From File");
            createJOBJFromFile.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_JOBJ root)
                {
                    var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);
                    if(f != null)
                    {
                        HSDRaw.HSDRawFile file = new HSDRaw.HSDRawFile(f);

                        var node = file.Roots[0].Data;
                        if (node is HSD_JOBJ newchild)
                            root.AddChild(newchild);
                    }
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            addChild.DropDownItems.Add(createJOBJFromFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static HSD_MatAnimJoint GenerateMatAnimJointFromJOBJ(HSD_JOBJ node)
        {
            HSD_MatAnimJoint joint = new HSD_MatAnimJoint();

            if (node.Dobj != null)
                foreach (var v in node.Dobj.List)
                {
                    if (joint.MaterialAnimation == null)
                        joint.MaterialAnimation = new HSD_MatAnim();
                    else
                        joint.MaterialAnimation.Add(new HSD_MatAnim() { });
                }

            foreach(var v in node.Children)
            {
                joint.AddChild(GenerateMatAnimJointFromJOBJ(v));
            }

            return joint;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static HSD_AnimJoint GenerateAnimJointFromJOBJ(HSD_JOBJ node)
        {
            HSD_AnimJoint joint = new HSD_AnimJoint();

            foreach (var v in node.Children)
                joint.AddChild(GenerateAnimJointFromJOBJ(v));

            return joint;
        }
    }
}
