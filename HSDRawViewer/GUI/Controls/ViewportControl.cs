﻿using HSDRaw.Common;
using HSDRawViewer.GUI.Controls;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.Rendering;
using HSDRawViewer.Tools;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.WinForms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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

        public System.Drawing.Color ViewportBackColor { get; set; } = System.Drawing.Color.FromArgb(30, 30, 40);
        public System.Drawing.Color GridColor { get; set; } = System.Drawing.Color.White;

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
                FrameChange?.Invoke(_frame);
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
            get => animationGroup.Visible;
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
        public bool IsAltKey
        {
            get; internal set;
        }

        [Browsable(false)]
        public bool IsControlKey
        {
            get; internal set;
        }

        public static int CSPWidth { get; internal set; } = 136 * 2;
        public static int CSPHeight { get; internal set; } = 188 * 2;

        private List<IDrawable> Drawables { get; set; } = new List<IDrawable>();

        private bool Selecting = false;
        private Vector2 mouseStart;
        private Vector2 mouseEnd;

        private Vector2 PrevCursorPos;
        private Vector2 DeltaCursorPos;

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
            }
        }

        private int TakeScreenShot { get; set; } = 0;

        public delegate void ScreenshotTakenCallack(ViewportControl control);
        public ScreenshotTakenCallack ScreenshotTaken;

        private bool _cspMode = false;
        public bool CSPMode
        {
            get => _cspMode;
            set
            {
                _cspMode = value;
                if (_cspMode)
                {
                    glControl.Dock = DockStyle.None;
                    glControl.SendToBack();
                    glControl.Width = CSPWidth * 2;
                    glControl.Height = CSPHeight * 2;
                }
                else
                {
                    glControl.Dock = DockStyle.Fill;
                    glControl.BringToFront();
                }
            }
        }

        private bool IsCameraFrozen
        {
            get
            {
                if (Selecting || IsAltKey)
                    return true;

                foreach (IDrawable d in Drawables)
                    if (d is IDrawableInterface inter && inter.FreezeCamera())
                        return true;

                return false;
            }
        }

        public ViewportControl()
        {
            InitializeComponent();

            nudPlaybackSpeed.Value = 60;

            glControl.LostFocus += (s, a) =>
            {
                IsAltKey = false;
                IsControlKey = false;
            };

            glControl.KeyUp += (sender, args) =>
            {
                if (args.KeyCode == Keys.Menu)
                    IsAltKey = false;

                if (args.KeyCode == Keys.ControlKey)
                    IsControlKey = false;
            };

            glControl.KeyDown += (sender, args) =>
            {
                if (args.KeyCode == Keys.Menu)
                    IsAltKey = true;

                if (args.KeyCode == Keys.ControlKey)
                    IsControlKey = true;

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

                foreach (IDrawable v in Drawables)
                    if (v is IDrawableInterface inter)
                        inter.ViewportKeyPress(args); //keyState
            };

            glControl.MouseClick += (sender, args) =>
            {
                // var point = new Vector2(glControl.PointToClient(Cursor.Position).X, glControl.PointToClient(Cursor.Position).Y);
                Vector2 point = new(args.X, args.Y);

                foreach (IDrawable v in Drawables)
                    if (v is IDrawableInterface inter)
                        inter.ScreenClick(args.Button, GetScreenPosition(point));

                glControl.Focus();
            };

            glControl.DoubleClick += (sender, args) =>
            {
                Vector2 point = new(glControl.PointToClient(Cursor.Position).X, glControl.PointToClient(Cursor.Position).Y);

                foreach (IDrawable v in Drawables)
                    if (v is IDrawableInterface inter)
                        inter.ScreenDoubleClick(GetScreenPosition(point));
            };

            glControl.MouseDown += (sender, args) =>
            {
                if (IsControlKey && args.Button == MouseButtons.Left)
                    Selecting = true;

                mouseStart = new Vector2(args.X, args.Y);
            };

            glControl.MouseMove += (sender, args) =>
            {
                // update end position
                mouseEnd = new Vector2(args.X, args.Y);

                // interact with drawable
                System.Drawing.Point p = glControl.PointToClient(Cursor.Position);
                Vector2 point = new(p.X, p.Y);
                foreach (IDrawable v in Drawables)
                    if (v is IDrawableInterface inter)
                        inter.ScreenDrag(args, GetScreenPosition(point), DeltaCursorPos.X * 40, DeltaCursorPos.Y * 40);

                // move camera
                Vector2 pos = new(Cursor.Position.X, Cursor.Position.Y);
                DeltaCursorPos = PrevCursorPos - pos;
                PrevCursorPos = pos;
                if (!IsCameraFrozen)
                {
                    float speed = 0.10f;
                    float speedpane = 0.75f;
                    if (args.Button == MouseButtons.Right)
                    {
                        _camera.Pan(-DeltaCursorPos.X * speedpane, -DeltaCursorPos.Y * speedpane);
                    }
                    if (args.Button == MouseButtons.Left && !Lock2D)
                    {
                        _camera.RotationXDegrees -= DeltaCursorPos.Y * speed;
                        _camera.RotationYDegrees -= DeltaCursorPos.X * speed;
                    }
                }

            };

            glControl.MouseUp += (sender, args) =>
            {
                // select drawable area
                if (Selecting)
                {
                    foreach (IDrawable v in Drawables)
                        if (v is IDrawableInterface inter)
                            inter.ScreenSelectArea(GetScreenPosition(mouseStart), GetScreenPosition(mouseEnd));
                }

                Selecting = false;
            };

            glControl.MouseWheel += (sender, args) =>
            {
                // zoom camera
                int zoomMultiplier = 1;
                if (!IsCameraFrozen)
                    _camera.Zoom(args.Delta / 1000f * zoomMultiplier, true);
            };
        }

        public delegate void FrameChanged(float value);
        public FrameChanged FrameChange;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        private delegate void SafeUpdateFrame(decimal frame);
        private void UpdateFrame(decimal frame)
        {
            if (nudFrame.InvokeRequired && !nudFrame.IsDisposed)
            {
                SafeUpdateFrame d = new(UpdateFrame);
                try
                {
                    nudFrame.Invoke(d, new object[] { frame });
                }
                catch (Exception)
                {

                }
            }
            else
            {
                if (frame > (decimal)MaxFrame)
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

                    frame = (decimal)MaxFrame;
                    _frame = MaxFrame;
                }

                nudFrame.Value = frame;
                animationTrack.Frame = (float)frame;
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

            Matrix4 inv = _camera.MvpMatrix.Inverted();

            Vector4 va = new Vector4(x, y, -1.0f, 1.0f) * inv;
            Vector4 vb = new Vector4(x, y, 1.0f, 1.0f) * inv;

            va.Xyz /= va.W;
            vb.Xyz /= vb.W;

            Vector3 p1 = va.Xyz;
            Vector3 p2 = p1 - ((va - (va + vb)).Xyz) * 100;

            // CrossHair = p1;

            PickInformation info = new(new Vector2(point.X, point.Y), p1, p2);

            return info;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearDrawables()
        {
            Drawables.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawable"></param>
        public void AddRenderer(IDrawable drawable)
        {
            Drawables.Add(drawable);

            Drawables = Drawables.OrderBy(x => (int)(x.DrawOrder)).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawable"></param>
        public void RemoveRenderer(IDrawable drawable)
        {
            if (Drawables.Contains(drawable))
            {
                // free resources
                glControl.MakeCurrent();
                drawable.GLFree();

                // remove drawable
                Drawables.Remove(drawable);
            }
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
            Render(glControl.Width, glControl.Height);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Render(int width, int height)
        {
            glControl.MakeCurrent();

            // set clear color
            if (EnableBack)
                GL.ClearColor(ViewportBackColor);
            else
                GL.ClearColor(0, 0, 0, 0);

            // setup viewport
            GL.Viewport(0, 0, width, height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // setup immediate mode matricies
            Matrix4 v = _camera.PerspectiveMatrix;
            Matrix4 m = _camera.ModelViewMatrix;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref v);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref m);

            // draw grid if enabled
            GL.Enable(EnableCap.DepthTest);
            if (DisplayGrid)
                DrawShape.Floor(GridColor, 50, 5);

            // draw drawbles
            GL.PushAttrib(AttribMask.AllAttribBits);
            foreach (IDrawable r in Drawables)
                r.Draw(_camera, glControl.Width, glControl.Height);
            GL.PopAttrib();

            // draw selection
            if (Selecting)
                RenderSelectionOutline();

            // render screenshot outline
            RenderScreenshotSelection();

            // swap buffer to display render
            glControl.SwapBuffers();

            // check to take screenshot
            TakeGLScreenShot();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderSelectionOutline()
        {
            GL.PushMatrix();

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            float x1 = (mouseStart.X / glControl.Width) * 2 - 1f;
            float y1 = 1f - (mouseStart.Y / glControl.Height) * 2;
            float x2 = (mouseEnd.X / glControl.Width) * 2 - 1f;
            float y2 = 1f - (mouseEnd.Y / glControl.Height) * 2;

            GL.LineWidth(1f);
            GL.Color3(1f, 1f, 1f);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2(x1, y1);
            GL.Vertex2(x2, y1);
            GL.Vertex2(x2, y2);
            GL.Vertex2(x1, y2);
            GL.End();

            GL.PopMatrix();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderScreenshotSelection()
        {
            if (CSPMode && TakeScreenShot == 0)
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                GL.Disable(EnableCap.DepthTest);

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

                // 136 x 188

                float width = CSPWidth / (float)glControl.Width;
                float height = CSPHeight / (float)glControl.Height;

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
        }

        /// <summary>
        /// 
        /// </summary>
        private void TakeGLScreenShot()
        {
            if (TakeScreenShot == 1)
            {
                TakeScreenShot = 2;
            }
            else
            if (TakeScreenShot == 2)
            {
                TakeScreenShot = 0;

                string fileName;

                if (CSPMode && MainForm.Instance.FilePath != null)
                    fileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(MainForm.Instance.FilePath), "csp_" + System.IO.Path.GetFileNameWithoutExtension(MainForm.Instance.FilePath) + ".png");
                else
                    fileName = "render_" + System.DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss") + ".png";

                using Image<Rgba32> bitmap = ReadDefaultFramebufferImagePixels(Camera.RenderWidth, Camera.RenderHeight, true);
                if (CSPMode)
                {
                    using Image<Rgba32> resize = ResizeImage(bitmap, Camera.RenderWidth / 2, Camera.RenderHeight / 2);
                    // optionally mirror
                    if (_camera.MirrorScreenshot)
                        resize.Mutate(ctx => ctx.Flip(FlipMode.Horizontal));

                    // generate csp
                    Converters.SBM.CSPMaker.MakeCSP(resize);

                    // crop
                    resize.Mutate(ctx => ctx.Crop(new Rectangle((glControl.Width - CSPWidth) / 4, (glControl.Height - CSPHeight) / 4, CSPWidth / 2, CSPHeight / 2)));

                    // save to file
                    resize.Save(fileName);
                }
                else
                {
                    bitmap.Save(fileName);
                }

                MessageBox.Show("Screenshot saved as " + fileName);
                ScreenshotTaken?.Invoke(this);
            }
        }


        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Image<Rgba32> ResizeImage(Image<Rgba32> image, int width, int height)
        {
            // Clone the input image and resize it
            return image.Clone(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = ResizeMode.Max, // Adjust as needed (Max, Crop, Stretch, Pad, etc.)
                Sampler = KnownResamplers.Bicubic // High-quality resampling
            }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void FrameView(IList<Vector2> points)
        {
            Vector2 min = new(float.MaxValue, float.MaxValue);
            Vector2 max = new(float.MinValue, float.MinValue);

            foreach (Vector2 v in points)
            {
                min.X = Math.Min(min.X, v.X);
                min.Y = Math.Min(min.Y, v.Y);
                max.X = Math.Max(max.X, v.X);
                max.Y = Math.Max(max.Y, v.Y);
            }

            if (_camera != null)
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
            Timer _timer = new();
            _timer.Tick += (sender, e) =>
            {
                Render();
            };
            _timer.Interval = 12;   // 1000 ms per sec / 50 ms per frame = 20 FPS
            _timer.Start();

            // advance frame value
            System.Timers.Timer _timer2 = new();
            _timer2.Elapsed += (sender, e) =>
            {
                if (IsPlaying)
                {
                    if (_playbackMode == PlaybackMode.Forward && !(!LoopPlayback && Frame == MaxFrame))
                    {
                        Frame += _fps / 60f;
                    }
                    if (_playbackMode == PlaybackMode.Reverse && !(!LoopPlayback && Frame == 0))
                    {
                        Frame -= _fps / 60f;
                    }
                }
            };
            _timer2.Interval = 16; // 1000 ms per sec / 50 ms per frame = 20 FPS
            _timer2.Start();

            // stop timer on dispose
            Disposed += (sender, args) =>
            {
                _timer.Stop();
                _timer2.Stop();

                _timer.Dispose();
                _timer2.Dispose();
            };

            // init gl resources
            foreach (IDrawable r in Drawables)
                r.GLInit();
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
            float speed = 0.1f;
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
        private void nudPlaybackSpeed_ValueChanged(object sender, EventArgs e)
        {
            _fps = (int)nudPlaybackSpeed.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hsdCam"></param>
        public void LoadHSDCamera(HSD_Camera hsdCam)
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
        public static Image<Rgba32> ReadDefaultFramebufferImagePixels(int width, int height, bool saveAlpha = false)
        {
            // RGBA unsigned byte
            int pixelSizeInBytes = sizeof(byte) * 4;
            int imageSizeInBytes = width * height * pixelSizeInBytes;

            // TODO: Does the draw buffer need to be set?
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            byte[] pixels = GetBitmapPixels(width, height, pixelSizeInBytes, saveAlpha);

            Image<Rgba32> bitmap = GetImage(width, height, pixels);

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
        public static Image<Rgba32> GetImage(int width, int height, byte[] imageData)
        {
            // Create a new Image<Rgba32> with the specified dimensions
            Image<Rgba32> image = new(width, height);

            // Ensure the input imageData length matches the expected size
            if (imageData.Length != width * height * 4)
                throw new ArgumentException("The imageData size does not match the specified dimensions.");

            // Copy the byte array into the image
            int pixelIndex = 0;
            image.ProcessPixelRows(accessor =>
            {
                for (int y = height - 1; y >= 0; y--)
                {
                    Span<Rgba32> row = accessor.GetRowSpan(y);
                    for (int x = 0; x < width; x++)
                    {
                        row[x] = new Rgba32(
                            imageData[pixelIndex + 2], // R
                            imageData[pixelIndex + 1], // G
                            imageData[pixelIndex],     // B
                            imageData[pixelIndex + 3]  // A
                        );
                        pixelIndex += 4;
                    }
                }
            });

            return image;
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
            using PropertyDialog d = new("Camera Settings", _camera);
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
            Screenshot();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Screenshot()
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
            using ColorDialog d = new();
            d.Color = ViewportBackColor;

            if (d.ShowDialog() == DialogResult.OK)
                ViewportBackColor = d.Color;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using ColorDialog d = new();
            d.Color = GridColor;

            if (d.ShowDialog() == DialogResult.OK)
                GridColor = d.Color;
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

        public GLControl GetControl()
        {
            return glControl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudMaxFrame_ValueChanged(object sender, EventArgs e)
        {
            animationTrack.EndFrame = (float)nudMaxFrame.Value;
            animationTrack.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Image<Rgba32> GenerateBitmap(int width, int height)
        {
            Render(width, height);
            return ReadDefaultFramebufferImagePixels(width, height, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportFrameAsPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = FileIO.SaveFile("PNG (.png)|*.png");

            if (string.IsNullOrEmpty(file))
                return;

            Frame = 0;
            using Image<Rgba32> bmp = GenerateBitmap(glControl.Width, glControl.Height);
            bmp.Save(file);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportFrameToFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MaxFrame == 0)
                return;

            if (!AnimationTrackEnabled)
                return;

            string file = FileIO.SaveFile("PNG (*.png)|*.png");

            if (string.IsNullOrEmpty(file))
                return;

            string path = Path.GetDirectoryName(file);
            string filename = Path.GetFileNameWithoutExtension(file);
            string ext = Path.GetExtension(file);

            for (int i = 0; i <= MaxFrame; i++)
            {
                Frame = i;
                using Image<Rgba32> bmp = GenerateBitmap(glControl.Width, glControl.Height);
                bmp.Save(Path.Combine(path, $"{filename}_{i:D3}{ext}"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void asGIFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MaxFrame == 0)
                return;

            if (!AnimationTrackEnabled)
                return;

            string file = FileIO.SaveFile("GIF (*.gif)|*.gif");

            if (string.IsNullOrEmpty(file))
                return;

            // Define the width and height of the GIF frames
            int width = glControl.Width;
            int height = glControl.Height;

            // Delay between frames in (1/60) of a second.
            const int frameDelay = 1;

            // Create empty image.
            Image<Rgba32> gif = null;

            // bake animation
            for (int i = 0; i <= MaxFrame; i++)
            {
                Frame = i;
                Image<Rgba32> bmp = GenerateBitmap(glControl.Width, glControl.Height);

                // Set the delay until the next image is displayed.
                SixLabors.ImageSharp.Formats.Gif.GifFrameMetadata metadata = bmp.Frames.RootFrame.Metadata.GetGifMetadata();
                metadata.FrameDelay = frameDelay;

                // add frame
                if (gif == null)
                {
                    gif = bmp;
                }
                else
                {
                    gif.Frames.AddFrame(bmp.Frames.RootFrame);
                    bmp.Dispose();
                }
            }

            if (gif != null)
            {
                // Set animation loop 
                SixLabors.ImageSharp.Formats.Gif.GifMetadata gifMetaData = gif.Metadata.GetGifMetadata();
                gifMetaData.RepeatCount = 0;

                // Save the final result.
                gif.SaveAsGif(file);
            }
        }
    }
}
