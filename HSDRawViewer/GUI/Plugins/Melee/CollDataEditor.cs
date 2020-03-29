using System;
using System.Linq;
using WeifenLuo.WinFormsUI.Docking;
using HSDRaw.Melee.Gr;
using HSDRawViewer.Rendering;
using System.Windows.Forms;
using OpenTK;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Input;
using System.ComponentModel;
using HSDRawViewer.GUI.Plugins.Melee;

namespace HSDRawViewer.GUI.Plugins
{
    public partial class CollDataEditor : DockContent, EditorBase, IDrawableInterface
    {
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

        private BindingList<CollLineGroup> LineGroups = new BindingList<CollLineGroup>();

        private IEnumerable<CollLine> SelectedLines
        {
            get
            {
                foreach(CollLine l in listBox1.SelectedItems)
                {
                    yield return l;
                }
            }
        }

        private BindingList<CollLine> Lines
        {
            get; set;
        } = new BindingList<CollLine>();

        private CollLineGroup SelectedLineGroup
        {
            get
            {
                return listBox1.SelectedItem as CollLineGroup;
            }
        }

        private List<CollVertex> Vertices
        {
            get
            {
                var vl = new List<CollVertex>();
                foreach (var l in Lines)
                {
                    if(!vl.Contains(l.v1))
                        vl.Add(l.v1);
                    if (!vl.Contains(l.v2))
                        vl.Add(l.v2);
                }
                return vl;
            }
        }

        private List<CollLine> SelectedGroupLines
        {
            get
            {
                var vl = new List<CollLine>();
                foreach (var l in Lines)
                {
                    if (cbShowAllGroups.Checked || l.Group == SelectedLineGroup)
                        vl.Add(l);
                }
                return vl;
            }
        }

        private List<CollVertex> SelectedGroupVertices
        {
            get
            {
                var vl = new List<CollVertex>();
                foreach (var l in Lines)
                {
                    if (cbShowAllGroups.Checked && l.Group != SelectedLineGroup)
                        continue;
                    if (!vl.Contains(l.v1))
                        vl.Add(l.v1);
                    if (!vl.Contains(l.v2))
                        vl.Add(l.v2);
                }
                return vl;
            }
        }

        // Parameters

        private float PlatformWidth = 20;
        private float PickRange = 100;

        public CollDataEditor()
        {
            InitializeComponent();

            cbSelectType.SelectedIndex = 0;
            cbRenderModes.SelectedIndex = 0;

            listLines.DataSource = Lines;

            PluginManager.GetCommonViewport()?.AddRenderer(this);
            
            listBox1.SelectedIndexChanged += (sender, args) =>
            {
                ClearSelection();
                propertyGrid1.SelectedObject = SelectedLineGroup;
            };

            bool updateList = false;

            listLines.SelectedIndexChanged += (sender, args) =>
            {
                if (!updateList)
                {
                    var sel = new CollLine[listLines.SelectedItems.Count];
                    for (int i = 0; i < sel.Length; i++)
                        sel[i] = listLines.SelectedItems[i] as CollLine;
                    propertyGrid1.SelectedObjects = sel;
                }
            };

            propertyGrid1.SelectedObjectsChanged += (sender, args) =>
            {
                updateList = true;
                listLines.SelectedItem = null;
                foreach (var ob in propertyGrid1.SelectedObjects)
                    if (ob is CollLine l)
                    {
                        var index = Lines.IndexOf(l);
                        listLines.SetSelected(index, true);
                    }
                updateList = false;

                deleteButton.Enabled = (propertyGrid1.SelectedObjects.Length != 0);

                editVertexMenu.Enabled = (propertyGrid1.SelectedObjects.Length > 0 && propertyGrid1.SelectedObject is CollVertex);

                editLineMenu.Enabled = (propertyGrid1.SelectedObjects.Length > 0 && propertyGrid1.SelectedObject is CollLine);
            };

            FormClosing += (sender, args) =>
            {
                PluginManager.GetCommonViewport()?.RemoveRenderer(this);
            };
        }

        private void ClearSelection()
        {
            propertyGrid1.SelectedObject = null;
        }
        
        #region Interaction

