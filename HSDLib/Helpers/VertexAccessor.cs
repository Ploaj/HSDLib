using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSDLib.Common;
using HSDLib.GX;
using System.IO;

namespace HSDLib.Helpers
{
    public struct GXVector2
    {
        public float X, Y;

        public GXVector2(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
    public struct GXVector3
    {
        public float X, Y, Z;

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

    public struct GXVertex
    {
        public GXVector3 Pos;
        public GXVector3 Nrm;
        public GXVector2 TEX0;
        public GXVector2 TEX1;
        public GXColor4 Clr0;
        public GXColor4 Clr1;
        public ushort PMXID;
        public ushort TEX0MTXIDX;

        public static bool operator ==(GXVertex x, GXVertex y)
        {
            return x.Pos == y.Pos && x.PMXID == y.PMXID && x.TEX0MTXIDX == y.TEX0MTXIDX;
        }
        public static bool operator !=(GXVertex x, GXVertex y)
        {
            return !(y == x);
        }
    }

    public class VertexDecoded
    {
        public GXAttribName Name;
        public byte[] Data;
    }

    /// <summary>
    /// Helper class for decoding the vertex information into a more usable format
    /// </summary>
    public class VertexAccessor
    {
        public static GXVertex[] GetDecodedVertices(GXPrimitiveGroup PrimitiveGroup, HSD_AttributeGroup Group)
        {
            // Create Vertex List
            List<GXVertex> Vertices = new List<GXVertex>();

            // Prepare vertex buffers for reading
            Dictionary<GXVertexBuffer, HSDReader> Buffers = new Dictionary<GXVertexBuffer, HSDReader>();
            foreach (GXVertexBuffer buffer in Group.Attributes)
            {
                Buffers.Add(buffer, buffer.DataBuffer == null ? null : new HSDReader(new MemoryStream(buffer.DataBuffer)));
            }

            // Decode
            foreach (GXIndexGroup ig in PrimitiveGroup.Indices)
            {
                GXVertex Vertex = new GXVertex();
                for (int i = 0; i < Group.Attributes.Count; i++)
                {
                    GXVertexBuffer Attr = Group.Attributes[i];
                    HSDReader VertexBuffer = Buffers[Attr];
                    int index = ig.Indices[i];
                    float[] f = new float[0];
                    if (VertexBuffer != null)
                    {
                        VertexBuffer.Seek((uint)(Attr.Stride * index));
                        f = Read(VertexBuffer, Attr.CompType, Attr.Stride);
                    }
                    switch (Attr.Name)
                    {
                        case GXAttribName.GX_VA_PNMTXIDX:
                            Vertex.PMXID = (ushort)index;
                            break;
                        case GXAttribName.GX_VA_TEX0MTXIDX:
                            Vertex.TEX0MTXIDX = (ushort)index;
                            break;
                        case GXAttribName.GX_VA_POS:
                            Vertex.Pos.X = f[0] / (float)Math.Pow(2, Attr.Scale);
                            Vertex.Pos.Y = f[1] / (float)Math.Pow(2, Attr.Scale);
                            Vertex.Pos.Z = f[2] / (float)Math.Pow(2, Attr.Scale);
                            break;
                        case GXAttribName.GX_VA_NRM:
                            Vertex.Nrm.X = f[0] / (float)Math.Pow(2, Attr.Scale);
                            Vertex.Nrm.Y = f[1] / (float)Math.Pow(2, Attr.Scale);
                            Vertex.Nrm.Z = f[2] / (float)Math.Pow(2, Attr.Scale);
                            break;
                        case GXAttribName.GX_VA_TEX0:
                            Vertex.TEX0.X = f[0] / (float)Math.Pow(2, Attr.Scale);
                            Vertex.TEX0.Y = f[1] / (float)Math.Pow(2, Attr.Scale);
                            break;
                        case GXAttribName.GX_VA_TEX1:
                            Vertex.TEX1.X = f[0] / (float)Math.Pow(2, Attr.Scale);
                            Vertex.TEX1.Y = f[1] / (float)Math.Pow(2, Attr.Scale);
                            break;
                        case GXAttribName.GX_VA_CLR0:
                            Vertex.Clr0.R = ig.Clr0[0] / 255f;
                            Vertex.Clr0.G = ig.Clr0[1] / 255f;
                            Vertex.Clr0.B = ig.Clr0[2] / 255f;
                            Vertex.Clr0.A = ig.Clr0[3] / 255f;
                            break;
                        case GXAttribName.GX_VA_CLR1:
                            Vertex.Clr1.R = ig.Clr1[0] / 255f;
                            Vertex.Clr1.G = ig.Clr1[1] / 255f;
                            Vertex.Clr1.B = ig.Clr1[2] / 255f;
                            Vertex.Clr1.A = ig.Clr1[3] / 255f;
                            break;
                        default:
                            Console.WriteLine("To be implemented: " + Attr.Name);
                            break;
                    }
                }
                Vertices.Add(Vertex);
            }
            

            return Vertices.ToArray();
        }


        /// <summary>
        /// Returns display list for <see cref="HSD_POBJ"/>
        /// </summary>
        /// <param name="Polygon"></param>
        /// <returns></returns>
        public static GXDisplayList GetDisplayList(HSD_POBJ Polygon)
        {
            return new GXDisplayList(Polygon.DisplayListBuffer, Polygon.VertexAttributes);
        }

        /// <summary>
        /// Gets decoded vertices for <see cref="HSD_POBJ"/>
        /// </summary>
        /// <param name="Polygon"></param>
        /// <returns></returns>
        public static GXVertex[] GetDecodedVertices(HSD_POBJ Polygon)
        {
            return GetDecodedVertices(GetDisplayList(Polygon), Polygon);
        }

        /// <summary>
        /// Reads the vertex buffer into a more accessable format : <see cref="GXVertex"/>
        /// </summary>
        /// <param name="DisplayList">Display list belonging to given PBOJ</param>
        /// <param name="Polygon"><see cref="HSD_POBJ"/> the the display list belong to</param>
        /// <returns>Array of <see cref="GXVertex"/></returns>
        private static GXVertex[] GetDecodedVertices(GXDisplayList DisplayList, HSD_POBJ Polygon)
        {
            // Create Vertex List
            List<GXVertex> Vertices = new List<GXVertex>();

            // Read through the Display Lists
            foreach (GXPrimitiveGroup pg in DisplayList.Primitives)
            {
                Vertices.AddRange(GetDecodedVertices(pg, Polygon.VertexAttributes));
            }

            return Vertices.ToArray();
        }

        private static float[] Read(HSDReader d, GXCompType type, int size)
        {
            switch (type)
            {
                case GXCompType.UInt16: size /= 2; break;
                case GXCompType.Int16: size /= 2; break;
                case GXCompType.Float: size /= 4; break;
            }

            float[] a = new float[size];

            switch (type)
            {
                case GXCompType.UInt8:
                    for (int i = 0; i < size; i++)
                        a[i] = d.ReadByte();
                    break;
                case GXCompType.Int8:
                    for (int i = 0; i < size; i++)
                        a[i] = d.ReadSByte();
                    break;
                case GXCompType.UInt16:
                    for (int i = 0; i < size; i++)
                        a[i] = d.ReadUInt16();
                    break;
                case GXCompType.Int16:
                    for (int i = 0; i < size; i++)
                        a[i] = d.ReadInt16();
                    break;
                case GXCompType.Float:
                    for (int i = 0; i < size; i++)
                        a[i] = d.ReadSingle();
                    break;
                default:
                    for (int i = 0; i < size; i++)
                        a[i] = d.ReadByte();
                    break;
            }

            return a;
        }
    }
}
