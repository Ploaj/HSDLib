using System.Windows.Forms;
using HSDRaw;
using WeifenLuo.WinFormsUI.Docking;
using HSDRawViewer.GUI;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System;

namespace HSDRawViewer.Rendering
{
    public class CommonViewport : DockContent, IDrawable
    {
        public ViewportControl glViewport;

        public int ViewportWidth => glViewport.Width;
        public int ViewportHeight => glViewport.Height;

        public Camera Camera;

        public bool ReadyToRender { get; internal set; } = false;
        
        private HSDAccessor _selectedAccessor { get; set; }

        public DrawOrder DrawOrder => DrawOrder.First;

        public CommonViewport()
        {
            Text = "Viewport";

            //_glViewport = new GLControl(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 8, 16));
            glViewport = new ViewportControl();
            glViewport.Dock = DockStyle.Fill;
            glViewport.EnableFloor = true;

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
        
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {

        }
    }
}
