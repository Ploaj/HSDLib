using HSDRaw.Common;
using System;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Plugins
{
    [SupportedTypes(new Type[] { typeof(HSD_LOBJ) })]
    public partial class LObjEditorDock : DockContent, EditorBase
    {
        public DockState DefaultDockState => DockState.DockLeft;

        public DataNode Node
        {
            get => _node;
            set
            {
                _node = value;

                if (_node.Accessor is HSD_LOBJ lobj)
                    lObjEditor1.SetLObj(lobj);
            }
        }

        private DataNode _node;

        public LObjEditorDock()
        {
            InitializeComponent();
        }
    }
}
