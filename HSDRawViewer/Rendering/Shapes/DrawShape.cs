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
            GL.Color4(1f, 1f, 1f, 1f);
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
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="c"></param>
        public static void DrawRectangle(float x, float y, float x2, float y2, Color c)
        {
            DrawRectangle(x, y, x2, y2, 0, 1, c);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="c"></param>
        public static void DrawRectangle(float x, float y, float x2, float y2, float z, Color c)
        {
            DrawRectangle(x, y, x2, y2, z, 1, c);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="c"></param>
        public static void DrawRectangle(float x, float y, float x2, float y2, float z, float thickness, Color c)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            
            GL.Color4(c);
            GL.Begin(PrimitiveType.Quads);

            GL.Vertex3(x, y, z);
            GL.Vertex3(x2, y, z);
            GL.Vertex3(x2, y2, z);
            GL.Vertex3(x, y2, z);

            GL.End();

            GL.LineWidth(thickness);
            GL.Color4(1f, 1f, 1f, 1f);
            GL.Begin(PrimitiveType.LineLoop);

            GL.Vertex3(x, y, z);
            GL.Vertex3(x2, y, z);
            GL.Vertex3(x2, y2, z);
            GL.Vertex3(x, y2, z);

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

            GL.MatrixMode(MatrixMode.Modelview);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="length"></param>
        /// <param name="angle"></param>
        public static void DrawAngleLine(Camera cam, Matrix4 transform, float length, float angle)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Disable(EnableCap.DepthTest);

            var arrowSize = length / 8f;

            var pos = Vector3.TransformPosition(Vector3.Zero, transform);

            var rot = Matrix4.CreateRotationX(-angle);
            var start = pos;
            var end = start + Vector3.TransformNormal(new Vector3(0, 0, length), rot);

            GL.LineWidth(2f);
            GL.Color3(Color.White);
            
            GL.Begin(PrimitiveType.Lines);
            
            GL.Vertex3(start);
            GL.Vertex3(end);

            GL.Vertex3(end);
            GL.Vertex3(start + Vector3.TransformNormal(new Vector3(0, -arrowSize, length - arrowSize), rot));

            GL.Vertex3(end);
            GL.Vertex3(start + Vector3.TransformNormal(new Vector3(0, arrowSize, length - arrowSize), rot));

            GL.End();
            
            GL.PopAttrib();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="length"></param>
        public static void DrawSakuraiAngle(Camera cam, Matrix4 transform, float length)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Disable(EnableCap.DepthTest);

            var pos = Vector3.TransformPosition(Vector3.Zero, transform);

            var arrowSize = length / 3;

            var campos = (cam.RotationMatrix * new Vector4(cam.Translation, 1)).Xyz;
            var world = Matrix4.LookAt(pos, campos, Vector3.UnitY).Inverted();

            GL.LineWidth(2f);
            GL.Color3(Color.White);

            GL.PushMatrix();
            GL.MultMatrix(ref world);
            DrawCircleOutline(Vector3.Zero, arrowSize, 16);
            GL.PopMatrix();

            GL.PopAttrib();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="precision"></param>
        private static void DrawCircleOutline(Vector3 center, float radius, uint precision)
        {
            float theta = 2.0f * (float)Math.PI / precision;
            float cosine = (float)Math.Cos(theta);
            float sine = (float)Math.Sin(theta);

            float x = radius;
            float y = 0;

            GL.Begin(PrimitiveType.LineStrip);
            for (int i = 0; i < precision + 1; i++)
            {
                GL.Vertex3(x + center.X, y + center.Y, center.Z);

                //apply the rotation matrix
                var temp = x;
                x = cosine * x - sine * y;
                y = sine * temp + cosine * y;
            }
            GL.End();
        }

    }
}