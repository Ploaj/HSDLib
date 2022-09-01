using OpenTK.Mathematics;
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
        /// Gets interaction of ray and quad in model space
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="P3"></param>
        /// <param name="P4"></param>
        /// <returns></returns>
        public bool IntersectsQuad(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4)
        {
            return IntersectsQuad(P1, P2, P3, P4, out Vector3 temp);
        }

        /// <summary>
        /// Gets interaction of ray and quad in model space
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
        public bool CheckSphereHitDistance(Vector3 center, float radius, out float distance)
        {
            Vector3 m = Origin - center;
            float b = Vector3.Dot(m, -Direction);
            float c = Vector3.Dot(m, m) - radius * radius;

            distance = float.MaxValue;

            // Exit if r’s origin outside s (c > 0) and r pointing away from s (b > 0) 
            if (c > 0.0f && b > 0.0f) 
                return false;

            float discr = b * b - c;

            // A negative discriminant corresponds to ray missing sphere 
            if (discr < 0.0f) 
                return false;

            // Ray now found to intersect sphere, compute smallest t value of intersection
            distance = -b - (float)Math.Sqrt(discr);

            // If t is negative, ray started inside sphere so clamp t to zero 
            if (distance < 0.0f)
                distance = 0.0f;

            var q = Origin + distance * -Direction;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="rad"></param>
        /// <param name="closest"></param>
        /// <returns></returns>
        public bool CheckSphereHitIntersection(Vector3 center, float radius, out Vector3 intersection)
        {
            Vector3 m = Origin - center;
            float b = Vector3.Dot(m, -Direction);
            float c = Vector3.Dot(m, m) - radius * radius;

            intersection = Vector3.Zero;
            var distance = float.MaxValue;

            // Exit if r’s origin outside s (c > 0) and r pointing away from s (b > 0) 
            if (c > 0.0f && b > 0.0f)
                return false;

            float discr = b * b - c;

            // A negative discriminant corresponds to ray missing sphere 
            if (discr < 0.0f)
                return false;

            // Ray now found to intersect sphere, compute smallest t value of intersection
            distance = -b - (float)Math.Sqrt(discr);

            // If t is negative, ray started inside sphere so clamp t to zero 
            if (distance < 0.0f)
                distance = 0.0f;

            intersection = Origin + distance * -Direction;

            return true;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="hit"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public bool CheckTriangleHit(Vector3 v0, Vector3 v1, Vector3 v2, ref Vector3 hit, out float depth)
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
