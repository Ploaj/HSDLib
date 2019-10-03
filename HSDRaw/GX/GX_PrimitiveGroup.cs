using System;

namespace HSDRaw.GX
{
    /// <summary>
    /// Index Groups are used by primitive groups to index vertex information
    /// </summary>
    public class GX_IndexGroup
    {
        public ushort[] Indices;
        public byte[] Clr0;
        public byte[] Clr1;
    }

    /// <summary>
    /// Primitive Groups describe how to render vertex strips
    /// </summary>
    public class GX_PrimitiveGroup
    {
        public GXPrimitiveType PrimitiveType;
        public GX_IndexGroup[] Indices;

        public int Count => Indices.Length;

        public bool Read(BinaryReaderExt Reader, GX_Attribute[] Attributes)
        {
            PrimitiveType = (GXPrimitiveType)Reader.ReadByte();
            if (PrimitiveType == 0)
                return false;
            var count = Reader.ReadUInt16();
            Indices = new GX_IndexGroup[count];
            for (int j = 0; j < count; j++)
            {
                GX_IndexGroup g = new GX_IndexGroup();
                g.Indices = new ushort[Attributes.Length];
                int i = 0;
                Indices[j] = g;
                foreach (var att in Attributes)
                {
                    if (att.AttributeName == GXAttribName.GX_VA_NULL)
                        continue;
                    switch (att.AttributeType)
                    {
                        case GXAttribType.GX_DIRECT:
                            if (att.AttributeName == GXAttribName.GX_VA_CLR0)
                                g.Clr0 = ReadGXClr(Reader, (int)att.CompType);
                            else if (att.AttributeName == GXAttribName.GX_VA_CLR1)
                                g.Clr1 = ReadGXClr(Reader, (int)att.CompType);
                            else
                                g.Indices[i] = Reader.ReadByte();
                            break;
                        case GXAttribType.GX_INDEX8:
                            g.Indices[i] = Reader.ReadByte();
                            break;
                        case GXAttribType.GX_INDEX16:
                            g.Indices[i] = Reader.ReadUInt16();
                            break;
                    }
                    i++;
                }
            }
            return true;
        }

        public void Write(BinaryWriterExt writer, GX_Attribute[] Attributes)
        {
            writer.Write((byte)PrimitiveType);
            writer.Write((ushort)Indices.Length);
            foreach (GX_IndexGroup ig in Indices)
            {
                GX_IndexGroup g = ig;
                int i = 0;
                foreach (var att in Attributes)
                {
                    if (att.AttributeName == GXAttribName.GX_VA_NULL)
                        continue;
                    switch (att.AttributeType)
                    {
                        case GXAttribType.GX_DIRECT:
                            if (att.AttributeName == GXAttribName.GX_VA_CLR0)
                                WriteGXClr(g.Clr0, writer, att.CompType);
                            else if (att.AttributeName == GXAttribName.GX_VA_CLR1)
                                WriteGXClr(g.Clr1, writer, att.CompType);
                            else
                                writer.Write((byte)g.Indices[i]);
                            break;
                        case GXAttribType.GX_INDEX8:
                            writer.Write((byte)g.Indices[i]);
                            break;
                        case GXAttribType.GX_INDEX16:
                            writer.Write(g.Indices[i]);
                            break;
                    }
                    i++;
                }
            }
        }

        private static byte[] ReadGXClr(BinaryReaderExt Reader, int CompType)
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
                default:
                    throw new Exception("Unknown Color Type");
            }

            return clr;
        }
        
        private static void WriteGXClr(byte[] clr, BinaryWriterExt d, GXCompType type)
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
                    d.Write((byte)((three >> 16) & 0xFF));
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
}
