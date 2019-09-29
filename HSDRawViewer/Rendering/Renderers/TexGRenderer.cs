using HSDRaw.Common;
using HSDRawViewer.Converters;
using HSDRawViewer.GUI;
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
                                frame.Dispose();
                            }
                        }
                        bitmap.Save(d.FileName);
                        bitmap.Dispose();
                        Form1.SelectedDataNode.Refresh();
                    }
                }
            };
            _toolStrip.Items.Add(export);

            ToolStripButton import = new ToolStripButton("Import Strip");
            import.Click += (sender, args) =>
            {
                using (OpenFileDialog d = new OpenFileDialog())
                {
                    d.Filter = "PNG (*.png)|*.png";

                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        using (TextureImportDialog td = new TextureImportDialog())
                        {
                            td.ShowCount = true;

                            if(td.ShowDialog() == DialogResult.OK)
                            {
                                var bmp = new Bitmap(d.FileName);
                                HSD_TOBJ[] images = new HSD_TOBJ[td.ImageCount];
                                for(int i = 0; i < td.ImageCount; i++)
                                {
                                    images[i] = new HSD_TOBJ();
                                    var image = bmp.Clone(new Rectangle(bmp.Width / td.ImageCount * i, 0, bmp.Width / td.ImageCount, bmp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                    TOBJConverter.InjectBitmap(images[i], image, td.TextureFormat, td.PaletteFormat);
                                    image.Dispose();
                                }
                                bmp.Dispose();
                                ParticleGroup.SetFromTOBJs(images);
                                ParticleGroup = null;
                                Form1.SelectedDataNode.Refresh();
                            }
                        }
                    }
                }
            };
            _toolStrip.Items.Add(import);
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
