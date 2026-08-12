using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.IO.Splines;
using HSDRawViewer.Tools;
using System.Collections.Generic;

namespace HSDRawViewer.GUI.Plugins.GrTool.Converters
{
    public static class KdSplineIO
    {
        private static string FileFilter = @"OBJ (.obj)|*.obj";

        public static void ExportSplines(string fname, string[] splineNames, IEnumerable<KdSpline> splines)
        {
            var fp = FileIO.SaveFile(FileFilter, $"{fname}.obj");
            if (fp == null) return;
            ExportSplines(splines, splineNames, fp);
        }

        public static void ExportSplines(IEnumerable<KdSpline> splines, string[] splineNames, string filePath)
        {
            SplineObj obj = new SplineObj();

            int index = 0;
            foreach (var spline in splines)
            {
                int offset = obj.Vertices.Count;
                foreach (var v in spline.Points)
                {
                    obj.Vertices.Add(new HSDRaw.Common.HSD_Vector3(v.X, v.Y, v.Z));
                }

                var o = new SplineObjObject();
                if (splineNames != null && index >= 0 && index < splineNames.Length)
                {
                    o.Name = splineNames[index];
                }
                else
                {
                    o.Name = $"Spline_{index:D2}";
                }

                var l = new SplineObjLine();
                for (int i = 0; i < spline.Points.Count; i++)
                    l.Indices.Add(offset + i);
                o.Lines.Add(l);

                obj.Objects.Add(o);
                index++;
            }

            obj.Save(filePath);
        }

        public static IEnumerable<(string, KdSpline)> ImportSplines()
        {
            var f = FileIO.OpenFile(FileFilter);
            if (f == null)
                yield break;

            var obj = new SplineObj();
            obj.Open(f);

            foreach (var o in obj.Objects)
            {
                var spline = new KdSpline();

                foreach (int index in o.GetConnectedIndices())
                {
                    var v = obj.Vertices[index];

                    spline.Points.Add(new KdVector(
                        v.X,
                        v.Y,
                        v.Z));
                }

                if (spline.Points.Count > 1)
                {
                    spline.RebuildArcLengthData();
                    yield return (o.Name, spline);
                }
            }
        }
    }
}
