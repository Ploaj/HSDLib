using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Rendering.Shapes
{
    public class Capsule
    {
        public Vector3 Point1 { get; internal set; }

        public Vector3 Point2 { get; internal set; }

        public float Radius { get; internal set; }

        private float Height { get; set; }

        private Matrix4 Orientation;

        private readonly List<Vector3> Positions = new();

        private readonly List<int> Indices = new();

        private readonly int Segments = 16;

        private readonly int SubdivisionsHeight = 16;

        public Capsule(Vector3 p1, Vector3 p2, float rad)
        {
            SetParameters(p1, p2, rad);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="rad"></param>
        public void SetParam(Vector3 p1, Vector3 p2, float rad)
        {
            SetParameters(p1, p2, rad);
        }

        /// <summary>
        /// Sets the parameters and regenerates the capsule if necessary
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="rad"></param>
        public void SetParameters(Vector3 p1, Vector3 p2, float rad)
        {
            // Only regerenate capsule if something is different from last time
            if (p1 != Point1 || p2 != Point2 || rad != Radius)
            {
                Point1 = p1;
                Point2 = p2;
                Radius = rad;
                Height = (float)Math.Sqrt(Math3D.DistanceSquared(p1, p2));

                Orientation = Matrix4.Identity;
                if (p1 != p2)
                {
                    Vector3 dir = Vector3.Normalize(p2 - p1);
                    if (dir.X == 0 && dir.Z == 0)
                    {
                        Orientation = new Matrix4(Vector4.UnitX, new Vector4(dir), Vector4.UnitZ, Vector4.UnitW);
                    }
                    else
                    {
                        Vector3 axis = Vector3.Cross(Vector3.UnitY, dir);
                        float angle = (float)Math.Acos(Vector3.Dot(Vector3.UnitY, dir));
                        Orientation = Matrix4.CreateFromAxisAngle(axis, angle);
                    }
                }

                Orientation *= Matrix4.CreateTranslation(Point1);

                CreateCapsule();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateCapsule()
        {
            Positions.Clear();
            Indices.Clear();
            int ringsBody = SubdivisionsHeight + 1;
            int ringsTotal = SubdivisionsHeight + ringsBody;

            float bodyIncr = 1.0f / (ringsBody - 1);
            float ringIncr = 1.0f / (SubdivisionsHeight - 1);
            for (int r = 0; r < SubdivisionsHeight / 2; r++)
            {
                CalculateRing(Segments, (float)Math.Sin(Math.PI * r * ringIncr), (float)Math.Sin(Math.PI * (r * ringIncr - 0.5f)), -0.5f);
            }

            for (int r = 0; r < ringsBody; r++)
            {
                CalculateRing(Segments, 1.0f, 0.0f, r * bodyIncr - 0.5f);
            }

            for (int r = SubdivisionsHeight / 2; r < SubdivisionsHeight; r++)
            {
                CalculateRing(Segments, (float)Math.Sin(Math.PI * r * ringIncr), (float)Math.Sin(Math.PI * (r * ringIncr - 0.5f)), +0.5f);
            }

            for (int r = 0; r < ringsTotal - 1; r++)
            {
                for (int s = 0; s < Segments - 1; s++)
                {
                    Indices.Add(r * Segments + (s + 1));
                    Indices.Add(r * Segments + (s + 0));
                    Indices.Add((r + 1) * Segments + (s + 1));

                    Indices.Add((r + 1) * Segments + s);
                    Indices.Add((r + 1) * Segments + (s + 1));
                    Indices.Add(r * Segments + s);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="segments"></param>
        /// <param name="r"></param>
        /// <param name="y"></param>
        /// <param name="dy"></param>
        private void CalculateRing(int Segments, float r, float y, float dy)
        {
            double segIncr = 1.0 / (Segments - 1);

            for (int s = 0; s < Segments; s++)
            {
                float x = -(float)Math.Cos((Math.PI * 2) * s * segIncr) * r;
                float z = (float)Math.Sin((Math.PI * 2) * s * segIncr) * r;

                Positions.Add(new Vector3(Radius * x, Radius * y + Height * dy + Height / 2, Radius * z));
                //positions.push([radius * x, radius * y + height * dy, radius * z])
                //normals.push([x, y, z])

                //var u = (s * segIncr);
                //var v = 0.5 - ((Radius * y + height * dy) / (2.0 * Radius + height));
                //uvs.push([u, 1.0 - v]);
            }
        }

        public void Draw(Matrix4 transform, Vector4 color)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.MultMatrix(ref transform);
            GL.MultMatrix(ref Orientation);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Color4(color);
            GL.Begin(PrimitiveType.Triangles);
            foreach (int i in Indices)
                GL.Vertex3(Positions[i]);
            GL.End();

            /*GL.Disable(EnableCap.DepthTest);
            
            GL.Color3(1f, 0,0 );
            GL.LineWidth(5f);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(Vector3.Zero);
            GL.Vertex3(0, Height, 0);
            GL.End();*/

            GL.PopMatrix();
            GL.PopAttrib();
        }
    }
}
