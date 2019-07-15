using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Windows.Forms;
using HSDRaw;
using HSDRaw.Common.Animation;

namespace HSDRawViewer.Rendering
{
    public class Viewport : Panel
    {
        private GLControl _glViewport;

        public Matrix4 Camera;

        public Matrix4 Translation
        {
            get
            {
                return Matrix4.CreateTranslation(_translation);
            }
            set
            {
                _translation = value.ExtractTranslation();
                UpdateCamera();
            }
        }

        private Matrix4 Transform
        {
            get
            {
                return Matrix4.CreateRotationZ(_rotation.Z) * Matrix4.CreateRotationY(_rotation.Y) * Matrix4.CreateRotationX(_rotation.X);
            }
        }

        float PrevX = 0, PrevY = 0;

        public float X
        {
            get
            {
                return _translation.X;
            }
            set
            {
                _translation.X = value;
                UpdateCamera();
            }
        }
        public float Y
        {
            get
            {
                return _translation.Y;
            }
            set
            {
                _translation.Y = value;
                UpdateCamera();
            }
        }
        public float Z
        {
            get
            {
                return _translation.Z;
            }
            set
            {
                _translation.Z = value;
                UpdateCamera();
            }
        }
        private Vector3 _translation = Vector3.Zero;

        public float XRotation
        {
            get
            {
                return _rotation.X;
            }
            set
            {
                _rotation.X = value;
                UpdateCamera();
            }
        }
        public float YRotation
        {
            get
            {
                return _rotation.Y;
            }
            set
            {
                _rotation.Y = value;
                UpdateCamera();
            }
        }
        public float ZRotation
        {
            get
            {
                return _rotation.Z;
            }
            set
            {
                _rotation.Z = value;
                UpdateCamera();
            }
        }
        private Vector3 _rotation = Vector3.Zero;

        private Matrix4 Perspective
        {
            get
            {
                return Matrix4.CreatePerspectiveFieldOfView(1f, Width / (float)Height, 0.1f, 10000);
            }
        }

        private Vector3 _defaultTranslation = new Vector3(0, -50, -100);
        private Vector3 _defaultRotation = new Vector3(0, 0, 0);

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
            }
        }

        private HSDAccessor _selectedAccessor { get; set; }

        public Viewport()
        {
            _glViewport = new GLControl(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 8, 16));
            _glViewport.Load += Viewport_Loaded;
            _glViewport.Resize += Viewport_Resize;
            _glViewport.MouseMove += Viewport_MouseMove;
            _glViewport.KeyPress += Viewport_KeyDown;
            _glViewport.Paint += Render;
            _glViewport.Dock = DockStyle.Fill;

            _renderer = new Renderer();

            Controls.Add(_glViewport);
        }

        private void UpdateCamera()
        {
            Camera = Transform * Translation * Perspective;
        }
    
        public void Render(object sender, EventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref Camera);

            RenderFloor();

            // rendering here
            if (SelectedAccessor != null)
                _renderer.Render(Camera, SelectedAccessor);
            
            _glViewport.SwapBuffers();
        }


        private void Viewport_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            UpdateCamera();
            _glViewport.Invalidate();
        }

        private void RenderFloor()
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            int size = 50;
            int space = 5;

            GL.LineWidth(1f);
            GL.Color3(Color.White);
            GL.Begin(PrimitiveType.Lines);

            for (int i = -size; i <= size; i += space)
            {
                GL.Vertex3(-size, 0, i);
                GL.Vertex3(size, 0, i);

                GL.Vertex3(i, 0, -size);
                GL.Vertex3(i, 0, size);
            }

            GL.End();
            GL.PopAttrib();
        }

        private void Viewport_Loaded(object sender, EventArgs args)
        {
            GL.ClearColor(Color.DarkSlateGray);
            Translation = Matrix4.CreateTranslation(_defaultTranslation);
        }
        
        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            float speed = 1 / Vector3.TransformPosition(Vector3.Zero, Camera).LengthFast;
            speed = (1 - speed) * 1;
            if (e.Button == MouseButtons.Left)
            {
                YRotation -= (PrevX - e.X) / 50f;
                XRotation -= (PrevY - e.Y) / 50f;
            }
            if (e.Button == MouseButtons.Right)
            {
                X -= (PrevX - e.X) * speed;
                Y += (PrevY - e.Y) * speed;
            }
            PrevX = e.X;
            PrevY = e.Y;

            _glViewport.Invalidate();
        }

        public void Viewport_KeyDown(object sender, System.Windows.Forms.KeyPressEventArgs args)
        {
            if (args.KeyChar == 'w')
                Z += 5f;

            if (args.KeyChar == 's')
                Z -= 5f;

            _glViewport.Invalidate();
        }

    }
}
