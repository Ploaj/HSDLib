using HSDRaw.Common;
using OpenTK;
using System;

namespace HSDRawViewer
{
    public static class OpenTKExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <returns></returns>
        public static Matrix4 ToTKMatrix(this HSD_Matrix4x3 mat)
        {
            return new Matrix4(
                mat.M11, mat.M21, mat.M31, 0,
                mat.M12, mat.M22, mat.M32, 0,
                mat.M13, mat.M23, mat.M33, 0,
                mat.M14, mat.M24, mat.M34, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static HSD_Matrix4x3 ToHsdMatrix(this Matrix4 mat)
        {
            return new HSD_Matrix4x3()
            {
                M11 = mat.M11,
                M21 = mat.M12,
                M31 = mat.M13,
                M12 = mat.M21,
                M22 = mat.M22,
                M32 = mat.M23,
                M13 = mat.M31,
                M23 = mat.M32,
                M33 = mat.M33,
                M14 = mat.M41,
                M24 = mat.M42,
                M34 = mat.M43,
            };
        }

        private static float ClampPI(float value)
        {
            return value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector3 ExtractRotationEuler(this Matrix4 m)
        {
            var v = Vector3.Zero;
            var dVar2 = new Vector2(m.M11, m.M12).LengthFast;
            if (dVar2 <= 1.0E-5)
            {
                v.X = (float)Math.Atan2(-m.M32, m.M22);
                v.Y = (float)Math.Atan2(-m.M13, dVar2);
                v.Z = 0;
            }
            else
            {
                v.X = (float)Math.Atan2(m.M23, m.M33);
                v.Y = (float)Math.Atan2(-m.M13, dVar2);
                v.Z = (float)Math.Atan2(m.M12, m.M11);
            }
            return v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param_1"></param>
        /// <param name="param_2"></param>
        /// <returns></returns>
        public static Quaternion FromEuler(this Quaternion quat, float X, float Y, float Z)
        {
            var dVar1 = Math.Cos(0.5f * X);
            var dVar2 = Math.Cos(0.5f * Y);
            var dVar3 = Math.Cos(0.5f * Z);
            var dVar4 = Math.Sin(0.5f * X);
            var dVar5 = Math.Sin(0.5f * Y);
            var dVar6 = Math.Sin(0.5f * Z);

            quat.W = (float)(dVar1 * (dVar2 * dVar3) + (dVar4 * (dVar5 * dVar6)));
            quat.X = (float)(dVar4 * (dVar2 * dVar3) - (dVar1 * (dVar5 * dVar6)));
            quat.Y = (float)(dVar3 * (dVar1 * dVar5) + (dVar6 * (dVar4 * dVar2)));
            quat.Z = (float)(dVar6 * (dVar1 * dVar2) - (dVar3 * (dVar4 * dVar5)));

            return quat;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="v"></param>
        /// <param name="axis"></param>
        public static void RotateAboutUnitAxis(ref this Vector3 v, float angle, Vector3 axis)
        {
            // The implementation is unnecessarily complex. the idea is to reduce the problem to the
            // case where the rotation axis is pointing along the z-axis by using two initial rotations,
            // then the new v is rotated about the z-axis by angle, and finally the first two rotations
            // are reversed.

            float len_axis_yz = (float)Math.Sqrt(axis.Y * axis.Y + axis.Z * axis.Z);
            float s = (float)Math.Sin(angle);
            float c = (float)Math.Cos(angle);
            float unit_axis_yz_y = 0;
            float unit_axis_yz_z = 0;
            float x, y, z;
            float x2, z2;
            float x3, y3;

            // rotation (1) about the x-axis: rotate everything such that the rotation axis lies in the xz-plane.
            // new v is then (x,y,z), new rotation axis is (b.x, 0, len_axis_yz)
            if (len_axis_yz > 0.0000000001f)
            {
                unit_axis_yz_y = axis.Z / len_axis_yz;
                unit_axis_yz_z = axis.Y / len_axis_yz;

                x = v.X;
                y = v.Y * unit_axis_yz_y - v.Z * unit_axis_yz_z;
                z = v.Y * unit_axis_yz_z + v.Z * unit_axis_yz_y;
            }
            else
            {
                x = v.X;
                y = v.Y;
                z = v.Z;
            }

            // rotation (2) about the y-axis: rotate everything such that the rotation axis aligns with the z-axis
            // new v is then (x2,y2,z2)
            x2 = x * len_axis_yz - z * axis.X;
            // y2 = y
            z2 = x * axis.X + z * len_axis_yz;

            // rotate by 'angle' about the z axis, which now aligns with the rotation axis
            x3 = x2 * c - y * s; // remember that y2=y
            y3 = x2 * s + y * c;
            // z3 = z2

            // opposite of rotation (2). We overwrite (x,y,z) with the resulting new v.
            x = x3 * len_axis_yz + z2 * axis.X; // remember that z3=z2
            y = y3;
            z = -x3 * axis.X + z2 * len_axis_yz;

            // opposite of rotation (1)
            if (len_axis_yz > 0.0000000001f)
            {
                v.X = x;
                v.Y = y * unit_axis_yz_y + z * unit_axis_yz_z;
                v.Z = -y * unit_axis_yz_z + z * unit_axis_yz_y;
            }
            else
            {
                v.X = x;
                v.Y = y;
                v.Z = z;
            }
        }
    }
}
