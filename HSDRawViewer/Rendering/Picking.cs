using OpenTK;
using System;

namespace HSDRawViewer.Rendering
{
    public class PickInformation
    {
        public Vector2 ScreenPoint { get; internal set; }

        public Vector3 Origin { get; internal set; }
        public Vector3 End { get; internal set; }

        public Vector3 Direction { get { return (Origin - End).Normalized(); } }

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="P3"></param>
        /// <param name="P4"></param>
        /// <returns></returns>
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
            Vector3 rayD = Direction;
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
        public bool CheckSphereHit(Vector3 sphere, float rad, out float distance)
        {
            Vector3 difference = sphere - Origin;
            float differenceLengthSquared = difference.LengthSquared;
            float sphereRadiusSquared = rad * rad;
            float distanceAlongRay;
            if (differenceLengthSquared < sphereRadiusSquared)
            {
                distance = 0;
                return true;
            }
            Vector3 refDirection = Direction;
            Vector3.Dot(ref refDirection, ref difference, out distanceAlongRay);
            if (distanceAlongRay < 0)
            {
                distance = 0;
                return false;
            }
            float dist = sphereRadiusSquared + distanceAlongRay * distanceAlongRay - differenceLengthSquared;

            distance = dist;// distanceAlongRay - (float?)Math.Sqrt(dist);

            return (dist < 0) ? false : true;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool IntersectsLine(Vector3 start, Vector3 end)
        {
            Vector3 da = Direction;  // Unnormalized direction of the ray
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool CheckAABBHit(Vector3 center, float size, ref Vector3 hit)
        {
            return CheckAABBHit(center - new Vector3(size), center + new Vector3(size), ref hit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool CheckAABBHit(Vector3 start, Vector3 end, ref Vector3 hit)
        {
            return CheckLineBox(start, end, Origin, End, ref hit);
        }

        private const double Epsilon = 0.000001d;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rayOrigin"></param>
        /// <param name="rayDirection"></param>
        /// <param name="vert0"></param>
        /// <param name="vert1"></param>
        /// <param name="vert2"></param>
        /// <returns></returns>
        public bool CheckTriangleHit(Vector3 vert0, Vector3 vert1, Vector3 vert2, ref Vector3 hit)
        {
            Vector3 rayOrigin = Origin;
            Vector3 rayDirection = Direction; 

            var edge1 = vert1 - vert0;
            var edge2 = vert2 - vert0;

            var pvec = Vector3.Cross(rayDirection, edge2);

            var det = Vector3.Dot(edge1, pvec);

            if (det > -Epsilon && det < Epsilon)
            {
                return false;
            }

            var invDet = 1d / det;

            var tvec = rayOrigin - vert0;

            var u = Vector3.Dot(tvec, pvec) * invDet;

            if (u < 0 || u > 1)
            {
                return false;
            }

            var qvec = Vector3.Cross(tvec, edge1);

            var v = Vector3.Dot(rayDirection, qvec) * invDet;

            if (v < 0 || u + v > 1)
            {
                return false;
            }

            var t = Vector3.Dot(edge2, qvec) * invDet;

            hit = new Vector3((float)t, (float)u, (float)v);
            return true;
        }

        public bool CheckTriangleHit2(Vector3 v0, Vector3 v1, Vector3 v2, ref Vector3 hit, out float depth)
        {
            depth = float.MaxValue;
            var e1 = v1 - v0;
            var e2 = v2 - v0;
            var d = -Direction;
            var p = Origin;

            var h = Vector3.Cross(d, e2);

            var a = Vector3.Dot(e1, h);

            if (a > -0.00001 && a < 0.00001)
                return (false);

            var f = 1 / a;
            var s = p - v0;
            var u = f * (Vector3.Dot(s, h));

            if (u < 0.0 || u > 1.0)
                return (false);

            var q = Vector3.Cross(s, e1);
            var v = f * Vector3.Dot(d, q);

            if (v < 0.0 || u + v > 1.0)
                return (false);

            // at this stage we can compute t to find out where
            // the intersection point is on the line
            var t = f * Vector3.Dot(e2, q);


            if (t > 0.00001) // ray intersection
            {
                depth = t;
                hit = Origin - Direction * t;
                return (true);
            }
            else // this means that there is a line intersection
                 // but not a ray intersection
                return (false);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="B1"></param>
        /// <param name="B2"></param>
        /// <param name="L1"></param>
        /// <param name="L2"></param>
        /// <param name="Hit"></param>
        /// <returns></returns>
        public static bool CheckLineBox(Vector3 B1, Vector3 B2, Vector3 L1, Vector3 L2, ref Vector3 Hit)
        {
            if (L2.X < B1.X && L1.X < B1.X) return false;
            if (L2.X > B2.X && L1.X > B2.X) return false;
            if (L2.Y < B1.Y && L1.Y < B1.Y) return false;
            if (L2.Y > B2.Y && L1.Y > B2.Y) return false;
            if (L2.Z < B1.Z && L1.Z < B1.Z) return false;
            if (L2.Z > B2.Z && L1.Z > B2.Z) return false;
            if (L1.X > B1.X && L1.X < B2.X &&
                L1.Y > B1.Y && L1.Y < B2.Y &&
                L1.Z > B1.Z && L1.Z < B2.Z)
            {
                Hit = L1;
                return true;
            }
            if ((GetIntersection(L1.X - B1.X, L2.X - B1.X, L1, L2, ref Hit) && InBox(Hit, B1, B2, 1))
              || (GetIntersection(L1.Y - B1.Y, L2.Y - B1.Y, L1, L2, ref Hit) && InBox(Hit, B1, B2, 2))
              || (GetIntersection(L1.Z - B1.Z, L2.Z - B1.Z, L1, L2, ref Hit) && InBox(Hit, B1, B2, 3))
              || (GetIntersection(L1.X - B2.X, L2.X - B2.X, L1, L2, ref Hit) && InBox(Hit, B1, B2, 1))
              || (GetIntersection(L1.Y - B2.Y, L2.Y - B2.Y, L1, L2, ref Hit) && InBox(Hit, B1, B2, 2))
              || (GetIntersection(L1.Z - B2.Z, L2.Z - B2.Z, L1, L2, ref Hit) && InBox(Hit, B1, B2, 3)))
                return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fDst1"></param>
        /// <param name="fDst2"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="Hit"></param>
        /// <returns></returns>
        public static bool GetIntersection(float fDst1, float fDst2, Vector3 P1, Vector3 P2, ref Vector3 Hit)
        {
            if ((fDst1 * fDst2) >= 0.0f) return false;
            if (fDst1 == fDst2) return false;
            Hit = P1 + (P2 - P1) * (-fDst1 / (fDst2 - fDst1));
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Hit"></param>
        /// <param name="B1"></param>
        /// <param name="B2"></param>
        /// <param name="Axis"></param>
        /// <returns></returns>
        public static bool InBox(Vector3 Hit, Vector3 B1, Vector3 B2, int Axis)
        {
            if (Axis == 1 && Hit.Z > B1.Z && Hit.Z < B2.Z && Hit.Y > B1.Y && Hit.Y < B2.Y) return true;
            if (Axis == 2 && Hit.Z > B1.Z && Hit.Z < B2.Z && Hit.X > B1.X && Hit.X < B2.X) return true;
            if (Axis == 3 && Hit.X > B1.X && Hit.X < B2.X && Hit.Y > B1.Y && Hit.Y < B2.Y) return true;
            return false;
        }
    }
}
