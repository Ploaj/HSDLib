using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;

namespace HSDRawViewer.Rendering
{
    public class TextureManager
    {
        public int TextureCount => Textures.Count;

        private List<int> Textures = new List<int>();
        private List<Vector2> TextureSizes = new List<Vector2>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int Get(int index)
        {
            return Textures[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        public void Swap(int index1, int index2)
        {
            var t = Textures[index1];
            Textures[index1] = Textures[index2];
            Textures[index2] = t;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        public int Add(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
             bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
             System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            
            IntPtr ptr = bmpData.Scan0;
            
            int bytes = bmpData.Stride * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes); bmp.UnlockBits(bmpData);

            return Add(rgbValues, bmp.Width, bmp.Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rgba"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>Index of Texture</returns>
        public int Add(byte[] rgba, int width, int height)
        {
            OpenTKResources.MakeCurrentDummy();

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
            return Textures.Count - 1;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearTextures()
        {
            foreach(var tex in Textures)
            {
                GL.DeleteTexture(tex);
            }
            Textures.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector2 GetTextureSize(int index)
        {
            if (index < TextureSizes.Count)
                return TextureSizes[index];

            return Vector2.Zero;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sizeOfChecker"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        /// <param name="actualSize"></param>
        public void RenderTexture(int index, int windowWidth, int windowHeight, bool actualSize)
        {
            RenderTexture(index, windowWidth, windowHeight, actualSize, (int)TextureSizes[index].X, (int)TextureSizes[index].Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public static void RenderCheckerBack(int windowWidth, int windowHeight)
        {
            GL.PushMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            RenderCheckerBack(64, windowWidth, windowHeight);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        /// <param name="actualSize"></param>
        /// <param name="actualWidth"></param>
        /// <param name="actualHeight"></param>
        public void RenderTexture(int index, int windowWidth, int windowHeight, bool actualSize, int actualWidth, int actualHeight)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Enable(EnableCap.Texture2D);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

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
            GL.PopAttrib();
        }
    }
}
