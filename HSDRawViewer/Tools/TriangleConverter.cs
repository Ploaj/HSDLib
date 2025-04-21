using HSDRaw.GX;
using System.Collections.Generic;

namespace HSDRawViewer.Tools
{
    public class TriangleConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<GX_Vertex> QuadToList(List<GX_Vertex> input)
        {
            List<GX_Vertex> output = new();

            for (int i = 0; i < input.Count; i += 4)
            {
                output.Add(input[i]);
                output.Add(input[i + 1]);
                output.Add(input[i + 2]);

                output.Add(input[i + 2]);
                output.Add(input[i + 3]);
                output.Add(input[i]);
            }

            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<GX_Vertex> StripToList(List<GX_Vertex> input)
        {
            List<GX_Vertex> output = new();

            for (int index = 2; index < input.Count; index++)
            {
                bool isEven = index % 2 != 1;

                GX_Vertex vert1 = input[index - 2];
                GX_Vertex vert2 = isEven ? input[index] : input[index - 1];
                GX_Vertex vert3 = isEven ? input[index - 1] : input[index];

                if (!vert1.POS.Equals(vert2.POS) && !vert2.POS.Equals(vert3.POS) && !vert3.POS.Equals(vert1.POS))
                {
                    output.Add(vert3);
                    output.Add(vert2);
                    output.Add(vert1);
                }
            }

            return output;
        }

        /// <summary>
        /// Converts a list of quads into triangles
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="outVertices"></param>
        public static void QuadToList<T>(List<T> vertices, out List<T> outVertices)
        {
            outVertices = new List<T>();

            for (int index = 0; index < vertices.Count; index += 4)
            {
                outVertices.Add(vertices[index]);
                outVertices.Add(vertices[index + 1]);
                outVertices.Add(vertices[index + 2]);
                outVertices.Add(vertices[index + 1]);
                outVertices.Add(vertices[index + 3]);
                outVertices.Add(vertices[index + 2]);
            }
        }

        /// <summary>
        /// Converts a list of triangle strips into triangles
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="outVertices"></param>
        public static void StripToList(List<GX_Vertex> vertices, out List<GX_Vertex> outVertices)
        {
            outVertices = new List<GX_Vertex>();

            for (int index = 2; index < vertices.Count; index++)
            {
                bool isEven = index % 2 != 1;

                GX_Vertex vert1 = vertices[index - 2];
                GX_Vertex vert2 = isEven ? vertices[index] : vertices[index - 1];
                GX_Vertex vert3 = isEven ? vertices[index - 1] : vertices[index];

                if (vert1 != vert2
                    && vert2 != vert3
                    && vert3 != vert1)
                {
                    outVertices.Add(vert3);
                    outVertices.Add(vert2);
                    outVertices.Add(vert1);
                }
                else
                {
                    //Console.WriteLine("ignoring degenerate triangle");
                }
            }
        }
    }
}
