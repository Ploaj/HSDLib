using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using OpenTK.Mathematics;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public class GrPositionNode : GrDrawNode, IGrTranslate
    {
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

    }
}
