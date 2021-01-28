using HSDRaw.Common.Animation;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class AnimJointContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_AnimJoint) };

        public AnimJointContextMenu() : base()
        {
            MenuItem OpenAsAJ = new MenuItem("Add AOBJ");
            OpenAsAJ.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_AnimJoint anim)
                {
                    anim.AOBJ = new HSD_AOBJ();
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            MenuItems.Add(OpenAsAJ);


            MenuItem addChild = new MenuItem("Add Child");
            MenuItems.Add(addChild);

            MenuItem createJOBJ = new MenuItem("From Scratch");
            createJOBJ.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_AnimJoint root)
                {
                    root.AddChild(new HSD_AnimJoint()
                    {
                    });
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            addChild.MenuItems.Add(createJOBJ);


            MenuItem createJOBJFromFile = new MenuItem("From File");
            createJOBJFromFile.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_AnimJoint root)
                {
                    var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);
                    if (f != null)
                    {
                        HSDRaw.HSDRawFile file = new HSDRaw.HSDRawFile(f);

                        var node = file.Roots[0].Data;
                        if (node is HSD_AnimJoint newchild)
                            root.AddChild(newchild);
                    }
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            addChild.MenuItems.Add(createJOBJFromFile);


            MenuItem compress = new MenuItem("Compress");
            compress.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_AnimJoint anim)
                {
                    Tools.AnimationCompressor.CompressAnimation(anim, 0.001f);
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            MenuItems.Add(compress);
        }
    }
}
