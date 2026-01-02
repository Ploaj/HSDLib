using HSDRaw.GX;
using OpenTK.Mathematics;

namespace HSDRawViewer.Extensions
{
    public static class GXExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static GXVector3 ToGXVector(this Vector3 v)
        {
            return new GXVector3(v.X, v.Y, v.Z);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 ToTKVector(this GXVector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static GXVector3 TransformPosition(this GXVector3 v, Matrix4 m)
        {
            return Vector3.TransformPosition(v.ToTKVector(), m).ToGXVector();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static GXVector3 TransformNormal(this GXVector3 v, Matrix4 m)
        {
            return Vector3.TransformNormal(v.ToTKVector(), m).ToGXVector();
        }
    }
}
