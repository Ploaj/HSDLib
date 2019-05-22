using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSDLib.Common;
using System.Windows.Forms;

namespace HALSysDATViewer.Nodes
{
    public class Node_JOBJ : IBaseNode
    {
        public static Node_JOBJ Instance = new Node_JOBJ();

        public HSD_JOBJ JOBJ;

        public Node_JOBJ()
        {
            Register();
        }

        public Node_JOBJ(HSD_JOBJ JOBJ, IBaseNode Parent) : base()
        {
            Parent.Nodes.Add(this);
            Text = "Joint Object_" + JOBJ.Flags.ToString("X");
            this.JOBJ = JOBJ;
            foreach(var child in JOBJ.Children)
            {
                new Node_JOBJ(child, this);

            }
            /*if (JOBJ.Child != null)
            {
                new Node_JOBJ(JOBJ.Child, this);
            }
            if(JOBJ.Next != null)
            {
                new Node_JOBJ(JOBJ.Next, Parent);
            }*/
        }

        public override void Register()
        {
            if (!Initialized)
            {
                Initialized = true;
            }
        }
        
        public override void ParseData(DataGridView Grid)
        {
            ParseData(JOBJ, Grid);
        }
    }
}
