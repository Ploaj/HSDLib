using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.Common
{
    public class HSD_Matrix4x3 : IHSDNode
    {
        [FieldData(typeof(float))]
        public float M00 { get; set; }
        [FieldData(typeof(float))]
        public float M01 { get; set; }
        [FieldData(typeof(float))]
        public float M02 { get; set; }
        [FieldData(typeof(float))]
        public float M03 { get; set; }
        [FieldData(typeof(float))]
        public float M10 { get; set; }
        [FieldData(typeof(float))]
        public float M11 { get; set; }
        [FieldData(typeof(float))]
        public float M12 { get; set; }
        [FieldData(typeof(float))]
        public float M13 { get; set; }
        [FieldData(typeof(float))]
        public float M20 { get; set; }
        [FieldData(typeof(float))]
        public float M21 { get; set; }
        [FieldData(typeof(float))]
        public float M22 { get; set; }
        [FieldData(typeof(float))]
        public float M23 { get; set; }
    }
}
