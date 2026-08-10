using System;
using HSDRawViewer.Rendering.Renderers;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace HSDRawViewer.Rendering.Widgets
{
    [Flags]
    public enum RotationComponent
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4,
        View = X | Y | Z,
    }

    public class RotationWidget : IWidget
    {
        public Matrix4 Transform = Matrix4.Identity;

        public float Size { get; set; } = 4f;
        public float LineThickness { get; set; } = 2f;

        public RotationComponent SelectedComponent = RotationComponent.None;

        public Vector3 ColorX = Vector3.UnitX;
        public Vector3 ColorY = Vector3.UnitY;
        public Vector3 ColorZ = Vector3.UnitZ;
        public Vector3 ColorSelected = new(1, 1, 0);

        public bool PendingUpdate { get; internal set; }

        public bool Interacting { get; private set; }

        private bool WasInteracting;

        public delegate void UpdateTransform(Matrix4 newTransform);
        public UpdateTransform TransformUpdated;

        private float scale;
        private Matrix4 ScaleMatrix;

        private Vector3 Center;

        // Screen-space center of the widget.
        private Vector2 ScreenCenter;

        // Radius of the rendered/pickable rings in screen space.
        private float ScreenRadius;

        // --------------------------------------------------------------------
        // Axis rotation state
        // --------------------------------------------------------------------

        private Vector3 AxisWorld;
        private Vector3 StartAxisVector;
        private Matrix4 TransformBefore;

        // --------------------------------------------------------------------
        // Arcball state
        // --------------------------------------------------------------------

        private Vector3 ArcballStart;
        private Vector3 ArcballAxis;
        private float ArcballAngle;

        // --------------------------------------------------------------------
        // Picking
        // --------------------------------------------------------------------

        private PickInformation ray;

        /// <summary>
        /// Updates widget size and screen-space picking information.
        /// </summary>
        private void Update(Camera camera)
        {
            Center = Transform.ExtractTranslation();

            // Keep the widget approximately constant in screen size.
            if (!Interacting)
            {
                scale = -Vector3.TransformPosition(
                    Center,
                    camera.ModelViewMatrix).Z / 40f;

                scale *= 2f * (float)Math.Tan(camera.FovRadians / 2.0);

                if (camera.RenderWidth > camera.RenderHeight)
                    scale *= 640f / camera.RenderWidth;
                else
                    scale *= 640f / camera.RenderHeight;

                ScaleMatrix = Matrix4.CreateScale(scale);
            }

            Matrix4 trans = ScaleMatrix * Transform;

            Vector3 projected = camera.Project(trans, Vector3.Zero);

            ScreenCenter = projected.Xy;

            // Size is the radius in widget units.
            Vector3 projectedRadius = camera.Project(
                trans,
                Vector3.UnitX * Size);

            ScreenRadius = Vector2.Distance(
                ScreenCenter,
                projectedRadius.Xy);
        }

        // --------------------------------------------------------------------
        // Math helpers
        // --------------------------------------------------------------------

        private static Vector3 SafeNormalize(Vector3 v)
        {
            float lenSq = v.LengthSquared;

            if (lenSq < 0.000001f)
                return Vector3.Zero;

            return v / MathF.Sqrt(lenSq);
        }

        private static float SignedAngle(
            Vector3 a,
            Vector3 b,
            Vector3 axis)
        {
            a = SafeNormalize(a);
            b = SafeNormalize(b);
            axis = SafeNormalize(axis);

            if (a.LengthSquared < 0.000001f ||
                b.LengthSquared < 0.000001f)
                return 0;

            float sin = Vector3.Dot(axis, Vector3.Cross(a, b));
            float cos = Math.Clamp(Vector3.Dot(a, b), -1f, 1f);

            return MathF.Atan2(sin, cos);
        }

        private static Quaternion QuaternionFromTo(
            Vector3 from,
            Vector3 to)
        {
            from = SafeNormalize(from);
            to = SafeNormalize(to);

            if (from.LengthSquared < 0.000001f ||
                to.LengthSquared < 0.000001f)
                return Quaternion.Identity;

            float dot = Math.Clamp(Vector3.Dot(from, to), -1f, 1f);

            // Same direction.
            if (dot > 0.999999f)
                return Quaternion.Identity;

            // Opposite direction.
            if (dot < -0.999999f)
            {
                Vector3 axis = Vector3.Cross(from, Vector3.UnitX);

                if (axis.LengthSquared < 0.000001f)
                    axis = Vector3.Cross(from, Vector3.UnitY);

                axis.Normalize();

                return Quaternion.FromAxisAngle(axis, MathF.PI);
            }

            Vector3 axisCross = Vector3.Cross(from, to);

            Quaternion q = new Quaternion(
                axisCross.X,
                axisCross.Y,
                axisCross.Z,
                1f + dot);

            q.Normalize();

            return q;
        }

        // --------------------------------------------------------------------
        // Arcball
        // --------------------------------------------------------------------

        /// <summary>
        /// Converts a screen coordinate into a virtual trackball point.
        ///
        /// The sphere is centered on the widget and lies in screen space.
        /// Points outside the sphere are projected onto the sphere's edge.
        /// </summary>
        private Vector3 ProjectArcball(
            Camera camera,
            Vector2 point)
        {
            Vector2 p =
                point - ScreenCenter;

            float radius =
                Math.Max(ScreenRadius, 0.0001f);

            p /= radius;

            float lengthSq =
                p.X * p.X +
                p.Y * p.Y;

            Vector3 cameraVector;

            if (lengthSq <= 1f)
            {
                float z =
                    MathF.Sqrt(1f - lengthSq);

                cameraVector =
                    SafeNormalize(
                        new Vector3(
                            p.X,
                            -p.Y,
                            z));
            }
            else
            {
                p.Normalize();

                cameraVector =
                    new Vector3(
                        p.X,
                        -p.Y,
                        0);
            }

            // Camera -> world.
            Matrix4 invView =
                camera.ModelViewMatrix.Inverted();

            Vector3 worldVector =
                Vector3.TransformNormal(
                    cameraVector,
                    invView);

            return SafeNormalize(worldVector);
        }

        private void BeginArcball(PickInformation info)
        {
            ArcballStart = ProjectArcball(info.Camera, info.ScreenPoint);

            ArcballAxis = Vector3.Zero;
            ArcballAngle = 0;

            TransformBefore = Transform;
        }

        private void UpdateArcball(PickInformation info)
        {
            Vector3 current = ProjectArcball(info.Camera, info.ScreenPoint);

            Quaternion q =
                QuaternionFromTo(
                    ArcballStart,
                    current);

            // Arcball rotation is expressed in view/world space.
            //
            // We apply it to the transform while preserving translation.
            Vector3 position = TransformBefore.ExtractTranslation();

            Matrix4 rotation =
                Matrix4.CreateFromQuaternion(q);

            Matrix4 result =
                rotation * TransformBefore;

            result.Row3 = new Vector4(position, 1);

            Transform = result;

            TransformUpdated?.Invoke(Transform);
        }

        // --------------------------------------------------------------------
        // Axis rotation
        // --------------------------------------------------------------------

        private Vector3 GetWorldAxis(
            RotationComponent component)
        {
            Vector3 axis = component switch
            {
                RotationComponent.X => Vector3.UnitX,
                RotationComponent.Y => Vector3.UnitY,
                RotationComponent.Z => Vector3.UnitZ,
                _ => Vector3.Zero
            };

            return SafeNormalize(
                Vector3.TransformNormal(axis, Transform));
        }

        private Vector3 GetAxisPlaneHit(
            PickInformation info,
            Vector3 axis)
        {
            Vector3 hit =
                info.GetPlaneIntersection(
                    axis,
                    Center);

            return hit;
        }

        private void BeginAxisRotation(
            PickInformation info,
            RotationComponent component)
        {
            TransformBefore = Transform;

            Center = TransformBefore.ExtractTranslation();

            AxisWorld = GetWorldAxis(component);

            Vector3 hit =
                GetAxisPlaneHit(info, AxisWorld);

            StartAxisVector =
                SafeNormalize(hit - Center);
        }

        private void UpdateAxisRotation(PickInformation info)
        {
            Vector3 hit =
                GetAxisPlaneHit(info, AxisWorld);

            Vector3 current =
                SafeNormalize(hit - Center);

            if (current.LengthSquared < 0.000001f ||
                StartAxisVector.LengthSquared < 0.000001f)
                return;

            // Angle measured around the selected WORLD axis.
            float angle =
                SignedAngle(
                    StartAxisVector,
                    current,
                    AxisWorld);

            // Selected axis in LOCAL space.
            Vector3 localAxis = SelectedComponent switch
            {
                RotationComponent.X => Vector3.UnitX,
                RotationComponent.Y => Vector3.UnitY,
                RotationComponent.Z => Vector3.UnitZ,
                _ => Vector3.Zero
            };

            Quaternion q =
                Quaternion.FromAxisAngle(
                    localAxis,
                    angle);

            Matrix4 delta =
                Matrix4.CreateFromQuaternion(q);

            // IMPORTANT:
            // OpenTK is using row vectors here, so applying a local
            // rotation means the delta goes BEFORE the transform.
            Matrix4 result =
                delta * TransformBefore;

            // Preserve translation.
            result.Row3 =
                TransformBefore.Row3;

            Transform = result;

            TransformUpdated?.Invoke(Transform);
        }

        // --------------------------------------------------------------------
        // Ring picking
        // --------------------------------------------------------------------

        private float DistanceToScreenCircle(
            Vector2 point,
            float radius)
        {
            return MathF.Abs(
                Vector2.Distance(point, ScreenCenter) -
                radius);
        }

        private bool PickAxisRing(
            PickInformation info,
            RotationComponent component)
        {
            Vector2 p = info.ScreenPoint;

            /*
             * Project the actual axis ring into screen space.
             *
             * This is more robust than simply using the distance from the
             * center because an X/Y/Z ring becomes elliptical when viewed
             * from an angle.
             */

            const int segments = 32;

            float bestDistance = float.MaxValue;

            Vector3 axis =
                component switch
                {
                    RotationComponent.X => Vector3.UnitX,
                    RotationComponent.Y => Vector3.UnitY,
                    RotationComponent.Z => Vector3.UnitZ,
                    _ => Vector3.Zero
                };

            Vector3 u;
            Vector3 v;

            // Construct two vectors perpendicular to the rotation axis.
            if (MathF.Abs(Vector3.Dot(axis, Vector3.UnitY)) < 0.9f)
                u = SafeNormalize(Vector3.Cross(axis, Vector3.UnitY));
            else
                u = SafeNormalize(Vector3.Cross(axis, Vector3.UnitX));

            v = SafeNormalize(Vector3.Cross(axis, u));

            Matrix4 trans =
                ScaleMatrix * Transform;

            Vector2 previous =
                CameraProject(trans, u * Size);

            for (int i = 1; i <= segments; i++)
            {
                float t =
                    i / (float)segments *
                    MathF.PI * 2f;

                Vector3 local =
                    (u * MathF.Cos(t) +
                     v * MathF.Sin(t)) * Size;

                Vector2 current =
                    CameraProject(trans, local);

                float distance =
                    DistanceToSegment(
                        p,
                        previous,
                        current);

                if (distance < bestDistance)
                    bestDistance = distance;

                previous = current;
            }

            // Picking tolerance in pixels.
            return bestDistance <= Math.Max(8f, LineThickness * 3f);
        }

        private Vector2 CameraProject(
            Matrix4 transform,
            Vector3 position)
        {
            // This is replaced by the actual camera projection during
            // Update/Pick because PickAxisRing doesn't have a camera argument.
            //
            // Screen-space ring picking is therefore performed by the
            // overloaded version below.
            return Vector2.Zero;
        }

        private static float DistanceToSegment(
            Vector2 p,
            Vector2 a,
            Vector2 b)
        {
            Vector2 ab = b - a;

            float lengthSq = ab.LengthSquared;

            if (lengthSq < 0.000001f)
                return Vector2.Distance(p, a);

            float t =
                Vector2.Dot(p - a, ab) /
                lengthSq;

            t = Math.Clamp(t, 0f, 1f);

            Vector2 closest =
                a + ab * t;

            return Vector2.Distance(p, closest);
        }

        // --------------------------------------------------------------------
        // Actual camera-aware ring picking
        // --------------------------------------------------------------------

        private bool PickAxisRing(
            Camera camera,
            PickInformation info,
            RotationComponent component)
        {
            Vector2 p = info.ScreenPoint;

            const int segments = 48;

            Vector3 axis =
                component switch
                {
                    RotationComponent.X => Vector3.UnitX,
                    RotationComponent.Y => Vector3.UnitY,
                    RotationComponent.Z => Vector3.UnitZ,
                    _ => Vector3.Zero
                };

            Vector3 u;

            if (MathF.Abs(Vector3.Dot(
                    axis,
                    Vector3.UnitY)) < 0.9f)
            {
                u = SafeNormalize(
                    Vector3.Cross(axis, Vector3.UnitY));
            }
            else
            {
                u = SafeNormalize(
                    Vector3.Cross(axis, Vector3.UnitX));
            }

            Vector3 v =
                SafeNormalize(
                    Vector3.Cross(axis, u));

            Matrix4 trans =
                ScaleMatrix * Transform;

            Vector2 previous =
                camera.Project(
                    trans,
                    u * Size).Xy;

            float bestDistance = float.MaxValue;

            for (int i = 1; i <= segments; i++)
            {
                float t =
                    i / (float)segments *
                    MathF.PI * 2f;

                Vector3 local =
                    (u * MathF.Cos(t) +
                     v * MathF.Sin(t)) * Size;

                Vector2 current =
                    camera.Project(
                        trans,
                        local).Xy;

                float distance =
                    DistanceToSegment(
                        p,
                        previous,
                        current);

                bestDistance =
                    Math.Min(
                        bestDistance,
                        distance);

                previous = current;
            }

            return bestDistance <=
                Math.Max(8f, LineThickness * 3f);
        }

        // --------------------------------------------------------------------
        // Mouse interaction
        // --------------------------------------------------------------------

        public void Drag(PickInformation info)
        {
            if (info == null)
                return;

            ray = info;

            if (Interacting)
            {
                switch (SelectedComponent)
                {
                    case RotationComponent.X:
                    case RotationComponent.Y:
                    case RotationComponent.Z:
                        UpdateAxisRotation(info);
                        break;

                    case RotationComponent.View:
                        UpdateArcball(info);
                        break;
                }

                return;
            }

            SelectedComponent =
                RotationComponent.None;

            // ------------------------------------------------------------
            // Center / arcball
            // ------------------------------------------------------------

            Vector2 centerDelta =
                info.ScreenPoint - ScreenCenter;

            if (centerDelta.Length <= ScreenRadius * 0.75f)
            {
                SelectedComponent =
                    RotationComponent.View;

                return;
            }

            // ------------------------------------------------------------
            // Axis rings
            // ------------------------------------------------------------

            Camera camera = info.Camera;

            if (PickAxisRing(
                    camera,
                    info,
                    RotationComponent.X))
            {
                SelectedComponent =
                    RotationComponent.X;

                BeginAxisRotation(
                    info,
                    RotationComponent.X);

                return;
            }

            if (PickAxisRing(
                    camera,
                    info,
                    RotationComponent.Y))
            {
                SelectedComponent =
                    RotationComponent.Y;

                BeginAxisRotation(
                    info,
                    RotationComponent.Y);

                return;
            }

            if (PickAxisRing(
                    camera,
                    info,
                    RotationComponent.Z))
            {
                SelectedComponent =
                    RotationComponent.Z;

                BeginAxisRotation(
                    info,
                    RotationComponent.Z);

                return;
            }
        }

        public void MouseDown(PickInformation info)
        {
            if (SelectedComponent != RotationComponent.None &&
                !WasInteracting)
            {
                Interacting = true;

                if (SelectedComponent ==
                    RotationComponent.View)
                {
                    BeginArcball(info);
                }
                else
                {
                    BeginAxisRotation(
                        info,
                        SelectedComponent);
                }
            }

            WasInteracting = true;
        }

        public void MouseUp()
        {
            if (Interacting)
                PendingUpdate = true;

            Interacting = false;
            WasInteracting = false;
        }

        // --------------------------------------------------------------------
        // Rendering
        // --------------------------------------------------------------------

        private void DrawRing(
            Vector3 axis,
            Vector3 color,
            RotationComponent component)
        {
            Vector3 u;

            if (MathF.Abs(
                    Vector3.Dot(
                        axis,
                        Vector3.UnitY)) < 0.9f)
            {
                u = SafeNormalize(
                    Vector3.Cross(
                        axis,
                        Vector3.UnitY));
            }
            else
            {
                u = SafeNormalize(
                    Vector3.Cross(
                        axis,
                        Vector3.UnitX));
            }

            Vector3 v =
                SafeNormalize(
                    Vector3.Cross(
                        axis,
                        u));

            bool selected =
                SelectedComponent == component;

            GL.Color3(
                selected
                    ? ColorSelected
                    : color);

            GL.LineWidth(selected ? LineThickness * 2 : LineThickness);

            GL.Begin(PrimitiveType.LineLoop);

            const int segments = 64;

            for (int i = 0; i < segments; i++)
            {
                float t =
                    i / (float)segments *
                    MathF.PI * 2f;

                Vector3 p =
                    (u * MathF.Cos(t) +
                     v * MathF.Sin(t)) *
                    Size;

                GL.Vertex3(p);
            }

            GL.End();
        }

        private void DrawArcball(Camera camera)
        {
            bool selected =
                SelectedComponent == RotationComponent.View;

            GL.Color3(
                selected
                    ? ColorSelected
                    : new Vector3(0.8f, 0.8f, 0.8f));

            GL.LineWidth(selected ? LineThickness * 2 : LineThickness);

            // Get the camera's world-space right/up vectors.
            Matrix4 invView =
                camera.ModelViewMatrix.Inverted();

            Vector3 worldRight =
                SafeNormalize(invView.Row0.Xyz);

            Vector3 worldUp =
                SafeNormalize(invView.Row1.Xyz);

            // We are currently rendering with:
            //
            //     ScaleMatrix * Transform
            //
            // so convert the camera vectors back into the widget's
            // local coordinate system before submitting them.
            Matrix4 invTransform =
                Transform.Inverted();

            Vector3 right =
                SafeNormalize(
                    Vector3.TransformNormal(
                        worldRight,
                        invTransform));

            Vector3 up =
                SafeNormalize(
                    Vector3.TransformNormal(
                        worldUp,
                        invTransform));

            GL.Begin(PrimitiveType.LineLoop);

            const int segments = 64;

            for (int i = 0; i < segments; i++)
            {
                float angle =
                    i / (float)segments * MathF.PI * 2f;

                Vector3 point =
                    (right * MathF.Cos(angle) +
                     up * MathF.Sin(angle)) *
                    Size;

                GL.Vertex3(point);
            }

            GL.End();
        }

        public void Render(
            Camera camera,
            GLTextRenderer text = null)
        {
            Update(camera);

            GL.PushAttrib(
                AttribMask.AllAttribBits);

            GL.Disable(
                EnableCap.DepthTest);

            GL.MatrixMode(
                MatrixMode.Modelview);

            GL.PushMatrix();

            Matrix4 trans =
                ScaleMatrix * Transform;

            GL.MultMatrix(
                ref trans);

            // Axis rings.
            DrawRing(
                Vector3.UnitX,
                ColorX,
                RotationComponent.X);

            DrawRing(
                Vector3.UnitY,
                ColorY,
                RotationComponent.Y);

            DrawRing(
                Vector3.UnitZ,
                ColorZ,
                RotationComponent.Z);

            // Free rotation / arcball ring.
            DrawArcball(camera);

            GL.PopMatrix();

            GL.PopAttrib();
        }
    }
}
