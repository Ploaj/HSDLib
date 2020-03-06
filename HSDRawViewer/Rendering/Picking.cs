using OpenTK;
using System;

namespace HSDRawViewer.Rendering
{
    public class PickInformation
    {
        public Vector2 ScreenPoint { get; internal set; }

        public Vector3 Origin { get; internal set; }
        public Vector3 End { get; internal set; }

        public Vector3 Dir { get { return (Origin - End).Normalized(); } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public PickInformation(Vector2 screenPoint, Vector3 p1, Vector3 p2)
        {
            ScreenPoint = screenPoint;
            Origin = p1;
            End = p2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="closest"></param>
        /// <returns></returns>
        /// http://csharphelper.com/blog/2016/09/find-the-shortest-distance-between-a-point-and-a-line-segment-in-c/
        public static float GetDistanceToSegment(Vector2 pt, Vector2 p1, Vector2 p2, out Vector2 closest)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return (float)Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new Vector2(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new Vector2(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new Vector2(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return (float)Math.Sqrt(dx * dx + dy * dy);
        }


        public bool IntersectsQuad(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4)
        {
            Vector3 temp;
            return IntersectsQuad(P1, P2, P3, P4, out temp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="P3"></param>
        /// <param name="P4"></param>
        /// <returns></returns>
        public bool IntersectsQuad(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4, out Vector3 intersection)
        {
            var mid = (P1 + P2 + P3 + P4) / 4;

            var nrm = Math3D.CalculateSurfaceNormal(P1, P2, P3);

            intersection = GetPlaneIntersection(nrm, mid);
            
            Vector3 V1 = (P2 - P1).Normalized();
            Vector3 V2 = (P3 - P2).Normalized();
            Vector3 V3 = (P4 - P3).Normalized();
            Vector3 V4 = (P1 - P4).Normalized();
            Vector3 V5 = (intersection - P1).Normalized();
            Vector3 V6 = (intersection - P2).Normalized();
            Vector3 V7 = (intersection - P3).Normalized();
            Vector3 V8 = (intersection - P4).Normalized();
            if (Vector3.Dot(V1, V5) < 0.0) return false;
            if (Vector3.Dot(V2, V6) < 0.0) return false;
            if (Vector3.Dot(V3, V7) < 0.0) return false;
            if (Vector3.Dot(V4, V8) < 0.0) return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Norm"></param>
        /// <returns></returns>
        public Vector3 GetPlaneIntersection(Vector3 Norm, Vector3 Position)
        {
            Vector3 rayP = Origin;
            Vector3 rayD = Dir;
            Vector3 planeN = Norm;
            var d = Vector3.Dot(Position, -Norm);
            var t = -(d + rayP.Z * planeN.Z + rayP.Y * planeN.Y + rayP.X * planeN.X) / (rayD.Z * planeN.Z + rayD.Y * planeN.Y + rayD.X * planeN.X);
            return rayP + t * rayD;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="rad"></param>
        /// <param name="closest"></param>
        /// <returns></returns>
        public bool CheckSphereHit(Vector3 sphere, float rad, out Vector3 closest)
        {
            Vector3 dirToSphere = sphere - Origin;
            Vector3 vLineDir = (End - Origin).Normalized();
            float fLineLength = 100;

            float t = Vector3.Dot(dirToSphere, vLineDir);

            if (t <= 0.0f)
                closest = Origin;
            else if (t >= fLineLength)
                closest = End;
            else
                closest = Origin + vLineDir * t;

            return (Math.Pow(sphere.X - closest.X, 2)
                + Math.Pow(sphere.Y - closest.Y, 2)
                + Math.Pow(sphere.Z - closest.Z, 2) <= rad * rad);
        }


        /// <summary>
        /// Checks if selection point is near bounds
        /// </summary>
        /// <param name="selectionPoint"></param>
        /// <param name="boundMin"></param>
        /// <param name="boundMax"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool CheckBoundHit(Vector2 selectionPoint, Vector2 boundMin, Vector2 boundMax, float range)
        {
            Vector2 close;

            if (GetDistanceToSegment(selectionPoint, new Vector2(boundMin.X, boundMin.Y), new Vector2(boundMax.X, boundMin.Y), out close) < range)
                return true;
            if (GetDistanceToSegment(selectionPoint, new Vector2(boundMax.X, boundMin.Y), new Vector2(boundMax.X, boundMax.Y), out close) < range)
                return true;
            if (GetDistanceToSegment(selectionPoint, new Vector2(boundMin.X, boundMin.Y), new Vector2(boundMin.X, boundMax.Y), out close) < range)
                return true;
            if (GetDistanceToSegment(selectionPoint, new Vector2(boundMin.X, boundMax.Y), new Vector2(boundMax.X, boundMax.Y), out close) < range)
                return true;

            return false;
        }
        
        private float coPlanerThreshold = 0.7f; // Some threshold value that is application dependent
        private float lengthErrorThreshold = 1e-3f;

        public bool IntersectsLine(Vector3 start, Vector3 end)
        {
            Vector3 da = Dir;  // Unnormalized direction of the ray
            Vector3 db = end - start;
            Vector3 dc = start - Origin;

            if (Math.Abs(Vector3.Dot(dc, Vector3.Cross(da, db))) >= coPlanerThreshold) // Lines are not coplanar
                return false;

            float s = Vector3.Dot(Vector3.Cross(dc, db), Vector3.Cross(da, db)) / Vector3.Cross(da, db).LengthSquared;

            if (s >= 0.0f && s <= 1.0f)   // Means we have an intersection
            {
                Vector3 intersection = Origin + s * da;

                // See if this lies on the segment
                if ((intersection - start).LengthSquared + (intersection - end).LengthSquared <= (end - start).LengthSquared + lengthErrorThreshold)
                    return true;
            }

            return false;
        }
    }
}
