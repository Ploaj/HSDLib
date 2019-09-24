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

        private static HSD_TexGraphic ParticleGroup;
        
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

        public static void Render(HSD_TexGraphic particle, int windowWidth, int windowHeight)
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

            TextureManager.RenderTexture(Viewport.Frame, windowWidth, windowHeight, ActualSize, ParticleGroup.Width, ParticleGroup.Height);
        }
        
    }
}
