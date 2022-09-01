using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Drawing;
using HSDRawViewer.Rendering.Renderers;

namespace HSDRawViewer.Rendering.Widgets
{
    [Flags]
    public enum TranslationComponent
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4,
        View = X | Y | Z,
    }

    public class TranslationWidget
    {
        // TODO: option to make x move into negative axis

        public Matrix4 Transform = Matrix4.Identity;

        public float Size { get; set; } = 4;

        public float LineThickness { get; set; } = 2f;

        public TranslationComponent SelectedComponent = TranslationComponent.None;

        public Vector3 ColorX = Vector3.UnitX;
        public Vector3 ColorY = Vector3.UnitY;
        public Vector3 ColorZ = Vector3.UnitZ;
        public Vector3 ColorSelected = new Vector3(1, 1, 0);


        private Vector3 Center1;
        private Vector3 Center2;
        private Vector3 Center3;
        private Vector3 Center4;

        public Vector4 CenterSquare;
        public Vector2 PickPoint;

        private float scale;
        private Matrix4 ScaleMatrix;

        public class Plane
        {
            public Vector3 Position;
            public Vector3 Normal;
        }

        private Plane CenterPlane = new Plane();
        private Vector3 CenterPlaneHit;

        public class QuadHitTest
        {
            public Vector3 P1;
            public Vector3 P2;
            public Vector3 P3;
            public Vector3 P4;

            public Vector3 Offset;

            public Plane plane = new Plane();

            public void Init(Vector3 normal, Vector3 plane1, Vector3 plane2, ref Matrix4 trans)
            {
                P1 = Vector3.TransformPosition(Vector3.Zero, trans);
                P2 = Vector3.TransformPosition(plane1 * 2, trans);
                P3 = Vector3.TransformPosition((plane1 + plane2) * 2, trans);
                P4 = Vector3.TransformPosition(plane2 * 2, trans);

                plane.Position = P1;
                plane.Normal = Vector3.TransformNormal(normal, trans);
            }
        }

        private PickInformation ray;

        private QuadHitTest XPlane = new QuadHitTest();
        private QuadHitTest YPlane = new QuadHitTest();
        private QuadHitTest ZPlane = new QuadHitTest();

        public bool Interacting { get; internal set; }
        private bool WasInteracting = false;

        public delegate void UpdateTransform(Matrix4 newTransform);
        public UpdateTransform TransformUpdated;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        private void Update(Camera camera)
        {
            // temporary center
            var center = Transform.ExtractTranslation();

            // update scaling only if not currently interacting with widget
            if (!Interacting)
            {
                // calcuate screen scale
                scale = -Vector3.TransformPosition(center, camera.ModelViewMatrix).Z / 40f;

                // adjust scale from fov
                scale *= (2 * (float)Math.Tan(camera.FovRadians / 2.0));

                // adjust scale from aspect
                if (camera.RenderWidth > camera.RenderHeight)
                    scale *= 640f / camera.RenderWidth;
                else
                    scale *= 640f / camera.RenderHeight;

                // create scale matrix
                ScaleMatrix = Matrix4.CreateScale(scale);
            }

            // get right and up vectors
            var invMv = (Transform * camera.ModelViewMatrix).Inverted();
            var right = invMv.Row0.Xyz;
            var up = invMv.Row1.Xyz;

            // update center collision plane
            CenterPlane.Position = center;
            CenterPlane.Normal = (center - camera.TransformedPosition).Normalized();

            // update rendering points
            // billboard center picking
            Center1 = (-right / 2 - up / 2);
            Center2 = (right / 2 - up / 2);
            Center3 = (right / 2 + up / 2);
            Center4 = (-right / 2 + up / 2);

            // project center collision info
            CenterSquare.Xy = camera.Project(ScaleMatrix * Transform, Center4).Xy;
            CenterSquare.Zw = camera.Project(ScaleMatrix * Transform, Center2).Xy;


            // calculate x quad
            var trans = ScaleMatrix * Transform;
            XPlane.Init(Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ, ref trans);
            YPlane.Init(Vector3.UnitY, Vector3.UnitX, Vector3.UnitZ, ref trans);
            ZPlane.Init(Vector3.UnitZ, Vector3.UnitY, Vector3.UnitX, ref trans);

            Center = Vector3.TransformPosition(Vector3.Zero, trans);
            XEnd = Vector3.TransformPosition(Vector3.UnitX * Size, trans);
            YEnd = Vector3.TransformPosition(Vector3.UnitY * Size, trans);
            ZEnd = Vector3.TransformPosition(Vector3.UnitZ * Size, trans);
        }

        private Vector3 XOffset;
        private Vector3 XEnd;
        private Vector3 YOffset;
        private Vector3 YEnd;
        private Vector3 ZOffset;
        private Vector3 ZEnd;
        private Vector3 Center;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="plane"></param>
        /// <param name="axis"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private Vector3 GetPlaneInsectionAxis(PickInformation info, QuadHitTest plane, Vector3 axis, Vector3 offset)
        {
            var hit = info.GetPlaneIntersection(plane.plane.Normal, plane.plane.Position) + offset;
            hit = Vector3.TransformPosition(hit, Transform.Inverted());
            return Vector3.TransformPosition(hit * axis, Transform);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public void Drag(PickInformation info)
        {
            if (info == null)
                return;

            ray = info;

            if (Interacting)
            {
                switch (SelectedComponent)
                {
                    case TranslationComponent.View:
                        {
                            CenterPlaneHit = info.GetPlaneIntersection(CenterPlane.Normal, CenterPlane.Position);
                            Transform.Row3 = new Vector4(CenterPlaneHit, 1);
                        }
                        break;
                    case TranslationComponent.Y | TranslationComponent.Z:
                        {
                            CenterPlaneHit = info.GetPlaneIntersection(XPlane.plane.Normal, XPlane.plane.Position) + XPlane.Offset;
                            Transform.Row3 = new Vector4(CenterPlaneHit, 1);
                        }
                        break;
                    case TranslationComponent.X | TranslationComponent.Z:
                        {
                            CenterPlaneHit = info.GetPlaneIntersection(YPlane.plane.Normal, YPlane.plane.Position) + YPlane.Offset;
                            Transform.Row3 = new Vector4(CenterPlaneHit, 1);
                        }
                        break;
                    case TranslationComponent.Y | TranslationComponent.X:
                        {
                            CenterPlaneHit = info.GetPlaneIntersection(ZPlane.plane.Normal, ZPlane.plane.Position) + ZPlane.Offset;
                            Transform.Row3 = new Vector4(CenterPlaneHit, 1);
                        }
                        break;
                    case TranslationComponent.X:
                        {
                            CenterPlaneHit = GetPlaneInsectionAxis(info, ZPlane, Vector3.UnitX, XOffset);
                            Transform.Row3 = new Vector4(CenterPlaneHit, 1);
                        }
                        break;
                    case TranslationComponent.Y:
                        {
                            CenterPlaneHit = GetPlaneInsectionAxis(info, XPlane, Vector3.UnitY, YOffset);
                            Transform.Row3 = new Vector4(CenterPlaneHit, 1);
                        }
                        break;
                    case TranslationComponent.Z:
                        {
                            CenterPlaneHit = GetPlaneInsectionAxis(info, XPlane, Vector3.UnitZ, ZOffset);
                            Transform.Row3 = new Vector4(CenterPlaneHit, 1);
                        }
                        break;
                }

                TransformUpdated?.Invoke(Transform);
            }
            else
            {
                PickPoint = info.ScreenPoint;

                SelectedComponent = TranslationComponent.None;

                var p = info.ScreenPoint;
                Vector3 intersect;

                if (p.X > CenterSquare.X && p.X < CenterSquare.Z &&
                    p.Y > CenterSquare.Y && p.Y < CenterSquare.W)
                {
                    SelectedComponent = TranslationComponent.View;
                }
                else
                if (info.IntersectsQuad(XPlane.P1, XPlane.P2, XPlane.P3, XPlane.P4, out intersect))
                {
                    XPlane.Offset = XPlane.P1 - intersect;
                    SelectedComponent = TranslationComponent.Y | TranslationComponent.Z;
                }
                else
                if (info.IntersectsQuad(YPlane.P1, YPlane.P2, YPlane.P3, YPlane.P4, out intersect))
                {
                    YPlane.Offset = YPlane.P1 - intersect;
                    SelectedComponent = TranslationComponent.X | TranslationComponent.Z;
                }
                else
                if (info.IntersectsQuad(ZPlane.P1, ZPlane.P2, ZPlane.P3, ZPlane.P4, out intersect))
                {
                    ZPlane.Offset = ZPlane.P1 - intersect;
                    SelectedComponent = TranslationComponent.Y | TranslationComponent.X;
                }
                else
                if (ray.CheckSphereHitIntersection(ZEnd, scale * Size / 3, out intersect))
                {
                    ZOffset = Center - GetPlaneInsectionAxis(info, XPlane, Vector3.UnitZ, Vector3.Zero);
                    SelectedComponent = TranslationComponent.Z;
                }
                else
                if (ray.CheckSphereHitIntersection(YEnd, scale * Size / 3, out intersect))
                {
                    YOffset = Center - GetPlaneInsectionAxis(info, XPlane, Vector3.UnitY, Vector3.Zero);
                    SelectedComponent = TranslationComponent.Y;
                }
                else
                if (ray.CheckSphereHitIntersection(XEnd, scale * Size / 3, out intersect))
                {
                    XOffset = Center - GetPlaneInsectionAxis(info, ZPlane, Vector3.UnitX, Vector3.Zero);
                    SelectedComponent = TranslationComponent.X;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public void MouseDown(PickInformation info)
        {
            if (SelectedComponent != TranslationComponent.None && !WasInteracting)
            {
                Interacting = true;
            }
            WasInteracting = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void MouseUp()
        {
            Interacting = false;
            WasInteracting = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderComponent(Vector3 direction, Vector3 color, TranslationComponent component)
        {
            // draw cone pointer
            GL.PushMatrix();
            switch (component)
            {
                case TranslationComponent.X:
                    {
                        var trans = Matrix4.CreateRotationZ(-1.57f) * Matrix4.CreateTranslation(direction * Size);
                        GL.MultMatrix(ref trans);
                    }
                    break;
                case TranslationComponent.Y:
                    {
                        var trans = Matrix4.CreateTranslation(direction * Size);
                        GL.MultMatrix(ref trans);
                    }
                    break;
                case TranslationComponent.Z:
                    {
                        var trans = Matrix4.CreateRotationX(1.57f) * Matrix4.CreateTranslation(direction * Size);
                        GL.MultMatrix(ref trans);
                    }
                    break;
            }

            // draw top
            DrawCone(1, 0.33f, 6, SelectedComponent == component ? ColorSelected : color);
            GL.PopMatrix();

            // draw line
            GL.LineWidth(LineThickness);
            GL.Color3(SelectedComponent == component ? ColorSelected : color);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(direction * Size / 3);
            GL.Vertex3(direction * Size);
            GL.End();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        public void Render(Camera camera, GLTextRenderer text)
        {
            //
            Update(camera);

            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Disable(EnableCap.DepthTest);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            var trans = ScaleMatrix * Transform;
            GL.MultMatrix(ref trans);

            // render components
            RenderComponent(Vector3.UnitX, ColorX, TranslationComponent.X);
            RenderComponent(Vector3.UnitY, ColorY, TranslationComponent.Y);
            RenderComponent(Vector3.UnitZ, ColorZ, TranslationComponent.Z);

            // render labels
            if (text != null)
            {
                text.RenderText(camera, "X", Matrix4.CreateTranslation(Vector3.UnitX * Size) * trans);
                text.RenderText(camera, "Y", Matrix4.CreateTranslation(Vector3.UnitY * Size) * trans);
                text.RenderText(camera, "Z", Matrix4.CreateTranslation(Vector3.UnitZ * Size) * trans);
            }

            // TODO: render plane components
            GL.LineWidth(LineThickness);

            if (SelectedComponent == (TranslationComponent.Y | TranslationComponent.Z))
                DrawPlaneSelect(Vector3.UnitY, Vector3.UnitZ);

            if (SelectedComponent == (TranslationComponent.X | TranslationComponent.Z))
                DrawPlaneSelect(Vector3.UnitX, Vector3.UnitZ);

            if (SelectedComponent == (TranslationComponent.Y | TranslationComponent.X))
                DrawPlaneSelect(Vector3.UnitY, Vector3.UnitX);

            GL.Color3(ColorX);
            GL.Begin(PrimitiveType.LineStrip);
            GL.Vertex3((Vector3.UnitX + Vector3.UnitY) * 2);
            GL.Vertex3((Vector3.UnitX) * 2);
            GL.Vertex3((Vector3.UnitX + Vector3.UnitZ) * 2);
            GL.End();

            GL.Color3(ColorY);
            GL.Begin(PrimitiveType.LineStrip);
            GL.Vertex3((Vector3.UnitY + Vector3.UnitX) * 2);
            GL.Vertex3(Vector3.UnitY * 2);
            GL.Vertex3((Vector3.UnitY + Vector3.UnitZ) * 2);
            GL.End();

            GL.Color3(ColorZ);
            GL.Begin(PrimitiveType.LineStrip);
            GL.Vertex3((Vector3.UnitZ + Vector3.UnitY) * 2);
            GL.Vertex3(Vector3.UnitZ * 2);
            GL.Vertex3((Vector3.UnitZ + Vector3.UnitX) * 2);
            GL.End();

            // render view translation
            // billboard to camera
            if (SelectedComponent == TranslationComponent.View)
                GL.Color3(ColorSelected);
            else
                GL.Color3(0.8f, 0.8f, 0.8f);
            GL.LineWidth(LineThickness);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex3(Center1);
            GL.Vertex3(Center2);
            GL.Vertex3(Center3);
            GL.Vertex3(Center4);
            GL.End();

            GL.PopMatrix();
            GL.PopAttrib();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        private void DrawPlaneSelect(Vector3 p1, Vector3 p2)
        {
            GL.Color3(ColorSelected);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3((p1) * 2);
            GL.Vertex3((p2 + p1) * 2);
            GL.Vertex3(p2 * 2);
            GL.End();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="precision"></param>
        public static void DrawCone(float height, float radius, uint precision, Vector3 color)
        {
            GL.Color4(color.X, color.Y, color.Z, 1);

            GL.Begin(PrimitiveType.TriangleFan);

            GL.Vertex3(0, height, 0);

            float twicePi = 2.0f * (float)Math.PI;

            for (int i = 0; i <= precision; i++)
            {
                GL.Vertex3(
                        0 + (radius * Math.Cos(i * twicePi / precision)),
                        0,
                        0 + (radius * Math.Sin(i * twicePi / precision))
                );

            }

            GL.End();
        }

    }
}
