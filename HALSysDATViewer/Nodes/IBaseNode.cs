using System;
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
            bs.Add(node);
            Grid.AutoSize = true;
            Grid.AutoGenerateColumns = false;
            Grid.DataSource = bs;
            Grid.Columns.Clear();
            //Grid.Rows.Clear();

            foreach (var prop in node.GetType().GetProperties())
            {
                var attrs = (FieldData[])prop.GetCustomAttributes(typeof(FieldData), false);
                foreach (var attr in attrs)
                {
                    if(!prop.Name.Contains("Offset") && 
                        attr.Viewable &&
                        (prop.PropertyType == typeof(byte) ||
                        prop.PropertyType == typeof(ushort) ||
                        prop.PropertyType == typeof(short) ||
                        prop.PropertyType == typeof(uint) ||
                        prop.PropertyType == typeof(int) ||
                        prop.PropertyType == typeof(float) ||
                        prop.PropertyType.IsEnum))
                    {
                        DataGridViewColumn row = new DataGridViewTextBoxColumn();
                        row.DataPropertyName = prop.Name;
                        row.Name = prop.Name;
                        Grid.Columns.Add(row);
                    }
                }
            }
        }
    }

    public class BindInteger
    {
        public String Name { get; set; }
        public int Value { get; set; }
    }
}
