using HSDRaw.Common.Animation;
using System;
using System.Windows.Forms;
using System.Linq;
using HSDRaw.Tools;
using HSDRawViewer.GUI;

namespace HSDRawViewer.ContextMenus
{
    public class AnimJointContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_AnimJoint) };

        public class AObjClass
        {
            public float EndFrame { get; set; }

            public AOBJ_Flags Flags { get; set; }
        }

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

#if DEBUG

            MenuItem reverse = new MenuItem("Reverse");
            reverse.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_AnimJoint anim)
                {
                    foreach (var n in anim.BreathFirstList)
                    {
                        if (n.AOBJ != null)
                            foreach(var a in n.AOBJ.FObjDesc.List)
                            {
                                var keys = a.GetDecodedKeys();
                                var frameCount = keys.Max(e=>e.Frame);

                                Console.WriteLine(frameCount);
                                
                                foreach (var k in keys)
                                {
                                    k.Frame = frameCount - k.Frame;
                                }

                                a.SetKeys(keys, a.TrackType);
                            }
                    }
                }
            };
            MenuItems.Add(reverse);
#endif
            MenuItem editAOBJ = new MenuItem("Edit All AObj Flags");
            editAOBJ.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_AnimJoint anim)
                {
                    AObjClass setting = new AObjClass();

                    using (PropertyDialog d = new PropertyDialog("AObj Settings", setting))
                    {
                        if (d.ShowDialog() == DialogResult.OK)
                        {
                            foreach (var n in anim.BreathFirstList)
                            {
                                if (n.AOBJ != null)
                                {
                                    n.AOBJ.EndFrame = setting.EndFrame;
                                    n.AOBJ.Flags = setting.Flags;
                                }
                            }
                        }
                    }

                }
            };
            MenuItems.Add(editAOBJ);
        }
    }
}
