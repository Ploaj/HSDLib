using OpenTK;

namespace HSDRawViewer.Rendering
{
    public class Camera
    {
        public Matrix4 Transform { get; internal set; }

        public float X
        {
            get
            {
                return _translation.X;
            }
            set
            {
                _translation.X = value;
                Update();
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
                Update();
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
                Update();
            }
        }
        private Vector3 _translation = Vector3.Zero;
        public Vector3 Translation
        {
            get
            {
                return _translation;
            }
            set
            {
                _translation = value;
                Update();
            }
        }
        
        private Vector3 Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
                Update();
            }
        }
        public float XRotation
        {
            get
            {
                return _rotation.X;
            }
            set
            {
                _rotation.X = value;
                Update();
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
                Update();
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
                Update();
            }
        }
        private Vector3 _rotation = Vector3.Zero;

        private Matrix4 Perspective
        {
            get
            {
                var aspect = 1f;
                if(Height != 0)
                    aspect = Width / Height;
                if (aspect == 0)
                    aspect = 1f;
                return Matrix4.CreatePerspectiveFieldOfView(1f, aspect, 0.1f, 10000);
            }
        }

        private Vector3 _defaultTranslation = new Vector3(0, -25, -80);
        private Vector3 _defaultRotation = new Vector3(0, 0, 0);

        private float Width, Height;
        
        public Camera(float width, float height)
        {
            SetDefault();
        }

        public void SetDefault()
        {
            Translation = _defaultTranslation;
            Rotation = _defaultRotation;
        }

        public void SetViewSize(float width, float height)
        {
            Width = width;
            Height = height;
            Update();
        }

        private void Update()
        {
            Transform = Matrix4.CreateRotationZ(_rotation.Z) 
                * Matrix4.CreateRotationY(_rotation.Y) 
                * Matrix4.CreateRotationX(_rotation.X)
                * Matrix4.CreateTranslation(Translation)
                * Perspective;
        }
    }
}
