using HSDRaw.Tools;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.Tools.KeyFilters
{
    public class EulerFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tracks"></param>
        public static void Filter(List<FOBJ_Player> tracks)
        {
            var x = tracks.FirstOrDefault(e => e.JointTrackType == HSDRaw.Common.Animation.JointTrackType.HSD_A_J_ROTX);
            var y = tracks.FirstOrDefault(e => e.JointTrackType == HSDRaw.Common.Animation.JointTrackType.HSD_A_J_ROTY);
            var z = tracks.FirstOrDefault(e => e.JointTrackType == HSDRaw.Common.Animation.JointTrackType.HSD_A_J_ROTZ);

            if (x != null && y != null && z != null)
                Filter(x, y, z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void Filter(FOBJ_Player x, FOBJ_Player y, FOBJ_Player z)
        {
            if (x == null || y == null || z == null)
                return;

            if (x.Keys.Count <= 1 || y.Keys.Count <= 1 || z.Keys.Count <= 1)
                return;

            Vector3 prev = new Vector3(x.GetValue(0), y.GetValue(0), z.GetValue(0));

            List<FOBJKey> xfilter = new List<FOBJKey>();
            List<FOBJKey> yfilter = new List<FOBJKey>();
            List<FOBJKey> zfilter = new List<FOBJKey>();

            for (int i = 0; i < x.FrameCount; i++)
            {
                var e = new Vector3(
                    naive_flip_diff(prev.X, x.GetValue(i)),
                    naive_flip_diff(prev.Y, y.GetValue(i)),
                    naive_flip_diff(prev.Z, z.GetValue(i)));

                var fe = flip_euler(e);
                fe = new Vector3(
                    naive_flip_diff(prev.X, fe.X),
                    naive_flip_diff(prev.Y, fe.Y),
                    naive_flip_diff(prev.Z, fe.Z));

                var de = euler_distance(prev, e);
                var dfe = euler_distance(prev, fe);

                if (dfe < de)
                    e = fe;

                prev = e;

                xfilter.Add(new FOBJKey() { Frame = i, Value = e.X, InterpolationType = HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_LIN });
                yfilter.Add(new FOBJKey() { Frame = i, Value = e.Y, InterpolationType = HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_LIN });
                zfilter.Add(new FOBJKey() { Frame = i, Value = e.Z, InterpolationType = HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_LIN });
            }

            x.Keys = xfilter;
            y.Keys = yfilter;
            z.Keys = zfilter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        private static float naive_flip_diff(double a1, double a2)
        {
            const double twopi = 2 * Math.PI;
            while (Math.Abs(a1 - a2) > Math.PI)
            {
                if (a1 < a2)
                    a2 -= twopi;
                else
                    a2 += twopi;
            }
            return (float)a2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="euler"></param>
        /// <returns></returns>
        private static Vector3 flip_euler(Vector3 euler)
        {
            return new Vector3(euler.X + (float)Math.PI,
                euler.Y * -1 + (float)Math.PI,
                euler.Z + (float)Math.PI);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        private static float euler_distance(Vector3 e1, Vector3 e2)
        {
            return Math.Abs(e1[0] - e2[0]) + Math.Abs(e1[1] - e2[1]) + Math.Abs(e1[2] - e2[2]);
        }
    }
}
