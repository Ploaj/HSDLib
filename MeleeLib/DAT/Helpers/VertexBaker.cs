using System;
using System.Collections.Generic;
using MeleeLib.IO;
using MeleeLib.GCX;

namespace MeleeLib.DAT.Helpers
{
    public class GXVertexDecompressor
    {
        DATRoot Root;

        public GXVertexDecompressor(DATRoot Root)
        {
            this.Root = Root;
        }

        public GXVertex[] GetFormattedVertices(DatPolygon poly)
        {
            List<GXVertex> V = new List<GXVertex>();
            foreach (GXDisplayList dl in poly.DisplayLists)
                V.AddRange(GetFormattedVertices(dl, poly));
            return V.ToArray();
        }

        public GXVertex[] GetFormattedVertices(GXDisplayList dl, DatPolygon poly)
        {
            DatJOBJ[] jobs = Root.GetJOBJinOrder();
            List<DatJOBJ> js = new List<DatJOBJ>();
            js.AddRange(jobs);

            Dictionary<GXAttr, DATReader> Buffers = new Dictionary<GXAttr, DATReader>();

            foreach (GXAttr att in poly.AttributeGroup.Attributes)
            {
                Buffers.Add(att, new DATReader(att.DataBuffer));
            }

            List<GXVertex> V = new List<GXVertex>();
            {
                foreach (GXIndexGroup ig in dl.Indices)
                {
                    int nodeid = -1;
                    GXVertex Vertex = new GXVertex();
                    Vertex.CLR0 = new GXVector4(1, 1, 1, 1);
                    int i = 0;
                    foreach (GXAttr att in poly.AttributeGroup.Attributes)
                    {
                        DATReader VertexBuffer = Buffers[att];

                        int value = ig.Indices[i++];
                        float[] f;
                        int te;
                        switch (att.Name)
                        {
                            case GXAttribName.GX_VA_TEX0MTXIDX:
                                Vertex.TEX0MTXID = value;
                                break;
                            case GXAttribName.GX_VA_CLR0:
                                if(att.AttributeType == GXAttribType.GX_DIRECT)
                                {
                                    Vertex.CLR0.X = ig.Clr[0] / (float)0xFF;
                                    Vertex.CLR0.Y = ig.Clr[1] / (float)0xFF;
                                    Vertex.CLR0.Z = ig.Clr[2] / (float)0xFF;
                                    Vertex.CLR0.W = ig.Clr[3] / (float)0xFF;
                                }
                                else
                                {
                                    VertexBuffer.Seek(att.Stride * value);
                                    f = read3(VertexBuffer, att.CompType, att.Stride);
                                    Vertex.CLR0.X = f[0] / 0xFF;
                                    Vertex.CLR0.Y = f[1] / 0xFF;
                                    Vertex.CLR0.Z = f[2] / 0xFF;
                                    Vertex.CLR0.W = f[3] / 0xFF;
                                }
                                break;
                            case GXAttribName.GX_VA_PNMTXIDX:
                                nodeid = value / 3;
                                if (poly.BoneWeightList.Count == 0) continue; //|| nodeid / 3 >= poly.BoneWeightList.Count
                                if(nodeid >= poly.BoneWeightList.Count)
                                {
                                    Vertex.W = new float[1];
                                    Vertex.N = new int[1];
                                    Vertex.N[0] = js.IndexOf(poly.BoneWeightList[0][0].jobj);
                                    Vertex.W[0] = 1;

                                    continue;
                                }
                                List<DatBoneWeight> bw = poly.BoneWeightList[nodeid];
                                Vertex.W = new float[bw.Count];
                                Vertex.N = new int[bw.Count];
                                Vertex.PMXID = value;
                                for (int k = 0; k < bw.Count; k++)
                                {
                                    Vertex.N[k] = js.IndexOf(bw[k].jobj);
                                    Vertex.W[k] = bw[k].Weight;
                                }
                                break;
                            case GXAttribName.GX_VA_POS:
                                VertexBuffer.Seek(att.Stride * value);
                                f = read3(VertexBuffer, att.CompType, att.Stride);
                                Vertex.Pos.X = f[0] / (float)Math.Pow(2, att.Scale);
                                Vertex.Pos.Y = f[1] / (float)Math.Pow(2, att.Scale);
                                Vertex.Pos.Z = f[2] / (float)Math.Pow(2, att.Scale);
                                break;
                            case GXAttribName.GX_VA_TEX0:
                                VertexBuffer.Seek(att.Stride * value);
                                f = read3(VertexBuffer, att.CompType, att.Stride);
                                Vertex.TX0.X = f[0] / (float)Math.Pow(2, att.Scale);
                                Vertex.TX0.Y = f[1] / (float)Math.Pow(2, att.Scale);
                                break;
                            case GXAttribName.GX_VA_TEX1:
                                VertexBuffer.Seek(att.Stride * value);
                                f = read3(VertexBuffer, att.CompType, att.Stride);
                                Vertex.TX1.X = f[0] / (float)Math.Pow(2, att.Scale);
                                Vertex.TX1.Y = f[1] / (float)Math.Pow(2, att.Scale);
                                break;
                            case GXAttribName.GX_VA_NRM:
                                VertexBuffer.Seek(att.Stride * value);
                                f = read3(VertexBuffer, att.CompType, att.Stride);
                                Vertex.Nrm.X = f[0] / (float)Math.Pow(2, att.Scale);
                                Vertex.Nrm.Y = f[1] / (float)Math.Pow(2, att.Scale);
                                Vertex.Nrm.Z = f[2] / (float)Math.Pow(2, att.Scale);
                                break;
                            case GXAttribName.GX_VA_NBT: 
                                VertexBuffer.Seek(att.Stride * value);
                                f = read3(VertexBuffer, att.CompType, att.Stride);
                                Vertex.Nrm.X = f[0] / (float)Math.Pow(2, att.Scale);
                                Vertex.Nrm.Y = f[1] / (float)Math.Pow(2, att.Scale);
                                Vertex.Nrm.Z = f[2] / (float)Math.Pow(2, att.Scale);
                                Vertex.Bit.X = f[3] / (float)Math.Pow(2, att.Scale);
                                Vertex.Bit.Y = f[4] / (float)Math.Pow(2, att.Scale);
                                Vertex.Bit.Z = f[5] / (float)Math.Pow(2, att.Scale);
                                Vertex.Tan.X = f[6] / (float)Math.Pow(2, att.Scale);
                                Vertex.Tan.Y = f[7] / (float)Math.Pow(2, att.Scale);
                                Vertex.Tan.Z = f[8] / (float)Math.Pow(2, att.Scale);
                                break;
                            default:
                                Console.WriteLine("Warning Unsupported Command " + att.Name.ToString() + " will not rebuild correctly");
                                break;
                        }
                    }
                    V.Add(Vertex);
                }
            }
            //Console.WriteLine("VertexEnd: " + VertexBuffer.Pos().ToString("x"));
            return V.ToArray();
        }


        private static float[] read3(DATReader d, GXCompType type, int size)
        {
            switch (type)
            {
                case GXCompType.GX_U16: size /= 2; break;
                case GXCompType.GX_S16: size /= 2; break;
                case GXCompType.GX_F32: size /= 4; break;
            }

            float[] a = new float[size];

            switch (type)
            {
                case GXCompType.GX_U8:
                    for (int i = 0; i < size; i++)
                        a[i] = d.Byte();
                    break;
                case GXCompType.GX_S8:
                    for (int i = 0; i < size; i++)
                        a[i] = unchecked((sbyte)(d.Byte()));
                    break;
                case GXCompType.GX_U16:
                    for (int i = 0; i < size; i++)
                        a[i] = (d.Short());
                    break;
                case GXCompType.GX_S16:
                    for (int i = 0; i < size; i++)
                        a[i] = (short)(d.Short());
                    break;
                case GXCompType.GX_F32:
                    for (int i = 0; i < size; i++)
                        a[i] = d.Float();
                    break;
                case GXCompType.GX_CLR:
                    for (int i = 0; i < size; i++)
                        a[i] = d.Byte();
                    break;
            }

            return a;
        }

    }
}
