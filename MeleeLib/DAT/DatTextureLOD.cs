using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.GCX;
using MeleeLib.IO;

namespace MeleeLib.DAT
{
    public class DatTextureLOD : DatNode
    {
        public GXTexFilter MinFilter;
        public float Bias;
        public bool BiasClamp;
        public bool EnableEdgeLOD;
        public GXAnisotropy Anisotropy;

        public void Deserialize(DATReader d, DATRoot Root)
        {
            MinFilter = (GXTexFilter)d.Int();
            Bias = d.Float();
            BiasClamp = d.Byte() != 0;
            EnableEdgeLOD = d.Byte() != 0;
            d.Skip(2);
            Anisotropy = (GXAnisotropy)d.Int();
        }

        public override void Serialize(DATWriter Node)
        {
            Node.AddObject(this);
            Node.Int((int)MinFilter);
            Node.Float(Bias);
            Node.Byte(BiasClamp ? (byte)1 : (byte)0);
            Node.Byte(EnableEdgeLOD ? (byte)1 : (byte)0);
            Node.Short(0);
            Node.Int((int)Anisotropy);
        }


    }
}
