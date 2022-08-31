using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Renderers;
using HSDRawViewer.Rendering.Widgets;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HSDRawViewer.Tools
{
    public class TestRendering : IDrawableInterface
    {
        public DrawOrder DrawOrder => DrawOrder.First;

        private TranslationWidget w = new TranslationWidget();
        private GLTextRenderer text = new GLTextRenderer();

        private Matrix4 Transform = Matrix4.CreateRotationY(1f);

        public TestRendering()
        {
            w.Transform = Transform;

            w.TransformUpdated += (t) =>
            {
                Transform.Row3 = t.Row3;
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void GLInit()
        {
            text.InitializeRender(@"lib\Consolas.bff");
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

            w.Render(cam, text);

            var right = cam.ModelViewMatrix.Row0;
            var up = cam.ModelViewMatrix.Row1;
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
