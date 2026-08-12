using HSDRawViewer.Rendering.Renderers;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace HSDRawViewer.Rendering.Widgets
{
    /// <summary>
    /// Arcball direction widget.
    /// Lets the user define a normalized direction vector
    /// by dragging on a sphere surface.
    /// </summary>
    public class ArcballWidget : IWidget
    {
        /// <summary>
        /// Widget transform.
        /// Translation = widget position.
        /// Rotation is optional visual orientation.
        /// </summary>
        public Matrix4 Transform = Matrix4.Identity;

        /// <summary>
        /// Current direction.
        /// </summary>
        public Vector3 Direction = Vector3.UnitZ;

        /// <summary>
        /// Sphere size.
        /// </summary>
        public float Radius = 4f;

        /// <summary>
        /// Screen scale behavior.
        /// </summary>
        public bool ConstantScreenSize = true;

        public float LineThickness = 2f;

        public Vector3 SphereColor = new(0.7f, 0.7f, 0.7f);
        public Vector3 ArrowColor = new(1f, 1f, 0f);
        public Vector3 AxisXColor = new(1f, 0f, 0f);
        public Vector3 AxisYColor = new(0f, 1f, 0f);
        public Vector3 AxisZColor = new(0f, 0f, 1f);

        public bool Interacting { get; set; }

        public delegate void UpdateDirection(Vector3 direction);
        public UpdateDirection DirectionUpdated;

        private float scale = 1f;
        private Matrix4 ScaleMatrix = Matrix4.Identity;

        private Vector3 Center;

        private Vector3 DragStartVector;
        private Vector3 DirectionBeforeDrag;

        /// <summary>
        /// Updates scaling.
        /// </summary>
        private void Update(Camera camera)
        {
            Center = Transform.ExtractTranslation();

            if (ConstantScreenSize)
            {
                scale = -Vector3.TransformPosition(
                    Center,
                    camera.ModelViewMatrix).Z / 40f;

                scale *= (2 * (float)Math.Tan(camera.FovRadians / 2.0));

                if (camera.RenderWidth > camera.RenderHeight)
                    scale *= 640f / camera.RenderWidth;
                else
                    scale *= 640f / camera.RenderHeight;

                ScaleMatrix = Matrix4.CreateScale(scale);
            }
            else
            {
                scale = 1f;
                ScaleMatrix = Matrix4.Identity;
            }
        }

        /// <summary>
        /// Mouse move / hover.
        /// </summary>
        public void Drag(PickInformation info)
        {
            if (info == null)
                return;

            if (!Interacting)
                return;

            if (!info.CheckSphereHitIntersection(
                Center,
                Radius * scale,
                out Vector3 hit))
                return;

            Vector3 currentVector =
                (hit - Center).Normalized();

            Quaternion delta =
                RotationBetweenVectors(
                    DragStartVector,
                    currentVector);

            Direction =
                Vector3.Transform(
                    DirectionBeforeDrag,
                    delta).Normalized();

            DirectionUpdated?.Invoke(Direction);
        }

        /// <summary>
        /// Begin interaction.
        /// </summary>
        public bool MouseDown(PickInformation info)
        {
            if (info == null || Interacting)
                return false;

            if (!info.CheckSphereHitIntersection(
                Center,
                Radius * scale,
                out Vector3 hit))
                return false;

            var prev = Interacting;
            Interacting = true;

            DragStartVector =
                (hit - Center).Normalized();

            DirectionBeforeDrag = Direction;

            return !prev && Interacting;
        }

        /// <summary>
        /// End interaction.
        /// </summary>
        public void MouseUp()
        {
            Interacting = false;
        }

        /// <summary>
        /// Render widget.
        /// </summary>
        public void Render(Camera camera, GLTextRenderer text = null)
        {
            Update(camera);

            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Disable(EnableCap.DepthTest);

            GL.MatrixMode(MatrixMode.Modelview);

            GL.PushMatrix();

            Matrix4 trans =
                ScaleMatrix * Transform;

            GL.MultMatrix(ref trans);

            DrawSphere(Radius, 8, 8);

            DrawAxes();

            DrawDirectionArrow();

            GL.PopMatrix();

            GL.PopAttrib();
        }

        /// <summary>
        /// Draw XYZ guides.
        /// </summary>
        private void DrawAxes()
        {
            GL.LineWidth(LineThickness);

            GL.Begin(PrimitiveType.Lines);

            GL.Color3(AxisXColor);
            GL.Vertex3(Vector3.Zero);
            GL.Vertex3(Vector3.UnitX * Radius);

            GL.Color3(AxisYColor);
            GL.Vertex3(Vector3.Zero);
            GL.Vertex3(Vector3.UnitY * Radius);

            GL.Color3(AxisZColor);
            GL.Vertex3(Vector3.Zero);
            GL.Vertex3(Vector3.UnitZ * Radius);

            GL.End();
        }

        /// <summary>
        /// Draw current direction arrow.
        /// </summary>
        private void DrawDirectionArrow()
        {
            Vector3 end =
                Direction * Radius;

            GL.LineWidth(LineThickness * 2);

            GL.Color3(ArrowColor);

            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(Vector3.Zero);
            GL.Vertex3(end);
            GL.End();

            GL.PushMatrix();

            Matrix4 rot =
                CreateRotationFromDirection(Direction);

            Matrix4 trans =
                rot *
                Matrix4.CreateTranslation(end);

            GL.MultMatrix(ref trans);

            DrawShape.DrawCone(
                Radius * 0.25f,
                Radius * 0.08f,
                16,
                ArrowColor);

            GL.PopMatrix();
        }

        /// <summary>
        /// Rotation between two unit vectors.
        /// </summary>
        private Quaternion RotationBetweenVectors(
            Vector3 start,
            Vector3 end)
        {
            float cosTheta =
                Vector3.Dot(start, end);

            Vector3 axis;

            if (cosTheta < -0.9999f)
            {
                axis =
                    Vector3.Cross(
                        Vector3.UnitX,
                        start);

                if (axis.LengthSquared < 0.0001f)
                {
                    axis =
                        Vector3.Cross(
                            Vector3.UnitY,
                            start);
                }

                axis.Normalize();

                return Quaternion.FromAxisAngle(
                    axis,
                    MathF.PI);
            }

            axis =
                Vector3.Cross(start, end);

            float s =
                MathF.Sqrt(
                    (1 + cosTheta) * 2);

            float invs = 1 / s;

            return new Quaternion(
                axis.X * invs,
                axis.Y * invs,
                axis.Z * invs,
                s * 0.5f);
        }

        /// <summary>
        /// Creates orientation matrix from direction.
        /// </summary>
        private Matrix4 CreateRotationFromDirection(
            Vector3 dir)
        {
            Vector3 forward =
                dir.Normalized();

            Vector3 up =
                Math.Abs(Vector3.Dot(
                    forward,
                    Vector3.UnitY)) > 0.99f
                ? Vector3.UnitX
                : Vector3.UnitY;

            Vector3 right =
                Vector3.Cross(up, forward).Normalized();

            up =
                Vector3.Cross(forward, right);

            Matrix4 m = Matrix4.Identity;

            m.Row0 = new Vector4(right, 0);
            m.Row1 = new Vector4(forward, 0);
            m.Row2 = new Vector4(up, 0);

            return m;
        }

        private void DrawSphere(
            float radius,
            int slices,
            int stacks)
        {
            GL.Color3(SphereColor);

            for (int i = 0; i <= stacks; i++)
            {
                float lat0 =
                    MathF.PI * (-0.5f + (float)(i - 1) / stacks);

                float z0 = MathF.Sin(lat0);
                float zr0 = MathF.Cos(lat0);

                float lat1 =
                    MathF.PI * (-0.5f + (float)i / stacks);

                float z1 = MathF.Sin(lat1);
                float zr1 = MathF.Cos(lat1);

                GL.Begin(PrimitiveType.LineStrip);

                for (int j = 0; j <= slices; j++)
                {
                    float lng =
                        2 * MathF.PI * (float)(j - 1) / slices;

                    float x = MathF.Cos(lng);
                    float y = MathF.Sin(lng);

                    GL.Vertex3(
                        x * zr0 * radius,
                        y * zr0 * radius,
                        z0 * radius);

                    GL.Vertex3(
                        x * zr1 * radius,
                        y * zr1 * radius,
                        z1 * radius);
                }

                GL.End();
            }
        }
    }
}