using OpenTK.Mathematics;

namespace HSDRawViewer.Tools.SpatialOrganizer
{
    public class SpatialTriangle
    {
        public int ZoneIndex = -1;
        public int Index;

        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;

        public Vector3 Min => Vector3.ComponentMin(p1, Vector3.ComponentMin(p2, p3));

        public Vector3 Max => Vector3.ComponentMax(p1, Vector3.ComponentMax(p2, p3));

        public Vector3 Middle => (p1 + p2 + p3) / 3;

        public float GetMaxAxis(int axis)
        {
            switch (axis)
            {
                case 0: return Max.X;
                case 1: return Max.Y;
                case 2: return Max.Z;
            }
            return 0;
        }

        public float GetMinAxis(int axis)
        {
            switch (axis)
            {
                case 0: return Min.X;
                case 1: return Min.Y;
                case 2: return Min.Z;
            }
            return 0;
        }

        public override string ToString()
        {
            return $"{p1} {p2} {p3}";
        }
    }
}
