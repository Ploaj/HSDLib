using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using System;
using System.Linq;
using System.Windows.Forms;

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
            ToolStripMenuItem OpenAsAJ = new("Add AOBJ");
            OpenAsAJ.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_AnimJoint anim)
                {
                    anim.AOBJ = new HSD_AOBJ();
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            Items.Add(OpenAsAJ);


            ToolStripMenuItem addChild = new("Add Child");
            Items.Add(addChild);

            ToolStripMenuItem createJOBJ = new("From Scratch");
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
            addChild.DropDownItems.Add(createJOBJ);


            ToolStripMenuItem createJOBJFromFile = new("From File");
            createJOBJFromFile.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_AnimJoint root)
                {
                    string f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);
                    if (f != null)
                    {
                        HSDRaw.HSDRawFile file = new(f);

                        HSDRaw.HSDAccessor node = file.Roots[0].Data;
                        if (node is HSD_AnimJoint newchild)
                            root.AddChild(newchild);
                    }
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            addChild.DropDownItems.Add(createJOBJFromFile);

#if DEBUG

            ToolStripMenuItem reverse = new ToolStripMenuItem("Reverse");
            reverse.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_AnimJoint anim)
                {
                    foreach (var n in anim.TreeList)
                    {
                        if (n.AOBJ != null)
                            foreach (var a in n.AOBJ.FObjDesc.List)
                            {
                                var player = new FOBJ_Player(a);
                                player.Reverse();
                                a.SetKeys(player.Keys, a.TrackType);
                            }
                    }
                }
            };
            Items.Add(reverse);
#endif
            ToolStripMenuItem editAOBJ = new("Edit All AObj Flags");
            editAOBJ.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_AnimJoint anim)
                {
                    AObjClass setting = new();

                    using PropertyDialog d = new("AObj Settings", setting);
                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        foreach (HSD_AnimJoint n in anim.TreeList)
                        {
                            if (n.AOBJ != null)
                            {
                                n.AOBJ.EndFrame = setting.EndFrame;
                                n.AOBJ.Flags = setting.Flags;
                            }
                        }
                    }

                }
            };
            Items.Add(editAOBJ);


            ToolStripMenuItem invertAnimation = new("Invert");
            invertAnimation.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_AnimJoint anim &&
                    anim.AOBJ != null &&
                    anim.AOBJ.FObjDesc != null)
                {
                    HSDRaw.Common.HSD_JOBJ jobjdesc = new()
                    {
                        SX = 1,
                        SY = 1,
                        SZ = 1,
                    };

                    LiveJObj jobj = new(jobjdesc);

                    System.Collections.Generic.List<FOBJ_Player> tracks = anim.AOBJ.FObjDesc.List.Select(e => new FOBJ_Player(e)).ToList();
                    AnimNode node = new();

                    for (int i = 0; i <= anim.AOBJ.EndFrame; i++)
                    {
                        jobj.ApplyAnimation(tracks, i);
                        jobj.RecalculateTransforms(null, false);

                        OpenTK.Mathematics.Matrix4 world = jobj.WorldTransform.Inverted();
                        OpenTK.Mathematics.Vector3 t = world.ExtractTranslation();
                        OpenTK.Mathematics.Vector3 r = world.ExtractRotationEuler();

                        node.AddLinearKey(JointTrackType.HSD_A_J_TRAX, i, t.X);
                        node.AddLinearKey(JointTrackType.HSD_A_J_TRAY, i, t.Y);
                        node.AddLinearKey(JointTrackType.HSD_A_J_TRAZ, i, t.Z);
                        node.AddLinearKey(JointTrackType.HSD_A_J_ROTX, i, r.X);
                        node.AddLinearKey(JointTrackType.HSD_A_J_ROTY, i, r.Y);
                        node.AddLinearKey(JointTrackType.HSD_A_J_ROTZ, i, r.Z);
                    }

                    {
                        jobj.ApplyAnimation(tracks, 0);
                        jobj.RecalculateTransforms(null, false);

                        OpenTK.Mathematics.Matrix4 world = jobj.WorldTransform.Inverted();
                        OpenTK.Mathematics.Vector3 t = world.ExtractTranslation();
                        OpenTK.Mathematics.Vector3 r = world.ExtractRotationEuler();

                        node.AddLinearKey(JointTrackType.HSD_A_J_TRAX, 6000, t.X);
                        node.AddLinearKey(JointTrackType.HSD_A_J_TRAY, 6000, t.Y);
                        node.AddLinearKey(JointTrackType.HSD_A_J_TRAZ, 6000, t.Z);
                        node.AddLinearKey(JointTrackType.HSD_A_J_ROTX, 6000, r.X);
                        node.AddLinearKey(JointTrackType.HSD_A_J_ROTY, 6000, r.Y);
                        node.AddLinearKey(JointTrackType.HSD_A_J_ROTZ, 6000, r.Z);
                    }

                    JointAnimManager m = new();
                    m.Nodes.Add(node);

                    HSD_AnimJoint na = m.ToAnimJoint(jobjdesc, AOBJ_Flags.ANIM_LOOP);
                    anim.AOBJ = na.AOBJ;
                }
            };
            Items.Add(invertAnimation);

        }
    }
}
