using HSDRaw.Common;
using System;
using System.Drawing;
using System.Windows.Forms;
using HSDRawViewer.GUI.Dialog;
using HSDRaw.GX;
using HSDRaw.Melee;
using HSDRawViewer.Converters;
using HSDRawViewer.Tools;
using HSDRaw.Tools;

namespace HSDRawViewer.GUI.Plugins
{
    [SupportedTypes(new Type[] 
    { 
        typeof(HSD_TOBJ), 
        typeof(SBM_MemCardBanner), 
        typeof(SBM_MemCardIcon),
    })]
    public partial class TOBJEditor : PluginBase
    {
        /// <summary>
        /// 
        /// </summary>
        public TOBJEditor()
        {
            InitializeComponent();

            FormClosing += (sender, args) =>
            {
                if(texturePanel.Image != null)
                {
                    texturePanel.Image.Dispose();
                    texturePanel.Image = null;
                }
            };
        }

        public override DataNode Node
        {
            get => _node;
            set
            {
                _node = value;
                RefreshControl();
            }
        }
        private DataNode _node;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Bitmap GetImage()
        {
            // get tobj image
            if (_node.Accessor is HSD_TOBJ tobj && 
                tobj.ImageData != null)
            {
                if (tobj.LOD != null && tobj.ImageData.MaxLOD > 0)
                {
                    customPaintTrackBar1.Maximum = (int)tobj.ImageData.MaxLOD - 1;
                    panel1.Visible = true;
                    return BitmapTools.BGRAToBitmap(
                        tobj.GetDecodedImageData(customPaintTrackBar1.Value),
                        (int)Math.Ceiling(tobj.ImageData.Width / Math.Pow(2, customPaintTrackBar1.Value)),
                        (int)Math.Ceiling(tobj.ImageData.Height / Math.Pow(2, customPaintTrackBar1.Value)));
                }
                else
                {
                    return TOBJConverter.ToBitmap(tobj);
                }
            }
            else
            if (_node.Accessor is SBM_MemCardBanner banner)
            {
                var decoded = GXImageConverter.DecodeTPL(GXTexFmt.RGB5A3, 96, 32, banner._s.GetData(), 0);
                return BitmapTools.BGRAToBitmap(decoded, 96, 32);
            }
            else
            if (_node.Accessor is SBM_MemCardIcon icon)
            {
                var desc = icon._s.GetData();
                var decoded = GXImageConverter.DecodeTPL(GXTexFmt.CI8, 32, 32, desc, GXTlutFmt.RGB5A3, 256, icon._s.GetBytes(0x400, 256 * 2), 0);
                return BitmapTools.BGRAToBitmap(decoded, 32, 32);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="fmt"></param>
        /// <param name="pal"></param>
        public void SetImage(Bitmap bmp, GXTexFmt fmt, GXTlutFmt pal)
        {
            var brga = bmp.GetBGRAData();

            if (_node.Accessor is HSD_TOBJ tobj)
            {
                tobj.ImageData = new HSD_Image();
                tobj.EncodeImageData(brga, bmp.Width, bmp.Height, fmt, pal);
            }
            else
            if (_node.Accessor is SBM_MemCardBanner banner)
            {
                if (bmp.Width != 96 || bmp.Height != 32)
                {
                    MessageBox.Show("Banner Image must be 96x32", "Image Size Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    banner._s.SetData(GXImageConverter.EncodeImage(brga, 96, 32, GXTexFmt.RGB5A3, GXTlutFmt.RGB5A3, out byte[] palData));
                }
            }
            else
            if (_node.Accessor is SBM_MemCardIcon icon)
            {
                if (bmp.Width != 32 || bmp.Height != 32)
                {
                    MessageBox.Show("Icon Image must be 32x32", "Image Size Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    var imgdata = GXImageConverter.EncodeImage(brga, 32, 32, GXTexFmt.CI8, GXTlutFmt.RGB5A3, out byte[] palData);
                    Array.Resize(ref imgdata, imgdata.Length + palData.Length);
                    Array.Copy(palData, 0, imgdata, imgdata.Length, palData.Length);
                    icon._s.SetData(imgdata);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshControl()
        {
            if (texturePanel.Image != null)
            {
                texturePanel.Image.Dispose();
                texturePanel.Image = null;
            }

            propertyGrid.SelectedObject = null;
            panel1.Visible = false;

            texturePanel.Image = GetImage();

            propertyGrid.SelectedObject = _node.Accessor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var export = Tools.FileIO.SaveFile(ApplicationSettings.ImageFileFilter);

            if (export != null)
                using (var bmp = GetImage())
                    bmp.Save(export);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var import = Tools.FileIO.OpenFile(ApplicationSettings.ImageFileFilter);

            if (import != null)
            {
                using (TextureImportDialog d = new TextureImportDialog())
                {
                    if(d.ShowDialog() == DialogResult.OK)
                    {
                        using (var bmp = new Bitmap(import))
                        {
                            d.ApplySettings(bmp);
                            SetImage(bmp, d.TextureFormat, d.PaletteFormat);
                        }
                    }
                }
            }

            RefreshControl();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customPaintTrackBar1_ValueChanged(object sender, EventArgs e)
        {
            RefreshControl();
        }
    }
}
