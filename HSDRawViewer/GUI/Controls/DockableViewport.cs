using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Controls
{
    public partial class DockableViewport : DockContent
    {
        public ViewportControl glViewport;

        public delegate void GLLoaded();
        public GLLoaded GLLoad;

        public DockableViewport()
        {
            InitializeComponent();

            // name
            Text = "Viewport";

            //_glViewport = new GLControl(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 8, 16));
            glViewport = new ViewportControl();
            glViewport.Dock = DockStyle.Fill;
            glViewport.DisplayGrid = true;

            // add viewport
            Controls.Add(glViewport);

            // disable ability to close
            CloseButtonVisible = false;

            glViewport.Load += (s, a) =>
            {
                GLLoad?.Invoke();
            };

            // prevent user closing
            FormClosing += (sender, args) =>
            {
                if (args.CloseReason == CloseReason.UserClosing)
                {
                    args.Cancel = true;
                }
            };
        }
    }
}
