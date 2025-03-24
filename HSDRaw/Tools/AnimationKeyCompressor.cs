using HSDRaw.Common;
using HSDRaw.Common.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSDRaw.Tools
{
    /// <summary>
    /// Very basic spline key fitting to help reduce animation file size
    /// </summary>
    public class AnimationKeyCompressor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tracks"></param>
        /// <param name="joint"></param>
        /// <param name="settings"></param>
        /// <param name="optimizeChildren"></param>
        public static void OptimizeJointTracks(HSD_JOBJ joint, ref List<FOBJ_Player> tracks, float error = 0.001f)
        {
            List<FOBJ_Player> toRemove = new List<FOBJ_Player>();


            Parallel.ForEach(tracks, track =>
            {
                // remove the none tracks
                if (track.JointTrackType == JointTrackType.HSD_A_J_NONE)
                {
                    toRemove.Add(track);
                }
                else
                {
                    // remove constant tracks that don't change value
                    if (IsConstant(track, 0.01f) &&
                        Math.Abs(joint.GetDefaultValue(track.JointTrackType) - track.GetValue(0)) < 0.01f)
                    {
                        toRemove.Add(track);
                    }
                    else
                    if (error != 0)
                    {
                        // bake keys
                        // they need to be backed before being compressed
                        BakeTrack(track);

                        // perform key fitting compression
                        CompressTrack(track, error);
                    }
                    else
                    {
                        // optimize differences in slope
                        for (int i = track.Keys.Count - 1; i > 0; i--)
                        {
                            // previous slope
                            if (track.Keys[i].InterpolationType != GXInterpolationType.HSD_A_OP_SLP)
                                continue;

                            if (Math.Abs(track.Keys[i - 1].Tan - track.Keys[i].Tan) < 0.01f)
                                track.Keys.RemoveAt(i);
                        }
                    }
                }
            });

            // remove certain tracks
            foreach (var rem in toRemove)
                tracks.Remove(rem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public static void BakeTrack(FOBJ_Player player)
        {
            var keys = new List<FOBJKey>();

            for (int i = 0; i <= player.FrameCount; i++)
            {
                keys.Add(new FOBJKey()
                {
                    Frame = i,
                    Value = player.GetValue(i),
                    InterpolationType = GXInterpolationType.HSD_A_OP_SPL,
                    Tan = CalculateTangent(player, i)
                }
                );
            }

            player.Keys = keys;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        //public static void CompressTrack(FOBJ_Player player, float epsilon = 0.001f)
        //{
        //    if (player.FrameCount > 1000)
        //    {
        //        var newPlayer = new FOBJ_Player();

        //        for (int i = 0; i < player.FrameCount; i += 500)
        //        {
        //            var comp = new FOBJ_Player();
        //            for (int j = i; j < Math.Min(player.FrameCount, i + 500); j++)
        //            {
        //                comp.Keys.Add(new FOBJKey()
        //                {
        //                    Frame = j - i,
        //                    Value = player.GetValue(j),
        //                    InterpolationType = GXInterpolationType.HSD_A_OP_LIN,
        //                });
        //            }

        //            var before = comp.Keys.Count;
        //            CompressTrack2(comp, epsilon);
        //            System.Diagnostics.Debug.WriteLine($"{before}->{comp.Keys.Count}");
        //            foreach (var k in comp.Keys)
        //            {
        //                k.Frame += i;
        //            }
        //            newPlayer.Keys.AddRange(comp.Keys);
        //        }

        //        player.Keys = newPlayer.Keys;
        //        System.Diagnostics.Debug.WriteLine($"{player.Keys.Count}");
        //    }
        //    else
        //    {
        //        CompressTrack2(player, epsilon);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public static void CompressTrack(FOBJ_Player player, float epsilon = 0.001f)
        {
            var newPlayer = new FOBJ_Player();

            // Method 1: Error Redution

            newPlayer.Keys.Add(new FOBJKey()
            {
                Frame = 0,
                Value = player.GetValue(0),
                InterpolationType = GXInterpolationType.HSD_A_OP_SPL,
                Tan = CalculateTangent(player, 0)
            });

            newPlayer.Keys.Add(new FOBJKey()
            {
                Frame = player.FrameCount,
                Value = player.GetValue(player.FrameCount),
                InterpolationType = GXInterpolationType.HSD_A_OP_SPL,
                Tan = CalculateTangent(player, player.FrameCount)
            });

            while (true)
            {
                var errorIndex = CheckError(player, newPlayer, epsilon, out float maxError);

                if (errorIndex == -1)
                    break;
                else
                {
                    newPlayer.Keys.Add(new FOBJKey()
                    {
                        Frame = errorIndex,
                        Value = player.GetValue(errorIndex),
                        InterpolationType = GXInterpolationType.HSD_A_OP_SPL,
                        Tan = CalculateTangent(player, errorIndex)
                    });
                    newPlayer.Keys = newPlayer.Keys.OrderBy(a => a.Frame).ToList();
                }
            }

            RemoveUselessKeys(newPlayer, epsilon);

            player.Keys = newPlayer.Keys;
        }

        /// <summary>
        /// 
        /// </summary>
        public static float CalculateTangent(FOBJ_Player player, float i)
        {
            //for (int i = 0; i < player.Keys.Count; i++)
            {
                var current = player.GetValue(i);

                //current.InterpolationType = GXInterpolationType.HSD_A_OP_SPL;
                float Tan = 0;
                var weight = 0;

                var prevKey = player.Keys.LastOrDefault(e => e.Frame < i);
                var nextKey = player.Keys.FirstOrDefault(e => e.Frame > i);

                if (i != 0)
                {
                    float dis = 1;
                    if (prevKey != null)
                        dis = i - prevKey.Frame;
                    var prev = player.GetValue(i - dis);
                    Tan += (current - prev) / dis;
                    weight++;
                }

                if (i != player.Keys.Count - 1)
                {
                    float dis = 1;
                    if (nextKey != null)
                        dis = nextKey.Frame - i;
                    var next = player.GetValue(i + dis);
                    Tan += (next - current) / dis;
                    weight++;
                }

                Tan /= weight;

                return Tan;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="newtrack"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static int CheckError(FOBJ_Player original, FOBJ_Player newtrack, float error, out float maxError)
        {
            maxError = 0;
            int maxErrorIndex = -1;

            for (int i = 0; i < newtrack.FrameCount; i++)
            {
                var err = Math.Abs(original.GetValue(i) - newtrack.GetValue(i));
                if (err > maxError)
                {
                    maxError = err;
                    maxErrorIndex = i;
                }
            }

            if (maxError > error)
                return maxErrorIndex;
            else
                return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="epsilon"></param>
        private static void RemoveUselessKeys(FOBJ_Player player, float epsilon)
        {
            int i = 1;
            while (i + 1 < player.Keys.Count)
            {
                var prev = player.Keys[i - 1];
                var next = player.Keys[i + 1];
                FOBJ_Player tester = new FOBJ_Player();
                tester.Keys.Add(prev);
                tester.Keys.Add(next);

                var remove = true;
                for (int j = (int)prev.Frame; j < next.Frame; j++)
                {
                    if (Math.Abs(tester.GetValue(j) - player.GetValue(j)) > epsilon)
                    {
                        remove = false;
                        break;
                    }
                }

                if (remove)
                {
                    player.Keys.RemoveAt(i);
                    if (i + 1 >= player.Keys.Count)
                        break;
                }
                else
                    i++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool IsConstant(FOBJ_Player player, float epsilon = 0.001f)
        {
            var start = player.GetValue(0);

            for (int i = 0; i < player.FrameCount; i++)
            {
                if (Math.Abs(player.GetValue(i) - start) >= epsilon)
                    return false;
            }

            return true;
        }
    }
}
