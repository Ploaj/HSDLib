using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.Animation
{
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
