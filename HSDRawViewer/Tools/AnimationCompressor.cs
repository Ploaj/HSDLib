using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer.Tools
{
    public class AnimationCompressor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="animJoint"></param>
        public static void CompressAnimation(HSD_AnimJoint animJoint, float epsilon = 0.001f)
        {
            foreach (var j in animJoint.BreathFirstList)
            {
                if (j.AOBJ != null)
                    foreach (var t in j.AOBJ.FObjDesc.List)
                    {
                        var player = new FOBJ_Player();
                        player.Keys = t.GetDecodedKeys();
                        BakeTrack(player);
                        CompressTrack(player, epsilon);
                        t.SetKeys(player.Keys, t.TrackType);
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="j"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static int MaxDepth(HSD_AnimJoint j, int depth = 0)
        {
            var maxDepth = depth;

            if (j.Child != null)
                foreach (var c in j.Children)
                    maxDepth = Math.Max(MaxDepth(c, depth + 1), maxDepth);

            return maxDepth;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animJoint"></param>
        public static int AdaptiveCompressAnimation(HSD_AnimJoint j, int index = 0)
        {
            //float rangeStart = 0.05f;
            //float rangeEnd = 0.001f;

            //var percent = ((float)depth / maxDepth);

            //var lerp = ((rangeStart - rangeEnd) - ((rangeStart - rangeEnd) * percent)) + rangeEnd;

            //lerp = rangeEnd;

            //Console.WriteLine(index + " " + 0.005f);

            float error = 0.001f;

            //TODO: adaptive error
            if (index < 16)
                error = 0.001f;

            if (j.AOBJ != null)
                foreach (var t in j.AOBJ.FObjDesc.List)
                {
                    var player = new FOBJ_Player();
                    player.Keys = t.GetDecodedKeys();
                    BakeTrack(player);
                    CompressTrack(player, error);
                    t.SetKeys(player.Keys, t.TrackType);
                }

            if (j.Child != null)
                foreach (var c in j.Children)
                    index = AdaptiveCompressAnimation(c, index + 1);

            return index;
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
        /// <param name="player"></param>
        private static void CalculateTangents(FOBJ_Player player)
        {
            for (int i = 0; i < player.Keys.Count; i++)
            {
                player.Keys[i].InterpolationType = GXInterpolationType.HSD_A_OP_SPL;
                player.Keys[i].Tan = CalculateTangent(player, i);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static float CalculateTangent(FOBJ_Player player, int i)
        {
            //for (int i = 0; i < player.Keys.Count; i++)
            {
                var current = player.GetValue(i);

                //current.InterpolationType = GXInterpolationType.HSD_A_OP_SPL;
                float Tan = 0;
                var weight = 0;

                if (i != 0)
                {
                    var dis = 1;
                    var prev = player.GetValue(i - dis);
                    Tan += (current - prev) / dis;
                    weight++;
                }

                if (i != player.Keys.Count - 1)
                {
                    var dis = 1;
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
    }
}
