using OpenTK;
using System;
namespace HSDRawViewer.Rendering
{
    public class Math3D
    {
        public static double TwoPI { get; } = 2.0 * Math.PI;

        public static float DegToRad { get; } = (float)(Math.PI / 180f);
        
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
        public static Quaternion FromEulerAngles(float z, float y, float x)
        {
            {
                Quaternion xRotation = Quaternion.FromAxisAngle(Vector3.UnitX, x);
                Quaternion yRotation = Quaternion.FromAxisAngle(Vector3.UnitY, y);
                Quaternion zRotation = Quaternion.FromAxisAngle(Vector3.UnitZ, z);

                Quaternion q = (zRotation * yRotation * xRotation);

                //if (q.W < 0)
                //    q *= -1;

                //return xRotation * yRotation * zRotation;
                return q;
            }
        }


        /// <summary>
        /// Converts quaternion into euler angles in ZYX order
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Vector3 ToEulerAngles(Quaternion q)
        {
            Matrix4 mat = Matrix4.CreateFromQuaternion(q);
            float x, y, z;

            y = (float)Math.Asin(-Clamp(mat.M31, -1, 1));

            if (Math.Abs(mat.M31) < 0.99999)
            {
                x = (float)Math.Atan2(mat.M32, mat.M33);
                z = (float)Math.Atan2(mat.M21, mat.M11);
            }
            else
            {
                x = 0;
                z = (float)Math.Atan2(-mat.M12, mat.M22);
            }
            return new Vector3(x, y, z);
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
