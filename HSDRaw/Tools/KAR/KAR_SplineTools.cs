using HSDRaw.Common;

namespace HSDRaw.Tools.KAR
{
    public class KAR_SplineTools
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spline"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void CreateRangeSpline(HSD_Spline spline, out HSD_Spline left, out HSD_Spline right)
        {
            left = new HSD_Spline()
            {
                Tension = spline.Tension,
                Lengths = spline.Lengths,
                TotalLength = spline.TotalLength,
            };
            right = new HSD_Spline()
            {
                Tension = spline.Tension,
                Lengths = spline.Lengths,
                TotalLength = spline.TotalLength,
            };

            var points = spline.Points;
            HSD_Vector3[] lp = new HSD_Vector3[points.Length];
            HSD_Vector3[] rp = new HSD_Vector3[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                var p = new HSD_Vector3(points[i].X, points[i].Y, points[i].Z);
                var nrm = new HSD_Vector3();

                if (i < points.Length - 1)
                {
                    var norm1 = new HSD_Vector3(-(points[i + 1].Z - points[i].Z), (points[i + 1].X - points[i].X), 0);
                    norm1.Normalize();
                    nrm = new HSD_Vector3(norm1.X, 0, norm1.Y);
                }

                if (i > 0)
                {
                    var norm1 = new HSD_Vector3(-(points[i].Z - points[i - 1].Z), (points[i].X - points[i - 1].X), 0);
                    norm1.Normalize();

                    if (nrm.X == 0 && nrm.Y == 0 && nrm.Z == 0)
                        nrm = new HSD_Vector3(norm1.X, 0, norm1.Y);
                    else
                        nrm = (nrm + new HSD_Vector3(norm1.X, 0, norm1.Y)) / 2;
                }

                nrm *= 10;

                var l = p + nrm;
                var r = p - nrm;

                lp[i] = new HSD_Vector3() { X = l.X, Y = l.Y, Z = l.Z };
                rp[i] = new HSD_Vector3() { X = r.X, Y = r.Y, Z = r.Z };
            }

            left.Points = lp;
            right.Points = rp;
        }
    }
}
