using HSDRaw.Common;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HSDRawViewer.GUI.Dialog;

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
            arrayMemberEditor1.SetArrayFromProperty(this, "Images");
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
                texturePanel1.Image = Converters.TOBJConverter.ToBitmap(proxy.TOBJ);
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
                var export = Tools.FileIO.SaveFile(ApplicationSettings.ImageFileFilter);

                if (export != null)
                    using (var bmp = Converters.TOBJConverter.ToBitmap(proxy.TOBJ))
                        bmp.Save(export);
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
                var export = Tools.FileIO.SaveFile(ApplicationSettings.ImageFileFilter);

                if (export != null)
                {
                    var path = System.IO.Path.GetDirectoryName(export);
                    var fname = System.IO.Path.GetFileNameWithoutExtension(export);
                    var extension = System.IO.Path.GetExtension(export);
                    int index = 0;
                    foreach(var v in Images)
                    {
                        using (var bmp = Converters.TOBJConverter.ToBitmap(v.TOBJ))
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
                var imports = Tools.FileIO.OpenFiles(ApplicationSettings.ImageFileFilter);

                if (imports != null)
                {
                    var texFmt = _texGraphic.ImageFormat;
                    var palFmt = _texGraphic.PaletteFormat;
                    var width = -1;
                    var height = -1;

                    if (Images.Length == 0)
                    {
                        using (var d = new TextureImportDialog())
                            if (d.ShowDialog() == DialogResult.OK)
                            {
                                texFmt = d.TextureFormat;
                                palFmt = d.PaletteFormat;
                            }
                            else
                                return;
                    }

                    if(Images.Length > 0)
                    {
                        width = _texGraphic.Width;
                        height = _texGraphic.Height;
                    }

                    foreach (var import in imports)
                        using (var bmp = new Bitmap(import))
                        {
                            if(width == -1 || height == -1)
                            {
                                width = bmp.Width;
                                height = bmp.Height;
                            }

                            if (bmp.Width != width || bmp.Height != height)
                            {
                                MessageBox.Show($"Incorrect Dimensions.\nFile: {System.IO.Path.GetFileName(import)}\nExpected:{width}x{height} got {bmp.Width}x{bmp.Height}");
                                continue;
                            }

                            arrayMemberEditor1.AddItem(new TOBJProxy() { TOBJ = Converters.TOBJConverter.BitmapToTOBJ(bmp, texFmt, palFmt) });
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
                var import = Tools.FileIO.OpenFile(ApplicationSettings.ImageFileFilter);

                if (import != null)
                {
                    var texFmt = _texGraphic.ImageFormat;
                    var palFmt = _texGraphic.PaletteFormat;

                    using (var bmp = new Bitmap(import))
                        proxy.TOBJ.EncodeImageData(bmp.GetBGRAData(), _texGraphic.Width, _texGraphic.Height, texFmt, palFmt);

                    SaveTextures();
                }
            }
        }
    }
}
