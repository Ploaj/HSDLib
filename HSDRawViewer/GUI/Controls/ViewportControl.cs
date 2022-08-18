using System;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using HSDRawViewer.Rendering;
using System.Collections.Generic;
using OpenTK.Mathematics;
using System.Linq;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using HSDRawViewer.GUI.Controls;
using HSDRawViewer.GUI.Dialog;

namespace HSDRawViewer.GUI
{
    /// <summary>
    /// 
    /// </summary>
    public enum PlaybackMode
    {
        None,
        Forward,
        Reverse
    }

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
        public bool EnableHelpDisplay { get; set; } = true;

        [Browsable(false)]
        public bool LoopPlayback { get => cbLoop.Checked; set => cbLoop.Checked = value; }

        /// <summary>
        /// Checks if track is currently playing
        /// </summary>
        public PlaybackMode PlaybackMode
        {
            get => _playbackMode; 
            internal set
            {
                _playbackMode = value;

                buttonPlayForward.Image = Properties.Resources.pb_play;
                buttonPlayReverse.Image = Properties.Resources.pb_play_reverse;

                switch (value)
                {
                    case PlaybackMode.Forward:
                        buttonPlayForward.Image = Properties.Resources.pb_pause;
                        break;
                    case PlaybackMode.Reverse:
                        buttonPlayReverse.Image = Properties.Resources.pb_pause;
                        break;
                }
            }
        }
        private PlaybackMode _playbackMode = PlaybackMode.None;

