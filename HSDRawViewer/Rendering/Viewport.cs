using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Windows.Forms;
using HSDRaw;
using HSDRaw.Common.Animation;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.Rendering
{
    public class Viewport : DockContent
    {
        private GroupBox _animationGroup;
        private Button _animationPlayButton;
        private static TrackBar _animationTrackBar;

        private GLControl _glViewport;

        public int ViewportWidth => _glViewport.Width;
        public int ViewportHeight => _glViewport.Height;

        public Camera Camera;

        public bool ReadyToRender { get; internal set; } = false;
        
        private Renderer _renderer { get; set; }
        public HSDAccessor SelectedAccessor
        {
            internal get { return _selectedAccessor; }
            set {
                if (value is HSD_FigaTree tree)
                    _renderer.SetFigaTree(tree);
                else
                if (value is HSD_AnimJoint joint)
                    _renderer.SetAnimJoint(joint);
                else
                {
                    _renderer.SetFigaTree(null);
                    _selectedAccessor = value;
                }
                ResetCamera();
            }
        }

        private HSDAccessor _selectedAccessor { get; set; }


        // Animation Track
        public static int AnimSpeed = 1;
        public static int MaxFrame { get => _animationTrackBar.Maximum; set => _animationTrackBar.Maximum = value; }
        public static int Frame
        {
            get
            {
                return _animationTrackBar.Value;
            }
            set
            {
                if (value < _animationTrackBar.Minimum)
                    value = _animationTrackBar.Maximum;

                if (value > _animationTrackBar.Maximum)
                    value = _animationTrackBar.Minimum;

                _animationTrackBar.Value = value;
            }
        }
        public static bool EnableAnimationTrack
        {
            get => _enableAnimationTrack;
                set
            {
                _enableAnimationTrack = value;
            }
        }
        private static bool _enableAnimationTrack = false;

        private int FrameTimer = 0;
        private bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;

                if (IsPlaying)
                    _animationPlayButton.Text = "Pause";
                else
                    _animationPlayButton.Text = "Play";
            }
        }
        private bool _isPlaying = false;
        
        public Viewport()
        {
            Text = "Viewport";

            _glViewport = new GLControl(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 8, 16));
            _glViewport.Load += Viewport_Loaded;
            _glViewport.Resize += Viewport_Resize;
            _glViewport.MouseMove += Viewport_MouseMove;
            _glViewport.KeyPress += Viewport_KeyDown;
            _glViewport.Paint += Render;
            _glViewport.AutoSize = true;
            _glViewport.Dock = DockStyle.Fill;

            _renderer = new Renderer();

            _animationGroup = new GroupBox();
            _animationGroup.Visible = false;
            _animationGroup.Text = "Animation Track";
            _animationGroup.Dock = DockStyle.Bottom;

            _animationPlayButton = new Button();
            _animationPlayButton.Text = "Play";
            _animationPlayButton.Click += (sender, args) =>
            {
                IsPlaying = !IsPlaying;
            };
            _animationPlayButton.Dock = DockStyle.Fill;

            _animationTrackBar = new TrackBar();
            _animationTrackBar.Dock = DockStyle.Top;

            _animationGroup.Controls.Add(_animationPlayButton);
            _animationGroup.Controls.Add(_animationTrackBar);

            ClearControls();

            Application.Idle += RenderLoop;

            FormClosing += (sender, args) =>
            {
                if (args.CloseReason == CloseReason.UserClosing)
                {
                    args.Cancel = true;
                    MainForm.Instance.TryClose(this);
                }
            };
        }

        public void RenderLoop(object sender, EventArgs args)
        {
            if (ReadyToRender && EnableAnimationTrack)
            {
                _glViewport.Invalidate();
            }
        }

        public void ClearControls()
        {
            Controls.Clear();
            Frame = 0;
            IsPlaying = false;
            EnableAnimationTrack = false;
            Controls.Add(_glViewport);
            Controls.Add(_animationGroup);
        }

        private void ResetCamera()
        {
            Camera.SetDefault();
            _glViewport.Invalidate();
        }
    
        public void Render(object sender, EventArgs args)
        {
            if (!_animationGroup.Visible && EnableAnimationTrack)
            {
                _animationGroup.Visible = true;
                _animationTrackBar.Minimum = 0;
            }

            if (_animationGroup.Visible && !EnableAnimationTrack)
                _animationGroup.Visible = false;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.MatrixMode(MatrixMode.Modelview);
            var t = Camera.Transform;
            GL.LoadMatrix(ref t);

            if (IsPlaying)
            {
                FrameTimer++;
                if (FrameTimer > AnimSpeed)
                {
                    IncrementFrame();
                    FrameTimer = 0;
                }
            }

            // rendering here
            if (SelectedAccessor != null)
                _renderer.Render(Camera.Transform, SelectedAccessor, this);
            
            _glViewport.SwapBuffers();
        }

        public void AddToolStrip(ToolStrip t)
        {
            Controls.Add(t);
            _glViewport.BringToFront();
        }

        public static void IncrementFrame()
        {
            Frame++;
        }

        public static void DecrementFrame()
        {
            Frame--;
        }
        
        private void Viewport_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, _glViewport.Width, _glViewport.Height);
            if(Camera != null)
                Camera.SetViewSize(_glViewport.Width, _glViewport.Height);
            _glViewport.Invalidate();
        }

        private void Viewport_Loaded(object sender, EventArgs args)
        {
            GL.ClearColor(Color.FromArgb((0xFF << 24) | 0x333333));
            Camera = new Camera(_glViewport.Width, _glViewport.Height);
            ReadyToRender = true;
        }


        float PrevX = 0, PrevY = 0;
        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            float speed = 1 / Vector3.TransformPosition(Vector3.Zero, Camera.Transform).LengthFast;
            speed = (1 - speed) * 1;
            if (e.Button == MouseButtons.Left)
            {
                Camera.YRotation -= (PrevX - e.X) / 50f;
                Camera.XRotation -= (PrevY - e.Y) / 50f;
            }
            if (e.Button == MouseButtons.Right)
            {
                Camera.X -= (PrevX - e.X) * speed;
                Camera.Y += (PrevY - e.Y) * speed;
            }
            PrevX = e.X;
            PrevY = e.Y;

            _glViewport.Invalidate();
        }

        public void Viewport_KeyDown(object sender, System.Windows.Forms.KeyPressEventArgs args)
        {
            if (args.KeyChar == 'w')
                Camera.Z += 5f;

            if (args.KeyChar == 's')
                Camera.Z -= 5f;

            _glViewport.Invalidate();
        }

    }
}
