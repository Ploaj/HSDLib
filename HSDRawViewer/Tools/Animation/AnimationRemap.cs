using HSDRaw;
using HSDRaw.Common;
using HSDRawViewer.Converters.Animation;
using HSDRawViewer.Rendering;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Tools.Animation
{
    public class AnimationRemap
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="animfile"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static HSDRawFile RemapRel(
            string source_joint_filepath,
            string target_joint_filepath, 
            string target_anim_filepath, 
            string source_jointmap_filepath, 
            string target_jointmap_filepath)
        {
            var from = new JointMap(source_jointmap_filepath);
            var to = new JointMap(target_jointmap_filepath);
            var anim = JointAnimationLoader.LoadJointAnimFromFile(from, target_anim_filepath);

            var from_jobj = (new HSDRawFile(source_joint_filepath).Roots[0].Data as HSD_JOBJ).TreeList;
            var to_jobj = (new HSDRawFile(target_joint_filepath).Roots[0].Data as HSD_JOBJ).TreeList;

            var new_anim = Remap(anim, from, to, from_jobj, to_jobj);

            var file = new HSDRawFile();

            file.Roots.Add(new HSDRootNode()
            {
                Name = "_figatree",
                Data = new_anim.ToFigaTree(0.01f),
            });

            return file;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animfile"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static HSDRawFile Remap(string anim_filepath, string from_jointmap_filepath, string to_jointmap_filepath)
        {
            var from = new JointMap(from_jointmap_filepath);
            var to = new JointMap(to_jointmap_filepath);
            var anim = JointAnimationLoader.LoadJointAnimFromFile(from, anim_filepath);

            var new_anim = Remap(anim, from, to);

            var file = new HSDRawFile();

            file.Roots.Add(new HSDRootNode()
            {
                Name = "_figatree",
                Data = new_anim.ToFigaTree(0.01f),
            });

            return file;
        }

        /// <summary>
        /// Generates a new joint anim remapping bones between skeletons
        /// Note: does *not* retarget <see cref="AnimationRetarget"/> the animation
        /// </summary>
        /// <param name="anim"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static JointAnimManager Remap(JointAnimManager anim, JointMap from, JointMap to)
        {
            JointAnimManager n = new JointAnimManager();
            n.FrameCount = anim.FrameCount;
            for (int i = 0; i < to.Count; i++)
                n.Nodes.Add(new AnimNode());

            for (int i = 0; i < anim.NodeCount; i++)
            {
                if (i >= from.Count)
                    break;

                int remap_index = to.IndexOf(from[i]);

                System.Diagnostics.Debug.WriteLine(from[i] + " " + i + " -> " + remap_index);

                if (remap_index != -1)
                {
                    n.Nodes[remap_index].Tracks = anim.Nodes[i].Tracks;
                }
            }

            return n;
        }

        
        /// <summary>
        /// Generates a new joint anim remapping bones between skeletons
        /// Note: does *not* retarget <see cref="AnimationRetarget"/> the animation
        /// </summary>
        /// <param name="anim"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static JointAnimManager Remap(JointAnimManager anim, 
            JointMap from, 
            JointMap to, 
            List<HSD_JOBJ> fromj,
            List<HSD_JOBJ> toj)
        {
            JointAnimManager n = new JointAnimManager();
            n.FrameCount = anim.FrameCount;

            for (int i = 0; i < to.Count; i++)
                n.Nodes.Add(new AnimNode());

            for (int i = 0; i < anim.NodeCount; i++)
            {
                if (i >= from.Count)
                    break;

                int remap_index = to.IndexOf(from[i]);

                System.Diagnostics.Debug.WriteLine(from[i] + " " + i + " -> " + remap_index);

                if (remap_index != -1)
                {
                    n.Nodes[remap_index].Tracks = anim.Nodes[i].Tracks;

                    foreach (var t in n.Nodes[remap_index].Tracks)
                    {
                        switch(t.JointTrackType)
                        {
                            case HSDRaw.Common.Animation.JointTrackType.HSD_A_J_TRAX:
                                foreach (var k in t.Keys)
                                {
                                    k.Value -= fromj[i].TX - toj[remap_index].TX;
                                }
                                break;
                            case HSDRaw.Common.Animation.JointTrackType.HSD_A_J_TRAY:
                                foreach (var k in t.Keys)
                                {
                                    k.Value -= fromj[i].TY - toj[remap_index].TY;
                                }
                                break;
                            case HSDRaw.Common.Animation.JointTrackType.HSD_A_J_TRAZ:
                                foreach (var k in t.Keys)
                                {
                                    k.Value -= fromj[i].TZ - toj[remap_index].TZ;
                                }
                                break;
                        }
                    }
                }
            }

            return n;
        }
    }
}
