using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace HSDRawViewer.Rendering
{
    public class DrawShape
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Floor()
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            int size = 50;
            int space = 5;

            GL.LineWidth(1f);
            GL.Color3(Color.White);
            GL.Begin(PrimitiveType.Lines);

            for (int i = -size; i <= size; i += space)
            {
                GL.Vertex3(-size, 0, i);
                GL.Vertex3(size, 0, i);

                GL.Vertex3(i, 0, -size);
                GL.Vertex3(i, 0, size);
            }

            GL.End();
            GL.PopAttrib();
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="lats"></param>
        /// <param name="longs"></param>
        public static void DrawSphere(Matrix4 t, double r, int lats, int longs, Vector3 color, float alpha)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.PushMatrix();
            GL.MultMatrix(ref t);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Color4(color.X, color.Y, color.Z, alpha);

            int i, j;
            for (i = 0; i <= lats; i++)
            {
                double lat0 = Math.PI * (-0.5 + (double)(i - 1) / lats);
                double z0 = Math.Sin(lat0);
                double zr0 = Math.Cos(lat0);

                double lat1 = Math.PI * (-0.5 + (double)i / lats);
                double z1 = Math.Sin(lat1);
                double zr1 = Math.Cos(lat1);

                GL.Begin(PrimitiveType.QuadStrip);
                for (j = 0; j <= longs; j++)
                {
                    double lng = 2 * Math.PI * (j - 1) / longs;
                    double x = Math.Cos(lng);
                    double y = Math.Sin(lng);
                    //GL.Normal3(x * zr0, y * zr0, z0);
                    GL.Vertex3(x * zr0 * r, y * zr0 * r, z0 * r);
                    //GL.Normal3(x * zr1, y * zr1, z1);
                    GL.Vertex3(x * zr1 * r, y * zr1 * r, z1 * r);
                }
                GL.End();
            }

            GL.PopMatrix();
            GL.PopAttrib();
        }
    }
}