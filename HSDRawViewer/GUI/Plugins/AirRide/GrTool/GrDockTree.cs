using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool
{
    public partial class GrDockTree : DockContent
    {
        private GrStageNode StageNode { get; }

        private GrModelNode ModelNode { get; }

        private GrCategoryCollision CollisionNode { get; }

        private GrCategoryZone ZoneNode { get; }

        private GrRootPositionNode PositionNode { get; }

        private GrCategoryCourseSpline CourseSplineNode { get; }

        private GrCategoryConveyorSpline ConveyorSplineNode { get; }

        private GrNode Animations { get; }



        public delegate void SelectedNodeChanged(GrNode node);

        public SelectedNodeChanged OnSelectedNodeChanged;


        public TreeNode SelectedNode { get => treeView1.SelectedNode; }


        public GrDockTree(GrDataResource res)
        {
            InitializeComponent();

            treeView1.CheckBoxes = true;
            treeView1.HideSelection = false;

            treeView1.AfterSelect += (s, e) =>
            {
                OnSelectedNodeChanged?.Invoke(e.Node as GrNode);

                if (e.Node is GrNode node)
                {
                    //node.BuildToolStrip(toolstr);

                    menuStrip1.SuspendLayout();
                    menuStrip1.Items.Clear();
                    node.BuildToolStrip(menuStrip1);
                    //menuStrip1.Visible = (menuStrip1.Items.Count > 0);
                    menuStrip1.ResumeLayout();
                }
            };

            treeView1.BeginUpdate();

            StageNode = new GrStageNode();
            ModelNode = new GrModelNode();
            CollisionNode = new GrCategoryCollision("Collisions", res.Meshes);
            ZoneNode = new GrCategoryZone("Zones", res.Zones);
            PositionNode = new GrRootPositionNode(res);
            CourseSplineNode = new GrCategoryCourseSpline("Course Spline", res.CourseSpline.Splines, res.CourseSpline);
            ConveyorSplineNode = new GrCategoryConveyorSpline("Conveyor Spline", res.ConveyorSplines);

            Animations = new GrAnimationNode() { Text = "Animations", Checked = true };
            Animations.Nodes.Add(new GrCategoryAnimation("SuperJump", res.SuperJumpAnimations));
            Animations.Nodes.Add(new GrCategoryAnimation("Leap", res.LeapAnimations));
            Animations.Nodes.Add(new GrCategoryAnimation("Rail", res.RailAnimations));
            Animations.Nodes.Add(new GrCategoryAnimation("x0C", res.x0CAnimations));
            Animations.Nodes.Add(new GrCategoryAnimation("x10", res.x10Animations));
            Animations.Nodes.Add(new GrCategoryAnimation("Event Animations", res.EventAnimations));

            treeView1.Nodes.Add(StageNode);
            treeView1.Nodes.Add(ModelNode);
            treeView1.Nodes.Add(CollisionNode);
            treeView1.Nodes.Add(ZoneNode);
            treeView1.Nodes.Add(PositionNode);
            treeView1.Nodes.Add(Animations);
            treeView1.Nodes.Add(CourseSplineNode);
            treeView1.Nodes.Add(ConveyorSplineNode);

            treeView1.EndUpdate();
        }

        public void LoadMiscData(KAR_grData data)
        {
            StageNode.Tag = data.StageNode;

            foreach (TreeNode c1 in PositionNode.Nodes)
            {
                foreach (TreeNode c2 in c1.Nodes)
                    c2.Checked = false;

                if (c1.Nodes.Count > 0)
                {
                    c1.Nodes[0].Checked = true;
                }
            }

#if DEBUG
            treeView1.Nodes.Add(new GrPartitionNode(0, data.PartitionNode.Partition.Buckets[0], data.PartitionNode.Partition.Buckets, data.PartitionNode.Partition.ZoneIndices));
#endif
        }

        public void SaveMiscData(KAR_grData data)
        {

        }

        private (TreeNode Node, float Distance) TryPickNodes(
            TreeNodeCollection nodes,
            PickInformation pick,
            LiveJObj joint)
        {
            TreeNode bestNode = null;
            float bestDistance = float.PositiveInfinity;

            foreach (TreeNode node in nodes)
            {
                if (node is GrDrawNode drawNode &&
                    drawNode.TryPickNode(pick, joint, out float depth) &&
                    depth < bestDistance)
                {
                    bestNode = node;
                    bestDistance = depth;
                }

                var (childNode, childDistance) = TryPickNodes(node.Nodes, pick, joint);

                if (childDistance < bestDistance)
                {
                    bestNode = childNode;
                    bestDistance = childDistance;
                }
            }

            return (bestNode, bestDistance);
        }

        public object TryPickNode(GrSelectModeKind kind, PickInformation pick, LiveJObj joint)
        {
            switch (kind)
            {
                case GrSelectModeKind.Node:
                    {
                        var node = TryPickNodes(treeView1.Nodes, pick, joint);

                        if (node.Node != null)
                        {
                            treeView1.SelectedNode = node.Node;
                            node.Node.EnsureVisible();
                            treeView1.Focus();
                            return node.Node.Tag;
                        }
                    }
                    break;
                case GrSelectModeKind.Data:
                    {
                        if (treeView1.SelectedNode is GrDrawNode node)
                        {
                            return node.PickData(pick, joint);
                        }
                    }
                    break;
            }

            return null;
        }

        private void DrawNodes(TreeNodeCollection nodes, GrRenderResource resource, bool is_overlay, object selected_object)
        {
            foreach (TreeNode node in nodes)
            {
                if (node is GrDrawNode d)
                {
                    if (is_overlay)
                        d.DrawOverlay(resource, selected_object);
                    else
                        d.Draw(resource, selected_object);
                }

                if (node.Nodes.Count > 0)
                {
                    DrawNodes(node.Nodes, resource, is_overlay, selected_object);
                }
            }
        }

        public void Draw(GrRenderResource resource, object selected_object)
        {
            DrawNodes(treeView1.Nodes, resource, false, selected_object);
        }

        public void DrawOverlay(GrRenderResource resource, object selected_object)
        {
            DrawNodes(treeView1.Nodes, resource, true, selected_object);
        }

        public void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.H)
            {
                if (treeView1.SelectedNode != null)
                {
                    treeView1.SelectedNode.Checked = !treeView1.SelectedNode.Checked;
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            else
            {
                if (treeView1.SelectedNode is GrNode node)
                {
                    if (node.HandleShortcut(e.KeyCode, e.Modifiers))
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private readonly ContextMenuStrip _contextMenu = new();

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            treeView1.SelectedNode = e.Node;

            _contextMenu.Items.Clear();

            if (e.Node is GrNode node)
                node.BuildContextMenu(_contextMenu);

            treeView1.ContextMenuStrip = _contextMenu;
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);
            OnSelectedNodeChanged.Invoke(treeView1.SelectedNode as GrNode);
        }
    }
}
