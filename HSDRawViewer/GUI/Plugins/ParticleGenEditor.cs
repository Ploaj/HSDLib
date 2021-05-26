using HSDRaw.Common;
using HSDRaw.Tools;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins
{
    [SupportedTypes(new Type[] { typeof(HSD_ParticleGroup) })]
    public partial class ParticleGenEditor : DockContent, EditorBase
    {
        public DockState DefaultDockState => DockState.Document;

        public DataNode Node
        {
            get
            {
                return _node;
            }
            set
            {
                _node = value;
                if (_node != null && _node.Accessor is HSD_ParticleGroup group)
                    ParticleGroup = group;
                RefreshGUI();
            }
        }
        private DataNode _node;

        private HSD_ParticleGroup ParticleGroup;

        public HSD_ParticleGenerator[] Particles { get => ParticleGroup.Particles; set => ParticleGroup.Particles = value; }

        public ParticleOpCode[] OpCodes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ParticleGenEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshGUI()
        {
            particleArrayEditor.SetArrayFromProperty(this, "Particles");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void particleArrayEditor_SelectedObjectChanged(object sender, EventArgs e)
        {
            if(particleArrayEditor.SelectedObject is HSD_ParticleGenerator partgen)
            {
                OpCodes = ParticleEncoding.DecodeParticleOpCodes(partgen.TrackData).Select(t => new ParticleOpCode() { Code = t.Item1, Params = t.Item2 }).ToArray();
                OpCodeArrayEditor.SetArrayFromProperty(this, "OpCodes");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RecompileTrack()
        {
            if (particleArrayEditor.SelectedObject is HSD_ParticleGenerator partgen)
            {
                // todo: recompile track data
                partgen.TrackData = ParticleEncoding.EncodeParticleCodes(OpCodes.Select(e=>new Tuple<byte, object[]>(e.Code, e.Params)));
            }

            ParticleGroup.Particles = Particles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpCodeArrayEditor_ArrayUpdated(object sender, EventArgs e)
        {
            RecompileTrack();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpCodeArrayEditor_DoubleClickedNode(object sender, EventArgs e)
        {
            if (OpCodeArrayEditor.SelectedObject is ParticleOpCode opcode)
                using (ParticleGenEditorPanel panel = new ParticleGenEditorPanel(opcode))
                    if (panel.ShowDialog() == DialogResult.OK)
                        RecompileTrack();
        }
    }
}