        public void ScreenClick(MouseButtons button, PickInformation ray)
        {
            propertyGrid1.Refresh();
        }

        public void ScreenDoubleClick(PickInformation ray)
        {
            var keyState = Keyboard.GetState();
            bool multiSelect = keyState.IsKeyDown(Key.ControlLeft) || keyState.IsKeyDown(Key.ControlRight);

            float closest = float.MaxValue;
            var pick2D = ray.GetPlaneIntersection(-Vector3.UnitZ, Vector3.Zero);
            
            List<object> selected = new List<object>();

            object selectedObject = null;

            if (multiSelect)
                selected.AddRange(propertyGrid1.SelectedObjects);

            if (cbSelectType.SelectedIndex == 1)
            {
                var lindex = -1;
                foreach (var l in SelectedGroupLines)
                {
                    lindex++;
                    var p1 = l.v1;
                    var p2 = l.v2;
                    var dis = Math3D.DistanceSquared(ray.Origin, new Vector3((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2, 0));
                    if (dis < closest && ray.IntersectsQuad(
                        new Vector3(p1.X, p1.Y, PlatformWidth / 2),
                        new Vector3(p2.X, p2.Y, PlatformWidth / 2),
                        new Vector3(p2.X, p2.Y, -PlatformWidth / 2),
                        new Vector3(p1.X, p1.Y, -PlatformWidth / 2))
                       )
                    {
                        closest = dis;
                        selectedObject = l;
                    }
                }
            }
            if (cbSelectType.SelectedIndex == 0)
            {
                foreach (var v in SelectedGroupVertices)
                {
                    var dis = Math3D.DistanceSquared(pick2D, new Vector3(v.X, v.Y, 0));
                    if (dis < closest && dis < PickRange)
                    {
                        closest = dis;
                        selectedObject = v;
                    }
                }
            }
            
            if(selectedObject != null)
            {
                if (!selected.Contains(selectedObject))
                    selected.Add(selectedObject);

                propertyGrid1.SelectedObjects = selected.ToArray();
            }
            else
            {
                if (!multiSelect)
                    propertyGrid1.SelectedObject = null;
            }
        }
        
        public void ScreenDrag(PickInformation pick, float Xdelta, float Ydelta)
        {
            var mouseState = Mouse.GetState();

            var keyState = Keyboard.GetState();
            bool drag = keyState.IsKeyDown(Key.AltLeft) || keyState.IsKeyDown(Key.AltRight);

            HashSet<Vector2> moved = new HashSet<Vector2>();

            if (drag && mouseState.IsButtonDown(MouseButton.Left))
            {
                var pick2D = pick.GetPlaneIntersection(-Vector3.UnitZ, Vector3.Zero);
                foreach (var v in propertyGrid1.SelectedObjects)
                {
                    if (v is CollVertex vert && !moved.Contains(vert.ToVector2()))
                    {
                        if(propertyGrid1.SelectedObjects.Length == 1)
                        {
                            vert.X = pick2D.X;
                            vert.Y = pick2D.Y;
                        }
                        else
                        {
                            vert.X -= Xdelta;
                            vert.Y += Ydelta;
                        }
                        moved.Add(vert.ToVector2());
                    }
                    if (v is CollLine line)
                    {
                        var group = listBox1.SelectedItem as CollLineGroup;

                        line.v1.X -= Xdelta;
                        line.v1.Y += Ydelta;

                        line.v2.X -= Xdelta;
                        line.v2.Y += Ydelta;
                    }
                }
            }

        }

        public void ScreenSelectArea(PickInformation start, PickInformation end)
        {
            var pickedStart = start.GetPlaneIntersection(-Vector3.UnitZ, Vector3.Zero);
            var pickedEnd = end.GetPlaneIntersection(-Vector3.UnitZ, Vector3.Zero);

            List<object> selected = new List<object>();

            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Key.ControlLeft) || keyState.IsKeyDown(Key.ControlRight))
                selected.AddRange(propertyGrid1.SelectedObjects);

