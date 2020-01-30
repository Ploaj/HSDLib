using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Converters;
using HSDRawViewer.Rendering;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using HSDRaw.GX;

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
        private ImageList BitmapList = new ImageList();

        private ViewportControl viewport;

        private int TextureCount
        {
            get
            {
                if (TexAnim != null)
                    return TexAnim.ImageCount;

                if (TEXG != null)
                    return TEXG.ImageCount;

                if (TOBJ != null)
                    return 1;

                return 0;
            }
        }

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

                    buttonLeft.Visible = false;
                    buttonRight.Visible = false;
                    buttonAdd.Visible = false;
                    buttonRemove.Visible = false;

                    textureList.Visible = false;

                    //viewport.AnimationTrackEnabled = false;
                    //viewport.MaxFrame = 0;
                }

                if (a is HSD_TexAnim ta)
                {
                    TexAnim = ta;
                    //viewport.MaxFrame = TexAnim.ImageCount - 1;
                }

                if (a is HSD_TexGraphic tgb)
                {
                    TEXG = tgb;
                    //viewport.MaxFrame = TEXG.ImageCount - 1;
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
            viewport.AnimationTrackEnabled = false;
            viewport.AddRenderer(this);
            Controls.Add(viewport);
            viewport.BringToFront();

            toolStripComboBox1.SelectedIndex = 0;

            BitmapList.ImageSize = new Size(64, 64);
            BitmapList.ColorDepth = ColorDepth.Depth32Bit;
            textureList.LargeImageList = BitmapList;

            toolStripComboBox1.SelectedIndex = 4;
            
            FormClosing += (sender, args) =>
            {
                BitmapList.Dispose();
                viewport.Dispose();
                ReleaseResource();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadTexture(byte[] data, int width, int height)
        {
            var id = TextureManager.TextureCount;
            TextureManager.Add(data, width, height);
            var image = TOBJConverter.RgbaToImage(data, width, height);
            BitmapList.Images.Add(image);
            textureList.Items.Add(new ListViewItem() { ImageIndex = id, Text = "Image_" + id });
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReleaseResource()
        {
            TextureManager.ClearTextures();
            textureList.Clear();
            foreach (Bitmap bmp in BitmapList.Images)
                bmp.Dispose();
            BitmapList.Images.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReloadTextures()
        {
            ReleaseResource();

            var tobjs = GetTOBJs();

            foreach(var tobj in tobjs)
            {
                LoadTexture(tobj.GetDecodedImageData(), tobj.ImageData.Width, tobj.ImageData.Height);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private HSD_TOBJ[] GetTOBJs()
        {
            HSD_TOBJ[] tobjs = new HSD_TOBJ[TextureCount];
            
            if (TOBJ != null)
                tobjs[0] = TOBJ;

            if (TexAnim != null)
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

                    tobjs[i] = tobj;
                }
            }

            if (TEXG != null)
                tobjs = TEXG.ConvertToTOBJs();

            return tobjs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        private void SetTOBJs(HSD_TOBJ[] tobjs)
        {
            if (TOBJ != null)
                TOBJ = tobjs[0];

            if (TexAnim != null)
            {
                var imgs = new List<HSD_TexBuffer>();
                var pals = new List<HSD_TlutBuffer>();

                foreach(var tobj in tobjs)
                {
                    HSD_TexBuffer buf = new HSD_TexBuffer();
                    buf.Data = tobj.ImageData;
                    imgs.Add(buf);

                    if (TPLConv.IsPalettedFormat(tobj.ImageData.Format))
                    {
                        HSD_TlutBuffer palbuf = new HSD_TlutBuffer();
                        palbuf.Data = tobj.TlutData;
                        pals.Add(palbuf);
                    }
                }

                // convert tobjs into proper strip
                var ib = new HSDArrayAccessor<HSD_TexBuffer>();
                ib.Array = imgs.ToArray();

                TexAnim.ImageBuffers = ib;

                if (tobjs.Length > 0 && TPLConv.IsPalettedFormat(tobjs[0].ImageData.Format))
                {
                    var tb = new HSDRaw.HSDArrayAccessor<HSD_TlutBuffer>();
                    tb.Array = pals.ToArray();

                    TexAnim.TlutBuffers = tb;

                    if (TexAnim?.AnimationObject?.FObjDesc?.List.Count < 2)
                    {
                        TexAnim.AnimationObject.FObjDesc.Next = new HSD_FOBJDesc();
                        TexAnim.AnimationObject.FObjDesc.Next.FromFOBJ(new HSD_FOBJ() { AnimationType = JointTrackType.HSD_A_J_SCAX, Buffer = new byte[0] });
                    }
                }
                else
                {
                    TexAnim.TlutBuffers = null;
                    if (TexAnim?.AnimationObject?.FObjDesc?.List.Count > 1)
                        TexAnim.AnimationObject.FObjDesc.Next = null;
                }
            }

            if (TEXG != null)
                TEXG.SetFromTOBJs(tobjs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            TextureManager.RenderCheckerBack(windowWidth, windowHeight);

            var selectedIndex = -1;

            if (textureList.SelectedItems != null && textureList.SelectedItems.Count > 0)
                selectedIndex = textureList.SelectedItems[0].ImageIndex;

            if (TextureCount == 1)
                selectedIndex = 0;

            if (selectedIndex == -1)
                return;

            switch (toolStripComboBox1.SelectedIndex)
            {
                case 0: // fill
                    TextureManager.RenderTexture(selectedIndex, windowWidth, windowHeight, false);
                    break;
                case 1: //300
                    TextureManager.RenderTexture(selectedIndex, windowWidth, windowHeight, true, TextureWidth * 2 * 3, TextureHeight * 2 * 3);
                    break;
                case 2: //200
                    TextureManager.RenderTexture(selectedIndex, windowWidth, windowHeight, true, TextureWidth * 2 * 2, TextureHeight * 2 * 2);
                    break;
                case 3: //150
                    TextureManager.RenderTexture(selectedIndex, windowWidth, windowHeight, true, (int)(TextureWidth * 2 * 1.5f), (int)(TextureHeight * 2 * 1.5f));
                    break;
                case 4: //100
                    TextureManager.RenderTexture(selectedIndex, windowWidth, windowHeight, true, TextureWidth * 2, TextureHeight * 2);
                    break;
                case 5: //50
                    TextureManager.RenderTexture(selectedIndex, windowWidth, windowHeight, true, TextureWidth, TextureHeight);
                    break;
            }
        }

        #region Controls

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReplace_Click(object sender, EventArgs e)
        {
            if ((textureList.SelectedIndices == null || textureList.SelectedIndices.Count == 0) && TOBJ == null)
                return;

            var index = 0;
            if(TOBJ == null)
                index = textureList.SelectedIndices[0];

            var tobjs = GetTOBJs();

            var f = Tools.FileIO.OpenFile("PNG (.png)|*.png");
            if (f != null)
            {
                if (tobjs.Length == 1)
                {
                    using (TextureImportDialog settings = new TextureImportDialog())
                    {
                        if (settings.ShowDialog() == DialogResult.OK)
                        {
                            TOBJConverter.InjectBitmap(tobjs[index], f, settings.TextureFormat, settings.PaletteFormat);
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    var beforeH = tobjs[index].ImageData.Height;
                    var beforeW = tobjs[index].ImageData.Width;

                    var bmp = new Bitmap(f);

                    if (beforeW != bmp.Width || beforeH != bmp.Height)
                    {
                        MessageBox.Show($"Error the texture size does not match\n{beforeW}x{beforeH} -> {bmp.Width}x{bmp.Height}");
                        return;
                    }

                    bmp.Dispose();

                    TOBJConverter.InjectBitmap(tobjs[index], f, tobjs[index].ImageData.Format, tobjs[index].TlutData != null ? tobjs[index].TlutData.Format : GXTlutFmt.IA8);
                }
            }
            else
            {
                return;
            }

            SetTOBJs(tobjs);

            ReloadTextures();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T[] InsertAt<T>(T[] source, T item, int index)
        {
            T[] dest = new T[source.Length + 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            dest[index] = item;

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            //if (textureList.SelectedIndices == null || textureList.SelectedIndices.Count == 0)
            //    return;

            //var index = textureList.SelectedIndices[0];

            var tobjs = GetTOBJs();

            HSD_TOBJ tobj;
            var f = Tools.FileIO.OpenFiles("PNG (.png)|*.png");
            if (f != null)
            {
                GXTexFmt texFormat = GXTexFmt.CMP;
                GXTlutFmt palFormat = GXTlutFmt.IA8;
                if(tobjs.Length == 0)
                {
                    using (TextureImportDialog settings = new TextureImportDialog())
                    {
                        if (settings.ShowDialog() == DialogResult.OK)
                        {
                            texFormat = settings.TextureFormat;
                            palFormat = settings.PaletteFormat;
                            tobjs = new HSD_TOBJ[f.Length];

                            int ti = 0;
                            foreach(var fi in f)
                            {
                                tobj = new HSD_TOBJ();
                                TOBJConverter.InjectBitmap(tobj, fi, texFormat, palFormat);
                                tobjs[ti++] = tobj;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    texFormat = tobjs[0].ImageData.Format;
                    if (tobjs[0].TlutData != null)
                        palFormat = tobjs[0].TlutData.Format;

                    foreach(var fi in f)
                    {
                        tobj = new HSD_TOBJ();
                        TOBJConverter.InjectBitmap(tobj, fi, texFormat, palFormat);

                        if (tobj.ImageData.Width != tobjs[0].ImageData.Width || tobj.ImageData.Height != tobjs[0].ImageData.Height)
                        {
                            MessageBox.Show($"Error the texture size does not match\n{tobj.ImageData.Width}x{tobj.ImageData.Height} -> {tobjs[0].ImageData.Width}x{tobjs[0].ImageData.Height}");
                            return;
                        }

                        tobjs = InsertAt(tobjs, tobj, tobjs.Length);
                    }
                }
            }
            else
            {
                return;
            }
            
            SetTOBJs(tobjs);

            ReloadTextures();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T[] RemoveAt<T>(T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (textureList.SelectedIndices == null || textureList.SelectedIndices.Count == 0)
                return;

            var index = textureList.SelectedIndices[0];
            
            var tobjs = GetTOBJs();

            tobjs = RemoveAt(tobjs, index);

            SetTOBJs(tobjs);

            ReloadTextures();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRight_Click(object sender, EventArgs e)
        {
            if (textureList.SelectedIndices == null || textureList.SelectedIndices.Count == 0)
                return;

            var index = textureList.SelectedIndices[0];

            if (index == textureList.Items.Count - 1)
                return;
            
            Swap(index, index + 1);

            textureList.Items[index].Selected = false;
            textureList.Items[index + 1].Selected = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLeft_Click(object sender, EventArgs e)
        {
            if (textureList.SelectedIndices == null || textureList.SelectedIndices.Count == 0)
                return;

            var index = textureList.SelectedIndices[0];

            if (index == 0)
                return;

            Swap(index, index - 1);

            textureList.Items[index].Selected = false;
            textureList.Items[index - 1].Selected = true;
        }

        /// <summary>
        /// Swaps two images
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        private void Swap(int index1, int index2)
        {
            var tobjs = GetTOBJs();

            var temp = tobjs[index1];
            tobjs[index1] = tobjs[index2];
            tobjs[index2] = temp;

            SetTOBJs(tobjs);

            TextureManager.Swap(index1, index2);

            var t = BitmapList.Images[index1];
            BitmapList.Images[index1] = BitmapList.Images[index2];
            BitmapList.Images[index2] = t;

            textureList.Invalidate();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((textureList.SelectedIndices == null || textureList.SelectedIndices.Count == 0) && TOBJ == null)
                return;

            var f = Tools.FileIO.SaveFile("PNG (.png)|*.png");
            if (f != null)
            {
                var bmp = TOBJConverter.ToBitmap(GetTOBJs()[textureList.SelectedIndices[0]]);
                bmp.Save(f);
                bmp.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.SaveFile("PNG (*.png)|*.png");
            if (f != null)
            {
                var i = 0;
                foreach(var tobj in GetTOBJs())
                {
                    var frame = TOBJConverter.RgbaToImage(tobj.GetDecodedImageData(), tobj.ImageData.Width, tobj.ImageData.Height);
                    frame.Save(Path.GetDirectoryName(f) + "\\" + Path.GetFileNameWithoutExtension(f) + "_" + i.ToString() + Path.GetExtension(f));
                    frame.Dispose();
                    i++;
                }
            }
        }

        #endregion


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
                            {
                                TexAnim.AnimationObject.FObjDesc.Next = new HSD_FOBJDesc();
                                TexAnim.AnimationObject.FObjDesc.Next.FromFOBJ(new HSD_FOBJ() { AnimationType = JointTrackType.HSD_A_J_SCAX, Buffer = new byte[0] });
                            }
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
        

    }
}
