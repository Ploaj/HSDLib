using HSDRaw.Tools;
using HSDRawViewer.Rendering;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.Tools.KeyFilters
{
    public class DiscontinuityFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tracks"></param>
        public static bool Filter(List<FOBJ_Player> tracks)
        {
            bool filtered = false;
            var x = tracks.FirstOrDefault(e => e.JointTrackType == HSDRaw.Common.Animation.JointTrackType.HSD_A_J_ROTX);
            var y = tracks.FirstOrDefault(e => e.JointTrackType == HSDRaw.Common.Animation.JointTrackType.HSD_A_J_ROTY);
            var z = tracks.FirstOrDefault(e => e.JointTrackType == HSDRaw.Common.Animation.JointTrackType.HSD_A_J_ROTZ);

            if (x != null && y != null && z != null)
            {
                List<Quaternion> newKeys = new List<Quaternion>();
                Quaternion prev = Math3D.EulerToQuat(x.GetValue(0), y.GetValue(0), z.GetValue(0));
                newKeys.Add(prev);
                for (int i = 1; i <= x.FrameCount; i++)
                {
                    Quaternion curr = Math3D.EulerToQuat(x.GetValue(i), y.GetValue(i), z.GetValue(i));

                    // Flip quaternion if it suddenly inverts
                    if (prev.Dot(curr) < 0)
                    {
                        curr *= -1;
                        filtered = true;
                    }

                    newKeys.Add(curr);
                    prev = curr;
                }

                x.Keys.Clear();
                y.Keys.Clear();
                z.Keys.Clear();

                for (int i = 0; i < newKeys.Count; i++)
                {
                    var eul = Matrix4.CreateFromQuaternion(newKeys[i]).ExtractRotationEuler();
                    x.Keys.Add(new FOBJKey() { Frame = i, Value = eul.X, InterpolationType = HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_LIN });
                    y.Keys.Add(new FOBJKey() { Frame = i, Value = eul.Y, InterpolationType = HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_LIN });
                    z.Keys.Add(new FOBJKey() { Frame = i, Value = eul.Z, InterpolationType = HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_LIN });
                }

                x.AxisFilter();
                y.AxisFilter();
                z.AxisFilter();
            }

            return filtered;
        }
    }
}
