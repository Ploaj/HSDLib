using HSDRaw.Common;
using System;

namespace HSDRawViewer.GUI.Plugins
{
    [SupportedTypes(new Type[] { typeof(HSD_LOBJ) })]
    public partial class LObjEditorDock : PluginBase
    {
        public override DataNode Node
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
