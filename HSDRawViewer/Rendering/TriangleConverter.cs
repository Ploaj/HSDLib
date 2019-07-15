using System;
using System.Collections.Generic;
using HSDRaw.GX;

namespace HSDRawViewer.Rendering
{
    public class TriangleConverter
    {
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
                bool isEven = (index % 2 != 1);

                var vert1 = vertices[index - 2];
                var vert2 = isEven ? vertices[index] : vertices[index - 1];
                var vert3 = isEven ? vertices[index - 1] : vertices[index];

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
