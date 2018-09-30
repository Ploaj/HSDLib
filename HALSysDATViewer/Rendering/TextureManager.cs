using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using HSDLib.Common;
using HSDLib.Helpers;

namespace HALSysDATViewer.Rendering
{
    public class GLTexture
    {
        public int ID;

        public GLTexture(Bitmap Bitmap)
        {
            ID = CreateGlTextureFromBitmap(Bitmap);
        }

        public static GLTexture FromTOBJ(HSD_TOBJ tobj)
        {
            Bitmap B = null;
            if(tobj.ImageData != null)
            {
                if (tobj.Tlut != null)
                    B = TPL.ConvertFromTextureMelee(tobj.ImageData.Data, tobj.ImageData.Width, tobj.ImageData.Height, (int)tobj.ImageData.Format, tobj.Tlut.Data, tobj.Tlut.ColorCount, (int)tobj.Tlut.Format);
                else
                    B = TPL.ConvertFromTextureMelee(tobj.ImageData.Data, tobj.ImageData.Width, tobj.ImageData.Height, (int)tobj.ImageData.Format, null, 0, 0);

            }
            GLTexture T = new GLTexture(B);
            B.Dispose();
            GC.Collect();

            GL.BindTexture(TextureTarget.Texture2D, T.ID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GXTranslator.toWrapMode(tobj.WrapS));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GXTranslator.toWrapMode(tobj.WrapT));
            GL.BindTexture(TextureTarget.Texture2D, 0);

            return T;
        }

        public static int CreateGlTextureFromBitmap(Bitmap image)
        {
            int texID = GL.GenTexture();

            // Read the pixel data from the bitmap.
            GL.BindTexture(TextureTarget.Texture2D, texID);
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            image.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 1);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            return texID;
        }
    }

    public class TextureManager
    {
        public static Dictionary<HSD_TOBJ, GLTexture> Textures = new Dictionary<HSD_TOBJ, GLTexture>();

        public static GLTexture GetGLTexture(HSD_TOBJ tobj)
        {
            if (tobj == null) return null;
            if (Textures.ContainsKey(tobj))
                return Textures[tobj];

            GLTexture tex = GLTexture.FromTOBJ(tobj);
            Textures.Add(tobj, tex);
            return tex;
        }

        public static void Clear()
        {
            foreach(GLTexture t in Textures.Values)
            {
                GL.DeleteTexture(t.ID);
            }
            Textures.Clear();
        }

    }
}
