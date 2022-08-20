using HSDRaw;
using HSDRaw.Common;
using HSDRawViewer.Converters;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Controls.JObjEditor
{
    public partial class DockableTextureEditor : DockContent
    {
        private HSD_MOBJ _mobj { get => _proxy?.DOBJ?.Mobj; }
        private DObjProxy _proxy;

        public TObjProxy[] TextureLists { get; set; }

        public delegate void TObjSelected(DObjProxy dobj, TObjProxy tobj, int index);
        public TObjSelected SelectTObj;

        public delegate void InvalidateTextures();
        public InvalidateTextures InvalidateTexture;

        private bool InitializingList = false;

        /// <summary>
        /// 
        /// </summary>
        public DockableTextureEditor()
        {
            InitializeComponent();

            Text = "Texture Editor";

            // prevent user closing
            CloseButtonVisible = false;
            FormClosing += (sender, args) =>
            {
                if (args.CloseReason == CloseReason.UserClosing)
                {
                    args.Cancel = true;
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobj"></param>
        public void SetTextures(DObjProxy dobj)
        {
            _proxy = null;

            ClearTextureList();

            _proxy = dobj;

            PopulateTextureList();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearTextureList()
        {
            if (TextureLists != null)
            {
                TextureLists = new TObjProxy[0];
                textureArrayEditor.SetArrayFromProperty(this, "TextureLists");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void PopulateTextureList()
        {
            if (_mobj == null || _mobj.Textures == null)
                return;

            InitializingList = true;

            TextureLists = _mobj.Textures.List.Select(e => new TObjProxy(e)).ToArray();
            textureArrayEditor.SetArrayFromProperty(this, "TextureLists");

            InitializingList = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textureArrayEditor_ArrayUpdated(object sender, System.EventArgs e)
        {
            // make sure material is not null
            if (_mobj == null|| InitializingList)
                return;

            // clear all flags by default
            _mobj.RenderFlags &= ~(RENDER_MODE.TEX0 | RENDER_MODE.TEX1 | RENDER_MODE.TEX2 | RENDER_MODE.TEX3 | RENDER_MODE.TEX4 | RENDER_MODE.TEX5 | RENDER_MODE.TEX6 | RENDER_MODE.TEX7);

            // no textures
            if (TextureLists.Length == 0)
                _mobj.Textures = null;

            // update mobj textures
            for (int i = 0; i < TextureLists.Length; i++)
            {
                // enable material flag for texture
                if (i <= 7)
                    _mobj.RenderFlags |= (RENDER_MODE)(1 << (4 + i));

                // update map id
                TextureLists[i].TOBJ.TexMapID = HSDRaw.GX.GXTexMapID.GX_TEXMAP0 + i;

                // update linkage
                if (i < TextureLists.Length - 1)
                    TextureLists[i].TOBJ.Next = TextureLists[i + 1].TOBJ;
                else
                    TextureLists[i].TOBJ.Next = null;
            }

            // invoke update for rendering
            InvalidateTexture?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textureArrayEditor_SelectedObjectChanged(object sender, System.EventArgs e)
        {
            // make sure material is not null
            if (_mobj == null)
                return;

            // 
            if (textureArrayEditor.SelectedObject is TObjProxy p)
                SelectTObj?.Invoke(_proxy, p, textureArrayEditor.SelectedIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSaveTexture_Click(object sender, System.EventArgs e)
        {
            if (textureArrayEditor.SelectedObject is TObjProxy proxy)
            {
                proxy.TOBJ.ExportTOBJToFile();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, System.EventArgs e)
        {
            var tobj = TOBJConverter.ImportTOBJFromFile();

            if (tobj != null)
            {
                // add tobj
                textureArrayEditor.AddItem(new TObjProxy(tobj));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReplace_Click(object sender, System.EventArgs e)
        {
            if (textureArrayEditor.SelectedObject is TObjProxy proxy)
            {
                var tobj = TOBJConverter.ImportTOBJFromFile();

                if (tobj != null)
                {
                    // replace tobj
                    proxy.TOBJ.ImageData = tobj.ImageData;
                    proxy.TOBJ.TlutData = tobj.TlutData;

                    // update flags
                    textureArrayEditor_ArrayUpdated(null, null);

                    // refresh
                    textureArrayEditor.Invalidate();
                }
            }
        }
    }
}
