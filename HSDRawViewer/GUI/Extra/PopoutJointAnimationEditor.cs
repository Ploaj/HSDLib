using HSDRaw.Common;
using HSDRaw.Tools;
using HSDRawViewer.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static HSDRawViewer.GUI.Controls.GraphEditor;
using HSDRawViewer.GUI.Dialog;

namespace HSDRawViewer.GUI.Extra
{
    public partial class PopoutJointAnimationEditor : Form
    {
        /// <summary>
        /// settings class for animation key compression
        /// </summary>
        private class OptimizeSettings
        {
            //public bool BakeAnimation = true;
            public float ErrorMargin { get; set; } = 0.001f;
        }

        private static OptimizeSettings _settings = new OptimizeSettings();

        public bool CloseOnExit { get; set; } = true;

        public bool MadeChanges { get; internal set; } = false;

        /// <summary>
        /// 
        /// </summary>
        private class JointNode : TreeNode
        {
            public HSD_JOBJ JOBJ;

            public AnimNode AnimNode;

            public void Optimize(OptimizeSettings settings, bool optimizeChildren = true)
            {
                if (AnimNode == null || JOBJ == null)
                    return;

                AnimationKeyCompressor.OptimizeJointTracks(JOBJ, ref AnimNode.Tracks, settings.ErrorMargin);

                if (optimizeChildren)
                    foreach (JointNode child in Nodes)
                        child.Optimize(settings, optimizeChildren);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="animManager"></param>
        public PopoutJointAnimationEditor(bool closeOnExit)
        {
            InitializeComponent();

            CloseOnExit = closeOnExit;

            FormClosing += (sender, args) =>
            {
                if (!CloseOnExit)
                {
                    args.Cancel = true;
                    Visible = false;
                }
            };

            graphEditor1.TrackListUpdated += (s, a) =>
            {
                if (jointTree.SelectedNode is JointNode node)
                    node.AnimNode.Tracks = graphEditor1.TrackPlayers.ToList();
            };

            graphEditor1.TrackEdited += (s, a) =>
            {
                MadeChanges = true;
            };

            TopMost = true;

            CenterToScreen();
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetJoint(HSD_JOBJ jobj, JointAnimManager animManager)
        {
            MadeChanges = false;
            graphEditor1.ClearTracks();
            jointTree.BeginUpdate();
            jointTree.Nodes.Clear();

            var jobjs = jobj.ToList;

            Dictionary<HSD_JOBJ, JointNode> childToParent = new Dictionary<HSD_JOBJ, JointNode>();
            
            for(int i = 0; i < Math.Min(animManager.NodeCount, jobjs.Count); i++)
            {
                var node = new JointNode() { JOBJ = jobjs[i], AnimNode = animManager.Nodes[i], Text = $"Joint_{i}" };
                
                foreach (var c in jobjs[i].Children)
                    childToParent.Add(c, node);

                if(childToParent.ContainsKey(jobjs[i]))
                {
                    childToParent[jobjs[i]].Nodes.Add(node);
                }
                else
                {
                    jointTree.Nodes.Add(node);
                }
            }

            jointTree.ExpandAll();
            jointTree.EndUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void jointTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            graphEditor1.ClearTracks();

            if (jointTree.SelectedNode is JointNode node)
                graphEditor1.LoadTracks(AnimType.Joint, node.AnimNode.Tracks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void optimizeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (PropertyDialog d = new PropertyDialog("Animation Optimize Settings", _settings))
            {
                if (d.ShowDialog() == DialogResult.OK)
                    foreach (JointNode node in jointTree.Nodes)
                    {
                        node.Optimize(_settings);
                    }
            }
        }
    }
}
