using HSDLib.Helpers;
using System;
using System.Collections.Generic;

namespace HSDLib.Common
{
    public class HSD_Spline : IHSDNode
    {
        public List<GXVector3> Points = new List<GXVector3>();
        public List<float> PointPercents = new List<float>();

        public float UnknownFloat;

        public override void Open(HSDReader Reader)
        {
            int PathPointCount = Reader.ReadInt32();
            if (Reader.ReadInt32() != 0)
                Console.WriteLine("Resave not supported"); //throw new NotSupportedException("Dat not supported");
            uint PointTableOffset = Reader.ReadUInt32();
            UnknownFloat = Reader.ReadSingle();
            uint PathTableOffset = Reader.ReadUInt32();
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
