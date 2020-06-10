using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Rendering.Renderers;
using OpenTK.Input;
using HSDRawViewer.Converters.Animation;

namespace HSDRawViewer.GUI.Controls
{
    public partial class GraphEditor : UserControl
    {
        public enum AnimType
        {
            Joint,
            Texture,
            Material,
            Light
        }

        public class GraphDisplayOptions
        {
            [DisplayName("Show All Tracks")]
            public bool ShowAllTracks { get; set; } = true;
        }

        private List<FOBJ_Player> _players = new List<FOBJ_Player>();

        private GraphDisplayOptions _options = new GraphDisplayOptions();
        
        private int _selectedPlayerIndex = 0;

        private int _startSelectionFrame = 0;

        private int _frame { get; set; } = 0;

        private AnimType _animType = AnimType.Joint;

        private int _frameCount
        {
            get
            {
                if (_selectedPlayer == null)
                    return 0;

                return _selectedPlayer.FrameCount;
            }
        }

        private FOBJ_Player _selectedPlayer
        {
            get
            {
                if (_selectedPlayerIndex < 0 || _selectedPlayerIndex > _players.Count)
                    return null;

                return _players[_selectedPlayerIndex];
            }
        }

        private List<FOBJKey> Keys
        {
            get
            {
                if (_selectedPlayer == null)
                    return null;

                return _selectedPlayer.Keys;
            }
        }

        public bool IsControl
        {
            get
            {
                var state = Keyboard.GetState();
                return state.IsKeyDown(Key.ControlLeft) || state.IsKeyDown(Key.ControlRight);
            }
        }

        private DrawingPanel _graph;

        public static Brush BackgroundColor = new SolidBrush(Color.FromArgb(255, 40, 40, 40));
        private static Pen FrameIndicatorPen = new Pen(Color.White);
        private static Brush SelectionColor = new SolidBrush(Color.FromArgb(180, 128, 128, 200));

        private static Pen LineColor = new Pen(Color.Gray);
        private static Pen SelectedLineColor = new Pen(Color.White);