            if (cbSelectType.SelectedIndex == 1)
            {
                foreach (var l in SelectedGroupLines)
                {
                    var p1 = l.v1;
                    var p2 = l.v2;

                    if (Within(pickedStart.Xy, pickedEnd.Xy, new Vector2(p1.X, p1.Y))
                        || Within(pickedStart.Xy, pickedEnd.Xy, new Vector2(p2.X, p2.Y)))
                    {
                        if (!selected.Contains(l))
                            selected.Add(l);
                    }
                }
            }
            if (cbSelectType.SelectedIndex == 0)
            {
                foreach (var v in SelectedGroupVertices)
                {
                    if (Within(pickedStart.Xy, pickedEnd.Xy, new Vector2(v.X, v.Y)))
                    {
                        if (!selected.Contains(v))
                            selected.Add(v);
                    }
                }
            }

            propertyGrid1.SelectedObjects = selected.ToArray();
        }

        private bool Within(Vector2 start, Vector2 end, Vector2 point)
        {
            return point.X > Math.Min(start.X, end.X) && point.X < Math.Max(start.X, end.X) &&
                point.Y > Math.Min(start.Y, end.Y) && point.Y < Math.Max(start.Y, end.Y)
                ;
        }

        #endregion

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

        //private int BlinkTimer = 0;

        //private bool Blink { get => BlinkTimer < 15; }

        public void Draw(Camera camera, int windowWidth, int windowHeight)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            // clear depth buffer to draw overtop of scene
            GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Disable(EnableCap.CullFace);

            RenderColl_Data();

            GL.PopAttrib();

            /*BlinkTimer++;
            if (BlinkTimer > 30)
                BlinkTimer = 0;*/
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void RenderColl_Data()
        {
            var selected = SelectedLineGroup;

            // render selected group first
            RenderGroup(selected, 1);

            // render all group ranges with lowered opacity
            foreach (var group in LineGroups)
            {
                if(selected == group)
                    continue;
                RenderGroup(group, 0.25f);
            }

            // render lines
            GL.Begin(PrimitiveType.Quads);

            foreach (var line in Lines)
            {
                var alpha = 0.25f;
                if (line.Group == SelectedLineGroup)
                    alpha = 1;
                else if (!cbShowAllGroups.Checked)
                    continue;

                if (IsSelected(line))// && Blink)
                {
                    GL.Color4(1, 1, 1, alpha);
                }
                else
                {
                    if (cbRenderModes.SelectedIndex == 0)
                    {
                        if (materialToColor.ContainsKey(line.Material))
                            GL.Color4(materialToColor[line.Material].X, materialToColor[line.Material].Y, materialToColor[line.Material].Z, alpha);
                        else
                            GL.Color4(0f, 0f, 0f, alpha);
                    }
                    else
                    if (cbRenderModes.SelectedIndex == 1)
                    {
                        if (line.DynamicCollision)
                        {
                            GL.Color4(1f, 1f, 1f, alpha);
                        }
                        else
                            switch (line.CollisionFlag)
                            {
                                case CollPhysics.Top:
                                    GL.Color4(0.75f, 0.75f, 0.75f, alpha);
                                    break;
                                case CollPhysics.Bottom:
                                    GL.Color4(0.75f, 0.5f, 0.5f, alpha);
                                    break;
                                case CollPhysics.Left:
                                    GL.Color4(0.5f, 0.5f, 0.75f, alpha);
                                    break;
                                case CollPhysics.Right:
                                    GL.Color4(0.5f, 0.75f, 0.5f, alpha);
                                    break;
                            }
                    }
                    else
                    if (cbRenderModes.SelectedIndex == 2)
                    {
                        GL.Color4(0f, 0f, 0f, alpha);
                        switch (line.Flag)
                        {
                            case CollProperty.DropThrough:
                                GL.Color4(1f, 0f, 0f, alpha);
                                break;
                            case CollProperty.LedgeGrab:
                                GL.Color4(0f, 1f, 0f, alpha);
                                break;
                            case CollProperty.Unknown:
                                GL.Color4(1f, 1f, 1f, alpha);
                                break;
                        }
                    }
                }
                var p1 = line.v1;
                var p2 = line.v2;
                GL.Vertex3(p1.X, p1.Y, 10);
                GL.Vertex3(p2.X, p2.Y, 10);
                GL.Vertex3(p2.X, p2.Y, -10);
                GL.Vertex3(p1.X, p1.Y, -10);
            }

            GL.End();

            // render points
            GL.LineWidth(2f);
            GL.Begin(PrimitiveType.Lines);

            GL.DepthFunc(DepthFunction.Always);

            foreach (var v in Lines)
            {
                if (v.Group != SelectedLineGroup && !cbShowAllGroups.Checked)
                    continue;

                if (IsSelected(v.v1)) //&& Blink)
                    GL.Color3(Color.White);
                else
                    GL.Color3(Color.Black);
                GL.Vertex3(v.v1.X, v.v1.Y, 10);
                GL.Vertex3(v.v1.X, v.v1.Y, -10);

                if (IsSelected(v.v2)) // && Blink)
                    GL.Color3(Color.White);
                else
                    GL.Color3(Color.Black);
                GL.Vertex3(v.v2.X, v.v2.Y, 10);
                GL.Vertex3(v.v2.X, v.v2.Y, -10);
            }

            GL.DepthFunc(DepthFunction.Lequal);

            GL.End();
        }

