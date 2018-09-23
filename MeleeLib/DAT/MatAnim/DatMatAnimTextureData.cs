using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;

namespace MeleeLib.DAT.MatAnim
{
    public class DatMatAnimTextureData : DatNode
    {
        public ushort Width, Height;
        public TPL_TextureFormat Format;
        public byte[] Data;

        public void Deserialize(DATReader d, DATRoot Root)
        {
            int Offset = d.Int();
            Width = (ushort)d.Short();
            Height = (ushort)d.Short();
            Format = (TPL_TextureFormat)d.Int();
            if (d.Int() != 0) throw new Exception("DatMatAnimTextureData Palette?");
            if (d.Int() != 0) throw new Exception("DatMatAnimTextureData Palette?");
            if (d.Int() != 0) throw new Exception("DatMatAnimTextureData Palette?");

            Data = d.getSection(Offset, TPL.textureByteSize(Format, Width, Height));
        }

        public override void Serialize(DATWriter Node)
        {
            Node.AddObject(this);
            Node.Object(Data);
            Node.Short((short)Width);
            Node.Short((short)Height);
            Node.Int((int)Format);
            Node.Int(0);
            Node.Int(0);
            Node.Int(0);
        }
    }
}
