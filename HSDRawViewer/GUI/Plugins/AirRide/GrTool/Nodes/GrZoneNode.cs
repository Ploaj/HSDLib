using HSDRawViewer.GUI.Plugins.AirRide.GrTool.Converters;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools;
using IONET;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public class GrZoneNode : GrDrawNode, IGrTranslate
    {
        private const float PICK_RADIUS_POINT = 8f;
        private const float PICK_RADIUS_LINE = 8f;

        public override bool HasTransform => false;

        public void ImportModelFile()
        {
            if (Tag is not KdZone m) return;

            var f = FileIO.OpenFile(IOManager.GetImportFileFilter());
            if (f == null) return;

            var scene = IOManager.LoadScene(f, new ImportSettings()
            {
                Triangulate = true,
            });

            if (KdZoneIOConverter.FromIOScene(scene, out KdZone temp, out string error))
            {
                m.Triangles.Clear();
                m.Vertices.Clear();

                m.Triangles.AddRange(temp.Triangles);
                m.Vertices.AddRange(temp.Vertices);
            }
            else
            {
                MessageBox.Show(error, 
                    "Mesh Import Error",
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        public void ExportToObjectFile()
        {
            if (Tag is not KdZone m) return;

            var f = FileIO.SaveFile("Wavefront OBJ|*.obj", Text + ".obj");
            if (f == null) return;

            IOManager.ExportScene(KdZoneIOConverter.ToIOScene(m), f, new ExportSettings());
        }


        public override void BuildContextMenu(ContextMenuStrip menu)
        {
            menu.Items.Add("Import Model", null, (s, e) => {
                ImportModelFile();
            });

            menu.Items.Add("Export Model", null, (s, e) => {
                ExportToObjectFile();
            });

            menu.Items.Add("Delete", null, (s, e) => {
                OnDeleteNode?.Invoke(this);
            });
        }

        public override bool HandleShortcut(Keys key, Keys modifier)
        {
            if (key == Keys.D || key == Keys.Delete)
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
                render.DrawKdZone(m, false, selected_object);
        }

        private VertexAccessor selected_vertex = null;
        private EdgeAccessor selected_edge = null;

        public override void DrawOverlay(GrRenderResource render, object selected_object)
        {
            if (Tag is not KdZone m) return;
            if (!Visible) return;

            if (IsSelected)
            {
                render.DrawKdZone(m, true, selected_object);
                
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

            var ModelView = GetTransform(joint);
            PickInformation localPick = pick.Transform(ModelView.Inverted());

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
                    Vector3 worldHit = Vector3.TransformPosition(hit, ModelView);
                    float worldDistance = (worldHit - localPick.Origin).Length;

                    if (worldDistance < distance)
                    {
                        tri = t;
                        distance = worldDistance;
                    }
                }
            }

            return tri != null;
        }

        private EdgeAccessor TryPickEdge(PickInformation pick, Matrix4 modelView)
        {
            if (Tag is not KdZone m) return null;

            EdgeAccessor edge = null;
            float distance = float.PositiveInfinity;
            float d;
            foreach (var t in m.Triangles)
            {
                if (t.Indices.Length < 3) continue;

                Vector3 p1, p2, p3;

                if (!TryGetPoint(m, t.Indices[0], out p1)) continue;
                if (!TryGetPoint(m, t.Indices[1], out p2)) continue;
                if (!TryGetPoint(m, t.Indices[2], out p3)) continue;

                var v1 = m.Vertices[t.Indices[0]];
                var v2 = m.Vertices[t.Indices[1]];
                var v3 = m.Vertices[t.Indices[2]];

                p1 = Vector3.TransformPosition(p1, modelView);
                p2 = Vector3.TransformPosition(p2, modelView);
                p3 = Vector3.TransformPosition(p3, modelView);

                if (pick.CheckScreenLine(p1, p2, PICK_RADIUS_LINE, out d) && d < distance)
                {
                    distance = d;
                    edge = new EdgeAccessor(v1, v2);
                }
                if (pick.CheckScreenLine(p2, p3, PICK_RADIUS_LINE, out d) && d < distance)
                {
                    distance = d;
                    edge = new EdgeAccessor(v2, v3);
                }
                if (pick.CheckScreenLine(p1, p3, PICK_RADIUS_LINE, out d) && d < distance)
                {
                    distance = d;
                    edge = new EdgeAccessor(v1, v3);
                }
            }

            if (edge != null)
            {
                selected_edge = edge;
                return edge;
            }

            return null;
        }

        public override object PickData(PickInformation pick, LiveJObj joint)
        {
            if (Tag is not KdZone m) return null;

            //if (TryPickTriangle(pick, joint, out KdZoneTriangle tri, out float distance))
            //    return tri;

            Matrix4 modelview = Matrix4.Identity;
            PickInformation localPick = pick;

            if (joint != null && m.Parent >= 0 && m.Parent < joint.JointCount)
            {
                modelview = joint.GetJObjAtIndex(m.Parent).WorldTransform;
                localPick = pick.Transform(modelview.Inverted());
            }

            float distance = float.PositiveInfinity;
            List<float> vd = null;

            for (int i = 0; i < m.Vertices.Count; i++)
            {
                if (TryGetPoint(m, i, out Vector3 p))
                {
                    p = Vector3.TransformPosition(p, modelview);
                    if (localPick.CheckScreenPoint(p, PICK_RADIUS_POINT, out float d) &&
                        d < distance)
                    {
                        distance = d;
                        vd = m.Vertices[i];
                    }
                }
            }

            if (vd != null)
            {
                selected_vertex = new VertexAccessor(vd);
                return selected_vertex;
            }

            var edge = TryPickEdge(localPick, modelview);
            if (edge != null) return edge;


            return null;
        }

        public override bool TryPickNode(PickInformation pick, LiveJObj joint, out float distance)
        {
            return TryPickTriangle(pick, joint, out KdZoneTriangle tri, out distance);
        }

        private Matrix4 GetTransform(LiveJObj joint)
        {
            if (Tag is not KdZone m) return Matrix4.Identity;

            if (joint != null && 
                m.Parent >= 0 && 
                m.Parent < joint.JointCount)
            {
                return joint.GetJObjAtIndex(m.Parent).WorldTransform;
            }

            return Matrix4.Identity;
        }

        public bool CanTranslate(object selected_object)
        {
            if (selected_object == this)
                return true;

            if (selected_object == selected_vertex)
                return true;

            if (selected_object == selected_edge)
                return true;

            return false;
        }

        public Vector3 GetTranslate(object selected_object, LiveJObj joint)
        {
            var vec = Vector3.Zero;

            if (selected_object == this)
                vec = new Vector3(Vector3.Zero);

            if (selected_object == selected_vertex)
                vec = new Vector3(selected_vertex.X, selected_vertex.Y, selected_vertex.Z);

            if (selected_object == selected_edge)
                vec = selected_edge.MidPoint;

            vec = Vector3.TransformPosition(vec, GetTransform(joint));

            return vec;
        }

        public void SetTranslate(object selected_object, LiveJObj joint, Vector3 value)
        {
            value = Vector3.TransformPosition(value, GetTransform(joint).Inverted());

            if (selected_object == this)
                return;

            if (selected_object == selected_vertex)
            {
                selected_vertex.X = value.X;
                selected_vertex.Y = value.Y;
                selected_vertex.Z = value.Z;
            }

            if (selected_object == selected_edge)
                selected_edge.SetMidpoint(value);
        }
    }
}
