using System.Windows.Forms;
using HSDRaw;
using HSDRaw.Common.Animation;
using WeifenLuo.WinFormsUI.Docking;
using HSDRawViewer.GUI;

namespace HSDRawViewer.Rendering
{
    public class CommonViewport : DockContent
    {
        private GroupBox _animationGroup;
        private Button _animationPlayButton;
        private static TrackBar _animationTrackBar;

        public ViewportControl glViewport;

        public int ViewportWidth => glViewport.Width;
        public int ViewportHeight => glViewport.Height;

        public Camera Camera;

        public bool ReadyToRender { get; internal set; } = false;
        
        private HSDAccessor _selectedAccessor { get; set; }
        
        public CommonViewport()
        {
            Text = "Viewport";

            //_glViewport = new GLControl(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 8, 16));
            glViewport = new ViewportControl();
            glViewport.Dock = DockStyle.Fill;

            Controls.Add(glViewport);
            
            FormClosing += (sender, args) =>
            {
                if (args.CloseReason == CloseReason.UserClosing)
                {
                    args.Cancel = true;
                    MainForm.Instance.TryClose(this);
                }
            };
        }
    }
}
