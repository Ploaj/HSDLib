using HSDRaw;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Converters.Animation;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Renderers;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

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
                    if (slopekey != null)
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
            None,
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

        private readonly List<FOBJ_Player> _players = new();

        private static readonly GraphDisplayOptions _options = new();

        private int _selectedPlayerIndex = 0;

        private int _startSelectionFrame = 0;

        private int _frame { get => (int)nudFrame.Value; set => nudFrame.Value = Math.Max(Math.Min(value, nudFrame.Maximum), 0); }

        private AnimType _animType = AnimType.None;

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
                //var state = Keyboard.GetState();
                //return state.IsKeyDown(Key.ControlLeft) || state.IsKeyDown(Key.ControlRight);
                return false;
            }
        }

        private readonly Button _ptclButton;
        private readonly Label _ptclLabel;

        private readonly GLTextRenderer textRenderer = new();

        public static Vector4 FrameIndicatorPen = new(1f, 1f, 1f, 1f);
        public static Vector4 SelectionColor = new(128 / 255f, 128 / 255f, 200 / 255f, 90 / 255f);

        public static Vector4 TickColor = new(1f, 1f, 1f, 1f);
        public static Vector4 BackTickColor = new(0.2f, 0.2f, 0.2f, 1f);

        private static Vector4 LineColor = new(0.5f, 0.5f, 0.5f, 1f);
        private static Vector4 SelectedLineColor = new(1f, 1f, 1f, 1f);

        private static Vector4 FontColor = new(1f, 1f, 1f, 1f);

        public event EventHandler TrackListUpdated;
        protected virtual void OnTrackListUpdated(EventArgs e)
        {
            EventHandler handler = TrackListUpdated;
            OnTrackEdited(EventArgs.Empty);
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler TrackEdited;
        protected virtual void OnTrackEdited(EventArgs e)
        {
            EventHandler handler = TrackEdited;
            if (handler != null)
                handler(this, e);
        }

        public float Zoom = 0.9f;


        public class ParticleOptions
        {
            public int ParticleBank
            {
                get => _ptclBank;
                set
                {
                    if (value < 0x3F)
                        _ptclBank = value;
                    else
                        _ptclBank = 0x3F;
                }
            }
            private int _ptclBank;

            public int ParticleId
            {
                get => _ptclId;
                set
                {
                    if (value < 0x3FFFF)
                        _ptclId = value;
                    else
                        _ptclId = 0x3FFFF;
                }
            }
            private int _ptclId;
        }

        /// <summary>
        /// 
        /// </summary>
        public GraphEditor()
        {
            InitializeComponent();

            _ptclButton = new Button();
            _ptclButton.Dock = DockStyle.Top;
            _ptclButton.Visible = false;
            _ptclButton.Click += (sender, args) =>
            {
                ParticleOptions options = new()
                {
                    ParticleBank = _selectedPlayer.PtclBank,
                    ParticleId = _selectedPlayer.PtclId
                };
                using PropertyDialog d = new("Particle Options", options);
                if (d.ShowDialog() == DialogResult.OK)
                {
                    _selectedPlayer.PtclBank = options.ParticleBank;
                    _selectedPlayer.PtclId = options.ParticleId;
                    UpdatePtclLabel();
                }
            };
            graphBox.Controls.Add(_ptclButton);

            _ptclLabel = new Label();
            _ptclLabel.Dock = DockStyle.Top;
            _ptclLabel.Text = "Effect Bank: Effect ID:";
            _ptclLabel.Visible = false;
            graphBox.Controls.Add(_ptclLabel);

            glviewport.MouseUp += (sender, args) =>
            {
                SelectFrameFromMouse(args.X);

                if (IsControl)
                    SelectKeysInRange(_startSelectionFrame, _frame);

                glviewport.Invalidate();
            };

            glviewport.MouseDown += (sender, args) =>
            {
                SelectFrameFromMouse(args.X);

                _startSelectionFrame = _frame;

                glviewport.Invalidate();
            };

            glviewport.MouseMove += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                {
                    SelectFrameFromMouse(args.X);
                }
            };

            glviewport.Paint += (sender, args) =>
            {
                Render();
            };

            glviewport.Disposed += (s, a) =>
            {
                textRenderer.Dispose();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdatePtclLabel()
        {
            if (_selectedPlayer == null)
                _ptclLabel.Text = "";
            else
                _ptclLabel.Text = $"Bank: {_selectedPlayer.PtclBank} ID: {_selectedPlayer.PtclId}";
        }

        /// <summary>
        /// 
        /// </summary>
        private void PrepareGraph()
        {
            if (_selectedPlayer == null)
            {
                panel1.Visible = false;
                glviewport.Visible = false;
                _ptclLabel.Visible = false;
                _ptclButton.Visible = false;
            }
            if (_selectedPlayer.JointTrackType == JointTrackType.HSD_A_J_PTCL)
            {
                panel1.Visible = false;
                glviewport.Visible = false;
                _ptclLabel.Visible = true;
                _ptclButton.Visible = true;
                UpdatePtclLabel();
            }
            else
            {
                panel1.Visible = true;
                glviewport.Visible = true;
                _ptclLabel.Visible = false;
                _ptclButton.Visible = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HSD_FOBJDesc ToFOBJs()
        {
            HSD_FOBJDesc fobj = null;
            foreach (FOBJ_Player p in _players)
            {
                HSD_FOBJDesc f = p.ToFobjDesc();

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
            ClearTracks();

            SetTrackType(type);

            if (aobj.FObjDesc != null)
                foreach (HSD_FOBJDesc v in aobj.FObjDesc.List)
                    AddPlayer(new FOBJ_Player(v));

            if (trackTree.Nodes.Count > 0)
                trackTree.SelectedNode = trackTree.Nodes[0];

            glviewport.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="aobj"></param>
        public void LoadTracks(AnimType type, IEnumerable<FOBJ_Player> players)
        {
            ClearTracks();

            SetTrackType(type);

            if (players != null)
            {
                foreach (FOBJ_Player p in players)
                    AddPlayer(p);
            }

            if (trackTree.Nodes.Count > 0)
                trackTree.SelectedNode = trackTree.Nodes[0];

            glviewport.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        private void SetTrackType(AnimType type)
        {
            Type tt = typeof(JointTrackType);
            switch (type)
            {
                case AnimType.Material:
                    tt = typeof(MatTrackType);
                    break;
                case AnimType.Texture:
                    tt = typeof(TexTrackType);
                    break;
                case AnimType.Light:
                    tt = typeof(LightTrackType);
                    break;
                default:
                    tt = typeof(JointTrackType);
                    break;
            }

            if (type != _animType)
            {
                trackTypeBox.Items.Clear();
                foreach (object item in Enum.GetValues(tt))
                    trackTypeBox.Items.Add(item);
            }

            _animType = type;
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
                    return ((LightTrackType)type).ToString();
                default:
                    return "Type_" + type;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SelectFrameFromMouse(int mouseX)
        {
            float position = (mouseX - (glviewport.Width - glviewport.Width * Zoom) / 2) / (glviewport.Width * Zoom);

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
                int keyIndex = _selectedPlayer.Keys.FindIndex(e => e.Frame == _frame && e.InterpolationType != GXInterpolationType.HSD_A_OP_SLP);
                if (keyIndex != -1)
                {
                    FOBJKey key = _selectedPlayer.Keys[keyIndex];
                    FOBJKey slope = keyIndex + 1 < Keys.Count && Keys[keyIndex + 1].InterpolationType == GXInterpolationType.HSD_A_OP_SLP ? Keys[keyIndex + 1] : null;

                    keyProperty.SelectedObject = new KeyProxy() { key = key, slopekey = slope };
                }

                label4.Text = _selectedPlayer.GetValue(_frame).ToString();
            }

            glviewport.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SelectKeysInRange(int start, int end)
        {
            keyProperty.SelectedObject = null;

            if (_selectedPlayer != null)
            {
                List<object> sel = new();
                for (int i = Math.Min(start, end); i < Math.Max(start, end); i++)
                {
                    int keyIndex = _selectedPlayer.Keys.FindIndex(e => e.Frame == _frame && e.InterpolationType != GXInterpolationType.HSD_A_OP_SLP);
                    if (keyIndex != -1)
                    {
                        FOBJKey key = _selectedPlayer.Keys[keyIndex];
                        FOBJKey slope = keyIndex + 1 < Keys.Count && Keys[keyIndex + 1].InterpolationType == GXInterpolationType.HSD_A_OP_SLP ? Keys[keyIndex + 1] : null;

                        sel.Add(new KeyProxy() { key = key, slopekey = slope });
                    }
                }
                keyProperty.SelectedObjects = sel.ToArray();
            }

            glviewport.Invalidate();
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

            glviewport.Invalidate();

            OnTrackEdited(EventArgs.Empty);
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
            foreach (KeyProxy k in keyProperty.SelectedObjects)
            {
                int next = Keys.IndexOf(k.key) + 1;

                if (next < Keys.Count & !k.DifferentTangents)
                {
                    if (Keys[next].InterpolationType == GXInterpolationType.HSD_A_OP_SLP)
                        Keys.RemoveAt(next);
                }
                else
                {
                    if (!Keys.Contains(k.slopekey))
                    {
                        if (next < Keys.Count && Keys[next].InterpolationType != GXInterpolationType.HSD_A_OP_SLP)
                        {
                            k.slopekey.Frame = Keys[next].Frame;
                            Keys.Insert(next, k.slopekey);
                        }
                    }
                }
            }

            glviewport.Invalidate();

            OnTrackEdited(EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addKeyButton_Click(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
            {
                if (!_selectedPlayer.Keys.Exists(k => k.Frame == _frame))
                {
                    FOBJKey currKey = _selectedPlayer.Keys.Last(e => e.Frame < _frame);

                    FOBJKey key = new()
                    {
                        Frame = _frame,
                        Value = _selectedPlayer.GetValue(_frame),
                        InterpolationType = currKey != null ? currKey.InterpolationType : GXInterpolationType.HSD_A_OP_LIN
                    };

                    int insertIndex = _selectedPlayer.Keys.FindIndex(k => k.Frame >= _frame);

                    if (insertIndex == -1 || insertIndex >= _selectedPlayer.Keys.Count)
                    {
                        _selectedPlayer.Keys.Add(key);
                    }
                    else
                    {
                        _selectedPlayer.Keys.Insert(insertIndex, key);
                        key.Tan = AnimationKeyCompressor.CalculateTangent(_selectedPlayer, _frame);
                    }

                    SelectKeyAtFrame();

                }

                glviewport.Invalidate();

                OnTrackEdited(EventArgs.Empty);
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

            keyProperty.SelectedObject = null;
            _frame = 0;

            if ((e.Node.Tag as FOBJ_Player).TrackType < trackTypeBox.Items.Count)
                trackTypeBox.SelectedIndex = (e.Node.Tag as FOBJ_Player).TrackType;

            PrepareGraph();
            glviewport.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void optionsButton_Click(object sender, EventArgs e)
        {
            using (PropertyDialog d = new("Graph Display Options", _options))
                d.ShowDialog();

            glviewport.Invalidate();
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
                if ((byte)trackTypeBox.SelectedIndex != _selectedPlayer.TrackType)
                    OnTrackEdited(EventArgs.Empty);

                _selectedPlayer.TrackType = (byte)trackTypeBox.SelectedIndex;

                trackTree.Nodes[_selectedPlayerIndex].Text = GetTrackName(_selectedPlayer.TrackType);

                PrepareGraph();
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

            if (trackTree.Nodes.Count > 0)
                trackTree.SelectedNode = trackTree.Nodes[0];
            else
                trackTree.SelectedNode = null;

            glviewport.Invalidate();

            OnTrackListUpdated(EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addTrackButton_Click(object sender, EventArgs e)
        {
            FOBJ_Player player = new() { Keys = new List<FOBJKey>() };

            player.Keys.Add(new FOBJKey() { Frame = 0, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });
            player.Keys.Add(new FOBJKey() { Frame = _frameCount == 0 ? 10 : _frameCount, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });

            AddPlayer(player);

            trackTree.SelectedNode = trackTree.Nodes[trackTree.Nodes.Count - 1];

            glviewport.Invalidate();

            OnTrackListUpdated(EventArgs.Empty);
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
                List<FOBJKey> keys = HSDK.LoadKeys();
                if (keys != null)
                {
                    _selectedPlayer.Keys = keys;
                    glviewport.Invalidate();
                    OnTrackEdited(EventArgs.Empty);
                }
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
            using HelpBox hb = new(HelpText);
            hb.ShowDialog();
        }

        private static readonly string HelpText = @"Graph Editor:

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
            glviewport.Invalidate();
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
            glviewport.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBakeTrack_Click(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
            {
                _selectedPlayer.Bake();
                glviewport.Invalidate();
                OnTrackEdited(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class CompSettings
        {
            [Category("Settings"), DisplayName("Compression Level"), Description("Acceptable error range for compression.\nThe smaller the value the more accurate the compression but the larger the key count.")]
            public float CompressionLevel { get; set; } = 0.001f;
        }

        private static readonly CompSettings _compSettings = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCompressTrack_Click(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
            {
                /*for (int i = 0; i < _selectedPlayer.Keys.Count; i++)
                {
                    Simpoly.SimplyPoly(_selectedPlayer, new Simpoly.Options());
                    Console.WriteLine(_selectedPlayer.Keys[i].Frame + " " + _selectedPlayer.Keys[i].Tan);
                    _graph.Invalidate();
                    OnTrackEdited(EventArgs.Empty);
                }*/
                using PropertyDialog d = new("Compression Settings", _compSettings);
                if (d.ShowDialog() == DialogResult.OK)
                {
                    _selectedPlayer.Bake();
                    AnimationKeyCompressor.CompressTrack(_selectedPlayer);
                    glviewport.Invalidate();
                    OnTrackEdited(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reverseButton_Click(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
            {
                _selectedPlayer.Reverse();
                glviewport.Invalidate();
                OnTrackEdited(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glviewport_Resize(object sender, EventArgs e)
        {
        }

        private Camera _camera;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glviewport_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.1568f, 0.1568f, 0.1568f, 1);

            textRenderer.InitializeRender(@"GraphArial.bff");

            // setup camera
            _camera = new Camera();
            _camera.RenderWidth = glviewport.Width;
            _camera.RenderHeight = glviewport.Height;
            _camera.Translation = new Vector3(0, 10, -80);
        }

        private readonly GraphRenderer gr = new();

        /// <summary>
        /// 
        /// </summary>
        private void Render()
        {
            glviewport.MakeCurrent();

            // setup viewport
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Viewport(0, 0, glviewport.Width, glviewport.Height);

            Rectangle rect = glviewport.ClientRectangle;
            float x1 = rect.Width - rect.Width * Zoom;
            float y1 = rect.Height - rect.Height * Zoom;

            // create view matrix
            Matrix4 v = Matrix4.CreateOrthographicOffCenter(0, glviewport.Width, glviewport.Height, 0, 0, 1);
            Matrix4 mv = Matrix4.CreateScale(Zoom) * Matrix4.CreateTranslation(x1 / 2, y1 / 2, 0);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref v);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref mv);

            // draw frame ticks
            if (_options.ShowFrameTicks)
            {
                if (_selectedPlayer != null)
                {
                    gr.SetKeys(_selectedPlayer);

                    // horizontal lines
                    GL.Color4(BackTickColor);
                    GL.Begin(PrimitiveType.Lines);

                    GL.Vertex2(-x1, 0); GL.Vertex2(x1 * 2 + rect.Width, 0);
                    GL.Vertex2(-x1, rect.Height / 4f); GL.Vertex2(x1 * 2 + rect.Width, rect.Height / 4f);
                    GL.Vertex2(-x1, rect.Height / 2f); GL.Vertex2(x1 * 2 + rect.Width, rect.Height / 2f);
                    GL.Vertex2(-x1, rect.Height * 3f / 4f); GL.Vertex2(x1 * 2 + rect.Width, rect.Height * 3f / 4f);
                    GL.Vertex2(-x1, rect.Height); GL.Vertex2(x1 * 2 + rect.Width, rect.Height);

                    GL.End();

                    //graphics.DrawLine(BackTickColor, -x1, 0, x1 * 2 + rect.Width, 0);
                    //graphics.DrawLine(BackTickColor, -x1, rect.Height / 4f, x1 * 2 + rect.Width, rect.Height / 4f);
                    //graphics.DrawLine(BackTickColor, -x1, rect.Height / 2f, x1 * 2 + rect.Width, rect.Height / 2f);
                    //graphics.DrawLine(BackTickColor, -x1, rect.Height * 3f / 4f, x1 * 2 + rect.Width, rect.Height * 3f / 4f);
                    //graphics.DrawLine(BackTickColor, -x1, rect.Height, x1 * 2 + rect.Width, rect.Height);

                    Vector3 textPos = Vector3.TransformPosition(new Vector3(-20, 0, 0), mv);
                    GL.Color4(FontColor);
                    textRenderer.RenderText(gr.MinValue.ToString("n2"), glviewport.Width, glviewport.Height, (int)textPos.X, (int)textPos.Y, StringAlignment.Center);

                    textPos = Vector3.TransformPosition(new Vector3(-20, rect.Height / 2f, 0), mv);
                    GL.Color4(FontColor);
                    textRenderer.RenderText(((gr.MinValue + gr.MaxValue) / 2).ToString("n2"), glviewport.Width, glviewport.Height, (int)textPos.X, (int)textPos.Y, StringAlignment.Center);

                    textPos = Vector3.TransformPosition(new Vector3(-20, rect.Height, 0), mv);
                    GL.Color4(FontColor);
                    textRenderer.RenderText(gr.MaxValue.ToString("n2"), glviewport.Width, glviewport.Height, (int)textPos.X, (int)textPos.Y, StringAlignment.Center);

                    //graphics.DrawString(gr.MinValue.ToString("n2"), _markerFont, FontColor, -20, 0, _markerFormat);
                    //graphics.DrawString(((gr.MinValue + gr.MaxValue) / 2).ToString("n2"), _markerFont, FontColor, -20, rect.Height / 2f, _markerFormat);
                    //graphics.DrawString(gr.MaxValue.ToString("n2"), _markerFont, FontColor, -20, rect.Height, _markerFormat);
                }

                float tickWidth = (rect.Width / (float)_frameCount);
                int increment = 1;

                // fix infinity
                if (_frameCount == 0)
                    tickWidth = 1;

                // don't crunch numbers too far together
                if (tickWidth * Zoom < 40)
                    increment = (int)Math.Ceiling(40 / (tickWidth * Zoom));

                // round increment to upper 5
                if (increment != 1)
                    increment = (int)(Math.Ceiling(increment / 5f) * 5f);

                // draw vertical tick lines
                GL.Begin(PrimitiveType.Lines);
                for (int i = 0; i <= _frameCount; i++)
                {
                    if (i % increment == 0)
                    {
                        float x = i * tickWidth;

                        // frame line
                        GL.Color4(BackTickColor);
                        GL.Vertex2(x, 0); GL.Vertex2(x, rect.Height);
                        //graphics.DrawLine(BackTickColor, x, 0, x, rect.Height);

                        // tick line
                        GL.Color4(TickColor);
                        GL.Vertex2(x, 0); GL.Vertex2(x, 5f);
                        //graphics.DrawLine(TickColor, x, 0, x, 5f);
                    }
                }
                GL.End();

                // draw numbers
                for (int i = 0; i <= _frameCount; i++)
                {
                    if (i % increment == 0)
                    {
                        float x = i * tickWidth;

                        // tick number
                        Vector3 textPos = Vector3.TransformPosition(new Vector3(x, -14 * Zoom, 0), mv);
                        GL.Color4(FontColor);
                        textRenderer.RenderText(i.ToString(), glviewport.Width, glviewport.Height, (int)textPos.X, (int)textPos.Y, StringAlignment.Center);
                        // graphics.DrawString(i.ToString(), _markerFont, FontColor, x, -12, _markerFormat);
                    }
                }
            }

            // draw tracks
            foreach (FOBJ_Player p in _players)
            {
                if (!_options.ShowAllTracks && p != _selectedPlayer)
                    continue;

                gr.RenderTangents = _options.ShowTangents;

                if (p == _selectedPlayer)
                {
                    gr.LineColor = SelectedLineColor;
                }
                else
                {
                    gr.RenderPoints = false;
                    gr.LineColor = LineColor;
                }

                gr.SetKeys(p);

                if (IsControl)
                    gr.Draw(rect,
                        Math.Min(_frame, _startSelectionFrame),
                        Math.Max(_frame, _startSelectionFrame));
                else
                    gr.Draw(rect, _frame, _frame);
            }


            // draw selection
            float linex = _frame * (rect.Width / (float)_frameCount);

            if (!float.IsNaN(linex) && !float.IsInfinity(linex))
            {
                if (IsControl)
                {
                    float start = _startSelectionFrame * (rect.Width / (float)_frameCount);

                    //graphics.FillRectangle(SelectionColor, Math.Min(start, linex), 0, Math.Abs(start - linex), glviewport.Height);

                    //graphics.DrawLine(FrameIndicatorPen, start, 0, start, glviewport.Height);

                    float s1 = Math.Min(start, linex);
                    float s2 = Math.Abs(start - linex);
                    GL.Color4(SelectionColor);
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex2(s1, 0);
                    GL.Vertex2(s2, 0);
                    GL.Vertex2(s2, glviewport.Height);
                    GL.Vertex2(s1, glviewport.Height);
                    GL.End();

                    GL.Color4(FrameIndicatorPen);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex2(start, 0);
                    GL.Vertex2(start, glviewport.Height);
                    GL.End();
                }

                GL.Color4(FrameIndicatorPen);
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(linex, 0);
                GL.Vertex2(linex, rect.Height);
                GL.End();
                // graphics.DrawLine(FrameIndicatorPen, linex, 0, linex, glviewport.Height);
            }

            glviewport.SwapBuffers();
        }


        /// <summary>
        /// 
        /// </summary>
        public class ShiftSettings
        {
            [Category("Settings"), DisplayName("Shift Amount"), Description("Add this value to all key's values.")]
            public float ShiftAmount { get; set; } = 0;

            [Category("Settings"), DisplayName("Start Frame"), Description("Frame to start shifting. -1 for all")]
            public int StartFrame { get; set; } = -1;

            [Category("Settings"), DisplayName("End Frame"), Description("Frame to end shifting. -1 for all")]
            public int EndFrame { get; set; } = -1;
        }

        private static readonly ShiftSettings _shiftSettings = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void shiftValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedPlayer != null)
            {
                using PropertyDialog d = new("Shift Settings", _shiftSettings);
                if (d.ShowDialog() == DialogResult.OK)
                {
                    foreach (FOBJKey k in _selectedPlayer.Keys)
                    {
                        if (_shiftSettings.StartFrame > 0 &&
                            k.Frame < _shiftSettings.StartFrame)
                            continue;

                        if (_shiftSettings.EndFrame > 0 &&
                            k.Frame > _shiftSettings.EndFrame)
                            continue;

                        k.Value += _shiftSettings.ShiftAmount;
                    }
                    glviewport.Invalidate();
                    OnTrackEdited(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuImportTracks_Click(object sender, EventArgs e)
        {
            var f = Tools.FileIO.OpenFile("Supported Image Formats|*.json;*.MaterialColor;");

            if (f == null)
                return;

            List<FOBJ_Player> newPlayers = null;
            switch (Path.GetExtension(f).ToLower())
            {
                case ".json":
                    {
                        try
                        {
                            newPlayers = JsonSerializer.Deserialize<List<FOBJ_Player>>(File.ReadAllText(f), jsonOptions);
                        } 
                        catch (Exception er)
                        {
                            MessageBox.Show(er.Message, "Json Import Error");
                            return;
                        }
                    }
                    break;
                case ".materialcolor":
                    {
                        using (var stream = new FileStream(f, FileMode.Open))
                        using (var d = new BinaryReaderExt(stream))
                        {
                            FOBJ_Player r = new FOBJ_Player() { TrackType = (byte)MatTrackType.HSD_A_M_DIFFUSE_R };
                            FOBJ_Player g = new FOBJ_Player() { TrackType = (byte)MatTrackType.HSD_A_M_DIFFUSE_G };
                            FOBJ_Player b = new FOBJ_Player() { TrackType = (byte)MatTrackType.HSD_A_M_DIFFUSE_B };
                            FOBJ_Player a = new FOBJ_Player() { TrackType = (byte)MatTrackType.HSD_A_M_ALPHA };
                            newPlayers = new List<FOBJ_Player>()
                            {
                                r, g, b, a
                            };

                            d.ReadInt32();
                            d.ReadInt32();
                            var count = d.ReadInt32();

                            float frame = 0;
                            for (int i = 0; i < count; i++)
                            {
                                if (d.Position + 4 > d.Length)
                                    break;

                                b.Keys.Add(new FOBJKey() { Frame = frame, Value = d.ReadByte() / 255f, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });
                                g.Keys.Add(new FOBJKey() { Frame = frame, Value = d.ReadByte() / 255f, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });
                                r.Keys.Add(new FOBJKey() { Frame = frame, Value = d.ReadByte() / 255f, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });
                                a.Keys.Add(new FOBJKey() { Frame = frame, Value = d.ReadByte() / 255f, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });
                                frame += 1.0f;
                            }
                        }
                    }
                    break;
            }

            if (newPlayers == null)
                return;

            // load new tracks and refresh tree
            //_players.Clear();
            //trackTree.Nodes.Clear();
            foreach (var p in newPlayers)
                AddPlayer(p);

            // invalidate
            glviewport.Invalidate();
            OnTrackListUpdated(EventArgs.Empty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuExportTracks_Click(object sender, EventArgs e)
        {
            if (_players.Count == 0)
                return;

            var f = Tools.FileIO.SaveFile(ApplicationSettings.JsonFileFilter);

            if (f == null)
                return;

            var json = JsonSerializer.Serialize(_players, jsonOptions);
            System.IO.File.WriteAllText(f, json);
        }
    }
}
