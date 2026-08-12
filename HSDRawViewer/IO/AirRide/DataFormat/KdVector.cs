using OpenTK.Mathematics;
using System;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public class KdVector
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public KdVector()
        {
        }

        public KdVector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3 ToTkVector()
        {
            return new Vector3(X, Y, Z);
        }

        public override bool Equals(object obj)
        {
            if (obj is KdVector v)
                return X == v.X && Y == v.Y && Z == v.Z;

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
    }
}
