using HSDRaw;
using HSDRaw.Common;
using HSDRawViewer.GUI.PropertyGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public class KdCourseSpline
    {
        [DisplayName("Spline Data")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public KdSpline Spline { get; set; } = new KdSpline();

        [DisplayName("Spline IDs")]
        [Description("The IDs associated with this spline. Most of the time are sequential, but alt paths will share id values with each other such as on GrSky2.")]
        [TypeConverter(typeof(ListConverter<int>))]
        public List<int> IDs { get; set; } = new List<int>();

        [DisplayName("Key Groups")]
        [Description("The Key Groups associated with this spline. Splines that share a group are considered on the same progression along the course. Higher values occur later in the track.")]
        [TypeConverter(typeof(ListConverter<int>))]
        public List<int> KeyGroups { get; set; } = new List<int>();

        public KdCourseSpline() { }

        public KdCourseSpline(HSD_Spline spline, HSDIntArray altPath, HSDIntArray keyGroupLookup)
        {
            Spline = new KdSpline(spline);
            this.IDs.AddRange(altPath.Array);
            this.KeyGroups.AddRange(keyGroupLookup.Array);
        }

        public override string ToString()
        {
            return $"K{Spline.Kind}_ID{string.Join("-", IDs)}_G{string.Join("-", KeyGroups)}";
        }

        public void FromString(string s)
        {
            var args = s.Split("_");

            foreach (var a in args)
            {
                if (a.StartsWith("K") && Enum.TryParse<KdSplineKind>(a[1..], out KdSplineKind k))
                {
                    Spline.Kind = k;
                }

                if (a.StartsWith("ID"))
                {
                    IDs.Clear();
                    foreach (var id in a[2..].Split("-"))
                    {
                        if (int.TryParse(id, out int value))
                        {
                            IDs.Add(value);
                        }
                    }
                }

                if (a.StartsWith("G"))
                {
                    KeyGroups.Clear();
                    foreach (var id in a[1..].Split("-"))
                    {
                        if (int.TryParse(id, out int value))
                        {
                            KeyGroups.Add(value);
                        }
                    }
                }
            }
        }
    }
}
