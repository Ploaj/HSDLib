using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSDLib.Common;
using HSDLib.Helpers;
using System.IO;
using HSDLib.GX;

namespace HSDLib.Helpers
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
        public byte[] Clr0;
        public byte[] Clr1;
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
                            else if(att.Name == GXAttribName.GX_VA_CLR0)
                                g.Clr0 = ReadGXClr(Reader, (int)att.CompType);
                            else if (att.Name == GXAttribName.GX_VA_CLR1)
                                g.Clr1 = ReadGXClr(Reader, (int)att.CompType);
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

        public void Write(HSDWriter Writer, HSD_AttributeGroup Attributes)
        {
            Writer.Write((byte)PrimitiveType);
            Writer.Write((ushort)Indices.Length);
            foreach(GXIndexGroup ig in Indices)
            {
                GXIndexGroup g = ig;
                int i = 0;
                foreach (GXVertexBuffer att in Attributes.Attributes)
                {
                    switch (att.AttributeType)
                    {
                        case GXAttribType.GX_DIRECT:
                            if (att.Name != GXAttribName.GX_VA_CLR0)
                                Writer.Write((byte)g.Indices[i]);
                            else if (att.Name == GXAttribName.GX_VA_CLR0)
                                WriteGXClr(g.Clr0, Writer, att.CompType);
                            else if (att.Name == GXAttribName.GX_VA_CLR1)
                                WriteGXClr(g.Clr1, Writer, att.CompType);
                            break;
                        case GXAttribType.GX_INDEX8:
                            Writer.Write((byte)g.Indices[i]);
                            break;
                        case GXAttribType.GX_INDEX16:
                            Writer.Write(g.Indices[i]);
                            break;
                    }
                    i++;
                }
            }
        }

        public static void WriteGXClr(byte[] clr, HSDWriter d, GXCompType type)
        {
            switch ((int)type)
            {
                case 0: // GX_RGB565
                    d.Write((short)ClrTo565(clr));
                    break;
                case 1: // GX_RGB888
                    d.Write(clr[0]);
                    d.Write(clr[1]);
                    d.Write(clr[2]);
                    break;
                case 2: // GX_RGBX888
                    d.Write(clr[0]);
                    d.Write(clr[1]);
                    d.Write(clr[2]);
                    d.Write(0);
                    break;
                case 3: // GX_RGBA4
                    short s = (short)((((clr[0] >> 4) & 0xF) << 12) | (((clr[1] >> 4) & 0xF) << 8) | (((clr[2] >> 4) & 0xF) << 4) | (((clr[3] >> 4) & 0xF)));
                    d.Write((ushort)s);
                    break;
                case 4: // GX_RGBA6
                    int three = (((clr[0] >> 2) << 18) | ((clr[1] >> 2) << 12) | ((clr[2] >> 2) << 6) | (clr[3] >> 2));
                    d.Write((byte)((three >> 16)&0xFF));
                    d.Write((byte)((three >> 8) & 0xFF));
                    d.Write((byte)((three) & 0xFF));
                    break;
                case 5: // GX_RGBX888
                    d.Write(clr[0]);
                    d.Write(clr[1]);
                    d.Write(clr[2]);
                    d.Write(clr[3]);
                    break;
            }
        }

        private static ushort ClrTo565(byte[] color)
        {
            byte red = color[0];
            byte green = color[0];
            byte blue = color[0];

            int b = (blue >> 3) & 0x1f;
            int g = ((green >> 2) & 0x3f) << 5;
            int r = ((red >> 3) & 0x1f) << 11;

            return (ushort)(r | g | b);
        }
    }

    public class GXDisplayList
    {
        public List<GXPrimitiveGroup> Primitives = new List<GXPrimitiveGroup>();

        public GXDisplayList()
        {

        }

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

        public byte[] ToBuffer(HSD_AttributeGroup Group)
        {
            MemoryStream o = new MemoryStream();
            HSDWriter Writer = new HSDWriter(o);

            foreach(GXPrimitiveGroup g in Primitives)
            {
                g.Write(Writer, Group);
            }
            Writer.Write((byte)0);

            Writer.Align(0x20);

            Writer.Close();
            byte[] bytes = o.ToArray();
            o.Close();
            return bytes;
        }
    }
}
