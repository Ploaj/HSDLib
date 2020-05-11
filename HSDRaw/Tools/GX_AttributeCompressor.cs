using HSDRaw.GX;
using System;
using System.Collections.Generic;
using System.IO;

namespace HSDRaw.Tools
{
    public class GX_AttributeCompressor
    {
        public static float Epsilon = 0.0001f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Attribute"></param>
        /// <param name="Values"></param>
        public static void GenerateBuffer(GX_Attribute Attribute, List<float[]> Values)
        {
            switch (Attribute.AttributeName)
            {
                case GXAttribName.GX_VA_PNMTXIDX:
                case GXAttribName.GX_VA_TEX0MTXIDX:
                case GXAttribName.GX_VA_TEX1MTXIDX:
                case GXAttribName.GX_VA_TEX2MTXIDX:
                case GXAttribName.GX_VA_TEX3MTXIDX:
                case GXAttribName.GX_VA_TEX4MTXIDX:
                case GXAttribName.GX_VA_TEX5MTXIDX:
                case GXAttribName.GX_VA_TEX6MTXIDX:
                case GXAttribName.GX_VA_TEX7MTXIDX:
                case GXAttribName.GX_VA_CLR0:
                case GXAttribName.GX_VA_CLR1:
                    Attribute.AttributeType = GXAttribType.GX_DIRECT;
                    break;
            }

            OptimizeCompression(Attribute, Values);

            MemoryStream o = new MemoryStream();

            using (BinaryWriterExt Writer = new BinaryWriterExt(o))
            {
                Writer.BigEndian = true;
                foreach (float[] ob in Values)
                {
                    foreach (var v in ob)
                    {
                        WriteData(Writer, v, Attribute.CompType, Attribute.Scale);
                    }
                }
                Writer.Align(0x20);
            }

            if (Attribute.Buffer == null)
                Attribute.Buffer = new HSDAccessor();

            Attribute.Buffer._s.SetData(o.ToArray());

            o.Dispose();
        }

