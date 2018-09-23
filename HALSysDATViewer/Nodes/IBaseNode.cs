using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HSDLib;

namespace HALSysDATViewer.Nodes
{
    public abstract class IBaseNode : TreeNode
    {
        public static ContextMenu ClassContextMenu = new ContextMenu();
        public bool Initialized = false;
        
        public virtual void Register()
        {

        }

        public virtual void ParseData(DataGridView Grid)
        {

        }

        public void ParseData(IHSDNode node, DataGridView Grid)
        {

            BindingSource bs = new BindingSource();

            foreach (var prop in node.GetType().GetProperties())
            {
                var attrs = (FieldData[])prop.GetCustomAttributes(typeof(FieldData), false);
                foreach (var attr in attrs)
                {
                    if(prop.PropertyType == typeof(uint))
                    {
                    }
                }
            }

            Grid.DataSource = bs;
        }
    }

    public class BindInteger
    {
        public String Name { get; set; }
        public int Value { get; set; }
    }
}
