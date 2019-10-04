using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace HSDRawViewer.Rendering
{
    public class TextureManager
    {
        public int TextureCount => Textures.Count;

        private List<int> Textures = new List<int>();
        private List<Vector2> TextureSizes = new List<Vector2>();

        public int Get(int index)
        {
            return Textures[index];
        }

        public void Add(byte[] rgba, int width, int height)
        {
            int texid;
            GL.GenTextures(1, out texid);

            GL.BindTexture(TextureTarget.Texture2D, texid);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, rgba);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 1);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            
            Textures.Add(texid);
            TextureSizes.Add(new Vector2(width, height));
        }

        public void ClearTextures()
        {
            foreach(var tex in Textures)
            {
                GL.DeleteTexture(tex);
            }
            Textures.Clear();
        }
        
        private static void RenderCheckerBack(int sizeOfChecker, float width, float height)
        {
            float wAmt = width / sizeOfChecker;
            float hAmt = height / sizeOfChecker;
            for (int h = 0; h < hAmt * 2; h++)
            {
                for (int w = 0; w < wAmt * 2; w++)
                {
                    if ((w + h) % 2 == 0)
                        GL.Color3(0.7f, 0.7f, 0.7f);
                    else
                        GL.Color3(0.5f, 0.5f, 0.5f);
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex2(w * sizeOfChecker / width - 1, 1 - h * sizeOfChecker / height);
                    GL.Vertex2((w + 1) * sizeOfChecker / width - 1, 1 - h * sizeOfChecker / height);
                    GL.Vertex2((w + 1) * sizeOfChecker / width - 1, 1 - (h + 1) * sizeOfChecker / height);
                    GL.Vertex2(w * sizeOfChecker / width - 1, 1 - (h + 1) * sizeOfChecker / height);
                    GL.End();
                }
            }
        }

        public void RenderTexture(int index, int windowWidth, int windowHeight, bool actualSize)
        {
            RenderTexture(index, windowWidth, windowHeight, actualSize, (int)TextureSizes[index].X, (int)TextureSizes[index].Y);
        }
        
        public void RenderTexture(int index, int windowWidth, int windowHeight, bool actualSize, int actualWidth, int actualHeight)
        {
            GL.PushMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            RenderCheckerBack(64, windowWidth, windowHeight);

            GL.Enable(EnableCap.Texture2D);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Get(index));

            float w = windowWidth;
            float h = windowHeight;

            if (actualSize)
            {
                w = actualWidth / 2;
                h = actualHeight / 2;
            }
            else
            {
                if (windowHeight > windowWidth)
                {
                    var aspect = actualHeight / (float)actualWidth;
                    w = windowWidth;
                    h = windowWidth * aspect;
                }
                else
                {
                    var aspect = actualWidth / (float)actualHeight;
                    w = windowHeight * aspect;
                    h = windowHeight;
                }
            }

            w /= windowWidth;
            h /= windowHeight;
            
            GL.Color3(1f, 1f, 1f);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 1); GL.Vertex3(-w, -h, 0);
            GL.TexCoord2(1, 1); GL.Vertex3(w, -h, 0);
            GL.TexCoord2(1, 0); GL.Vertex3(w, h, 0);
            GL.TexCoord2(0, 0); GL.Vertex3(-w, h, 0);
            GL.End();

            GL.PopMatrix();
        }
    }
}
