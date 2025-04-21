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
            GX.GXVector3[] verts = coll.Vertices;
            KAR_CollisionTriangle[] tris = coll.Triangles;

            foreach (KAR_CollisionTriangle tri in tris)
            {
                // calculate surface normal
                Vector3 v0 = GXTranslator.toVector3(verts[tri.V1]);
                Vector3 v1 = GXTranslator.toVector3(verts[tri.V2]);
                Vector3 v2 = GXTranslator.toVector3(verts[tri.V3]);

                Vector3 faceNrm = Vector3.Cross(v1 - v0, v2 - v0).Normalized();

                tri.Flags &= ~(KCCollFlag.Ceiling | KCCollFlag.Floor | KCCollFlag.Wall);

                // guess flag
                if (faceNrm.Y > 0.5f)
                    tri.Flags |= KCCollFlag.Ceiling;
                else
                if (faceNrm.Y <= 0.5f && faceNrm.Y > -0.5f)
                    tri.Flags |= KCCollFlag.Wall;
                else
                    tri.Flags |= KCCollFlag.Floor;
            }

            coll.Triangles = tris;
        }
    }
}
