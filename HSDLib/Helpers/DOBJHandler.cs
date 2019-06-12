using HSDLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.Helpers
{
    public class DOBJHandler
    {
        private HSD_DOBJ _dobj;

        public DOBJHandler(HSD_DOBJ dobj)
        {
            _dobj = dobj;
        }

        public void GetPolygons(out List<GXVertex> vertices, out List<GXDisplayList> displayLists)
        {
            vertices = new List<GXVertex>();
            displayLists = new List<GXDisplayList>();
            if (_dobj.POBJ == null)
                return;
            foreach(var pobj in _dobj.POBJ.List)
            {
                vertices.AddRange(VertexAccessor.GetDecodedVertices(pobj));
                var displayList = VertexAccessor.GetDisplayList(pobj);
                

            }
        }
    }
}
