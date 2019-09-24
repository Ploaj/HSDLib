using HSDRaw.Common;
using System.Windows.Forms;

namespace HSDRawViewer.Rendering.Renderers
{
    public class RendererTOBJ
    {
        public static ToolStrip ToolStrip
        {
            get
            {
                if (_toolStrip == null)
                {
                    InitToolStrip();
                }
                return _toolStrip;
            }
        }
        private static ToolStrip _toolStrip;

        private static HSD_TOBJ Tobj;

        private static TextureManager Manager = new TextureManager();

        private static bool ActualSize = false;

        private static void InitToolStrip()
        {
            _toolStrip = new ToolStrip();

            ToolStripButton toggleSize = new ToolStripButton("Toggle Size");
            toggleSize.Click += (sender, args) =>
            {
                ActualSize = !ActualSize;
            };
            _toolStrip.Items.Add(toggleSize);
        }

        public static void Render(HSD_TOBJ tobj, int windowWidth, int windowHeight)
        {
            if(Tobj != tobj)
            {
                Manager.ClearTextures();
                Tobj = tobj;
                Manager.Add(tobj.GetDecodedImageData(), tobj.ImageData.Width, tobj.ImageData.Height);
            }

            if (tobj == null || tobj.ImageData == null)
                return;

            Manager.RenderTexture(0, windowWidth, windowHeight, ActualSize, tobj.ImageData.Width, tobj.ImageData.Height);

        }
    }
}
