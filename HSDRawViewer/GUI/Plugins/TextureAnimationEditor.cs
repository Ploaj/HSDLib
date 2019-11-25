using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Converters;
using HSDRawViewer.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins
{
    /// <summary>
    /// Tool for editing Texture Animations <see cref="HSD_TexGraphic"/> and <see cref="HSD_TexAnim"/>
    /// </summary>
    public partial class TextureAnimationEditor : DockContent, EditorBase, IDrawable
    {
        public DockState DefaultDockState => DockState.Document;

        public DrawOrder DrawOrder => DrawOrder.Last;

        public Type[] SupportedTypes => new Type[] { typeof(HSD_TexAnim), typeof(HSD_TexGraphic), typeof(HSD_TOBJ) };
        
        private HSD_TexAnim TexAnim;
        private HSD_TexGraphic TEXG;
        private HSD_TOBJ TOBJ;

        private TextureManager TextureManager = new TextureManager();

        private ViewportControl viewport;

        private int TextureWidth
        {
            get
            {
                if (TexAnim != null)
                    return TexAnim.ImageBuffers[0].Data.Width;

                if (TEXG != null)
                    return TEXG.Width;

                if (TOBJ != null)
                    return TOBJ.ImageData.Width;

                return 0;
            }
        }
        private int TextureHeight
        {
            get
            {
                if (TexAnim != null)
                    return TexAnim.ImageBuffers[0].Data.Height;

                if (TEXG != null)
                    return TEXG.Height;

                if (TOBJ != null)
                    return TOBJ.ImageData.Height;

                return 0;
            }
        }

        public DataNode Node
        {
            get
            {
                return _node;
            }
            set
            {
                _node = value;
                var a = _node.Accessor;

                viewport.Frame = 0;

                if (a is HSD_TOBJ tobj)
                {
                    TOBJ = tobj;
                    exportStripToolStripMenuItem1.Visible = false;
                    importStripToolStripMenuItem1.Visible = false;
                    exportFramesToolStripMenuItem.Visible = false;
                    importFramesToolStripMenuItem.Visible = false;

                    viewport.AnimationTrackEnabled = false;
                    viewport.MaxFrame = 0;
                }

                if (a is HSD_TexAnim ta)
                {
                    TexAnim = ta;
                    exportStripToolStripMenuItem1.Visible = false;
                    importStripToolStripMenuItem1.Visible = false;
                    exportPNGToolStripMenuItem.Visible = false;
                    importPNGToolStripMenuItem.Visible = false;
                    
                    viewport.MaxFrame = TexAnim.ImageCount - 1;
                }

                if (a is HSD_TexGraphic tgb)
                {
                    TEXG = tgb;
                    exportFramesToolStripMenuItem.Visible = false;
                    importFramesToolStripMenuItem.Visible = false;
                    exportPNGToolStripMenuItem.Visible = false;
                    importPNGToolStripMenuItem.Visible = false;
                    
                    viewport.MaxFrame = TEXG.ImageCount - 1;
                }

                ReloadTextures();
            }
        }
        private DataNode _node;
        
        public TextureAnimationEditor()
        {
            InitializeComponent();

            viewport = new ViewportControl();
            viewport.Dock = DockStyle.Fill;
            viewport.AnimationTrackEnabled = true;
            viewport.AddRenderer(this);
            Controls.Add(viewport);
            viewport.BringToFront();

            toolStripComboBox1.SelectedIndex = 0;

            FormClosing += (sender, args) =>
            {
                viewport.Dispose();
                TextureManager.ClearTextures();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReloadTextures()
        {
            TextureManager.ClearTextures();
            
            if(TOBJ != null)
            {
                TextureManager.Add(TOBJ.GetDecodedImageData(), TOBJ.ImageData.Width, TOBJ.ImageData.Height);
            }
            if(TexAnim != null)
            {
                for (int i = 0; i < TexAnim.ImageCount; i++)
                {
                    var v = TexAnim.ImageBuffers.Array[i];
                    HSD_TlutBuffer pal = null;

                    if (TexAnim.TlutBuffers != null)
                    {
                        pal = TexAnim.TlutBuffers.Array[i];
                    }

                    HSD_TOBJ tobj = new HSD_TOBJ();
                    tobj.ImageData = v.Data;
                    if (pal != null)
                        tobj.TlutData = pal.Data;

                    TextureManager.Add(tobj.GetDecodedImageData(), v.Data.Width, v.Data.Height);
                }
            }
            if(TEXG != null)
            {
                foreach (var v in TEXG.GetRGBAImageData())
                {
                    TextureManager.Add(v, TEXG.Width, TEXG.Height);
                }
            }
        }

        public void Draw(int windowWidth, int windowHeight)
        {
            switch(toolStripComboBox1.SelectedIndex)
            {
                case 0:
                    TextureManager.RenderTexture((int)viewport.Frame, windowWidth, windowHeight, false);
                    break;
                case 1:
                    TextureManager.RenderTexture((int)viewport.Frame, windowWidth, windowHeight, true, TextureWidth * 2, TextureHeight * 2);
                    break;
                case 2:
                    TextureManager.RenderTexture((int)viewport.Frame, windowWidth, windowHeight, true);
                    break;
            }
        }

        private void exportStripToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (TEXG == null)
                return;
            var f = Tools.FileIO.SaveFile("PNG (*.png)|*.png");
            if (f != null)
            {
                var bitmap = new Bitmap(TEXG.Width * TEXG.ImageCount, TEXG.Height);
                using (var g = Graphics.FromImage(bitmap))
                {
                    var imageData = TEXG.GetRGBAImageData();
                    for (int i = 0; i < TEXG.ImageCount; i++)
                    {
                        var frame = TOBJConverter.RgbaToImage(imageData[i], TEXG.Width, TEXG.Height);
                        g.DrawImage(frame, TEXG.Width * i, 0);
                        frame.Dispose();
                    }
                }
                bitmap.Save(f);
                bitmap.Dispose();

                Node = Node;
            }
        }

        private void importStripToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (TEXG == null)
                return;
            var f = Tools.FileIO.OpenFile("PNG (*.png)|*.png");
            if (f != null)
            {
                using (TextureImportDialog td = new TextureImportDialog())
                {
                    td.ShowCount = true;

                    if (td.ShowDialog() == DialogResult.OK)
                    {
                        var bmp = new Bitmap(f);
                        HSD_TOBJ[] images = new HSD_TOBJ[td.ImageCount];
                        for (int i = 0; i < td.ImageCount; i++)
                        {
                            images[i] = new HSD_TOBJ();
                            var image = bmp.Clone(new Rectangle(bmp.Width / td.ImageCount * i, 0, bmp.Width / td.ImageCount, bmp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                            TOBJConverter.InjectBitmap(images[i], image, td.TextureFormat, td.PaletteFormat);
                            image.Dispose();
                        }
                        bmp.Dispose();
                        TEXG.SetFromTOBJs(images);

                        Node = Node;
                    }
                }
            }
        }

        private void exportFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TexAnim == null)
                return;

            var f = Tools.FileIO.SaveFile("PNG (*.png)|*.png");
            if (f != null)
            {
                for (int i = 0; i < TexAnim.ImageCount; i++)
                {
                    var v = TexAnim.ImageBuffers.Array[i];
                    HSD_TlutBuffer pal = null;

                    if (TexAnim.TlutBuffers != null)
                    {
                        pal = TexAnim.TlutBuffers.Array[i];
                    }

                    HSD_TOBJ tobj = new HSD_TOBJ();
                    tobj.ImageData = v.Data;
                    if (pal != null)
                        tobj.TlutData = pal.Data;

                    var frame = TOBJConverter.RgbaToImage(tobj.GetDecodedImageData(), v.Data.Width, v.Data.Height);
                    frame.Save(Path.GetDirectoryName(f) + "\\" + Path.GetFileNameWithoutExtension(f) + "_" + i.ToString() + Path.GetExtension(f));
                    frame.Dispose();
                }
            }
        }

        private void importFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TexAnim == null)
                return;

            var files = Tools.FileIO.OpenFiles("PNG (*.png)|*.png");
            if (files != null)
            {
                using (TextureImportDialog td = new TextureImportDialog())
                {
                    if (td.ShowDialog() == DialogResult.OK)
                    {
                        var matched = new HashSet<int>();
                        var tobjs = new List<HSD_TexBuffer>();
                        var pals = new List<HSD_TlutBuffer>();
                        foreach (var f in files)
                        {
                            int num = 0;
                            int.TryParse(Regex.Match(Path.GetFileName(f).ToLower(), @"(?<=.*_)(\d+?)(?=\.)").Name, out num);
                            Console.WriteLine(num + " " + Regex.Match(Path.GetFileName(f).ToLower(), @"(?<=.*_)(\d+?)(?=\.)").Groups[0].Value);
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

                                if (TPLConv.IsPalettedFormat(td.TextureFormat))
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

                        TexAnim.ImageBuffers = ib;

                        if (TPLConv.IsPalettedFormat(td.TextureFormat))
                        {
                            var tb = new HSDRaw.HSDArrayAccessor<HSD_TlutBuffer>();
                            tb.Array = pals.ToArray();

                            TexAnim.TlutBuffers = tb;

                            if (TexAnim?.AnimationObject?.FObjDesc?.List.Count < 2)
                                TexAnim.AnimationObject.FObjDesc.Next = new HSD_FOBJDesc()
                                {
                                    FOBJ = new HSD_FOBJ() { AnimationType = JointTrackType.HSD_A_J_SCAX, Buffer = new byte[0] }
                                };
                        }
                        else
                        {
                            TexAnim.TlutBuffers = null;
                            if (TexAnim?.AnimationObject?.FObjDesc?.List.Count > 1)
                                TexAnim.AnimationObject.FObjDesc.Next = null;
                        }

                        Node = Node;
                    }
                }
            }
        }

        private void exportPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TOBJ == null)
                return;
            var f = Tools.FileIO.SaveFile("PNG (.png)|*.png");
            if (f != null)
            {
                var bmp = TOBJConverter.ToBitmap(TOBJ);
                bmp.Save(f);
                bmp.Dispose();
            }
        }

        private void importPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TOBJ == null)
                return;
            var f = Tools.FileIO.OpenFile("PNG (.png)|*.png");
            if (f != null)
            {
                using (TextureImportDialog settings = new TextureImportDialog())
                {
                    if (settings.ShowDialog() == DialogResult.OK)
                    {
                        TOBJConverter.InjectBitmap(TOBJ, f, settings.TextureFormat, settings.PaletteFormat);
                    }
                }
            }
        }
    }
}
