using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.KAR
{
    public class KAR_GrData : IHSDNode
    {
        [FieldData(typeof(uint))]
        public uint Unk1 { get; set; }

        [FieldData(typeof(KAR_GrStageNode))]
        public KAR_GrStageNode StageNode { get; set; }
    }
}
