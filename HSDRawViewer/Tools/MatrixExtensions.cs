using HSDRaw.Common;
using OpenTK;

namespace HSDRawViewer.Tools
{
    public static class MatrixExtensions
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
    }
}
