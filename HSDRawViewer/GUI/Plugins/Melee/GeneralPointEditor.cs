using System;
using HSDRaw.Melee.Gr;
using HSDRaw.Common;
using HSDRawViewer.Rendering;
using System.Windows.Forms;
using System.ComponentModel;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using HSDRawViewer.Converters.Melee;
using HSDRawViewer.Rendering.Models;
using WeifenLuo.WinFormsUI.Docking;
using HSDRawViewer.GUI.Controls.MapHeadViewer;

namespace HSDRawViewer.GUI.Plugins
{
    [SupportedTypes(new Type[] { typeof(SBM_GeneralPoints) })]
    public partial class GeneralPointEditor : PluginBase, IDrawableInterface
    {
        private class PointLink
        {
            public PointType Type { get; set; } = PointType.Player1Respawn;

            public float X { get => JObj.Translation.X; set { JObj.Translation.X = value; JObj.Desc.TX = value; } }

            public float Y { get => JObj.Translation.Y; set { JObj.Translation.Y = value; JObj.Desc.TY = value; } }

            public LiveJObj JObj;

            public void Move(float delX, float delY)
            {
                X += delX;
                Y += delY;
            }

            public override string ToString()
            {
                return Type.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override DataNode Node
        {
            get => _node;
            set
            {
                _node = value;
                GeneralPoints = _node.Accessor as SBM_GeneralPoints;
                LoadData();

                var map_head = MainForm.Instance.GetSymbol("map_head") as SBM_Map_Head;
                mapheadviewer.LoadMapHead(map_head);
            }
        }
        private DataNode _node;

        private SBM_GeneralPoints GeneralPoints;

        public DrawOrder DrawOrder => DrawOrder.Last;

        private BindingList<PointLink> PointLinks = new BindingList<PointLink>();

        private LiveJObj LiveJObj;

        private MapHeadViewControl mapheadviewer;

        /// <summary>
        /// 
        /// </summary>
        public GeneralPointEditor()
        {
            InitializeComponent();

            mapheadviewer = new MapHeadViewControl();
            mapheadviewer.Dock = DockStyle.Fill;
            mapheadviewer.glViewport.AddRenderer(this);
            groupBox3.Controls.Add(mapheadviewer);

            PointList.DataSource = PointLinks;

            mapheadviewer.glViewport.AddRenderer(this);

            propertyGrid1.PropertyValueChanged += (sender, args) =>
            {
                // refresh
                PointLinks[PointList.SelectedIndex] = PointLinks[PointList.SelectedIndex];
                SavePointChanges();
            };

            FormClosing += (sender, args) =>
            {
                mapheadviewer.glViewport.RemoveRenderer(this);
                mapheadviewer.Dispose();
                SavePointChanges();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera camera, int windowWidth, int windowHeight)
        {
            // draw overtop
            GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            // update transforms
            LiveJObj.RecalculateTransforms(camera, true);

            Vector3 MinCam = Vector3.Zero, MaxCam = Vector3.Zero;
            Vector3 MinDeath = Vector3.Zero, MaxDeath = Vector3.Zero;
            bool CamSelected = false, DeathSelected = false;

            foreach (var v in PointLinks)
            {
                var selected = (propertyGrid1.SelectedObject == v);

                var point = v.JObj.WorldTransform.ExtractTranslation();

                switch (v.Type)
                {
                    case PointType.TopLeftBoundary:
                        if (selected)
                            CamSelected = true;
                        MinCam = point;
                        break;
                    case PointType.BottomRightBoundary:
                        if (selected)
                            CamSelected = true;
                        MaxCam = point;
                        break;
                    case PointType.TopLeftBlastZone:
                        if (selected)
                            DeathSelected = true;
                        MinDeath = point;
                        break;
                    case PointType.BottomRightBlastZone:
                        if (selected)
                            DeathSelected = true;
                        MaxDeath = point;
                        break;
                    case PointType.ItemSpawn1:
                    case PointType.ItemSpawn2:
                    case PointType.ItemSpawn3:
                    case PointType.ItemSpawn4:
                    case PointType.ItemSpawn5:
                    case PointType.ItemSpawn6:
                    case PointType.ItemSpawn7:
                    case PointType.ItemSpawn8:
                    case PointType.ItemSpawn9:
                    case PointType.ItemSpawn10:
                        RenderItem(point, selected);
                        break;
                    case PointType.Target1:
                    case PointType.Target2:
                    case PointType.Target3:
                    case PointType.Target4:
                    case PointType.Target5:
                    case PointType.Target6:
                    case PointType.Target7:
                    case PointType.Target8:
                    case PointType.Target9:
                    case PointType.Target10:
                        RenderTarget(point, selected);
                        break;
                    case PointType.Player1Spawn:
                    case PointType.Player2Spawn:
                    case PointType.Player3Spawn:
                    case PointType.Player4Spawn:
                    case PointType.Player1Respawn:
                    case PointType.Player2Respawn:
                    case PointType.Player3Respawn:
                    case PointType.Player4Respawn:
                        RenderSpawn(point, selected);
                        break;
                    default:
                        RenderPoint(point, selected);
                        break;
                }
            }

            if (MinCam != Vector3.Zero && MaxCam != Vector3.Zero)
                RenderBounds(MinCam, MaxCam, new Vector3(0.5f, 0.75f, 0.75f), CamSelected);

            if (MinDeath != Vector3.Zero && MaxDeath != Vector3.Zero)
                RenderBounds(MinDeath, MaxDeath, new Vector3(1f, 1f, 0.75f), DeathSelected);
        }

        private bool dragging = false;
        private Vector2 previousPoint = Vector2.Zero;
        private Vector2 previousPickPosition = Vector2.Zero;
        private float _selectionDistance = 5f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pick"></param>
        private void SelectPoint(PickInformation pick)
        {
            var selected = false;
            var Picked = pick.GetPlaneIntersection(Vector3.UnitZ, Vector3.Zero);
            foreach (var v in PointLinks)
            {
                var point = v.JObj.WorldTransform.ExtractTranslation();

                var close = Vector3.Zero;

                if (Math3D.FastDistance(point, Picked, RenderSize))
                {
                    PointList.SelectedItem = v;
                    //propertyGrid1.SelectedObject = v;
                    selected = true;
                }
            }
            if (!selected)
                propertyGrid1.SelectedObject = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="pick"></param>
        public void ScreenClick(MouseButtons button, PickInformation pick)
        {
            if (button == MouseButtons.Right &&
                dragging &&
                propertyGrid1.SelectedObject is PointLink point)
            {
                point.X = previousPoint.X;
                point.Y = previousPoint.Y;
            }
            else
            if (button == MouseButtons.Left)
            {
                SelectPoint(pick);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pick"></param>
        public void ScreenDoubleClick(PickInformation pick)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="pick"></param>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        public void ScreenDrag(MouseEventArgs args, PickInformation pick, float deltaX, float deltaY)
        {
            var Picked = pick.GetPlaneIntersection(Vector3.UnitZ, Vector3.Zero);

            if (mapheadviewer.glViewport.IsAltKey &&
                args.Button.HasFlag(MouseButtons.Left) &&
                propertyGrid1.SelectedObject is PointLink point &&
                Math3D.FastDistance(previousPickPosition, new Vector2(point.X, point.Y), _selectionDistance))
            {
                point.X = Picked.X;
                point.Y = Picked.Y;
                if (!dragging)
                    previousPoint = Picked.Xy;
                dragging = true;
            }
            else
                dragging = false;
            previousPickPosition = Picked.Xy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void ScreenSelectArea(PickInformation start, PickInformation end)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadData()
        {
            // get jobjs
            var jobj = GeneralPoints.JOBJReference.ToList;

            // set joints for rendering
            LiveJObj = new LiveJObj(GeneralPoints.JOBJReference);

            // load points
            foreach (var v in GeneralPoints.Points)
            {
                PointLinks.Add(new PointLink()
                {
                    Type = v.Type,
                    JObj = LiveJObj.GetJObjAtIndex(v.JOBJIndex),
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SavePointChanges()
        {
            var list = PointLinks;
            SBM_GeneralPointInfo[] p = new SBM_GeneralPointInfo[list.Count];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = new SBM_GeneralPointInfo()
                {
                    Type = list[i].Type,
                    JOBJIndex = (short)LiveJObj.GetIndexOfDesc(list[i].JObj.Desc)
                };
            }
            GeneralPoints.Points = p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointList_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = PointList.SelectedItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            // remove jobj and remove from list
            if (PointList.SelectedItem is PointLink link)
            {
                var jobj = link.JObj.Desc;

                foreach (var v in LiveJObj.Enumerate)
                {
                    if (v.Sibling != null && v.Sibling.Desc == jobj)
                    {
                        v.Sibling.Desc.Next = v.Sibling.Sibling.Desc.Next;
                        v.Sibling = v.Sibling.Sibling;
                        break;
                    }

                    if (v.Child != null && v.Child.Desc == jobj)
                    {
                        v.Child.Desc = v.Child.Sibling.Desc;
                        v.Child = v.Child.Sibling;
                        break;
                    }
                }

                PointLinks.Remove(link);
            }

            // save changes
            SavePointChanges();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // create new point link
            PointLinks.Add(new PointLink()
            {
                JObj = LiveJObj.AddChild(new HSD_JOBJ())
            });

            // save changes
            SavePointChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUp_Click(object sender, EventArgs e)
        {
            var index = PointList.SelectedIndex;
            if (index == -1)
                return;
            var item = PointLinks[index];

            PointLinks.RemoveAt(index);
            index -= 1;
            if (index < 0)
                index = PointLinks.Count;
            PointLinks.Insert(index, item);

            PointList.SelectedIndex = index;

            SavePointChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDown_Click(object sender, EventArgs e)
        {
            var index = PointList.SelectedIndex;
            if (index == -1)
                return;
            var item = PointLinks[index];

            PointLinks.RemoveAt(index);
            index += 1;
            if (index >= PointLinks.Count)
                index = 0;
            PointLinks.Insert(index, item);

            PointList.SelectedIndex = index;

            SavePointChanges();
        }

        #region Shapes

        private DrawableCircle circle = new DrawableCircle(0, 0, 1);

        private float RenderSize = 10;

        private void RenderBounds(Vector3 start, Vector3 end, Vector3 color, bool selected)
        {
            RenderPoint(start, selected);
            RenderPoint(end, selected);

            if (selected)
                GL.Color3(1f, 1f, 0.75f);
            else
                GL.Color3(color);

            GL.LineWidth(1f);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex3(start.X, start.Y, start.Z);
            GL.Vertex3(end.X, start.Y, start.Z);
            GL.Vertex3(end.X, end.Y, end.Z);
            GL.Vertex3(start.X, end.Y, end.Z);
            GL.End();
        }

        private void RenderPoint(Vector3 position, bool selected)
        {
            if (selected)
                circle.Color = System.Drawing.Color.Yellow;
            else
                circle.Color = System.Drawing.Color.MediumPurple;
            circle.Position = position;
            circle.Radius = RenderSize / 4;
            circle.Draw(0, 0);
        }

        private void RenderItem(Vector3 position, bool selected)
        {
            if (selected)
                GL.Color3(1f, 1f, 0f);
            else
                GL.Color3(0.5f, 0.5f, 1f);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex3(position + new Vector3(0, -RenderSize / 2, 0));
            GL.Vertex3(position + new Vector3(-RenderSize / 2, 0, 0));
            GL.Vertex3(position + new Vector3(RenderSize / 2, 0, 0));
            GL.End();
        }

        private void RenderTarget(Vector3 position, bool selected)
        {
            circle.Position = position;
            circle.Color = selected ? System.Drawing.Color.Yellow : System.Drawing.Color.Red;
            circle.Radius = RenderSize / 2;
            circle.Draw(0, 0);

            circle.Position = position + new Vector3(0, 0, 0.1f);
            circle.Color = selected ? System.Drawing.Color.Black : System.Drawing.Color.White;
            circle.Radius = RenderSize / 3;
            circle.Draw(0, 0);

            circle.Position = position + new Vector3(0, 0, 0.2f);
            circle.Color = selected ? System.Drawing.Color.Yellow : System.Drawing.Color.Red;
            circle.Radius = RenderSize / 6;
            circle.Draw(0, 0);
        }

        private void RenderSpawn(Vector3 position, bool selected)
        {
            // player
            if (selected)
                GL.Color3(1f, 1f, 0f);
            else
                GL.Color3(0.5f, 0.5f, 1f);
            GL.Begin(PrimitiveType.Quads);

            GL.Vertex3(position + new Vector3(-RenderSize / 3, 0, 0));
            GL.Vertex3(position + new Vector3(RenderSize / 3, 0, 0));
            GL.Vertex3(position + new Vector3(RenderSize / 3, RenderSize, 0));
            GL.Vertex3(position + new Vector3(-RenderSize / 3, RenderSize, 0));

            GL.End();

            // lines----------------------------------
            GL.Color3(0, 0, 0);
            GL.Begin(PrimitiveType.Lines);

            GL.Vertex3(position + new Vector3(-RenderSize / 3, 0, 0));
            GL.Vertex3(position + new Vector3(RenderSize / 3, 0, 0));

            GL.Vertex3(position + new Vector3(RenderSize / 3, RenderSize, 0));
            GL.Vertex3(position + new Vector3(-RenderSize / 3, RenderSize, 0));

            GL.Vertex3(position + new Vector3(-RenderSize / 3, 0, 0));
            GL.Vertex3(position + new Vector3(-RenderSize / 3, RenderSize, 0));

            GL.Vertex3(position + new Vector3(RenderSize / 3, 0, 0));
            GL.Vertex3(position + new Vector3(RenderSize / 3, RenderSize, 0));

            GL.End();


            // platform------------------------------------------
            if (selected)
                GL.Color3(1f, 1f, 0f);
            else
                GL.Color3(0.5f, 0.5f, 0.5f);
            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex3(position + new Vector3(RenderSize / 2, 0, 0));
            GL.Vertex3(position + new Vector3(0, 0, RenderSize / 2));
            GL.Vertex3(position + new Vector3(-RenderSize / 2, 0, 0));

            GL.Vertex3(position + new Vector3(-RenderSize / 2, 0, 0));
            GL.Vertex3(position + new Vector3(0, 0, -RenderSize / 2));
            GL.Vertex3(position + new Vector3(RenderSize / 2, 0, 0));

            // front
            GL.Vertex3(position + new Vector3(0, -RenderSize / 2, 0));
            GL.Vertex3(position + new Vector3(-RenderSize / 2, 0, 0));
            GL.Vertex3(position + new Vector3(RenderSize / 2, 0, 0));

            // back
            GL.Vertex3(position + new Vector3(0, -RenderSize / 2, 0));
            GL.Vertex3(position + new Vector3(0, 0, -RenderSize / 2));
            GL.Vertex3(position + new Vector3(0, 0, RenderSize / 2));

            // sides
            GL.Vertex3(position + new Vector3(0, -RenderSize / 2, 0));
            GL.Vertex3(position + new Vector3(0, 0, -RenderSize / 2));
            GL.Vertex3(position + new Vector3(RenderSize / 2, 0, 0));

            GL.Vertex3(position + new Vector3(0, -RenderSize / 2, 0));
            GL.Vertex3(position + new Vector3(0, 0, RenderSize / 2));
            GL.Vertex3(position + new Vector3(-RenderSize / 2, 0, 0));

            GL.End();
        }

        #endregion

        private void importPointsFromSSFFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("Smash Stage File (.ssf)|*.ssf;");

            if (f != null)
            {
                var ssf = SSF.Open(f);

                PointLinks = new BindingList<PointLink>();

                GeneralPoints.JOBJReference = new HSD_JOBJ();
                LiveJObj = new LiveJObj(GeneralPoints.JOBJReference);

                foreach (var p in ssf.Points)
                {
                    var pl = new PointLink()
                    {
                        JObj = LiveJObj.AddChild(new HSD_JOBJ()),
                        X = p.X,
                        Y = p.Y
                    };
                    switch (p.Tag.ToLower())
                    {
                        case "blaststart": pl.Type = PointType.TopLeftBlastZone; break;
                        case "blastend": pl.Type = PointType.BottomRightBlastZone; break;
                        case "camerastart": pl.Type = PointType.TopLeftBlastZone; break;
                        case "cameraend": pl.Type = PointType.BottomRightBlastZone; break;
                        case "spawn0": pl.Type = PointType.Player1Spawn; break;
                        case "spawn1": pl.Type = PointType.Player2Spawn; break;
                        case "spawn2": pl.Type = PointType.Player3Spawn; break;
                        case "spawn3": pl.Type = PointType.Player4Spawn; break;
                        case "respawn0": pl.Type = PointType.Player1Respawn; break;
                        case "respawn1": pl.Type = PointType.Player2Respawn; break;
                        case "respawn2": pl.Type = PointType.Player3Respawn; break;
                        case "respawn3": pl.Type = PointType.Player4Respawn; break;
                        default:
                            pl = null;
                            break;
                    }
                    if (pl != null)
                        PointLinks.Add(pl);
                }

                SavePointChanges();

                PointLinks.ResetBindings();
                PointList.Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kbState"></param>
        public void ViewportKeyPress(KeyEventArgs kbState)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void GLInit()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void GLFree()
        {
        }
        public bool FreezeCamera()
        {
            return false;
        }
    }
}
