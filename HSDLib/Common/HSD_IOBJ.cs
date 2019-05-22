using HSDLib.GX;
using HSDLib.Helpers;

namespace HSDLib.Common
{
    public class HSD_IOBJ : IHSDNode
    {
        public ushort Width { get; set; }

        public ushort Height { get; set; }

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
