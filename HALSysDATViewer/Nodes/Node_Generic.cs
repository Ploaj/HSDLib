using System.Linq;
using System.Windows.Forms;
using HSDLib;
using HSDLib.Common;
using System;

namespace HALSysDATViewer.Nodes
{
    public class Node_Generic : IBaseNode
    {
        public IHSDNode Node;

        public Node_Generic(IHSDNode Node)
        {
            this.Node = Node;
            if (Node == null)
                return;

            if(Node.GetType() == typeof(HSD_JOBJ))
            {
                ImageKey = "jobj";
                SelectedImageKey = "jobj";
            }
        }
        
        public void Refresh()
        {
            Nodes.Clear();
            Open();
        }

        public void Open()
        {
            if (Node == null) return;
            Text = Node.GetType().Name.Replace("HSD_", "").Replace("KAR_", "");
            foreach (var prop in Node.Children)
            {
                if (prop != null)
                {
                    Node_Generic Child = new Node_Generic(prop);
                    Nodes.Add(Child);
                    Child.Open();
                }
            }
        }

        public override void ParseData(DataGridView Grid)
        {
            ParseData(Node, Grid);
        }
    }
}
