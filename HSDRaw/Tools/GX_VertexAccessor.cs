using HSDRaw.Common;
using HSDRaw.GX;
using System;
using System.Collections.Generic;

namespace HSDRaw.Tools
{
    public class GX_VertexAccessor
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
            
            // Decode
            foreach (GX_IndexGroup ig in PrimitiveGroup.Indices)
            {
                GX_Vertex Vertex = new GX_Vertex();
                for (int i = 0; i < Attributes.Length; i++)
                {
                    var attribute = Attributes[i];
                    
                    int index = ig.Indices[i];
                    float[] f = new float[4];

                    if(attribute.AttributeType != GXAttribType.GX_DIRECT)
                    {
                        var values = attribute.GetDecodedDataAt(index);

                        // convert to float
                        f = new float[values.Length];
                        for (int j = 0; j < f.Length; j++)
                            f[j] = (float)values[j];
                    }

                    switch (attribute.AttributeName)
                    {
                        case GXAttribName.GX_VA_NULL:
                            break;
                        case GXAttribName.GX_VA_PNMTXIDX:
                            if(attribute.AttributeType == GXAttribType.GX_DIRECT)
                                Vertex.PNMTXIDX = (ushort)index;
                            break;
                        case GXAttribName.GX_VA_TEX0MTXIDX:
                            if (attribute.AttributeType == GXAttribType.GX_DIRECT)
                                Vertex.TEX0MTXIDX = (ushort)index;
                            break;
                        case GXAttribName.GX_VA_TEX1MTXIDX:
                            if (attribute.AttributeType == GXAttribType.GX_DIRECT)
                                Vertex.TEX1MTXIDX = (ushort)index;
                            break;
                        case GXAttribName.GX_VA_POS:

                            if (attribute.AttributeType != GXAttribType.GX_DIRECT)
                            {
                                if (f.Length > 0)
                                    Vertex.POS.X = f[0];
                                if (f.Length > 1)
                                    Vertex.POS.Y = f[1];
                                if(f.Length > 2)
                                    Vertex.POS.Z = f[2];
                            }
                            break;
                        case GXAttribName.GX_VA_NRM:
                            if (attribute.AttributeType != GXAttribType.GX_DIRECT)
                            {
                                Vertex.NRM.X = f[0];
                                Vertex.NRM.Y = f[1];
                                Vertex.NRM.Z = f[2];
                            }
                            break;
                        case GXAttribName.GX_VA_NBT:
                            if (attribute.AttributeType != GXAttribType.GX_DIRECT)
                            {
                                Vertex.NRM.X = f[0];
                                Vertex.NRM.Y = f[1];
                                Vertex.NRM.Z = f[2];
                                Vertex.BITAN.X = f[3];
                                Vertex.BITAN.Y = f[4];
                                Vertex.BITAN.Z = f[5];
                                Vertex.TAN.X = f[6];
                                Vertex.TAN.Y = f[7];
                                Vertex.TAN.Z = f[8];
                            }
                            break;
                        case GXAttribName.GX_VA_TEX0:
                            if (attribute.AttributeType != GXAttribType.GX_DIRECT)
                            {
                                Vertex.TEX0.X = f[0];
                                Vertex.TEX0.Y = f[1];
                            }
                            break;
                        case GXAttribName.GX_VA_TEX1:
                            if (attribute.AttributeType != GXAttribType.GX_DIRECT)
                            {
                                Vertex.TEX1.X = f[0];
                                Vertex.TEX1.Y = f[1];
                            }
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

    }
}
