using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSDLib.Common;
using System.IO;

namespace HSDLib.GX
{
    public enum GXPrimitiveType
    {
        Points = 0xB8,
        Lines = 0xA8,
        LineStrip = 0xB0,
        Triangles = 0x90,
        TriangleStrip = 0x98,
        TriangleFan = 0xA0,
        Quads = 0x80
    }

    public class GXIndexGroup
    {
        public ushort[] Indices;
        public byte[] Clr;
    }

    public class GXPrimitiveGroup
    {
        public GXPrimitiveType PrimitiveType;
        public ushort Count;
        public GXIndexGroup[] Indices;

        public bool Read(HSDReader Reader, HSD_AttributeGroup Attributes)
        {
            PrimitiveType = (GXPrimitiveType)Reader.ReadByte();
            if (PrimitiveType == 0)
                return false;
            Count = Reader.ReadUInt16();
            Indices = new GXIndexGroup[Count];
            for (int j = 0; j < Count; j++)
            {
                GXIndexGroup g = new GXIndexGroup();
                g.Indices = new ushort[Attributes.Attributes.Count];
                int i = 0;
                Indices[j] = g;
                foreach (GXVertexBuffer att in Attributes.Attributes)
                {
                    switch (att.AttributeType)
                    {
                        case GXAttribType.GX_DIRECT:
                            if (att.Name != GXAttribName.GX_VA_CLR0)
                                g.Indices[i] = Reader.ReadByte();
                            else
                                g.Clr = ReadGXClr(Reader, (int)att.CompType);
                            break;
                        case GXAttribType.GX_INDEX8:
                            g.Indices[i] = Reader.ReadByte();
                            break;
                        case GXAttribType.GX_INDEX16:
                            g.Indices[i] = (ushort)Reader.ReadUInt16();
                            break;
                    }
                    i++;
                }
            }
            return true;
        }
        private static byte[] ReadGXClr(HSDReader Reader, int CompType)
        {
            byte[] clr = new byte[] { 1, 1, 1, 1 };
            int b;

            switch (CompType)
            {
                case 0: // GX_RGB565
                    b = Reader.ReadUInt16();
                    clr[0] = (byte)((((b >> 11) & 0x1F) << 3) | (((b >> 11) & 0x1F) >> 2));
                    clr[1] = (byte)((((b >> 5) & 0x3F) << 2) | (((b >> 5) & 0x3F) >> 4));
                    clr[2] = (byte)((((b) & 0x1F) << 3) | (((b) & 0x1F) >> 2));
                    break;
                case 1: // GX_RGB888
                    clr[0] = Reader.ReadByte();
                    clr[1] = Reader.ReadByte();
                    clr[2] = Reader.ReadByte();
                    break;
                case 2: // GX_RGBX888
                    clr[0] = Reader.ReadByte();
                    clr[1] = Reader.ReadByte();
                    clr[2] = Reader.ReadByte();
                    Reader.ReadByte();
                    break;
                case 3: // GX_RGBA4
                    b = Reader.ReadUInt16();
                    clr[0] = (byte)((((b >> 12) & 0xF) << 4) | ((b >> 12) & 0xF));
                    clr[1] = (byte)((((b >> 8) & 0xF) << 4) | ((b >> 8) & 0xF));
                    clr[2] = (byte)((((b >> 4) & 0xF) << 4) | ((b >> 4) & 0xF));
                    clr[3] = (byte)((((b) & 0xF) << 4) | ((b) & 0xF));
                    break;
                case 4: // GX_RGBA6
                    b = (Reader.ReadByte() << 16) | (Reader.ReadByte() << 8) | (Reader.ReadByte());
                    clr[0] = (byte)((((b >> 18) & 0x3F) << 2) | (((b >> 18) & 0x3F) >> 4));
                    clr[1] = (byte)((((b >> 12) & 0x3F) << 2) | (((b >> 12) & 0x3F) >> 4));
                    clr[2] = (byte)((((b >> 6) & 0x3F) << 2) | (((b >> 6) & 0x3F) >> 4));
                    clr[3] = (byte)((((b) & 0x3F) << 2) | (((b) & 0x3F) >> 4));
                    break;
                case 5: // GX_RGBX888
                    clr[0] = Reader.ReadByte();
                    clr[1] = Reader.ReadByte();
                    clr[2] = Reader.ReadByte();
                    clr[3] = Reader.ReadByte();
                    break;
            }

            return clr;
        }
    }

    public class GXDisplayList
    {
        public List<GXPrimitiveGroup> Primitives = new List<GXPrimitiveGroup>();
        public GXDisplayList(byte[] Buffer, HSD_AttributeGroup Group)
        {
            HSDReader Reader = new HSDReader(new MemoryStream(Buffer));
            while(Reader.Position() < Buffer.Length)
            {
                GXPrimitiveGroup g = new GXPrimitiveGroup();
                if (!g.Read(Reader, Group))
                    break;
                Primitives.Add(g);
            }
            Reader.Close();
        }
    }
}
