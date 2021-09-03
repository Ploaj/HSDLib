using HSDRaw;
using HSDRaw.AirRide.Gr;
using HSDRaw.AirRide.Gr.Data;
using HSDRaw.Common;
using HSDRaw.GX;
using HSDRaw.Tools.KAR;
using HSDRawViewer.Converters.AirRide;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.GX;
using IONET;
using IONET.Core;
using IONET.Core.Model;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins.AirRide
{
    [SupportedTypes(new Type[] { typeof(KAR_grData) })]
    public partial class AirRideGrDataEditor : DockContent, EditorBase, IDrawableInterface
    {
        public enum EditorMode
        {
            Collision,
            Zones,
            CourseSpline,
            RangeSpline,
            RailSpline1,
            RailSpline2,
            ConveyorSpline,
            Buckets,
            StartPosition,
            EnemyPosition,
            GravityPosition,
            AirFlowPosition,
            ConveyorPosition,
            ItemPosition,
            EventPosition,
            VehiclePosition,
            GlobalDeadPosition,
            LocalDeadPosition,
            YakumonoPosition,
            ItemAreaPosition,
            VehicleAreaPosition,
        }

        public DockState DefaultDockState => DockState.Document;

        public EditorMode EditMode
        {
            get => _editMode;
            set => SwitchMode(value);
        }
        private EditorMode _editMode = EditorMode.Collision;

        public DataNode Node { get => _node;
            set
            {
                _node = value;
                if (_node.Accessor is KAR_grData data)
                    LoadData(data);
            }
        }
        private DataNode _node;
        private KAR_grData _data;

        public DrawOrder DrawOrder => DrawOrder.Last;

        private ViewportControl _viewport;

        public HSD_Spline[] _splines { get; internal set; } = new HSD_Spline[0];
        public KAR_grRangeSpline[] _rangeSplines { get; internal set; } = new KAR_grRangeSpline[0];
        public HSD_Spline[] _conveyorSplines { get; internal set; } = new HSD_Spline[0];
        public HSD_Spline[] _railSplines1 { get; internal set; } = new HSD_Spline[0];
        public HSD_Spline[] _railSplines2 { get; internal set; } = new HSD_Spline[0];

        private AirRideGrDataPosition _startPos;
        private AirRideGrDataPosition _enemyPos;
        private AirRideGrDataPosition _gravityPos;
        private AirRideGrDataPosition _airFlowPos;
        private AirRideGrDataPosition _conveyorPos;
        private AirRideGrDataPosition _itemPos;
        private AirRideGrDataPosition _eventPos;
        private AirRideGrDataPosition _vehiclePos;
        private AirRideGrDataPosition _globalDeadPos;
        private AirRideGrDataPosition _localDeadPos;
        private AirRideGrDataPosition _yakumonoPos;
        private AirRideGrDataPosition _itemAreaPos;
        private AirRideGrDataPosition _vehicleAreaPos;

        private KAR_grPartitionBucket[] _buckets;
        private KAR_grPartitionBucket _selectedBucket;
        private HashSet<int> _selectedTriangles = new HashSet<int>();

        private KAR_CollisionJoint[] _joints;
        public KAR_CollisionTriangle[] _tris { get; internal set; }
        private GXVector3[] _vertices;

        private KAR_ZoneCollisionJoint[] _zoneJoints { get; set; }
        public KAR_ZoneCollisionTriangle[] _zoneTris { get; internal set; }
        private GXVector3[] _zoneVertices;

        private Dictionary<EditorMode, Action> editor_renders;

        /// <summary>
        /// 
        /// </summary>
        public AirRideGrDataEditor()
        {
            InitializeComponent();


            editor_renders = new Dictionary<EditorMode, Action>()
            {
                { EditorMode.Buckets, () => RenderBuckets()},
                { EditorMode.Zones, () => RenderZones()},
                { EditorMode.CourseSpline, () => RenderSplines(_splines)},
                { EditorMode.RangeSpline, () => RenderRangeSplines()},
                { EditorMode.RailSpline1, () => RenderSplines(_railSplines1)},
                { EditorMode.RailSpline2, () => RenderSplines(_railSplines2)},
                { EditorMode.ConveyorSpline, () => RenderSplines(_conveyorSplines)},
                { EditorMode.StartPosition, () => RenderPosition(_startPos)},
                { EditorMode.EnemyPosition, () => RenderPosition(_enemyPos)},
                { EditorMode.GravityPosition, () => RenderPosition(_gravityPos)},
                { EditorMode.AirFlowPosition, () => RenderPosition(_airFlowPos)},
                { EditorMode.ConveyorPosition, () => RenderPosition(_conveyorPos)},
                { EditorMode.ItemPosition, () => RenderPosition(_itemPos)},
                { EditorMode.EventPosition, () => RenderPosition(_eventPos)},
                { EditorMode.VehiclePosition, () => RenderPosition(_vehiclePos)},
                { EditorMode.GlobalDeadPosition, () => RenderPosition(_globalDeadPos)},
                { EditorMode.LocalDeadPosition, () => RenderPosition(_localDeadPos)},
                { EditorMode.ItemAreaPosition, () => RenderPosition(_itemAreaPos)},
                { EditorMode.VehicleAreaPosition, () => RenderPosition(_vehicleAreaPos)},
            };


            // initialize viewport
            _viewport = new ViewportControl();
            _viewport.Dock = DockStyle.Fill;
            _viewport.AddRenderer(this);
            tabControl1.TabPages[0].Controls.Add(_viewport);
            _viewport.BringToFront();

            // set mode selection
            modeComboBox.ComboBox.DataSource = Enum.GetValues(typeof(EditorMode));
            modeComboBox.SelectedIndex = 0;

            // save on close
            FormClosing += (sender, args) =>
            {
                Save();
            };

            // dispose of viewport on close
            Disposed += (sender, args) =>
            {
                _viewport.Dispose();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnFileSave()
        {
            Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        public void SwitchMode(EditorMode mode)
        {
            _editMode = mode;

            arrayMemberEditor1.SetArrayFromProperty(null, null);

            switch (_editMode)
            {
                case EditorMode.Collision:
                    break;
                case EditorMode.Zones:
                    if (_zoneJoints != null)
                        arrayMemberEditor1.SetArrayFromProperty(this, "_zoneJoints");
                    break;
                case EditorMode.CourseSpline:
                    if (_splines != null)
                        arrayMemberEditor1.SetArrayFromProperty(this, "_splines");
                    break;
                case EditorMode.RangeSpline:
                    if (_rangeSplines != null)
                        arrayMemberEditor1.SetArrayFromProperty(this, "_rangeSplines");
                    break;
                case EditorMode.RailSpline1:
                    if (_railSplines1 != null)
                        arrayMemberEditor1.SetArrayFromProperty(this, "_railSplines1");
                    break;
                case EditorMode.RailSpline2:
                    if (_railSplines2 != null)
                        arrayMemberEditor1.SetArrayFromProperty(this, "_railSplines2");
                    break;
                case EditorMode.ConveyorSpline:
                    if (_conveyorSplines != null)
                        arrayMemberEditor1.SetArrayFromProperty(this, "_unkSplines");
                    break;
                case EditorMode.StartPosition: SelectPosition(_startPos); break;
                case EditorMode.EnemyPosition: SelectPosition(_enemyPos); break;
                case EditorMode.GravityPosition: SelectPosition(_gravityPos); break;
                case EditorMode.AirFlowPosition: SelectPosition(_airFlowPos); break;
                case EditorMode.ConveyorPosition: SelectPosition(_conveyorPos); break;
                case EditorMode.ItemPosition: SelectPosition(_itemPos); break;
                case EditorMode.EventPosition: SelectPosition(_eventPos); break;
                case EditorMode.VehiclePosition: SelectPosition(_vehiclePos); break;
                case EditorMode.GlobalDeadPosition: SelectPosition(_globalDeadPos); break;
                case EditorMode.LocalDeadPosition: SelectPosition(_localDeadPos); break;
                case EditorMode.ItemAreaPosition: SelectPosition(_itemAreaPos); break;
                case EditorMode.VehicleAreaPosition: SelectPosition(_vehicleAreaPos); break;
                case EditorMode.Buckets:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        private void SelectPosition(AirRideGrDataPosition pos)
        {
            if (pos != null && pos._positions != null)
                arrayMemberEditor1.SetArrayFromProperty(pos, "_positions");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void LoadData(KAR_grData data)
        {
            _data = data;
            //collisionManager.LoadCollision(data.CollisionNode);

            LoadCollisionData(data.CollisionNode);
            LoadPartitionData(data.PartitionNode.Partition);

            if(data.PositionNode != null)
            {
                var _positionJoint = data.PositionNode.PositionJoint;
                _startPos = new AirRideGrDataPosition(_positionJoint, data.PositionNode.Startpos);
                _enemyPos = new AirRideGrDataPosition(_positionJoint, data.PositionNode.Enemypos);
                _gravityPos = new AirRideGrDataPosition(_positionJoint, data.PositionNode.Gravitypos);
                _airFlowPos = new AirRideGrDataPosition(_positionJoint, data.PositionNode.Airflowpos);
                _conveyorPos = new AirRideGrDataPosition(_positionJoint, data.PositionNode.Conveyorpos);
                _itemPos = new AirRideGrDataPosition(_positionJoint, data.PositionNode.ItemPos);
                _eventPos = new AirRideGrDataPosition(_positionJoint, data.PositionNode.Eventpos);
                _vehiclePos = new AirRideGrDataPosition(_positionJoint, data.PositionNode.Vehiclepos);
                _globalDeadPos = new AirRideGrDataPosition(_positionJoint, data.PositionNode.GlobalDeadPos);
                _localDeadPos = new AirRideGrDataPosition(_positionJoint, data.PositionNode.LocalDeadPos);
                _yakumonoPos = new AirRideGrDataPosition(_positionJoint, data.PositionNode.Yakumonopos);
                _itemAreaPos = new AirRideGrDataPosition(_positionJoint, data.PositionNode.ItemAreaPos);
                _vehicleAreaPos = new AirRideGrDataPosition(_positionJoint, data.PositionNode.VehicleAreapos);
            }

            if (data.SplineNode != null)
            {
                if (data.SplineNode.SplineSetup.CourseSplineList != null)
                    _splines = data.SplineNode.SplineSetup.CourseSplineList.Splines;

                if (data.SplineNode.RangeSplineSetup != null)
                    _rangeSplines = data.SplineNode.RangeSplineSetup.Splines;

                if (data.SplineNode.ConveyorSpline != null)
                    _conveyorSplines = data.SplineNode.ConveyorSpline.SplineList.Splines;

                if (data.SplineNode.RailSpline1 != null)
                    _railSplines1 = data.SplineNode.RailSpline1.Splines;

                if (data.SplineNode.RailSpline2 != null)
                    _railSplines2 = data.SplineNode.RailSpline2.Splines;
            }

            SelectBucket(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void LoadCollisionData(KAR_grCollisionNode node)
        {
            if (node == null)
                return;

            _joints = node.Joints;
            _tris = node.Triangles;
            _vertices = node.Vertices;

            _zoneJoints = node.ZoneJoints;
            _zoneTris = node.ZoneTriangles;
            _zoneVertices = node.ZoneVertices;

            if (_editMode == EditorMode.Collision)
                EditMode = _editMode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void LoadPartitionData(KAR_grCollisionTree node)
        {
            if (node == null)
                return;

            _buckets = node.Buckets;

            if (_editMode == EditorMode.Collision)
                EditMode = _editMode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketIndex"></param>
        private void SelectBucket(int bucketIndex)
        {
            _selectedBucket = _buckets[bucketIndex];

            _selectedTriangles.Clear();
            for (int i = _selectedBucket.CollTriangleStart; i < _selectedBucket.CollTriangleStart + _selectedBucket.CollTriangleCount; i++)
                _selectedTriangles.Add(_data.PartitionNode.Partition.CollidableTriangles[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            if (_data == null)
                return;

            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            if (renderCollisionsToolStripMenuItem.Checked)
                RenderCollisions(_editMode == EditorMode.Collision ? 1f : 0.25f, _selectedTriangles);

            GL.Disable(EnableCap.DepthTest);

            if (editor_renders.ContainsKey(_editMode))
                editor_renders[_editMode]();

            DrawShape.DrawBox(Color.Blue, selectedPoint.X, selectedPoint.Y, selectedPoint.Z, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderCollisions(float alpha, HashSet<int> visible = null)
        {
            GL.PushAttrib(AttribMask.AllAttribBits);

            GL.Enable(EnableCap.CullFace);

            // render collisions
            GL.Begin(PrimitiveType.Triangles);
            foreach (var j in _joints)
            {
                for (int i = j.FaceStart; i < j.FaceStart + j.FaceSize; i++)
                {
                    var t = _tris[i];
                    GL.Color4(0.5f, 0.5f, 0.5f, alpha);

                    if ((t.Flags & 0x01) != 0)
                        GL.Color4(1f, 0f, 0f, alpha);
                    if ((t.Flags & 0x02) != 0)
                        GL.Color4(0f, 1f, 0f, alpha);
                    if ((t.Flags & 0x04) != 0)
                        GL.Color4(0f, 0f, 1f, alpha);

                    if (visible != null && visible.Contains(i))
                        GL.Color3(Color.Yellow);

                    GL.Vertex3(GXTranslator.toVector3(_vertices[t.V3]));
                    GL.Vertex3(GXTranslator.toVector3(_vertices[t.V2]));
                    GL.Vertex3(GXTranslator.toVector3(_vertices[t.V1]));
                }
            }
            GL.End();

            GL.Begin(PrimitiveType.Lines);
            foreach (var j in _joints)
            {
                for (int i = j.FaceStart; i < j.FaceStart + j.FaceSize; i++)
                {
                    GL.Color3(Color.White);

                    var t = _tris[i];
                    GL.Vertex3(GXTranslator.toVector3(_vertices[t.V1]));
                    GL.Vertex3(GXTranslator.toVector3(_vertices[t.V2]));

                    GL.Vertex3(GXTranslator.toVector3(_vertices[t.V2]));
                    GL.Vertex3(GXTranslator.toVector3(_vertices[t.V3]));

                    GL.Vertex3(GXTranslator.toVector3(_vertices[t.V3]));
                    GL.Vertex3(GXTranslator.toVector3(_vertices[t.V1]));
                }
            }
            GL.End();

            GL.PopAttrib();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderZones()
        {
            if (_zoneJoints == null)
                return;

            // render collisions
            GL.Begin(PrimitiveType.Triangles);
            foreach (var j in _zoneJoints)
            {
                for (int i = j.ZoneFaceStart; i < j.ZoneFaceStart + j.ZoneFaceSize; i++)
                {
                    var t = _zoneTris[i];
                    GL.Color3(1f, 1f, 1f);

                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V1]));
                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V2]));
                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V3]));
                }
            }
            GL.End();

            GL.Begin(PrimitiveType.Lines);
            foreach (var j in _zoneJoints)
            {
                for (int i = j.ZoneFaceStart; i < j.ZoneFaceStart + j.ZoneFaceSize; i++)
                {
                    GL.Color3(Color.Black);

                    var t = _zoneTris[i];
                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V1]));
                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V2]));

                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V2]));
                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V3]));

                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V3]));
                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V1]));
                }
            }
            GL.End();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderBuckets()
        {
            var selected_bucket = arrayMemberEditor1.SelectedObject as KAR_grPartitionBucket;

            // render buckets
            foreach (var b in _buckets)
            {
                if (b != null)
                    DrawShape.DrawBox(selected_bucket == b ? Color.Yellow : Color.White, b.MinX, b.MinY, b.MinZ, b.MaxX, b.MaxY, b.MaxZ);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderSplines(HSD_Spline[] splines)
        {
            var selected_spline = arrayMemberEditor1.SelectedObject as HSD_Spline;

            if (splines != null)
                foreach (var s in splines)
                {
                    if (selected_spline == s)
                        DrawShape.RenderSpline(s, Color.Yellow, Color.Red);
                    else
                        DrawShape.RenderSpline(s, Color.Green, Color.Blue);
                }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderRangeSplines()
        {
            var selected_spline = arrayMemberEditor1.SelectedObject as KAR_grRangeSpline;

            if (_rangeSplines != null)
                foreach (var s in _rangeSplines)
                {
                    if (selected_spline == s)
                    {
                        DrawShape.RenderSpline(s.LeftSpline, Color.Yellow, Color.Red);
                        DrawShape.RenderSpline(s.RightSpline, Color.Yellow, Color.Red);
                    }
                    else
                    {
                        DrawShape.RenderSpline(s.LeftSpline, Color.Green, Color.Blue);
                        DrawShape.RenderSpline(s.RightSpline, Color.Green, Color.Blue);
                    }
                }
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="positions"></param>
        private void RenderPosition (AirRideGrDataPosition list)
        {
            if (list == null)
                return;

            var selected_pos = arrayMemberEditor1.SelectedObject as AirRideGrDataPositionProxy;

            foreach (var p in list._positions)
            {
                DrawShape.DrawBox(p == selected_pos ? Color.Yellow : Color.Red, p.X, p.Y, p.Z, 1f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(toolStripTextBox1.Text, out int i) && i < _buckets.Length)
                SelectBucket(i);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void modeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Enum.TryParse(modeComboBox.ComboBox.SelectedValue.ToString(), out EditorMode mode))
                EditMode = mode;
        }




        public void ViewportKeyPress(KeyboardState kbState)
        {
        }

        public void ScreenClick(MouseButtons button, PickInformation pick)
        {
        }

        private OpenTK.Vector3 selectedPoint = OpenTK.Vector3.Zero;

        public void ScreenDoubleClick(PickInformation pick)
        {
            if (_editMode == EditorMode.Collision)
            {
                float near_dis = float.MaxValue;

                int selected_index = -1;
                int index = 0;
                foreach (var t in _tris)
                {
                    OpenTK.Vector3 hit = OpenTK.Vector3.Zero;
                    if (pick.CheckTriangleHit2(
                        GXTranslator.toVector3(_vertices[t.V1]), 
                        GXTranslator.toVector3(_vertices[t.V2]), 
                        GXTranslator.toVector3(_vertices[t.V3]),
                        ref hit,
                        out float depth))
                    {
                        if (depth < near_dis)
                        {
                            near_dis = depth;
                            selected_index = index;
                            selectedPoint = hit;
                        }
                    }
                    index++;
                }

                _selectedTriangles.Clear();
                if (selected_index != -1)
                {
                    Console.WriteLine(_tris[selected_index].Flags.ToString("X"));
                    _selectedTriangles.Add(selected_index);
                }
            }

            if (_editMode == EditorMode.CourseSpline)
            {
                Vector3 o = new Vector3();
                foreach (var s in _splines)
                {
                    foreach (var p in s.Points)
                    {
                        if (pick.CheckAABBHit(new OpenTK.Vector3(p.X, p.Y, p.Z), 2, ref o))
                        {
                            Console.WriteLine(p.X + " " + p.Y + " " + p.Z);
                        }
                    }
                }
            }
        }


        public void ScreenDrag(PickInformation pick, float deltaX, float deltaY)
        {
        }

        public void ScreenSelectArea(PickInformation start, PickInformation end)
        {
        }

        private void importFromKCLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("KCL (*.kcl)|*.kcl");

            if (f != null)
            {
                var node = KCLConv.KCLtoKAR(f, out KAR_grCollisionTree tree);

                node.CalculateCollisionFlags();

                LoadCollisionData(node);
                LoadPartitionData(tree);
            }
        }

        private void importFromKMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("KMP (*.kmp)|*.kmp");

            if (f != null)
            {
                var spline = KCLConv.KMP_ExtractRouteSpline(f);

                _splines = new HSD_Spline[] { spline };
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Save();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            // save collition data
            _data.CollisionNode = GenerateCollisionNode();
            _data.PartitionNode = new KAR_grCollisionTreeNode()
            {
                Partition = BucketGen.GenerateBucketPartition(_data.CollisionNode)
            };

            // save splines
            var splineNode = _data.SplineNode;

            // course spline
            splineNode.SplineSetup.CourseSplineList = new KAR_grSplineList()
            {
                Splines = _splines,
                Count = _splines.Length
            };

            // range spline
            splineNode.SplineSetup.CourseSplineList.Splines = _splines;
            if (_rangeSplines != null && _rangeSplines.Length > 0)
                splineNode.RangeSplineSetup = new KAR_grRangeSplineSetup()
                {
                    Splines = _rangeSplines
                };
            else
                splineNode.RangeSplineSetup = null;

            // conveyor spline
            if (splineNode.ConveyorSpline != null && _conveyorSplines != null && _conveyorSplines.Length > 0)
            {
                splineNode.ConveyorSpline = new KAR_grConveyorPath();
                splineNode.ConveyorSpline.SplineList = new KAR_grSplineList()
                {
                    Splines = _conveyorSplines,
                    Count = _conveyorSplines.Length
                };
            }
            else
                splineNode.ConveyorSpline = null;

            // rail1
            if (_railSplines1 != null && _railSplines1.Length > 0)
            {
                splineNode.RailSpline1 = new KAR_grSplineList()
                {
                    Splines = _railSplines1,
                    Count = _railSplines1.Length
                };
            }
            else
                splineNode.RailSpline1 = null;

            // rail2
            if (_railSplines2 != null && _railSplines2.Length > 0)
            {
                splineNode.RailSpline2 = new KAR_grSplineList()
                {
                    Splines = _railSplines2,
                    Count = _railSplines2.Length
                };
            }
            else
                splineNode.RailSpline2 = null;


            // TODO: generate position list
            var positionJoint = new HSD_JOBJ();

            _data.PositionNode.Startpos = _startPos?.ToKarPositionList(positionJoint);
            _data.PositionNode.Enemypos = _enemyPos?.ToKarPositionList(positionJoint);
            _data.PositionNode.Gravitypos = _gravityPos?.ToKarPositionList(positionJoint);
            _data.PositionNode.Airflowpos = _airFlowPos?.ToKarPositionList(positionJoint);
            _data.PositionNode.Conveyorpos = _conveyorPos?.ToKarPositionList(positionJoint);
            _data.PositionNode.ItemPos = _itemPos?.ToKarPositionList(positionJoint);
            _data.PositionNode.Eventpos = _eventPos?.ToKarPositionList(positionJoint);
            _data.PositionNode.Vehiclepos = _vehiclePos?.ToKarPositionList(positionJoint);
            _data.PositionNode.GlobalDeadPos = _globalDeadPos?.ToKarPositionList(positionJoint);
            _data.PositionNode.Yakumonopos = _yakumonoPos?.ToKarPositionList(positionJoint);
            _data.PositionNode.ItemAreaPos = _itemAreaPos?.ToKarAreaPositionList(positionJoint);
            _data.PositionNode.VehicleAreapos = _vehicleAreaPos?.ToKarAreaPositionList(positionJoint);

            _data.PositionNode.PositionJoint = positionJoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public KAR_grCollisionNode GenerateCollisionNode()
        {
            return new KAR_grCollisionNode()
            {
                Joints = _joints,
                Triangles = _tris,
                Vertices = _vertices,
                ZoneJoints = _zoneJoints,
                ZoneTriangles = _zoneTris,
                ZoneVertices = _zoneVertices
            };
        }

        private void generateRangeSplinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _rangeSplines = new KAR_grRangeSpline[_splines.Length];

            for (int i = 0; i < _rangeSplines.Length; i++)
            {
                _rangeSplines[i] = new KAR_grRangeSpline();
                _rangeSplines[i].x08 = -1;
                _rangeSplines[i].x0C = -1;
                _rangeSplines[i].x10 = -1;
                KAR_SplineTools.CreateRangeSpline(_splines[i], out HSD_Spline left, out HSD_Spline right);
                _rangeSplines[i].LeftSpline = left;
                _rangeSplines[i].RightSpline = right;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importFromOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filePath = Tools.FileIO.OpenFile(IOManager.GetModelImportFileFilter());

            if (filePath == null)
                return;

            var scene = IOManager.LoadScene(filePath, new ImportSettings()
            {
                SmoothNormals = true
            });

            ImportCollisionFromScene(scene);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        private void ImportCollisionFromScene(IOScene scene)
        {
            List<GXVector3> points = new List<GXVector3>();

            List<KAR_CollisionTriangle> triangles = new List<KAR_CollisionTriangle>();

            // calculate mesh bounding
            int meshIndex = 0;
            foreach (var m in scene.Models[0].Meshes)
            {
                // make sure the mesh is in triangle form
                m.MakeTriangles();

                int pointStart = points.Count;

                // add vertices
                foreach (var v in m.Vertices)
                    points.Add(new GXVector3() { X = v.Position.X, Y = v.Position.Y, Z = v.Position.Z });

                // add triangles
                foreach (var p in m.Polygons)
                {
                    for (int i = 0; i < p.Indicies.Count; i += 3)
                    {
                        triangles.Add(new KAR_CollisionTriangle()
                        {
                            V1 = p.Indicies[i + 0] + pointStart,
                            V2 = p.Indicies[i + 1] + pointStart,
                            V3 = p.Indicies[i + 2] + pointStart,
                            Flags = 0x80
                        });
                    }
                }

                meshIndex++;
            }

            // generate a collision node
            KAR_grCollisionNode collision = new KAR_grCollisionNode()
            {
                Vertices = points.ToArray(),
                Triangles = triangles.ToArray(),
                Joints = new KAR_CollisionJoint[]{
                    new KAR_CollisionJoint()
                    {
                        BoneID = 0,
                        FaceStart = 0,
                        FaceSize = triangles.Count,
                        VertexStart = 0,
                        VertexSize = points.Count
                    }
                }
            };

            // calculate the collision flags
            collision.CalculateCollisionFlags();

            // load the collision data
            LoadCollisionData(collision);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.SaveFile("Waveform OBJ (*.obj)|*.obj");

            if (f == null)
                return;

            ExportCollisionModel(f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        private void ExportCollisionModel(string f)
        {
            IOScene scene = new IOScene();

            IOModel model = new IOModel();
            scene.Models.Add(model);

            foreach (var j in _joints)
            {
                var mesh = new IOMesh();
                mesh.Name = $"Joint_{j.BoneID}";
                model.Meshes.Add(mesh);

                var poly = new IOPolygon()
                {
                    PrimitiveType = IOPrimitive.TRIANGLE,
                    Indicies = new List<int>()
                };
                mesh.Polygons.Add(poly);

                for (int i = j.FaceStart; i < j.FaceStart + j.FaceSize; i++)
                {
                    var tri = _tris[i];

                    var v1 = _vertices[tri.V1];
                    var v2 = _vertices[tri.V2];
                    var v3 = _vertices[tri.V3];

                    mesh.Vertices.Add(new IOVertex()
                    {
                        Position = new System.Numerics.Vector3(v1.X, v1.Y, v1.Z)
                    });
                    mesh.Vertices.Add(new IOVertex()
                    {
                        Position = new System.Numerics.Vector3(v2.X, v2.Y, v2.Z)
                    });
                    mesh.Vertices.Add(new IOVertex()
                    {
                        Position = new System.Numerics.Vector3(v3.X, v3.Y, v3.Z)
                    });

                    poly.Indicies.Add(poly.Indicies.Count);
                    poly.Indicies.Add(poly.Indicies.Count);
                    poly.Indicies.Add(poly.Indicies.Count);
                }

                mesh.Optimize();
            }

            // TODO: zones

            IOManager.ExportScene(scene, f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recalculateCollisionFlagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var coll = GenerateCollisionNode();
            coll.CalculateCollisionFlags();
            LoadCollisionData(coll);
        }
    }

}
