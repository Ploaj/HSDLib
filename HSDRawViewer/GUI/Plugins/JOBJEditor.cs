using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRawViewer.Rendering;
using System;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins
{
    /// <summary>
    /// 
    /// </summary>
    public partial class JOBJEditor : DockContent, EditorBase, IDrawable
    {
        public DockState DefaultDockState => DockState.DockLeft;

        public DrawOrder DrawOrder => DrawOrder.First;

        public JOBJEditor()
        {
            InitializeComponent();

            Renderer = new RendererJOBJ();

            if(PluginManager.GetCommonViewport() != null)
                PluginManager.GetCommonViewport().AddRenderer(this);

            FormClosing += (sender, args) =>
            {
                if (PluginManager.GetCommonViewport() != null)
                {
                    PluginManager.GetCommonViewport().AnimationTrackEnabled = false;
                    PluginManager.GetCommonViewport().RemoveRenderer(this);
                    Renderer.ClearCache();
                }
            };
        }

        public Type[] SupportedTypes => new Type[] { typeof(HSD_JOBJ) };

        public DataNode Node { get => _node;
            set
            {
                _node = value;

                if (_node.Accessor is HSD_JOBJ jobj)
                    root = jobj;
            }
        }

        private DataNode _node;
        private HSD_JOBJ root;

        private RendererJOBJ Renderer;

        public void LoadAnimation(HSD_FigaTree tree)
        {
            var vp = PluginManager.GetCommonViewport();
            vp.AnimationTrackEnabled = true;
            vp.Frame = 0;
            vp.MaxFrame = tree.FrameCount;
            Renderer.SetFigaTree(tree);
        }

        public void LoadAnimation(HSD_AnimJoint joint)
        {
            var vp = PluginManager.GetCommonViewport();
            vp.AnimationTrackEnabled = true;
            vp.Frame = 0;
            vp.MaxFrame = Renderer.SetAnimJoint(joint);
        }

        public void Draw(int windowWidth, int windowHeight)
        {
            Renderer.Render(root, (int)PluginManager.GetCommonViewport().Frame);
        }
    }
}
