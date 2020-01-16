using System;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using HSDRawViewer.Rendering;
using System.Collections.Generic;
using OpenTK;
using System.Timers;
using System.Linq;
using OpenTK.Input;

namespace HSDRawViewer.GUI
{
    ///new GLControl(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 8, 16));
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
                UpdateFrame((decimal)_frame);
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
                nudFrame.Maximum = (decimal)value;
                nudMaxFrame.Maximum = (decimal)value;
                nudMaxFrame.Value = (decimal)value;
            }
        }

        public bool AnimationTrackEnabled
        {
            set
            {
                animationGroup.Visible = value;
                if (!value)
                {
                    Frame = 0;
                    buttonPlay.Text = "Play";
                }
            }
        }

        /// <summary>
        /// If set to try camera cannot be rotated in z direction
        /// </summary>
        public bool Lock2D { get; set; } = false;

        private List<IDrawable> Drawables { get; set; } = new List<IDrawable>();

        private EventHandler RenderLoop;

        private bool Selecting = false;
        private Vector2 mouseStart;
        private Vector2 mouseEnd;

        private Vector2 prevPos;
        private Vector2 deltaPos;

        public bool EnableCrossHair = false;
        private Vector3 CrossHair = new Vector3();

        public bool EnableFloor { get; set; } = false;

        public ViewportControl()
        {
            InitializeComponent();

            RenderLoop = (sender, args) =>
            {
                var pos = new Vector2(Cursor.Position.X, Cursor.Position.Y);

                if (prevPos == null)
                    prevPos = pos;

                deltaPos = prevPos - pos;

                prevPos = pos;

                if (MainForm.Instance == null || MainForm.Instance.WindowState == FormWindowState.Minimized)
                    return;

                if (_camera == null || !Visible)
                    return;
                
                if (buttonPlay.Text == "Pause")
                    Frame++;
                
                panel1.Invalidate();
            };

            Application.Idle += RenderLoop;
            //System.Timers.Timer timer = new System.Timers.Timer(30 / 1000d);
            /*timer.Elapsed += RenderLoop;
            timer.Start();*/

            panel1.MouseClick += (sender, args) =>
            {
                var point = new Vector2(panel1.PointToClient(Cursor.Position).X, panel1.PointToClient(Cursor.Position).Y);

                foreach (var v in Drawables)
                    if(v is IDrawableInterface inter)
                        inter.ScreenClick(args.Button, GetScreenPosition(point));
            };

            panel1.DoubleClick += (sender, args) =>
            {
                var point = new Vector2(panel1.PointToClient(Cursor.Position).X, panel1.PointToClient(Cursor.Position).Y);

                foreach (var v in Drawables)
                    if (v is IDrawableInterface inter)
                        inter.ScreenDoubleClick(GetScreenPosition(point));
            };

            panel1.MouseDown += (sender, args) =>
            {
                var keyState = Keyboard.GetState();

                if (keyState.IsKeyDown(Key.ControlLeft) || keyState.IsKeyDown(Key.ControlRight))
                {
                    Selecting = true;
                }
                mouseStart = new Vector2(args.X, args.Y);
            };

            panel1.MouseMove += (sender, args) =>
            {
                var p = panel1.PointToClient(Cursor.Position);
                var point = new Vector2(p.X, p.Y);
                
                foreach (var v in Drawables)
                    if (v is IDrawableInterface inter)
                         inter.ScreenDrag(GetScreenPosition(point), deltaPos.X * 40, deltaPos.Y * 40);
            };

            panel1.MouseUp += (sender, args) =>
            {
                if (Selecting)
                {
                    foreach (var v in Drawables)
                        if (v is IDrawableInterface inter)
                            inter.ScreenSelectArea(GetScreenPosition(mouseStart), GetScreenPosition(mouseEnd));
                }

                Selecting = false;
            };

            panel1.MouseWheel += (sender, args) =>
            {
                var zoomMultiplier = 1;
                try
                {
                    var ks = Keyboard.GetState();

                    if (ks.IsKeyDown(Key.ShiftLeft) || ks.IsKeyDown(Key.ShiftRight))
                        zoomMultiplier = 4;
                }
                catch (Exception)
                {

                }
                _camera.Zoom(args.Delta / 1000f * zoomMultiplier, true);
            };

            panel1.VSync = false;

            Disposed += (sender, args) =>
            {
                //timer.Stop();
                Application.Idle -= RenderLoop;
                //timer.Elapsed -= RenderLoop;
                //timer.Dispose();
            };
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        private delegate void SafeUpdateFrame(decimal frame);
        private void UpdateFrame(decimal frame)
        {
            if (nudFrame.InvokeRequired)
            {
                var d = new SafeUpdateFrame(UpdateFrame);
                nudFrame.Invoke(d, new object[] { frame });
            }
            else
            {
                nudFrame.Value = frame;
                animationTrack.Value = (int)frame;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private PickInformation GetScreenPosition(Vector2 point)
        {
            float x = (2.0f * point.X) / panel1.Width - 1.0f;
            float y = 1.0f - (2.0f * point.Y) / panel1.Height;

            var inv = _camera.MvpMatrix.Inverted();

            Vector4 va = Vector4.Transform(new Vector4(x, y, -1.0f, 1.0f), inv);
            Vector4 vb = Vector4.Transform(new Vector4(x, y, 1.0f, 1.0f), inv);

            va.Xyz /= va.W;
            vb.Xyz /= vb.W;

            Vector3 p1 = va.Xyz;
            Vector3 p2 = p1 - ((va - (va + vb)).Xyz) * 100;

            CrossHair = p1;

            PickInformation info = new PickInformation(new Vector2(point.X, point.Y), p1, p2);

            return info;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearDrawables()
        {
            Drawables.Clear();
        }

        public void AddRenderer(IDrawable drawable)
        {
            Drawables.Add(drawable);

            Drawables = Drawables.OrderBy(x => (int)(x.DrawOrder)).ToList();
        }

        public void RemoveRenderer(IDrawable drawable)
        {
            Drawables.Remove(drawable);
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
            if (buttonPlay.Text == "Play")
                buttonPlay.Text = "Pause";
            else
                buttonPlay.Text = "Play";
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

            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.MatrixMode(MatrixMode.Modelview);
            var v = _camera.MvpMatrix;
            GL.LoadMatrix(ref v);

            if (EnableCrossHair)
            {
                GL.PointSize(5f);
                GL.Color3(Color.Yellow);
                GL.Begin(PrimitiveType.Points);
                GL.Vertex3(CrossHair);
                GL.End();
            }

            if (EnableFloor)
            {
                DrawShape.Floor();
            }

            foreach (var r in Drawables)
            {
                r.Draw(_camera, panel1.Width, panel1.Height);
            }
            
            GL.PopAttrib();
            
            if(Selecting)
            {
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                var x1 = (mouseStart.X / panel1.Width) * 2 - 1f;
                var y1 = 1f - (mouseStart.Y / panel1.Height) * 2;
                var x2 = (mouseEnd.X / panel1.Width) * 2 - 1f;
                var y2 = 1f - (mouseEnd.Y / panel1.Height) * 2;

                GL.LineWidth(1f);
                GL.Color3(1f, 1f, 1f);
                GL.Begin(PrimitiveType.LineLoop);
                GL.Vertex2(x1, y1);
                GL.Vertex2(x2, y1);
                GL.Vertex2(x2, y2);
                GL.Vertex2(x1, y2);
                GL.End();
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

            if(_camera != null)
            {
                //_camera.FrameBoundingSphere(new Vector3((max.X + min.X) / 2, (max.Y + min.Y) / 2, 0), Math.Max(max.X - min.X, max.Y - min.Y), 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.FromArgb((0xFF << 24) | 0x333333));
            _camera = new Camera();
            _camera.RenderWidth = panel1.Width;
            _camera.RenderHeight = panel1.Height;
            _camera.Translation = new Vector3(0, 10, -80);
            ReadyToRender = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Resize(object sender, EventArgs e)
        {
            RefreshSize();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RefreshSize()
        {
            if (_camera != null)
            {
                _camera.RenderWidth = panel1.Width;
                _camera.RenderHeight = panel1.Height;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_KeyDown(object sender, KeyEventArgs e)
        {
            var speed = 0.1f;
            if (e.Shift)
                speed *= 4;
            if (e.KeyCode == Keys.W)
                _camera.Zoom(speed, true);
            if (e.KeyCode == Keys.S)
                _camera.Zoom(-speed, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var keyState = Keyboard.GetState();

            mouseEnd = new Vector2(e.X, e.Y);

            if (!keyState.IsKeyDown(Key.ControlLeft) && !keyState.IsKeyDown(Key.ControlRight)
              &&!keyState.IsKeyDown(Key.AltLeft) && !keyState.IsKeyDown(Key.AltRight))
            {
                var speed = 0.10f;
                if (e.Button == MouseButtons.Right)
                {
                    _camera.Pan(-deltaPos.X * speed, -deltaPos.Y * speed);
                }
                if (e.Button == MouseButtons.Left && !Lock2D)
                {
                    _camera.RotationXDegrees -= deltaPos.Y * speed;
                    _camera.RotationYDegrees -= deltaPos.X * speed;
                }
            }
        }
    }
}
