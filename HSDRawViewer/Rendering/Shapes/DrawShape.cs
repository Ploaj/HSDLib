using HSDRaw.Common;
using HSDRawViewer.Rendering.Renderers;
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
        /// <param name="spline"></param>
        public static void RenderSpline(HSD_Spline spline, Color c1, Color c2)
        {
            GL.UseProgram(-1);

            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.LineWidth(2);
            GL.Begin(PrimitiveType.LineStrip);

            var points = spline.Points;
            var pointMax = spline.PointCount;

            for (int i = 0; i < points.Length; i++)
            {
                GL.Color3(Mix(c1, c2, i / (float)pointMax));
                GL.Vertex3(points[i].X, points[i].Y, points[i].Z);
            }

            GL.End();

            GL.Color3(1f, 1f, 1f);
            GL.PointSize(4f);
            GL.Begin(PrimitiveType.Points);
            foreach(var p in points)
                GL.Vertex3(p.X, p.Y, p.Z);
            GL.End();

            GL.PopAttrib();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="amt"></param>
        /// <returns></returns>
        private static Color Mix(Color a, Color b, float amt)
        {
            return Color.FromArgb(
                (int)(a.R + (b.R - a.R) * amt),
                (int)(a.G + (b.G - a.G) * amt),
                (int)(a.B + (b.B - a.B) * amt));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        public static void Line(Vector3 start, Vector3 end, Vector4 color, float thickness)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Color4(color);
            GL.LineWidth(thickness);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(start);
            GL.Vertex3(end);
            GL.End();

            GL.PopAttrib();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Floor(Color color, int size, int space)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.LineWidth(1f);
            GL.Color4(color);
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
        public static void DrawRectangle(RectangleF rect, Color c)
        {
            DrawRectangle(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height, 0, 1, c);
        }

        public static void DrawRectangle(RectangleF rect, float thickNess, Color c)
        {
            DrawRectangle(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height, 0, thickNess, c);
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

            GL.Vertex3(x, y2, z);
            GL.Vertex3(x2, y2, z);
            GL.Vertex3(x2, y, z);
            GL.Vertex3(x, y, z);

            GL.End();

            GL.LineWidth(thickness);
            GL.Color4(1f, 1f, 1f, 1f);
            GL.Begin(PrimitiveType.LineLoop);

            GL.Vertex3(x, y2, z);
            GL.Vertex3(x2, y2, z);
            GL.Vertex3(x2, y, z);
            GL.Vertex3(x, y, z);

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
        public static void DrawLedgeBox(float x, float y, float x2, float y2, Color color)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.Disable(EnableCap.DepthTest);

            GL.LineWidth(1);
            GL.Color3(color);
            GL.Begin(PrimitiveType.LineLoop);

            GL.Vertex3(0, y2, x);
            GL.Vertex3(0, y2, x2);
            GL.Vertex3(0, y, x2);
            GL.Vertex3(0, y, x);

            GL.End();

            GL.PopAttrib();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="thickness"></param>
        /// <param name="c"></param>
        public static void DrawECB(Vector3 topN, float minx, float miny, float maxx, float maxy, bool grounded)
        {
            if (grounded)
                miny = topN.Y;

            var midx = Math.Abs(minx - maxx) / 2;
            var midy = (miny + maxy) / 2;

            GL.PushAttrib(AttribMask.DepthBufferBit);
            GL.Disable(EnableCap.DepthTest);

            Line(topN, topN + new Vector3(0, 2, 0), Vector4.One, 3);
            Line(topN, topN + new Vector3(0, -2, 0), Vector4.One, 3);
            Line(topN, topN + new Vector3(0, 0, 2), Vector4.One, 3);
            Line(topN, topN + new Vector3(0, 0, -2), Vector4.One, 3);
            
            DrawDiamond(
                new Vector3(0, midy, topN.Z - midx),
                new Vector3(0, maxy, topN.Z), 
                new Vector3(0, midy, topN.Z + midx),
                new Vector3(0, miny, topN.Z),
                1,
                Color.Orange);

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
        public static void DrawDiamond(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float thickness, Color c)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Color4(c);
            GL.Begin(PrimitiveType.Quads);

            GL.Vertex3(p1);
            GL.Vertex3(p2);
            GL.Vertex3(p3);
            GL.Vertex3(p4);

            GL.End();

            GL.LineWidth(thickness);
            GL.Color4(1f, 1f, 1f, 1f);
            GL.Begin(PrimitiveType.LineLoop);

            GL.Vertex3(p1);
            GL.Vertex3(p2);
            GL.Vertex3(p3);
            GL.Vertex3(p4);

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
            for (i = 1; i <= lats; i++)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="precision"></param>
        public static void DrawCircle(Vector3 center, float radius, uint precision, Vector3 color)
        {
            GL.Begin(PrimitiveType.TriangleFan);

            GL.Color4(color.X, color.Y, color.Z, 1);
            GL.Vertex3(center);

            float twicePi = 2.0f * (float)Math.PI;
            float x = 0;
            float y = 0;

            for (int i = 0; i <= precision; i++)
            {
                GL.Color4(color.X, color.Y, color.Z, 0f);
                GL.Vertex3(
                        center.X + (radius * Math.Cos(i * twicePi / precision)),
                        center.Y + (radius * Math.Sin(i * twicePi / precision)),
                        center.Z
                );

            }

            GL.End();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="cz"></param>
        /// <param name="size"></param>
        public static void DrawBox(Color color, float cx, float cy, float cz, float size)
        {
            DrawBox(color, Matrix4.Identity, cx - size, cy - size, cz - size, cx + size, cy + size, cz + size);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="z2"></param>
        public static void DrawBox(Color color, Matrix4 transform, float x1, float y1, float z1, float x2, float y2, float z2)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.MultMatrix(ref transform);
            DrawBox(color, x1, y1, z1, x2, y2, z2);
            GL.PopMatrix();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="z2"></param>
        public static void DrawBox(Color color, float x1, float y1, float z1, float x2, float y2, float z2)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);
            
            GL.LineWidth(2f);
            GL.Color3(color);

            GL.Begin(PrimitiveType.Lines);


            GL.Vertex3(x1, y1, z1);
            GL.Vertex3(x2, y1, z1);

            GL.Vertex3(x1, y1, z1);
            GL.Vertex3(x1, y2, z1);

            GL.Vertex3(x1, y1, z1);
            GL.Vertex3(x1, y1, z2);
            

            GL.Vertex3(x2, y2, z2);
            GL.Vertex3(x1, y2, z2);

            GL.Vertex3(x2, y2, z2);
            GL.Vertex3(x2, y1, z2);

            GL.Vertex3(x2, y2, z2);
            GL.Vertex3(x2, y2, z1);


            GL.Vertex3(x2, y1, z1);
            GL.Vertex3(x2, y2, z1);

            GL.Vertex3(x2, y1, z1);
            GL.Vertex3(x2, y1, z2);

            GL.Vertex3(x2, y2, z1);
            GL.Vertex3(x1, y2, z1);


            GL.Vertex3(x1, y2, z2);
            GL.Vertex3(x1, y1, z2);

            GL.Vertex3(x1, y2, z2);
            GL.Vertex3(x1, y2, z1);

            GL.Vertex3(x1, y1, z2);
            GL.Vertex3(x2, y1, z2);



            GL.End();

            GL.PopAttrib();
        }

    }
}