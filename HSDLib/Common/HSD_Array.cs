using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.Common
{
    public class HSD_Array<T> : IHSDNode where T : IHSDNode
    {
        public T[] Elements;
        public int SetSize = -1;

        public override void Open(HSDReader Reader)
        {
            uint Offset = Reader.ReadUInt32();
            List<uint> Offsets = new List<uint>();
            while (Offset != 0)
            {
                if (SetSize != -1 && Offsets.Count >= SetSize)
                    break;
                Offsets.Add(Offset);
                Offset = Reader.ReadUInt32();
            }
            int i = 0;
            Elements = new T[Offsets.Count];
            foreach(uint Off in Offsets)
            {
                Elements[i++] = Reader.ReadObject<T>(Off);
            }
        }

        public override void Save(HSDWriter Writer)
        {
            if (Elements == null) return;
            foreach(IHSDNode node in Elements)
            {
                Writer.WriteObject(node);
            }

            Writer.AddObject(this);
            foreach (IHSDNode node in Elements)
            {
                Writer.WritePointer(node);
            }
            Writer.Write(0);
        }
    }
}
