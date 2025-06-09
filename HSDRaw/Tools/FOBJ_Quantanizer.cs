using HSDRaw.Common.Animation;
using System;
using System.Collections.Generic;

namespace HSDRaw.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class FOBJQuantanizer
    {
        private float maxAbsValue = float.MinValue;
        private bool isSigned = false;
        private int scalePower = 0;
        private float scaleFactor = 1f;
        private GXAnimDataFormat dataFormat = GXAnimDataFormat.HSD_A_FRAC_FLOAT;
        private bool quantized = false;

        private List<float> values = new List<float>();

        public float Error { get; set; } = 0.0001f;

        public FOBJQuantanizer() { }

        public FOBJQuantanizer(float error)
        {
            Error = error;
        }

        public void AddValue(float value)
        {
            if (value < 0) isSigned = true;
            values.Add(value);
            maxAbsValue = Math.Max(Math.Abs(value), maxAbsValue);
            quantized = false;
        }

        private void Quantize()
        {
            quantized = true;
            scalePower = FindOptimalScale();
            scaleFactor = (float)Math.Pow(2, scalePower);
        }

        private int Clamp(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        private int QuantizeToInt(float value)
        {
            return (int)Math.Round(scaleFactor * value);
        }

        private bool IsAcceptable(GXAnimDataFormat format, int power)
        {
            float scale = (float)Math.Pow(2, power);

            foreach (var v in values)
            {
                int quantizedInt = (int)Math.Round(v * scale);

                switch (format)
                {
                    case GXAnimDataFormat.HSD_A_FRAC_U8:
                        if (quantizedInt < byte.MinValue || quantizedInt > byte.MaxValue)
                            return false;
                        break;
                    case GXAnimDataFormat.HSD_A_FRAC_S8:
                        if (quantizedInt < sbyte.MinValue || quantizedInt > sbyte.MaxValue)
                            return false;
                        break;
                    case GXAnimDataFormat.HSD_A_FRAC_U16:
                        if (quantizedInt < ushort.MinValue || quantizedInt > ushort.MaxValue)
                            return false;
                        break;
                    case GXAnimDataFormat.HSD_A_FRAC_S16:
                        if (quantizedInt < short.MinValue || quantizedInt > short.MaxValue)
                            return false;
                        break;
                }

                float reconstructed = quantizedInt / scale;
                if (Math.Abs(v - reconstructed) > Error)
                    return false;
            }

            return true;
        }

        private int FindOptimalScale()
        {
            // Try signed formats first
            if (isSigned)
            {
                dataFormat = GXAnimDataFormat.HSD_A_FRAC_S8;
                for (int i = 0; i < 31; i++)
                {
                    if (IsAcceptable(dataFormat, i))
                        return i;
                }

                dataFormat = GXAnimDataFormat.HSD_A_FRAC_S16;
                for (int i = 0; i < 31; i++)
                {
                    if (IsAcceptable(dataFormat, i))
                        return i;
                }
            }
            else
            {
                dataFormat = GXAnimDataFormat.HSD_A_FRAC_U8;
                for (int i = 0; i < 31; i++)
                {
                    if (IsAcceptable(dataFormat, i))
                        return i;
                }

                dataFormat = GXAnimDataFormat.HSD_A_FRAC_U16;
                for (int i = 0; i < 31; i++)
                {
                    if (IsAcceptable(dataFormat, i))
                        return i;
                }
            }

            dataFormat = GXAnimDataFormat.HSD_A_FRAC_FLOAT;
            return 0;
        }

        public uint GetValueScale()
        {
            if (!quantized)
                Quantize();

            return (uint)scaleFactor;
        }

        public GXAnimDataFormat GetDataFormat()
        {
            if (!quantized)
                Quantize();

            return dataFormat;
        }

        public void WriteValue(BinaryWriterExt d, float value)
        {
            if (!quantized)
                Quantize();

            int quantizedInt = QuantizeToInt(value);

            switch (dataFormat)
            {
                case GXAnimDataFormat.HSD_A_FRAC_FLOAT:
                    d.Write(value * scaleFactor);
                    break;
                case GXAnimDataFormat.HSD_A_FRAC_S16:
                    d.Write((short)Clamp(quantizedInt, short.MinValue, short.MaxValue));
                    break;
                case GXAnimDataFormat.HSD_A_FRAC_U16:
                    d.Write((ushort)Clamp(quantizedInt, ushort.MinValue, ushort.MaxValue));
                    break;
                case GXAnimDataFormat.HSD_A_FRAC_S8:
                    d.Write((sbyte)Clamp(quantizedInt, sbyte.MinValue, sbyte.MaxValue));
                    break;
                case GXAnimDataFormat.HSD_A_FRAC_U8:
                    d.Write((byte)Clamp(quantizedInt, byte.MinValue, byte.MaxValue));
                    break;
                default:
                    throw new Exception("Unknown GXAnimDataFormat " + dataFormat);
            }
        }
    }
}
