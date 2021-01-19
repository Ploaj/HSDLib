using HSDRaw.Common;
using System;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins
{
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

        public Type[] SupportedTypes => new Type[] { typeof(HSD_TOBJ) };

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

            if (_tobj != null)
            {
                texturePanel.Image = Converters.TOBJConverter.ToBitmap(_tobj);
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
    }
}