        private void RenderGroup(CollLineGroup group, float alpha)
        {
            GL.Color4(1f, 0f, 0f, alpha);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex3(group.Range.X, group.Range.Y, 0);
            GL.Vertex3(group.Range.Z, group.Range.Y, 0);
            GL.Vertex3(group.Range.Z, group.Range.W, 0);
            GL.Vertex3(group.Range.X, group.Range.W, 0);
            GL.End();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private bool IsSelected(object o)
        {
            foreach(var v in propertyGrid1.SelectedObjects)
            {
                if (v == o)
                    return true;
            }
            return false;
        }

        #endregion

        #region Buttons

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveCollData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddGroup_Click(object sender, EventArgs e)
        {
            LineGroups.Add(new CollLineGroup());
            ClearSelection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDeleteGroup_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedItem != null)
            {
                LineGroups.Remove(listBox1.SelectedItem as CollLineGroup);
            }
            ClearSelection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            var selected = propertyGrid1.SelectedObjects;
            ClearSelection();

            var group = listBox1.SelectedItem as CollLineGroup;

            foreach (var v in selected)
            {
                // delete all lines containing vert
                if(v is CollVertex vert)
                {
                    var toRem = new List<CollLine>();
                    foreach(var l in Lines)
                    {
                        if(l.v1 == v || l.v2 == v)
                        {
                            toRem.Add(l);
                        }
                    }
                    foreach(var r in toRem)
                        Lines.Remove(r);
                }

                // delete line
                if (v is CollLine line)
                {
                    Lines.Remove(line);
                }

                if (v is CollLineGroup g)
                    LineGroups.Remove(g);

            }
        }

        #endregion
        
        #region Saving Loading

