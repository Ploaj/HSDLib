using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;

namespace MeleeLib.DAT
{
    public class DatTEV : DatNode
    {

        public int Flags1, Flags2;
        public byte A1, B1, C1, D1;
        public byte[] TEV = new byte[16];

        public void Deserialize(DATReader d, DATRoot Root)
        {
            d.Int();//Strin fofset
            Flags1 = d.Int();
            Flags2 = d.Int();
            A1 = d.Byte();
            B1 = d.Byte();
            C1 = d.Byte();
            D1 = d.Byte();
            for (int i = 0; i < 16; i++)
                TEV[i] = d.Byte();// if (d.Byte() != 0) Console.WriteLine("What is TEV?");
        }

        public override void Serialize(DATWriter Node)
        {
            Node.AddObject(this);
            Node.Int(0);
            Node.Int(Flags1);
            Node.Int(Flags2);
            Node.Byte(A1);
            Node.Byte(B1);
            Node.Byte(C1);
            Node.Byte(D1);
            for (int i = 0; i < 16; i++)
                Node.Byte(TEV[i]);
        }

    }
}
