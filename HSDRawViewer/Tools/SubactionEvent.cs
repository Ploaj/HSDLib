using HSDRaw;
using HSDRaw.Tools.Melee;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace HSDRawViewer.Tools
{
    [TypeConverter(typeof(CustomIntPropertyConverter))]
    public class CustomIntProperty
    {
        private int _bitcount;

        public long Value
        {
            get => _value;
            set
            {
                _value = value;

                if (Signed)
                {
                    _value = SignValue(_value);

                    var mask = (1L << (_bitcount - 1)) - 1L;

                    if (_value > mask)
                        _value = mask;

                    if (_value < -mask)
                        _value = -mask;
                }
                else
                {
                    var mask = (1L << _bitcount) - 1L;

                    if (_value > mask)
                        _value = mask;

                    if (_value < 0)
                        _value = 0;
                }
            }
        }

        private bool Signed;

        private bool Hex;

        private long _value;

        public CustomIntProperty(int bitcount, bool signed, bool hex)
        {
            if (bitcount > 32)
                bitcount = 32;
            _bitcount = bitcount;
            Signed = signed;
            Hex = hex;
        }

        public override string ToString()
        {
            return Hex ? "0x" + Value.ToString("X8") : Value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private long SignValue(long v)
        {
            var isSigned = ((v >> (_bitcount - 1)) & 0x1) == 1;

            if (isSigned)
            {
                var bitMask = 0;
                for (int j = 0; j < _bitcount; j++)
                    bitMask |= (1 << j);
                v = ~v;
                v = v & bitMask;
                v += 1;
                v *= -1;
            }

            return v;
        }

        public class CustomIntPropertyConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context,
                                                Type sourceType)
            {
                return sourceType == typeof(string);
            }

            public override object ConvertFrom(ITypeDescriptorContext context,
                CultureInfo culture, object value)
            {
                var v = context.PropertyDescriptor.GetValue(context.Instance);

                if (value is string str && v is CustomIntProperty i)
                {
                    if (i.Hex)
                    {
                        if (int.TryParse(str.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int val))
                            i.Value = val;
                    }
                    else
                    {
                        if (int.TryParse(str, out int val))
                            i.Value = val;
                    }
                }

                return v;
            }

            public override object ConvertTo(ITypeDescriptorContext context,
                CultureInfo culture, object value, Type destinationType)
            {
                if (value != null && destinationType == typeof(string))
                    return value.ToString();

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    [TypeConverter(typeof(CustomEnumPropertyConverter))]
    public class CustomEnumProperty 
    {
        private List<string> enums;

        public string SelectedEnum { get; set; }

        public CustomEnumProperty(string[] enums)
        {
            this.enums = new List<string>();
            this.enums.AddRange(enums);
        }

        public List<string> Enums
        {
            get { return enums; }
        }

        public int SelectedEnumIndex { get => enums.IndexOf(SelectedEnum); set => SelectedEnum = enums[value]; }

        public override string ToString()
        {
            return SelectedEnum;
        }

        public class CustomEnumPropertyConverter : TypeConverter
        {

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                var value = context.PropertyDescriptor.GetValue(context.Instance);

                if (value is CustomEnumProperty prop)
                    return new StandardValuesCollection(prop.Enums);

                return base.GetStandardValues(context);
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context,
                                                  System.Type sourceType)
            {
                if (sourceType == typeof(string))
                {
                    return true;
                }
                else
                    return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                var v = context.PropertyDescriptor.GetValue(context.Instance);

                if (v is CustomEnumProperty prop && value is string str)
                {
                    prop.SelectedEnum = str;
                    return prop;
                }

                return base.ConvertFrom(context, culture, value);
            }
        }
    }

    [TypeConverter(typeof(CustomPointerPropertyConverter))]
    public class CustomPointerProperty
    {
        public static List<Tuple<string, HSDStruct>> pointers = new List<Tuple<string, HSDStruct>>();

        public HSDStruct Pointer;

        public CustomPointerProperty()
        {
            Pointer = new HSDStruct();
        }

        public override string ToString()
        {
            return pointers.FirstOrDefault(e => e.Item2 == Pointer)?.Item1;
        }

        public class CustomPointerPropertyConverter : TypeConverter
        {

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                var value = context.PropertyDescriptor.GetValue(context.Instance);

                if (value is CustomPointerProperty prop)
                    return new StandardValuesCollection(CustomPointerProperty.pointers.Select(e => e.Item1).ToArray());

                return base.GetStandardValues(context);
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context,
                                                  System.Type sourceType)
            {
                if (sourceType == typeof(string))
                {
                    return true;
                }
                else
                    return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                var v = context.PropertyDescriptor.GetValue(context.Instance);

                if (v is CustomPointerProperty prop && value is string str)
                {
                    prop.Pointer = CustomPointerProperty.pointers.FirstOrDefault(e => e.Item1 == str)?.Item2;
                    return prop;
                }

                return base.ConvertFrom(context, culture, value);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SubactionEvent : CustomClass
    {
        public byte Code { get; internal set; } = 0;

        public SubactionGroup Type { get; internal set; } = SubactionGroup.Fighter;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetParameter(int i)
        {
            // load description and code byte
            var desc = SubactionManager.GetSubaction(Code, Type);

            var p = desc.Parameters[i];

            if (p.IsPointer)
            {
                return 0;
            }
            if (p.HasEnums)
            {
                return ((CustomEnumProperty)this[i].Value).SelectedEnumIndex;
            }
            else
            if (p.IsFloat)
            {
                return BitConverter.ToInt32(BitConverter.GetBytes((float)this[i].Value));
            }
            else
            {
                return (int)((CustomIntProperty)this[i].Value).Value;
            }
        }

        /// <summary>
        /// returns pointer to hsdstruct data if one exists
        /// returns null otherwise
        /// </summary>
        /// <returns></returns>
        public HSDStruct GetPointer()
        {
            var desc = SubactionManager.GetSubaction(Code, Type);

            for (int i = 0; i < desc.Parameters.Length; i++)
            {
                var p = desc.Parameters[i];

                if (p.IsPointer)
                {
                    return ((CustomPointerProperty)this[i].Value).Pointer;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public SubactionEvent(SubactionGroup type, byte code)
        {
            SetCode(type, code);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SubactionManager.GetSubaction(Code, Type).Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        public void SetCode(SubactionGroup type, byte code)
        {
            Type = type;
            Code = code;

            // get action
            var desc = SubactionManager.GetSubaction(code, Type);

            if (!desc.IsCustom)
                Code &= 0xFC;

            // clear current params
            Clear();

            // check if params are null
            if (desc.Parameters == null)
                return;

            // add param from action desciptor
            for (int i = 0; i < desc.Parameters.Length; i++)
            {
                var p = desc.Parameters[i];

                // p.IsPointer
                if (p.IsPointer)
                {
                    Add(p.Name, new CustomPointerProperty());
                }
                else
                if (p.HasEnums)
                {
                    Add(p.Name, new CustomEnumProperty(p.Enums));
                }
                else
                if (p.IsFloat)
                {
                    Add(p.Name, (float)0);
                }
                else
                {
                    Add(p.Name, new CustomIntProperty(p.BitCount, p.Signed, p.Hex));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void LoadCode(SubactionGroup type, byte[] cmdList, HSDStruct reference)
        {
            byte code = cmdList[0];

            // load description and code byte
            var desc = SubactionManager.GetSubaction(code, type);

            // setup code
            SetCode(type, code);

            // 
            if (desc.Parameters == null)
                return;

            //
            Bitreader r = new Bitreader(cmdList);
            r.Skip(desc.IsCustom ? 8 : 6);

            // load params from data
            for (int i = 0; i < desc.Parameters.Length; i++)
            {
                var p = desc.Parameters[i];

                int value = r.Read(p.BitCount);

                if (p.IsPointer)
                {
                    ((CustomPointerProperty)this[i].Value).Pointer = reference;
                }
                else
                if (p.HasEnums)
                {
                    ((CustomEnumProperty)this[i].Value).SelectedEnumIndex = value;
                }
                else
                if (p.IsFloat)
                {
                    this[i].Value = BitConverter.ToSingle(BitConverter.GetBytes(value));
                }
                else
                {
                    ((CustomIntProperty)this[i].Value).Value = value;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public HSDStruct CompileCode()
        {
            // load description and code byte
            var desc = SubactionManager.GetSubaction(Code, Type);

            // 
            if (desc.Parameters == null)
                return null;

            //
            BitWriter r = new BitWriter();

            // write code
            if (desc.IsCustom)
                r.Write(Code, 8);
            else
                r.Write(Code >> 2, 6);

            // load params from data
            HSDStruct pointer = null;
            for (int i = 0; i < desc.Parameters.Length; i++)
            {
                var p = desc.Parameters[i];

                if (p.IsPointer)
                {
                    pointer = ((CustomPointerProperty)this[i].Value).Pointer;
                    r.Write(0, p.BitCount);
                }
                else
                if (p.HasEnums)
                {
                    r.Write(((CustomEnumProperty)this[i].Value).SelectedEnumIndex, p.BitCount);
                }
                else
                if (p.IsFloat)
                {
                    r.Write(BitConverter.ToInt32(BitConverter.GetBytes((float)this[i].Value)), p.BitCount);
                }
                else
                {
                    r.Write(((CustomIntProperty)this[i].Value).Value, p.BitCount);
                }
            }

            // end of script
            r.Align();

            // create compiled struct
            var s = new HSDStruct(r.Bytes.ToArray());

            // add pointer if one exists
            if (pointer != null)
                s.SetReferenceStruct(0x04, pointer);

            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetParamsAsString()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return $"{this[i].Name}={this[i].Value}";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static IEnumerable<SubactionEvent> GetEvents(SubactionGroup type, HSDStruct str)
        {
            // get subaction data
            var data = str.GetData();

            // process data
            for (int i = 0; i < data.Length;)
            {
                // get subaction
                var sa = SubactionManager.GetSubaction((byte)(data[i]), type);

                // create new script node
                var sas = new SubactionEvent(type, data[i]);

                // check if data is out of range
                if (i + sa.ByteSize > data.Length)
                    break;

                // store any pointers within this subaction
                HSDStruct reference = null;
                foreach (var r in str.References)
                {
                    if (r.Key >= i && r.Key < i + sa.ByteSize)
                        if (reference != null)
                            throw new NotSupportedException("Multiple References not supported and unexpected");
                        else
                            reference = r.Value;
                }

                // copy subaction data to script node
                var sub = new byte[sa.ByteSize];
                for (int j = 0; j < sub.Length; j++)
                    sub[j] = data[i + j];

                // increment offset
                i += sa.ByteSize;

                // load param data
                sas.LoadCode(type, sub, reference);

                // return subaction event
                yield return sas;

                // if end of script then stop reading
                //if (sa.Code == 0)
                //    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        public static HSDStruct CompileEvent(IEnumerable<SubactionEvent> events)
        {
            HSDStruct s = new HSDStruct();

            foreach (var e in events)
                s.AppendStruct(e.CompileCode());

            return s;
        }
    }
}
