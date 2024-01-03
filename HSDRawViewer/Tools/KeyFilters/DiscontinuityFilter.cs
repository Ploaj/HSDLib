using HSDRaw.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.Tools.KeyFilters
{
    public class DiscontinuityFilter
    {
        private const float Threshold = 180.0f; // Threshold for discontinuity check in degrees
        private const float Alpha = 1f; // 0.98f; // Filter coefficient

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
            {
                Filter(x);
                Filter(y);
                Filter(z);
            }
        }

        public static void Filter(FOBJ_Player track)
        {
            List<FOBJKey> newkeys = new List<FOBJKey>();

            var prev = track.GetValue(0);
            for (int i = 0; i < track.FrameCount; i++)
            {
                var v = track.GetValue(i);

                float delta = v - prev;
                if (Math.Abs(delta) > Threshold * Math.PI / 180.0f)
                {
                    if (delta > 0)
                    {
                        v -= 2 * (float)Math.PI;
                    }
                    else
                    {
                        v += 2 * (float)Math.PI;
                    }
                }

                float filteredYaw = v;// + (1 - Alpha) * yaw.GetValue(i);

                prev = filteredYaw;

                // System.Diagnostics.Debug.WriteLine($"Filtered Yaw[{i}]: {filteredYaw}, Filtered Pitch[{i}]: {filteredPitch}, Filtered Roll[{i}]: {filteredRoll}");

                newkeys.Add(new FOBJKey()
                {
                    Frame = i,
                    InterpolationType = HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_LIN,
                    Value = filteredYaw,
                });
            }

            track.Keys = newkeys;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="yaw"></param>
        /// <param name="pitch"></param>
        /// <param name="roll"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void Filter(FOBJ_Player yaw, FOBJ_Player pitch, FOBJ_Player roll)
        {
            if (yaw.FrameCount != pitch.FrameCount || pitch.FrameCount != roll.FrameCount)
            {
                throw new ArgumentException("Input arrays must have the same length.");
            }

            List<FOBJKey> xfilter = new List<FOBJKey>();
            List<FOBJKey> yfilter = new List<FOBJKey>();
            List<FOBJKey> zfilter = new List<FOBJKey>();

            var prevYaw = yaw.GetValue(0);
            var prevPitch = pitch.GetValue(0);
            var prevRoll = roll.GetValue(0);

            for (int i = 0; i < yaw.FrameCount; i++)
            {
                float deltaYaw = NormalizeAngle(yaw.GetValue(i) - prevYaw);
                float deltaPitch = NormalizeAngle(pitch.GetValue(i) - prevPitch);
                float deltaRoll = NormalizeAngle(roll.GetValue(i) - prevRoll);

                if (Math.Abs(deltaYaw) > Threshold * Math.PI / 180.0f)
                {
                    if (deltaYaw > 0)
                    {
                        prevYaw -= 2 * (float)Math.PI;
                    }
                    else
                    {
                        prevYaw += 2 * (float)Math.PI;
                    }
                }

                if (Math.Abs(deltaPitch) > Threshold * Math.PI / 180.0f)
                {
                    if (deltaPitch > 0)
                    {
                        prevPitch -= 2 * (float)Math.PI;
                    }
                    else
                    {
                        prevPitch += 2 * (float)Math.PI;
                    }
                }

                if (Math.Abs(deltaRoll) > Threshold * Math.PI / 180.0f)
                {
                    if (deltaRoll > 0)
                    {
                        prevRoll -= 2 * (float)Math.PI;
                    }
                    else
                    {
                        prevRoll += 2 * (float)Math.PI;
                    }
                }

                float filteredYaw = Alpha * prevYaw;// + (1 - Alpha) * yaw.GetValue(i);
                float filteredPitch = Alpha * prevPitch;// + (1 - Alpha) * pitch.GetValue(i);
                float filteredRoll = Alpha * prevRoll;// + (1 - Alpha) * roll.GetValue(i);

                prevYaw = filteredYaw;
                prevPitch = filteredPitch;
                prevRoll = filteredRoll;

                // System.Diagnostics.Debug.WriteLine($"Filtered Yaw[{i}]: {filteredYaw}, Filtered Pitch[{i}]: {filteredPitch}, Filtered Roll[{i}]: {filteredRoll}");

                xfilter.Add(new FOBJKey()
                {
                    Frame = i,
                    InterpolationType = HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_LIN,
                    Value = filteredYaw,
                });
                yfilter.Add(new FOBJKey()
                {
                    Frame = i,
                    InterpolationType = HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_LIN,
                    Value = filteredPitch,
                });
                zfilter.Add(new FOBJKey()
                {
                    Frame = i,
                    InterpolationType = HSDRaw.Common.Animation.GXInterpolationType.HSD_A_OP_LIN,
                    Value = filteredRoll,
                });
            }

            yaw.Keys = xfilter;
            pitch.Keys = yfilter;
            roll.Keys = zfilter;
        }

        private static float NormalizeAngle(float angle)
        {
            angle %= 2 * (float)Math.PI;
            if (angle < -Math.PI)
            {
                angle += 2 * (float)Math.PI;
            }
            else if (angle > Math.PI)
            {
                angle -= 2 * (float)Math.PI;
            }
            return angle;
        }
    }
}
