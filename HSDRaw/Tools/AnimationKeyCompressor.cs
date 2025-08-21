using HSDRaw.Common;
using HSDRaw.Common.Animation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRaw.Tools
{
    /// <summary>
    /// Very basic spline key fitting to help reduce animation file size
    /// </summary>
    public class AnimationKeyCompressor
    {
        private const float TRANSLATION_EPSILON = 0.1f;
        private const float ROTATION_EPSILON = 0.0087f;
        private const float SCALE_EPSILON = 0.01f;

        public static List<FOBJ_Player> OptimizeJointTracks(HSD_JOBJ joint, List<FOBJ_Player> tracks, float error = 0.001f)
        {
            var optimizedTracks = new List<FOBJ_Player>();

            foreach (var track in tracks)
            {
                if (track.JointTrackType == JointTrackType.HSD_A_J_NONE)
                    continue;

                if (IsConstant(track, 0.01f))
                {
                    if (Math.Abs(joint.GetDefaultValue(track.JointTrackType) - track.GetValue(0)) < 0.01f)
                    {
                        continue;
                    }
                    else
                    {
                        optimizedTracks.Add(new FOBJ_Player()
                        {
                            JointTrackType = track.JointTrackType,
                            Keys = new List<FOBJKey>()
                            {
                                new FOBJKey()
                                {
                                    Frame = 0,
                                    InterpolationType = GXInterpolationType.HSD_A_OP_KEY,
                                    Value = track.GetValue(0)
                                }
                            }
                        });
                    }
                }

                if (error != 0)
                {
                    BakeAndCompressTrack(track);
                    optimizedTracks.Add(track);
                }
                else
                {
                    OptimizeSlope(track);
                    optimizedTracks.Add(track);
                }
            }

            return optimizedTracks;
        }

        private static void BakeAndCompressTrack(FOBJ_Player player)
        {
            int frameCount = player.FrameCount;
            var cachedValues = new float[frameCount + 1];

            for (int i = 0; i <= frameCount; i++)
                cachedValues[i] = player.GetValue(i);

            var keys = new List<FOBJKey>(frameCount + 1);

            for (int i = 0; i <= frameCount; i++)
            {
                keys.Add(new FOBJKey
                {
                    Frame = i,
                    Value = cachedValues[i],
                    InterpolationType = GXInterpolationType.HSD_A_OP_SPL,
                    Tan = CalculateTangentFromCache(i, cachedValues)
                });
            }

            player.Keys = keys;
            CompressTrack(player);
        }

        private static float CalculateTangentFromCache(int i, float[] cached)
        {
            float tan = 0;
            int weight = 0;

            if (i > 0)
            {
                tan += (cached[i] - cached[i - 1]);
                weight++;
            }

            if (i < cached.Length - 1)
            {
                tan += (cached[i + 1] - cached[i]);
                weight++;
            }

            return weight > 0 ? tan / weight : 0;
        }

        public static void CompressTrack(FOBJ_Player player)
        {
            float epsilon = GetEpsilonForTrack(player.JointTrackType);
            var newKeys = new List<FOBJKey>
            {
                new FOBJKey
                {
                    Frame = 0,
                    Value = player.GetValue(0),
                    InterpolationType = GXInterpolationType.HSD_A_OP_SPL,
                    Tan = CalculateTangent(player, 0)
                },
                new FOBJKey
                {
                    Frame = player.FrameCount,
                    Value = player.GetValue(player.FrameCount),
                    InterpolationType = GXInterpolationType.HSD_A_OP_SPL,
                    Tan = CalculateTangent(player, player.FrameCount)
                }
            };

            while (true)
            {
                int index = CheckError(player, newKeys, epsilon, out float maxError);
                if (index == -1) break;

                newKeys.Add(new FOBJKey
                {
                    Frame = index,
                    Value = player.GetValue(index),
                    InterpolationType = GXInterpolationType.HSD_A_OP_SPL,
                    Tan = CalculateTangent(player, index)
                });

                newKeys.Sort((a, b) => a.Frame.CompareTo(b.Frame));
            }

            RemoveUselessKeys(player, newKeys, epsilon);
            player.Keys = newKeys;
        }

        private static float GetEpsilonForTrack(JointTrackType type)
        {
            switch (type)
            {
                case JointTrackType.HSD_A_J_TRAX:
                case JointTrackType.HSD_A_J_TRAY:
                case JointTrackType.HSD_A_J_TRAZ:
                    return TRANSLATION_EPSILON;
                case JointTrackType.HSD_A_J_ROTX:
                case JointTrackType.HSD_A_J_ROTY:
                case JointTrackType.HSD_A_J_ROTZ:
                    return ROTATION_EPSILON;
                case JointTrackType.HSD_A_J_SCAX:
                case JointTrackType.HSD_A_J_SCAY:
                case JointTrackType.HSD_A_J_SCAZ:
                    return SCALE_EPSILON;
                default:
                    return 0.001f;
            }
        }

        public static float CalculateTangent(FOBJ_Player player, float i)
        {
            float current = player.GetValue(i);
            float tan = 0;
            int weight = 0;

            var prevKey = player.Keys.LastOrDefault(e => e.Frame < i);
            var nextKey = player.Keys.FirstOrDefault(e => e.Frame > i);

            if (i != 0)
            {
                float dis = prevKey != null ? i - prevKey.Frame : 1;
                float prev = player.GetValue(i - dis);
                tan += (current - prev) / dis;
                weight++;
            }

            if (i != player.Keys.Count - 1)
            {
                float dis = nextKey != null ? nextKey.Frame - i : 1;
                float next = player.GetValue(i + dis);
                tan += (next - current) / dis;
                weight++;
            }

            return weight > 0 ? tan / weight : 0;
        }

        private static int CheckError(FOBJ_Player original, List<FOBJKey> testKeys, float error, out float maxError)
        {
            var tester = new FOBJ_Player { Keys = testKeys };
            maxError = 0;
            int maxErrorIndex = -1;

            for (int i = 0; i < tester.FrameCount; i++)
            {
                float err = Math.Abs(original.GetValue(i) - tester.GetValue(i));
                if (err > maxError)
                {
                    maxError = err;
                    maxErrorIndex = i;
                }
            }

            return maxError > error ? maxErrorIndex : -1;
        }

        private static void RemoveUselessKeys(FOBJ_Player original, List<FOBJKey> keys, float epsilon)
        {
            int i = 1;
            while (i + 1 < keys.Count)
            {
                var tester = new FOBJ_Player { Keys = new List<FOBJKey> { keys[i - 1], keys[i + 1] } };
                bool remove = true;

                for (int j = (int)keys[i - 1].Frame; j < keys[i + 1].Frame; j++)
                {
                    if (Math.Abs(tester.GetValue(j) - original.GetValue(j)) > epsilon)
                    {
                        remove = false;
                        break;
                    }
                }

                if (remove) keys.RemoveAt(i);
                else i++;
            }
        }

        public static void OptimizeSlope(FOBJ_Player track)
        {
            for (int i = track.Keys.Count - 1; i > 0; i--)
            {
                if (track.Keys[i].InterpolationType != GXInterpolationType.HSD_A_OP_SLP)
                    continue;

                if (Math.Abs(track.Keys[i - 1].Tan - track.Keys[i].Tan) < 0.01f)
                    track.Keys.RemoveAt(i);
            }
        }

        public static bool IsConstant(FOBJ_Player player, float epsilon = 0.001f)
        {
            var first = player.GetValue(0);
            for (int i = 0; i < player.FrameCount; i++)
            {
                if (Math.Abs(player.GetValue(i) - first) >= epsilon)
                    return false;
            }
            return true;
        }
    }

}
