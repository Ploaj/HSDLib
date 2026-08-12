using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public abstract class GrNode : TreeNode
    {
        public delegate void DeleteNode(GrNode self);

        public DeleteNode OnDeleteNode;


        public bool Visible
        {
            get => Checked && (Parent is not GrNode node || node.Visible);
        }

        public virtual bool IsParentSelected()
        {
            if (Parent == null) return false;

            if (Parent.IsSelected)
                return true;

            if (Parent is GrNode node)
                return node.IsParentSelected();

            return false;
        }

        public virtual void OnSelect(GrRenderResource _render, GrDataResource _data)
        {

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

        public virtual void OnTagPropertyUpdate(PropertyValueChangedEventArgs args)
        {
        }
    }
}
