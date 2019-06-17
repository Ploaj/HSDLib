using HSDLib.Helpers;
using System;
using System.Collections.Generic;

namespace HSDLib.KAR
{
    public class KAR_GrSplineNode : IHSDNode
    {
        public KAR_GrMainPathSetup MainPathSetup { get; set; }

        //TODO: rest of these
        public int AIPathSetup { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public int ConveyPath { get; set; }
        public int Unknown3 { get; set; }
        public int RailPath { get; set; }
    }

    public class KAR_GrMainPathSetup : IHSDNode
    {
        public KAR_GrPathList MainPathList1 { get; set; }
        public KAR_GrPathList MainPathList2 { get; set; }
        public KAR_GrPathList MainPathList3 { get; set; }
        public KAR_GrPathList MainPathList4 { get; set; }
        public int Unknown1 { get; set; } = 1;
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public int Unknown4 { get; set; }
        public int Unknown5 { get; set; }
        public int Unknown6 { get; set; }
        public int Unknown7 { get; set; }
        public int Unknown8 { get; set; }
        public int Unknown9 { get; set; }
        public int Unknown10 { get; set; }
    }

    public class KAR_GrPathList : IHSDNode
    {
        public List<KAR_GrPath> Paths = new List<KAR_GrPath>();

        public override void Open(HSDReader Reader)
        {
            var offset = Reader.ReadUInt32();
            var count = Reader.ReadInt32();

            uint[] offsets = new uint[count];
            Reader.Seek(offset);
            for (int i = 0; i < count; i++)
            {
                offsets[i] = Reader.ReadUInt32();
            }

            for(int i = 0; i < count; i++)
            {
                if (offsets[i] == 0)
                    continue;
                Reader.Seek(offsets[i]);
                var path = new KAR_GrPath();
                path.Open(Reader);
                Paths.Add(path);
            }
        }

        public override void Save(HSDWriter Writer)
        {
            foreach(var p in Paths)
            {
                p.Save(Writer);
            }

            Writer.AddObject(Paths);
            foreach (var p in Paths)
            {
                Writer.WritePointer(p);
            }

            if (Paths.Count == 0)
                Writer.Write(0);

            Writer.AddObject(this);
            Writer.WritePointer(Paths);
            Writer.Write(Paths.Count == 0 ? 1 : Paths.Count);
        }
    }

    public class KAR_GrPath : IHSDNode
    {
        public List<GXVector3> Points = new List<GXVector3>();
        public List<float> PointPercents = new List<float>();

        public float UnknownFloat;

        public override void Open(HSDReader Reader)
        {
            int PathPointCount = Reader.ReadInt32();
            if (Reader.ReadInt32() != 0)
                throw new NotSupportedException("Dat not supported");
            uint PointTableOffset = Reader.ReadUInt32();
            UnknownFloat = Reader.ReadSingle();
            uint PathTableOffset = Reader.ReadUInt32();
            if (Reader.ReadInt32() != 0)
                throw new NotSupportedException("Dat not supported");
            if (Reader.ReadInt32() != 0)
                throw new NotSupportedException("Dat not supported");
            if (Reader.ReadInt32() != 0)
                throw new NotSupportedException("Dat not supported");

            Reader.Seek(PointTableOffset);
            for(int i = 0; i < PathPointCount; i++)
            {
                Points.Add(new GXVector3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle()));
            }

            Reader.Seek(PathTableOffset);
            for (int i = 0; i < PathPointCount; i++)
            {
                PointPercents.Add(Reader.ReadSingle());
            }
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(Points);
            foreach (var point in Points)
            {
                Writer.Write(point.X);
                Writer.Write(point.Y);
                Writer.Write(point.Z);
            }

            Writer.AddObject(PointPercents);
            foreach (var per in PointPercents)
                Writer.Write(per);

            Writer.AddObject(this);
            Writer.Write(Points.Count);
            Writer.Write(0);
            Writer.WritePointer(Points);
            Writer.Write(UnknownFloat);
            Writer.WritePointer(PointPercents);
            Writer.Write(0);
            Writer.Write(0);
            Writer.Write(0);
        }
    }
}
