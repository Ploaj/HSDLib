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
            Renderer.UpdateNoRender();

            foreach(var v in PointLinks)
            {
                var selected = (propertyGrid1.SelectedObject == v);

                var t = Renderer.GetWorldTransform(v.JOBJ);
                var point = Vector3.TransformPosition(Vector3.Zero, t);

                switch (v.Type)
                {
                    case PointType.Player1Spawn:
                    case PointType.Player2Spawn:
                    case PointType.Player3Spawn:
                    case PointType.Player4Spawn:
                    case PointType.GlobalPlayerRespawn:
                        RenderSpawn(point, selected);
                        break;
                    default:
                        if(selected)
                            GL.Color3(1f, 1f, 0f);
                        else
                            GL.Color3(1f, 1f, 1f);
                        GL.PointSize(7f);
                        GL.Begin(PrimitiveType.Points);
                        GL.Vertex3(point);
                        GL.End();
                        break;
                }
            }
        }

        public void ScreenClick(MouseButtons button, PickInformation pick)
        {
        }

        public void ScreenDoubleClick(PickInformation pick)
        {
            float closest = float.MaxValue;
            foreach(var v in PointLinks)
            {
                var t = Renderer.GetWorldTransform(v.JOBJ);
                var point = Vector3.TransformPosition(Vector3.Zero, t);

                var close = Vector3.Zero;

                if(pick.CheckSphereHit(point, 15, out close))
                {
                    var dis = Math3D.DistanceSquared(close, pick.Origin);

                    if(dis < closest)
                    {
                        closest = dis;
                        propertyGrid1.SelectedObject = v;
                    }
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

        private void RenderSpawn(Vector3 position, bool selected)
        {
            float size = 7;

            // player
            if (selected)
                GL.Color3(1f, 1f, 0f);
            else
                GL.Color3(0.5f, 0.5f, 1f);
            GL.Begin(PrimitiveType.Quads);

            GL.Vertex3(position + new Vector3(-size / 3, 0, 0));
            GL.Vertex3(position + new Vector3(size / 3, 0, 0));
            GL.Vertex3(position + new Vector3(size / 3, size, 0));
            GL.Vertex3(position + new Vector3(-size / 3, size, 0));

            GL.End();

            // lines----------------------------------
            GL.Color3(0, 0, 0);
            GL.Begin(PrimitiveType.Lines);

            GL.Vertex3(position + new Vector3(-size / 3, 0, 0));
            GL.Vertex3(position + new Vector3(size / 3, 0, 0));

            GL.Vertex3(position + new Vector3(size / 3, size, 0));
            GL.Vertex3(position + new Vector3(-size / 3, size, 0));

            GL.Vertex3(position + new Vector3(-size / 3, 0, 0));
            GL.Vertex3(position + new Vector3(-size / 3, size, 0));

            GL.Vertex3(position + new Vector3(size / 3, 0, 0));
            GL.Vertex3(position + new Vector3(size / 3, size, 0));

            GL.End();


            // platform------------------------------------------
            if (selected)
                GL.Color3(1f, 1f, 0f);
            else
                GL.Color3(0.5f, 0.5f, 0.5f);
            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex3(position + new Vector3(size / 2, 0, 0));
            GL.Vertex3(position + new Vector3(0, 0, size / 2));
            GL.Vertex3(position + new Vector3(-size / 2, 0, 0));

            GL.Vertex3(position + new Vector3(-size / 2, 0, 0));
            GL.Vertex3(position + new Vector3(0, 0, -size / 2));
            GL.Vertex3(position + new Vector3(size / 2, 0, 0));

            // front
            GL.Vertex3(position + new Vector3(0, -size / 2, 0));
            GL.Vertex3(position + new Vector3(-size / 2, 0, 0));
            GL.Vertex3(position + new Vector3(size / 2, 0, 0));

            // back
            GL.Vertex3(position + new Vector3(0, -size / 2, 0));
            GL.Vertex3(position + new Vector3(0, 0, -size / 2));
            GL.Vertex3(position + new Vector3(0, 0, size / 2));

            // sides
            GL.Vertex3(position + new Vector3(0, -size / 2, 0));
            GL.Vertex3(position + new Vector3(0, 0, -size / 2));
            GL.Vertex3(position + new Vector3(size / 2, 0, 0));

            GL.Vertex3(position + new Vector3(0, -size / 2, 0));
            GL.Vertex3(position + new Vector3(0, 0, size / 2));
            GL.Vertex3(position + new Vector3(-size / 2, 0, 0));

            GL.End();

            

        }

        #endregion
    }
}
