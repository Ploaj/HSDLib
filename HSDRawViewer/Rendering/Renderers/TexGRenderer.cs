using HSDRaw.Common;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;

namespace HSDRawViewer.Rendering
{
    public class TexGraphicRenderer
    {
        public static ToolStrip ToolStrip
        {
            get
            {
                if(_toolStrip == null)
                {
                    InitToolStrip();
                }
                return _toolStrip;
            }
        }
        private static ToolStrip _toolStrip;

        private static HSD_ParticleImage ParticleGroup;
        
        private static bool ActualSize = false;

        private static TextureManager TextureManager = new TextureManager();

        private static void InitToolStrip()
        {
            _toolStrip = new ToolStrip();

            ToolStripButton toggleSize = new ToolStripButton("Toggle Size");
            toggleSize.Click += (sender, args) =>
            {
                ActualSize = !ActualSize;
            };
            _toolStrip.Items.Add(toggleSize);
        }

        private static void RenderCheckerBack(int sizeOfChecker, float width, float height)
        {
            float wAmt = width / sizeOfChecker;
            float hAmt = height / sizeOfChecker;
            for (int h = 0; h < hAmt * 2; h ++)
            {
                for (int w = 0; w < wAmt * 2; w ++)
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

        public static void Render(HSD_ParticleImage particle, int windowWidth, int windowHeight)
        {
            if(ParticleGroup != particle)
            {
                TextureManager.ClearTextures();
                Viewport.Frame = 0;
                Viewport.MaxFrame = particle.ImageCount;
                Viewport.EnableAnimationTrack = true;
                Viewport.AnimSpeed = 30;
                foreach(var v in particle.GetRGBAImageData())
                {
                    TextureManager.Add(v, particle.Width, particle.Height);
                }
                ParticleGroup = particle;
            }

            if (TextureManager.TextureCount == 0)
                return;
            
            GL.PushMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            RenderCheckerBack(64, windowWidth, windowHeight);

            GL.Enable(EnableCap.Texture2D);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureManager.Get(Viewport.Frame));

            GL.Color3(1f, 1f, 1f);

            float w = windowWidth;
            float h = windowHeight;
            
            if (ActualSize)
            {
                w = ParticleGroup.Width / 2;
                h = ParticleGroup.Height / 2;
            }
            else
            {
                if (windowHeight > windowWidth)
                {
                    var aspect = ParticleGroup.Height / (float)ParticleGroup.Width;
                    w = windowWidth;
                    h = windowWidth * aspect;
                }
                else
                {
                    var aspect = ParticleGroup.Width / (float)ParticleGroup.Height;
                    w = windowHeight * aspect;
                    h = windowHeight;
                }
            }

            w /= windowWidth;
            h /= windowHeight;

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
