using HSDRaw.Common;
using HSDRaw.GX;
using System;
using System.Collections.Generic;

namespace HSDRaw.Tools
{
    public class GX_VertexAttributeAccessor
    {
        /// <summary>
        /// Reads the vertex buffer into a more accessable format : <see cref="GXVertex"/>
        /// </summary>
        /// <param name="DisplayList">Display list belonging to given PBOJ</param>
        /// <param name="Polygon"><see cref="HSD_POBJ"/> the the display list belong to</param>
        /// <returns>Array of <see cref="GXVertex"/></returns>
        public static GX_Vertex[] GetDecodedVertices(GX_DisplayList DisplayList, HSD_POBJ Polygon)
        {
            // Create Vertex List
            List<GX_Vertex> Vertices = new List<GX_Vertex>();

            // Read through the Display Lists
            foreach (GX_PrimitiveGroup pg in DisplayList.Primitives)
            {
                Vertices.AddRange(GetDecodedVertices(pg, Polygon.Attributes));
            }

            return Vertices.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PrimitiveGroup"></param>
        /// <param name="Attributes"></param>
        /// <returns></returns>
        private static GX_Vertex[] GetDecodedVertices(GX_PrimitiveGroup PrimitiveGroup, GX_Attribute[] Attributes)
        {
            // Create Vertex List
            List<GX_Vertex> Vertices = new List<GX_Vertex>();

            // Create accessors for the buffers
            Dictionary<GXAttribName, HSDAccessor> attributeToBuffer = new Dictionary<GXAttribName, HSDAccessor>();
            foreach(var attr in Attributes)
            {
                if(attr.Buffer != null)
                {
                    HSDAccessor a = new HSDAccessor();
                    a._s = new HSDStruct(attr.Buffer);
                    attributeToBuffer.Add(attr.AttributeName, a);
                }
            }

            // Decode
            foreach (GX_IndexGroup ig in PrimitiveGroup.Indices)
            {
                GX_Vertex Vertex = new GX_Vertex();
                for (int i = 0; i < Attributes.Length; i++)
                {
                    var attribute = Attributes[i];
                    HSDAccessor accessor = null;

                    if(attributeToBuffer.ContainsKey(attribute.AttributeName))
                        accessor = attributeToBuffer[attribute.AttributeName];

                    int index = ig.Indices[i];
                    float[] f = new float[0];
                    
                    if (accessor != null)
                    {
                        // VertexBuffer.Seek((uint)(attribute.Stride * index));
                        // attribute.CompType, attribute.Stride);
                        f = Read(accessor, attribute, index);
                    }

                    switch (attribute.AttributeName)
                    {
                        case GXAttribName.GX_VA_NULL:
                            break;
                        case GXAttribName.GX_VA_PNMTXIDX:
                            Vertex.PNMTXIDX = (ushort)index;
                            break;
                        case GXAttribName.GX_VA_TEX0MTXIDX:
                            Vertex.TEX0MTXIDX = (ushort)index;
                            break;
                        case GXAttribName.GX_VA_POS:
                            Vertex.POS.X = f[0] / (float)Math.Pow(2, attribute.Scale);
                            Vertex.POS.Y = f[1] / (float)Math.Pow(2, attribute.Scale);
                            Vertex.POS.Z = f[2] / (float)Math.Pow(2, attribute.Scale);
                            break;
                        case GXAttribName.GX_VA_NRM:
                            Vertex.NRM.X = f[0] / (float)Math.Pow(2, attribute.Scale);
                            Vertex.NRM.Y = f[1] / (float)Math.Pow(2, attribute.Scale);
                            Vertex.NRM.Z = f[2] / (float)Math.Pow(2, attribute.Scale);
                            break;
                        case GXAttribName.GX_VA_TEX0:
                            Vertex.TEX0.X = f[0] / (float)Math.Pow(2, attribute.Scale);
                            Vertex.TEX0.Y = f[1] / (float)Math.Pow(2, attribute.Scale);
                            break;
                        case GXAttribName.GX_VA_TEX1:
                            Vertex.TEX1.X = f[0] / (float)Math.Pow(2, attribute.Scale);
                            Vertex.TEX1.Y = f[1] / (float)Math.Pow(2, attribute.Scale);
                            break;
                        case GXAttribName.GX_VA_CLR0:
                            if (attribute.AttributeType == GXAttribType.GX_DIRECT)
                            {
                                Vertex.CLR0.R = ig.Clr0[0] / 255f;
                                Vertex.CLR0.G = ig.Clr0[1] / 255f;
                                Vertex.CLR0.B = ig.Clr0[2] / 255f;
                                Vertex.CLR0.A = ig.Clr0[3] / 255f;
                            }
                            else
                            {
                                f = ReadClr(accessor, attribute, index);
                                Vertex.CLR0.R = f[0];
                                Vertex.CLR0.G = f[1];
                                Vertex.CLR0.B = f[2];
                                Vertex.CLR0.A = f[3];
                            }
                            break;
                        case GXAttribName.GX_VA_CLR1:
                            if (attribute.AttributeType == GXAttribType.GX_DIRECT)
                            {
                                Vertex.CLR1.R = ig.Clr1[0] / 255f;
                                Vertex.CLR1.G = ig.Clr1[1] / 255f;
                                Vertex.CLR1.B = ig.Clr1[2] / 255f;
                                Vertex.CLR1.A = ig.Clr1[3] / 255f;
                            }else
                            {
                                f = ReadClr(accessor, attribute, index);
                                Vertex.CLR1.R = f[0];
                                Vertex.CLR1.G = f[1];
                                Vertex.CLR1.B = f[2];
                                Vertex.CLR1.A = f[3];
                            }
                            break;
                        default:
                            Console.WriteLine("To be implemented: " + attribute.AttributeName);
                            break;
                    }
                }
                Vertices.Add(Vertex);
            }
            
            return Vertices.ToArray();
        }

        private static float[] ReadClr(HSDAccessor accessor, GX_Attribute attribute, int index)
        {
            var c = new float[4] { 1f, 1f, 1f, 1f};

            int offset = index * attribute.Stride;

            // TODO: are these in the right order?
            switch (attribute.CompType)
            {
                case GXCompType.RGB565:
                    var pixel = accessor._s.GetInt16(offset);
                    c[0] = ((((pixel >> 0) & 0x1F) << 3) & 0xff) / 255f;
                    c[1] = ((((pixel >> 5) & 0x3F) << 2) & 0xff) / 255f;
                    c[2] = ((((pixel >> 11) & 0x1F) << 3) & 0xff) / 255f;
                    break;
                case GXCompType.RGB8:
                    c[0] = accessor._s.GetByte(offset) / 255f;
                    c[1] = accessor._s.GetByte(offset + 1) / 255f;
                    c[2] = accessor._s.GetByte(offset + 2) / 255f;
                    break;
                case GXCompType.RGBA4: 
                    c[0] = (accessor._s.GetByte(offset) >> 4) * 16 / 255f;
                    c[1] = (accessor._s.GetByte(offset) & 0xF) * 16 / 255f;
                    c[2] = (accessor._s.GetByte(offset + 1) >> 4) * 16 / 255f;
                    c[4] = (accessor._s.GetByte(offset + 1) & 0xF) * 16 / 255f;
                    break;
                case GXCompType.RGBA6: //TODO: this is approximate
                    var p = accessor._s.GetInt32(offset) & 0xFFFFFF;
                    c[0] = ((p >> 18) & 0x3F) / 0x3F;
                    c[1] = ((p >> 12) & 0x3F) / 0x3F;
                    c[2] = ((p >> 6) & 0x3F) / 0x3F;
                    c[3] = ((p) & 0x3F) / 0x3F;
                    break;
                case GXCompType.RGBA8:
                    c[0] = accessor._s.GetByte(offset) / 255f;
                    c[1] = accessor._s.GetByte(offset + 1) / 255f;
                    c[2] = accessor._s.GetByte(offset + 2) / 255f;
                    c[2] = accessor._s.GetByte(offset + 3) / 255f;
                    break;
                case GXCompType.RGBX8:
                    c[0] = accessor._s.GetByte(offset) / 255f;
                    c[1] = accessor._s.GetByte(offset + 1) / 255f;
                    c[2] = accessor._s.GetByte(offset + 2) / 255f;
                    c[2] = accessor._s.GetByte(offset + 3) / 255f;
                    break;
            }

            return c;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessor"></param>
        /// <param name="attribute"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static float[] Read(HSDAccessor accessor, GX_Attribute attribute, int index)
        {
            var size = attribute.Stride;

            switch (attribute.CompType)
            {
                case GXCompType.UInt16: size /= 2; break;
                case GXCompType.Int16: size /= 2; break;
                case GXCompType.Float: size /= 4; break;
            }

            float[] a = new float[size];

            int offset = attribute.Stride * index;

            switch (attribute.CompType)
            {
                case GXCompType.UInt8:
                    for (int i = 0; i < size; i++)
                        a[i] = accessor._s.GetByte(offset + i); //d.ReadByte();
                    break;
                case GXCompType.Int8:
                    for (int i = 0; i < size; i++)
                        a[i] = (sbyte)accessor._s.GetByte(offset + i); //d.ReadSByte();
                    break;
                case GXCompType.UInt16:
                    for (int i = 0; i < size; i++)
                        a[i] = (ushort)accessor._s.GetInt16(offset + (i * 2));// d.ReadUInt16();
                    break;
                case GXCompType.Int16:
                    for (int i = 0; i < size; i++)
                        a[i] = accessor._s.GetInt16(offset + (i * 2));//d.ReadInt16();
                    break;
                case GXCompType.Float:
                    for (int i = 0; i < size; i++)
                        a[i] = accessor._s.GetFloat(offset + (i * 4));//d.ReadSingle();
                    break;
                default:
                    for (int i = 0; i < size; i++)
                        a[i] = accessor._s.GetByte(offset + i); //d.ReadByte();
                    break;
            }

            return a;
        }
    }
}
