using HSDRawViewer.GUI.Plugins.AirRide.GrTool.Converters;
using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools;
using IONET;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using static HSDRawViewer.GUI.Plugins.AirRide.AirRideGrDataEditor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Nodes
{
    public class GrCollisionNode : GrDrawNode
    {
        public override bool HasTransform => false;

        public void ImportModelFile()
        {
            if (Tag is not KdMesh m) return;

            var f = FileIO.OpenFile(IOManager.GetImportFileFilter());
            if (f == null) return;

            var scene = IOManager.LoadScene(f, new ImportSettings()
            {
                Triangulate = true,
            });

            var temp = KdMeshIOConverter.FromIOScene(scene);

            m.Materials.Clear();
            m.Triangles.Clear();
            m.Vertices.Clear();

            m.Triangles.AddRange(temp.Triangles);
            m.Vertices.AddRange(temp.Vertices);
            m.Materials.AddRange(temp.Materials);
        }

        public void ExportToObjectFile()
        {
            if (Tag is not KdMesh m) return;

            var f = FileIO.SaveFile("Wavefront OBJ|*.obj", Text + ".obj");
            if (f == null) return;

            IOManager.ExportScene(KdMeshIOConverter.ToIOScene(m), f, new ExportSettings());
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

        public override void BuildToolStrip(ToolStrip strip)
        {
            //var exportButton = new ToolStripButton("Export OBJ");
            //exportButton.Click += (s, e) => {
            //    ExportToObjectFile();
            //};

            //var deleteButton = new ToolStripButton("Delete");
            //deleteButton.Click += (s, e) => {
            //    OnDeleteNode?.Invoke(this);
            //};

            //strip.Items.Add(exportButton);
            //strip.Items.Add(deleteButton);
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


        public static bool TryGetPoint(KdMesh mesh, int index, out Vector3 p)
        {
            p = Vector3.Zero;

            if (index < 0 || index >= mesh.Vertices.Count) return false;

            var vd = mesh.Vertices[index];

            if (vd.Count < 3) return false;

            p = new Vector3(vd[0], vd[1], vd[2]);

            return true;
        }

        private bool TryPickTriangle(PickInformation pick, LiveJObj joint, out KdTriangle tri, out float distance)
        {
            tri = null;
            distance = float.PositiveInfinity;

            if (Tag is not KdMesh m) return false;
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

        public override bool TryPickNode(PickInformation pick, LiveJObj joint, out float distance)
        {
            return TryPickTriangle(pick, joint, out KdTriangle tri, out distance);
        }

        public override object PickData(PickInformation pick, LiveJObj joint)
        {
            if (Tag is not KdMesh m) return null;

            if (TryPickTriangle(pick, joint, out KdTriangle tri, out float distance))
                return new TriangleAccessor(m, tri);

            return null;
        }

        public class TriangleAccessor
        {
            public KdMesh _mesh;

            public KdTriangle _triangle;

            [Category("Material")]
            [DisplayName("ID#")]
            [Description("Index of Material in the parent Collision to use")]
            public int MaterialIndex
            {
                get => _triangle.Material;
                set
                {
                    _triangle.Material = value;
                }
            }

            [Category("Material")]
            [DisplayName("Data")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public KdMaterial Material
            {
                get
                {
                    int index = _triangle.Material;

                    if (index < 0 || index >= _mesh.Materials.Count)
                        return null;

                    return _mesh.Materials[index];
                }
            }

            [Category("Vertices")]
            [DisplayName("P1")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public VertexAccessor V1 => GetVertexInfo(0);

            [Category("Vertices")]
            [DisplayName("P2")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public VertexAccessor V2 => GetVertexInfo(1);

            [Category("Vertices")]
            [DisplayName("P3")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public VertexAccessor V3 => GetVertexInfo(2);

            public TriangleAccessor(KdMesh mesh, KdTriangle triangle)
            {
                _mesh = mesh;
                _triangle = triangle;
            }

            private VertexAccessor GetVertexInfo(int index)
            {
                if (_triangle.Indices.Length <= index) return null;
                int i = _triangle.Indices[index];
                if (i < 0 || i >= _mesh.Vertices.Count) return null;
                return new VertexAccessor(_mesh.Vertices[i]);
            }

            public override string ToString()
            {
                return $"Triangle";
            }
        }

        public override void Draw(GrRenderResource render, object selected_object)
        {
            if (Tag is not KdMesh m) return;
            if (!Visible) return;

            render.DrawKdMesh(m, IsSelected);
        }

        public override void DrawOverlay(GrRenderResource render, object selected_object)
        {
            if (Tag is not KdMesh m) return;
            if (!Visible) return;

            if (selected_object is TriangleAccessor acc &&
                acc._mesh == Tag)
            {
                render.DrawKdSelectedTriangle(acc._mesh, acc._triangle);
            }
        }
    }
}
