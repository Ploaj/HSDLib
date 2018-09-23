using System;
using System.Collections.Generic;
using System.Linq;
using MeleeLib.GCX;
using MeleeLib.IO;

namespace MeleeLib.DAT.Helpers
{
    public class GXVertexCompressor
    {
        private class BufferGroup
        {
            List<object> Objects = new List<object>();
            private GXAttr a;

            public BufferGroup(GXAttr att)
            {
                a = att;
            }

            public byte[] GetBuffer()
            {
                DATWriter o = new DATWriter();
                WriteCompressedData(o, a);
                return o.GetBytes();
            }

            public void WriteCompressedData(DATWriter o, GXAttr a)
            {
                if(a.AttributeType != GXAttribType.GX_DIRECT)
                {
                    foreach(object ob in Objects)
                        switch (a.Name)
                        {
                            case GXAttribName.GX_VA_POS:
                                WriteCompress(o, a.CompType, a.Scale, new float[] { ((GXVector3)ob).X , ((GXVector3)ob).Y, ((GXVector3)ob).Z });
                                break;
                            case GXAttribName.GX_VA_NRM:
                                WriteCompress(o, a.CompType, a.Scale, new float[] { ((GXVector3)ob).X, ((GXVector3)ob).Y, ((GXVector3)ob).Z });
                                break;
                            case GXAttribName.GX_VA_TEX0:
                                WriteCompress(o, a.CompType, a.Scale, new float[] { ((GXVector2)ob).X, ((GXVector2)ob).Y });
                                break;
                            case GXAttribName.GX_VA_TEX1:
                                WriteCompress(o, a.CompType, a.Scale, new float[] { ((GXVector2)ob).X, ((GXVector2)ob).Y });
                                break;
                            case GXAttribName.GX_VA_CLR0:
                                WriteCompress(o, a.CompType, a.Scale, new float[] { ((GXVector4)ob).X*0xFF, ((GXVector4)ob).Y * 0xFF, ((GXVector4)ob).Z * 0xFF, ((GXVector4)ob).W * 0xFF });
                                break;
                            case GXAttribName.GX_VA_NBT:
                                WriteCompress(o, a.CompType, a.Scale, new float[] { ((GXVector3[])ob)[0].X, ((GXVector3[])ob)[0].Y, ((GXVector3[])ob)[0].Z,
                                ((GXVector3[])ob)[1].X, ((GXVector3[])ob)[1].Y, ((GXVector3[])ob)[1].Z,
                                ((GXVector3[])ob)[2].X, ((GXVector3[])ob)[2].Y, ((GXVector3[])ob)[2].Z, });
                                break;
                            default:
                                throw new Exception("Unimplemented Attribute Compression " + a.Name.ToString());
                        }
                }
            }

            private static void WriteCompress(DATWriter d, GXCompType type, float Scale, float[] f)
            {
                for (int i = 0; i < f.Length; i++)
                {
                    switch (type)
                    {
                        case GXCompType.GX_U8:
                            d.Byte((byte)(f[i] * (float)Math.Pow(2, Scale)));
                            break;
                        case GXCompType.GX_S8:
                            d.Byte((byte)(f[i] * (float)Math.Pow(2, Scale)));
                            break;
                        case GXCompType.GX_U16:
                            d.Short((short)(f[i] * (float)Math.Pow(2, Scale)));
                            break;
                        case GXCompType.GX_S16:
                            d.Short((short)(f[i] * (float)Math.Pow(2, Scale)));
                            break;
                        case GXCompType.GX_F32:
                            d.Float(f[i] * (float)Math.Pow(2, Scale));
                            break;
                        case GXCompType.GX_CLR:
                            d.Float((byte)(f[i] * (float)Math.Pow(2, Scale)));
                            break;
                    }
                }
            }

