using HSDRawViewer.GUI.Plugins.AirRide.GrTool.Converters;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Rendering.Widgets;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public class GrZoneNode : GrDrawNode
    {
        public override bool HasTransform => false;

        public override void BuildContextMenu(ContextMenuStrip menu)
        {
            menu.Items.Add("Delete", null, (s, e) => {
                OnDeleteNode?.Invoke(this);
            });
        }

        public override bool HandleShortcut(Keys key, Keys modifier)
        {
            if (key == Keys.D)
            {
                OnDeleteNode?.Invoke(this);
                return true;
            }
            return false;
        }

        public override void Draw(GrRenderResource render, object selected_object)
        {
            if (Tag is not KdZone m) return;
            if (!Visible) return;

            if (!IsSelected)
                render.DrawKdZone(m, false);
        }

        private TranslationWidget w = new TranslationWidget();
        private VertexAccessor selected_vertex = null;

        public override void DrawOverlay(GrRenderResource render, object selected_object)
        {
            if (Tag is not KdZone m) return;
            if (!Visible) return;

            if (IsSelected)
            {
                render.DrawKdZone(m, true);
                
            }

            if (selected_object == selected_vertex)
            {
                w.Render(render.Camera);
            }
            //if (selected_object is TriangleAccessor acc &&
            //    acc._mesh == Tag)
            //{
            //    render.DrawKdSelectedTriangle(acc._mesh, acc._triangle);
            //}
        }

        public static bool TryGetPoint(KdZone mesh, int index, out Vector3 p)
        {
            p = Vector3.Zero;

            if (index < 0 || index >= mesh.Vertices.Count) return false;

            var vd = mesh.Vertices[index];

            if (vd.Count < 3) return false;

            p = new Vector3(vd[0], vd[1], vd[2]);

            return true;
        }

        private bool TryPickTriangle(PickInformation pick, LiveJObj joint, out KdZoneTriangle tri, out float distance)
        {
            tri = null;
            distance = float.PositiveInfinity;

            if (Tag is not KdZone m) return false;
            if (!Visible) return false;

            Matrix4 modelview = Matrix4.Identity;
            PickInformation localPick = pick;

            if (joint != null && m.Parent >= 0 && m.Parent < joint.JointCount)
            {
                modelview = joint.GetJObjAtIndex(m.Parent).WorldTransform;
                localPick = pick.Transform(modelview.Inverted());
            }

            foreach (var t in m.Triangles)
            {
                if (t.Indices.Length < 3) continue;

                Vector3 p1, p2, p3;

                if (!TryGetPoint(m, t.Indices[0], out p1)) continue;
                if (!TryGetPoint(m, t.Indices[1], out p2)) continue;
                if (!TryGetPoint(m, t.Indices[2], out p3)) continue;

                Vector3 hit = Vector3.Zero;
                if (localPick.CheckTriangleHit(p1, p2, p3, ref hit, out float depth))
                {
                    Vector3 worldHit = Vector3.TransformPosition(hit, modelview);
                    float worldDistance = (worldHit - pick.Origin).Length;

                    if (worldDistance < distance)
                    {
                        tri = t;
                        distance = worldDistance;
                    }
                }
            }

            return tri != null;
        }

        public override object PickData(PickInformation pick, LiveJObj joint)
        {
            if (Tag is not KdZone m) return null;

            //if (TryPickTriangle(pick, joint, out KdZoneTriangle tri, out float distance))
            //    return tri;

            //float distance = float.PositiveInfinity;
            //List<float> vd = null;

            //for (int i = 0; i < m.Vertices.Count; i++)
            //{
            //    if (TryGetPoint(m, i, out Vector3 p))
            //    {
            //        if (pick.CheckSphereHitDistance(p, 500f, out float d) &&
            //            d < distance)
            //        {
            //            distance = d;
            //            vd = m.Vertices[i];
            //        }
            //    }
            //}

            //if (vd != null)
            //{
            //    selected_vertex = new VertexAccessor(vd);
            //    w.Transform = Matrix4.CreateTranslation(selected_vertex.X, selected_vertex.Y, selected_vertex.Z);
            //    return selected_vertex;
            //}

            return null;
        }

        public override bool TryPickNode(PickInformation pick, LiveJObj joint, out float distance)
        {
            return TryPickTriangle(pick, joint, out KdZoneTriangle tri, out distance);
        }
    }
}
