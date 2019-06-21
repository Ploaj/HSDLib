using HSDLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.KAR
{
    public class KAR_GrMainModel : IHSDNode
    {
        public HSD_JOBJ JOBJRoot { get; set; }

        public uint Unk1 { get; set; }

        public uint Unk2 { get; set; }

        public uint Unk3 { get; set; }

        public KAR_GrModel_Bounding ModelBounding { get; set; }
    }
}
