using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.DAT;
using MeleeLib.IO;

namespace MeleeLib.GCX
{
    public class GXIndexGroup
    {
        public ushort[] Indices;
        public byte[] Clr;
    }

    public class GXDisplayList
    {
        public GXPrimitiveType PrimitiveType;
        public int Count;

        public GXIndexGroup[] Indices;

        public int GetSize(List<GXAttr> Attributes)
        {
            int Stride = 0;
            foreach (GXAttr att in Attributes)
            {
                switch (att.AttributeType)
                {
                    case GXAttribType.GX_DIRECT:
                        if (att.Name != GXAttribName.GX_VA_CLR0)
                            Stride += 1;
                        else
                            switch ((int)att.CompType)
                            {
                                case 0: // GX_RGB565
                                    Stride += 2;
                                    break;
                                case 1: // GX_RGB888
                                    Stride += 3;
                                    break;
                                case 2: // GX_RGBX888
                                    Stride += 11;
                                    break;
                                case 3: // GX_RGBA4
                                    Stride += 2;
                                    break;
                                case 4: // GX_RGBA6
                                    Stride += 3;
                                    break;
                                case 5: // GX_RGBX888
                                    Stride += 4;
                                    break;
                            }
                        break;
                    case GXAttribType.GX_INDEX8:
                        Stride += 1;
                        break;
                    case GXAttribType.GX_INDEX16:
                        Stride += 2;
                        break;
                    default:
                        break;
                }
            }
            return 3 + Stride * Indices.Length;
        }

        public void Deserialize(DATReader d, DATRoot root, List<GXAttr> Attributes)
        {
            PrimitiveType = (GXPrimitiveType)d.Byte();
            Count = d.Short();
            Indices = new GXIndexGroup[Count];
            for (int j = 0; j < Count; j++)
            {
                GXIndexGroup g = new GXIndexGroup();
                g.Indices = new ushort[Attributes.Count];
                int i = 0;
                Indices[j] = g;
                foreach (GXAttr att in Attributes)
                {
                    switch (att.AttributeType)
                    {
                        case GXAttribType.GX_DIRECT:
                            if (att.Name != GXAttribName.GX_VA_CLR0)
                                g.Indices[i] = d.Byte();
                            else
                                g.Clr = readGXClr(d, (int)att.CompType);
                            break;
                        case GXAttribType.GX_INDEX8:
                            g.Indices[i] = d.Byte();
                            break;
                        case GXAttribType.GX_INDEX16:
                            g.Indices[i] = (ushort)d.Short();
                            break;
                        default:
                            throw new Exception("New Att Type? " + att.AttributeType);
                            //break;
                    }
                    i++;
                }
            }
        }

        public static byte[] readGXClr(DATReader d, int type)
        {
            byte[] clr = new byte[] { 1, 1, 1, 1 };
            int b;

            switch (type)
            {
                case 0: // GX_RGB565
                    b = d.Short();
                    clr[0] = (byte)((((b >> 11) & 0x1F) << 3) | (((b >> 11) & 0x1F) >> 2));
                    clr[1] = (byte)((((b >> 5) & 0x3F) << 2) | (((b >> 5) & 0x3F) >> 4));
                    clr[2] = (byte)((((b) & 0x1F) << 3) | (((b) & 0x1F) >> 2));
                    break;
                case 1: // GX_RGB888
                    clr[0] = d.Byte();
                    clr[1] = d.Byte();
                    clr[2] = d.Byte();
                    break;
                case 2: // GX_RGBX888
                    clr[0] = d.Byte();
                    clr[1] = d.Byte();
                    clr[2] = d.Byte();
                    d.Skip(8);
                    break;
                case 3: // GX_RGBA4
                    b = d.Short();
                    clr[0] = (byte)((((b >> 12) & 0xF) << 4) | ((b >> 12) & 0xF));
                    clr[1] = (byte)((((b >> 8) & 0xF) << 4) | ((b >> 8) & 0xF));
                    clr[2] = (byte)((((b >> 4) & 0xF) << 4) | ((b >> 4) & 0xF));
                    clr[3] = (byte)((((b) & 0xF) << 4) | ((b) & 0xF));
                    break;
                case 4: // GX_RGBA6
                    b = d.Three();
                    clr[0] = (byte)((((b >> 18) & 0x3F) << 2) | (((b >> 18) & 0x3F) >> 4));
                    clr[1] = (byte)((((b >> 12) & 0x3F) << 2) | (((b >> 12) & 0x3F) >> 4));
                    clr[2] = (byte)((((b >> 6) & 0x3F) << 2) | (((b >> 6) & 0x3F) >> 4));
                    clr[3] = (byte)((((b) & 0x3F) << 2) | (((b) & 0x3F) >> 4));
                    break;
                case 5: // GX_RGBX888
                    clr[0] = d.Byte();
                    clr[1] = d.Byte();
                    clr[2] = d.Byte();
                    clr[3] = d.Byte();
                    break;
            }

            return clr;
        }

        public void Serialize(DATWriter o, List<GXAttr> Attributes)
        {
            o.AddObject(this);
            o.Byte((byte)PrimitiveType);
            o.Short((short)Indices.Length);

            foreach(GXIndexGroup ig in Indices)
            {
                int i = 0;
                foreach (GXAttr att in Attributes)
                {
                    switch (att.AttributeType)
                    {
                        case GXAttribType.GX_DIRECT:
                            if (att.Name != GXAttribName.GX_VA_CLR0)
                                o.Byte((byte)ig.Indices[i]);
                            else
                                writeGXClr(ig.Clr, o, att.CompType);
                            break;
                        case GXAttribType.GX_INDEX8:
                            o.Byte((byte)ig.Indices[i]);
                            break;
                        case GXAttribType.GX_INDEX16:
                            o.Short((short)ig.Indices[i]);
                            break;
                        default:
                            break;
                    }
                    i++;
                }
            }
        }

        public static void writeGXClr(byte[] clr, DATWriter d, GXCompType type)
        {
            switch ((int)type)
            {
                case 0: // GX_RGB565
                    d.Short((short)ClrTo565(clr));
                    break;
                case 1: // GX_RGB888
                    d.Byte(clr[0]);
                    d.Byte(clr[1]);
                    d.Byte(clr[2]);
                    break;
                case 2: // GX_RGBX888
                    d.Byte(clr[0]);
                    d.Byte(clr[1]);
                    d.Byte(clr[2]);
                    d.Byte(0);
                    break;
                case 3: // GX_RGBA4
                    short s = (short)((((clr[0] >> 4) & 0xF) << 12) | (((clr[1] >> 4) & 0xF) << 8) | (((clr[2] >> 4) & 0xF) << 4) | (((clr[3] >> 4) & 0xF)));
                    d.Short(s);
                    break;
                case 4: // GX_RGBA6
                   d.Three(((clr[0] >> 2) << 18) | ((clr[1] >> 2) << 12) | ((clr[2] >> 2) << 6) | (clr[3] >> 2));
                    break;
                case 5: // GX_RGBX888
                    d.Byte(clr[0]);
                    d.Byte(clr[1]);
                    d.Byte(clr[2]);
                    d.Byte(clr[3]);
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
