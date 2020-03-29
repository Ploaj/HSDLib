using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Windows.Forms;

namespace HSDRawViewer.Rendering.Renderers
{
    public class GLTextRenderer
    {
        public static class TextSettings
        {
            public static int GlyphsPerLine = 16;
            public static int GlyphLineCount = 16;
            public static int GlyphWidth = 16;
            public static int GlyphHeight = 22;

            // Used to offset rendering glyphs to bitmap
            public static int AtlasOffsetX = -3, AtlassOffsetY = -1;
            public static int FontSize = 12;
            public static bool BitmapFont = false;
            public static string FontName = "Consolas";
        }

        private static TextureManager TextureManager;
        private static Font RenderFont = new Font(new FontFamily(TextSettings.FontName), TextSettings.FontSize);

        public static void RenderText(Camera cam, string text, Matrix4 worldSpace, StringAlignment align = StringAlignment.Near, bool dropShadow = false)
        {
            var pos = Vector3.TransformPosition(Vector3.Zero, worldSpace * cam.MvpMatrix);
            pos.Xy /= pos.Z;
            pos.Y *= -1;
            pos.X *= cam.RenderWidth / 2;
            pos.Y *= cam.RenderHeight / 2;
            pos.X += cam.RenderWidth / 2;
            pos.Y += cam.RenderHeight / 2;
            RenderText(cam, text, pos.X, pos.Y, align, dropShadow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public static void RenderText(Camera cam, string text, float x, float y, StringAlignment align = StringAlignment.Near, bool dropShadow = false)
        {
            if(TextureManager == null)
            {
                TextureManager = new TextureManager();
                GenerateFontImage();
            }

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            var mat = Matrix4.CreateOrthographicOffCenter(0, cam.RenderWidth, cam.RenderHeight, 0, 0, 1);
            GL.LoadMatrix(ref mat);

            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Enable(EnableCap.Texture2D);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureManager.Get(0));

            GL.Begin(PrimitiveType.Quads);

            float u_step = TextSettings.GlyphWidth / TextureManager.GetTextureSize(0).X;
            float v_step = TextSettings.GlyphHeight / TextureManager.GetTextureSize(0).Y;

            if (align == StringAlignment.Center)
            {
                x -= TextRenderer.MeasureText(text, RenderFont).Width / 3;
                y -= TextRenderer.MeasureText(text, RenderFont).Height / 2;
            }
            if (align == StringAlignment.Far)
                x -= TextRenderer.MeasureText(text, RenderFont).Width;

            for (int n = 0; n < text.Length; n++)
            {
                char idx = text[n];
                float u = (float)(idx % TextSettings.GlyphsPerLine) * u_step;
                float v = (float)(idx / TextSettings.GlyphsPerLine) * v_step;

                if(dropShadow)
                {
                    GL.Color3(Color.Black);
                    GL.TexCoord2(u, v);
                    GL.Vertex2(x + 1, y + 1);
                    GL.TexCoord2(u + u_step, v);
                    GL.Vertex2(x + TextSettings.GlyphWidth + 1, y + 1);
                    GL.TexCoord2(u + u_step, v + v_step);
                    GL.Vertex2(x + TextSettings.GlyphWidth + 1, y + TextSettings.GlyphHeight + 1);
                    GL.TexCoord2(u, v + v_step);
                    GL.Vertex2(x + 1, y + TextSettings.GlyphHeight + 1);
                }

                GL.Color3(Color.White);
                GL.TexCoord2(u, v);
                GL.Vertex2(x, y);
                GL.TexCoord2(u + u_step, v);
                GL.Vertex2(x + TextSettings.GlyphWidth, y);
                GL.TexCoord2(u + u_step, v + v_step);
                GL.Vertex2(x + TextSettings.GlyphWidth, y + TextSettings.GlyphHeight);
                GL.TexCoord2(u, v + v_step);
                GL.Vertex2(x, y + TextSettings.GlyphHeight);
                
                x += CharSpacing[text[n]];// TextRenderer.MeasureText(text[n].ToString(), RenderFont).Width;
            }
            //Console.WriteLine(x + " " + TextRenderer.MeasureText(text, RenderFont));

            GL.End();

            GL.PopMatrix();
            GL.PopAttrib();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void ClearResources()
        {
            if(TextureManager != null)
            { 
                TextureManager.ClearTextures();
                TextureManager = null;
            }
        }

        private static Dictionary<char, float> CharSpacing = new Dictionary<char, float>();
        /// <summary>
        /// 
        /// </summary>
        private static void GenerateFontImage()
        {
            int bitmapWidth = TextSettings.GlyphsPerLine * TextSettings.GlyphWidth;
            int bitmapHeight = TextSettings.GlyphLineCount * TextSettings.GlyphHeight;

            using (Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    if (TextSettings.BitmapFont)
                    {
                        g.SmoothingMode = SmoothingMode.None;
                        g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                    }
                    else
                    {
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    }

                    for (int p = 0; p < TextSettings.GlyphLineCount; p++)
                    {
                        for (int n = 0; n < TextSettings.GlyphsPerLine; n++)
                        {
                            char c = (char)(n + p * TextSettings.GlyphsPerLine);
                            CharSpacing.Add(c, TextRenderer.MeasureText(g, c.ToString(), RenderFont, new Size(1, 1), TextFormatFlags.NoPadding).Width + 1);
                            g.DrawString(c.ToString(), RenderFont, Brushes.White,
                                n * TextSettings.GlyphWidth + TextSettings.AtlasOffsetX, p * TextSettings.GlyphHeight + TextSettings.AtlassOffsetY);
                        }
                    }
                }
                TextureManager.Add(bitmap);
            }
        }
    }
}
