using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HSDRawViewer
{
    class RawHSD
    {
        public List<DataNode> RootNodes = new List<DataNode>();
        public Dictionary<int, byte[]> offsetToData = new Dictionary<int, byte[]>();
        public Dictionary<int, List<int>> offsetToOffsets = new Dictionary<int, List<int>>();
        public Dictionary<int, List<int>> offsetToInnerOffsets = new Dictionary<int, List<int>>();

        public void Open(string FilePath)
        {
            RootNodes.Clear();
            offsetToData.Clear();
            offsetToOffsets.Clear();
            offsetToInnerOffsets.Clear();
            using (HSDLib.HSDReader r = new HSDLib.HSDReader(new FileStream(FilePath, FileMode.Open)))
            {
                r.ReadInt32(); // dat size
                int relocOffset = r.ReadInt32() + 0x20;
                int relocCount = r.ReadInt32();
                int rootCount = r.ReadInt32();

                List<int> Offsets = new List<int>();
                HashSet<int> OffsetContain = new HashSet<int>();
                Offsets.Add(relocOffset);

                Dictionary<int, int> relocOffsets = new Dictionary<int, int>();

                r.BaseStream.Position = relocOffset;
                for (int i = 0; i < relocCount; i++)
                {
                    int offset = r.ReadInt32() + 0x20;

                    var temp = r.BaseStream.Position;
                    r.BaseStream.Position = offset;
                    var objectOff = r.ReadInt32() + 0x20;

                    relocOffsets.Add(offset, objectOff);

                    if (!OffsetContain.Contains(objectOff))
                    {
                        OffsetContain.Add(objectOff);
                        Offsets.Add(objectOff);
                    }

                    r.BaseStream.Position = temp;
                }

                List<int> rootOffsets = new List<int>();
                List<string> rootStrings = new List<string>();
                var stringStart = r.BaseStream.Position + rootCount * 8;
                for (int i = 0; i < rootCount; i++)
                {
                    rootOffsets.Add(r.ReadInt32() + 0x20);
                    rootStrings.Add(r.ReadString((int)stringStart + r.ReadInt32()));
                }
                Offsets.AddRange(rootOffsets);

                Offsets.Sort();

                for (int i = 0; i < Offsets.Count - 1; i++)
                {
                    r.BaseStream.Position = Offsets[i];
                    byte[] data = r.ReadBytes(Offsets[i + 1] - Offsets[i]);

                    if (!offsetToOffsets.ContainsKey(Offsets[i]))
                    {
                        var relocKets = relocOffsets.Keys.ToList().FindAll(e => e >= Offsets[i] && e < Offsets[i + 1]);
                        var list = new List<int>();
                        foreach (var k in relocKets)
                            list.Add(relocOffsets[k]);
                        offsetToOffsets.Add(Offsets[i], list);
                        offsetToInnerOffsets.Add(Offsets[i], relocKets);
                    }

                    if (!offsetToData.ContainsKey(Offsets[i]))
                        offsetToData.Add(Offsets[i], data);
                }

                for (int i = 0; i < rootOffsets.Count; i++)
                {
                    var name = rootOffsets[i].ToString("X8");
                    if (rootStrings[i].Contains("grData"))
                        name = "GrData";

                    RootNodes.Add(new DataNode(rootStrings[i], rootOffsets[i], offsetToData[rootOffsets[i]]));
                }
            }
        }

        public void Comparse(RawHSD toCompare)
        {
            for(int i = 0; i < Math.Min(RootNodes.Count, toCompare.RootNodes.Count); i++)
            {
                var root1 = RootNodes[i];
                var root2 = toCompare.RootNodes[i];

                CompareOffsets(toCompare, root1.Offset, root2.Offset, "");
            }
        }
        private void CompareOffsets(RawHSD toCompare, int offset1, int offset2, string depth)
        {
            if (offsetToData[offset1].Length != toCompare.offsetToData[offset2].Length)
            {
                Console.WriteLine($"{depth}: Size Mismatch sections at: {offset1.ToString("X")} {offset2.ToString("X")} {offsetToData[offset1].Length.ToString("X")} {toCompare.offsetToData[offset2].Length.ToString("X")}");
            }
            if (offsetToOffsets[offset1].Count != toCompare.offsetToOffsets[offset2].Count)
                Console.WriteLine($"{depth}: pointer count mismatch");

            // compare content
            var data1 = offsetToData[offset1];
            var data2 = toCompare.offsetToData[offset2];

            for (int j = 0; j < Math.Min(offsetToOffsets[offset1].Count, toCompare.offsetToOffsets[offset2].Count); j++)
            {
                int relativeOffset = (offsetToInnerOffsets[offset1][j] - offset1);
                int relativeOffset2 = (toCompare.offsetToInnerOffsets[offset2][j] - offset2);
                var nextoffset1 = offsetToOffsets[offset1][j];
                var nextoffset2 = toCompare.offsetToOffsets[offset2][j];
                CompareOffsets(toCompare, nextoffset1, nextoffset2, $"{depth}{relativeOffset.ToString("X")}->");

                data1[relativeOffset] = 0;
                data1[relativeOffset+1] = 0;
                data1[relativeOffset+2] = 0;
                data1[relativeOffset+3] = 0;
                data2[relativeOffset2] = 0;
                data2[relativeOffset2 + 1] = 0;
                data2[relativeOffset2 + 2] = 0;
                data2[relativeOffset2 + 3] = 0;
            }

            if (!data1.SequenceEqual(data2))
                Console.WriteLine($"{depth}: content mismatch");
        }
    }
}
