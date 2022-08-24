using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Pl;
using HSDRawViewer.GUI.Controls.JObjEditor;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Animation;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins
{
    [SupportedTypes(new Type[] { typeof(HSD_JOBJ) })]
    public partial class JobjEditorDock : PluginBase
    {
        public override DataNode Node
        {
            get => _node;
            set
            {
                _node = value;

                if (_node.Accessor is HSD_JOBJ jobj)
                    Editor.SetJOBJ(jobj);
            }
        }
        private DataNode _node;

        public JObjEditorNew Editor { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public JobjEditorDock()
        {
            InitializeComponent();

            Editor = new JObjEditorNew();
            Editor.Dock = DockStyle.Fill;
            Controls.Add(Editor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        public void LoadPhysics(SBM_PhysicsGroup group)
        {
            //Editor.LoadPhysics(group);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        public void LoadAnimation(HSD_MatAnimJoint joint)
        {
            Editor.LoadAnimation(new MatAnimManager(joint));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anim"></param>
        public void LoadAnimation(JointAnimManager anim)
        {
            Editor.LoadAnimation(anim);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anim"></param>
        public void LoadAnimation(HSD_ShapeAnimJoint anim)
        {
            ShapeAnimManager m = new ShapeAnimManager();
            m.FromShapeAnim(anim);
            Editor.LoadAnimation(m);
        }
    }
}
