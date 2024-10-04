using HSDRaw.Common.Animation;
using System;
using System.Windows.Forms;
using System.Linq;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.Rendering.Models;
using HSDRaw.Tools;
using System.Collections.Generic;
using HSDRawViewer.Rendering;

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
            ToolStripMenuItem OpenAsAJ = new ToolStripMenuItem("Add AOBJ");
            OpenAsAJ.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_AnimJoint anim)
                {
                    anim.AOBJ = new HSD_AOBJ();
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            Items.Add(OpenAsAJ);


            ToolStripMenuItem addChild = new ToolStripMenuItem("Add Child");
            Items.Add(addChild);

            ToolStripMenuItem createJOBJ = new ToolStripMenuItem("From Scratch");
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


            ToolStripMenuItem createJOBJFromFile = new ToolStripMenuItem("From File");
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
            ToolStripMenuItem editAOBJ = new ToolStripMenuItem("Edit All AObj Flags");
            editAOBJ.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_AnimJoint anim)
                {
                    AObjClass setting = new AObjClass();

                    using (PropertyDialog d = new PropertyDialog("AObj Settings", setting))
                    {
                        if (d.ShowDialog() == DialogResult.OK)
                        {
                            foreach (var n in anim.TreeList)
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
            Items.Add(editAOBJ);


            ToolStripMenuItem invertAnimation = new ToolStripMenuItem("Invert");
            invertAnimation.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_AnimJoint anim && 
                    anim.AOBJ != null &&
                    anim.AOBJ.FObjDesc != null)
                {
                    var jobjdesc = new HSDRaw.Common.HSD_JOBJ()
                    {
                        SX = 1,
                        SY = 1,
                        SZ = 1,
                    };

                    LiveJObj jobj = new LiveJObj(jobjdesc);

                    var tracks = anim.AOBJ.FObjDesc.List.Select(e => new FOBJ_Player(e)).ToList();
                    var node = new AnimNode();

                    for (int i = 0; i <= anim.AOBJ.EndFrame; i++)
                    {
                        jobj.ApplyAnimation(tracks, i);
                        jobj.RecalculateTransforms(null, false);

                        var world = jobj.WorldTransform.Inverted();
                        var t = world.ExtractTranslation();
                        var r = world.ExtractRotationEuler();

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

                        var world = jobj.WorldTransform.Inverted();
                        var t = world.ExtractTranslation();
                        var r = world.ExtractRotationEuler();

                        node.AddLinearKey(JointTrackType.HSD_A_J_TRAX, 6000, t.X);
                        node.AddLinearKey(JointTrackType.HSD_A_J_TRAY, 6000, t.Y);
                        node.AddLinearKey(JointTrackType.HSD_A_J_TRAZ, 6000, t.Z);
                        node.AddLinearKey(JointTrackType.HSD_A_J_ROTX, 6000, r.X);
                        node.AddLinearKey(JointTrackType.HSD_A_J_ROTY, 6000, r.Y);
                        node.AddLinearKey(JointTrackType.HSD_A_J_ROTZ, 6000, r.Z);
                    }

                    JointAnimManager m = new JointAnimManager();
                    m.Nodes.Add(node);

                    var na = m.ToAnimJoint(jobjdesc, AOBJ_Flags.ANIM_LOOP);
                    anim.AOBJ = na.AOBJ;
                }
            };
            Items.Add(invertAnimation);

        }
    }
}
