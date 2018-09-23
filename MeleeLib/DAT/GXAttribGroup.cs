using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.GCX;

namespace MeleeLib.DAT
{
    public class GXAttribGroup : DatNode
    {
        public List<GXAttr> Attributes = new List<GXAttr>();
        public int ID;

        public void Add(GXAttr a)
        {
            Attributes.Add(a);
        }
    }
}
