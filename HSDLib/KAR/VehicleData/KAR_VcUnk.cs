using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.KAR
{
    public class KAR_VcUnk : IHSDNode
    {
        [FieldData(typeof(KAR_VehicleUnk_1))]
        public KAR_VehicleUnk_1 Unk_1 { get; set; }

        [FieldData(typeof(int))]
        public int HasUnk_1 { get; set; }
    }

    public class KAR_VehicleUnk_1 : IHSDNode
    {
        [FieldData(typeof(int))]
        public int Unknown1 { get; set; }

        [FieldData(typeof(float))]
        public float Unknown2 { get; set; }

        [FieldData(typeof(float))]
        public float Unknown3 { get; set; }

        [FieldData(typeof(float))]
        public float Unknown4 { get; set; }

        [FieldData(typeof(float))]
        public float Unknown5 { get; set; }

        [FieldData(typeof(int))]
        public int Unknown6 { get; set; }

    }
}
