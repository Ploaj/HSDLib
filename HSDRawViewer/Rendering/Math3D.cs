using OpenTK;
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
            var sx = (float)Math.Sin(X);
            var cx = (float)Math.Cos(X);
            var sy = (float)Math.Sin(Y);
            var cy = (float)Math.Cos(Y);
            var sz = (float)Math.Sin(Z);
            var cz = (float)Math.Cos(Z);

            var M11 = cy * cz;
            var M12 = cy * sz;
            var M13 = -sy;
            var M21 = cz * sx * sy - cx * sz;
            var M22 = sz * sx * sy + cx * cz;
            var M23 = sx * cy;
            var M31 = cz * cx * sy + sx * sz;
            var M32 = sz * cx * sy - sx * cz;
            var M33 = cx * cy;

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
        public static Vector3 CalculateSurfaceNormal(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var U = p2 - p1;
            var V = p3 - p1;

            var x = (U.Y * V.Z) - (U.Z * V.Y);

            var y = (U.Z * V.X) - (U.X * V.Z);

            var z = (U.X * V.Y) - (U.Y * V.X);

            return new Vector3(x, y, z);
        }
    }
}
