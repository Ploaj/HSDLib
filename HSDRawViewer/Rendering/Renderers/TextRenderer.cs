using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace HSDRawViewer.Rendering.Renderers
{
    public class GLTextRenderer : IDisposable
    {
        private bool Initialized = false;

        private TextureManager TextureManager;

        private int image_width;
        private int image_height;
        private int cell_width;
        private int cell_height;

        private byte bpp;
        private char char_offset;

        private byte[] char_widths;

        /// <summary>
        /// 
        /// </summary>
        public GLTextRenderer()
        {
            TextureManager = new TextureManager();
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadFont(string fontFile)
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string filePath = Path.Combine(currentDirectory, "lib\\" + fontFile);

            using FileStream s = new(filePath, FileMode.Open);
            using BinaryReader r = new(s);
            if (r.ReadByte() != 0xBF || r.ReadByte() != 0xF2)
                throw new InvalidDataException($"{fontFile} was not a valid bff file");

            image_width = r.ReadInt32();
            image_height = r.ReadInt32();

            cell_width = r.ReadInt32();
            cell_height = r.ReadInt32();

            bpp = r.ReadByte();
            char_offset = r.ReadChar();

            char_widths = r.ReadBytes(256);

            // TODO: support 24 and 32 bpp
            byte[] imageData = r.ReadBytes((int)(s.Length - s.Position));

            // extend image data
            byte[] final = new byte[image_width * image_height * 4];
            for (int i = 0; i < imageData.Length; i++)
            {
                final[i * 4] = 255;
                final[i * 4 + 1] = 255;
                final[i * 4 + 2] = 255;
                final[i * 4 + 3] = imageData[i];
            }

            // add texture
            TextureManager.Add(final, image_width, image_height);
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitializeRender(string fontFile)
        {
            TextureManager.ClearTextures();
            LoadFont(fontFile);
            Initialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="text"></param>
        /// <param name="worldSpace"></param>
        /// <param name="align"></param>
        /// <param name="dropShadow"></param>
        public void RenderText(Camera cam, string text, Matrix4 worldSpace, StringAlignment align = StringAlignment.Near, bool dropShadow = false)
        {
            if (!Initialized)
                return;

            Vector3 pos = cam.Project(worldSpace, Vector3.Zero);
            RenderText(cam, text, pos.X, pos.Y, align, dropShadow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private float MeasureText(string text)
        {
            int size = 0;

            for (int n = 0; n < text.Length; n++)
            {
                char idx = text[n];
                size += char_widths[idx];
            }

            return size;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void RenderText(Camera cam, string text, float x, float y, StringAlignment align = StringAlignment.Near, bool dropShadow = false)
        {
            RenderText(text, cam.RenderWidth, cam.RenderHeight, x, y, align, dropShadow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void RenderText(string text, int screenwidth, int screenheight, float x, float y, StringAlignment align = StringAlignment.Near, bool dropShadow = false)
        {
            if (!Initialized)
                return;

            Matrix4 mat = Matrix4.CreateOrthographicOffCenter(0, screenwidth, screenheight, 0, 0, 1);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadMatrix(ref mat);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Disable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.Texture2D);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureManager.GetGLID(0));

            GL.Begin(PrimitiveType.Quads);

            int RowPitch = image_width / cell_width;

            if (align == StringAlignment.Center)
            {
                x -= MeasureText(text) / 2;
            }
            if (align == StringAlignment.Far)
            {
                x -= MeasureText(text);
            }

            for (int n = 0; n < text.Length; n++)
            {
                char idx = text[n];
                int row = (idx - char_offset) / RowPitch;
                int col = (idx - char_offset) - (row * RowPitch);

                float coll_factor = cell_width / (float)image_width;
                float row_factor = cell_height / (float)image_height;

                float u = col * coll_factor;
                float v = row * row_factor;

                if (dropShadow)
                {
                    GL.Color3(Color.Black);

                    GL.TexCoord2(u, v);
                    GL.Vertex2(x + 1, y + 1);
                    GL.TexCoord2(u + coll_factor, v);
                    GL.Vertex2(x + cell_width + 1, y + 1);
                    GL.TexCoord2(u + coll_factor, v + row_factor);
                    GL.Vertex2(x + cell_width + 1, y + cell_height + 1);
                    GL.TexCoord2(u, v + row_factor);
                    GL.Vertex2(x + 1, y + cell_height + 1);
                }

                GL.Color3(Color.White);
                GL.TexCoord2(u, v);
                GL.Vertex2(x, y);
                GL.TexCoord2(u + coll_factor, v);
                GL.Vertex2(x + cell_width, y);
                GL.TexCoord2(u + coll_factor, v + row_factor);
                GL.Vertex2(x + cell_width, y + cell_height);
                GL.TexCoord2(u, v + row_factor);
                GL.Vertex2(x, y + cell_height);

                x += char_widths[idx];
            }

            GL.End();

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();

            GL.PopAttrib();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (TextureManager != null)
            {
                TextureManager.ClearTextures();
                TextureManager = null;
            }
        }
    }
}
