namespace HSDRaw.GX
{
    public struct GXVector4
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
    }

    public struct GXColor4
    {
        public float R, G, B, A;

        public GXColor4(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }

    public struct GXVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public GXVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static bool operator ==(GXVector3 x, GXVector3 y)
        {
            return x.X == y.X && x.Y == y.Y && x.Z == y.Z;
        }
        public static bool operator !=(GXVector3 x, GXVector3 y)
        {
            return !(x.X == y.X && x.Y == y.Y && x.Z == y.Z);
        }
    }

    public struct GXVector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public GXVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(GXVector2 x, GXVector2 y)
        {
            return x.X == y.X && x.Y == y.Y;
        }
        public static bool operator !=(GXVector2 x, GXVector2 y)
        {
            return !(x.X == y.X && x.Y == y.Y);
        }
    }
}
