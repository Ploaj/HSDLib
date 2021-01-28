using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Rendering;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <returns></returns>
        public static HSD_FigaTree Retarget(HSD_JOBJ source, HSD_JOBJ target, HSD_FigaTree animation)
        {
            JointAnimManager manager = new JointAnimManager();
            manager.FromFigaTree(animation);

            var retarget = Retarget(source, target, manager.ToAnimJoint(source, AOBJ_Flags.ANIM_LOOP));

            manager.FromAnimJoint(retarget);

            return manager.ToFigaTree();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="animation"></param>
        /// <returns></returns>
        public static HSD_AnimJoint Retarget(HSD_JOBJ source, HSD_JOBJ target, HSD_AnimJoint animation)
        {
            var sourceManager = new JOBJManager();
            sourceManager.SetJOBJ(source);

            var sourceInverseWorldTransforms = new List<Matrix4>();
            for (int i = 0; i < sourceManager.JointCount; i++)
                sourceInverseWorldTransforms.Add(sourceManager.GetWorldTransform(i).Inverted());

            sourceManager.SetAnimJoint(animation);



            var targetManager = new JOBJManager();
            targetManager.SetJOBJ(target);


            JointAnimManager manager = new JointAnimManager();
            manager.FrameCount = sourceManager.Animation.FrameCount;
            var targetWorldTransforms = new List<Matrix4>();
            for (int i = 0; i < targetManager.JointCount; i++)
            {
                targetWorldTransforms.Add(targetManager.GetWorldTransform(i));

                List<FOBJ_Player> players = new List<FOBJ_Player>();

                players.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_TRAX });
                players.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_TRAY });
                players.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_TRAZ });
                players.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_ROTX });
                players.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_ROTY });
                players.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_ROTZ });
                players.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_SCAX });
                players.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_SCAY });
                players.Add(new FOBJ_Player() { JointTrackType = JointTrackType.HSD_A_J_SCAZ });

                manager.Nodes.Add(new AnimNode()
                {
                    Tracks = players
                });
            }

            targetManager.Animation = manager;
            for(int f = 0; f <= sourceManager.Animation.FrameCount; f++)
            {
                sourceManager.Frame = f - 1;
                sourceManager.UpdateNoRender();

                targetManager.Frame = f;
                targetManager.UpdateNoRender();

                // bake animation on target skeleton
                for (int i = 0; i < sourceManager.JointCount; i++)
                {
                    int parentIndex = targetManager.ParentIndex(targetManager.GetJOBJ(i));

                    var inverseSourceWorldRotation = sourceInverseWorldTransforms[i];
                    var sourceAnimatedWorldRotation = sourceManager.GetWorldTransform(i);

                    var targetWorldRotation = targetWorldTransforms[i];
                    var targetParentAnimatedWorldRotation =
                        parentIndex == -1 ?
                        Matrix4.Identity :
                        targetManager.GetWorldTransform(parentIndex);

                    var rel = targetWorldRotation *
                        inverseSourceWorldRotation *
                        sourceAnimatedWorldRotation;

                    rel *= targetParentAnimatedWorldRotation.Inverted();

                    var rot = Math3D.ToEulerAngles(rel.ExtractRotation().Inverted());

                    var targetJOBJ = targetManager.GetJOBJ(i);
                    var sourceJOBJ = sourceManager.GetJOBJ(i);


                    sourceManager.Animation.GetAnimatedState(f, i, sourceJOBJ, out float TX, out float TY, out float TZ, out float RX, out float RY, out float RZ, out float SX, out float SY, out float SZ);


                    var relTranslate = new Vector3(sourceJOBJ.TX - TX, sourceJOBJ.TY - TY, sourceJOBJ.TZ - TZ);
                    //relTranslate = Vector3.TransformNormal(relTranslate, Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(sourceJOBJ.RZ, sourceJOBJ.RY, sourceJOBJ.RX).Inverted()));
                    //relTranslate = Vector3.TransformNormal(relTranslate, Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(targetJOBJ.RZ, targetJOBJ.RY, targetJOBJ.RX)));


                    var relScale = new Vector3(sourceJOBJ.SX - SX, sourceJOBJ.SY - SY, sourceJOBJ.SZ - SZ);
                    //relScale = Vector3.TransformNormal(relScale, inverseSourceWorldRotation);
                    //relScale = Vector3.TransformNormal(relScale, targetWorldRotation);


                    AddKey(i, f, targetJOBJ.TX - relTranslate.X, JointTrackType.HSD_A_J_TRAX, manager);
                    AddKey(i, f, targetJOBJ.TY - relTranslate.Y, JointTrackType.HSD_A_J_TRAY, manager);
                    AddKey(i, f, targetJOBJ.TZ - relTranslate.Z, JointTrackType.HSD_A_J_TRAZ, manager);

                    AddKey(i, f, rot.X, JointTrackType.HSD_A_J_ROTX, manager);
                    AddKey(i, f, rot.Y, JointTrackType.HSD_A_J_ROTY, manager);
                    AddKey(i, f, rot.Z, JointTrackType.HSD_A_J_ROTZ, manager);

                    AddKey(i, f, targetJOBJ.SX - relScale.X, JointTrackType.HSD_A_J_SCAX, manager);
                    AddKey(i, f, targetJOBJ.SY - relScale.Y, JointTrackType.HSD_A_J_SCAY, manager);
                    AddKey(i, f, targetJOBJ.SZ - relScale.Z, JointTrackType.HSD_A_J_SCAZ, manager);

                    targetManager.UpdateNoRender();
                }
            }

            var targetAnim = manager.ToAnimJoint(target, AOBJ_Flags.ANIM_LOOP);
            AnimationCompressor.AdaptiveCompressAnimation(targetAnim);
            RemoveUnusedTracks(target, targetAnim);
            manager.FromAnimJoint(targetAnim);


            return targetAnim;
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
        private static void AddKey(int node, int frame, float value, JointTrackType track, JointAnimManager manager)
        {
            manager.Nodes[node].Tracks.Find(e => e.JointTrackType == track).Keys.Add(new FOBJKey()
            {
                Frame = frame,
                Value = value,
                InterpolationType = frame == 0 ? GXInterpolationType.HSD_A_OP_KEY : GXInterpolationType.HSD_A_OP_CON
            });
        }
    }
}
