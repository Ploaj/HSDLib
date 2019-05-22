using System.Collections.Generic;
using HSDLib.GX;

namespace HSDLib.Common
{
    public class HSD_AttributeGroup : IHSDNode
    {
        public List<GXVertexBuffer> Attributes = new List<GXVertexBuffer>();

        public override void Open(HSDReader Reader)
        {
            GXVertexBuffer a = new GXVertexBuffer();
            a.Open(Reader);
            while(a.Name != GXAttribName.GX_VA_NULL)
            {
                Attributes.Add(a);
                a = new GXVertexBuffer();
                a.Open(Reader);
            }
        }

        public override void Save(HSDWriter Writer)
        {
            Writer.AddObject(this);
            foreach(GXVertexBuffer v in Attributes)
            {
                v.Save(Writer);
            }
            GXVertexBuffer end = new GXVertexBuffer();
            end.Save(Writer);
        }
    }
}
