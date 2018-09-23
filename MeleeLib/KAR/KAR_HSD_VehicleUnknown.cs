using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.DAT;
using MeleeLib.IO;

namespace MeleeLib.KAR
{
    public class KAR_HSD_VehicleUnknown : DatNode
    {
        public int Unknown;
        public float Float1;
        public float Float2;
        public float Float3;
        public float Float4;

        public void Deserialize(DATReader r, DATRoot Root)
        {
            int ToData = r.Int();
            if (r.Int() != 1) throw new Exception("Error Reading Vehicle Unknown");

            r.Seek(ToData);
            Unknown = r.Int();
            Float1 = r.Float();
            Float2 = r.Float();
            Float3 = r.Float();
            Float4 = r.Float();
            if (r.Int() != 0) throw new Exception("Error Reading Vehicle Unknown");
        }

        public override void Serialize(DATWriter Node)
        {
            object data = new object();
            Node.AddObject(data);
            Node.Int(Unknown);
            Node.Float(Float1);
            Node.Float(Float2);
            Node.Float(Float3);
            Node.Float(Float4);
            Node.Int(0);

            Node.AddObject(this);
            Node.Object(data);
            Node.Int(1);
        }

    }
}
