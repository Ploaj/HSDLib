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
            MenuItem addChild = new MenuItem("Add Child");
            MenuItems.Add(addChild);

            MenuItem createJOBJ = new MenuItem("From Scratch");
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
            addChild.MenuItems.Add(createJOBJ);


            MenuItem createJOBJFromFile = new MenuItem("From File");
            createJOBJFromFile.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_MatAnimJoint root)
                {
                    var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);
                    if (f != null)
                    {
                        HSDRaw.HSDRawFile file = new HSDRaw.HSDRawFile(f);

                        var node = file.Roots[0].Data;
                        if (node is HSD_MatAnimJoint newchild)
                            root.AddChild(newchild);
                    }
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            addChild.MenuItems.Add(createJOBJFromFile);
        }
    }
}
