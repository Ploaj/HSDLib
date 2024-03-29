﻿using System.Collections.Generic;

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
            return (POS, NRM, CLR0, TEX0, TEX1, TEX2, TEX3, PNMTXIDX, TEX0MTXIDX).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is GX_Vertex vert && this == vert;
        }

        public static bool operator ==(GX_Vertex x, GX_Vertex y)
        {
            return x.POS == y.POS && x.NRM == y.NRM && x.TEX0 == y.TEX0 && x.TEX1 == y.TEX1 && x.TEX2 == y.TEX2 && x.TEX3 == y.TEX3 && x.PNMTXIDX == y.PNMTXIDX && x.TEX0MTXIDX == y.TEX0MTXIDX;
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

        public override bool Equals(object obj)
        {
            return obj is GX_Shape shape &&
                   EqualityComparer<GXVector3>.Default.Equals(POS, shape.POS) &&
                   EqualityComparer<GXVector3>.Default.Equals(NRM, shape.NRM);
        }

        public override int GetHashCode()
        {
            int hashCode = 1527933933;
            hashCode = hashCode * -1521134295 + POS.GetHashCode();
            hashCode = hashCode * -1521134295 + NRM.GetHashCode();
            return hashCode;
        }
    }
}
