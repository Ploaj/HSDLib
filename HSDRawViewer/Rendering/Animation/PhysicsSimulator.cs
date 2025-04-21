using HSDRaw.Melee.Pl;
using HSDRawViewer.Rendering.Models;
using System.Collections.Generic;

namespace HSDRawViewer.Rendering.Animation
{
    public class PhysicsSimulator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dynamicDesc"></param>
        /// <param name="hitbubbles"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static PhysicsPlayer Init(IEnumerable<SBM_DynamicDesc> dynamicDesc, IEnumerable<SBM_DynamicHitBubble> hitbubbles, LiveJObj m)
        {
            PhysicsPlayer p = new();

            if (hitbubbles != null)
                foreach (SBM_DynamicHitBubble v in hitbubbles)
                    p.Hitbubbles.Add(v);

            if (dynamicDesc != null)
                foreach (SBM_DynamicDesc desc in dynamicDesc)
                {
                    if (desc == null)
                        continue;

                    Dynamics set = new();
                    set.InitBones(m, desc.BoneIndex, desc.Parameters.Length);
                    set.ApplyNum = 0;
                    // toggle dynamics
                    set.InitParams(desc);
                    p.Sets.Add(set);
                }

            return p;

        }
    }
}