        /// <summary>
        /// Load the coll_data into a custom editor
        /// This allows most data to be generated instead of being manually edited
        /// </summary>
        private void LoadCollData()
        {
            // Load Vertices
            Dictionary<int, CollVertex> indexToVertex = new Dictionary<int, CollVertex>();
            Dictionary<CollVertex, int> vertexToIndex = new Dictionary<CollVertex, int>();
            List<Vector2> v = new List<Vector2>();
            foreach (var ve in CollData.Vertices)
            {
                var vert = new CollVertex(ve.X, ve.Y);
                indexToVertex.Add(v.Count, vert);
                vertexToIndex.Add(vert, v.Count);
                v.Add(new Vector2(ve.X, ve.Y));
            }

            // Frame Viewport
            PluginManager.GetCommonViewport().FrameView(v);

            //
            LineGroups.Clear();

            var links = CollData.Links;
            var verts = CollData.Vertices;
            var groups = CollData.LineGroups.ToList();

            //List<Line> Lines = new List<Line>();
            
            for(int lineIndex = 0; lineIndex < links.Length; lineIndex++)
            {
                var line = links[lineIndex];
                Lines.Add(new CollLine()
                {
                    v1 = indexToVertex[line.VertexIndex1],
                    v2 = indexToVertex[line.VertexIndex2],
                    Material = line.Material,
                    Flag = line.Flag,
                    CollisionFlag = line.CollisionFlag,
                    DynamicCollision = lineIndex >= CollData.DynamicLinksOffset && lineIndex < CollData.DynamicLinksOffset + CollData.DynamicLinksCount
                });
            }

            for (int lineIndex = 0; lineIndex < links.Length; lineIndex++)
            {
                var line = links[lineIndex];
                var l = Lines[lineIndex];

                if (line.NextLineAltGroup != -1)
                    l.AltNext = Lines[line.NextLineAltGroup];

                if (line.PreviousLineAltGroup != -1)
                    l.AltPrevious = Lines[line.PreviousLineAltGroup];
            }

            foreach (var group in groups)
            {
                // Create group and range
                var lineGroup = new CollLineGroup();
                lineGroup.Range = new Vector4(group.XMin, group.YMin, group.XMax, group.YMax);

                // add vertices
                var index = 0;
                foreach(var l in Lines)
                {
                    // if the vertex belongs to this group
                    if ((vertexToIndex[l.v1] >= group.VertexStart && vertexToIndex[l.v1] < group.VertexStart + group.VertexCount) ||
                        (vertexToIndex[l.v2] >= group.VertexStart && vertexToIndex[l.v2] < group.VertexStart + group.VertexCount))
                        l.Group = lineGroup;

                    // if the line is indexed here
                    /*if ((index >= group.TopLineIndex && index < group.TopLineIndex + group.TopLineCount) ||
                        (index >= group.BottomLineIndex && index < group.BottomLineIndex + group.BottomLineCount) ||
                        (index >= group.LeftLineIndex && index < group.LeftLineIndex + group.LeftLineCount) ||
                        (index >= group.RightLineIndex && index < group.RightLineIndex + group.RightLineCount))
                        l.Group = lineGroup;*/

                    index++;
                }

                LineGroups.Add(lineGroup);
            }

            listBox1.DataSource = LineGroups;
            listBox1.SelectedIndex = 0;
        }

        public void SaveCollData()
        {
            // remove fake lines
            var fake = Lines.Where(e => e.CollisionFlag == 0).ToArray();
            foreach (var v in fake)
                Lines.Remove(v);

            CollDataBuilder.GenerateCollData(Lines, LineGroups, CollData);
        }
        
        #endregion

        #region Controls
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCalculateRange_Click(object sender, EventArgs e)
        {
            var verts = new List<CollVertex>();

            foreach (var l in Lines.Where(p => p.Group == SelectedLineGroup))
            {
                verts.Add(l.v1);
                verts.Add(l.v2);
            }

            SelectedLineGroup.CalcuateRange(verts);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (propertyGrid1.SelectedObjects.Length == 1 && propertyGrid1.SelectedObject is CollVertex vert)
            {
                var lines = Lines.Where(r => r.v2 == vert).ToList();
                var v2 = new CollVertex(vert.X + 10, vert.Y);

                if (lines.Count > 0)
                {
                    v2 = new CollVertex((vert.X + lines[0].v1.X) / 2, (vert.Y + lines[0].v1.Y) / 2);
                    lines[0].v2 = v2;
                }

                Lines.Add(new CollLine()
                {
                    v1 = vert,
                    v2 = v2,
                    Group = lines[0].Group,
                    CollisionFlag = lines[0].CollisionFlag,
                    Flag = lines[0].Flag,
                    Material = lines[0].Material
                });
                propertyGrid1.SelectedObject = Lines[Lines.Count - 1];
            }
            else
            if (propertyGrid1.SelectedObjects.Length == 1 && propertyGrid1.SelectedObject is CollLine line)
            {
                Lines.Add(new CollLine()
                {
                    v1 = line.v2,
                    v2 = new CollVertex(line.v2.X + 10, line.v2.Y),
                    Material = line.Material,
                    CollisionFlag = line.CollisionFlag,
                    Flag = line.Flag,
                    Group = line.Group
                });
                propertyGrid1.SelectedObject = Lines[Lines.Count - 1];
            }
            if (propertyGrid1.SelectedObjects.Length == 0 || propertyGrid1.SelectedObject is CollLineGroup)
            {
                Lines.Add(new CollLine()
                {
                    v1 = new CollVertex(-10, 0),
                    v2 = new CollVertex(10, 0),
                    Group = (CollLineGroup)propertyGrid1.SelectedObject
                });
                propertyGrid1.SelectedObject = Lines[Lines.Count - 1];
            }
        }

