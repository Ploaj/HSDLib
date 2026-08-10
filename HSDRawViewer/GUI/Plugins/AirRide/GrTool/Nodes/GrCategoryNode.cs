using HSDRawViewer.Tools;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
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

        private void SetDataSource(ObservableList<T> list)
        {
            list.Added += (T m) =>
            {
                var node = CreateChild(m);
                node.Checked = true;
                node.Tag = m;

                node.OnDeleteNode += (n) =>
                {
                    if (MessageBox.Show(
                        $"Are you sure you want to delete {n.Text}?\n\nThis action cannot be undone.",
                        "Confirm Delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        list.Remove(m);
                    }
                };

                _nodes.Add(m, node);
                Nodes.Add(node);

                RefreshNodeNames();
            };
            list.Removed += (T m) =>
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
                    t.Text = $"{Text}_{i++:D3}";
            }
            finally
            {
                tree?.EndUpdate();
            }
        }
    }
}
