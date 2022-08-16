namespace HSDRaw.GX
{
    public struct GX_Vertex
    {
        public ushort PNMTXIDX;
        public ushort TEX0MTXIDX;
        public ushort TEX1MTXIDX;
        public ushort TEX2MTXIDX;
        public GXVector3 POS;
        public GXVector3 NRM;
        public GXVector3 TAN;
        public GXVector3 BITAN;
        public GXColor4 CLR0;
        public GXColor4 CLR1;
        public GXVector2 TEX0;
        public GXVector2 TEX1;
        public GXVector2 TEX2;
        public GXVector2 TEX3;
        public GXVector2 TEX4;
        public GXVector2 TEX5;
        public GXVector2 TEX6;
        public GXVector2 TEX7;

        public static int Stride = (1 + 1 + 3 + 3 + 3 + 3 + 4 + 4 + 2 + 2 + 2 + 2 + 2 + 2 + 2 + 2) * 4;

        public override int GetHashCode()
        {
            return (POS, NRM, PNMTXIDX, TEX0MTXIDX).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is GX_Vertex vert && this == vert;
        }

        public static bool operator ==(GX_Vertex x, GX_Vertex y)
        {
            return x.POS == y.POS && x.NRM == y.NRM && x.PNMTXIDX == y.PNMTXIDX && x.TEX0MTXIDX == y.TEX0MTXIDX;
        }
        public static bool operator !=(GX_Vertex x, GX_Vertex y)
        {
            return !(y == x);
        }
    }

    public struct GX_Shape
    {
        public GXVector3 POS;
        public GXVector3 NRM;

        public static int Stride = (3 + 3) * 4;

        public static bool operator ==(GX_Shape x, GX_Shape y)
        {
            return x.POS == y.POS && x.NRM == y.NRM;
        }
        public static bool operator !=(GX_Shape x, GX_Shape y)
        {
            return !(y == x);
        }
    }
}
