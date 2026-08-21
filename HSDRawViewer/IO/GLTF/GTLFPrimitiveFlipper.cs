using IONET.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.IO.GLTF
{
    internal static class GTLFPrimitiveFlipper
    {
        private static uint[] ConvertTriangleStrip(
            IReadOnlyList<int> source)
        {
            if (source.Count < 3)
                return Array.Empty<uint>();

            var result = new List<uint>(
                (source.Count - 2) * 3);

            for (int i = 0; i < source.Count - 2; i++)
            {
                uint a = (uint)source[i];
                uint b = (uint)source[i + 1];
                uint c = (uint)source[i + 2];

                if ((i & 1) == 0)
                {
                    // Original: a b c
                    result.Add(a);
                    result.Add(c);
                    result.Add(b);
                }
                else
                {
                    // Original: a c b
                    result.Add(a);
                    result.Add(b);
                    result.Add(c);
                }
            }

            return result.ToArray();
        }
        private static uint[] ConvertQuads(
            IReadOnlyList<int> source)
        {
            if (source.Count % 4 != 0)
                throw new InvalidOperationException(
                    "Quad index count must be divisible by 4.");

            var result = new uint[(source.Count / 4) * 6];

            int dst = 0;

            for (int i = 0; i < source.Count; i += 4)
            {
                uint a = (uint)source[i + 0];
                uint b = (uint)source[i + 1];
                uint c = (uint)source[i + 2];
                uint d = (uint)source[i + 3];

                // First triangle
                result[dst++] = a;
                result[dst++] = c;
                result[dst++] = b;

                // Second triangle
                result[dst++] = a;
                result[dst++] = d;
                result[dst++] = c;
            }

            return result;
        }

        private static uint[] ConvertTriangles(
            IReadOnlyList<int> source)
        {
            if (source.Count % 3 != 0)
                throw new InvalidOperationException(
                    "Triangle index count must be divisible by 3.");

            var result = new uint[source.Count];

            for (int i = 0; i < source.Count; i += 3)
            {
                result[i + 0] = (uint)source[i + 0];
                result[i + 1] = (uint)source[i + 2];
                result[i + 2] = (uint)source[i + 1];
            }

            return result;
        }
        private static uint[] ConvertTriangleFan(
            IReadOnlyList<int> source)
        {
            if (source.Count < 3)
                return Array.Empty<uint>();

            var result = new uint[(source.Count - 2) * 3];

            uint center = (uint)source[0];

            int dst = 0;

            for (int i = 1; i < source.Count - 1; i++)
            {
                uint b = (uint)source[i];
                uint c = (uint)source[i + 1];

                result[dst++] = center;
                result[dst++] = c;
                result[dst++] = b;
            }

            return result;
        }

        public static uint[] ConvertIndices(
            IOPolygon polygon)
        {
            var source = polygon.Indicies;

            switch (polygon.PrimitiveType)
            {
                case IOPrimitive.TRIANGLE:
                    return ConvertTriangles(source);

                case IOPrimitive.QUAD:
                    return ConvertQuads(source);

                case IOPrimitive.TRISTRIP:
                    return ConvertTriangleStrip(source);

                case IOPrimitive.TRIFAN:
                    return ConvertTriangleFan(source);

                case IOPrimitive.POINT:
                case IOPrimitive.LINE:
                case IOPrimitive.LINESTRIP:
                    return source
                        .Select(x => (uint)x)
                        .ToArray();

                default:
                    throw new NotSupportedException(
                        $"Primitive type {polygon.PrimitiveType} is not supported.");
            }
        }

    }
}
