using OpenTK.Mathematics;

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
    }
}
