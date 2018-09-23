using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.DAT;
using MeleeLib.IO;

namespace MeleeLib.KAR
{
    public class HSD_ParamArray : DatNode
    {
        public byte[] Data;

        public void Deserialize(DATReader r, DATRoot Root, int Size)
        {
            Data = new byte[Size];
            for (int i = 0; i < Size; i++)
                Data[i] = r.Byte();
        }

        public override void Serialize(DATWriter Node)
        {
            Node.AddObject(this);
            Node.Bytes(Data);
        }
    }
}
