using HSDRaw.Common;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Renderers;
using HSDRawViewer.Tools;
using System;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins
{
    [SupportedTypes(new Type[] { typeof(HSD_ParticleGroup) })]
    public partial class ParticleEditor : PluginBase, IDrawable
    {
        public override DataNode Node
        {
            get => _node;
            set
            {
                _node = value;
                if (_node.Accessor is HSD_ParticleGroup g)
                {
                    _group = g;
                    _texg = SearchForTexG(_node);

                    _generators = g.Generators.ToArray();
                    generateArrayEditor.SetArrayFromProperty(this, nameof(_generators));

                    SetupRendering();

                    generateArrayEditor.ItemIndexOffset = g.EffectIDStart;
                }
                else
                {
                    Close();
                }
            }
        }
        private DataNode _node;

        public DrawOrder DrawOrder => DrawOrder.Last;

        private readonly ViewportControl _viewport;

        private readonly ParticleSystem _system = new();

        public HSD_ParticleGenerator[] _generators { get; set; }

        public ParticleEvent[] _events { get; set; }

        private HSD_ParticleGroup _group;
        private HSD_TEXGraphicBank _texg;

        private readonly GLTextRenderer TextRenderer = new();

        /// <summary>
        /// 
        /// </summary>
        public ParticleEditor()
        {
            InitializeComponent();

            _viewport = new ViewportControl();
            _viewport.Dock = DockStyle.Fill;
            _viewport.DisplayGrid = true;
            _viewport.AddRenderer(this);
            _viewport.Play(PlaybackMode.Forward);
            groupBoxPreview.Controls.Add(_viewport);
            _viewport.RefreshSize();
            _viewport.BringToFront();

            buttonRandom.Checked = ParticleGenerator.RandomSeed;

            cbEventType.Items.AddRange(ParticleManager.GetDescriptors());

            generateArrayEditor.SelectedObjectChanged += (sender, args) =>
            {
                _system.DestroyAllGenerators();
                propertyGridGenerator.SelectedObject = generateArrayEditor.SelectedObject;

                if (generateArrayEditor.SelectedObject is HSD_ParticleGenerator gen)
                {
                    _events = ParticleManager.DecompileCode(gen.TrackData).ToArray();
                    ptclEventArrayEditor.SetArrayFromProperty(this, nameof(_events));

                    SelectedEventChanged();
                }
            };

            generateArrayEditor.DoubleClickedNode += (sender, args) =>
            {
                if (generateArrayEditor.SelectedObject is HSD_ParticleGenerator gen)
                {
                    _system.SpawnGenerator(gen);
                }
            };

            ptclEventArrayEditor.ArrayUpdated += (sender, args) =>
            {
                CompileCode();
            };

            Disposed += (sender, args) =>
            {
                TextRenderer.Dispose();
                _system.Dispose();
                _viewport.Dispose();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetupRendering()
        {
            if (_texg != null)
                _system.LoadParticleGroup(_generators, _texg);
        }

        /// <summary>
        /// 
        /// </summary>
        private void CompileCode()
        {
            if (generateArrayEditor.SelectedObject is HSD_ParticleGenerator gen)
                gen.TrackData = ParticleManager.CompileCode(_events);
        }

        /// <summary>
        /// 
        /// </summary>
        private HSD_TEXGraphicBank SearchForTexG(DataNode d)
        {
            HSDRaw.HSDAccessor sym = MainForm.Instance.GetSymbol(d.Text.Replace("_ptcl", "_texg"));
            if (sym is HSD_TEXGraphicBank)
                return (HSD_TEXGraphicBank)sym;

            if (d.Parent == null)
                return (HSD_TEXGraphicBank)MainForm.Instance.GetSymbol("map_texg");

            if (((DataNode)d.Parent).Accessor is HSDRaw.Melee.Ef.SBM_EffectTable tbl)
                return tbl.TextureGraphics;

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void GLInit()
        {
            _system.Initialize();
            TextRenderer.InitializeRender(@"Consolas.bff");
        }

        /// <summary>
        /// 
        /// </summary>
        public void GLFree()
        {
            TextRenderer.Dispose();
            _system.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            if (!buttonPause.Checked)
                _system.Update();

            OpenTK.Graphics.OpenGL.GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.DepthTest);

            _system.Render(cam);

            TextRenderer.RenderText(cam, $"Generators: {_system.LiveGeneratorCount}", 16, 16);
            TextRenderer.RenderText(cam, $"Particles: {_system.ParticleCount}", 16, 32);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGenGenerator_Click(object sender, EventArgs e)
        {
            if (generateArrayEditor.SelectedObject is HSD_ParticleGenerator gen)
            {
                _system.SpawnGenerator(gen);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStep_Click(object sender, EventArgs e)
        {
            if (buttonPause.Checked)
                _system.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonKillAll_Click(object sender, EventArgs e)
        {
            _system.DestroyAllGenerators();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            _group.Generators = _generators;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SelectedEventChanged()
        {
            if (ptclEventArrayEditor.SelectedObject is ParticleEvent env)
            {
                //CustomClass c = new CustomClass();

                //foreach (var p in env.Params)
                //    c.Add(new CustomProperty(p.Name, p.Value, false, true));

                propertyGridEvent.SelectedObject = env;
                propertyGridEvent.ExpandAllGridItems();
                cbEventType.SelectedItem = ParticleManager.GetParticleDescriptor(env.Code);

                CompileCode();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ptclEventArrayEditor_SelectedObjectChanged(object sender, EventArgs e)
        {
            SelectedEventChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbEventType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ptclEventArrayEditor.SelectedObject is ParticleEvent env &&
                cbEventType.SelectedItem is ParticleDescriptor desc)
            {
                if (desc.Code != env.Code)
                {
                    env.SetCode(desc.Code);
                    SelectedEventChanged();
                    ptclEventArrayEditor.Invalidate();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void propertyGridEvent_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            CompileCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void generateArrayEditor_ArrayUpdated(object sender, EventArgs e)
        {
            SetupRendering();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRandom_CheckedChanged(object sender, EventArgs e)
        {
            ParticleGenerator.RandomSeed = buttonRandom.Checked;
        }
    }
}