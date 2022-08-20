﻿using HSDRaw;
using HSDRaw.Common;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools;
using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Controls.JObjEditor
{
    public partial class DockableJointTree : DockContent
    {
        /// <summary>
        /// 
        /// </summary>
        public JointMap _jointMap { get; internal set; }

        private int joint_count = 0;

        private HSD_JOBJ _root;

        public delegate void JObjSelected(int index, HSD_JOBJ jobj);
        public JObjSelected SelectJObj;

        /// <summary>
        /// 
        /// </summary>
        public DockableJointTree()
        {
            InitializeComponent();

            Text = "Joint Tree";

            _jointMap = new JointMap();

            // prevent user closing
            CloseButtonVisible = false;
            FormClosing += (sender, args) =>
            {
                if (args.CloseReason == CloseReason.UserClosing)
                {
                    args.Cancel = true;
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        public void SetJObj(HSD_JOBJ jobj)
        {
            _root = jobj;
            _jointMap.Clear();
            RefreshTree();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshTree()
        {
            treeJOBJ.BeginUpdate();
            treeJOBJ.Nodes.Clear();
            joint_count = 0;
            UpdateJObjDisplay(_root, null);
            treeJOBJ.EndUpdate();
            treeJOBJ.ExpandAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dobj"></param>
        /// <param name="parent"></param>
        private void UpdateJObjDisplay(HSD_JOBJ jobj, TreeNode parent)
        {
            TreeNode node = new TreeNode();

            // if joint map is loaded then display joint map name
            if (_jointMap[joint_count] != null)
            {
                node.Text = $"{joint_count} : {_jointMap[joint_count]}";
            }
            else
            // if class name exists then display class name
            if (!string.IsNullOrEmpty(jobj.ClassName))
            {
                node.Text = $"{joint_count} : {jobj.ClassName}";
            }
            else
            // otherwise just display joint + index
            {
                node.Text = "Joint_" + joint_count;
            }
            joint_count++;

            // add jobj as a tag
            node.Tag = jobj;

            // add to list
            if (parent == null)
                treeJOBJ.Nodes.Add(node);
            else
                parent.Nodes.Add(node);

            // add children
            if (jobj.Child != null)
            {
                UpdateJObjDisplay(jobj.Child, node);
            }

            // add siblings
            if (jobj.Next != null)
            {
                UpdateJObjDisplay(jobj.Next, parent);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeJOBJ_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is HSD_JOBJ jobj)
                SelectJObj?.Invoke(_root.BreathFirstList.IndexOf(jobj), jobj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importFromINIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("Label INI (*.ini)|*.ini");

            if (f != null)
            {
                _jointMap.Load(f);
                RefreshTree();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToINIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.SaveFile("Label INI (*.ini)|*.ini");

            if (f != null)
            {
                _jointMap.Save(f, _root);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void makeParticleJointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeJOBJ.SelectedNode != null && treeJOBJ.SelectedNode.Tag is HSD_JOBJ jobj)
            {
                if (MessageBox.Show(
                    "Are you sure you want to make this joint a particle joint?\n" +
                    "This will remove all objects on this joint",
                    "Make Particle Joint",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    jobj.ParticleJoint = new HSD_ParticleJoint();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void replaceBonesFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

            if (f != null)
            {
                var file = new HSDRawFile(f);
                if (file.Roots.Count > 0 && file.Roots[0].Data is HSD_JOBJ jobj)
                {
                    LiveJObj oldlist = new LiveJObj(_root);
                    LiveJObj newlist = new LiveJObj(jobj);

                    if (newlist.JointCount == oldlist.JointCount)
                    {
                        for (int i = 0; i < newlist.JointCount; i++)
                        {
                            var old = oldlist.GetJObjAtIndex(i).Desc;
                            var n = newlist.GetJObjAtIndex(i).Desc;

                            old.TX = n.TX; old.TY = n.TY; old.TZ = n.TZ;
                            old.RX = n.RX; old.RY = n.RY; old.RZ = n.RZ;
                            old.SX = n.SX; old.SY = n.SY; old.SZ = n.SZ;

                            if (old.InverseWorldTransform != null)
                            {
                                if (n.InverseWorldTransform == null)
                                    old.InverseWorldTransform = newlist.GetJObjAtIndex(i).WorldTransform.Inverted().ToHsdMatrix();
                                else
                                    old.InverseWorldTransform = n.InverseWorldTransform;
                            }
                        }

                        MessageBox.Show("Skeleton Sucessfully Replaced");
                    }
                }
            }
        }
    }
}