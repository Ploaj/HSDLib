using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public abstract class GrNode : TreeNode
    {
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

        public virtual bool HandleShortcut(Keys key, Keys modifier, GrNode selected_node)
        {
            return false;
        }

        public virtual void BuildContextMenu(ContextMenuStrip menu, GrNode selected_node)
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
