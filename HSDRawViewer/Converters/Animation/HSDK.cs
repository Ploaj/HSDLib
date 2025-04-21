using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using System;
using System.Collections.Generic;
using System.IO;

namespace HSDRawViewer.Converters.Animation
{
    public class HSDK
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        public static void ExportKeys(List<FOBJKey> keys)
        {
            string f = Tools.FileIO.SaveFile("HSD Keys (*.hsdk)|*.hsdk;*.txt", "keys.hsdk");

            if (f != null)
                ExportKeys(f, keys);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="keys"></param>
        public static void ExportKeys(string fileName, List<FOBJKey> keys)
        {
            using FileStream stream = new(fileName, FileMode.Create);
            using StreamWriter w = new(stream);
            foreach (FOBJKey k in keys)
            {
                w.Write($"{k.Frame} {k.Value} {k.InterpolationType.ToString().Replace("HSD_A_OP_", "")}");
                if (HasSlope(k.InterpolationType))
                    w.Write(" " + k.Tan);
                w.WriteLine();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<FOBJKey> LoadKeys()
        {
            string f = Tools.FileIO.OpenFile("HSD Keys (*.hsdk)|*.hsdk;*.txt", "");

            if (f != null)
                return LoadKeys(f);

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<FOBJKey> LoadKeys(string filePath)
        {
            List<FOBJKey> keys = new();

            string[] lines = File.ReadAllLines(filePath);

            foreach (string v in lines)
            {
                string[] a = v.Split(' ');

                if (a.Length >= 3)
                {
                    if (int.TryParse(a[0], out int frame)
                        && float.TryParse(a[1], out float value)
                        && Enum.TryParse("HSD_A_OP_" + a[2], out GXInterpolationType interpolation))
                    {
                        FOBJKey key = new()
                        {
                            Frame = frame,
                            Value = value,
                            InterpolationType = interpolation
                        };
                        if (HasSlope(interpolation))
                        {
                            if (a.Length >= 4 && float.TryParse(a[3], out float slope))
                                key.Tan = slope;
                        }
                        keys.Add(key);
                    }
                }
            }

            return keys;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool HasSlope(GXInterpolationType type)
        {
            switch (type)
            {
                case GXInterpolationType.HSD_A_OP_SLP:
                case GXInterpolationType.HSD_A_OP_SPL:
                    return true;
                default:
                    return false;
            }
        }

    }
}
