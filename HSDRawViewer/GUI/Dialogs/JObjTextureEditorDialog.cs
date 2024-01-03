using HSDRaw;
using HSDRaw.Common;
using HSDRaw.GX;
using HSDRawViewer.Converters;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Extra
{
    public partial class JObjTextureEditorDialog : Form
    {

        public class TextureListProxy : ImageArrayItem
        {
            private List<HSD_TOBJ> tobjs = new List<HSD_TOBJ>();

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
                foreach (var t in tobjs)
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
                    tobjs[0].ExportTOBJToFile();
            }

            public void Export(string file_path)
            {
                if (tobjs.Count > 0)
                    using (var bmp = tobjs[0].ToBitmap())
                        bmp.Save(file_path);
            }

            public Image ToImage()
            {
                if (tobjs.Count > 0)
                    return TOBJConverter.ToBitmap(tobjs[0]);
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

            var tex = new List<TextureListProxy>();

            foreach (var jobj in root.TreeList)
            {
                if (jobj.Dobj == null)
                    continue;

                foreach (var dobj in jobj.Dobj.List)
                {
                    if (dobj.Mobj == null || dobj.Mobj.Textures == null)
                        continue;

                    foreach (var tobj in dobj.Mobj.Textures.List)
                    {
                        var hash = HSDRawFile.ComputeHash(tobj.GetDecodedImageData());

                        var proxy = tex.Find(e => e._hash == hash);

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
            textureArrayEditor.SetArrayFromProperty(this, "TextureLists");
        }

        /// <summary>
        /// 
        /// </summary>
        private void UnloadTextureList()
        {
            if (TextureLists != null)
            {
                foreach (var t in TextureLists)
                    t.Dispose();
                TextureLists = new TextureListProxy[0];
                textureArrayEditor.SetArrayFromProperty(this, "TextureLists");
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
                if (!TOBJConverter.FormatFromString(f, out GXTexFmt imgFormat, out GXTlutFmt palFormat))
                {
                    using (var teximport = new TextureImportDialog())
                        if (teximport.ShowDialog() == DialogResult.OK)
                        {
                            imgFormat = teximport.TextureFormat;
                            palFormat = teximport.PaletteFormat;
                        }
                        else
                            return;
                }

                proxy.Replace(TOBJConverter.ImportTOBJFromFile(f, imgFormat, palFormat));
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
            var f = FileIO.OpenFile(ApplicationSettings.ImageFileFilter);
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var path = FileIO.OpenFolder();

            if (path != null)
            {
                int ti = 0;
                foreach (var proxy in TextureLists)
                {
                    proxy.Export(path + $"\\{TOBJConverter.FormatName($"Texture_{ti++}_", proxy.GetTObj())}.png");
                }
            }
        }
    }
}
