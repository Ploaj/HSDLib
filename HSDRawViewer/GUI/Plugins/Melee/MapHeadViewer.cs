using HSDRaw.Melee.Gr;
using System;

namespace HSDRawViewer.GUI.Plugins.Melee
{
    [SupportedTypes(new Type[] { typeof(SBM_Map_Head) })]
    public partial class MapHeadViewer : PluginBase
    {
        public override DataNode Node
        {
            get => _node;
            set
            {
                _node = value;

                if (_node.Accessor is SBM_Map_Head head)
                    mapHeadViewControl1.LoadMapHead(head);
            }
        }
        private DataNode _node;

        public MapHeadViewer()
        {
            InitializeComponent();
        }
    }
}
