using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Modlee
{
    public class DrawTools
    {
        public static void DrawFloor(Matrix4 mvpMatrix)
        {
            float scale = 10;

            GL.Color3(Color.White);
            GL.LineWidth(1f);
            
            {
                GL.Begin(PrimitiveType.Lines);
                for (var i = -scale / 2; i <= scale / 2; i++)
                {
                    if (i != 0)
                    {
                        GL.Vertex3(-scale, 0f, i * 2);
                        GL.Vertex3(scale, 0f, i * 2);
                        GL.Vertex3(i * 2, 0f, -scale);
                        GL.Vertex3(i * 2, 0f, scale);
                    }
                }
                GL.End();
            }
            
            {
                GL.Disable(EnableCap.DepthTest);
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(Color.White);
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex3(-scale, 0f, 0);
                GL.Vertex3(scale, 0f, 0);
                GL.Vertex3(0, 0f, -scale);
                GL.Vertex3(0, 0f, scale);
                GL.End();
                GL.Enable(EnableCap.DepthTest);

                GL.Disable(EnableCap.DepthTest);
                GL.Color3(Color.LightGray);
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex3(0, 5, 0);
                GL.Vertex3(0, 0, 0);

                GL.Color3(Color.OrangeRed);
                GL.Vertex3(0f, 0f, 0);
                GL.Color3(Color.OrangeRed);
                GL.Vertex3(5f, 0f, 0);

                GL.Color3(Color.Olive);
                GL.Vertex3(0, 0f, 0f);
                GL.Color3(Color.Olive);
                GL.Vertex3(0, 0f, 5f);

                GL.End();
            }

            GL.Enable(EnableCap.DepthTest);
        }
    }
}
