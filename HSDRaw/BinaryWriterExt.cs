using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HSDRaw
{
    public class BinaryWriterExt : BinaryWriter
    {
        public bool BigEndian { get; set; } = false;

        public long Length { get => BaseStream.Length; }

        public BinaryWriterExt(Stream stream) : base(stream)
        {
        }

        public void Align(int alignment, byte fill = 0)
        {
            if(BaseStream.Position % alignment > 0)
            {
                if(fill != 0)
                {
                    var data = new byte[alignment - (BaseStream.Position % alignment)];
                    for (int i = 0; i < data.Length; i++)
                        data[i] = fill;
                    Write(data);
                }
                else
                    Write(new byte[alignment - (BaseStream.Position % alignment)]);
            }
        }

        public override void Write(short s)
        {
            Write(Reverse(BitConverter.GetBytes(s)));
        }

        public override void Write(ushort s)
        {
            Write(Reverse(BitConverter.GetBytes(s)));
        }

        public override void Write(int s)
        {
            Write(Reverse(BitConverter.GetBytes(s)));
        }

        public override void Write(uint s)
        {
            Write(Reverse(BitConverter.GetBytes(s)));
        }

        public override void Write(float s)
        {
            Write(Reverse(BitConverter.GetBytes(s)));
        }
        
        public override void Write(string s)
        {
            var chars = s.ToCharArray();

            foreach (var v in chars)
                Write((byte)v);

            Write((byte)0);
        }

        public void Seek(uint Position)
        {
            BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        private byte[] Reverse(byte[] b)
        {
            if (BitConverter.IsLittleEndian && BigEndian)
                Array.Reverse(b);
            return b;
        }

        public void PrintPosition()
        {
            Console.WriteLine("Stream at 0x{0}", BaseStream.Position.ToString("X"));
        }

        public void WritePacked(int i)
        {
            while (i > 0x7F)
            {
                Write((byte)((i & 0x7F) | 0x80));
                i >>= 7;
            }
            Write((byte)(i & 0xFF));
        }

    }
}
