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
        public class KeyProxy
        {
            public FOBJKey key;
            public FOBJKey slopekey;

            public float Frame
            {
                get => key.Frame;
                set
                {
                    key.Frame = value;
                    if(slopekey != null)
                        slopekey.Frame = value;
                }
            }

            public float Value { get => key.Value; set => key.Value = value; }

            public float InTangent { get => key.Tan; set => key.Tan = value; }

            public float OutTangent
            {
                get
                {
                    if (slopekey != null)
                        return slopekey.Tan;

                    return InTangent;
                }
                set
                {
                    if (slopekey != null)
                        slopekey.Tan = value;
                }
            }

            public bool DifferentTangents
            {
                get => slopekey != null;
                set
                {
                    if (value && slopekey == null)
                    {
                        slopekey = new FOBJKey()
                        {
                            Frame = key.Frame,
                            Tan = key.Tan,
                            InterpolationType = GXInterpolationType.HSD_A_OP_SLP
                        };
                    }
                    if (!value)
                    {
                        slopekey = null;
                    }
                }
            }

            public GXInterpolationType Interpolation { get => key.InterpolationType; set => key.InterpolationType = value; }
        }

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
            public bool ShowAllTracks { get; set; } = false;

            [DisplayName("Show Frame Ticks")]
            public bool ShowFrameTicks { get; set; } = true;

            [DisplayName("Show Tangents")]
            public bool ShowTangents { get; set; } = true;
        }

        public FOBJ_Player[] TrackPlayers { get => _players.ToArray(); }

        private List<FOBJ_Player> _players = new List<FOBJ_Player>();

        private static GraphDisplayOptions _options = new GraphDisplayOptions();
        
        private int _selectedPlayerIndex = 0;

        private int _startSelectionFrame = 0;

        private int _frame { get => (int)nudFrame.Value; set => nudFrame.Value = Math.Max(Math.Min(value, nudFrame.Maximum), 0); }

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
                if (_selectedPlayerIndex < 0 || _selectedPlayerIndex >= _players.Count)
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
        private static Brush SelectionColor = new SolidBrush(Color.FromArgb(90, 128, 128, 200));

        public static Pen TickColor = new Pen(Color.White);
        public static Pen BackTickColor = new Pen(Color.FromArgb(255, 50, 50, 50));

        private static Pen LineColor = new Pen(Color.Gray);
        private static Pen SelectedLineColor = new Pen(Color.White);

        public event EventHandler OnTrackListUpdate;

        public float Zoom = 0.9f;

        public static Brush FontColor = new SolidBrush(Color.White);
        private Font _markerFont;
        private StringFormat _markerFormat = new StringFormat();

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

            _markerFont = new Font("Arial", 8);
            _markerFormat.LineAlignment = StringAlignment.Center;
            _markerFormat.Alignment = StringAlignment.Center;

            Disposed += (sender, args) =>
            {
                _markerFont.Dispose();
            };

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
                var graphics = args.Graphics;

                // set smoothing
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // fill background color
                var rect = _graph.ClientRectangle;
                graphics.FillRectangle(BackgroundColor, rect);

                var x1 = rect.Width - rect.Width * Zoom;
                var y1 = rect.Height - rect.Height * Zoom;

                graphics.TranslateTransform(x1 / 2, y1 / 2);
                graphics.ScaleTransform(Zoom, Zoom);

                // draw frame ticks
                if (_options.ShowFrameTicks)
                {
                    if (_selectedPlayer != null)
                    {
                        var gr = new GraphRenderer();
                        gr.SetKeys(_selectedPlayer);

                        // horizontal lines
                        graphics.DrawLine(BackTickColor, -x1, 0, x1 * 2 + rect.Width, 0);
                        graphics.DrawString(gr.MinValue.ToString("n2"), _markerFont, FontColor, -20, 0, _markerFormat);

                        graphics.DrawLine(BackTickColor, -x1, rect.Height / 4f, x1 * 2 + rect.Width, rect.Height / 4f);

                        graphics.DrawLine(BackTickColor, -x1, rect.Height / 2f, x1 * 2 + rect.Width, rect.Height / 2f);
                        graphics.DrawString(((gr.MinValue + gr.MaxValue) / 2).ToString("n2"), _markerFont, FontColor, -20, rect.Height / 2f, _markerFormat);

                        graphics.DrawLine(BackTickColor, -x1, rect.Height * 3f / 4f, x1 * 2 + rect.Width, rect.Height * 3f / 4f);

                        graphics.DrawLine(BackTickColor, -x1, rect.Height, x1 * 2 + rect.Width, rect.Height);
                        graphics.DrawString(gr.MaxValue.ToString("n2"), _markerFont, FontColor, -20, rect.Height, _markerFormat);
                    }

                    var tickWidth = (rect.Width / (float)_frameCount);
                    var increment = 1;

                    // fix infinity
                    if (_frameCount == 0)
                        tickWidth = 1;

                    // don't crunch numbers too far together
                    if (tickWidth * Zoom < 20)
                        increment = (int)Math.Ceiling(20 / (tickWidth * Zoom));

                    // round increment to upper 5
                    if(increment != 1)
                        increment = (int)(Math.Ceiling(increment / 5f) * 5f);

                    // vertical
                    for (int i = 0; i <= _frameCount; i++)
                    {
                        if(i % increment == 0)
                        {
                            var x = i * tickWidth;

                            // frame line
                            graphics.DrawLine(BackTickColor, x, 0, x, rect.Height);

                            // tick number
                            graphics.DrawString(i.ToString(), _markerFont, FontColor, x, -12, _markerFormat);

                            // tick line
                            graphics.DrawLine(TickColor, x, 0, x, 5f);
                        }
                    }
                }

                // draw tracks
                foreach (var p in _players)
                {
                    if (!_options.ShowAllTracks && p != _selectedPlayer)
                        continue;

                    var gr = new GraphRenderer();

                    gr.RenderTangents = _options.ShowTangents;

                    if(p == _selectedPlayer)
                        gr.LineColor = SelectedLineColor;
                    else
                    {
                        gr.RenderPoints = false;
                        gr.LineColor = LineColor;
                    }

                    gr.SetKeys(p);

                    if (IsControl)
                        gr.Draw(args.Graphics, rect, Math.Min(_frame, _startSelectionFrame), Math.Max(_frame, _startSelectionFrame) );
                    else
                        gr.Draw(args.Graphics, rect, _frame, _frame);
                }


                // draw selection
                var linex = _frame * (rect.Width / (float)_frameCount);

                if (!float.IsNaN(linex) && !float.IsInfinity(linex))
                {
                    if (IsControl)
                    {
                        var start = _startSelectionFrame * (rect.Width / (float)_frameCount);

                        graphics.FillRectangle(SelectionColor, Math.Min(start, linex), 0, Math.Abs(start - linex), _graph.Height);

                        graphics.DrawLine(FrameIndicatorPen, start, 0, start, _graph.Height);
                    }

                    graphics.DrawLine(FrameIndicatorPen, linex, 0, linex, _graph.Height);
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
        public void ClearTracks()
        {
            _players.Clear();
            trackTree.Nodes.Clear();
            Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aobj"></param>
        public void LoadTracks(AnimType type, HSD_AOBJ aobj)
        {
            SetTrackType(type);
            
            if (aobj.FObjDesc != null)
                foreach (var v in aobj.FObjDesc.List)
                    AddPlayer(new FOBJ_Player(v.TrackType, v.GetDecodedKeys()));

            if(trackTree.Nodes.Count > 0)
                trackTree.SelectedNode = trackTree.Nodes[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="aobj"></param>
        public void LoadTracks(AnimType type, IEnumerable<FOBJ_Player> players)
        {
            SetTrackType(type);

            foreach (var p in players)
                AddPlayer(p);

            if (trackTree.Nodes.Count > 0)
                trackTree.SelectedNode = trackTree.Nodes[0];
        }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
        private void SetTrackType(AnimType type)
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
            var position = (mouseX - (_graph.Width - _graph.Width * Zoom) / 2) / (_graph.Width * Zoom);

            if (position < 0)
                _frame = 0;
            else if (position > 1)
                _frame = _frameCount;
            else
                _frame = (int)(position * _frameCount);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SelectKeyAtFrame()
        {
            keyProperty.SelectedObject = null;

            if (_selectedPlayer != null)
            {
                var keyIndex = _selectedPlayer.Keys.FindIndex(e => e.Frame == _frame && e.InterpolationType != GXInterpolationType.HSD_A_OP_SLP);
                if(keyIndex != -1)
                {
                    var key = _selectedPlayer.Keys[keyIndex];
                    var slope = keyIndex + 1 < Keys.Count && Keys[keyIndex + 1].InterpolationType == GXInterpolationType.HSD_A_OP_SLP ? Keys[keyIndex + 1] : null;

                    keyProperty.SelectedObject = new KeyProxy() { key = key, slopekey = slope };
                }
            }

            _graph.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SelectKeysInRange(int start, int end)
        {
            keyProperty.SelectedObject = null;

            if (_selectedPlayer != null)
            {
                List<object> sel = new List<object>(); 
                for(int i = Math.Min(start, end); i < Math.Max(start, end); i++)
                {
                    var keyIndex = _selectedPlayer.Keys.FindIndex(e => e.Frame == _frame && e.InterpolationType != GXInterpolationType.HSD_A_OP_SLP);
                    if (keyIndex != -1)
                    {
                        var key = _selectedPlayer.Keys[keyIndex];
                        var slope = keyIndex + 1 < Keys.Count && Keys[keyIndex + 1].InterpolationType == GXInterpolationType.HSD_A_OP_SLP ? Keys[keyIndex + 1] : null;
                        
                        sel.Add(new KeyProxy() { key = key, slopekey = slope });
                    }
                }
                keyProperty.SelectedObjects = sel.ToArray();
            }

            _graph.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool IsKeySelected(FOBJKey key)
        {
            foreach (KeyProxy v in keyProperty.SelectedObjects)
                if (v.key == key || v.slopekey == key)
                    return true;

            return false;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteKeyButton_Click(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
                _selectedPlayer.Keys.RemoveAll(k => IsKeySelected(k));

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
            
            // add/remove slope key
            foreach(KeyProxy k in keyProperty.SelectedObjects)
            {
                var next = Keys.IndexOf(k.key) + 1;

                if (next < Keys.Count & !k.DifferentTangents)
                {
                    if (Keys[next].InterpolationType == GXInterpolationType.HSD_A_OP_SLP)
                        Keys.RemoveAt(next);
                }
                else
                {
                    if (!Keys.Contains(k.slopekey))
                    {
                        if(next < Keys.Count && Keys[next].InterpolationType != GXInterpolationType.HSD_A_OP_SLP)
                        {
                            k.slopekey.Frame = Keys[next].Frame;
                            Keys.Insert(next, k.slopekey);
                        }
                    }
                }
            }

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

                    SelectKeyAtFrame();
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

            if(OnTrackListUpdate != null)
                OnTrackListUpdate.Invoke(this, EventArgs.Empty);
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

            if (OnTrackListUpdate != null)
                OnTrackListUpdate.Invoke(this, EventArgs.Empty);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpButton_Click(object sender, EventArgs e)
        {
            using (HelpBox hb = new HelpBox(HelpText))
                hb.ShowDialog();
        }

        private static string HelpText = @"Graph Editor:

Click on Graph to select keys and frames

Hold Control to select multiple keys

Interpolation Help:
KEY - Key
    Used for tracks that only have 1 key, no interpolation
    If your track has more than one key, do not use this

CON - Constant
    Value is constant until next key

LIN - Linear
    Value is linealy interpolation (straight line) to next key
    Used for harsh curves

SPL - Spline
    A Cubic spline with a tangent
    Used for smooth curves

SPL0 - Spline 0
    A cubic spline with a tangent of 0
    Results in flat tangents

SLP - Slope (do not use)
NONE - None (do not use)";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudFrame_ValueChanged(object sender, EventArgs e)
        {
            SelectKeyAtFrame();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Zoom = (float)(numericUpDown1.Value) / 100f * 0.9f;
            _graph.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptionCheckChanged(object sender, EventArgs e)
        {
            _options.ShowAllTracks = showAllTracksToolStripMenuItem.Checked;
            _options.ShowFrameTicks = showFrameTicksToolStripMenuItem.Checked;
            _options.ShowTangents = showTangentsToolStripMenuItem.Checked;
            _graph.Invalidate();
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
