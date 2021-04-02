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
        private float MaxValue = float.MinValue;
        private bool ValueSigned = false;
        private int ValueScale = 0;
        private GXAnimDataFormat ValueType = GXAnimDataFormat.HSD_A_FRAC_FLOAT;
        private bool Quantanized = false;
        public float Error { get; set; } = 0.0001f;
        private List<float> Values = new List<float>();

        public FOBJQuantanizer()
        {

        }

        public FOBJQuantanizer(float error)
        {
            Error = error;
        }

        public void AddValue(float value)
        {
            if (value < 0) ValueSigned = true;
            Values.Add(value);
            MaxValue = Math.Max(Math.Abs(value), MaxValue);
            Quantanized = false;
        }

        private void Quantanize()
        {
            Quantanized = true;
            ValueScale = QuantizeScaler();
        }

        private int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        private int Quantanize(float value, GXAnimDataFormat format, int scale)
        {
            switch (format)
            {
                case GXAnimDataFormat.HSD_A_FRAC_U8:
                    return Clamp((byte)(scale * value), byte.MinValue, byte.MaxValue);
                case GXAnimDataFormat.HSD_A_FRAC_S8:
                    return Clamp((sbyte)(scale * value), sbyte.MinValue, sbyte.MaxValue);
                case GXAnimDataFormat.HSD_A_FRAC_S16:
                    return Clamp((short)(scale * value), short.MinValue, short.MaxValue);
                case GXAnimDataFormat.HSD_A_FRAC_U16:
                    return Clamp((ushort)(scale * value), ushort.MinValue, ushort.MaxValue);
            }
            return 0;
        }

        private bool IsAcceptable(GXAnimDataFormat format, int scale)
        {
            foreach (var v in Values)
            {
                var value = Math.Pow(2, scale) * v;
                switch (format)
                {
                    case GXAnimDataFormat.HSD_A_FRAC_U8:
                        if (v > byte.MaxValue)
                            return false;
                        break;
                    case GXAnimDataFormat.HSD_A_FRAC_S8:
                        if (v > sbyte.MaxValue)
                            return false;
                        break;
                    case GXAnimDataFormat.HSD_A_FRAC_S16:
                        if (v > short.MaxValue)
                            return false;
                        break;
                    case GXAnimDataFormat.HSD_A_FRAC_U16:
                        if (v > ushort.MaxValue)
                            return false;
                        break;
                }
                float Estimated = (float)(Quantanize(v, format, (int)Math.Pow(2, scale)) / Math.Pow(2, scale));

                if (Math.Abs(v - Estimated) > Error)
                    return false;
            }

            return true;
        }

        private int QuantizeScaler()
        {
            if (ValueSigned)
            {
                ValueType = GXAnimDataFormat.HSD_A_FRAC_S8;
                for (int i = 0; i < 0x1F; i++)
                {
                    if (IsAcceptable(ValueType, i))
                        return (int)Math.Pow(2, i);
                }
                ValueType = GXAnimDataFormat.HSD_A_FRAC_S16;
                for (int i = 0; i < 0x1F; i++)
                {
                    if (IsAcceptable(ValueType, i))
                        return (int)Math.Pow(2, i);
                }
            }
            else
            {
                ValueType = GXAnimDataFormat.HSD_A_FRAC_U8;
                for (int i = 0; i < 0x1F; i++)
                {
                    if (IsAcceptable(ValueType, i))
                        return (int)Math.Pow(2, i);
                }
                ValueType = GXAnimDataFormat.HSD_A_FRAC_U16;
                for (int i = 0; i < 0x1F; i++)
                {
                    if (IsAcceptable(ValueType, i))
                        return (int)Math.Pow(2, i);
                }
            }
            ValueType = GXAnimDataFormat.HSD_A_FRAC_FLOAT;
            return (int)Math.Pow(2, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public uint GetValueScale()
        {
            if (!Quantanized)
                Quantanize();

            return (uint)ValueScale;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GXAnimDataFormat GetDataFormat()
        {
            if (!Quantanized)
                Quantanize();

            return ValueType;
        }

        /// <summary>
        /// writes quantanized value to stream
        /// </summary>
        /// <param name="d"></param>
        /// <param name="Value"></param>
        public void WriteValue(BinaryWriterExt d, float Value)
        {
            if (!Quantanized)
                Quantanize();

            switch (ValueType)
            {
                case GXAnimDataFormat.HSD_A_FRAC_FLOAT:
                    d.Write(Value * ValueScale);
                    break;
                case GXAnimDataFormat.HSD_A_FRAC_S16:
                    d.Write((short)(Quantanize(Value, ValueType, ValueScale)));
                    break;
                case GXAnimDataFormat.HSD_A_FRAC_U16:
                    d.Write((ushort)(Quantanize(Value, ValueType, ValueScale)));
                    break;
                case GXAnimDataFormat.HSD_A_FRAC_S8:
                    d.Write((sbyte)(Quantanize(Value, ValueType, ValueScale)));
                    break;
                case GXAnimDataFormat.HSD_A_FRAC_U8:
                    d.Write((byte)(Quantanize(Value, ValueType, ValueScale)));
                    break;
                default:
                    throw new Exception("Unknown GXAnimDataFormat " + ValueType.ToString());
            }
        }
    }
}
