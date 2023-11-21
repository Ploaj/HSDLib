using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;

namespace HSDRawViewer.Tools
{
    public class AnimationRetarget
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="anim_file"></param>
        /// <param name="source_file"></param>
        /// <param name="target_file"></param>
        /// <param name="source_map"></param>
        /// <param name="target_map"></param>
        public static JointAnimManager Retarget(string anim_file, string source_file, string target_file, string source_map, string target_map)
        {
            var src_map = new JointMap(source_map);
            var tar_map = new JointMap(target_map);

            JointAnimManager anim = Converters.Animation.JointAnimationLoader.LoadJointAnimFromFile(src_map, anim_file);

            var src_jobj = new LiveJObj(new HSDRawFile(source_file).Roots[0].Data as HSD_JOBJ);
            var tar_jobj = new LiveJObj(new HSDRawFile(target_file).Roots[0].Data as HSD_JOBJ);

            return Retarget(anim, src_jobj, tar_jobj, src_map, tar_map);
        }

        private static float ClampRotation(float v)
        {
            if (System.Math.Abs(System.Math.Abs(v) - Math3D.TwoPI) < 0.001)
            {
                if (v > 0)
                    v -= (float)Math3D.TwoPI;

                if (v < 0)
                    v += (float)Math3D.TwoPI;
            }
            return v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source_anim"></param>
        /// <param name="source_model"></param>
        /// <param name="target_model"></param>
        /// <param name="source_map"></param>
        /// <param name="target_map"></param>
        public static JointAnimManager Retarget(JointAnimManager source_anim, LiveJObj source_model, LiveJObj target_model, JointMap source_map, JointMap target_map)
        {
            // create new animation
            JointAnimManager new_anim = new JointAnimManager(target_model.JointCount);
            new_anim.FrameCount = source_anim.FrameCount;
                
            // reset joints
            source_model.ResetTransforms();
            target_model.ResetTransforms();

            // loop through target bones
            for (var f = 0; f <= source_anim.FrameCount; f++)
            {
                source_anim.ApplyAnimation(source_model, f);
                source_model.RecalculateTransforms(null, true);

                for (int i = 0; i < target_model.JointCount; i++)
                {
                    var target_joint_name = target_map[i];

                    // bone not found so skip
                    if (target_joint_name == null)
                        continue;

                    // check if source has bone and get the source index
                    int source_joint_index = source_map.IndexOf(target_joint_name);
                    if (source_joint_index == -1)
                        continue;

                    // get source and target node
                    var target_node = new_anim.Nodes[i];
                    var target_joint = target_model.GetJObjAtIndex(i);
                    var source_joint = source_model.GetJObjAtIndex(source_joint_index);

                    // calculate retarget transform
                    ConstrainWorldTransform(source_joint, target_joint);

                    // bake all keys
                    target_node.AddLinearKey(JointTrackType.HSD_A_J_TRAX, f, target_joint.Translation.X);
                    target_node.AddLinearKey(JointTrackType.HSD_A_J_TRAY, f, target_joint.Translation.Y);
                    target_node.AddLinearKey(JointTrackType.HSD_A_J_TRAZ, f, target_joint.Translation.Z);
                    target_node.AddLinearKey(JointTrackType.HSD_A_J_ROTX, f, target_joint.Rotation.X);
                    target_node.AddLinearKey(JointTrackType.HSD_A_J_ROTY, f, target_joint.Rotation.Y);
                    target_node.AddLinearKey(JointTrackType.HSD_A_J_ROTZ, f, target_joint.Rotation.Z);
                    target_node.AddLinearKey(JointTrackType.HSD_A_J_SCAX, f, target_joint.Scale.X);
                    target_node.AddLinearKey(JointTrackType.HSD_A_J_SCAY, f, target_joint.Scale.Y);
                    target_node.AddLinearKey(JointTrackType.HSD_A_J_SCAZ, f, target_joint.Scale.Z);
                }
            }

            return new_anim;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private static void ConstrainWorldTransform(LiveJObj source, LiveJObj target)
        {
            var bind = target.InvertedTransform.Inverted() * source.InvertedTransform;

            var rel = bind * source.WorldTransform;

            if (target.Parent != null)
                rel *= target.Parent.WorldTransform.Inverted();

            var relTranslate = rel.ExtractTranslation();
            var rot = rel.ExtractRotationEuler();
            var relScale = rel.ExtractScale();

            target.Translation = relTranslate;
            target.Rotation = new OpenTK.Mathematics.Vector4(rot);
            target.Scale = relScale;

            target.RecalculateTransforms(null, true);
        }
    }
}
