using HSDRaw;
using HSDRaw.AirRide.Gr.Data;
using HSDRawViewer.GUI.PropertyGrid;
using HSDRawViewer.Tools;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HSDRawViewer.IO.AirRide.DataFormat
{
    public class KdCourseSplineSetup
    {
        [DisplayName("Loop")]
        [Description("Whether the course loops back to its starting point.")]
        public bool Loop { get; set; }

        [TypeConverter(typeof(ListConverter<float>))]
        public List<float> UnusedParams { get; } = new List<float>();

        [Browsable(false)]

        public ObservableList<KdCourseSpline> Splines = new ObservableList<KdCourseSpline>();

        public void Load(KAR_grSplineSetup setup)
        {
            Loop = setup.Loop;

            var splines = setup.CourseSplineList.Splines.Array;
            var altPath = setup.SplineAltPathLookup.Array;
            var grouplookup = setup.SplineGroupLookup.Array;
            for (int i = 0; i < splines.Length; i++)
            {
                Splines.Add(new KdCourseSpline(splines[i], altPath[i].List, grouplookup[i].List));
            }
        }

        public KAR_grSplineSetup Save()
        {
            var keyGroups = Splines.SelectMany(e => e.KeyGroups).Distinct().OrderBy(e => e).ToArray();

            return new KAR_grSplineSetup()
            {
                Loop = Loop,
                KeyGroups = new KAR_grSplineLinkList() { Count = keyGroups.Length, List = new HSDIntArray() { Array = keyGroups } },
                SplineAltPathLookup = new HSDArrayAccessor<KAR_grSplineLinkList>()
                {
                    Array = Splines.Select(e => new KAR_grSplineLinkList()
                    {
                        Count = e.IDs.Count,
                        List = new HSDIntArray() { Array = e.IDs.ToArray() }
                    }).ToArray()
                },
                SplineGroupLookup = new HSDArrayAccessor<KAR_grSplineLinkList>()
                {
                    Array = Splines.Select(e => new KAR_grSplineLinkList()
                    {
                        Count = e.KeyGroups.Count,
                        List = new HSDIntArray() { Array = e.KeyGroups.ToArray() }
                    }).ToArray()
                },
                x1C = UnusedParams.Count > 0 ? new HSDFloatArray() { Array = UnusedParams.ToArray() } : null,
                CourseSplineList = new KAR_grSplineList()
                {
                    Count = Splines.Count,
                    Splines = new HSDFixedLengthPointerArrayAccessor<HSDRaw.Common.HSD_Spline>()
                    {
                        Array = Splines.Select(e => e.Spline.ToHsdSpline()).ToArray()
                    }
                }
            };
        }
    }
}
