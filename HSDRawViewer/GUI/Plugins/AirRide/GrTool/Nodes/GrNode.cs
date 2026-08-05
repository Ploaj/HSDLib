using OpenTK.Mathematics;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public abstract class GrNode : TreeNode
    {
        public abstract bool HasTransform { get; }

        public Matrix4 LocalTransform { get; set; }

        public Matrix4 GlobalTransform { get; set; }


        public delegate void DeleteNode(GrNode self);

        public DeleteNode OnDeleteNode;


        public bool Visible
        {
            get => Checked && (Parent is not GrNode node || node.Visible);
        }

        public virtual bool HandleShortcut(Keys key, Keys modifier)
        {
            return false;
        }

        public virtual void BuildContextMenu(ContextMenuStrip menu)
        {
        }

        public virtual void BuildToolStrip(ToolStrip strip)
        {
        }
    }
}
