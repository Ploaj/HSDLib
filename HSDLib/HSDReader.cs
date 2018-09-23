using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using HSDLib.Common;

namespace HSDLib
{
    public class FieldData : Attribute
    {
        public Type Type{ get; private set; }
        public bool InLine { get; private set; }
        public int Size { get; private set; }

        public FieldData(Type Type)
        {
            this.Type = Type;
            InLine = false;
            Size = 4;
        }

        public FieldData(Type Type, bool InLine, int Size)
        {
            this.Type = Type;
            this.InLine = InLine;
            this.Size = Size;
        }
    }

    /// <summary>
    /// Assists with parsing HSD files
    /// </summary>
    public class HSDReader : BinaryReader
    {
        private List<uint> RelocationTable = new List<uint>();

        public HSDReader(Stream input) : base(input)
        {

        }

        public void ReadRelocationTable(int Size)
        {
            RelocationTable = new List<uint>(Size);
            for(int i = 0; i < Size; i++)
            {
                RelocationTable.Add(ReadUInt32() + 0x20);
            }
        }

        public T ReadType<T>(object Struct)
        {
            foreach (var field in typeof(T).GetFields(BindingFlags.Instance |
                                                 BindingFlags.NonPublic |
                                                 BindingFlags.Public))
            {
                if (field.FieldType == typeof(float))
                    field.SetValue(Struct, ReadSingle());else
                if (field.FieldType == typeof(UInt32))
                    field.SetValue(Struct, ReadUInt32());else
                if (field.FieldType == typeof(Int32))
                    field.SetValue(Struct, ReadInt32());
                else
                {
                    field.SetValue(Struct, ReadByte());
                }
            }
            return (T)Struct;
        }

        public byte[] Reverse(byte[] b)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);
            return b;
        }

        public override Int16 ReadInt16()
        {
            return BitConverter.ToInt16(Reverse(base.ReadBytes(2)), 0);
        }

        public override UInt16 ReadUInt16()
        {
            return BitConverter.ToUInt16(Reverse(base.ReadBytes(2)), 0);
        }

        public override Int32 ReadInt32()
        {
            if (RelocationTable.Contains((uint)BaseStream.Position))
                return BitConverter.ToInt32(Reverse(base.ReadBytes(4)), 0) + 0x20;
            else
                return BitConverter.ToInt32(Reverse(base.ReadBytes(4)), 0);
        }

        public override UInt32 ReadUInt32()
        {
            if (RelocationTable.Contains((uint)BaseStream.Position))
                return BitConverter.ToUInt32(Reverse(base.ReadBytes(4)), 0) + 0x20;
            else
                return BitConverter.ToUInt32(Reverse(base.ReadBytes(4)), 0);
        }

        public override float ReadSingle()
        {
            return BitConverter.ToSingle(Reverse(base.ReadBytes(4)), 0);
        }

        public void Seek(uint Position)
        {
            BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        public void PrintPosition()
        {
            Console.WriteLine("Stream at 0x{0}", BaseStream.Position.ToString("X"));
        }

        public override string ReadString()
        {
            string str = "";
            char ch;
            while ((ch = ReadChar()) != 0)
                str = str + ch;
            return str;
        }

        public uint Position()
        {
            return (uint)BaseStream.Position;
        }

        private Dictionary<uint, IHSDNode> NodeCache = new Dictionary<uint, IHSDNode>();
        public T ReadObject<T>(uint Offset) where T : IHSDNode
        {
            T Object = Activator.CreateInstance<T>();
            return ReadObject(Offset, Object);
        }

        public T ReadObject<T>(uint Offset, T Type) where T : IHSDNode
        {
            T Object = Activator.CreateInstance<T>();

            if (Offset <= 0)
                return null;

            if (NodeCache.ContainsKey(Offset))
                return (T)NodeCache[Offset];

            Seek(Offset);
            Object.Open(this);
            NodeCache.Add(Offset, Object);
            return Object;
        }

        private Dictionary<uint, byte[]> BufferCache = new Dictionary<uint, byte[]>();
        public byte[] ReadBuffer(uint Offset, int Size)
        {
            if (Offset <= 0)
                return null;

            if (BufferCache.ContainsKey(Offset))
                return BufferCache[Offset];

            Seek(Offset);
            byte[] Buffer = new byte[Size];
            for(int i = 0; i < Size; i++)
            {
                Buffer[i] = ReadByte();
            }

            BufferCache.Add(Offset, Buffer);
            return Buffer;
        }

        public override void Close()
        {
            // Grab Vertex Buffers Here
            
            // Gather all data offsets including the vertex buffer ones and texture buffers
            List<uint> Offsets = NodeCache.Keys.ToList();
            for(int i = 0; i < NodeCache.Keys.Count; i++)
            {
                IHSDNode ob = NodeCache[Offsets[i]];
                if (ob is HSD_AttributeGroup)
                {
                    foreach(HSDLib.GX.GXVertexBuffer a in ((HSD_AttributeGroup)ob).Attributes)
                    {
                        //Console.WriteLine(a.Offset.ToString("X"));
                        if(!Offsets.Contains(a.Offset))
                            Offsets.Add(a.Offset);
                    }
                }
            }
            Offsets.AddRange(BufferCache.Keys.ToList());

            // sort them from lowest to highest
            Offsets.Sort();

            // get the minimum next offset for each buffer
            foreach (uint off in NodeCache.Keys)
            {
                IHSDNode ob = NodeCache[off];
                if (ob is HSD_AttributeGroup)
                {
                    foreach (HSDLib.GX.GXVertexBuffer a in ((HSD_AttributeGroup)ob).Attributes)
                    {
                        if (a.AttributeType != GX.GXAttribType.GX_DIRECT)
                            a.DataBuffer = ReadBuffer(a.Offset, (int)(Offsets[BinarySearch(Offsets, a.Offset)] - a.Offset));
                    }
                }
            }

            // Close the stream
            base.Close();
        }

        public int BinarySearch(List<uint> List, uint Offset)
        {
            int low = 0;
            int hi = List.Count-1;

            while (true)
            {
                if (hi - low <= 1)
                    return hi;

                int start = (hi + low) / 2;
                //Console.WriteLine(low + " " + hi + " " + start);

                if (Offset < List[start])
                {
                    hi = start;
                }
                else
                {
                    low = start;
                }
            }
        }
    }
}
