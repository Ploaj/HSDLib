using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;

namespace HSDLib
{
    public enum WriterWriteMode
    {
        NORMAL,
        BUFFER, 
        TEXTURE
    }

    /// <summary>
    /// Class to assist with reading/writing HSD files
    /// </summary>
    public class HSDWriter : BinaryWriter
    {
        private class RelocationPointer
        {
            public uint Offset;
            public object Object;
        }

        public bool BigEndian { get; set; }
        private List<RelocationPointer> Pointers = new List<RelocationPointer>();
        private Dictionary<object, uint> ObjectOffsets = new Dictionary<object, uint>();
        private Dictionary<object, uint> ObjectOffsetsHOLD;
        private Dictionary<object, uint> BufferOffsets = new Dictionary<object, uint>();
        private HashSet<string> Writtenbuffers = new HashSet<string>();
        public WriterWriteMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
                if(_mode != WriterWriteMode.NORMAL)
                {
                    ObjectOffsetsHOLD = ObjectOffsets;
                    ObjectOffsets = new Dictionary<object, uint>();
                }
                else
                {
                    if (ObjectOffsetsHOLD != null)
                        ObjectOffsets = ObjectOffsetsHOLD;
                }
            }
        }
        public WriterWriteMode _mode = WriterWriteMode.NORMAL;


        public HSDWriter(Stream input) : base(input)
        {
            BigEndian = true;
        }

        public void WriteTexture(byte[] b)
        {
            if (Mode != WriterWriteMode.TEXTURE) return;
            string key = "";
            using (SHA256 sha256Hash = SHA256.Create())
            {
                key = Convert.ToBase64String(sha256Hash.ComputeHash(b));
            }
            if (Writtenbuffers.Contains(key)) return;
            Writtenbuffers.Add(key);
            Align(0x20);
            BufferOffsets.Add(key, (uint)BaseStream.Position);
            Write(b);
            Align(0x20);
        }

        public void WriteBuffer(byte[] b, int align = 0x20)
        {
            if (b == null)
                return;
            string key = "";
            using (SHA256 sha256Hash = SHA256.Create())
            {
                key = Convert.ToBase64String(sha256Hash.ComputeHash(b));
            }
            if (Mode != WriterWriteMode.BUFFER) return;
            if (Writtenbuffers.Contains(key)) return;
            Writtenbuffers.Add(key);
            Align(align);
            BufferOffsets.Add(key, (uint)BaseStream.Position);
            Write(b);
            Align(align);
        }
        
        public void WriteObject(IHSDNode o)
        {
            if (o == null)
                return;
            Align(4);
            if (!ObjectOffsets.ContainsKey(o))
                o.Save(this);
        }

        public void Align(int Block)
        {
            WriterWriteMode temp = Mode;
            Mode = WriterWriteMode.NORMAL;
            while (BaseStream.Position % Block > 0)
                Write((byte)0);
            Mode = temp;
        }

        public void AddObject(object o)
        {
            if (Mode != WriterWriteMode.NORMAL) return;
            ObjectOffsets.Add(o, (uint)BaseStream.Position);
        }

        public void WritePointer(object Object)
        {
            if (Mode != WriterWriteMode.NORMAL) return;
            if (Object == null)
            {
                Write(0);
                return;
            }
            RelocationPointer p = new RelocationPointer();
            p.Offset = (uint)BaseStream.Position;
            p.Object = Object;
            Pointers.Add(p);
            Write(0);
        }

        public byte[] Reverse(byte[] b)
        {
            if (Mode != WriterWriteMode.NORMAL)
                return new byte[0];
            if (BitConverter.IsLittleEndian && BigEndian)
                Array.Reverse(b);
            return b;
        }

        public override void Write(float value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(uint value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(int value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(ushort value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(short value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }
        
        public void ExtendedByte(int i)
        {
            if (i > 0xFF || (i & 0x80) > 0)
            {
                Write((byte)((i & 0x7F) | 0x80));
                Write((byte)(i >> 7));
            }
            else
            {
                Write((byte)i);
            }
        }

        public override void Write(byte value)
        {
            if (Mode != WriterWriteMode.NORMAL) return;
            base.Write(value);
        }

        public void WriteAt(int p, uint val)
        {
            uint temp = (uint)BaseStream.Position;
            BaseStream.Seek(p, SeekOrigin.Begin);
            Write(val);
            BaseStream.Seek(temp, SeekOrigin.Begin);
        }

        public void WritePointerAt(int p, object val)
        {
            uint temp = (uint)BaseStream.Position;
            BaseStream.Seek(p, SeekOrigin.Begin);
            WritePointer(val);
            BaseStream.Seek(temp, SeekOrigin.Begin);
        }

        public void WriteNullString(string s)
        {
            foreach(char c in s.ToCharArray())
            {
                Write(c);
            }
            Write((byte)0);
        }

        public int WriteRelocationTable(bool write = true)
        {
            foreach(RelocationPointer pointer in Pointers)
            {
                var obj = pointer.Object;
                if (obj is byte[] buffer)
                {
                    using (SHA256 sha256Hash = SHA256.Create())
                    {
                        obj = Convert.ToBase64String(sha256Hash.ComputeHash(buffer));
                    }
                }
                if (ObjectOffsets.ContainsKey(obj))
                    WriteAt((int)pointer.Offset, ObjectOffsets[obj] - 0x20);
                else
                if (BufferOffsets.ContainsKey(obj))
                    WriteAt((int)pointer.Offset, BufferOffsets[obj] - 0x20);
                if (write)
                    Write(pointer.Offset - 0x20);
            }
            int RelocCount = Pointers.Count;
            Pointers.Clear();
            return RelocCount;
        }
    }
}
