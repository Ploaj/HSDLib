using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.Animation
{
    public enum GXAnimDataFormat
    {
        Float = 0x00,
        Short = 0x20,
        UShort = 0x40,
        SByte = 0x60,
        Byte = 0x80
    }

    public enum InterpolationType
    {
        Step = 1,
        Linear = 2,
        HermiteValue = 3,
        Hermite = 4,
        HermiteCurve = 5,
        Constant = 6
    }

    public class HSD_Track : IHSDNode
    {
        [FieldData(typeof(ushort))]
        public ushort DataSize { get; set; }

        [FieldData(typeof(ushort))]
        public ushort Padding1 { get; set; }

        [FieldData(typeof(HSD_FOBJ), true, 0x8)]
        public HSD_FOBJ Track { get; set; }
        
        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);

            Console.WriteLine(Track.DataOffset.ToString("X") + " " + DataSize);
            Track.Data = Reader.ReadBuffer(Track.DataOffset, DataSize);
        }

        public override void Save(HSDWriter Writer)
        {
            base.Save(Writer);

            int start = (int)Writer.BaseStream.Position;
            Writer.WriteAt(start, (uint)Track.Data.Length << 16);
        }
    }
}
