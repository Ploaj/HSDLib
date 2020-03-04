using CSCore.CoreAudioAPI;
using HSDRaw;
using HSDRawViewer.GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace HSDRawViewer
{
    public class ApplicationSettings
    {
        [Browsable(false), ]
        public static bool UnlockedViewport { get; set; } = false;

        [Browsable(false)]
        public static string HSDFileFilter { get; } = "HSD DAT Archive File|*.dat;*.usd";

        [Browsable(false)]
        public static List<Type> HSDTypes { get; internal set; } = new List<Type>();

        [Browsable(false)]
        private static List<MMDevice> AudioDevices = new List<MMDevice>();
        
        public static MMDevice DefaultDevice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static void Init()
        {
            List<Type> types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                from assemblyType in domainAssembly.GetTypes()
                                where typeof(HSDAccessor).IsAssignableFrom(assemblyType)
                                select assemblyType).ToList();

            HSDTypes.AddRange(types);

            using (var mmdeviceEnumerator = new MMDeviceEnumerator())
            {
                using (var mmdeviceCollection = mmdeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
                {
                    foreach (var device in mmdeviceCollection)
                    {
                        AudioDevices.Add(device);
                    }
                }
            }

            DefaultDevice = AudioDevices[0];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SelectAudioPlaybackDevice()
        {
            // TODO: save selected device
            var c = new DefaultDeviceSelecter() { Device = DefaultDevice };
            using (PropertyDialog d = new PropertyDialog("Select Audio Device", c))
            {
                if(d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    DefaultDevice = c.Device;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class DefaultDeviceSelecter
        {
            [TypeConverter(typeof(DefaultDeviceConverter))]
            public MMDevice Device { get; set; }
        }
        /// <summary>
        /// 
        /// </summary>
        public class DefaultDeviceConverter : TypeConverter
        {

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(AudioDevices);
            }

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
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
            
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value is string)
                {
                    MMDevice item = AudioDevices.FirstOrDefault(q => (string.Compare(q.ToString(), value.ToString(), true) == 0));
                    return item;
                }
                else
                    return base.ConvertFrom(context, culture, value);
            }
        }
    }


}
