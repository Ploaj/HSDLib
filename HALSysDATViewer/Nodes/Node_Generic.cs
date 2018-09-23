using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HSDLib;

namespace HALSysDATViewer.Nodes
{
    public class Node_Generic : IBaseNode
    {
        public IHSDNode Node;

        public Node_Generic(IHSDNode Node)
        {
            this.Node = Node;
        }

        public void Open()
        {
            if (Node == null) return;
            Text = Node.GetType().Name.Replace("HSD_", "");
            foreach (var prop in Node.GetType().GetProperties().Reverse())
            {
                var attrs = (FieldData[])prop.GetCustomAttributes(typeof(FieldData), false);
                foreach (var attr in attrs)
                {
                    if (prop.GetValue(Node) != null)
                    {
                        if (prop.Name.Equals("Next") && attr.Type.IsSubclassOf(typeof(IHSDNode)))
                        {
                            Node_Generic Sibling = new Node_Generic((IHSDNode)prop.GetValue(Node));
                            Parent.Nodes.Add(Sibling);
                            Sibling.Open();
                        }
                        else
                        if (attr.Type.IsSubclassOf(typeof(IHSDNode)))
                        {
                            Node_Generic Child = new Node_Generic((IHSDNode)prop.GetValue(Node));
                            Nodes.Add(Child);
                            Child.Open();
                        }
                    }
                        
                }
            }
        }

        public override void ParseData(DataGridView Grid)
        {
            ParseData(Node, Grid);
        }
    }
}
