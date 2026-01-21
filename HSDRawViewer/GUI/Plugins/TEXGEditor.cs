using HSDRaw.Common;
using HSDRawViewer.GUI.Dialog;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins
{
    [SupportedTypes(new Type[] { typeof(HSD_TexGraphic) })]
    public partial class TEXGEditor : PluginBase
    {
        private HSD_TexGraphic _texGraphic => _node.Accessor as HSD_TexGraphic;

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
        public TEXGEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadTextures()
        {
            if (_texGraphic == null)
                return;

            Images = _texGraphic.ConvertToTOBJs().Select(e => new TOBJProxy() { TOBJ = e }).ToArray();
            arrayMemberEditor1.SetArrayFromProperty(this, nameof(Images));
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveTextures()
        {
            if (_texGraphic == null)
                return;

            _texGraphic.SetFromTOBJs(Images.Select(e => e.TOBJ).ToArray());
            arrayMemberEditor1.Invalidate();
            _node.Refresh();
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
                    proxy.TOBJ.SaveImagePNG();
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
                        using (System.Drawing.Image bmp = v.ToImage())
                            bmp.Save($"{path}/{fname}_{index.ToString("D2")}{extension}");
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
            {
                string[] imports = Tools.FileIO.OpenFiles(ApplicationSettings.ImageFileFilter);

                if (imports != null)
                {
                    HSDRaw.GX.GXTexFmt texFmt = _texGraphic.ImageFormat;
                    HSDRaw.GX.GXTlutFmt palFmt = _texGraphic.PaletteFormat;
                    int width = -1;
                    int height = -1;

                    if (Images.Length == 0)
                    {
                        using TextureImportDialog d = new();
                        if (d.ShowDialog() == DialogResult.OK)
                        {
                            texFmt = d.TextureFormat;
                            palFmt = d.PaletteFormat;
                        }
                        else
                            return;
                    }

                    if (Images.Length > 0)
                    {
                        width = _texGraphic.Width;
                        height = _texGraphic.Height;
                    }

                    foreach (string import in imports)
                        using (SixLabors.ImageSharp.Image<Bgra32> bmp = SixLabors.ImageSharp.Image.Load<Bgra32>(import))
                        {
                            if (width == -1 || height == -1)
                            {
                                width = bmp.Width;
                                height = bmp.Height;
                            }

                            if (bmp.Width != width || bmp.Height != height)
                            {
                                MessageBox.Show($"Incorrect Dimensions.\nFile: {System.IO.Path.GetFileName(import)}\nExpected:{width}x{height} got {bmp.Width}x{bmp.Height}");
                                continue;
                            }

                            arrayMemberEditor1.AddItem(new TOBJProxy() { TOBJ = bmp.ToTObj(texFmt, palFmt) });
                        }

                    SaveTextures();
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
                    HSDRaw.GX.GXTexFmt texFmt = _texGraphic.ImageFormat;
                    HSDRaw.GX.GXTlutFmt palFmt = _texGraphic.PaletteFormat;

                    using (SixLabors.ImageSharp.Image<Bgra32> bmp = SixLabors.ImageSharp.Image.Load<Bgra32>(import))
                        proxy.TOBJ.EncodeImageData(bmp.ToTObj(texFmt, palFmt).GetDecodedImageData(), _texGraphic.Width, _texGraphic.Height, texFmt, palFmt);

                    SaveTextures();
                }
            }
        }
    }
}
