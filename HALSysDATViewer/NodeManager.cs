using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HALSysDATViewer.Nodes;
using HSDLib;

namespace HALSysDATViewer
{
    public sealed class NodeManager
    {
        private NodeManager()
        {
        }

        public static NodeManager Instance { get { return Nested.instance; } }

        public Dictionary<Type, IBaseNode> NodesLookup = new Dictionary<Type, IBaseNode>();

        private class Nested
        {
            static Nested()
            {
            }
            internal static readonly NodeManager instance = new NodeManager();
        }
    }
}
