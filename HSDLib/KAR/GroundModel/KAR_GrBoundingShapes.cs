using System.Collections.Generic;
using System.ComponentModel;

namespace HSDLib.KAR
{
    public class KAR_GrViewRegion : IHSDNode
    {
        [Browsable(false)]
        public uint Offset { get; set; }

        [Browsable(false)]
        public ushort Size { get; set; }

        [Browsable(false)]
        public ushort Padding { get; set; }

        public float MinX { get; set; }

        public float MinY { get; set; }

        public float MinZ { get; set; }

        public float MaxX { get; set; }

        public float MaxY { get; set; }

        public float MaxZ { get; set; }

        public List<ushort> DOBJIndices = new List<ushort>();

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            uint BaseOffset = Reader.Position();

            DOBJIndices.Clear();
            Reader.Seek(Offset);
            for (int i = 0; i < Size; i++)
                DOBJIndices.Add(Reader.ReadUInt16());

            Reader.Seek(BaseOffset);
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(this);

            Writer.WritePointer(DOBJIndices);
            Writer.Write((ushort)DOBJIndices.Count);
            Writer.Write((ushort)0);

            Writer.Write(MinX);
            Writer.Write(MinY);
            Writer.Write(MinZ);
            Writer.Write(MaxX);
            Writer.Write(MaxY);
            Writer.Write(MaxZ);
        }
    }

    public class KAR_GrModel_ModelUnk1_2 : IHSDNode
    {
        [Browsable(false)]
        public uint Offset { get; set; }

        [Browsable(false)]
        public ushort Size { get; set; }

        [Browsable(false)]
        public ushort Padding { get; set; }

        public float UnkFloat1 { get; set; }

        public float UnkFloat2 { get; set; }

        public float UnkFloat3 { get; set; }

        public float UnkFloat4 { get; set; }

        public float UnkFloat5 { get; set; }

        public float UnkFloat6 { get; set; }

        public uint UnkUInt7 { get; set; }

        public List<ushort> DOBJIndices = new List<ushort>();

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            uint BaseOffset = Reader.Position();

            DOBJIndices.Clear();
            Reader.Seek(Offset);
            for (int i = 0; i < Size; i++)
                DOBJIndices.Add(Reader.ReadUInt16());

            Reader.Seek(BaseOffset);
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(this);

            Writer.WritePointer(DOBJIndices);
            Writer.Write((ushort)DOBJIndices.Count);
            Writer.Write((ushort)0);

            Writer.Write(UnkFloat1);
            Writer.Write(UnkFloat2);
            Writer.Write(UnkFloat3);
            Writer.Write(UnkFloat4);
            Writer.Write(UnkFloat5);
            Writer.Write(UnkFloat6);
            Writer.Write(UnkUInt7);
        }
    }

    public class KAR_GrBoundingBox : IHSDNode
    {
        public float MinX { get; set; }

        public float MinY { get; set; }

        public float MinZ { get; set; }

        public float MaxX { get; set; }

        public float MaxY { get; set; }

        public float MaxZ { get; set; }
    }

}
