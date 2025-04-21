﻿using CSCore.CoreAudioAPI;
using HSDRaw;
using HSDRawViewer.GUI.Dialog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace HSDRawViewer
{
    public class ApplicationSettings
    {
        [Browsable(false),]
        public static Brush SystemWindowTextColorBrush = new SolidBrush(System.Drawing.SystemColors.WindowText);

        [Browsable(false),]
        public static Brush SystemWindowTextRedColorBrush = new SolidBrush(Color.Red);

        [Browsable(false),]
        public static Brush SystemGrayTextColorBrush = new SolidBrush(System.Drawing.SystemColors.GrayText);

        [Browsable(false),]
        public static bool UnlockedViewport { get; set; } = false;

        [Browsable(false)]
        public static string HSDFileFilter { get; } = "HSD DAT Archive File|*.dat;*.usd";

        [Browsable(false)]
        public static string ImageFileFilter { get; } = "Supported Image Formats|*.png;*.bmp;*.jpg;*.jpeg";

        [Browsable(false)]
        public static List<Type> HSDTypes { get; internal set; } = new List<Type>();

        [Browsable(false)]
        private static readonly List<MMDevice> AudioDevices = new();

        public static MMDevice DefaultDevice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool IsDark(Color c)
        {
            return c.GetBrightness() < 0.5f;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Init()
        {
            //SystemWindowTextRedColorBrush = new SolidBrush(IsDark(SystemColors.ControlText) ? ControlPaint.Light(Color.Red) : ControlPaint.Dark(Color.Red));
            //SystemWindowTextRedColorBrush = new SolidBrush(ControlPaint.Dark(Color.Red));

            List<Type> types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                from assemblyType in domainAssembly.GetTypes()
                                where typeof(HSDAccessor).IsAssignableFrom(assemblyType)
                                select assemblyType).ToList();

            HSDTypes.AddRange(types);

            using MMDeviceEnumerator mmdeviceEnumerator = new();
            using (MMDeviceCollection mmdeviceCollection = mmdeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
            {
                foreach (MMDevice device in mmdeviceCollection)
                {
                    AudioDevices.Add(device);
                }
            }
            DefaultDevice = mmdeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SelectAudioPlaybackDevice()
        {
            // TODO: save selected device
            DefaultDeviceSelecter c = new() { Device = DefaultDevice };
            using PropertyDialog d = new("Select Audio Device", c);
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DefaultDevice = c.Device;
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
