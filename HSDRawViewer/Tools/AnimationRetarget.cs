using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Rendering;
using OpenTK;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Tools
{
    public class AnimationRetarget
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="animation"></param>
        /// <param name="sourceMap"></param>
        /// <param name="targetMap"></param>
        /// <returns></returns>
        public static JointAnimManager Port(JointAnimManager animation, JointMap sourceMap, JointMap targetMap)
        {
            JointAnimManager newAnim = new JointAnimManager();
            newAnim.FrameCount = animation.FrameCount;

            for (int i = 0; i < targetMap.Count; i++)
            {
                var bone = targetMap[i];

                var index = sourceMap.IndexOf(bone);

                if(index != -1 && index < animation.Nodes.Count)
                {
                    newAnim.Nodes.Add(animation.Nodes[index]);
                }
                else
                {
                    newAnim.Nodes.Add(new AnimNode());
                }
            }

            return newAnim;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="animation"></param>
        /// <returns></returns>
        public static JointAnimManager Retarget(HSD_JOBJ source, HSD_JOBJ target, JointAnimManager animation, JointMap sourceMap = null, JointMap targetMap = null)
        {
            var sourceManager = new JOBJManager();
            sourceManager.SetJOBJ(source);

            var sourceInverseWorldTransforms = new List<Matrix4>();
            for (int i = 0; i < sourceManager.JointCount; i++)
                sourceInverseWorldTransforms.Add(sourceManager.GetWorldTransform(i).Inverted());

            sourceManager.Animation = animation;


            var targetManager = new JOBJManager();
            targetManager.SetJOBJ(target);

            JointAnimManager newAnim = new JointAnimManager();
            newAnim.FrameCount = sourceManager.Animation.FrameCount;
            var targetWorldTransforms = new List<Matrix4>();
            for (int i = 0; i < targetManager.JointCount; i++)
            {
                targetWorldTransforms.Add(targetManager.GetWorldTransform(i));
                newAnim.Nodes.Add(new AnimNode());
            }

            targetManager.Animation = newAnim;
            for(int f = 0; f <= sourceManager.Animation.FrameCount; f++)
            {
                sourceManager.Frame = f - 1;
                sourceManager.UpdateNoRender();

                targetManager.Frame = f;
                targetManager.UpdateNoRender();

                // bake animation on target skeleton
                for (int i = 0; i < sourceManager.JointCount; i++)
                {
                    int targetIndex = i;
                    int sourceIndex = i;

                    // remap bone if joint maps are present
                    if (sourceMap != null && targetMap != null && !string.IsNullOrEmpty(sourceMap[i]))
                        targetIndex = targetMap.IndexOf(sourceMap[i]);

                    if (targetIndex == -1)
                        continue;

                    int targetParentIndex = targetManager.ParentIndex(targetManager.GetJOBJ(targetIndex));

                    var inverseSourceWorldRotation = sourceInverseWorldTransforms[sourceIndex];
                    var sourceAnimatedWorldRotation = sourceManager.GetWorldTransform(sourceIndex);

                    var targetWorldRotation = targetWorldTransforms[targetIndex];

                    var rel = targetWorldRotation *
                        inverseSourceWorldRotation *
                        sourceAnimatedWorldRotation;

                    if(targetParentIndex != -1)
                        rel *= targetManager.GetWorldTransform(targetParentIndex).Inverted();

                    var rot = Math3D.ToEulerAngles(rel.ExtractRotation().Inverted());

                    var targetJOBJ = targetManager.GetJOBJ(targetIndex);
                    var sourceJOBJ = sourceManager.GetJOBJ(sourceIndex);


                    sourceManager.Animation.GetAnimatedState(f - 1, sourceIndex, sourceJOBJ, out float TX, out float TY, out float TZ, out float RX, out float RY, out float RZ, out float SX, out float SY, out float SZ);


                    var relTranslate = new Vector3(sourceJOBJ.TX - TX, sourceJOBJ.TY - TY, sourceJOBJ.TZ - TZ);
                    //relTranslate = Vector3.TransformNormal(relTranslate, Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(sourceJOBJ.RZ, sourceJOBJ.RY, sourceJOBJ.RX).Inverted()));
                    //relTranslate = Vector3.TransformNormal(relTranslate, Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(targetJOBJ.RZ, targetJOBJ.RY, targetJOBJ.RX)));


                    var relScale = new Vector3(sourceJOBJ.SX - SX, sourceJOBJ.SY - SY, sourceJOBJ.SZ - SZ);
                    //relScale = Vector3.TransformNormal(relScale, inverseSourceWorldRotation);
                    //relScale = Vector3.TransformNormal(relScale, targetWorldRotation);


                    AddKey(targetIndex, f, targetJOBJ.TX - relTranslate.X, JointTrackType.HSD_A_J_TRAX, newAnim);
                    AddKey(targetIndex, f, targetJOBJ.TY - relTranslate.Y, JointTrackType.HSD_A_J_TRAY, newAnim);
                    AddKey(targetIndex, f, targetJOBJ.TZ - relTranslate.Z, JointTrackType.HSD_A_J_TRAZ, newAnim);

                    AddKey(targetIndex, f, rot.X, JointTrackType.HSD_A_J_ROTX, newAnim);
                    AddKey(targetIndex, f, rot.Y, JointTrackType.HSD_A_J_ROTY, newAnim);
                    AddKey(targetIndex, f, rot.Z, JointTrackType.HSD_A_J_ROTZ, newAnim);

                    AddKey(targetIndex, f, targetJOBJ.SX - relScale.X, JointTrackType.HSD_A_J_SCAX, newAnim);
                    AddKey(targetIndex, f, targetJOBJ.SY - relScale.Y, JointTrackType.HSD_A_J_SCAY, newAnim);
                    AddKey(targetIndex, f, targetJOBJ.SZ - relScale.Z, JointTrackType.HSD_A_J_SCAZ, newAnim);

                    targetManager.UpdateNoRender();
                }
            }

            EulerFilter(newAnim);

            //var targetAnim = newAnim.ToAnimJoint(target, AOBJ_Flags.ANIM_LOOP);
            //AnimationCompressor.AdaptiveCompressAnimation(targetAnim, targetMap);
            //RemoveUnusedTracks(target, targetAnim);
            //newAnim.FromAnimJoint(targetAnim);


            return newAnim;
        }

        private static void EulerFilter(JointAnimManager anim)
        {
            foreach (var n in anim.Nodes)
            {
                foreach (var t in n.Tracks)
                {
                    if (t.JointTrackType == JointTrackType.HSD_A_J_ROTX ||
                        t.JointTrackType == JointTrackType.HSD_A_J_ROTY ||
                        t.JointTrackType == JointTrackType.HSD_A_J_ROTZ)
                    {
                        // filter
                        for (int i = 1; i < t.Keys.Count; i++)
                        {
                            var prevKey = t.Keys[i - 1];
                            var key = t.Keys[i];

                            if(Math.Abs(prevKey.Value - key.Value) > Math.PI)
                            {
                                if (prevKey.Value < key.Value)
                                    key.Value -= (float)(2 * Math.PI);
                                else
                                    key.Value += (float)(2 * Math.PI);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void RemoveUnusedTracks(HSD_JOBJ model, HSD_AnimJoint animJoint)
        {
            JointAnimManager manager = new JointAnimManager();
            manager.FromAnimJoint(animJoint);

            var joints = model.BreathFirstList;

            if (joints.Count != manager.Nodes.Count)
                return;

            for(int i = 0; i < joints.Count; i++)
            {
                var fobjs = manager.Nodes[i].Tracks;
                var toRem = new List<FOBJ_Player>();

                foreach(var f in fobjs)
                {
                    bool remove = false;
                    switch(f.JointTrackType)
                    {
                        case JointTrackType.HSD_A_J_TRAX: remove = RemoveTrack(f, joints[i].TX); break;
                        case JointTrackType.HSD_A_J_TRAY: remove = RemoveTrack(f, joints[i].TY); break;
                        case JointTrackType.HSD_A_J_TRAZ: remove = RemoveTrack(f, joints[i].TZ); break;
                        case JointTrackType.HSD_A_J_ROTX: remove = RemoveTrack(f, joints[i].RX); break;
                        case JointTrackType.HSD_A_J_ROTY: remove = RemoveTrack(f, joints[i].RY); break;
                        case JointTrackType.HSD_A_J_ROTZ: remove = RemoveTrack(f, joints[i].RZ); break;
                        case JointTrackType.HSD_A_J_SCAX: remove = RemoveTrack(f, joints[i].SX); break;
                        case JointTrackType.HSD_A_J_SCAY: remove = RemoveTrack(f, joints[i].SY); break;
                        case JointTrackType.HSD_A_J_SCAZ: remove = RemoveTrack(f, joints[i].SZ); break;
                    }
                    if(remove)
                        toRem.Add(f);
                }

                foreach (var v in toRem)
                    fobjs.Remove(v);

            }

            var newAnimJoint = manager.ToAnimJoint(model, AOBJ_Flags.ANIM_LOOP);

            animJoint._s = newAnimJoint._s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static bool RemoveTrack(FOBJ_Player player, float defaultValue)
        {
            if (player.Keys.Count == 0)
                return true;

            if (player.Keys.Count > 2)
                return false;

            if (player.Keys[0].Value != player.Keys[1].Value)
                return false;

            return Math.Abs(player.Keys[0].Value - defaultValue) < 0.001f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="frame"></param>
        /// <param name="value"></param>
        /// <param name="track"></param>
        /// <param name="manager"></param>
        private static void AddKey(int node, int frame, float value, JointTrackType tracktype, JointAnimManager manager)
        {
            var track = manager.Nodes[node].Tracks.Find(e => e.JointTrackType == tracktype);

            if(track == null)
            {
                track = new FOBJ_Player()
                {
                    JointTrackType = tracktype
                };
                manager.Nodes[node].Tracks.Add(track);
            }

            track.Keys.Add(new FOBJKey()
            {
                Frame = frame,
                Value = value,
                InterpolationType = frame == 0 ? GXInterpolationType.HSD_A_OP_KEY : GXInterpolationType.HSD_A_OP_CON
            });
        }
    }
}
