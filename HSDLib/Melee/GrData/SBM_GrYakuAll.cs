using System;
using System.Collections.Generic;

namespace HSDLib.Melee
{
    public class SBM_GrYakuAll : IHSDNode
    {
        public List<SBM_YakuAllNode> Nodes { get; set; } = new List<SBM_YakuAllNode>();

        public override void Open(HSDReader Reader)
        {
            if (Reader.ReadInt32() != 0)
                throw new NotSupportedException("Yaku all not supported");

            var off = Reader.ReadUInt32();
            while (off != 0)
            {
                var temp = Reader.Position();
                Reader.Seek(off);
                SBM_YakuAllNode node = new SBM_YakuAllNode();
                node.Open(Reader);
                Nodes.Add(node);
                Reader.Seek(temp);
                off = Reader.ReadUInt32();
            }
        }

        public override void Save(HSDWriter Writer)
        {
            foreach (var v in Nodes)
            {
                v.Save(Writer);
            }

            Writer.AddObject(this);
            Writer.Write(0);
            foreach (var v in Nodes)
            {
                Writer.WritePointer(v);
            }
            Writer.Write(0);
        }
    }

    public class SBM_YakuAllNode : IHSDNode
    {
        public uint Flags { get; set; }
        public uint Flags2 { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public int Unknown4 { get; set; }
        public int Unknown5 { get; set; }
    }
}
