using System;
using System.Collections.Generic;

namespace HSDLib.KAR
{
    public class KAR_GrPartitionNode : IHSDNode
    {
        public KAR_GrPartitionSetup Setup { get; set; }
    }

    public class KAR_GrPartitionSetup : IHSDNode
    {
        public List<KAR_GrPartition> Partitions = new List<KAR_GrPartition>();

        public List<short> CollidableTriangles = new List<short>();

        public List<bool> CollidableTrianglesBits = new List<bool>();

        public override void Open(HSDReader Reader)
        {
            var partitionPointer = Reader.ReadUInt32();
            var partitionCount = Reader.ReadInt16();
            Reader.ReadInt16();

            {
                var temp = Reader.Position();
                Reader.Seek(partitionPointer);
                uint[] pointers = new uint[partitionCount];
                for (int i = 0; i < partitionCount; i++)
                {
                    pointers[i] = Reader.ReadUInt32();
                }
                    for (int i = 0; i < partitionCount; i++)
                {
                    Reader.Seek(pointers[i]);
                    KAR_GrPartition p = new KAR_GrPartition();
                    p.Open(Reader);
                    Partitions.Add(p);
                }
                Reader.Seek(temp);
            }

            // now for a few sections
            // there are 5
            // 1 - collidable triangles
            // 2 - ? related to pads
            // 3 - ?
            // 4 - ?
            // 5 - ?
            // 32 Data Type
            // 32 pointer
            // 16 count
            // 16 padding

            {
                var dataType = Reader.ReadInt32();
                var pointer = Reader.ReadUInt32();
                var count = Reader.ReadInt16();
                Reader.ReadInt16();

                var temp = Reader.Position();
                Reader.Seek(pointer);
                for (int i = 0; i < count; i++)
                    CollidableTriangles.Add(Reader.ReadInt16());
                Reader.Seek(temp);
            }

            { //TODO:
                var dataType = Reader.ReadInt32();
                var pointer = Reader.ReadUInt32();
                var count = Reader.ReadInt16();
                Reader.ReadInt16();
            }

            { //TODO:
                var dataType = Reader.ReadInt32();
                var pointer = Reader.ReadUInt32();
                var count = Reader.ReadInt16();
                Reader.ReadInt16();
            }

            { //TODO:
                var dataType = Reader.ReadInt32();
                var pointer = Reader.ReadUInt32();
                var count = Reader.ReadInt16();
                Reader.ReadInt16();
            }

            { //TODO:
                var dataType = Reader.ReadInt32();
                var pointer = Reader.ReadUInt32();
                var count = Reader.ReadInt16();
                Reader.ReadInt16();
            }

            if (Reader.ReadInt32() != 0)
                throw new NotSupportedException("Dat format not supported");

            // one more bool section, usually all false?

            { //TODO:
                var dataType = Reader.ReadInt32();
                var pointer = Reader.ReadUInt32();
                var count = Reader.ReadInt16();
                Reader.ReadInt16();
                var temp = Reader.Position();

                Reader.Seek(pointer);
                // TODO: bit reader
                Reader.Seek(temp);
            }
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(Partitions);
            foreach (var p in Partitions)
                Writer.WritePointer(p);

            foreach (var p in Partitions)
                p.Save(Writer);

            Writer.AddObject(CollidableTriangles);
            foreach (var v in CollidableTriangles)
                Writer.Write(v);
            Writer.Align(4);

            Writer.AddObject(CollidableTrianglesBits);
            Writer.Write(new byte[(CollidableTriangles.Count / 8) + 5]);
            Writer.Align(4);

            Writer.AddObject(this);
            Writer.WritePointer(Partitions);
            Writer.Write((short)Partitions.Count);
            Writer.Write((short)0);

            Writer.Write(0x00000005);
            Writer.WritePointer(CollidableTriangles);
            Writer.Write((short)CollidableTriangles.Count);
            Writer.Write((short)0);

            Writer.Write(0);
            Writer.Write(0);
            Writer.Write(0);

            Writer.Write(0);
            Writer.Write(0);
            Writer.Write(0);

            Writer.Write(0);
            Writer.Write(0);
            Writer.Write(0);

            Writer.Write(0);
            Writer.Write(0);
            Writer.Write(0);

            Writer.Write(0);
            Writer.Write(0);
            Writer.Write(0);

            Writer.Write(0x00000003);
            Writer.WritePointer(CollidableTrianglesBits);
            Writer.Write((short)CollidableTriangles.Count);
            Writer.Write((short)0);
        }
    }

    public class KAR_GrPartition : IHSDNode
    {
        public float MinX { get; set; }
        public float MinY { get; set; }
        public float MinZ { get; set; }
        public float MaxX { get; set; }
        public float MaxY { get; set; }
        public float MaxZ { get; set; }

        public short ChildStartIndex { get; set; }
        public short ChildEndIndex { get; set; }
        public short CollisionFaceStart { get; set; }
        public short CollisionFaceCount { get; set; }
        public short Unknown1 { get; set; }
        public short Unknown2 { get; set; }
        public short UnknownStart { get; set; }
        public short UnknownCount { get; set; }
        public short Unknown3 { get; set; }
        public short Unknown4 { get; set; }
        public short Unknown5 { get; set; }
        public short Unknown6 { get; set; }
        public short Unknown7 { get; set; }
        public short Unknown8 { get; set; }
        public short Unknown9 { get; set; }
        public short Unknown10 { get; set; }
        public short Unknown11 { get; set; }
        public short Unknown12 { get; set; }
        public short Unknown13 { get; set; }
        public short Unknown14 { get; set; }
        public short Unknown15 { get; set; }
        public short Unknown16 { get; set; }
        public short Unknown17 { get; set; }
        public short Unknown18 { get; set; }
        public short Unknown19 { get; set; }
        public short Unknown20 { get; set; }
        public int Depth { get; set; }
    }
}
