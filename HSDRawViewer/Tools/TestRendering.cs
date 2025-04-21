using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Renderers;
using HSDRawViewer.Rendering.Widgets;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HSDRawViewer.Tools
{
    public class TestRendering : IDrawableInterface
    {
        public DrawOrder DrawOrder => DrawOrder.First;

        private readonly TranslationWidget w = new();
        private readonly GLTextRenderer text = new();

        private Matrix4 Transform = Matrix4.Identity;

        private class Triangle
        {
            public bool Collided = false;
            public Vector3 P1;
            public Vector3 P2;
            public Vector3 P3;
        }

        private readonly List<Triangle> Triangles = new();

        /// <summary>
        /// 
        /// </summary>
        public TestRendering()
        {
            w.Transform = Transform;
            Triangles.Add(new Triangle()
            {
                P1 = new Vector3(-100, 0, 100),
                P2 = new Vector3(100, 0, -100),
                P3 = new Vector3(0, 100, 0),
            });

            //var bb = new BoundingBox(new Vector3(-10, 5, -10), new Vector3(10,25, 10));

            //Vector3 triangleP1 = new Vector3(-10, 0, 10);
            //Vector3 triangleP2 = new Vector3(10, 0, 0);
            //Vector3 triangleP3 = new Vector3(0, 10, 0);

            //bool intersection = bb.Intersects(triangleP1, triangleP2, triangleP3);

            //Console.WriteLine(intersection); // This should print true

            w.TransformUpdated += (t) =>
            {
                Transform = t;

                Vector3 center = Vector3.TransformPosition(Vector3.Zero, Transform);
                BoundingBox bb = new(center + new Vector3(-10, -10, -10), center + new Vector3(10, 10, 10));

                // check triangle collisions
                foreach (Triangle tri in Triangles)
                {
                    tri.Collided = bb.Intersects(tri.P1, tri.P2, tri.P3);
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void GLInit()
        {
            text.InitializeRender(@"Consolas.bff");
        }

        /// <summary>
        /// 
        /// </summary>
        public void GLFree()
        {
            text.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            DrawShape.DrawBox(System.Drawing.Color.White, Transform, -10, -10, -10, 10, 10, 10);

            GL.Begin(PrimitiveType.Triangles);
            foreach (Triangle tri in Triangles)
            {
                GL.Color3(tri.Collided ? Vector3.One : Vector3.UnitX);

                GL.Vertex3(tri.P1);
                GL.Vertex3(tri.P2);
                GL.Vertex3(tri.P3);
            }
            GL.End();

            w.Render(cam, text);

            //var right = cam.ModelViewMatrix.Row0;
            //var up = cam.ModelViewMatrix.Row1;
            text.RenderText(w.PickPoint.ToString(), cam.RenderWidth, cam.RenderHeight, 0, 16);
            text.RenderText(w.CenterSquare.ToString(), cam.RenderWidth, cam.RenderHeight, 0, 32); ;
        }

        public void ScreenClick(MouseButtons button, PickInformation pick)
        {
        }

        public void ScreenDoubleClick(PickInformation pick)
        {
        }

        public void ScreenDrag(MouseEventArgs args, PickInformation pick, float deltaX, float deltaY)
        {
            if (args.Button == MouseButtons.Left)
                w.MouseDown(pick);
            else
                w.MouseUp();

            w.Drag(pick);
        }

        public void ScreenSelectArea(PickInformation start, PickInformation end)
        {
        }

        public void ViewportKeyPress(KeyEventArgs kbState)
        {
        }

        public bool FreezeCamera()
        {
            return w.Interacting;
        }
    }

}
