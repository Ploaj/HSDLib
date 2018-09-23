using System.Runtime.InteropServices;

namespace HSDLib.Common
{
    public class HSD_Transforms : IHSDNode
    {
        [FieldData(typeof(float))]
        public float RX { get; set; }
        [FieldData(typeof(float))]
        public float RY { get; set; }
        [FieldData(typeof(float))]
        public float RZ { get; set; }
        [FieldData(typeof(float))]
        public float SX { get; set; }
        [FieldData(typeof(float))]
        public float SY { get; set; }
        [FieldData(typeof(float))]
        public float SZ { get; set; }
        [FieldData(typeof(float))]
        public float TX { get; set; }
        [FieldData(typeof(float))]
        public float TY { get; set; }
        [FieldData(typeof(float))]
        public float TZ { get; set; }
    }
}
