using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MeleeLib.IO
{
    public class DATReader
    {
        private byte[] Data;
        private byte[] DebugData;
        private int P;
        private bool DebugEnabled = false;

        private Dictionary<int, byte[]> SectionCache = new Dictionary<int, byte[]>();

        public bool BigEndian = true;

        public DATReader()
        {
            P = 0;
        }

        public DATReader(byte[] Data)
        {
            P = 0;
            this.Data = Data;
        }

        public void EnableDebug()
        {
            DebugEnabled = true;
            DebugData = new byte[Data.Length];
            Array.Copy(Data, DebugData, Data.Length);
        }

        public DATReader(String Fname)
        {
            P = 0;
            Data = File.ReadAllBytes(Fname);
        }

        public void WriteRead(string Fname)
        {
            File.WriteAllBytes(Fname, DebugData);
        }

        private void MarkViewed(int size, int offset = -1)
        {
            if(DebugEnabled)
            for(int i = 0; i < size; i++)
            {
                //if ((offset == -1 ? Pos() : offset) + i >= DebugData.Length) break;
                DebugData[(offset == -1 ? Pos() : offset) + i] = 0xFF;
            }
        }

        public int Pos()
        {
            return P;
        }

        public void Seek(int c)
        {
            P = c;
        }
        public void Skip(int c)
        {
            MarkViewed(c);
            P += c;
        }

        public byte Byte()
        {
            MarkViewed(1);
            return Data[P++];
        }
        
        public int ExtendedByte()
        {
            int type = Byte();
            int i = type;
            if ((i & 0x80) != 0) // max 16 bit I think
            {
                i = Byte();
                type = (type & 0x7F) | (i << 7);
            }
            return type;
        }

        public void ReadRelocationTable(int offset, int size)
        {
            for(int i =0; i < size; i++)
            {
                Seek(offset + 4 * i);
                Seek(Int() + 0x20);
                WriteInt(P, Int() + 0x20);
            }
        }

        public int Size()
        {
            if (Data == null)
                return 0;
            return Data.Length;
        }

        public String String(int P)
        {
            StringBuilder sb = new StringBuilder();

            while (Data[P] != 0x00)
                sb.Append((char)(Data[P++]));

            P++;

            return sb.ToString();
        }

        public String String()
        {
            StringBuilder sb = new StringBuilder();

            while (Data[P] != 0x00)
                sb.Append((char)(Data[P++]));

            P++;

            return sb.ToString();
        }

        public byte[] getSection(int offset, int size)
        {
            MarkViewed(size, offset);
            if (!SectionCache.Keys.Contains(offset))
            {
                byte[] by = new byte[size];
                Array.Copy(Data, offset, by, 0, size);
                SectionCache.Add(offset, by);
            }

            return SectionCache[offset];
        }

        public int Int()
        {
            MarkViewed(4);
            if (BigEndian)
                return ((Data[P++] & 0xFF) << 24) | ((Data[P++] & 0xFF) << 16) | ((Data[P++] & 0xFF) << 8) | (Data[P++] & 0xFF);
            else
                return ((Data[P++] & 0xFF)) | ((Data[P++] & 0xFF) << 8) | ((Data[P++] & 0xFF) << 16) | ((Data[P++] & 0xFF) << 24);
        }
        public int Three()
        {
            MarkViewed(3);
            if (BigEndian)
                return ((Data[P++] & 0xFF) << 16) | ((Data[P++] & 0xFF) << 8) | (Data[P++] & 0xFF);
            else
                return ((Data[P++] & 0xFF)) | ((Data[P++] & 0xFF) << 8) | ((Data[P++] & 0xFF) << 16);
        }
        public int Short()
        {
            if (P + 2 > Data.Length) return 0;
            MarkViewed(2);
            if (BigEndian)
                return ((Data[P++] & 0xFF) << 8) | (Data[P++] & 0xFF);
            else
                return ((Data[P++] & 0xFF)) | ((Data[P++] & 0xFF) << 8);
        }

        public float Float()
        {
            MarkViewed(4);
            float f = System.BitConverter.ToSingle(new byte[] { Data[P + 3], Data[P + 2], Data[P + 1], Data[P + 0] }, 0); ;
            if (!BigEndian)
                f = System.BitConverter.ToSingle(new byte[] { Data[P + 0], Data[P + 1], Data[P + 2], Data[P + 3] }, 0); ;
            P += 4;
            return f;
        }

        public void WriteInt(int Pos, int d)
        {
            Data[Pos + 0] = (byte)((d >> 24) & 0xFF);
            Data[Pos + 1] = (byte)((d >> 16) & 0xFF);
            Data[Pos + 2] = (byte)((d >> 8) & 0xFF);
            Data[Pos + 3] = (byte)((d) & 0xFF);
        }
    }


    public class DATWriter
    {
        private List<byte> Data = new List<byte>();

        public bool BigEndian = true;

        public struct ObjectOffset
        {
            public object obj;
            public int offset;
            public int startoff;
            public bool Relocate;
        }

        Dictionary<object, int> ObjectOffsets = new Dictionary<object, int>();
        List<ObjectOffset> OffsetsToFix = new List<ObjectOffset>();

        public DATWriter()
        {
        }
        
        public void Save(string Fname)
        {
            File.WriteAllBytes(Fname, Data.ToArray());
            Data.Clear();
        }

        public int Size()
        {
            return Data.Count;
        }


        public void ExtendedByte(int i)
        {
            //Console.WriteLine(i.ToString("x"));
            if(i > 0xFF || (i & 0x80) > 0)
            {
                Byte((byte)((i & 0x7F) | 0x80));
                Byte((byte)(i >> 7));
            }
            else
            {
                Byte((byte)i);
            }

            /*int type = Byte();
            int i = type;
            if ((i & 0x80) != 0) // max 16 bit I think
            {
                i = Byte();
                type = (type & 0x7F) | (i << 7);
            }*/
        }

        public void SByte(sbyte b)
        {
            Data.Add((byte)b);
        }

        public void Byte(byte b)
        {
            Data.Add(b);
        }

        public void Bytes(byte[] b)
        {
            Data.AddRange(b);
        }

        public void AddObject(object o)
        {
            if (o == null || ObjectOffsets.ContainsKey(o)) return;
            ObjectOffsets.Add(o, Data.Count);
        }

        public void Object(object o, int Off = 0, bool Reloc = true)
        {
            ObjectOffset ob = new ObjectOffset()
            {
                obj = o,
                offset = Data.Count,
                startoff = Off,
                Relocate = Reloc
            };
            Int(0);
            OffsetsToFix.Add(ob);
        }

        public void WriteData(DATWriter d)
        {
            for(int i = 0; i < d.OffsetsToFix.Count; i++)
            {
                OffsetsToFix.Add(new ObjectOffset() { obj = d.OffsetsToFix[i].obj, offset = d.OffsetsToFix[i].offset + Data.Count, startoff = d.OffsetsToFix[i].startoff});
            }
            foreach(object o in d.ObjectOffsets.Keys)
            {
                ObjectOffsets.Add(o, d.ObjectOffsets[o] + Data.Count);
            }
            Data.AddRange(d.Data);
        }

        public void String(String s)
        {
            foreach (char c in s.ToCharArray())
                Data.Add((byte)c);
            Data.Add(0x00);
        }

        public void Int(int i)
        {
            if (BigEndian)
                Data.AddRange(BitConverter.GetBytes(i).Reverse());
            else
                Data.AddRange(BitConverter.GetBytes(i));
        }
        public void Three(int i)
        {
            byte[] a = BitConverter.GetBytes(i);
            a = new byte[] { a[1], a[2], a[3]};
            if (BigEndian)
                Data.AddRange(a.Reverse());
            else
                Data.AddRange(a);
        }
        public void Short(short i)
        {
            if (BigEndian)
                Data.AddRange(BitConverter.GetBytes(i).Reverse());
            else
                Data.AddRange(BitConverter.GetBytes(i));
        }

        public void Float(float i)
        {
            if (BigEndian)
                Data.AddRange(BitConverter.GetBytes(i).Reverse());
            else
                Data.AddRange(BitConverter.GetBytes(i));
        }

        public void WriteIntAt(int p, int i)
        {
            //p += 4;
            byte[] d = BitConverter.GetBytes(i);
            //d.Reverse();
            Data[p] = d[3];
            Data[p+1] = d[2];
            Data[p+2] = d[1];
            Data[p+3] = d[0];
        }

        public void Align(int A)
        {
            while (Data.Count % A > 0)
            {
                Byte(0);
            }
        }

        public byte[] GetBytes()
        {
            return Data.ToArray();
        }

        public bool ContainsObject(object o)
        {
            return ObjectOffsets.ContainsKey(o);
        }

        public int WriteRelocationTable()
        {
            int c = 0;
            foreach(ObjectOffset o in OffsetsToFix)
            {
                if (o.Relocate)
                {
                    Int(o.offset - 0x20);
                }
                else
                    c--;
                if (!ContainsObject(o.obj) )
                {
                    Console.WriteLine(o.obj.GetType() + " " + o.obj.GetHashCode() + " " + o.offset.ToString("X"));
                    if (o.obj is byte[])
                        Console.WriteLine(((byte[])o.obj).Length);
                    continue;
                }

                WriteIntAt(o.offset, o.startoff + ObjectOffsets[o.obj] - (o.Relocate ? 0x20 : 0));
                c++;
            }
            OffsetsToFix.Clear();
            return c;
        }

        public void UpdateRelocationTable()
        {
            Console.WriteLine(OffsetsToFix.Count);
            foreach (ObjectOffset o in OffsetsToFix)
            {
                if (o.obj == null) continue;
                WriteIntAt(o.offset, o.startoff + ObjectOffsets[o.obj] - 0x20);
            }
        }
    }

}
