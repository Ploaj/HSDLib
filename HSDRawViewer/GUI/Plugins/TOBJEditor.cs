using HSDRaw.Common;
using System;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins
{
    [SupportedTypes(new Type[] { typeof(HSD_TOBJ) })]
    public partial class TOBJEditor : DockContent, EditorBase
    {
        private HSD_TOBJ _tobj => _node.Accessor as HSD_TOBJ;

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

        public DockState DefaultDockState => DockState.Document;

        public DataNode Node
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
        private void RefreshControl()
        {
            if (texturePanel.Image != null)
            {
                texturePanel.Image.Dispose();
                texturePanel.Image = null;
            }

            propertyGrid.SelectedObject = null;
            panel1.Visible = false;

            if (_tobj != null && _tobj.ImageData != null)
            {
                if(_tobj.LOD != null && _tobj.ImageData.MaxLOD > 0)
                {
                    customPaintTrackBar1.Maximum = (int)_tobj.ImageData.MaxLOD - 1;
                    texturePanel.Image = Tools.BitmapTools.BGRAToBitmap(
                        _tobj.GetDecodedImageData(customPaintTrackBar1.Value), 
                        (int)Math.Ceiling(_tobj.ImageData.Width / Math.Pow(2, customPaintTrackBar1.Value)),
                        (int)Math.Ceiling(_tobj.ImageData.Height / Math.Pow(2, customPaintTrackBar1.Value)));
                    panel1.Visible = true;
                }
                else
                {
                    texturePanel.Image = Converters.TOBJConverter.ToBitmap(_tobj);
                }
                propertyGrid.SelectedObject = _tobj;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (_tobj == null)
                return;

            var export = Tools.FileIO.SaveFile(ApplicationSettings.ImageFileFilter);

            if (export != null)
                using (var bmp = Converters.TOBJConverter.ToBitmap(_tobj))
                    bmp.Save(export);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (_tobj == null)
                return;

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
                            _tobj.EncodeImageData(bmp.GetBGRAData(), bmp.Width, bmp.Height, d.TextureFormat, d.PaletteFormat);
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
