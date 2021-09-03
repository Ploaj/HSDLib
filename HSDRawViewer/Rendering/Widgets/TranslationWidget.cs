using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;

namespace HSDRawViewer.Rendering.Widgets
{
    public enum TranslationComponent
    {
        None,
        X,
        Y,
        Z
    }

    public class TranslationWidget
    {
        public Matrix4 Transform
        {
            get; set;
        } = Matrix4.Identity;

        public float Size { get; set; } = 4;

        private float ArrowSize { get => Size * 0.25f; }

        public float LineThickness { get; set; } = 2f;

        public TranslationComponent SelectedComponent = TranslationComponent.None;

        public Color ColorX = Color.Red;
        public Color ColorY = Color.Green;
        public Color ColorZ = Color.Blue;
        public Color ColorSelected = Color.Yellow;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public void Drag(PickInformation info)
        {
            SelectedComponent = TranslationComponent.None;

            if(info.CheckSphereHit(Vector3.TransformPosition(Vector3.UnitX * Size, Transform), ArrowSize * 2, out float disx))
                SelectedComponent = TranslationComponent.X;

            if (info.CheckSphereHit(Vector3.TransformPosition(Vector3.UnitY * Size, Transform), ArrowSize * 2, out float disy))
                SelectedComponent = TranslationComponent.Y;

            if (info.CheckSphereHit(Vector3.TransformPosition(Vector3.UnitZ * Size, Transform), ArrowSize * 2, out float disz))
                SelectedComponent = TranslationComponent.Z;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public void Click(PickInformation info)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        public void Render(Camera camera)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Disable(EnableCap.DepthTest);

            GL.PushMatrix();
            Matrix4 transform = Transform;
            GL.MultMatrix(ref transform);

            GL.Begin(PrimitiveType.Points);
            GL.Vertex3(0, 0, 0);
            GL.End();

            GL.LineWidth(LineThickness);

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(SelectedComponent == TranslationComponent.X ? ColorSelected : ColorX);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(Size, 0, 0);

            GL.Vertex3(Size, 0, 0);
            GL.Vertex3(Size - ArrowSize, ArrowSize * 0.5f, 0);
            GL.Vertex3(Size, 0, 0);
            GL.Vertex3(Size - ArrowSize, -ArrowSize * 0.5f, 0);

            GL.Color3(SelectedComponent == TranslationComponent.Y ? ColorSelected : ColorY);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, Size, 0);

            GL.Vertex3(0, Size, 0);
            GL.Vertex3(ArrowSize * 0.5f, Size - ArrowSize, 0);
            GL.Vertex3(0, Size, 0);
            GL.Vertex3(-ArrowSize * 0.5f, Size - ArrowSize, 0);

            GL.Color3(SelectedComponent == TranslationComponent.Z ? ColorSelected : ColorZ);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, Size);

            GL.Vertex3(0, 0, Size);
            GL.Vertex3(0, ArrowSize * 0.5f, Size - ArrowSize);
            GL.Vertex3(0, 0, Size);
            GL.Vertex3(0, -ArrowSize * 0.5f, Size - ArrowSize);

            GL.End();

            GL.PopMatrix();
            GL.PopAttrib();
        }

    }
}
