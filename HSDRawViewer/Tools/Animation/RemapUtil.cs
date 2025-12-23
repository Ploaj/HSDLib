using HSDRaw;
using HSDRaw.Common;
using HSDRawViewer.Converters.Animation;
using HSDRawViewer.Rendering;
using System.Collections.Generic;
using System.Numerics;

namespace HSDRawViewer.Tools.Animation
{
    public class RemapUtil
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
            JointMap from = new(source_jointmap_filepath);
            JointMap to = new(target_jointmap_filepath);
            JointAnimManager anim = JointAnimationLoader.LoadJointAnimFromFile(from, target_anim_filepath);

            List<HSD_JOBJ> from_jobj = (new HSDRawFile(source_joint_filepath).Roots[0].Data as HSD_JOBJ).TreeList;
            List<HSD_JOBJ> to_jobj = (new HSDRawFile(target_joint_filepath).Roots[0].Data as HSD_JOBJ).TreeList;

            JointAnimManager new_anim = Remap(anim, from, to, from_jobj, to_jobj);

            HSDRawFile file = new();

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
            JointMap from = new(from_jointmap_filepath);
            JointMap to = new(to_jointmap_filepath);
            JointAnimManager anim = JointAnimationLoader.LoadJointAnimFromFile(from, anim_filepath);

            JointAnimManager new_anim = Remap(anim, from, to);

            HSDRawFile file = new();

            file.Roots.Add(new HSDRootNode()
            {
                Name = "_figatree",
                Data = new_anim.ToFigaTree(0.01f),
            });

            return file;
        }

        /// <summary>
        /// Generates a new joint anim remapping bones between skeletons
        /// Note: does *not* retarget <see cref="RetargetUtil"/> the animation
        /// </summary>
        /// <param name="anim"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static JointAnimManager Remap(JointAnimManager anim, JointMap from, JointMap to)
        {
            JointAnimManager n = new();
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
        /// Note: does *not* retarget <see cref="RetargetUtil"/> the animation
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
            JointAnimManager n = new();
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

                    var toLength = new Vector3(toj[remap_index].TX, toj[remap_index].TY, toj[remap_index].TZ).Length();
                    var fromLength = new Vector3(fromj[i].TX, fromj[i].TY, fromj[i].TZ).Length();

                    float scale = fromLength != 0.0f ? toLength / fromLength : 1.0f;

                    foreach (HSDRaw.Tools.FOBJ_Player t in n.Nodes[remap_index].Tracks)
                    {
                        switch (t.JointTrackType)
                        {
                            case HSDRaw.Common.Animation.JointTrackType.HSD_A_J_TRAX:
                                foreach (HSDRaw.Tools.FOBJKey k in t.Keys)
                                {
                                    k.Value *= scale;
                                }
                                break;
                            case HSDRaw.Common.Animation.JointTrackType.HSD_A_J_TRAY:
                                foreach (HSDRaw.Tools.FOBJKey k in t.Keys)
                                {
                                    k.Value *= scale;
                                }
                                break;
                            case HSDRaw.Common.Animation.JointTrackType.HSD_A_J_TRAZ:
                                foreach (HSDRaw.Tools.FOBJKey k in t.Keys)
                                {
                                    k.Value *= scale;
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
