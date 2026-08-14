using HSDRawViewer.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrCategoryNode<T, K> : GrNode where K : GrNode
    {
        private readonly Dictionary<T, K> _nodes = new Dictionary<T, K>();

        protected ObservableList<T> list;

        public GrCategoryNode(string name, ObservableList<T> list)
        {
            Text = name;
            Checked = true;
            this.list = list;
            SetDataSource(list);
        }

        protected virtual K CreateChild(T m)
        {
            return System.Activator.CreateInstance<K>();
        }

        private bool TryDelete(GrNode node, T obj)
        {
            if (!list.Contains(obj))
                return false;

            if (MessageBox.Show(
                $"Are you sure you want to delete {node.Text}?\n\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                list.Remove(obj);
                return true;
            }
            return false;
        }

        public override bool HandleShortcut(Keys key, Keys modifier, GrNode node)
        {
            if (key == Keys.Delete && node.Tag is T obj)
            {
                TryDelete(node, obj);
                return true;
            }
            return false;
        }

        public override void BuildContextMenu(ContextMenuStrip menu, GrNode selectedNode)
        {
            if (selectedNode is K &&
                selectedNode.Tag is T obj)
            {
                menu.Items.Add("Delete", null, (s, e) => {
                    TryDelete(selectedNode, obj);
                });
            }
        }

        private void SetDataSource(ObservableList<T> list)
        {
            list.Added += (m) =>
            {
                var node = CreateChild(m);
                node.Checked = true;
                node.Tag = m;

                _nodes.Add(m, node);
                Nodes.Add(node);

                RefreshNodeNames();
            };
            list.Removed += (m) =>
            {
                if (_nodes.Remove(m, out var node))
                {
                    node.Remove();
                    RefreshNodeNames();
                }
            };

        }

        private void RefreshNodeNames()
        {
            TreeView tree = Nodes.Count > 0 ? Nodes[0].TreeView : null;

            tree?.BeginUpdate();

            try
            {
                int i = 0;
                foreach (TreeNode t in Nodes)
                    t.Text = $"{i++:D3}";
            }
            finally
            {
                tree?.EndUpdate();
            }
        }
    }
}
