using HSDRaw.GX;
using System;
using System.Collections.Generic;
using System.IO;

namespace HSDRaw.Tools
{
    public class GX_AttributeCompressor
    {
        public static float Epsilon { get; } = 0.0001f;

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
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="values"></param>
        /// <exception cref="NotSupportedException"></exception>
        private static void OptimizeCompression(GX_Attribute attr, List<float[]> values)
        {
            if (attr.AttributeType == GXAttribType.GX_DIRECT)
                return;

            // Set component count based on attribute type
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
                    attr.CompCount = (attr.CompType == (GXCompType)GXCompTypeClr.RGBA4 ||
                                      attr.CompType == (GXCompType)GXCompTypeClr.RGBA6 ||
                                      attr.CompType == (GXCompType)GXCompTypeClr.RGBA8 ||
                                      attr.CompType == (GXCompType)GXCompTypeClr.RGBX8)
                                      ? GXCompCnt.ClrRGBA
                                      : GXCompCnt.ClrRGB;
                    break;
                default:
                    throw new NotSupportedException($"{attr.AttributeName} not supported for optimizing");
            }

            // Analyze values for max and sign
            double maxAbs = 0;
            bool hasNegative = false;
            foreach (float[] vec in values)
            {
                foreach (var v in vec)
                {
                    maxAbs = Math.Max(maxAbs, Math.Abs(v));
                    if (v < 0)
                        hasNegative = true;
                }
            }

            if (maxAbs == 0)
            {
                // Degenerate case — all zero values
                attr.CompType = GXCompType.UInt8;
                attr.Scale = 0;
                attr.AttributeType = GXAttribType.GX_INDEX8;
                attr.Stride = AttributeStride(attr);
                return;
            }

            // Find best scale
            byte bestByteScale = 1, bestSByteScale = 1, bestUShortScale = 1, bestShortScale = 1;
            byte scale = 1;

            while (scale < byte.MaxValue)
            {
                double scaled = maxAbs * (1 << scale);
                if (scaled <= byte.MaxValue)
                    bestByteScale = scale;
                if (scaled <= sbyte.MaxValue)
                    bestSByteScale = scale;
                if (scaled <= ushort.MaxValue)
                    bestUShortScale = scale;
                if (scaled <= short.MaxValue)
                    bestShortScale = scale;

                if (scaled >= ushort.MaxValue)
                    break;  // No point scaling further

                scale++;
            }

            // Helper to compute max quantization error
            double ComputeError<T>(int bits, byte scaleFactor, Func<double, T> castFunc) where T : struct
            {
                double factor = 1 << scaleFactor;
                double maxError = 0;
                double min = hasNegative ? -(Math.Pow(2, bits - 1)) : 0;
                double max = hasNegative ? (Math.Pow(2, bits - 1) - 1) : (Math.Pow(2, bits) - 1);

                foreach (float[] vec in values)
                {
                    foreach (var v in vec)
                    {
                        double scaledVal = v * factor;
                        scaledVal = Clamp(scaledVal, min, max);
                        double quantized = Convert.ToDouble(castFunc(scaledVal)) / factor;
                        maxError = Math.Max(maxError, Math.Abs(v - quantized));
                    }
                }

                return maxError;
            }

            double error;
            if (hasNegative)
            {
                error = ComputeError(8, bestSByteScale, x => (sbyte)Math.Round(x));
                if (error <= Epsilon)
                {
                    attr.CompType = GXCompType.Int8;
                    attr.Scale = bestSByteScale;
                }
                else
                {
                    error = ComputeError(16, bestShortScale, x => (short)Math.Round(x));
                    if (error <= Epsilon)
                    {
                        attr.CompType = GXCompType.Int16;
                        attr.Scale = bestShortScale;
                    }
                    else
                    {
                        attr.CompType = GXCompType.Float;
                        attr.Scale = 0;
                    }
                }
            }
            else
            {
                error = ComputeError(8, bestByteScale, x => (byte)Math.Round(x));
                if (error <= Epsilon)
                {
                    attr.CompType = GXCompType.UInt8;
                    attr.Scale = bestByteScale;
                }
                else
                {
                    error = ComputeError(16, bestUShortScale, x => (ushort)Math.Round(x));
                    if (error <= Epsilon)
                    {
                        attr.CompType = GXCompType.UInt16;
                        attr.Scale = bestUShortScale;
                    }
                    else
                    {
                        attr.CompType = GXCompType.Float;
                        attr.Scale = 0;
                    }
                }
            }

            // Set index type
            attr.AttributeType = values.Count > byte.MaxValue ? GXAttribType.GX_INDEX16 : GXAttribType.GX_INDEX8;

            // Calculate stride
            attr.Stride = AttributeStride(attr);

            System.Diagnostics.Debug.WriteLine($"{attr.AttributeName} {attr.CompType} scale={attr.Scale} error={error}");
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
                        case (GXCompType)GXCompTypeClr.RGBA4:
                        case (GXCompType)GXCompTypeClr.RGB565:
                            return 2;
                        case (GXCompType)GXCompTypeClr.RGB8:
                        case (GXCompType)GXCompTypeClr.RGBA6:
                            return 3;
                        case (GXCompType)GXCompTypeClr.RGBX8:
                        case (GXCompType)GXCompTypeClr.RGBA8:
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
                    Writer.Write((float)Scaled);
                    break;
                default:
                    Writer.Write((byte)Scaled);
                    break;
            }
        }


    }
}
