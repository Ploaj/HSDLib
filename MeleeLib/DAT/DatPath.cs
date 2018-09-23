using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;

namespace MeleeLib.DAT
{
    public struct DatPathPoint
    {
        public float Time;
        public float X;
        public float Y;
        public float Z;
    }

    public class DatPath : DatNode
    {
        public ushort Flags;
        public float Duration;
        public List<DatPathPoint> PathPoints;
        public int Unk1 = 0, Unk2 = 0;

        public void Deserialize(DATReader r, DATRoot Root)
        {
            Flags = (ushort)r.Short();
            int Count = r.Short();
            PathPoints = new List<DatPathPoint>(Count);
            Unk1 = r.Int();
            int PointOffset = r.Int();
            Duration = r.Float();
            int TimeOffset = r.Int();
            Unk2 = r.Int();

            for(int i = 0; i < Count; i++)
            {
                DatPathPoint point = new DatPathPoint();
                r.Seek(PointOffset + 0xC * i);
                point.X = r.Float();
                point.Y = r.Float();
                point.Z = r.Float();

                r.Seek(TimeOffset + 4 * i);
                point.Time = r.Float();
                PathPoints.Add(point);
            }
        }

        public override void Serialize(DATWriter Node)
        {
            Node.AddObject(PathPoints);
            foreach (DatPathPoint p in PathPoints)
            { Node.Float(p.X); Node.Float(p.Y); Node.Float(p.Z); }
            object temp = new object();
            Node.AddObject(temp);
            foreach (DatPathPoint p in PathPoints)
                Node.Float(p.Time);

            Node.Int(PathPoints.Count);
            Node.Int(Unk1);
            Node.Object(PathPoints);
            Node.Float(Duration);
            Node.Object(temp);
            Node.Int(Unk2);
        }
    }
}