        /// <summary>
        /// Removes the vertex but fuses the link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (CollVertex v in propertyGrid1.SelectedObjects)
            {
                var linesv1 = Lines.Where(l => l.v1 == v).ToList();
                var linesv2 = Lines.Where(l => l.v2 == v).ToList();

                if (linesv1.Count > 1)
                    foreach (var l in linesv1)
                        Lines.Remove(l);

                if (linesv2.Count > 1)
                    foreach (var l in linesv2)
                        Lines.Remove(l);

                if(linesv1.Count == 1 && linesv2.Count == 1)
                {
                    linesv1[0].v1 = linesv2[0].v1;
                    Lines.Remove(linesv2[0]);
                }
            }

            // just select nothing after the removal
            propertyGrid1.SelectedObject = null;
        }

        /// <summary>
        /// fuses all selected vertices into one vertex
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fuseSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (propertyGrid1.SelectedObjects.Length < 1)
                return;

            var mid = new CollVertex(0, 0);

            foreach(CollVertex v in propertyGrid1.SelectedObjects)
            {
                mid.X += v.X;
                mid.Y += v.Y;
            }
            mid.X /= propertyGrid1.SelectedObjects.Length;
            mid.Y /= propertyGrid1.SelectedObjects.Length;

            foreach(var l in Lines)
            {
                if (IsSelected(l.v1))
                    l.v1 = mid;
                if (IsSelected(l.v2))
                    l.v2 = mid;
            }

            propertyGrid1.SelectedObject = mid;
        }

        /// <summary>
        /// Splits vertices that share the same position into 2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (propertyGrid1.SelectedObjects.Length < 1)
                return;

            HashSet<Vector2> has = new HashSet<Vector2>();

            foreach(CollVertex v in propertyGrid1.SelectedObjects)
            {
                var lines = Lines.Where(l => l.v1 == v || l.v2 == v);

                foreach(var l in lines)
                {
                    if (has.Contains(l.v1.ToVector2()))
                        l.v1 = new CollVertex(v.X, v.Y);
                    else
                        has.Add(l.v1.ToVector2());

                    if (has.Contains(l.v2.ToVector2()))
                        l.v2 = new CollVertex(v.X, v.Y);
                    else
                        has.Add(l.v2.ToVector2());
                }

            }

            // just select nothing after the split
            propertyGrid1.SelectedObject = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addToSelectedGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(CollLine l in propertyGrid1.SelectedObjects)
            {
                l.Group = SelectedLineGroup;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(var p in propertyGrid1.SelectedObjects)
            {
                if(p is CollLine line)
                {
                    var v2 = new CollVertex((line.v1.X + line.v2.X) / 2, (line.v1.Y + line.v2.Y) / 2);

                    Lines.Add(new CollLine()
                    {
                        v1 = v2,
                        v2 = line.v2,
                        Group = line.Group,
                        Material = line.Material,
                        CollisionFlag = line.CollisionFlag,
                        Flag = line.Flag
                    });

                    line.v2 = v2;
                }
            }

            propertyGrid1.SelectedObject = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void createLineFromSelectedToolStripMenuItem_Click(object sender, EventArgs args)
        {
            if(propertyGrid1.SelectedObjects.Length == 2)
            {
                if(propertyGrid1.SelectedObjects[0] is CollVertex v1 && propertyGrid1.SelectedObjects[1] is CollVertex v2)
                {
                    // only create a new line if one does not already exist
                    var dup = Lines.Where(e => (e.v1 == v1 && e.v2 == v2) || (e.v1 == v2 && e.v2 == v1));

                    if (dup == null || dup.Count() == 0)
                        Lines.Add(new CollLine()
                        {
                            Group = SelectedLineGroup,
                            v1 = v1,
                            v2 = v2
                        });
                }
            }
        }
        #endregion
    }
}
