using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Converters;
using HSDRawViewer.GUI;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HSDRawViewer.Rendering.Renderers
{
    public class RendererTexAnim
    {

        public static ToolStrip ToolStrip
        {
            get
            {
                if (_toolStrip == null)
                {
                    InitToolStrip();
                }
                return _toolStrip;
            }
        }
        private static ToolStrip _toolStrip;

        private static HSD_TexAnim TextureAnim;

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

            ToolStripButton export = new ToolStripButton("Export Frames");
            export.Click += (sender, args) =>
            {
                using (SaveFileDialog d = new SaveFileDialog())
                {
                    d.Filter = "PNG (*.png)|*.png";

                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        for (int i = 0; i < TextureAnim.ImageCount; i++)
                        {
                            var v = TextureAnim.ImageBuffers.Array[i];
                            HSD_TlutBuffer pal = null;

                            if (TextureAnim.TlutBuffers != null)
                            {
                                pal = TextureAnim.TlutBuffers.Array[i];
                            }

                            HSD_TOBJ tobj = new HSD_TOBJ();
                            tobj.ImageData = v.Data;
                            if (pal != null)
                                tobj.TlutData = pal.Data;

                            var frame = TOBJConverter.RgbaToImage(tobj.GetDecodedImageData(), v.Data.Width, v.Data.Height);
                            frame.Save(Path.GetDirectoryName(d.FileName) + "\\" + Path.GetFileNameWithoutExtension(d.FileName) + "_" + i.ToString() + Path.GetExtension(d.FileName));
                            frame.Dispose();
                        }
                    }
                }
            };
            _toolStrip.Items.Add(export);

            ToolStripButton import = new ToolStripButton("Import Frames");
            import.Click += (sender, args) =>
            {
                using (OpenFileDialog d = new OpenFileDialog())
                {
                    d.Filter = "PNG (*.png)|*.png";
                    d.Multiselect = true;

                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        using (TextureImportDialog td = new TextureImportDialog())
                        {
                            if (td.ShowDialog() == DialogResult.OK)
                            {
                                var matched = new HashSet<int>();
                                var tobjs = new List<HSD_TexBuffer>();
                                var pals = new List<HSD_TlutBuffer>();
                                foreach (var f in d.FileNames)
                                {
                                    int num = 0;
                                    int.TryParse(Regex.Match(Path.GetFileName(f).ToLower(), @"(?<=.*_)(\d+?)(?=\.)").Name, out num);
                                    System.Console.WriteLine(num + " " + Regex.Match(Path.GetFileName(f).ToLower(), @"(?<=.*_)(\d+?)(?=\.)").Groups[0].Value);
                                    if (!int.TryParse(Regex.Match(Path.GetFileName(f).ToLower(), @"(?<=.*_)(\d+?)(?=\.)").Groups[0].Value, out num) || matched.Contains(num))
                                    {
                                        throw new InvalidDataException(num + " " + Path.GetFileName(f) + " is missing index tag or has duplicate index");
                                    }
                                    else
                                    {
                                        HSD_TOBJ temp = new HSD_TOBJ();
                                        TOBJConverter.InjectBitmap(temp, f, td.TextureFormat, td.PaletteFormat);

                                        HSD_TexBuffer buf = new HSD_TexBuffer();
                                        buf.Data = temp.ImageData;
                                        tobjs.Add(buf);

                                        if(TPLConv.IsPalettedFormat(td.TextureFormat))
                                        {
                                            HSD_TlutBuffer palbuf = new HSD_TlutBuffer();
                                            palbuf.Data = temp.TlutData;
                                            pals.Add(palbuf);
                                        }

                                        matched.Add(num);
                                    }
                                }

                                // convert tobjs into proper strip
                                var ib = new HSDRaw.HSDArrayAccessor<HSD_TexBuffer>();
                                ib.Array = tobjs.ToArray();

                                TextureAnim.ImageBuffers = ib;

                                if (TPLConv.IsPalettedFormat(td.TextureFormat))
                                {
                                    var tb = new HSDRaw.HSDArrayAccessor<HSD_TlutBuffer>();
                                    tb.Array = pals.ToArray();

                                    TextureAnim.TlutBuffers = tb;
                                }
                            }
                        }
                    }
                }
            };
            _toolStrip.Items.Add(import);
        }

        public static void Render(HSD_TexAnim anim, int windowWidth, int windowHeight)
        {
            if (!Viewport.EnableAnimationTrack)
            {
                Viewport.EnableAnimationTrack = true;
                Viewport.Frame = 0;
                Viewport.MaxFrame = anim.ImageCount;
                Viewport.AnimSpeed = 30;
            }

            if (TextureAnim != anim)
            {
                TextureManager.ClearTextures();
                
                for(int i = 0; i < anim.ImageCount; i++)
                {
                    var v = anim.ImageBuffers.Array[i];
                    HSD_TlutBuffer pal = null;

                    if (anim.TlutBuffers != null)
                    {
                        pal = anim.TlutBuffers.Array[i];
                    }

                    HSD_TOBJ tobj = new HSD_TOBJ();
                    tobj.ImageData = v.Data;
                    if(pal != null)
                        tobj.TlutData = pal.Data;

                    TextureManager.Add(tobj.GetDecodedImageData(), v.Data.Width, v.Data.Height);
                }

                TextureAnim = anim;
            }

            if (TextureManager.TextureCount == 0)
                return;

            TextureManager.RenderTexture(Viewport.Frame, windowWidth, windowHeight, ActualSize);
        }

    }
}
