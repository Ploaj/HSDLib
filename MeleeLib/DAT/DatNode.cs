using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;

namespace MeleeLib.DAT
{
    public class DatNode
    {
        public string Text;
        
        public virtual void Serialize(DATWriter Node)
        {

        }

        public override string ToString()
        {
            return Text;
        }

        public virtual List<DATRoot> GetRoots()
        {
            return new List<DATRoot>();
        }
    }
}
