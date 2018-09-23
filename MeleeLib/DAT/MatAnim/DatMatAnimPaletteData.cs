using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;
using MeleeLib.GCX;

namespace MeleeLib.DAT.MatAnim
{
    public class DatMatAnimPaletteData : DatNode
    {
        public ushort Width, Height;
        public int ColorCount;
        public TPL_PaletteFormat Format;
        public byte[] Data;

        public void Deserialize(DATReader d, DATRoot Root)
        {
            int Offset = d.Int();
            Format = (TPL_PaletteFormat)d.Int();
            if (d.Int() != 0) throw new Exception("DatMatAnimTextureData Palette? 0x" + d.Pos().ToString("x"));
            ColorCount = d.Short();
            d.Skip(2);

            Data = d.getSection(Offset, ColorCount * 2);
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
            Node.Int(0);
            Node.Short((short)ColorCount);
            Node.Short(0);
        }
    }
}