        /// <summary>
        /// 
        /// </summary>
        public GraphEditor()
        {
            InitializeComponent();

            _graph = new DrawingPanel();
            _graph.Dock = DockStyle.Fill;
            graphBox.Controls.Add(_graph);
            _graph.BringToFront();
            
            _graph.MouseUp += (sender, args) =>
            {
                SelectFrameFromMouse(args.X);

                if (IsControl)
                    SelectKeysInRange(_startSelectionFrame, _frame);

                _graph.Invalidate();
            };

            _graph.MouseDown += (sender, args) =>
            {
                SelectFrameFromMouse(args.X);

                _startSelectionFrame = _frame;

                _graph.Invalidate();
            };

            _graph.MouseMove += (sender, args) =>
            {
                if(args.Button == MouseButtons.Left)
                {
                    SelectFrameFromMouse(args.X);
                }
            };

            _graph.Paint += (sender, args) =>
            {
                args.Graphics.FillRectangle(BackgroundColor, _graph.ClientRectangle);

                foreach (var p in _players)
                {
                    if (!_options.ShowAllTracks && p != _selectedPlayer)
                        continue;

                    var gr = new GraphRenderer();

                    if(p == _selectedPlayer)
                        gr.LineColor = SelectedLineColor;
                    else
                    {
                        gr.RenderPoints = false;
                        gr.LineColor = LineColor;
                    }

                    gr.SetKeys(p);

                    if (IsControl)
                        gr.Draw(args.Graphics, _graph.ClientRectangle, Math.Min(_frame, _startSelectionFrame), Math.Max(_frame, _startSelectionFrame) );
                    else
                        gr.Draw(args.Graphics, _graph.ClientRectangle, _frame, _frame);
                }

                var linex = (_frame / (float)_frameCount) * graphBox.Width;
                if(!float.IsNaN(linex) && !float.IsInfinity(linex))
                {
                    if (IsControl)
                    {
                        var start = (_startSelectionFrame / (float)_frameCount) * graphBox.Width;

                        args.Graphics.FillRectangle(SelectionColor, Math.Min(start, linex), 0, Math.Abs(start - linex), _graph.Height);

                        args.Graphics.DrawLine(FrameIndicatorPen, start, 0, start, _graph.Height);
                    }

                    args.Graphics.DrawLine(FrameIndicatorPen, linex, 0, linex, _graph.Height);
                }

            };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HSD_FOBJDesc ToFOBJs()
        {
            HSD_FOBJDesc fobj = null;
            foreach(var p in _players)
            {
                HSD_FOBJDesc f = new HSD_FOBJDesc();
                f.SetKeys(p.Keys, p.TrackType);

                if (fobj == null)
                    fobj = f;
                else
                    fobj.Add(f);
            }
            return fobj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aobj"></param>
        public void LoadTracks(AnimType type, HSD_AOBJ aobj)
        {
            _animType = type;

            trackTypeBox.Items.Clear();
            var tt = typeof(JointTrackType);
            switch (_animType)
            {
                case AnimType.Material:
                    tt = typeof(MatTrackType);
                    break;
                case AnimType.Texture:
                    tt = typeof(TexTrackType);
                    break;
                case AnimType.Light:
                default:
                    tt = typeof(JointTrackType);
                    break;
            }
            foreach (var item in Enum.GetValues(tt))
                trackTypeBox.Items.Add(item);

            List<FOBJ_Player> players = new List<FOBJ_Player>();
            
            if (aobj.FObjDesc != null)
                foreach (var v in aobj.FObjDesc.List)
                {
                    AddPlayer(new FOBJ_Player(v.TrackType, v.GetDecodedKeys()));
                }

            if(trackTree.Nodes.Count > 0)
                trackTree.SelectedNode = trackTree.Nodes[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetTrackName(byte type)
        {
            switch (_animType)
            {
                case AnimType.Joint:
                    return ((JointTrackType)type).ToString();
                case AnimType.Material:
                    return ((MatTrackType)type).ToString();
                case AnimType.Texture:
                    return ((TexTrackType)type).ToString();
                case AnimType.Light:
                default:
                    return "Type_" + type;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SelectFrameFromMouse(int mouseX)
        {
            _frame = (int)(mouseX / (float)_graph.Width * _frameCount);

            if(_selectedPlayer != null)
            {
                keyProperty.SelectedObject = _selectedPlayer.Keys.Find(e => e.Frame == _frame);
            }

            _graph.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SelectKeysInRange(int start, int end)
        {
            if (_selectedPlayer != null)
                keyProperty.SelectedObjects = _selectedPlayer.Keys.FindAll(e => e.Frame >= Math.Min(start, end) && e.Frame <= Math.Max(start, end)).ToArray();

            _graph.Invalidate();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteKeyButton_Click(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
                _selectedPlayer.Keys.RemoveAll(k => keyProperty.SelectedObjects.Contains(k));

            keyProperty.SelectedObject = null;

            _graph.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void keyProperty_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // sort keys
            if (e.ChangedItem.Label.Equals("Frame") && _selectedPlayer != null)
                _selectedPlayer.Keys = _selectedPlayer.Keys.OrderBy(k => k.Frame).ToList();

            _graph.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addKeyButton_Click(object sender, EventArgs e)
        {
            if(_selectedPlayer != null)
            {
                if(!_selectedPlayer.Keys.Exists(k=>k.Frame == _frame))
                {
                    var key = new FOBJKey() { Frame = _frame, Value = _selectedPlayer.GetValue(_frame), InterpolationType = GXInterpolationType.HSD_A_OP_LIN };

                    var insertIndex = _selectedPlayer.Keys.FindIndex(k => k.Frame >= _frame);

                    if(insertIndex == -1 || insertIndex >= _selectedPlayer.Keys.Count)
                        _selectedPlayer.Keys.Add(key);
                    else
                        _selectedPlayer.Keys.Insert(insertIndex, key);

                    keyProperty.SelectedObject = key;
                }

                _graph.Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _selectedPlayerIndex = _players.IndexOf(e.Node.Tag as FOBJ_Player);
            trackTypeBox.SelectedIndex = (e.Node.Tag as FOBJ_Player).TrackType;
            _graph.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void optionsButton_Click(object sender, EventArgs e)
        {
            using (PropertyDialog d = new PropertyDialog("Graph Display Options", _options))
                d.ShowDialog();

            _graph.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
            {
                _selectedPlayer.TrackType = (byte)trackTypeBox.SelectedIndex;
                trackTree.Nodes[_selectedPlayerIndex].Text = GetTrackName(_selectedPlayer.TrackType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeTrackButton_Click(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
            {
                _players.Remove(_selectedPlayer);
                trackTree.Nodes.RemoveAt(_selectedPlayerIndex);
            }

            if(trackTree.Nodes.Count > 0)
                trackTree.SelectedNode = trackTree.Nodes[0];
            else
                trackTree.SelectedNode = null;

            _graph.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addTrackButton_Click(object sender, EventArgs e)
        {
            var player = new FOBJ_Player() { Keys = new List<FOBJKey>() };

            player.Keys.Add(new FOBJKey() { Frame = 0, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });
            player.Keys.Add(new FOBJKey() { Frame = _frameCount == 0 ? 10 : _frameCount, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });

            AddPlayer(player);

            trackTree.SelectedNode = trackTree.Nodes[trackTree.Nodes.Count - 1];

            _graph.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        private void AddPlayer(FOBJ_Player player)
        {
            _players.Add(player);
            trackTree.Nodes.Add(new TreeNode() { Text = GetTrackName(player.TrackType), Tag = player });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importKeyButton_Click(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
            {
                _selectedPlayer.Keys = HSDK.LoadKeys();
                _graph.Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportKeyButton_Click(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
            {
                HSDK.ExportKeys(_selectedPlayer.Keys);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DrawingPanel : Panel
    {
        public DrawingPanel()
        {
            DoubleBuffered = true;

            Resize += (sender, args) =>
            {
                Invalidate();
            };
        }
    }
}