            public ushort GetIndex(GXAttribName AttName, GXVertex Vertex)
            {
                object o = null;

                switch (AttName)
                {
                    case GXAttribName.GX_VA_PNMTXIDX: o = Vertex.PMXID; break;
                    case GXAttribName.GX_VA_TEX0MTXIDX: o = Vertex.TEX0MTXID; break;
                    case GXAttribName.GX_VA_POS: o = Vertex.Pos; break;
                    case GXAttribName.GX_VA_NRM: o = Vertex.Nrm; break;
                    case GXAttribName.GX_VA_TEX0: o = Vertex.TX0; break;
                    case GXAttribName.GX_VA_TEX1: o = Vertex.TX1; break;
                    case GXAttribName.GX_VA_CLR0: o = Vertex.CLR0; break;
                    case GXAttribName.GX_VA_NBT: o = new GXVector3[] { Vertex.Nrm, Vertex.Bit, Vertex.Tan }; break;
                    default:
                        throw new Exception("Unimplemented Attribute Compression for " + AttName.ToString());
                }

                int index = Objects.IndexOf(o);
                if (index == -1)
                {
                    Objects.Add(o);
                    return (ushort)(Objects.Count - 1);
                }
                return (ushort)index;
            }
        }

        private List<BufferGroup> Buffers = new List<BufferGroup>();
        
        private Dictionary<GXAttr, int> AttributeBufferLink = new Dictionary<GXAttr, int>();

        public GXVertexCompressor()
        {
        }

        public void CompileChanges()
        {
            List<byte[]> DataBuffers = new List<byte[]>();

            foreach (BufferGroup g in Buffers)
                DataBuffers.Add(g.GetBuffer());

            foreach(GXAttr a in AttributeBufferLink.Keys)
            {
                a.DataBuffer = DataBuffers[AttributeBufferLink[a]];
            }
        }

        public GXDisplayList CompressDisplayList(GXVertex[] Verts, GXPrimitiveType PrimitiveType, GXAttribGroup AttributeGroup)
        {
            GXDisplayList DL = new GXDisplayList();
            DL.PrimitiveType = PrimitiveType;
            DL.Count = Verts.Length;

            // Preprocess Attributes
            foreach (GXAttr a in AttributeGroup.Attributes)
            {
                if (AttributeBufferLink.ContainsKey(a)) continue;
                foreach (GXAttr att in AttributeBufferLink.Keys)
                {
                    if (att.AttributeType == a.AttributeType &&
                        att.Name == a.Name &&
                        att.CompType == a.CompType &&
                        att.Scale == a.Scale)
                    {
                        // these two use the same buffer
                        AttributeBufferLink.Add(a, AttributeBufferLink[att]);
                        break;
                    }
                }
                if (!AttributeBufferLink.ContainsKey(a))
                {
                    AttributeBufferLink.Add(a, Buffers.Count);
                    Buffers.Add(new BufferGroup(a));
                }
            }

            List<GXIndexGroup> Indices = new List<GXIndexGroup>();
            foreach(GXVertex v in Verts)
            {
                GXIndexGroup ig = new GXIndexGroup();
                ig.Indices = new ushort[AttributeGroup.Attributes.Count];
                int i = 0;
                foreach(GXAttr a in AttributeGroup.Attributes)
                {
                    if (a.AttributeType == GXAttribType.GX_DIRECT)
                    {
                        if (a.Name == GXAttribName.GX_VA_CLR0 || a.Name == GXAttribName.GX_VA_CLR1)
                        {
                            ig.Clr = new byte[] { (byte)(v.CLR0.X * 0xFF), (byte)(v.CLR0.Y * 0xFF), (byte)(v.CLR0.Z * 0xFF), (byte)(v.CLR0.W * 0xFF) };
                        }
                        else
                            if(a.Name == GXAttribName.GX_VA_PNMTXIDX)
                                ig.Indices[i] = (ushort)v.PMXID;
                        else
                            if (a.Name == GXAttribName.GX_VA_TEX0MTXIDX)
                            ig.Indices[i] = (ushort)v.TEX0MTXID;
                    }
                    else
                        ig.Indices[i] = Buffers[AttributeBufferLink[a]].GetIndex(a.Name, v);
                    i++;
                }
                Indices.Add(ig);
            }
            DL.Indices = Indices.ToArray();

            return DL;
        }
    }
}
