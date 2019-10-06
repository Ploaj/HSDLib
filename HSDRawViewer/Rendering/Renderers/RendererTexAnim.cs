using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Converters;
using HSDRawViewer.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HSDRawViewer.Rendering.Renderers
{
    public class RendererTexAnim : IRenderer
    {
        public Type[] SupportedTypes
        {
            get
            {
                return new Type[] { typeof(HSD_TexAnim)};
            }
        }

        public ToolStrip ToolStrip
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

        private ToolStrip _toolStrip;

        private HSD_TexAnim TextureAnim;

        private bool ActualSize = false;

        private TextureManager TextureManager = new TextureManager();

        private void InitToolStrip()
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
                                var ib = new HSDArrayAccessor<HSD_TexBuffer>();
                                ib.Array = tobjs.ToArray();

                                TextureAnim.ImageBuffers = ib;

                                if (TPLConv.IsPalettedFormat(td.TextureFormat))
                                {
                                    var tb = new HSDRaw.HSDArrayAccessor<HSD_TlutBuffer>();
                                    tb.Array = pals.ToArray();

                                    TextureAnim.TlutBuffers = tb;

                                    if (TextureAnim?.AnimationObject?.FObjDesc?.List.Count < 2)
                                        TextureAnim.AnimationObject.FObjDesc.Next = new HSD_FOBJDesc()
                                        {
                                            FOBJ = new HSD_FOBJ() { AnimationType = JointTrackType.HSD_A_J_SCAX, Buffer = new byte[0] }
                                        };
                                }
                                else
                                {
                                    TextureAnim.TlutBuffers = null;
                                    if (TextureAnim?.AnimationObject?.FObjDesc?.List.Count > 1)
                                        TextureAnim.AnimationObject.FObjDesc.Next = null;
                                }

                                ((DataNode)MainForm.SelectedDataNode.Parent).Refresh();
                            }
                        }
                    }
                }
            };
            _toolStrip.Items.Add(import);
        }

        public void Clear()
        {
            TextureAnim = null;
            TextureManager.ClearTextures();
        }

        public void Render(HSDAccessor a, int windowWidth, int windowHeight)
        {
            HSD_TexAnim anim = a as HSD_TexAnim;

            if (!Viewport.EnableAnimationTrack)
            {
                Viewport.EnableAnimationTrack = true;
            }

            if (TextureAnim != anim)
            {
                TextureManager.ClearTextures();

                Viewport.Frame = 0;
                Viewport.MaxFrame = anim.ImageCount - 1;
                Viewport.AnimSpeed = 30;

                for (int i = 0; i < anim.ImageCount; i++)
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
