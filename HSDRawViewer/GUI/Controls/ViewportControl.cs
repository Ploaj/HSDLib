using System;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using HSDRawViewer.Rendering;
using System.Collections.Generic;
using OpenTK;
using System.Linq;
using OpenTK.Input;
using System.Timers;
using HSDRawViewer.Rendering.Renderers;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace HSDRawViewer.GUI
{
    ///new GLControl(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 8, 16));
    /// <summary>
    /// 
    /// </summary>
    public partial class ViewportControl : UserControl
    {
        public Camera Camera { get => _camera; set { _camera = value; RefreshSize(); } }
        private Camera _camera;

        public Color ViewportBackColor { get; set; } = Color.FromArgb(30, 30, 40);
        public Color GridColor { get; set; } = Color.White;

        [Browsable(false)]
        public bool ReadyToRender { get; internal set; } = false;

        [Browsable(false)]
        public bool EnableHelpDisplay { get; set; } = true;

        [Browsable(false)]
        public bool LoopPlayback { get => cbLoop.Checked; set => cbLoop.Checked = value; }

        public float Frame
        {
            get
            {
                return _frame;
            }
            set
            {
                _frame = value;
                UpdateFrame((decimal)_frame);
            }
        }
        private float _frame;

        [Browsable(false)]
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

        [Browsable(false)]
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

        [Browsable(false)]
        public bool IsAltAction
        {
            get
            {
                var keyState = Keyboard.GetState();
                return keyState.IsKeyDown(Key.AltLeft) || keyState.IsKeyDown(Key.AltRight);
            }
        }

        [Browsable(false)]
        public bool Frozen
        {
            get
            {
                var keyState = Keyboard.GetState();
                return keyState.IsKeyDown(Key.ControlLeft) || keyState.IsKeyDown(Key.ControlRight) ||
                       IsAltAction;
            }
        }

        public static int CSPWidth { get; internal set; } = 136 * 2;
        public static int CSPHeight { get; internal set; } = 188 * 2;

        private List<IDrawable> Drawables { get; set; } = new List<IDrawable>();

        private EventHandler RenderLoop;
        private ElapsedEventHandler PlayerTimer;
        private System.Timers.Timer pbTimer;

        private bool Selecting = false;
        private Vector2 mouseStart;
        private Vector2 mouseEnd;

        private Vector2 PrevCursorPos;
        private Vector2 DeltaCursorPos;

        public bool EnableCrossHair = false;
        private Vector3 CrossHair = new Vector3();

        [Browsable(false)]
        public bool IsPlaying { get => (buttonPlay.Text == "Pause"); }

        public bool EnableFloor { get; set; } = false;

        private bool _enableBack = true;
        public bool EnableBack
        {
            get => _enableBack;
            set
            {
                _enableBack = value;

                if (EnableBack)
                    GL.ClearColor(ViewportBackColor);
                else
                    GL.ClearColor(0, 0, 0, 0);
            }
        }

        public bool TakeScreenShot = false;

        public bool EnableCSPMode { get; set; } = false;
        private bool _cspMode = false;
        public bool CSPMode
        {
            get => _cspMode;
            set
            {
                _cspMode = value;
                if (_cspMode && EnableCSPMode)
                {
                    panel1.Dock = DockStyle.Top;
                    panel1.Height = CSPHeight * 2;
                }
                else
                {
                    panel1.Dock = DockStyle.Fill;
                }
            }
        }

        public ViewportControl()
        {
            InitializeComponent();

            DateTime meansure = DateTime.Now;

            RenderLoop = (sender, args) =>
            {
                //while (ReadyToRender && panel1 != null && panel1.IsIdle)
                {
                    if (MainForm.Instance == null || 
                    MainForm.Instance.WindowState == FormWindowState.Minimized || 
                    !Visible ||
                    _camera == null)
                        return;

                    var el = DateTime.Now;
                    var elapsed = el - meansure;
                    if (ApplicationSettings.UnlockedViewport || elapsed.Milliseconds >= 16)
                    {
                        panel1_Paint(null, null);

                        meansure = el;
                    }

                }
            };
            
            PlayerTimer = (sender, args) =>
            {
                
                if (buttonPlay.Text == "Pause")
                {
                    if(!(!LoopPlayback && Frame == MaxFrame))
                    {
                        Frame++;
                    }
                }
            };

            Application.Idle += RenderLoop;

            pbTimer = new System.Timers.Timer(60 / 1000d);
            pbTimer.Elapsed += PlayerTimer;
            pbTimer.Start();
            nudPlaybackSpeed.Value = 60;

            panel1.KeyDown += (sender, args) =>
            {
                /*if (args.Alt)
                {
                    if (args.KeyCode == Keys.B)
                        EnableBack = !EnableBack;

                    if (args.KeyCode == Keys.G)
                        EnableFloor = !EnableFloor;

                    if (args.KeyCode == Keys.P)
                        TakeScreenShot = true;

                    if (args.KeyCode == Keys.O)
                        CSPMode = !CSPMode;

                    if (args.KeyCode == Keys.R)
                        _camera.RestoreDefault();

                    if (args.KeyCode == Keys.C)
                        using (PropertyDialog d = new PropertyDialog("Camera Settings", _camera))
                            d.ShowDialog();
                }*/

                var keyState = OpenTK.Input.Keyboard.GetState();
                foreach (var v in Drawables)
                    if (v is IDrawableInterface inter)
                        inter.ViewportKeyPress(keyState);
            };

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
                    Selecting = true;

                mouseStart = new Vector2(args.X, args.Y);
            };

            panel1.MouseMove += (sender, args) =>
            {
                mouseEnd = new Vector2(args.X, args.Y);

                var p = panel1.PointToClient(Cursor.Position);
                var point = new Vector2(p.X, p.Y);
                
                foreach (var v in Drawables)
                    if (v is IDrawableInterface inter)
                         inter.ScreenDrag(GetScreenPosition(point), DeltaCursorPos.X * 40, DeltaCursorPos.Y * 40);
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
                Application.Idle -= RenderLoop;
                pbTimer.Stop();
                pbTimer.Elapsed -= PlayerTimer;
                pbTimer.Dispose();
            };
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        private delegate void SafeUpdateFrame(decimal frame);
        private void UpdateFrame(decimal frame)
        {
            if (nudFrame.InvokeRequired && !nudFrame.IsDisposed)
            {
                var d = new SafeUpdateFrame(UpdateFrame);
                try
                {
                    nudFrame.Invoke(d, new object[] { frame });
                }
                catch (ObjectDisposedException)
                {

                }
            }
            else
            {
                if (frame < 0)
                    frame = 0;
                if (frame > nudFrame.Maximum)
                {
                    if (!LoopPlayback)
                    {
                        Stop();
                    }
                    frame = 0;
                    _frame = 0;
                }
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
            Play();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Play()
        {
            if (buttonPlay.Text == "Play")
                buttonPlay.Text = "Pause";
            else
                buttonPlay.Text = "Play";
        }

        /// <summary>
        /// Stops animation and resets to frame 0
        /// </summary>
        public void Stop()
        {
            buttonPlay.Text = "Pause";
            Frame = 0;
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
            ForceDraw();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ForceDraw()
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
                DrawShape.Floor(GridColor, 50, 5);

            foreach (var r in Drawables)
                r.Draw(_camera, panel1.Width, panel1.Height);

            GL.PopAttrib();

            if (Selecting)
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

            if (CSPMode && !TakeScreenShot)
            {
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                GL.Disable(EnableCap.DepthTest);

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

                // 136 x 188

                float width = CSPWidth / (float)panel1.Width;
                float height = CSPHeight / (float)panel1.Height;

                GL.Color4(0.5f, 0.5f, 0.5f, 0.5f);

                GL.Begin(PrimitiveType.Quads);

                GL.Vertex2(-1, -height);
                GL.Vertex2(1, -height);
                GL.Vertex2(1, -1);
                GL.Vertex2(-1, -1);

                GL.Vertex2(-1, 1);
                GL.Vertex2(1, 1);
                GL.Vertex2(1, height);
                GL.Vertex2(-1, height);

                GL.Vertex2(1, -height);
                GL.Vertex2(width, -height);
                GL.Vertex2(width, height);
                GL.Vertex2(1, height);

                GL.Vertex2(-width, -height);
                GL.Vertex2(-1, -height);
                GL.Vertex2(-1, height);
                GL.Vertex2(-width, height);

                GL.End();
            }

            /*if (EnableHelpDisplay && !TakeScreenShot)
            {
                if (IsAltAction)
                {
                    GLTextRenderer.RenderText(_camera, "R - Reset Camera", 0, 0);
                    GLTextRenderer.RenderText(_camera, "C - Open Camera Settings", 0, 16);
                    GLTextRenderer.RenderText(_camera, "G - Toggle Grid", 0, 32);
                    GLTextRenderer.RenderText(_camera, "B - Toggle Backdrop", 0, 48);
                    GLTextRenderer.RenderText(_camera, "P - Save Screenshot to File", 0, 64);
                }
                else
                {
                    GLTextRenderer.RenderText(_camera, "Alt+", 0, 0);
                }
            }*/

            panel1.SwapBuffers();

            if (TakeScreenShot)
            {
                string fileName;

                if (CSPMode)
                    fileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(MainForm.Instance.FilePath), "csp_" + System.IO.Path.GetFileNameWithoutExtension(MainForm.Instance.FilePath) + ".png");
                else
                    fileName = "render_" + System.DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss") + ".png";

                using (var bitmap = ReadDefaultFramebufferImagePixels(Camera.RenderWidth, Camera.RenderHeight, true))
                {
                    if (CSPMode)
                    {
                        using (var resize = ResizeImage(bitmap, Camera.RenderWidth / 2, Camera.RenderHeight / 2))
                        {
                            if (_camera.MirrorScreenshot)
                                resize.MirrorX();

                            Converters.SBM.CSPMaker.MakeCSP(resize);

                            using (var csp = resize.Clone(new Rectangle((panel1.Width - CSPWidth) / 4, (panel1.Height - CSPHeight) / 4, CSPWidth / 2, CSPHeight / 2), bitmap.PixelFormat))
                                csp.Save(fileName);
                        }
                    }
                    else
                        bitmap.Save(fileName);

                    MessageBox.Show("Screenshot saved as " + fileName);
                }

                TakeScreenShot = false;
            }
        }


        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
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
            GL.ClearColor(ViewportBackColor);
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
        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            PrevCursorPos = new Vector2(Cursor.Position.X, Cursor.Position.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var pos = new Vector2(Cursor.Position.X, Cursor.Position.Y);

            if (PrevCursorPos == null)
                PrevCursorPos = pos;

            DeltaCursorPos = PrevCursorPos - pos;

            PrevCursorPos = pos;

            if (!Frozen)
            {
                var speed = 0.10f;
                var speedpane = 0.75f;
                if (e.Button == MouseButtons.Right)
                {
                    _camera.Pan(-DeltaCursorPos.X * speedpane, -DeltaCursorPos.Y * speedpane);
                }
                if (e.Button == MouseButtons.Left && !Lock2D)
                {
                    _camera.RotationXDegrees -= DeltaCursorPos.Y * speed;
                    _camera.RotationYDegrees -= DeltaCursorPos.X * speed;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudPlaybackSpeed_ValueChanged(object sender, EventArgs e)
        {
            pbTimer.Stop();
            pbTimer.Interval = (1000f / (float)nudPlaybackSpeed.Value);
            pbTimer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hsdCam"></param>
        public void LoadHSDCamera(HSDRaw.Common.HSD_Camera hsdCam)
        {
            if (hsdCam.ProjectionType != 1)
                return;

            _camera.RenderWidth = hsdCam.ViewportRight;
            _camera.RenderHeight = hsdCam.ViewportBottom;
            
            _camera.SetLookAt(new Vector3(hsdCam.eye.V1, hsdCam.eye.V2, hsdCam.eye.V3),
                new Vector3(hsdCam.target.V1, hsdCam.target.V2, hsdCam.target.V3));
            
            _camera.FovRadians = hsdCam.FieldOfView;

            _camera.FarClipPlane = hsdCam.FarClip;
            _camera.NearClipPlane = hsdCam.NearClip;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="saveAlpha"></param>
        /// <returns></returns>
        public static Bitmap ReadDefaultFramebufferImagePixels(int width, int height, bool saveAlpha = false)
        {
            // RGBA unsigned byte
            int pixelSizeInBytes = sizeof(byte) * 4;
            int imageSizeInBytes = width * height * pixelSizeInBytes;

            // TODO: Does the draw buffer need to be set?
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            byte[] pixels = GetBitmapPixels(width, height, pixelSizeInBytes, saveAlpha);

            var bitmap = GetBitmap(width, height, pixels);

            // Adjust for differences in the origin point.
            bitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
            return bitmap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="pixelSizeInBytes"></param>
        /// <param name="saveAlpha"></param>
        /// <returns></returns>
        private static byte[] GetBitmapPixels(int width, int height, int pixelSizeInBytes, bool saveAlpha)
        {
            int imageSizeInBytes = width * height * pixelSizeInBytes;

            // Read the pixels from whatever framebuffer is currently bound.
            byte[] pixels = ReadPixels(width, height, imageSizeInBytes);

            if (!saveAlpha)
                SetAlphaToWhite(width, height, pixelSizeInBytes, pixels);
            return pixels;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="imageSizeInBytes"></param>
        /// <returns></returns>
        private static byte[] ReadPixels(int width, int height, int imageSizeInBytes)
        {
            byte[] pixels = new byte[imageSizeInBytes];

            // Read the pixels from the framebuffer. PNG uses the BGRA format. 
            GL.ReadPixels(0, 0, width, height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
            return pixels;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="pixelSizeInBytes"></param>
        /// <param name="pixels"></param>
        private static void SetAlphaToWhite(int width, int height, int pixelSizeInBytes, byte[] pixels)
        {
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    int pixelIndex = w + (h * width);
                    pixels[pixelIndex * pixelSizeInBytes + 3] = 255;
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="imageData"></param>
        /// <returns></returns>
        public static Bitmap GetBitmap(int width, int height, byte[] imageData)
        {
            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            Marshal.Copy(imageData, 0, bmpData.Scan0, imageData.Length);

            bmp.UnlockBits(bmpData);
            return bmp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetCameraButton_Click(object sender, EventArgs e)
        {
            _camera.RestoreDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editCameraButton_Click(object sender, EventArgs e)
        {
            using (PropertyDialog d = new PropertyDialog("Camera Settings", _camera))
                d.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnableFloor = !EnableFloor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnableBack = !EnableBack;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void screenshotButton_Click(object sender, EventArgs e)
        {
            TakeScreenShot = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleCSPModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CSPMode = !CSPMode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog d = new ColorDialog())
            {
                d.Color = ViewportBackColor;

                if (d.ShowDialog() == DialogResult.OK)
                {
                    ViewportBackColor = d.Color;

                    if (EnableBack)
                        GL.ClearColor(ViewportBackColor);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog d = new ColorDialog())
            {
                d.Color = GridColor;

                if (d.ShowDialog() == DialogResult.OK)
                    GridColor = d.Color;
            }
        }
    }
}
