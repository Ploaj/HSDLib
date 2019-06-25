using HSDLib.Helpers;
using System;
using System.Collections.Generic;

namespace HSDLib.Common
{
    public class HSD_Spline : IHSDNode
    {
        public short Type { get; set; }

        public float Tension { get; set; }

        public float TotalLength { get; set; }

        public List<GXVector3> Points = new List<GXVector3>();
        public List<float> SegmentLengths = new List<float>();

        public override void Open(HSDReader Reader)
        {
            Type = Reader.ReadInt16();
            int PathPointCount = Reader.ReadInt16();
            Tension = Reader.ReadSingle();
            uint PointTableOffset = Reader.ReadUInt32();
            TotalLength = Reader.ReadSingle();
            uint segmentLengthOffset = Reader.ReadUInt32();
            if (Reader.ReadInt32() != 0)
                Console.WriteLine("Resave not supported"); //throw new NotSupportedException("Dat not supported");
            if (Reader.ReadInt32() != 0)
                Console.WriteLine("Resave not supported"); //throw new NotSupportedException("Dat not supported");
            if (Reader.ReadInt32() != 0)
                Console.WriteLine("Resave not supported"); //throw new NotSupportedException("Dat not supported");

            Reader.Seek(PointTableOffset);
            for (int i = 0; i < PathPointCount; i++)
            {
                Points.Add(new GXVector3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle()));
            }

            Reader.Seek(segmentLengthOffset);
            for (int i = 0; i < PathPointCount; i++)
            {
                SegmentLengths.Add(Reader.ReadSingle());
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

            Writer.AddObject(SegmentLengths);
            foreach (var per in SegmentLengths)
                Writer.Write(per);

            Writer.AddObject(this);
            Writer.Write(Type);
            Writer.Write((ushort)Points.Count);
            Writer.Write(Tension);
            Writer.WritePointer(Points);
            Writer.Write(TotalLength);
            Writer.WritePointer(SegmentLengths);
            Writer.Write(0);
            Writer.Write(0);
            Writer.Write(0);
        }
    }
}
