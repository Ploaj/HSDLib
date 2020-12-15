using HSDRaw.Common;
using HSDRawViewer.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static HSDRawViewer.GUI.Controls.GraphEditor;

namespace HSDRawViewer.GUI.Extra
{
    public partial class PopoutJointAnimationEditor : Form
    {
        public bool CloseOnExit { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        private class JointNode : TreeNode
        {
            public HSD_JOBJ JOBJ;

            public AnimNode AnimNode;
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

            graphEditor1.OnTrackListUpdate += (s, a) =>
            {
                if (jointTree.SelectedNode is JointNode node)
                    node.AnimNode.Tracks = graphEditor1.TrackPlayers.ToList();
            };

            TopMost = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetJoint(HSD_JOBJ jobj, JointAnimManager animManager)
        {
            jointTree.BeginUpdate();
            jointTree.Nodes.Clear();

            var jobjs = jobj.BreathFirstList;

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
            if(jointTree.SelectedNode is JointNode node)
            {
                graphEditor1.ClearTracks();
                graphEditor1.LoadTracks(AnimType.Joint, node.AnimNode.Tracks);
            }
        }
    }
}