        /// <summary>
        /// 
        /// </summary>
        private int _fps = 60;

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public float MaxFrame
        {
            get
            {
                return (float)nudMaxFrame.Value;
            }
            set
            {
                animationTrack.EndFrame = (int)value;
                nudFrame.Maximum = (decimal)value;
                nudMaxFrame.Maximum = (decimal)value;
                nudMaxFrame.Value = (decimal)value;
                animationTrack.Invalidate();
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
                    PlaybackMode = PlaybackMode.None;
                }
            }
        }

        /// <summary>
        /// Access to frame tips
        /// </summary>
        public List<PlaybackBarFrameTip> FrameTips { get => animationTrack?.FrameTips; }

        /// <summary>
        /// If set to try camera cannot be rotated in z direction
        /// </summary>
        public bool Lock2D { get; set; } = false;

        [Browsable(false)]
        public bool IsAltAction
        {
            get
            {
                //var keyState = Keyboard.GetState();
                //return keyState.IsKeyDown(Key.AltLeft) || keyState.IsKeyDown(Key.AltRight);
                return false;
            }
        }

        [Browsable(false)]
        public bool Frozen
        {
            get
            {
                //var keyState = Keyboard.GetState();
                //return keyState.IsKeyDown(Key.ControlLeft) || keyState.IsKeyDown(Key.ControlRight) ||
                //       IsAltAction;
                return false;
            }
        }

        public static int CSPWidth { get; internal set; } = 136 * 2;
        public static int CSPHeight { get; internal set; } = 188 * 2;

        private List<IDrawable> Drawables { get; set; } = new List<IDrawable>();

        private bool Selecting = false;
        private Vector2 mouseStart;
        private Vector2 mouseEnd;

        private Vector2 PrevCursorPos;
        private Vector2 DeltaCursorPos;

        private Vector3 CrossHair = new Vector3();

        [Browsable(false)]
        public bool IsPlaying { get => _playbackMode != PlaybackMode.None; }

        public bool DisplayGrid { get; set; } = false;

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

        public int TakeScreenShot = 0;

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
                    glControl.Dock = DockStyle.Top;
                    glControl.Height = CSPHeight * 2;
                }
                else
                {
                    glControl.Dock = DockStyle.Fill;
                }
            }
        }
        public ViewportControl()
        {
            InitializeComponent();

            nudPlaybackSpeed.Value = 60;

            glControl.KeyDown += (sender, args) =>
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

                foreach (var v in Drawables)
                    if (v is IDrawableInterface inter)
                        inter.ViewportKeyPress(args); //keyState
            };

            glControl.MouseClick += (sender, args) =>
            {
                var point = new Vector2(glControl.PointToClient(Cursor.Position).X, glControl.PointToClient(Cursor.Position).Y);

                foreach (var v in Drawables)
                    if(v is IDrawableInterface inter)
                        inter.ScreenClick(args.Button, GetScreenPosition(point));
            };

            glControl.DoubleClick += (sender, args) =>
            {
                var point = new Vector2(glControl.PointToClient(Cursor.Position).X, glControl.PointToClient(Cursor.Position).Y);

                foreach (var v in Drawables)
                    if (v is IDrawableInterface inter)
                        inter.ScreenDoubleClick(GetScreenPosition(point));
            };

            glControl.MouseDown += (sender, args) =>
            {
                //var keyState = Keyboard.GetState();
                //if (keyState.IsKeyDown(Key.ControlLeft) || keyState.IsKeyDown(Key.ControlRight))
                //    Selecting = true;

                mouseStart = new Vector2(args.X, args.Y);
            };

            glControl.MouseMove += (sender, args) =>
            {
                mouseEnd = new Vector2(args.X, args.Y);

                var p = glControl.PointToClient(Cursor.Position);
                var point = new Vector2(p.X, p.Y);
                
                foreach (var v in Drawables)
                    if (v is IDrawableInterface inter)
                         inter.ScreenDrag(args, GetScreenPosition(point), DeltaCursorPos.X * 40, DeltaCursorPos.Y * 40);
            };

            glControl.MouseUp += (sender, args) =>
            {
                if (Selecting)
                {
                    foreach (var v in Drawables)
                        if (v is IDrawableInterface inter)
                            inter.ScreenSelectArea(GetScreenPosition(mouseStart), GetScreenPosition(mouseEnd));
                }

                Selecting = false;
            };

            glControl.MouseWheel += (sender, args) =>
            {
                var zoomMultiplier = 1;
                try
                {
                    //var ks = Keyboard.GetState();
                    //if (ks.IsKeyDown(Key.ShiftLeft) || ks.IsKeyDown(Key.ShiftRight))
                    //    zoomMultiplier = 4;
                }
                catch (Exception)
                {

                }
                _camera.Zoom(args.Delta / 1000f * zoomMultiplier, true);
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
                if (frame > nudFrame.Maximum)
                {
                    if (_playbackMode == PlaybackMode.Forward && !LoopPlayback)
                        Stop();

                    frame = 0;
                    _frame = 0;
                }

                if (frame < 0)
                {
                    if (_playbackMode == PlaybackMode.Reverse && !LoopPlayback)
                        Stop();

                    frame = nudFrame.Maximum;
                    _frame = (float)nudFrame.Maximum;
                }

                nudFrame.Value = frame;
                animationTrack.Frame = (int)frame;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private PickInformation GetScreenPosition(Vector2 point)
        {
            float x = (2.0f * point.X) / glControl.Width - 1.0f;
            float y = 1.0f - (2.0f * point.Y) / glControl.Height;

            var inv = _camera.MvpMatrix.Inverted();

            Vector4 va = new Vector4(x, y, -1.0f, 1.0f) * inv;
            Vector4 vb = new Vector4(x, y, 1.0f, 1.0f) * inv;

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
            if (Frame != animationTrack.Frame)
                Frame = animationTrack.Frame;
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
            Play(PlaybackMode.Forward);
        }

        private void buttonPlayReverse_Click(object sender, EventArgs e)
        {
            Play(PlaybackMode.Reverse);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Play(PlaybackMode mode)
        {
            if (PlaybackMode == PlaybackMode.None)
                PlaybackMode = mode;
            else
                PlaybackMode = PlaybackMode.None;
        }

        /// <summary>
        /// Stops animation and resets to frame 0
        /// </summary>
        public void Stop()
        {
            PlaybackMode = PlaybackMode.None;
            Frame = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Render()
        {
            glControl.MakeCurrent();

            GL.Viewport(0, 0, glControl.Width, glControl.Height);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.MatrixMode(MatrixMode.Projection);

            var v = _camera.MvpMatrix;
            GL.LoadMatrix(ref v);

            // if (DisplayGrid)
                DrawShape.Floor(GridColor, 50, 5);

            foreach (var r in Drawables)
                r.Draw(_camera, glControl.Width, glControl.Height);

            GL.PopAttrib();

            //if (Selecting)
            //{
            //    GL.MatrixMode(MatrixMode.Modelview);
            //    GL.LoadIdentity();

            //    var x1 = (mouseStart.X / glControl.Width) * 2 - 1f;
            //    var y1 = 1f - (mouseStart.Y / glControl.Height) * 2;
            //    var x2 = (mouseEnd.X / glControl.Width) * 2 - 1f;
            //    var y2 = 1f - (mouseEnd.Y / glControl.Height) * 2;

            //    GL.LineWidth(1f);
            //    GL.Color3(1f, 1f, 1f);
            //    GL.Begin(PrimitiveType.LineLoop);
            //    GL.Vertex2(x1, y1);
            //    GL.Vertex2(x2, y1);
            //    GL.Vertex2(x2, y2);
            //    GL.Vertex2(x1, y2);
            //    GL.End();
            //}

            //if (CSPMode && TakeScreenShot == 0)
            //{
            //    GL.MatrixMode(MatrixMode.Projection);
            //    GL.LoadIdentity();

            //    GL.MatrixMode(MatrixMode.Modelview);
            //    GL.LoadIdentity();

            //    GL.Disable(EnableCap.DepthTest);

            //    GL.Enable(EnableCap.Blend);
            //    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            //    // 136 x 188

            //    float width = CSPWidth / (float)glControl.Width;
            //    float height = CSPHeight / (float)glControl.Height;

            //    GL.Color4(0.5f, 0.5f, 0.5f, 0.5f);

            //    GL.Begin(PrimitiveType.Quads);

            //    GL.Vertex2(-1, -height);
            //    GL.Vertex2(1, -height);
            //    GL.Vertex2(1, -1);
            //    GL.Vertex2(-1, -1);

            //    GL.Vertex2(-1, 1);
            //    GL.Vertex2(1, 1);
            //    GL.Vertex2(1, height);
            //    GL.Vertex2(-1, height);

            //    GL.Vertex2(1, -height);
            //    GL.Vertex2(width, -height);
            //    GL.Vertex2(width, height);
            //    GL.Vertex2(1, height);

            //    GL.Vertex2(-width, -height);
            //    GL.Vertex2(-1, -height);
            //    GL.Vertex2(-1, height);
            //    GL.Vertex2(-width, height);

            //    GL.End();
            //}

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

            glControl.SwapBuffers();

            // TakeGLScreenShot();
        }

        private void TakeGLScreenShot()
        {
            if (TakeScreenShot == 1)
            {
                TakeScreenShot = 2;
            }
            else
            if (TakeScreenShot == 2)
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

                            using (var csp = resize.Clone(new Rectangle((glControl.Width - CSPWidth) / 4, (glControl.Height - CSPHeight) / 4, CSPWidth / 2, CSPHeight / 2), bitmap.PixelFormat))
                                csp.Save(fileName);
                        }
                    }
                    else
                        bitmap.Save(fileName);

                    MessageBox.Show("Screenshot saved as " + fileName);
                }

                TakeScreenShot = 0;
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
            // set clear color
            GL.ClearColor(ViewportBackColor);

            // setup camera
            _camera = new Camera();
            _camera.RenderWidth = glControl.Width;
            _camera.RenderHeight = glControl.Height;
            _camera.Translation = new Vector3(0, 10, -80);

            // Redraw the screen every 1/20 of a second.
            System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
            _timer.Tick += (sender, e) =>
            {
                if (IsPlaying)
                {
                    if (_playbackMode == PlaybackMode.Forward && !(!LoopPlayback && Frame == MaxFrame))
                    {
                        Frame++;
                    }
                    if (_playbackMode == PlaybackMode.Reverse && !(!LoopPlayback && Frame == 0))
                    {
                        Frame--;
                    }
                }

                Render();
            };
            _timer.Interval = 16;   // 1000 ms per sec / 50 ms per frame = 20 FPS
            _timer.Start();

            // stop timer on dispose
            Disposed += (sender, args) =>
            {
                _timer.Stop();
            };
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
                _camera.RenderWidth = glControl.Width;
                _camera.RenderHeight = glControl.Height;
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
            _fps = (int)nudPlaybackSpeed.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hsdCam"></param>
        public void LoadHSDCamera(HSDRaw.Common.HSD_Camera hsdCam)
        {
            if (hsdCam.ProjectionType != HSDRaw.Common.CameraProjection.PERSPECTIVE)
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
            DisplayGrid = !DisplayGrid;
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
            TakeScreenShot = 1;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void animationTrack_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.KeyCode);
        }
    }
}
