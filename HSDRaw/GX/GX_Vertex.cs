namespace HSDRaw.GX
{
    public struct GX_Vertex
    {
        public ushort PNMTXIDX;
        public ushort TEX0MTXIDX;
        public GXVector3 POS;
        public GXVector3 NRM;
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

        public static int Stride = (1 + 3 + 3 + 4 + 4 + 2 + 2 + 2 + 2 + 2 + 2 + 2 + 2) * 4;

        public static bool operator ==(GX_Vertex x, GX_Vertex y)
        {
            return x.POS == y.POS && x.NRM == y.NRM && x.PNMTXIDX == y.PNMTXIDX && x.TEX0MTXIDX == y.TEX0MTXIDX;
        }
        public static bool operator !=(GX_Vertex x, GX_Vertex y)
        {
            return !(y == x);
        }
    }
}
