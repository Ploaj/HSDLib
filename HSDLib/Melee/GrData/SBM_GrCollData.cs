using System;
using System.Collections.Generic;

namespace HSDLib.Melee
{
    public class SBM_GrCollData : IHSDNode
    {
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public short Unknown3 { get; set; }
        public short Unknown4 { get; set; }
        public short Unknown5 { get; set; }
        public short Unknown6 { get; set; }
        public int Unknown7 { get; set; }
        public int Unknown8 { get; set; }

        public List<SBM_GrCollVertex> Vertices = new List<SBM_GrCollVertex>();

        public List<SBM_GrCollLink> Links = new List<SBM_GrCollLink>();

        public List<SBM_GrCollAreaTable> AreaTables = new List<SBM_GrCollAreaTable>();

        public override void Open(HSDReader Reader)
        {
            Vertices.Clear();
            Links.Clear();
            AreaTables.Clear();
            
            var vertexOffset = Reader.ReadUInt32();
            var vertexCount = Reader.ReadInt32();
            var linkOffset = Reader.ReadUInt32();
            var linkCount = Reader.ReadInt32();
            Unknown1 = Reader.ReadInt32();
            Unknown2 = Reader.ReadInt32();
            Unknown3 = Reader.ReadInt16();
            Unknown4 = Reader.ReadInt16();
            Unknown5 = Reader.ReadInt16();
            Unknown6 = Reader.ReadInt16();
            Unknown7 = Reader.ReadInt32();
            var polyOffset = Reader.ReadUInt32();
            var polyCount = Reader.ReadInt32();
            Unknown8 = Reader.ReadInt32();

            Reader.Seek(vertexOffset);
            for(int i = 0; i < vertexCount; i++)
            {
                var v = new SBM_GrCollVertex();
                v.Open(Reader);
                Vertices.Add(v);
            }
            Reader.Seek(linkOffset);
            for (int i = 0; i < linkCount; i++)
            {
                var v = new SBM_GrCollLink();
                v.Open(Reader);
                Links.Add(v);
            }
            Reader.Seek(polyOffset);
            for (int i = 0; i < polyCount; i++)
            {
                var v = new SBM_GrCollAreaTable();
                v.Open(Reader);
                AreaTables.Add(v);
            }
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(Vertices);
            foreach (var v in Vertices)
                v.Save(Writer);

            foreach (var v in Links)
                v.Save(Writer);

            foreach (var v in AreaTables)
                v.Save(Writer);
            
            Writer.AddObject(this);
            Writer.WritePointer(Vertices.Count == 0 ? null : Vertices);
            Writer.Write(Vertices.Count);
            Writer.WritePointer(Links.Count == 0 ? null : Links[0]);
            Writer.Write(Links.Count);
            Writer.Write(Unknown1);
            Writer.Write(Unknown2);
            Writer.Write(Unknown3);
            Writer.Write(Unknown4);
            Writer.Write(Unknown5);
            Writer.Write(Unknown6);
            Writer.Write(Unknown7);
            Writer.WritePointer(AreaTables.Count == 0 ? null : AreaTables[0]);
            Writer.Write(AreaTables.Count);
            Writer.Write(Unknown8);
        }

    }

    public class SBM_GrCollVertex : IHSDNode
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class SBM_GrCollLink : IHSDNode
    {
        public short VertexIndex1 { get; set; }
        public short VertexIndex2 { get; set; }
        public short Connector1 { get; set; }
        public short Connector2 { get; set; }
        public short idxVertFromLink { get; set; } = -1;
        public short idxVertToLink { get; set; } = -1;
        public short CollisionAngle { get; set; }
        public byte Flag { get; set; }
        public byte Material { get; set; }
    }

    public class SBM_GrCollAreaTable : IHSDNode
    {
        public ushort TopLinkIndex { get; set; }
        public ushort TopLinkCount { get; set; }
        public ushort BottomLinkIndex { get; set; }
        public ushort BottomLinkCount { get; set; }
        public ushort RightLinkIndex { get; set; }
        public ushort RightLinkCount { get; set; }
        public ushort LeftLinkIndex { get; set; }
        public ushort LeftLinkCount { get; set; }
        public int Padding { get; set; }
        public float XMin { get; set; }
        public float YMin { get; set; }
        public float XMax { get; set; }
        public float YMax { get; set; }
        public ushort VertexStart { get; set; }
        public ushort VertexCount { get; set; }
    }
}