        /// <summary>
        /// </summary>
        private static void OptimizeCompression(GX_Attribute attr, List<float[]> values)
        {
            // no need to optimize direct
            if (attr.AttributeType == GXAttribType.GX_DIRECT)
                return;

            switch (attr.AttributeName)
            {
                case GXAttribName.GX_VA_POS:
                    attr.CompCount = GXCompCnt.PosXYZ;
                    break;
                case GXAttribName.GX_VA_NRM:
                    attr.CompCount = GXCompCnt.NrmXYZ;
                    break;
                case GXAttribName.GX_VA_NBT:
                    attr.CompCount = GXCompCnt.NrmNBT;
                    break;
                case GXAttribName.GX_VA_TEX0:
                case GXAttribName.GX_VA_TEX1:
                case GXAttribName.GX_VA_TEX2:
                case GXAttribName.GX_VA_TEX3:
                case GXAttribName.GX_VA_TEX4:
                case GXAttribName.GX_VA_TEX5:
                case GXAttribName.GX_VA_TEX6:
                case GXAttribName.GX_VA_TEX7:
                    attr.CompCount = GXCompCnt.TexST;
                    break;
                case GXAttribName.GX_VA_CLR0:
                case GXAttribName.GX_VA_CLR1:
                    if (attr.CompType == GXCompType.RGBA4 ||
                        attr.CompType == GXCompType.RGBA6 ||
                        attr.CompType == GXCompType.RGBA8 ||
                        attr.CompType == GXCompType.RGBX8)
                        attr.CompCount = GXCompCnt.ClrRGBA;
                    else
                        attr.CompCount = GXCompCnt.ClrRGB;
                    break;
                default:
                    throw new NotSupportedException($"{attr.AttributeName} not supported for optimizing");
            }

            // get normalized value range
            double max = 0;
            bool signed = false;
            foreach (float[] v in values)
            {
                foreach(var val in v)
                {
                    max = Math.Max(max, Math.Abs(val));
                    if (val< 0)
                        signed = true;
                }
            }

            // get best scale for 8

            byte scale = 1;
            byte byteScale = 1;
            byte sbyteScale = 1;
            byte shortScale = 1;
            byte ushortScale = 1;

            while (max != 0 && max * Math.Pow(2, scale) < ushort.MaxValue && scale < byte.MaxValue)
            {
                var val = max * Math.Pow(2, scale);
                if (val < byte.MaxValue)
                    byteScale = scale;
                if (val < sbyte.MaxValue)
                    sbyteScale = scale;
                if (val < short.MaxValue)
                    shortScale = scale;
                if (val < ushort.MaxValue)
                    ushortScale = scale;

                scale++;
            }

            double error = 0;
            if (!signed)
                // byte or ushort
                error = (byte)(max * Math.Pow(2, byteScale)) / Math.Pow(2, byteScale);
            else
                // sbyte or short
                error = (sbyte)(max * Math.Pow(2, sbyteScale)) / Math.Pow(2, sbyteScale);


            if (Math.Abs(max - error) < Epsilon)
            {
                if (signed)
                {
                    attr.CompType = GXCompType.Int8;
                    attr.Scale = sbyteScale;
                }
                else
                {
                    attr.CompType = GXCompType.UInt8;
                    attr.Scale = byteScale;
                }
            }
            else
            {
                if (signed)
                {
                    attr.CompType = GXCompType.Int16;
                    attr.Scale = shortScale;
                }
                else
                {
                    attr.CompType = GXCompType.UInt16;
                    attr.Scale = ushortScale;
                }
            }

            // set index type
            attr.AttributeType = GXAttribType.GX_INDEX8;
            if (values.Count > byte.MaxValue)
                attr.AttributeType = GXAttribType.GX_INDEX16;
            
            // calculate stride
            attr.Stride = AttributeStride(attr);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private static short AttributeStride(GX_Attribute attribute)
        {
            return (short)(CompTypeToInt(attribute.AttributeName, attribute.CompType) * CompCountToInt(attribute.AttributeName, attribute.CompCount));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static int CompTypeToInt(GXAttribName name, GXCompType type)
        {
            switch (name)
            {
                case GXAttribName.GX_VA_CLR0:
                case GXAttribName.GX_VA_CLR1:
                    switch (type)
                    {
                        case GXCompType.RGBA4:
                        case GXCompType.RGB565:
                            return 2;
                        case GXCompType.RGB8:
                        case GXCompType.RGBA6:
                            return 3;
                        case GXCompType.RGBX8:
                        case GXCompType.RGBA8:
                            return 4;
                        default:
                            return 0;
                    }
                default:
                    switch (type)
                    {
                        case GXCompType.Int8:
                        case GXCompType.UInt8:
                            return 1;
                        case GXCompType.Int16:
                        case GXCompType.UInt16:
                            return 2;
                        case GXCompType.Float:
                            return 4;
                        default:
                            return 0;
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cc"></param>
        /// <returns></returns>
        private static int CompCountToInt(GXAttribName name, GXCompCnt cc)
        {
            switch (name)
            {
                case GXAttribName.GX_VA_POS:
                    switch (cc)
                    {
                        case GXCompCnt.PosXY:
                            return 2;
                        case GXCompCnt.PosXYZ:
                            return 3;
                    }
                    break;
                case GXAttribName.GX_VA_NRM:
                    switch (cc)
                    {
                        case GXCompCnt.NrmXYZ:
                        case GXCompCnt.NrmNBT:
                            return 3;
                        case GXCompCnt.NrmNBT3: // ??
                            return 3;
                    }
                    break;
                case GXAttribName.GX_VA_NBT:
                    switch (cc)
                    {
                        case GXCompCnt.NrmNBT:
                            return 9;
                        case GXCompCnt.NrmNBT3:
                            return 9;
                    }
                    break;
                case GXAttribName.GX_VA_TEX0:
                case GXAttribName.GX_VA_TEX1:
                case GXAttribName.GX_VA_TEX2:
                case GXAttribName.GX_VA_TEX3:
                case GXAttribName.GX_VA_TEX4:
                case GXAttribName.GX_VA_TEX5:
                case GXAttribName.GX_VA_TEX6:
                case GXAttribName.GX_VA_TEX7:
                    switch (cc)
                    {
                        case GXCompCnt.TexST:
                            return 2;
                        case GXCompCnt.TexS:
                            return 1;
                    }
                    break;
                case GXAttribName.GX_VA_CLR0:
                case GXAttribName.GX_VA_CLR1:
                    if(cc == GXCompCnt.ClrRGBA)
                    {
                        return 4;
                    }
                    if (cc == GXCompCnt.ClrRGB)
                    {
                        return 3;
                    }
                    break;
            }

            return 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Writer"></param>
        /// <param name="Value"></param>
        /// <param name="Type"></param>
        /// <param name="Scale"></param>
        private static void WriteData(BinaryWriterExt Writer, float Value, GXCompType Type, float Scale)
        {
            double Scaled = Value * Math.Pow(2, Scale);
            switch (Type)
            {
                case GXCompType.UInt8:
                    Writer.Write((byte)Scaled);
                    break;
                case GXCompType.Int8:
                    Writer.Write((sbyte)Scaled);
                    break;
                case GXCompType.UInt16:
                    Writer.Write((ushort)Scaled);
                    break;
                case GXCompType.Int16:
                    Writer.Write((short)Scaled);
                    break;
                case GXCompType.Float:
                    Writer.Write(Scaled);
                    break;
                default:
                    Writer.Write((byte)Scaled);
                    break;
            }
        }


    }
}
