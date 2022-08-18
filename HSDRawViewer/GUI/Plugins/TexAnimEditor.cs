using HSDRaw.Common.Animation;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HSDRawViewer.GUI.Dialog;

namespace HSDRawViewer.GUI.Plugins
{
    [SupportedTypes(new Type[] { typeof(HSD_TexAnim) })]
    public partial class TexAnimEditor : EditorBase
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
            arrayMemberEditor1.SetArrayFromProperty(this, "Images");

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
            var imports = Tools.FileIO.OpenFiles(ApplicationSettings.ImageFileFilter);

            if (imports != null)
            {
                using (var d = new TextureImportDialog())
                {
                    if (Images.Length > 0)
                    {
                        var proxy = Images[0];

                        d.TextureFormat = proxy.TOBJ.ImageData.Format;

                        if (proxy.TOBJ.TlutData != null)
                            d.PaletteFormat = proxy.TOBJ.TlutData.Format;
                    }

                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        var texFmt = d.TextureFormat;
                        var palFmt = d.PaletteFormat;

                        foreach (var import in imports)
                            using (var bmp = new Bitmap(import))
                            {
                                d.ApplySettings(bmp);
                                arrayMemberEditor1.AddItem(new TOBJProxy() { TOBJ = Converters.TOBJConverter.BitmapToTOBJ(bmp, texFmt, palFmt) });
                            }
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
                var import = Tools.FileIO.OpenFile(ApplicationSettings.ImageFileFilter);

                if (import != null)
                {
                    using (var d = new TextureImportDialog())
                    {
                        d.TextureFormat = proxy.TOBJ.ImageData.Format;

                        if (proxy.TOBJ.TlutData != null)
                            d.PaletteFormat = proxy.TOBJ.TlutData.Format;

                        if (d.ShowDialog() == DialogResult.OK)
                        {
                            using (var bmp = new Bitmap(import))
                            {
                                d.ApplySettings(bmp);
                                proxy.TOBJ.EncodeImageData(bmp.GetBGRAData(), bmp.Width, bmp.Height, d.TextureFormat, d.PaletteFormat);
                            }

                            SaveTextures();
                            SelectNode();
                        }
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
