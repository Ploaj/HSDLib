using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace HSDRawViewer.GUI.MEX
{
    public class MEXConverter
    {
        public static List<string> ssmValues { get; } = new List<string>();

        public static List<string> effectValues { get; } = new List<string>();

        public static List<string> musicIDValues { get; } = new List<string>();

        public static List<string> internalIDValues { get; } = new List<string>();

        public static List<string> stageIDValues { get; } = new List<string>();

        public static List<string> externalIDValues
        {
            get
            {
                List<string> external = new List<string>(internalIDValues.Count);
                external.AddRange(new string[internalIDValues.Count]);
                external[0] = internalIDValues[0];
                for (int i = 1; i < internalIDValues.Count; i++)
                {
                    var newid = MEXIdConverter.ToExternalID(i - 1, internalIDValues.Count - 1) + 1;
                    external[newid] = internalIDValues[i];
                }
                return external;
            }
        }
    }

    public class FighterIDConverter : TypeConverter
    {
        public virtual List<string> values { get; } = new List<string>();

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is int)
            {
                int index = (int)value + 1;

                if (index >= 0 && index < values.Count)
                    return values[index];

                return values[0]; // error, go back to first
            }
            return value;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string s = value as string;
            if (s != null)
            {
                int index = values.IndexOf(s);
                if (index >= 0)
                    return index - 1;

                if (int.TryParse(s, out index) && index >= 0 && index < values.Count)
                    return index;

                return -1;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(values);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    public class NullIDConverter : TypeConverter
    {
        public virtual List<string> values { get; } = new List<string>();

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is int)
            {
                int index = (int)value;

                if (index >= 0 && index < values.Count)
                    return values[index];

                return "None";
            }
            return value;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string s = value as string;
            if (s != null)
            {
                int index = values.IndexOf(s);

                return index;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var l = new List<string>();
            l.Add("None");
            l.AddRange(values);
            return new StandardValuesCollection(l);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    public class IDConverter : TypeConverter
    {
        public virtual List<string> values { get; } = new List<string>();

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is int)
            {
                int index = (int)value;

                if (index >= 0 && index < values.Count)
                    return values[index];

                return values[0]; // error, go back to first
            }
            return value;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string s = value as string;
            if (s != null)
            {
                int index = values.IndexOf(s);
                if (index >= 0)
                    return index;

                if (int.TryParse(s, out index) && index >= 0 && index < values.Count)
                    return index;

                return -1;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(values);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    public class FighterInternalIDConverter : FighterIDConverter
    {
        public override List<string> values { get => MEXConverter.internalIDValues; }
    }

    public class FighterExternalIDConverter : FighterIDConverter
    {
        public override List<string> values { get => MEXConverter.externalIDValues; }
    }

    public class SSMIDConverter : NullIDConverter
    {
        public override List<string> values { get => MEXConverter.ssmValues; }
    }

    public class MusicIDConverter : IDConverter
    {
        public override List<string> values { get => MEXConverter.musicIDValues; }
    }

    public class EffectIDConverter : NullIDConverter
    {
        public override List<string> values { get => MEXConverter.effectValues; }
    }

    public class StageInternalIDConverter : NullIDConverter
    {
        public override List<string> values { get => MEXConverter.stageIDValues; }
    }
}
