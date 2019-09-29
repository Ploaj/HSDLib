using HSDRaw.Common;
using HSDRawViewer.Converters;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
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

            ToolStripButton export = new ToolStripButton("Export Strip");
            export.Click += (sender, args) =>
            {
                using (SaveFileDialog d = new SaveFileDialog())
                {
                    d.Filter = "PNG (*.png)|*.png";

                    if(d.ShowDialog() == DialogResult.OK)
                    {
                        var bitmap = new Bitmap(ParticleGroup.Width * ParticleGroup.ImageCount, ParticleGroup.Height);
                        using (var g = Graphics.FromImage(bitmap))
                        {
                            var imageData = ParticleGroup.GetRGBAImageData();
                            for (int i = 0; i < ParticleGroup.ImageCount; i++)
                            {
                                var frame = TOBJConverter.RgbaToImage(imageData[i], ParticleGroup.Width, ParticleGroup.Height);
                                g.DrawImage(frame, ParticleGroup.Width * i, 0);
                            }
                        }
                        bitmap.Save(d.FileName);
                        bitmap.Dispose();
                    }
                }
            };
            _toolStrip.Items.Add(export);

            ToolStripButton import = new ToolStripButton("Import Strip");
            import.Click += (sender, args) =>
            {

            };
            //_toolStrip.Items.Add(import);
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
