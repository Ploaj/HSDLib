using HSDLib.GX;
using System;
using System.Collections.Generic;
using HSDLib.Common;
using System.IO;

namespace HSDLib.Helpers
{
    /// <summary>
    /// Helper class for creating the vertex buffer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BufferMaker<T>
    {
        public List<T> Collection = new List<T>();
        private Dictionary<T, int> QuickIndex = new Dictionary<T, int>();

        public ushort GetIndex(T o)
        {
            int index = 0;
            if(!QuickIndex.ContainsKey(o))
            {
                index = Collection.Count;
                Collection.Add(o);
                QuickIndex.Add(o, index);
            }
            
            return (ushort)QuickIndex[o];
        }

        public byte[] GetData(GXVertexBuffer Buffer)
        {
            MemoryStream o = new MemoryStream();
            HSDWriter Writer = new HSDWriter(o);
            foreach(object ob in Collection)
            {
                if(ob is GXVector2)
                {
                    WriteData(Writer, ((GXVector2)ob).X, Buffer.CompType, Buffer.Scale);
                    WriteData(Writer, ((GXVector2)ob).Y, Buffer.CompType, Buffer.Scale);
                }
                if (ob is GXVector3)
                {
                    WriteData(Writer, ((GXVector3)ob).X, Buffer.CompType, Buffer.Scale);
                    WriteData(Writer, ((GXVector3)ob).Y, Buffer.CompType, Buffer.Scale);
                    WriteData(Writer, ((GXVector3)ob).Z, Buffer.CompType, Buffer.Scale);
                }
            }
            Writer.Align(0x20);
            byte[] data = o.ToArray();
            Writer.Close();
            o.Close();
            Writer.Dispose();

            return data;
        }

        private void WriteData(HSDWriter Writer, float Value, GXCompType Type, float Scale)
        {
            float Scaled = Value * (float)Math.Pow(2, Scale);
            switch (Type)
            {
                case GXCompType.UInt8:
                    Writer.Write((byte)Scaled);
                    break;
                case GXCompType.Int8:
                    Writer.Write((sbyte)Scaled);
                    break;
                case GXCompType.UInt16:
                    Writer.Write((ushort)Scaled);
                    break;
                case GXCompType.Int16:
                    Writer.Write((short)Scaled);
                    break;
                case GXCompType.Float:
                    Writer.Write(Scaled);
                    break;
                default:
                    Writer.Write((byte)Scaled);
                    break;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VertexCompressor
    {
        private Dictionary<GXVertexBuffer, BufferMaker<object>> BufferLinker = new Dictionary<GXVertexBuffer, BufferMaker<object>>();
        
        // Creates the new buffers
        public void SaveChanges()
        {
            Dictionary<BufferMaker<object>, byte[]> BufferData = new Dictionary<BufferMaker<object>, byte[]>();
            foreach (GXVertexBuffer b in BufferLinker.Keys)
            {
                if (BufferData.ContainsKey(BufferLinker[b]))
                {
                    b.DataBuffer = BufferData[BufferLinker[b]];
                    continue;
                }

                byte[] data = BufferLinker[b].GetData(b);

                b.DataBuffer = data;

                BufferData.Add(BufferLinker[b], data);
            }
        }

        /// <summary>
        /// Gets the index of a value in the buffer
        /// </summary>
        /// <param name="v"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        private ushort GetIndex(GXVertexBuffer v, object o)
        {
            if (!BufferLinker.ContainsKey(v))
            {
                foreach(GXVertexBuffer buf in BufferLinker.Keys)
                {
                    if(buf.Name == v.Name 
                        && buf.Scale == v.Scale
                        && buf.AttributeType == v.AttributeType
                        && buf.CompType == v.CompType
                        && buf.CompCount == v.CompCount)
                    {
                        BufferLinker.Add(v, BufferLinker[buf]);
                        return BufferLinker[buf].GetIndex(o);
                    }
                }
                BufferLinker.Add(v, new BufferMaker<object>());
            }
            return BufferLinker[v].GetIndex(o);
        }
        
        /// <summary>
        /// Creates a primitive group for the vertex buffer
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Vertices"></param>
        /// <param name="Attributes"></param>
        /// <returns></returns>
        public GXPrimitiveGroup Compress(GXPrimitiveType type, GXVertex[] Vertices, HSD_AttributeGroup Attributes)
        {
            GXPrimitiveGroup g = new GXPrimitiveGroup();
            g.PrimitiveType = type;
            g.Indices = new GXIndexGroup[Vertices.Length];
            int IndexGroupIndex = 0;
            foreach (GXVertex v in Vertices)
            {
                GXIndexGroup ig = new GXIndexGroup();
                ig.Indices = new ushort[Attributes.Attributes.Count];

                int i = 0;
                foreach(GXVertexBuffer b in Attributes.Attributes)
                {
                    switch (b.AttributeType)
                    {
                        case GXAttribType.GX_DIRECT:
                            if (b.Name == GXAttribName.GX_VA_CLR0)
                                ig.Clr0 = new byte[] { (byte)(v.Clr0.R * 0xFF), (byte)(v.Clr0.G * 0xFF), (byte)(v.Clr0.B * 0xFF), (byte)(v.Clr0.A * 0xFF) };
                            else
                            if (b.Name == GXAttribName.GX_VA_CLR1)
                                ig.Clr1 = new byte[] { (byte)(v.Clr1.R * 0xFF), (byte)(v.Clr1.G * 0xFF), (byte)(v.Clr1.B * 0xFF), (byte)(v.Clr1.A * 0xFF) };
                            else
                            if (b.Name == GXAttribName.GX_VA_PNMTXIDX)
                                ig.Indices[i] = v.PMXID;
                            if (b.Name == GXAttribName.GX_VA_TEX0MTXIDX)
                                ig.Indices[i] = v.TEX0MTXIDX;
                            break;
                        default:
                            switch (b.Name)
                            {
                                case GXAttribName.GX_VA_POS: ig.Indices[i] = GetIndex(b, v.Pos); break;
                                case GXAttribName.GX_VA_NRM: ig.Indices[i] = GetIndex(b, v.Nrm); break;
                                case GXAttribName.GX_VA_TEX0: ig.Indices[i] = GetIndex(b, v.TEX0); break;
                                case GXAttribName.GX_VA_TEX1: ig.Indices[i] = GetIndex(b, v.TEX1); break;
                                default:
                                    throw new Exception("Error Building " + b.Name);
                            }
                            
                            break;
                    }
                    i++;
                }
                g.Indices[IndexGroupIndex++] = ig;
            }

            return g;
        }

    }
}
