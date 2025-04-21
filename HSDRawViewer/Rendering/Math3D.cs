using OpenTK.Mathematics;
using System;

namespace HSDRawViewer.Rendering
{
    public class Math3D
    {
        public static double TwoPI { get; } = 2.0 * Math.PI;

        public static float DegToRad { get; } = (float)(Math.PI / 180f);
        public static float RadToDeg { get; } = (float)(180f / Math.PI);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static float AngleToTan(float angle)
        {
            return (float)Math.Tan(angle * RadToDeg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tan"></param>
        /// <returns></returns>
        public static float TanToAngle(float tan)
        {
            return (float)Math.Atan(tan) * DegToRad;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static bool FastDistance(Vector3 p1, Vector3 p2, float distance)
        {
            return Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2) + Math.Pow(p1.Z - p2.Z, 2) < distance * distance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static bool FastDistance(Vector2 p1, Vector2 p2, float distance)
        {
            return Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2) < distance * distance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static float DistanceSquared(Vector3 p1, Vector3 p2)
        {
            return (float)(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2) + Math.Pow(p1.Z - p2.Z, 2));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="z"></param>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Quaternion EulerToQuat(float x, float y, float z)
        {
            return new Quaternion().FromEuler(x, y, z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eul"></param>
        /// <returns></returns>
        public static Matrix4 CreateMatrix4FromEuler(Vector3 eul)
        {
            return CreateMatrix4FromEuler(eul.X, eul.Y, eul.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eulerAngles"></param>
        /// <returns></returns>
        public static Matrix4 CreateMatrix4FromEuler(float X, float Y, float Z)
        {
            float sx = (float)Math.Sin(X);
            float cx = (float)Math.Cos(X);
            float sy = (float)Math.Sin(Y);
            float cy = (float)Math.Cos(Y);
            float sz = (float)Math.Sin(Z);
            float cz = (float)Math.Cos(Z);

            float M11 = cy * cz;
            float M12 = cy * sz;
            float M13 = -sy;
            float M21 = cz * sx * sy - cx * sz;
            float M22 = sz * sx * sy + cx * cz;
            float M23 = sx * cy;
            float M31 = cz * cx * sy + sx * sz;
            float M32 = sz * cx * sy - sx * cz;
            float M33 = cx * cy;

            return new Matrix4(
                M11, M12, M13, 0,
                M21, M22, M23, 0,
                M31, M32, M33, 0,
                0, 0, 0, 1
                );
        }


        /// <summary>
        /// Clamps value between a minimum and maximum value
        /// </summary>
        /// <param name="v"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static Vector3 CalculateSurfaceNormal(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            // Calculate two vectors lying on the plane of the triangle
            Vector3 vector1 = v2 - v1;
            Vector3 vector2 = v3 - v1;

            // Calculate the cross product of the two vectors to get the surface normal
            Vector3 normal = Vector3.Cross(vector1, vector2);

            // Normalize the resulting vector to obtain a unit normal vector
            normal.Normalize();

            return normal;
        }
    }
}
