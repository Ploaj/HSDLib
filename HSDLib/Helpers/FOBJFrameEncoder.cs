using System;
using System.Collections.Generic;
using System.IO;
using HSDLib.Animation;

namespace HSDLib.Helpers
{
    public class FOBJQuantanizer
    {
        private float MaxValue = float.MinValue;
        private bool ValueSigned = false;
        private int ValueScale = 0;
        private GXAnimDataFormat ValueType = GXAnimDataFormat.Float;
        private bool Quantanized = false;
        public float Error { get; set; } = 0.0001f;
        private List<float> Values = new List<float>();

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
                case GXAnimDataFormat.Byte:
                    return Clamp((byte)(scale * value), byte.MinValue, byte.MaxValue);
                case GXAnimDataFormat.SByte:
                    return Clamp((sbyte)(scale * value), sbyte.MinValue, sbyte.MaxValue);
                case GXAnimDataFormat.Short:
                    return Clamp((short)(scale * value), short.MinValue, short.MaxValue);
                case GXAnimDataFormat.UShort:
                    return Clamp((ushort)(scale * value), ushort.MinValue, ushort.MaxValue);
            }
            return 0;
        }

        private bool IsAcceptable(GXAnimDataFormat format, int scale)
        {
            foreach(var v in Values)
            {
                var value = Math.Pow(2, scale) * v;
                switch (format)
                {
                    case GXAnimDataFormat.Byte:
                        if (v > byte.MaxValue)
                            return false;
                        break;
                    case GXAnimDataFormat.SByte:
                        if (v > sbyte.MaxValue)
                            return false;
                        break;
                    case GXAnimDataFormat.Short:
                        if (v > short.MaxValue)
                            return false;
                        break;
                    case GXAnimDataFormat.UShort:
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
                ValueType = GXAnimDataFormat.SByte;
                for (int i = 0; i < 0x1F; i++)
                {
                    if(IsAcceptable(ValueType, i))
                        return (int)Math.Pow(2, i);
                }
                ValueType = GXAnimDataFormat.Short;
                for (int i = 0; i < 0x1F; i++)
                {
                    if (IsAcceptable(ValueType, i))
                        return (int)Math.Pow(2, i);
                }
            }
            else
            {
                ValueType = GXAnimDataFormat.Byte;
                for (int i = 0; i < 0x1F; i++)
                {
                    if (IsAcceptable(ValueType, i))
                        return (int)Math.Pow(2, i);
                }
                ValueType = GXAnimDataFormat.UShort;
                for (int i = 0; i < 0x1F; i++)
                {
                    if (IsAcceptable(ValueType, i))
                        return (int)Math.Pow(2, i);
                }
            }
            ValueType = GXAnimDataFormat.Float;
            return (int)Math.Pow(2, 1);
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
        public void WriteValue(HSDWriter d, float Value)
        {
            if (!Quantanized)
                Quantanize();
            
            switch (ValueType)
            {
                case GXAnimDataFormat.Float:
                    d.Write(Value);
                    break;
                case GXAnimDataFormat.Short:
                    d.Write((short)(Quantanize(Value, ValueType, ValueScale)));
                    break;
                case GXAnimDataFormat.UShort:
                    d.Write((ushort)(Quantanize(Value, ValueType, ValueScale)));
                    break;
                case GXAnimDataFormat.SByte:
                    d.Write((sbyte)(Quantanize(Value, ValueType, ValueScale)));
                    break;
                case GXAnimDataFormat.Byte:
                    d.Write((byte)(Quantanize(Value, ValueType, ValueScale)));
                    break;
                default:
                    throw new Exception("Unknown GXAnimDataFormat " + ValueType.ToString());
            }
        }
    }

    public class FOBJFrameEncoder
    {
        public static HSD_FOBJ EncodeFrames(List<FOBJKey> Keys, byte TrackType)
        {
            HSD_FOBJ fobj = new HSD_FOBJ();
            fobj.AnimationType = TrackType;

            // perform quantization
            FOBJQuantanizer valueQ = new FOBJQuantanizer();
            FOBJQuantanizer tangentQ = new FOBJQuantanizer();

            foreach (FOBJKey key in Keys)
            {
                valueQ.AddValue(key.Value);
                tangentQ.AddValue(key.Tan);
            }

            fobj.ValueScale = valueQ.GetValueScale();
            fobj.ValueFormat = valueQ.GetDataFormat();

            fobj.TanScale = tangentQ.GetValueScale();
            fobj.TanFormat = tangentQ.GetDataFormat();

            MemoryStream o = new MemoryStream();
            using (HSDWriter Writer = new HSDWriter(o))
            {
                Writer.BigEndian = false;

                int time = 0;
                for (int i = 0; i < Keys.Count;)
                {
                    InterpolationType ip = Keys[i].InterpolationType;
                    int j;
                    for (j = 0; j < Keys.Count - i; j++)
                    {
                        if (Keys[i + j].InterpolationType != ip)
                            break;
                    }

                    int flag = ((j - 1) << 4) | (int)ip;
                    Writer.ExtendedByte(flag);

                    for (int k = i; k < i + j; k++)
                    {
                        int DeltaTime = 0;

                        if (k + 1 < Keys.Count)
                            DeltaTime = (int)(Keys[k + 1].Frame - Keys[k].Frame);

                        if (k == Keys.Count)
                            DeltaTime = 1;

                        switch (ip)
                        {
                            case InterpolationType.Step:
                                valueQ.WriteValue(Writer, Keys[k].Value);
                                Writer.ExtendedByte(DeltaTime);
                                break;
                            case InterpolationType.Linear:
                                valueQ.WriteValue(Writer, Keys[k].Value);
                                Writer.ExtendedByte(DeltaTime);
                                break;
                            case InterpolationType.HermiteValue:
                                valueQ.WriteValue(Writer, Keys[k].Value);
                                Writer.ExtendedByte(DeltaTime);
                                break;
                            case InterpolationType.Hermite:
                                valueQ.WriteValue(Writer, Keys[k].Value);
                                tangentQ.WriteValue(Writer, Keys[k].Tan);
                                Writer.ExtendedByte(DeltaTime);
                                break;
                            case InterpolationType.HermiteCurve:
                                tangentQ.WriteValue(Writer, Keys[k].Tan);
                                break;
                            case InterpolationType.Constant:
                                valueQ.WriteValue(Writer, Keys[k].Value);
                                break;
                            default:
                                throw new Exception("end");
                        }

                        if (ip != InterpolationType.HermiteCurve)
                            time = (int)Keys[k].Frame;
                    }

                    i += j;
                }
            }
            fobj.Data = o.ToArray();
            o.Close();
            o.Dispose();
            return fobj;
        }

    }
}
