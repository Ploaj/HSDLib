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

        public List<GX_Vertex> Vertices = new List<GX_Vertex>();

        public List<GX_Attribute> Attributes = new List<GX_Attribute>();

        public List<HSD_Envelope> Envelopes = new List<HSD_Envelope>();

        //TODO: single skin

        public GX_DisplayList()
        {

        }

        public GX_DisplayList(HSD_POBJ pobj)
        {
            Attributes.AddRange(pobj.Attributes);

            Open(pobj.DisplayListBuffer);
            
            Vertices.AddRange(GX_VertexAttributeAccessor.GetDecodedVertices(this, pobj));

            if(pobj.EnvelopeWeights != null)
                Envelopes.AddRange(pobj.EnvelopeWeights);
        }

        private void Open(byte[] Buffer)
        {
            if (Buffer == null)
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
