using HSDRaw;
using HSDRawViewer.Converters.Animation;
using HSDRawViewer.Rendering;
using System;

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
    }
}
