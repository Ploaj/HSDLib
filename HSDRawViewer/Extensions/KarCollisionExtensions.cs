using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.Rendering.GX;
using OpenTK.Mathematics;

namespace HSDRaw.AirRide.Gr
{
    public static class KarCollisionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        public static void CalculateCollisionFlags(this KAR_grCollisionNode coll)
        {
            var verts = coll.Vertices;
            var tris = coll.Triangles;

            foreach (var tri in tris)
            {
                // calculate surface normal
                var v0 = GXTranslator.toVector3(verts[tri.V1]);
                var v1 = GXTranslator.toVector3(verts[tri.V2]);
                var v2 = GXTranslator.toVector3(verts[tri.V3]);

                var faceNrm = Vector3.Cross(v1 - v0, v2 - v0).Normalized();

                tri.Flags &= ~0x7;

                // guess flag
                if (faceNrm.Y > 0.5f)
                    tri.Flags |= 0x04;
                else
                if (faceNrm.Y <= 0.5f && faceNrm.Y > -0.5f)
                    tri.Flags |= 0x02;
                else
                    tri.Flags |= 0x01;
            }

            coll.Triangles = tris;
        }
    }
}
