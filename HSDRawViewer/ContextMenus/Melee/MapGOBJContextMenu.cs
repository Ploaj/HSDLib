using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Melee.Gr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus.Melee
{
    public class MapGOBJContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_Map_GOBJ) };

        public MapGOBJContextMenu() : base()
        {
            ToolStripMenuItem genJointAnim = new ToolStripMenuItem("Create Joint Animation Bank");
            genJointAnim.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_Map_GOBJ gobj &&
                    gobj.JointAnimations == null &&
                    gobj.RootNode != null)
                {
                    gobj.JointAnimations = new HSDNullPointerArrayAccessor<HSDRaw.Common.Animation.HSD_AnimJoint>();
                    gobj.JointAnimations.Array = new HSDRaw.Common.Animation.HSD_AnimJoint[] { JOBJContextMenu.GenerateAnimJointFromJOBJ(gobj.RootNode) };

                    MainForm.SelectedDataNode.Refresh();
                }
            };
            Items.Add(genJointAnim);


            ToolStripMenuItem genMatAnim = new ToolStripMenuItem("Create Material Animation Bank");
            genMatAnim.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_Map_GOBJ gobj && 
                    gobj.MaterialAnimations == null &&
                    gobj.RootNode != null)
                {
                    gobj.MaterialAnimations = new HSDNullPointerArrayAccessor<HSDRaw.Common.Animation.HSD_MatAnimJoint>();
                    gobj.MaterialAnimations.Array = new HSDRaw.Common.Animation.HSD_MatAnimJoint[] { JOBJContextMenu.GenerateMatAnimJointFromJOBJ(gobj.RootNode) };
                    
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            Items.Add(genMatAnim);


            ToolStripMenuItem addFog = new ToolStripMenuItem("Add Fog Struct");
            addFog.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_Map_GOBJ gobj)
                {
                    gobj.Fog = new HSD_FogDesc();
                    MainForm.SelectedDataNode.Refresh();
                }
            };
            Items.Add(addFog);

#if DEBUG
            ToolStripMenuItem removeBones = new ToolStripMenuItem("Remove Unused Bones");
            removeBones.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_Map_GOBJ gobj)
                {
                    // remove joints
                    RemoveUnused(gobj.RootNode, out List<int> indices);

                    // remove from animations
                    if (gobj.JointAnimations != null)
                    {
                        foreach (var j in gobj.JointAnimations.Array)
                        {
                            var joints = j.TreeList;
                            foreach (var r in indices)
                            {
                                RemoveJoint(j, joints[r]);
                            }
                        }
                    }
                    if (gobj.MaterialAnimations != null)
                    {
                        foreach (var j in gobj.MaterialAnimations.Array)
                        {
                            var joints = j.TreeList;
                            foreach (var r in indices)
                            {
                                RemoveJoint(j, joints[r]);
                            }
                        }
                    }
                }
            };
            Items.Add(removeBones);
#endif
        }

        private static void RemoveJoint<T>(HSDTreeAccessor<T> tree, HSDTreeAccessor<T> toRemove) where T : HSDTreeAccessor<T>
        {
            if (tree == null)
                return;

            // check if sibling to removed
            if (tree.Next == toRemove)
                tree.Next = toRemove.Next;

            // check if first child to be removed
            if (tree.Child == toRemove)
                tree.Child = toRemove.Next;

            RemoveJoint(tree.Next, toRemove);
            RemoveJoint(tree.Child, toRemove);
        }

        private static void RemoveUnused(HSD_JOBJ jobj, out List<int> indices)
        {
            indices = new List<int>();

            var tree = jobj.TreeList;

            // gather all used (either has dobj or rigging)
            HashSet<HSD_JOBJ> used = new HashSet<HSD_JOBJ>();
            foreach (var j in tree)
            {
                if (j.Dobj != null)
                {
                    if (!used.Contains(j))
                        used.Add(j);

                    foreach (var d in j.Dobj.List)
                    {
                        foreach (var p in d.Pobj?.List)
                        {
                            if (p.EnvelopeWeights != null)
                            foreach (var e in p.EnvelopeWeights)
                            {
                                foreach (var j2 in e.JOBJs)
                                {
                                    if (!used.Contains(j2))
                                        used.Add(j2);
                                }
                            }
                        }
                    }
                }
            }

            // if you are used then your parents are used
            foreach (var j in tree)
            {
                if (used.Contains(j))
                    continue;

                if (j.Child != null && j.Child.TreeList.Any(e => used.Contains(e)))
                    used.Add(j);
            }

            //
            for (int i = 0; i < tree.Count; i++)
            {
                if (!used.Contains(tree[i]))
                {
                    RemoveJoint(jobj, tree[i]);
                    indices.Add(i);
                }
            }

            System.Diagnostics.Debug.WriteLine($"Used {used.Count} Original {tree.Count}");
        }
    }
}
