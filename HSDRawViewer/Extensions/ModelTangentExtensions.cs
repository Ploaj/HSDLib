using IONET.Core.Model;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.Extensions
{
    public static class ModelTangentExtensions
    {
        public static Vector3 defaultTangent = new(1, 0, 0);

        public static Vector3 defaultBitangent = new(0, 1, 0);

        public static void CalculateTangentBitangent(this IOMesh mesh)
        {
            Vector3[] pos = mesh.Vertices.Select(e => new Vector3(e.Position.X, e.Position.Y, e.Position.Z)).ToArray();
            Vector3[] nrm = mesh.Vertices.Select(e => new Vector3(e.Normal.X, e.Normal.Y, e.Normal.Z)).ToArray();
            Vector2[] uvs = mesh.Vertices.Select(e => new Vector2(e.UVs[0].X, e.UVs[0].Y)).ToArray();

            List<int> indices = new();

            foreach (IOPolygon poly in mesh.Polygons)
            {
                poly.ToTriangles(mesh);
                if (poly.PrimitiveType == IOPrimitive.TRIANGLE)
                    indices.AddRange(poly.Indicies);
            }

            CalculateTangentsBitangents(pos, nrm, uvs, indices, out Vector3[] tan, out Vector3[] bitan);

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                mesh.Vertices[i].Tangent = new System.Numerics.Vector3(tan[i].X, tan[i].Y, tan[i].Z);
                mesh.Vertices[i].Binormal = new System.Numerics.Vector3(bitan[i].X, bitan[i].Y, bitan[i].Z);
            }
        }

        public static void CalculateTangentsBitangents(IList<Vector3> positions, IList<Vector3> normals, IList<Vector2> uvs, IList<int> indices, out Vector3[] tangents, out Vector3[] bitangents)
        {
            if (normals.Count != positions.Count)
                throw new ArgumentOutOfRangeException(nameof(normals), "Vector source lengths do not match.");

            if (uvs.Count != positions.Count)
                throw new ArgumentOutOfRangeException(nameof(uvs), "Vector source lengths do not match.");

            tangents = new Vector3[positions.Count];
            bitangents = new Vector3[positions.Count];

            // Calculate the vectors.
            for (int i = 0; i < indices.Count; i += 3)
            {
                GenerateTangentBitangent(positions[indices[i]], positions[indices[i + 1]], positions[indices[i + 2]],
                    uvs[indices[i]], uvs[indices[i + 1]], uvs[indices[i + 2]], out Vector3 tangent, out Vector3 bitangent);

                tangents[indices[i]] += tangent;
                tangents[indices[i + 1]] += tangent;
                tangents[indices[i + 2]] += tangent;

                bitangents[indices[i]] += bitangent;
                bitangents[indices[i + 1]] += bitangent;
                bitangents[indices[i + 2]] += bitangent;
            }

            // Even if the vectors are not zero, they may still sum to zero.
            for (int i = 0; i < tangents.Length; i++)
            {
                if (tangents[i].Length == 0.0f)
                    tangents[i] = defaultTangent;

                if (bitangents[i].Length == 0.0f)
                    bitangents[i] = defaultBitangent;
            }

            // Account for mirrored normal maps.
            for (int i = 0; i < bitangents.Length; i++)
            {
                // The default bitangent may be parallel to the normal vector.
                if (Vector3.Cross(bitangents[i], normals[i]).Length != 0.0f)
                    bitangents[i] = Orthogonalize(bitangents[i], normals[i]);
                bitangents[i] *= -1;
            }

            for (int i = 0; i < tangents.Length; i++)
            {
                tangents[i].Normalize();
                bitangents[i].Normalize();
            }
        }

        public static Vector3 Orthogonalize(Vector3 vectorToOrthogonalize, Vector3 source)
        {
            return Vector3.Normalize(vectorToOrthogonalize - source * Vector3.Dot(source, vectorToOrthogonalize));
        }
        public static void GenerateTangentBitangent(Vector3 v1, Vector3 v2, Vector3 v3,
                                                    Vector2 uv1, Vector2 uv2, Vector2 uv3,
                                                    out Vector3 tangent, out Vector3 bitangent)
        {
            Vector3 posA = v2 - v1;
            Vector3 posB = v3 - v1;

            Vector2 uvA = uv2 - uv1;
            Vector2 uvB = uv3 - uv1;

            float div = uvA.X * uvB.Y - uvB.X * uvA.Y;

            // Fix +/- infinity from division by zero.
            float r = 1.0f;
            if (div != 0)
                r = 1.0f / div;

            tangent = CalculateTangent(posA, posB, uvA, uvB, r);
            bitangent = CalculateBitangent(posA, posB, uvA, uvB, r);

            // Set zero vectors to arbitrarily chosen orthogonal vectors.
            // This prevents unwanted black areas when rendering.
            if (tangent.Length == 0.0f)
                tangent = defaultTangent;
            if (bitangent.Length == 0.0f)
                bitangent = defaultBitangent;
        }
        private static Vector3 CalculateBitangent(Vector3 posA, Vector3 posB, Vector2 uvA, Vector2 uvB, float r)
        {
            Vector3 bitangent;
            float tX = uvA.X * posB.X - uvB.X * posA.X;
            float tY = uvA.X * posB.Y - uvB.X * posA.Y;
            float tZ = uvA.X * posB.Z - uvB.X * posA.Z;
            bitangent = new Vector3(tX, tY, tZ) * r;
            return bitangent;
        }

        private static Vector3 CalculateTangent(Vector3 posA, Vector3 posB, Vector2 uvA, Vector2 uvB, float r)
        {
            Vector3 tangent;
            float sX = uvB.Y * posA.X - uvA.Y * posB.X;
            float sY = uvB.Y * posA.Y - uvA.Y * posB.Y;
            float sZ = uvB.Y * posA.Z - uvA.Y * posB.Z;
            tangent = new Vector3(sX, sY, sZ) * r;
            return tangent;
        }

    }
}
