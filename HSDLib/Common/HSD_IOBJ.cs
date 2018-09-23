using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSDLib.GX;
using HSDLib.Helpers;

namespace HSDLib.Common
{
    public class HSD_IOBJ : IHSDNode
    {
        [FieldData(typeof(ushort))]
        public ushort Width { get; set; }
        [FieldData(typeof(ushort))]
        public ushort Height { get; set; }
        [FieldData(typeof(GXTexFmt))]
        public GXTexFmt Format { get; set; }

        public byte[] Data;

        public override void Open(HSDReader Reader)
        {
            uint DataOffset = Reader.ReadUInt32();
            base.Open(Reader);
            Data = Reader.ReadBuffer(DataOffset, TPL.TextureByteSize((TPL_TextureFormat)Format, Width, Height));
        }

        public override void Save(HSDWriter Writer)
        {
            if (Data != null)
                Writer.WriteTexture(Data);

            Writer.AddObject(this);
            Writer.WritePointer(Data);
            Writer.Write(Width);
            Writer.Write(Height);
            Writer.Write((uint)Format);
        }
    }
}
