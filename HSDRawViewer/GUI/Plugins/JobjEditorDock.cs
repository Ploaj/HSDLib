using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRawViewer.Rendering;
using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins
{
    public partial class JobjEditorDock : DockContent, EditorBase
    {
        public DockState DefaultDockState => DockState.DockLeft;

        public Type[] SupportedTypes => new Type[] { typeof(HSD_JOBJ) };

        public DataNode Node
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

        private JobjEditor Editor;

        /// <summary>
        /// 
        /// </summary>
        public JobjEditorDock()
        {
            InitializeComponent();

            Editor = new JobjEditor();
            Editor.Dock = DockStyle.Fill;
            Controls.Add(Editor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        public void LoadAnimation(HSD_MatAnimJoint joint)
        {
            Editor.LoadAnimation(joint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anim"></param>
        public void LoadAnimation(JointAnimManager anim)
        {
            Editor.LoadAnimation(anim);
        }
    }
}
