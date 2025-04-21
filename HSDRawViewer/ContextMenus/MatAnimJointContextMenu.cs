using HSDRaw.Common.Animation;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class MatAnimJointContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_MatAnimJoint) };

        public MatAnimJointContextMenu() : base()
        {
            ToolStripMenuItem addChild = new("Add Child");
            Items.Add(addChild);

            ToolStripMenuItem createJOBJ = new("From Scratch");
            createJOBJ.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_MatAnimJoint root)
                {
                    root.AddChild(new HSD_MatAnimJoint()
                    {
                    });
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            addChild.DropDownItems.Add(createJOBJ);


            ToolStripMenuItem createJOBJFromFile = new("From File");
            createJOBJFromFile.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_MatAnimJoint root)
                {
                    string f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);
                    if (f != null)
                    {
                        HSDRaw.HSDRawFile file = new(f);

                        HSDRaw.HSDAccessor node = file.Roots[0].Data;
                        if (node is HSD_MatAnimJoint newchild)
                            root.AddChild(newchild);
                    }
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            addChild.DropDownItems.Add(createJOBJFromFile);
        }
    }
}
