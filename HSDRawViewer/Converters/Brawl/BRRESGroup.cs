using HSDRaw;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.Converters.Brawl
{
    public class BRRESGroup
    {
        public string Name;
        public int DataP;

        private static void Parse(BinaryReaderExt e, ushort index, uint start, Dictionary<int, BRRESGroup> group)
        {
            if (group.ContainsKey(index))
                return;

            e.Seek(start + 0x08 + index * 0x10U);
            ushort id = e.ReadUInt16();
            ushort flag = e.ReadUInt16();
            ushort left = e.ReadUInt16();
            ushort right = e.ReadUInt16();
            string name = e.ReadString((int)start + e.ReadInt32(), -1);
            int offset = (int)start + e.ReadInt32();

            group.Add(index, new BRRESGroup()
            {
                Name = name,
                DataP = offset,
            });

            Parse(e, left, start, group);
            Parse(e, right, start, group);
        }

        public static BRRESGroup[] ReadGroups(BinaryReaderExt e)
        {
            uint start = e.Position;
            e.Skip(4); // data length
            uint count = e.ReadUInt32();
            Dictionary<int, BRRESGroup> group = new Dictionary<int, BRRESGroup>();
            Parse(e, 0, start, group);
            return group.Values.ToArray();
        }
    }

}
