using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;

namespace MeleeLib.DAT
{
    public class Matrix4x4 : DatNode
    {
        public static Matrix4x4 Identity = new Matrix4x4();
        public float M00 = 1, M01, M02, M03;
        public float M10, M11 = 1, M12, M13;
        public float M20, M21, M22 = 1, M23;
        public float M30, M31, M32, M33 = 1;

        public void Deserialize(DATReader d, DATRoot root)
        {
            M00 = d.Float(); M01 = d.Float(); M02 = d.Float(); M03 = d.Float();
            M10 = d.Float(); M11 = d.Float(); M12 = d.Float(); M13 = d.Float();
            M20 = d.Float(); M21 = d.Float(); M22 = d.Float(); M23 = d.Float();
        }

        public override void Serialize(DATWriter Node)
        {
            Node.AddObject(this);
            Node.Float(M00); Node.Float(M01); Node.Float(M02); Node.Float(M03);
            Node.Float(M10); Node.Float(M11); Node.Float(M12); Node.Float(M13);
            Node.Float(M20); Node.Float(M21); Node.Float(M22); Node.Float(M23);
        }
    }
}
