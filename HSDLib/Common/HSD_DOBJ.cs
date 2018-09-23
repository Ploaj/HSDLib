using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.Common
{
    public class HSD_DOBJ : IHSDList<HSD_DOBJ>
    {
        [FieldData(typeof(uint))]
        public uint NameOffset { get; set; }

        [FieldData(typeof(HSD_DOBJ))]
        public override HSD_DOBJ Next { get; set; }

        [FieldData(typeof(HSD_MOBJ))]
        public HSD_MOBJ MOBJ { get; set; }

        [FieldData(typeof(HSD_POBJ))]
        public HSD_POBJ POBJ { get; set; }
    }
}
