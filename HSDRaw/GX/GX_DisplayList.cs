using HSDRaw.Common;
using HSDRaw.Tools;
using System.Collections.Generic;
using System.IO;

namespace HSDRaw.GX
{
    /// <summary>
    /// Display Lists contain groups of primitives that make up the indices for the mesh
    /// </summary>
    public class GX_DisplayList
    {
        public List<GX_PrimitiveGroup> Primitives = new List<GX_PrimitiveGroup>();

        public List<GX_Attribute> Attributes = new List<GX_Attribute>();

        public List<HSD_Envelope> Envelopes = new List<HSD_Envelope>();

        public List<GX_Vertex> Vertices = new List<GX_Vertex>();

        public List<GX_Shape[]> ShapeSets = new List<GX_Shape[]>();

        //TODO: single skin

        public GX_DisplayList()
        {

        }

        public GX_DisplayList(HSD_POBJ pobj, GX_Attribute[] attrs = null)
        {
            // read attributes
            if (attrs == null)
                attrs = pobj.ToGXAttributes();

            if(attrs[attrs.Length - 1].AttributeName != GXAttribName.GX_VA_NULL)
            {
                System.Diagnostics.Debug.WriteLine("Attribute buffer does not end with null vertex attribute");
            }

            Attributes.AddRange(attrs);

            // read display list buffer
            Open(pobj.DisplayListBuffer);
            
            // load vertices
            Vertices.AddRange(GX_VertexAccessor.GetDecodedVertices(this, pobj));

            // load shape sets if they exist
            if(pobj.ShapeSet != null)
                for(int i = 0; i < pobj.ShapeSet.VertexIndices.Count; i++)
                    ShapeSets.Add(GX_VertexAccessor.GetShapeSet(this, pobj, i));

            // get envelopse
            if(pobj.EnvelopeWeights != null)
                Envelopes.AddRange(pobj.EnvelopeWeights);
        }

        private void Open(byte[] Buffer)
        {
            if (Buffer == null)
                return;

            if (Attributes.Count == 0 || Attributes[Attributes.Count - 1].AttributeName != GXAttribName.GX_VA_NULL)
                return;

            using (BinaryReaderExt reader = new BinaryReaderExt(new MemoryStream(Buffer)))
            {
                reader.BigEndian = true;
                while (reader.Position < Buffer.Length)
                {
                    GX_PrimitiveGroup g = new GX_PrimitiveGroup();
                    if (!g.Read(reader, Attributes.ToArray()))
                        break;
                    Primitives.Add(g);
                }
            }
        }

        public byte[] ToBuffer()
        {
            MemoryStream o = new MemoryStream();
            using (BinaryWriterExt writer = new BinaryWriterExt(o))
            {
                writer.BigEndian = true;
                foreach (GX_PrimitiveGroup g in Primitives)
                {
                    g.Write(writer, Attributes.ToArray());
                }
                writer.Write((byte)0);

                writer.Align(0x20);
            }

            byte[] bytes = o.ToArray();
            o.Dispose();

            return bytes;
        }
    }
}
