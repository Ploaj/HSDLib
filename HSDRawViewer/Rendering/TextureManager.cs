using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace HSDRawViewer.Rendering
{
    public class TextureManager
    {
        public int TextureCount => Textures.Count;

        private List<int> Textures = new List<int>();

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
        }

        public void ClearTextures()
        {
            foreach(var tex in Textures)
            {
                GL.DeleteTexture(tex);
            }
        }
    }
}
