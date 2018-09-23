
namespace MeleeLib.GCX
{
    public struct GXVector4
    {
        public float X, Y, Z, W;

        public GXVector4(float X, float Y, float Z, float W)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.W = W;
        }
    }
    public struct GXVector3
    {
        public float X, Y, Z;

        public GXVector3(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }
    public struct GXVector2
    {
        public float X, Y;

        public GXVector2(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }

    public struct GXVertex
    {
        public GXVector3 Pos;
        public GXVector3 Nrm;
        public GXVector3 Bit;
        public GXVector3 Tan;
        public GXVector4 CLR0;
        public GXVector2 TX0;
        public GXVector2 TX1;
        public int[] N;
        public float[] W;
        public int PMXID;
        public int TEX0MTXID;
    }
}
