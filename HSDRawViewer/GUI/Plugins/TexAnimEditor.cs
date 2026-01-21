using HSDRaw.Common.Animation;
using HSDRawViewer.GUI.Dialog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins
{
    [SupportedTypes(new Type[] { typeof(HSD_TexAnim) })]
    public partial class TexAnimEditor : PluginBase
    {
        private HSD_TexAnim _texAnim => _node.Accessor as HSD_TexAnim;

        public TOBJProxy[] Images { get; set; }

        public override DataNode Node
        {
            get => _node;
            set
            {
                _node = value;
                LoadTextures();
                SelectNode();
            }
        }
        private DataNode _node;

        /// <summary>
        /// 
        /// </summary>
        public TexAnimEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadTextures()
        {
            if (_texAnim == null)
                return;

            Images = _texAnim.ToTOBJs().Select(e => new TOBJProxy() { TOBJ = e }).ToArray();
            arrayMemberEditor1.SetArrayFromProperty(this, nameof(Images));

            if (_texAnim.AnimationObject != null)
                graphEditor1.LoadTracks(GUI.Controls.GraphEditor.AnimType.Texture, _texAnim.AnimationObject);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveTextures()
        {
            if (_texAnim == null)
                return;

            Console.WriteLine(Images.Length);

            _texAnim.FromTOBJs(Images.Select(e => e.TOBJ).ToArray(), false);
            arrayMemberEditor1.Invalidate();
            texturePanel1.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SelectNode()
        {
            if (texturePanel1.Image != null)
            {
                texturePanel1.Image.Dispose();
                texturePanel1.Image = null;
            }

            if (arrayMemberEditor1.SelectedObject is TOBJProxy proxy)
                texturePanel1.Image = proxy.TOBJ.ToImage().ToBitmap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void arrayMemberEditor1_SelectedObjectChanged(object sender, EventArgs e)
        {
            SelectNode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void arrayMemberEditor1_ArrayUpdated(object sender, EventArgs e)
        {
            SaveTextures();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExport_Click(object sender, EventArgs e)
        {
            if (arrayMemberEditor1.SelectedObject is TOBJProxy proxy)
            {
                string export = Tools.FileIO.SaveFile(ApplicationSettings.ImageFileFilter);

                if (export != null)
                {
                    proxy.ToImage().Save(export);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExportAll_Click(object sender, EventArgs e)
        {
            if (Images != null && Images.Length > 0)
            {
                string export = Tools.FileIO.SaveFile(ApplicationSettings.ImageFileFilter);

                if (export != null)
                {
                    string path = System.IO.Path.GetDirectoryName(export);
                    string fname = System.IO.Path.GetFileNameWithoutExtension(export);
                    string extension = System.IO.Path.GetExtension(export);
                    int index = 0;
                    foreach (TOBJProxy v in Images)
                    {
                        v.TOBJ.SaveImagePNG($"{path}/{fname}_{index.ToString("D2")}{extension}");
                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonImport_Click(object sender, EventArgs e)
        {
            string[] imports = Tools.FileIO.OpenFiles(ApplicationSettings.ImageFileFilter);

            if (imports != null)
            {
                using TextureImportDialog d = new();
                if (Images.Length > 0)
                {
                    TOBJProxy proxy = Images[0];

                    d.TextureFormat = proxy.TOBJ.ImageData.Format;

                    if (proxy.TOBJ.TlutData != null)
                        d.PaletteFormat = proxy.TOBJ.TlutData.Format;
                }

                if (d.ShowDialog() == DialogResult.OK)
                {
                    HSDRaw.GX.GXTexFmt texFmt = d.TextureFormat;
                    HSDRaw.GX.GXTlutFmt palFmt = d.PaletteFormat;

                    foreach (string import in imports)
                        using (Image<Bgra32> bmp = SixLabors.ImageSharp.Image.Load<Bgra32>(import))
                        {
                            d.ApplySettings(bmp);
                            arrayMemberEditor1.AddItem(new TOBJProxy() { TOBJ = bmp.ToTObj(texFmt, palFmt) });
                        }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReplace_Click(object sender, EventArgs e)
        {
            if (arrayMemberEditor1.SelectedObject is TOBJProxy proxy)
            {
                string import = Tools.FileIO.OpenFile(ApplicationSettings.ImageFileFilter);

                if (import != null)
                {
                    using TextureImportDialog d = new();
                    d.TextureFormat = proxy.TOBJ.ImageData.Format;

                    if (proxy.TOBJ.TlutData != null)
                        d.PaletteFormat = proxy.TOBJ.TlutData.Format;

                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        using (Image<Bgra32> bmp = SixLabors.ImageSharp.Image.Load<Bgra32>(import))
                        {
                            d.ApplySettings(bmp);
                            proxy.TOBJ.EncodeImageData(bmp.ToTObj(d.TextureFormat, d.PaletteFormat).GetDecodedImageData(), bmp.Width, bmp.Height, d.TextureFormat, d.PaletteFormat);
                        }

                        SaveTextures();
                        SelectNode();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            _texAnim.AnimationObject.FObjDesc = graphEditor1.ToFOBJs();
            MessageBox.Show("Saved Animation Changes");
        }
    }
}
