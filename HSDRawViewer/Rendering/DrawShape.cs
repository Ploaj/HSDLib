using OpenTK.Graphics.OpenGL;
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
    }
}
