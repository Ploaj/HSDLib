using System;
using WeifenLuo.WinFormsUI.Docking;
using HSDRaw.Melee.Gr;
using HSDRawViewer.Rendering;
using System.Windows.Forms;
using OpenTK;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace HSDRawViewer.GUI.Plugins
{
    public partial class CollDataEditor : DockContent, EditorBase, IDrawableInterface
    {
#region EditorClasses

        private class LineGroup
        {
            public List<Line> Lines = new List<Line>();
            public List<Vertex> Vertices = new List<Vertex>();

            public Vector4 Range = new Vector4();

            public void CalcuateRange()
            {
                Vector4 newRange = new Vector4(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);

                foreach(var v in Lines)
                {

                }

                Range = newRange;
            }

            public override string ToString()
            {
                return "Polygon Group";
            }
        }

        private class Line
        {
            public int v1;
            public int v2;

            public CollPhysics CollisionFlag { get; set; } = CollPhysics.Top;
            public CollProperty Flag { get; set; } = 0;
            public CollMaterial Material { get; set; } = CollMaterial.Basic;
        }

        private class Vertex
        {
            public float X { get; set; }
            public float Y { get; set; }

            public Vertex(float x, float y)
            {
                X = x;
                Y = y;
            }
        }

#endregion

        public Type[] SupportedTypes => new Type[] { typeof(SBM_Coll_Data) };

        public DataNode Node { get => _node;
            set
            {
                _node = value;

                CollData = _node.Accessor as SBM_Coll_Data;

                LoadCollData();
            }
        }

        public DockState DefaultDockState => DockState.DockLeft;

        public DrawOrder DrawOrder => DrawOrder.Last;

        private DataNode _node;

        private SBM_Coll_Data CollData;

        private List<LineGroup> LineGroups = new List<LineGroup>();

        private float PlatformWidth = 10;
        private float PickRange = 5;

        public CollDataEditor()
        {
            InitializeComponent();

            cbSelectType.SelectedIndex = 0;

            PluginManager.GetCommonViewport()?.AddRenderer(this);

            FormClosing += (sender, args) =>
            {
                PluginManager.GetCommonViewport()?.RemoveRenderer(this);
            };
        }

        /// <summary>
        /// Load the coll_data into a custom editor
        /// This allows most data to be generated instead of being manually edited
        /// </summary>
        private void LoadCollData()
        {
            List<Vector2> v = new List<Vector2>();
            foreach (var ve in CollData.Vertices)
            {
                v.Add(new Vector2(ve.X, ve.Y));
            }
            PluginManager.GetCommonViewport().FrameView(v);

            LineGroups.Clear();

            var links = CollData.Links;
            var verts = CollData.Vertices;
            foreach(var group in CollData.AreaTables)
            {
                Dictionary<int, int> indexToVertex = new Dictionary<int, int>();
                var lineGroup = new LineGroup();
                lineGroup.Range = new Vector4(group.XMin, group.YMin, group.XMax, group.YMax);
                LoadGroup(group.BottomLineIndex, group.BottomLineCount, lineGroup, verts, links, indexToVertex);
                LoadGroup(group.TopLineIndex, group.TopLineCount, lineGroup, verts, links, indexToVertex);
                LoadGroup(group.LeftLineIndex, group.LeftLineCount, lineGroup, verts, links, indexToVertex);
                LoadGroup(group.RightLineIndex, group.RightLineCount, lineGroup, verts, links, indexToVertex);
                LineGroups.Add(lineGroup);
            }

            listBox1.DataSource = LineGroups;
            listBox1.SelectedIndex = 0;
        }

        private void LoadGroup(int start, int count, LineGroup lineGroup, SBM_CollVertex[] verts, SBM_CollLine[] links, Dictionary<int, int> indexToVertex)
        {
            for (int i = start; i < start+count; i++)
            {
                if (!indexToVertex.ContainsKey(links[i].VertexIndex1))
                {
                    indexToVertex.Add(links[i].VertexIndex1, lineGroup.Vertices.Count);
                    lineGroup.Vertices.Add(new Vertex(verts[links[i].VertexIndex1].X, verts[links[i].VertexIndex1].Y));
                }
                if (!indexToVertex.ContainsKey(links[i].VertexIndex2))
                {
                    indexToVertex.Add(links[i].VertexIndex2, lineGroup.Vertices.Count);
                    lineGroup.Vertices.Add(new Vertex(verts[links[i].VertexIndex2].X, verts[links[i].VertexIndex2].Y));
                }
                lineGroup.Lines.Add(new Line()
                {
                    v1 = indexToVertex[links[i].VertexIndex1],
                    v2 = indexToVertex[links[i].VertexIndex2],
                    Material = links[i].Material,
                    Flag = links[i].Flag,
                    CollisionFlag = links[i].CollisionFlag
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveCollData()
        {

        }

        public void ScreenClick(MouseButtons button, PickInformation ray)
        {
        }

        public void ScreenDoubleClick(PickInformation ray)
        {
            Vector2 nearestLine;
            float closest = float.MaxValue;
            var depthPicked = ray.GetPlaneIntersection(-Vector3.UnitZ, new Vector3(0, 0, -PlatformWidth / 2));
            var picked = ray.GetPlaneIntersection(-Vector3.UnitZ, Vector3.Zero);
            
            List<object> selected = new List<object>();

            foreach (var g in LineGroups)
            {
                if (listBox1.SelectedItem != g)
                    continue;

                if (cbSelectType.SelectedIndex == 1)
                {
                    foreach (var l in g.Lines)
                    {
                        var p1 = g.Vertices[l.v1];
                        var p2 = g.Vertices[l.v2];
                        var dis = PickInformation.GetDistanceToSegment(depthPicked.Xy, new Vector2(p1.X, p1.Y), new Vector2(p2.X, p2.Y), out nearestLine);
                        if (dis < PlatformWidth / 2 & dis < closest)
                        {
                            closest = dis;
                            selected.Add(l);
                        }
                    }
                }
                if (cbSelectType.SelectedIndex == 0)
                {
                    foreach(var v in g.Vertices)
                    {
                        if (Math3D.FastDistance(picked, new Vector3(v.X, v.Y, 0), PickRange))
                        {
                            selected.Add(v);
                        }
                    }
                }
            }

            propertyGrid1.SelectedObjects = selected.ToArray();
        }

        public void ScreenDrag(float X, float Y)
        {
        }

        public void ScreenSelectArea(PickInformation start, PickInformation end)
        {
        }

#region Rendering

        private static Dictionary<CollMaterial, Vector3> materialToColor = new Dictionary<CollMaterial, Vector3>()
        {
            { CollMaterial.Basic, new Vector3(0x80 / 255f, 0x80 / 255f, 0x80 / 255f) },
            { CollMaterial.Rock, new Vector3(0x80 / 255f, 0x60 / 255f, 0x60 / 255f) },
            { CollMaterial.Grass, new Vector3(0x40 / 255f, 0xff / 255f, 0x40 / 255f) },
            { CollMaterial.Dirt, new Vector3(0xc0 / 255f, 0x60 / 255f, 0x60 / 255f) },
            { CollMaterial.Wood, new Vector3(0xC0 / 255f, 0x80 / 255f, 0x40 / 255f) },
            { CollMaterial.HeavyMetal, new Vector3(0x60 / 255f, 0x40 / 255f, 0x40 / 255f) },
            { CollMaterial.LightMetal, new Vector3(0x40 / 255f, 0x40 / 255f, 0x40 / 255f) },
            { CollMaterial.UnkFlatZone, new Vector3(0xC0 / 255f, 0xC0 / 255f, 0xC0 / 255f) },
            { CollMaterial.AlienGoop, new Vector3(0xDF / 255f, 0x8F / 255f, 0x7F / 255f) },
            { CollMaterial.Water, new Vector3(0x30 / 255f, 0x30 / 255f, 0xFF / 255f) },
            { CollMaterial.Glass, new Vector3(0xC0 / 255f, 0xC0 / 255f, 0xFF / 255f) },
            { CollMaterial.Checkered, new Vector3(0xFF / 255f, 0xFF / 255f, 0xC0 / 255f) },
            { CollMaterial.FlatZone, new Vector3(0xC0 / 255f, 0xC0 / 255f, 0xC0 / 255f) },
            { CollMaterial.GreatBay, new Vector3(1f, 0, 0) },
        };

        public void Draw(int windowWidth, int windowHeight)
        {
            // clear depth buffer to draw overtop of scene
            GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            RenderColl_Data();
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void RenderColl_Data()
        {
            foreach (var group in LineGroups)
            {
                var alpha = 0.25f;
                if(listBox1.SelectedItem == group)
                {
                    alpha = 1.0f;
                }
                // Render the range
                GL.Color4(1f, 0f, 0f, alpha);
                GL.Begin(PrimitiveType.LineLoop);
                GL.Vertex3(group.Range.X, group.Range.Y, 0);
                GL.Vertex3(group.Range.Z, group.Range.Y, 0);
                GL.Vertex3(group.Range.Z, group.Range.W, 0);
                GL.Vertex3(group.Range.X, group.Range.W, 0);
                GL.End();

                // render the lines
                DrawCollRegion(group, alpha);
            }
        }

        private bool IsSelected(object o)
        {
            foreach(var v in propertyGrid1.SelectedObjects)
            {
                if (v == o)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines"></param>
        private void DrawCollRegion(LineGroup group, float alpha)
        {
            GL.Begin(PrimitiveType.Quads);
            
            foreach(var line in group.Lines)
            {
                if (IsSelected(line))
                {
                    GL.Color4(1, 1, 1, alpha);
                }
                else
                {
                    if (materialToColor.ContainsKey(line.Material))
                        GL.Color4(materialToColor[line.Material].X, materialToColor[line.Material].Y, materialToColor[line.Material].Z, alpha);
                    else
                        GL.Color4(0f, 0f, 0f, alpha);
                }
                var p1 = group.Vertices[line.v1];
                var p2 = group.Vertices[line.v2];
                GL.Vertex3(p1.X, p1.Y, 10);
                GL.Vertex3(p2.X, p2.Y, 10);
                GL.Vertex3(p2.X, p2.Y, -10);
                GL.Vertex3(p1.X, p1.Y, -10);
            }

            GL.End();

            GL.LineWidth(1.5f);
            GL.Begin(PrimitiveType.Lines);

            foreach (var v in group.Vertices)
            {
                if(IsSelected(v))
                    GL.Color3(Color.White);
                else
                    GL.Color3(Color.Yellow);
                GL.Vertex3(v.X, v.Y, 10);
                GL.Vertex3(v.X, v.Y, -10);
                
                if(IsSelected(v))
                    GL.Color3(Color.White);
                else
                    GL.Color3(Color.Yellow);
                GL.Vertex3(v.X, v.Y, 10);
                GL.Vertex3(v.X, v.Y, -10);
            }

            GL.End();
        }

#endregion

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveCollData();
        }
    }
}
