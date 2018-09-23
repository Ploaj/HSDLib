using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.Animation
{
    public class HSD_FOBJ : IHSDNode
    {
        [FieldData(typeof(byte))]
        public byte AnimationType { get; set; }

        [FieldData(typeof(byte), Viewable: false)]
        public byte Flag1 { get; set; }

        [FieldData(typeof(byte), Viewable: false)]
        public byte Flag2 { get; set; }

        [FieldData(typeof(byte), Viewable: false)]
        public byte Padding2 { get; set; }

        [FieldData(typeof(uint))]
        public uint DataOffset { get; set; }

        public uint ValueScale
        {
            get
            {
                return (uint)Math.Pow(2, Flag1 & 0x1F);
            }
            set
            {
                Flag1 = (byte)((Flag1 & 0xF0) | (byte)Math.Log(value, 2));
            }
        }
        public uint TanScale
        {
            get
            {
                return (uint)Math.Pow(2, Flag2 & 0x1F);
            }
            set
            {
                Flag2 = (byte)((Flag2 & 0xF0) | (byte)Math.Log(value, 2));
            }
        }

        public GXAnimDataFormat ValueFormat
        {
            get
            {
                return (GXAnimDataFormat)(Flag1 & 0xF0);
            }
            set
            {
                Flag1 = (byte)((Flag1 & 0x1F) | (byte)value);
            }
        }
        public GXAnimDataFormat TanFormat
        {
            get
            {
                return (GXAnimDataFormat)(Flag2 & 0xF0);
            }
            set
            {
                Flag2 = (byte)((Flag2 & 0x1F) | (byte)value);
            }
        }

        public byte[] Data;

        public override void Save(HSDWriter Writer)
        {
            Writer.WriteBuffer(Data, 0x4);

            int start = (int)Writer.BaseStream.Position;
            base.Save(Writer);

            Writer.WritePointerAt(start + 4, Data);
        }
    }
}
