using HSDRaw.Common;
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
        public static void OptimizeTracks(HSD_JOBJ model, HSD_AnimJoint animJoint, float epsilon = 0.001f)
        {
            var joints = model.BreathFirstList;
            var anim_joints = animJoint.BreathFirstList;

            if (joints.Count != anim_joints.Count)
                return;

            for(int i = 0; i < joints.Count; i++)
            {
                if (anim_joints[i].AOBJ != null)
                {
                    Dictionary<JointTrackType, float> typeToDefaultValue = new Dictionary<JointTrackType, float>();
                    typeToDefaultValue.Add(JointTrackType.HSD_A_J_TRAX, joints[i].TX);
                    typeToDefaultValue.Add(JointTrackType.HSD_A_J_TRAY, joints[i].TY);
                    typeToDefaultValue.Add(JointTrackType.HSD_A_J_TRAZ, joints[i].TZ);
                    typeToDefaultValue.Add(JointTrackType.HSD_A_J_ROTX, joints[i].RX);
                    typeToDefaultValue.Add(JointTrackType.HSD_A_J_ROTY, joints[i].RY);
                    typeToDefaultValue.Add(JointTrackType.HSD_A_J_ROTZ, joints[i].RZ);
                    typeToDefaultValue.Add(JointTrackType.HSD_A_J_SCAX, joints[i].SX);
                    typeToDefaultValue.Add(JointTrackType.HSD_A_J_SCAY, joints[i].SY);
                    typeToDefaultValue.Add(JointTrackType.HSD_A_J_SCAZ, joints[i].SZ);

                    var tracks = anim_joints[i].AOBJ.FObjDesc.List;

                    HSD_FOBJDesc prev = null;
                    foreach (var t in tracks)
                    {
                        var keys = t.GetDecodedKeys();
                        if (typeToDefaultValue.ContainsKey(t.JointTrackType) && 
                            ConstantTrack(keys, typeToDefaultValue[t.JointTrackType], epsilon))
                        {
                            if (prev != null)
                                prev.Next = t;
                            else
                                anim_joints[i].AOBJ.FObjDesc = t;
                        }
                        else
                        {
                            prev = t;
                        }
                    }

                    if (anim_joints[i].AOBJ.FObjDesc != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"{tracks.Count} -> {anim_joints[i].AOBJ.FObjDesc.List.Count}");
                    }
                    else
                    {
                        anim_joints[i].AOBJ = null;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="defaultValue"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        private static bool ConstantTrack(List<FOBJKey> keys, float defaultValue, float epsilon)
        {
            return keys.Count == 1 && Math.Abs(keys[0].Value - defaultValue) < epsilon;
        }


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
        /// <param name="animJoint"></param>
        public static int AdaptiveCompressAnimation(HSD_AnimJoint j, JointMap map, int index = 0)
        {
            float error = map == null ? 0.001f : map.GetError(index);

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
                    index = AdaptiveCompressAnimation(c, map, index + 1);

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
