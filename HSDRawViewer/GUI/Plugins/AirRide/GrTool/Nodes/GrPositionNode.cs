using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools;
using OpenTK.Mathematics;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public class GrPositionNode : GrDrawNode, IGrTranslate, IGrRotate, IUndo
    {
        public override void BuildContextMenu(ContextMenuStrip menu)
        {
            menu.Items.Add("Delete", null, (s, e) => {
                OnDeleteNode?.Invoke(this);
            });
        }

        public override void Draw(GrRenderResource render, object selected_object)
        {
            if (Tag is not KdPosition p) return;
            if (!Visible) return;

            Vector3 colorx = Vector3.One;
            Vector3 colory = Vector3.One;
            Vector3 colorz = Vector3.One;
            var parent = Parent;
            while (parent != null)
            {
                if (parent is GrCategoryPositionList l)
                {
                    colorx = l.DisplayColorX;
                    colory = l.DisplayColorY;
                    colorz = l.DisplayColorZ;
                    break;
                }
                parent = parent.Parent;
            }

            var is_selected = IsSelected || IsParentSelected();
            render.DrawKdPosition(p, is_selected, selected_object, colorx, colory, colorz);
        }

        public override void DrawOverlay(GrRenderResource render, object selected_object)
        {
        }

        public override object PickData(PickInformation pick, LiveJObj joint)
        {
            return null;
        }

        public override bool TryPickNode(PickInformation pick, LiveJObj joint, out float distance)
        {
            distance = float.MaxValue;

            if (Tag is not KdPosition p) return false;
            if (!Visible) return false;

            if (pick.CheckSphereHitDistance(new Vector3(p.Position.X, p.Position.Y, p.Position.Z), GrRenderResource.Settings.PositionRadius, out distance))
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
                selected_object is KdPosition p)
            {
                return new Vector3(p.Position.X, p.Position.Y, p.Position.Z);
            }

            return Vector3.Zero;
        }

        public void SetTranslate(object selected_object, LiveJObj joint, Vector3 value)
        {
            if (selected_object == Tag &&
                selected_object is KdPosition p)
            {
                p.Position.X = value.X;
                p.Position.Y = value.Y;
                p.Position.Z = value.Z;
            }
        }

        private ObjectUndoManager _undo = new ObjectUndoManager();

        public void Undo(object selected_object)
        {
            if (selected_object != Tag ||
                Tag is not KdPosition p)
                return;

            _undo.Undo();
        }

        public void Commit(object selected_object)
        {
            if (selected_object != Tag ||
                Tag is not KdPosition p)
                return;

            _undo.Commit(p);
        }

        public void Redo(object selected_object)
        {
            if (selected_object != Tag ||
                Tag is not KdPosition p)
                return;

            _undo.Redo();
        }

        public void ClearHistory()
        {
            if (Tag is not KdPosition p)
                return;

            _undo.ClearHistory();
        }

        public bool CanRotate(object selected_object)
        {
            return selected_object == Tag;
        }

        public Matrix4 GetRotation(object selected_object, LiveJObj joint)
        {
            if (selected_object != Tag ||
                selected_object is not KdPosition p)
                return Matrix4.Identity;

            var forward = new Vector3(p.Forward.X, p.Forward.Y, p.Forward.Z);
            var up = Vector3.UnitY;
            var mid = new Vector3(p.Position.X, p.Position.Y, p.Position.Z);

            return Matrix4.CreateFromQuaternion(Math3D.FromForwardUp(forward, up)) * Matrix4.CreateTranslation(mid);
        }

        public void SetRotation(object selected_object, LiveJObj joint, Quaternion value)
        {
            if (selected_object != Tag ||
                selected_object is not KdPosition p)
                return;

            var forward = Vector3.Transform(Vector3.UnitZ, value);
            var up = Vector3.Transform(Vector3.UnitY, value);

            p.Forward.X = forward.X;
            p.Forward.Y = forward.Y;
            p.Forward.Z = forward.Z;

            p.Up.X = up.X;
            p.Up.Y = up.Y;
            p.Up.Z = up.Z;
        }
    }
}
