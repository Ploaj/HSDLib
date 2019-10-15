using System;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Renderers;
using System.Collections.Generic;
using HSDRaw;
using OpenTK;

namespace HSDRawViewer.GUI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ViewportControl : UserControl
    {
        private Camera _camera;

        private bool ReadyToRender = false;

        public float Frame
        {
            get
            {
                return _frame;
            }
            set
            {
                if (value > MaxFrame)
                    value = 0;
                if (value < 0)
                    value = MaxFrame;
                _frame = value;
                nudFrame.Value = (decimal)value;
                animationTrack.Value = (int)value;
            }
        }
        private float _frame;

        public float MaxFrame
        {
            get
            {
                return (float)nudMaxFrame.Value;
            }
            set
            {
                animationTrack.Maximum = (int)value;
                nudMaxFrame.Value = (decimal)value;
            }
        }

        public bool AnimationTrackEnabled
        {
            set
            {
                animationGroup.Visible = value;
            }
        }

        private List<Tuple<HSDAccessor, IRenderer>> Renderers { get; } = new List<Tuple<HSDAccessor, IRenderer>>();

        public ViewportControl()
        {
            InitializeComponent();
        }

        ~ViewportControl()
        {
            if(Renderers != null)
                foreach(var r in Renderers)
                    r.Item2.Clear();
        }

        public void AddRenderer(HSDAccessor accessor, IRenderer r)
        {
            Renderers.Add(new Tuple<HSDAccessor, IRenderer>(accessor, r));
        }

        public void RemoveRenderer(HSDAccessor accessor)
        {
            var r = Renderers.Find(e => e.Item1 == accessor);
            if(r != null)
            {
                r.Item2.Clear();
                Renderers.Remove(r);
            }
        }

        private void nudFrame_ValueChanged(object sender, EventArgs e)
        {
            if (Frame != (float)nudFrame.Value)
                Frame = (float)nudFrame.Value;
        }

        private void animationTrack_ValueChanged(object sender, EventArgs e)
        {
            if (Frame != animationTrack.Value)
                Frame = animationTrack.Value;
        }

        private void buttonSeekStart_Click(object sender, EventArgs e)
        {
            Frame = 0;
        }

        private void buttonSeekEnd_Click(object sender, EventArgs e)
        {
            Frame = MaxFrame;
        }

        private void buttonNextFrame_Click(object sender, EventArgs e)
        {
            Frame++;
        }

        private void buttonPrevFrame_Click(object sender, EventArgs e)
        {
            Frame--;
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void Render()
        {
            panel1.Invalidate();
            AnimationTrackEnabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (!ReadyToRender)
                return;

            panel1.MakeCurrent();
            GL.Viewport(0, 0, panel1.Width, panel1.Height);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            var v = _camera.Transform;
            GL.LoadMatrix(ref v);

            foreach(var r in Renderers)
            {
                r.Item2.Render(r.Item1, panel1.Width, panel1.Height);
            }
            
            panel1.SwapBuffers();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void FrameView(IList<Vector2> points)
        {
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            foreach (var v in points)
            {
                min.X = Math.Min(min.X, v.X);
                min.Y = Math.Min(min.Y, v.Y);
                max.X = Math.Max(max.X, v.X);
                max.Y = Math.Max(max.Y, v.Y);
            }

            //_camera.Z = Math.Max(max.X - min.X, max.Y - min.Y);
            //_camera.X = (max.X + min.X) / 2;
            //_camera.Y = (max.Y + min.Y) / 2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.FromArgb((0xFF << 24) | 0x333333));
            _camera = new Camera(panel1.Width, panel1.Height);
            ReadyToRender = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Resize(object sender, EventArgs e)
        {
            if (_camera != null)
                _camera.SetViewSize(panel1.Width, panel1.Height);
        }
    }
}
