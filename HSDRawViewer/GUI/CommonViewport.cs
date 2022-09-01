using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using HSDRawViewer.Rendering;

namespace HSDRawViewer.GUI
{
    public class CommonViewport : DockContent, IDrawable
    {
        public ViewportControl glViewport;

        public int ViewportWidth => glViewport.Width;
        public int ViewportHeight => glViewport.Height;

        public Camera Camera;

        public DrawOrder DrawOrder => DrawOrder.First;

        /// <summary>
        /// 
        /// </summary>
        public CommonViewport()
        {
            Text = "Viewport";

            //_glViewport = new GLControl(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 8, 16));
            glViewport = new ViewportControl();
            glViewport.Dock = DockStyle.Fill;
            glViewport.DisplayGrid = true;

            glViewport.AddRenderer(this);
            Controls.Add(glViewport);

            FormClosing += (sender, args) =>
            {
                if (args.CloseReason == CloseReason.UserClosing)
                {
                    args.Cancel = true;
                    //MainForm.Instance.TryClose(this);
                }
            };
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
        }

        public void GLInit()
        {
        }

        public void GLFree()
        {
        }
    }
}
