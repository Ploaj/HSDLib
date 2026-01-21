using HSDRaw;
using HSDRaw.Common;
using HSDRaw.GX;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.Tools;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Extra
{
    public partial class JObjTextureEditorDialog : Form
    {

        public class TextureListProxy : ImageArrayItem
        {
            private readonly List<HSD_TOBJ> tobjs = new();

            //private Bitmap PreviewImage;

            public int _hash;

            public string Hash { get => _hash.ToString("X"); }

            public GXTexFmt ImageFormat { get => tobjs[0].ImageData.Format; }

            public GXTlutFmt PaletteFormat
            {
                get
                {
                    if (tobjs[0].TlutData == null)
                        return GXTlutFmt.IA8;

                    return tobjs[0].TlutData.Format;
                }
            }

            public int Width
            {
                get
                {
                    if (tobjs[0].ImageData == null)
                        return 0;

                    return tobjs[0].ImageData.Width;
                }
            }

            public int Height
            {
                get
                {
                    if (tobjs[0].ImageData == null)
                        return 0;

                    return tobjs[0].ImageData.Height;
                }
            }

            public System.Drawing.Color Color
            {
                get
                {
                    using (Image<Bgra32> img = GetTObj().ToImage())
                    {
                        Bgra32? color = img.GetSolidColor();
                        if (color != null)
                        {
                            return System.Drawing.Color.FromArgb(
                                ((color.Value.A & 0xFF) << 24) |
                                ((color.Value.R & 0xFF) << 16) |
                                ((color.Value.G & 0xFF) << 8) |
                                ((color.Value.B & 0xFF)));
                        }
                    }
                    return System.Drawing.Color.Black;
                }
            }

            public TextureListProxy(int hash)
            {
                _hash = hash;
            }

            public void Dispose()
            {
                //PreviewImage.Dispose();
            }

            public override string ToString()
            {
                return $"References: {tobjs.Count}";
            }

            public void AddTOBJ(HSD_TOBJ tobj)
            {
                //if (PreviewImage == null)
                //    PreviewImage = TOBJConverter.ToBitmap(tobj);
                tobjs.Add(tobj);
            }

            public void Replace(HSD_TOBJ newTOBJ)
            {
                foreach (HSD_TOBJ t in tobjs)
                {
                    if (newTOBJ.ImageData != null)
                    {
                        if (t.ImageData == null)
                            t.ImageData = new HSD_Image();
                        t.ImageData._s.SetReference(0x00, newTOBJ.ImageData._s.GetReference<HSDAccessor>(0x00));
                        //t.ImageData.ImageData = newTOBJ.ImageData.ImageData;
                        t.ImageData.Format = newTOBJ.ImageData.Format;
                        t.ImageData.Width = newTOBJ.ImageData.Width;
                        t.ImageData.Height = newTOBJ.ImageData.Height;
                    }
                    else
                        t.ImageData = null;

                    if (newTOBJ.TlutData != null)
                    {
                        if (t.TlutData == null)
                            t.TlutData = new HSD_Tlut();
                        t.TlutData._s.SetReference(0x00, newTOBJ.TlutData._s.GetReference<HSDAccessor>(0x00));
                        //t.TlutData.TlutData = newTOBJ.TlutData.TlutData;
                        t.TlutData.Format = newTOBJ.TlutData.Format;
                        t.TlutData.ColorCount = newTOBJ.TlutData.ColorCount;
                        t.TlutData.GXTlut = newTOBJ.TlutData.GXTlut;
                    }
                    else
                        t.TlutData = null;

                    //if(PreviewImage != null)
                    //    PreviewImage.Dispose();
                    //PreviewImage = TOBJConverter.ToBitmap(newTOBJ);
                }
            }

            public void Export()
            {
                if (tobjs.Count > 0)
                    tobjs[0].SaveImagePNG();
            }

            public void Export(string file_path)
            {
                if (tobjs.Count > 0)
                    using (Image<Bgra32> bmp = tobjs[0].ToImage())
                        bmp.Save(file_path);
            }

            public System.Drawing.Image ToImage()
            {
                if (tobjs.Count > 0)
                    return tobjs[0].ToImage().ToBitmap();
                return null;
            }

            public HSD_TOBJ GetTObj()
            {
                if (tobjs.Count > 0)
                    return tobjs[0];

                return null;
            }
        }

        public JObjTextureEditorDialog(HSD_JOBJ root)
        {
            InitializeComponent();

            LoadTextureList(root);

            CenterToScreen();
        }

        public TextureListProxy[] TextureLists { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void LoadTextureList(HSD_JOBJ root)
        {
            UnloadTextureList();

            List<TextureListProxy> tex = new();

            foreach (HSD_JOBJ jobj in root.TreeList)
            {
                if (jobj.Dobj == null)
                    continue;

                foreach (HSD_DOBJ dobj in jobj.Dobj.List)
                {
                    if (dobj.Mobj == null || dobj.Mobj.Textures == null)
                        continue;

                    foreach (HSD_TOBJ tobj in dobj.Mobj.Textures.List)
                    {
                        int hash = HSDRawFile.ComputeHash(tobj.GetDecodedImageData());

                        TextureListProxy proxy = tex.Find(e => e._hash == hash);

                        if (proxy == null)
                        {
                            proxy = new TextureListProxy(hash);
                            tex.Add(proxy);
                        }

                        proxy.AddTOBJ(tobj);
                    }
                }
            }

            TextureLists = tex.ToArray();
            textureArrayEditor.SetArrayFromProperty(this, nameof(TextureLists));
        }

        /// <summary>
        /// 
        /// </summary>
        private void UnloadTextureList()
        {
            if (TextureLists != null)
            {
                foreach (TextureListProxy t in TextureLists)
                    t.Dispose();
                TextureLists = new TextureListProxy[0];
                textureArrayEditor.SetArrayFromProperty(this, nameof(TextureLists));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        private void ReplaceTexture(string f)
        {
            if (textureArrayEditor.SelectedObject is TextureListProxy proxy)
            {
                if (!TOBJExtentions.FormatFromString(f, out GXTexFmt imgFormat, out GXTlutFmt palFormat))
                {
                    using TextureImportDialog teximport = new();
                    if (teximport.ShowDialog() == DialogResult.OK)
                    {
                        imgFormat = teximport.TextureFormat;
                        palFormat = teximport.PaletteFormat;
                    }
                    else
                        return;
                }

                proxy.Replace(TOBJExtentions.ImportTObjFromFile(f, imgFormat, palFormat));
                textureArrayEditor.Invalidate();
                textureArrayEditor.Update();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exporttoolStripButton_Click(object sender, EventArgs e)
        {
            if (textureArrayEditor.SelectedObject is TextureListProxy proxy)
            {
                proxy.Export();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void replaceTextureButton_Click(object sender, EventArgs e)
        {
            string f = FileIO.OpenFile(ApplicationSettings.ImageFileFilter);
            if (f != null)
                ReplaceTexture(f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textureArrayEditor_DragDrop(object sender, DragEventArgs e)
        {

            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (s != null && s.Length > 0)
                ReplaceTexture(s[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textureArrayEditor_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textureArrayEditor_SelectedObjectChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = textureArrayEditor.SelectedObject;

            if (pictureBox1.Image != null)
                pictureBox1.Image.Dispose();

            if (textureArrayEditor.SelectedObject is TextureListProxy proxy)
            {
                pictureBox1.Image = proxy.ToImage();
            }
            else
            {
                pictureBox1.Image = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string path = FileIO.OpenFolder();

            if (path != null)
            {
                int ti = 0;
                foreach (TextureListProxy proxy in TextureLists)
                {
                    proxy.Export(path + $"\\{proxy.GetTObj().FormatName($"Texture_{ti++}_")}.png");
                }
            }
        }

        public class EditTextureSettings
        {
            public int Width { get; set; }

            public int Height { get; set; }

            public GXTexFmt TextureFormat { get; set; }

            public GXTlutFmt PaletteFormat { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (textureArrayEditor.SelectedObject is TextureListProxy proxy)
            {
                EditTextureSettings settings = new()
                {
                    Width = proxy.Width,
                    Height = proxy.Height,
                    TextureFormat = proxy.ImageFormat,
                    PaletteFormat = proxy.PaletteFormat,
                };

                using PropertyDialog d = new("Edit Texture", settings);
                if (settings.Width < 4)
                    settings.Width = 4;

                if (settings.Height < 4)
                    settings.Height = 4;

                if (d.ShowDialog() == DialogResult.OK)
                {
                    using Image<Bgra32> img = proxy.GetTObj().ToImage();
                    img.Mutate(x => x.Resize(settings.Width, settings.Height));
                    proxy.Replace(img.ToTObj(settings.TextureFormat, settings.PaletteFormat));
                    textureArrayEditor.Invalidate();
                    textureArrayEditor.Update();
                }
            }
        }
    }
}
