using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;
using MeleeLib.GCX;

namespace MeleeLib.DAT
{
    public class DatPalette : DatNode
    {
        public byte[] Data;
        public GXTlutFmt Format;
        public int GXTlut;
        public int ColorCount;

        public void Deserialize(DATReader d, DATRoot Root)
        {
            int Offset = d.Int();
            Format = (GXTlutFmt)d.Int();
            GXTlut = d.Int();
            ColorCount = d.Short();
            d.Skip(2); // padding
            Data = d.getSection(Offset, 2 * ColorCount);
        }

        public override void Serialize(DATWriter Node)
        {
            if (!Node.ContainsObject(Data))
            {
                Node.Align(32);
                Node.AddObject(Data);
                Node.Bytes(Data);
            }

            Node.AddObject(this);
            Node.Object(Data);
            Node.Int((int)Format);
            Node.Int(GXTlut);
            Node.Short((short)ColorCount);
            Node.Short(0);
        }
    }
}
