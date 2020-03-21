namespace HSDRaw.GX
{
    public struct GXVector4
    {
        public GXVector4(float x, float y, float z, float w) : this()
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
        }
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

        public byte[] ToBytes()
        {
            return new byte[] {
                (byte) (R > 1 ? 0xFF : R * 0xFF),
                (byte) (G > 1 ? 0xFF : G * 0xFF),
                (byte) (B > 1 ? 0xFF : B * 0xFF),
                (byte) (A > 1 ? 0xFF : A * 0xFF)
                };
        }

        public override string ToString()
        {
            return $"({R}, {G}, {B}, {A})";
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
