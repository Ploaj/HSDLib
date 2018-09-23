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
        public int BindCount;
        public int B1, B2, B3, B4;
        public float W1, W2, W3, W4;
        public const int Stride = (3 + 3 + 2 + 2 + 4 + 1 + 8) * 4;
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
        private GXVertexBuffer Attribute;
        private HSD_AttributeGroup AttributeGroup;

        public VertexAccessor(GXVertexBuffer Attribute, HSD_AttributeGroup Group)
        {
            this.Attribute = Attribute;
            this.AttributeGroup = Group;
        }

        public VertexDecoded[] GetDecodedVertexBuffers(List<HSD_POBJ> POBJS)
        {
            return null;
        }

        /// <summary>
        /// Reads the vertex buffer into a more accessable format : <see cref="GXVertex"/>
        /// </summary>
        /// <param name="Group">Attribute group used with the given display list</param>
        /// <param name="DisplayList">Display list belonging to given PBOJ</param>
        /// <param name="Bones">A list of the <see cref="HSD_JOBJ"/>'s that belong to this model in depth first order</param>
        /// <returns>Array of <see cref="GXVertex"/></returns>
        public static GXVertex[] GetDecodedVertices(GXDisplayList DisplayList, HSD_POBJ Polygon, HSD_JOBJ RootJOBJ)
        {
            // Create Vertex List
            List<GXVertex> Vertices = new List<GXVertex>();

            // Prepare vertex buffers for reading
            Dictionary<GXVertexBuffer, HSDReader> Buffers = new Dictionary<GXVertexBuffer, HSDReader>();
            foreach(GXVertexBuffer buffer in Polygon.VertexAttributes.Attributes)
            {
                Buffers.Add(buffer, buffer.DataBuffer == null ? null : new HSDReader(new MemoryStream(buffer.DataBuffer)));
            }

            List<HSD_JOBJ> JOBJS = RootJOBJ.DepthFirstList;

            // Read through the Display Lists
            foreach (GXPrimitiveGroup pg in DisplayList.Primitives)
            {
                foreach (GXIndexGroup ig in pg.Indices)
                {
                    GXVertex Vertex = new GXVertex();
                    for (int i = 0; i < Polygon.VertexAttributes.Attributes.Count; i++)
                    {
                        GXVertexBuffer Attr = Polygon.VertexAttributes.Attributes[i];
                        HSDReader VertexBuffer = Buffers[Attr];
                        int index = ig.Indices[i];
                        float[] f;
                        if(VertexBuffer != null)
                            VertexBuffer.Seek((uint)(Attr.Stride * index));
                        f = read3(VertexBuffer, Attr.CompType, Attr.Stride);
                        switch (Attr.Name)
                        {
                            case GXAttribName.GX_VA_PNMTXIDX:
                                int nodeid = index / 3;
                                if (Polygon.BindGroups.Elements.Length == 0) continue; //|| nodeid / 3 >= poly.BoneWeightList.Count
                                if (nodeid >= Polygon.BindGroups.Elements.Length)
                                {
                                    throw new Exception("Error Reading Joint Data");
                                }
                                HSD_JOBJWeight bw = Polygon.BindGroups.Elements[nodeid];
                                //Vertex.WeightInd = new float[6];
                                //Vertex.BoneInd = new int[6];
                                Vertex.BindCount = bw.JOBJs.Count;
                                for (int k = 0; k < bw.JOBJs.Count; k++)
                                {
                                    switch (k)
                                    {
                                        case 0:
                                            Vertex.B1 = JOBJS.IndexOf(bw.JOBJs[k]);
                                            Vertex.W1 = bw.Weights[k];
                                            break;
                                    }
                                    //Vertex.BoneInd[k] = Bones.IndexOf(bw.JOBJs[k]);
                                    //Vertex.WeightInd[k] = bw.Weights[k];
                                }
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
                            default:
                                Console.WriteLine("To be implemented: " + Attr.Name);
                                break;
                        }
                    }
                    Vertices.Add(Vertex);
                }
            }

            foreach(GXVertexBuffer b in Buffers.Keys)
            {
                if(Buffers[b] != null)
                    Buffers[b].Close();
            }

            return Vertices.ToArray();
        }

        private static float[] read3(HSDReader d, GXCompType type, int size)
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
