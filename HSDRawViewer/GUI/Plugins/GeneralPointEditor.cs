using System;
using WeifenLuo.WinFormsUI.Docking;
using HSDRaw.Melee.Gr;
using HSDRaw.Common;
using HSDRawViewer.Rendering;
using System.Windows.Forms;
using System.ComponentModel;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Input;

namespace HSDRawViewer.GUI.Plugins
{
    public partial class GeneralPointEditor : DockContent, EditorBase, IDrawableInterface
    {
        public GeneralPointEditor()
        {
            InitializeComponent();

            PointList.DataSource = PointLinks;

            PluginManager.GetCommonViewport()?.AddRenderer(this);

            propertyGrid1.PropertyValueChanged += (sender, args) =>
            {
                // refresh
                PointLinks[PointList.SelectedIndex] = PointLinks[PointList.SelectedIndex];
                SavePointChanges();
            };

            FormClosing += (sender, args) =>
            {
                Renderer.ClearRenderingCache();
                PluginManager.GetCommonViewport()?.RemoveRenderer(this);
                SavePointChanges();
            };
        }

        private class PointLink
        {
            public PointType Type { get; set; } = PointType.GlobalPlayerRespawn;

            public float X { get => JOBJ.TX; set => JOBJ.TX = value; }

            public float Y { get => JOBJ.TY; set => JOBJ.TY = value; }

            public HSD_JOBJ JOBJ;

            public void Move(float delX, float delY)
            {
                X -= delX / 2;
                Y += delY / 2;
            }

            public override string ToString()
            {
                return Type.ToString();
            }
        }

        public DockState DefaultDockState => DockState.DockLeft;

        public Type[] SupportedTypes => new Type[] { typeof(HSDRaw.Melee.Gr.SBM_GeneralPoints) };

        public DataNode Node
        {
            get => _node;
            set
            {
                _node = value;
                GeneralPoints = _node.Accessor as SBM_GeneralPoints;
                LoadData();
            }
        }
        private DataNode _node;

        private SBM_GeneralPoints GeneralPoints;

        public DrawOrder DrawOrder => DrawOrder.Last;
        
        private BindingList<PointLink> PointLinks = new BindingList<PointLink>();

        private JOBJManager Renderer = new JOBJManager();

        public void Draw(int windowWidth, int windowHeight)
        {
            // draw overtop
            GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            Renderer.UpdateNoRender();

            Vector3 MinCam = Vector3.Zero, MaxCam = Vector3.Zero;
            Vector3 MinDeath = Vector3.Zero, MaxDeath = Vector3.Zero;
            bool CamSelected = false, DeathSelected = false;

            foreach(var v in PointLinks)
            {
                var selected = (propertyGrid1.SelectedObject == v);

                var t = Renderer.GetWorldTransform(v.JOBJ);
                var point = Vector3.TransformPosition(Vector3.Zero, t);

                switch (v.Type)
                {
                    case PointType.TopLeftBoundary:
                        if(selected)
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
                    case PointType.GlobalPlayerRespawn:
                        RenderSpawn(point, selected);
                        break;
                    default:
                        RenderPoint(point, selected);
                        break;
                }
            }

            if(MinCam != Vector3.Zero && MaxCam != Vector3.Zero)
                RenderBounds(MinCam, MaxCam, new Vector3(0.5f, 0.75f, 0.75f), CamSelected);

            if (MinDeath != Vector3.Zero && MaxDeath != Vector3.Zero)
                RenderBounds(MinDeath, MaxDeath, new Vector3(1f, 1f, 0.75f), DeathSelected);
        }

        public void ScreenClick(MouseButtons button, PickInformation pick)
        {
        }

        public void ScreenDoubleClick(PickInformation pick)
        {
            var Picked = pick.GetPlaneIntersection(-Vector3.UnitZ, Vector3.Zero);
            foreach(var v in PointLinks)
            {
                var t = Renderer.GetWorldTransform(v.JOBJ);
                var point = Vector3.TransformPosition(Vector3.Zero, t);

                var close = Vector3.Zero;

                if(Math3D.FastDistance(point, Picked, RenderSize))
                {
                    propertyGrid1.SelectedObject = v;
                }
            }
        }

        public void ScreenDrag(PickInformation pick, float deltaX, float deltaY)
        {
            var mouseState = Mouse.GetState();

            var keyState = Keyboard.GetState();
            bool drag = keyState.IsKeyDown(Key.AltLeft) || keyState.IsKeyDown(Key.AltRight);

            if (drag && mouseState.IsButtonDown(MouseButton.Left) && propertyGrid1.SelectedObject is PointLink point)
            {
                point.Move(deltaX, deltaY);
            }
        }

        public void ScreenSelectArea(PickInformation start, PickInformation end)
        {
        }

        private void LoadData()
        {
            var jobj = GeneralPoints.JOBJReference.DepthFirstList;

            foreach(var v in GeneralPoints.Points)
            {
                PointLinks.Add(new PointLink() {
                    Type = v.Type,
                    JOBJ = jobj[v.JOBJIndex]
                });
            }

            Renderer.SetJOBJ(GeneralPoints.JOBJReference);
        }

        private void SavePointChanges()
        {
            SBM_GeneralPointInfo[] p = new SBM_GeneralPointInfo[PointLinks.Count];
            var jobjs = GeneralPoints.JOBJReference.DepthFirstList;

            for(int i = 0; i < p.Length; i++)
            {
                p[i] = new SBM_GeneralPointInfo()
                {
                    Type = PointLinks[i].Type,
                    JOBJIndex = (short)jobjs.IndexOf(PointLinks[i].JOBJ)
                };
            }
            GeneralPoints.Points = p;
        }

        private void PointList_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = PointList.SelectedItem;
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            // remove jobj and remove from list
            Renderer.RemoveJOBJ((PointList.SelectedItem as PointLink).JOBJ);
            PointLinks.Remove(PointList.SelectedItem as PointLink);

            SavePointChanges();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // create new jobj
            var jobj = new HSD_JOBJ();

            GeneralPoints.JOBJReference.AddChild(jobj);
            PointLinks.Add(new PointLink()
            {
                JOBJ = jobj
            });

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
            if(selected)
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
    }
}
