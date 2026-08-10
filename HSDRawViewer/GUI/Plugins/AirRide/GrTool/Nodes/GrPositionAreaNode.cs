using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools;
using OpenTK.Mathematics;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public class GrPositionAreaNode : GrDrawNode, IGrTranslate, IGrRotate, IUndo
    {
        public override void BuildContextMenu(ContextMenuStrip menu)
        {
            menu.Items.Add("Delete", null, (s, e) => {
                OnDeleteNode?.Invoke(this);
            });
        }

        public override void Draw(GrRenderResource render, object selected_object)
        {
            if (Tag is not KdPositionArea p) return;
            if (!Visible) return;

            Vector3 color = Vector3.One;
            var parent = Parent;
            while (parent != null)
            {
                if (parent is GrCategoryPositionAreaList l)
                {
                    color = l.DisplayColor;
                    break;
                }
                parent = parent.Parent;
            }

            var is_selected = IsSelected || IsParentSelected();
            render.DrawKdPositionArea(p, is_selected, selected_object, color);
            render.DrawKdPositionAreaOverlay(p, selected_object);
        }

        public override void DrawOverlay(GrRenderResource render, object selected_object)
        {
            if (Tag is not KdPositionArea p) return;
            if (!Visible) return;
            if (!IsSelected) return;

            render.DrawKdPositionAreaOverlay(p, selected_object);
        }

        public override object PickData(PickInformation pick, LiveJObj joint)
        {
            return null;
        }

        public static bool RayTest(
            Vector3 origin,
            Vector3 direction,
            Vector3 p1,
            Vector3 p2,
            Vector3 forward,
            Vector3 up,
            out float distance,
            out Vector3 hit)
        {
            distance = 0;
            hit = default;

            Vector3 center = (p1 + p2) * 0.5f;
            Vector3 half = (p2 - p1) * 0.5f;

            forward = Vector3.Normalize(forward);
            up = Vector3.Normalize(up);

            Vector3 right = Vector3.Normalize(
                Vector3.Cross(up, forward));

            up = Vector3.Normalize(
                Vector3.Cross(forward, right));

            // Transform ray into box-local coordinates.
            Vector3 relativeOrigin = origin - center;

            Vector3 localOrigin = new Vector3(
                Vector3.Dot(relativeOrigin, right),
                Vector3.Dot(relativeOrigin, up),
                Vector3.Dot(relativeOrigin, forward));

            Vector3 localDirection = new Vector3(
                Vector3.Dot(direction, right),
                Vector3.Dot(direction, up),
                Vector3.Dot(direction, forward));

            // Ray vs AABB.
            float tMin = float.NegativeInfinity;
            float tMax = float.PositiveInfinity;

            if (!RaySlab(localOrigin.X, localDirection.X, half.X,
                         ref tMin, ref tMax))
                return false;

            if (!RaySlab(localOrigin.Y, localDirection.Y, half.Y,
                         ref tMin, ref tMax))
                return false;

            if (!RaySlab(localOrigin.Z, localDirection.Z, half.Z,
                         ref tMin, ref tMax))
                return false;

            // Entire intersection is behind the ray.
            if (tMax < 0)
                return false;

            // If we're inside the box, return the exit point.
            distance = tMin >= 0 ? tMin : tMax;

            hit = origin + direction * distance;

            return true;
        }

        private static bool RaySlab(
            float origin,
            float direction,
            float halfExtent,
            ref float tMin,
            ref float tMax)
        {
            const float epsilon = 0.000001f;

            if (MathF.Abs(direction) < epsilon)
            {
                // Ray is parallel to this pair of planes.
                return origin >= -halfExtent &&
                       origin <= halfExtent;
            }

            float invDirection = 1.0f / direction;

            float t1 = (-halfExtent - origin) * invDirection;
            float t2 = (halfExtent - origin) * invDirection;

            if (t1 > t2)
                (t1, t2) = (t2, t1);

            tMin = MathF.Max(tMin, t1);
            tMax = MathF.Min(tMax, t2);

            return tMin <= tMax;
        }

        public override bool TryPickNode(PickInformation pick, LiveJObj joint, out float distance)
        {
            distance = float.MaxValue;

            if (Tag is not KdPositionArea p) return false;
            if (!Visible) return false;

            if (RayTest(
                pick.Origin, 
                -pick.Direction, 
                new Vector3(p.P1.X, p.P1.Y, p.P1.Z),
                new Vector3(p.P2.X, p.P2.Y, p.P2.Z),
                new Vector3(p.Forward.X, p.Forward.Y, p.Forward.Z),
                Vector3.UnitY,
                out distance, 
                out Vector3 hit))
            {
                return true;
            }

            return false;
        }

        public bool CanTranslate(object selected_object)
        {
            return selected_object == Tag;
        }

        public Vector3 GetTranslate(object selected_object, LiveJObj joint)
        {
            if (selected_object == Tag &&
                selected_object is KdPositionArea p)
            {
                var mid = (new Vector3(p.P1.X, p.P1.Y, p.P1.Z) + new Vector3(p.P2.X, p.P2.Y, p.P2.Z)) * 0.5f;

                return mid;
            }

            return Vector3.Zero;
        }

        public void SetTranslate(object selected_object, LiveJObj joint, Vector3 value)
        {
            if (selected_object == Tag &&
                selected_object is KdPositionArea p)
            {
                var mid = (new Vector3(p.P1.X, p.P1.Y, p.P1.Z) + new Vector3(p.P2.X, p.P2.Y, p.P2.Z)) * 0.5f;
                var diff = value - mid;

                p.P1.X += diff.X;
                p.P1.Y += diff.Y;
                p.P1.Z += diff.Z;

                p.P2.X += diff.X;
                p.P2.Y += diff.Y;
                p.P2.Z += diff.Z;
            }
        }

        public bool CanRotate(object selected_object)
        {
            return selected_object == Tag;
        }

        public Matrix4 GetRotation(object selected_object, LiveJObj joint)
        {
            if (selected_object != Tag ||
                selected_object is not KdPositionArea p)
                return Matrix4.Identity;

            var forward = new Vector3(p.Forward.X, p.Forward.Y, p.Forward.Z);
            var up = Vector3.UnitY;
            var mid = (new Vector3(p.P1.X, p.P1.Y, p.P1.Z) + new Vector3(p.P2.X, p.P2.Y, p.P2.Z)) * 0.5f;

            return Matrix4.CreateFromQuaternion(Math3D.FromForwardUp(forward, up)) * Matrix4.CreateTranslation(mid);
        }

        public void SetRotation(object selected_object, LiveJObj joint, Quaternion value)
        {
            if (selected_object != Tag ||
                selected_object is not KdPositionArea p)
                return;

            Vector3 forward = Vector3.Transform(Vector3.UnitZ, value);

            p.Forward.X = forward.X;
            p.Forward.Y = forward.Y;
            p.Forward.Z = forward.Z;
        }


        private ObjectUndoManager _undo = new ObjectUndoManager();

        public void Undo(object selected_object)
        {
            if (selected_object != Tag ||
                Tag is not KdPositionArea p)
                return;

            _undo.Undo();
        }

        public void Commit(object selected_object)
        {
            if (selected_object != Tag ||
                Tag is not KdPositionArea p)
                return;

            _undo.Commit(p);
        }

        public void Redo(object selected_object)
        {
            if (selected_object != Tag ||
                Tag is not KdPositionArea p)
                return;

            _undo.Redo();
        }

        public void ClearHistory()
        {
            if (Tag is not KdPositionArea p)
                return;

            _undo.ClearHistory();
        }
    }
}
