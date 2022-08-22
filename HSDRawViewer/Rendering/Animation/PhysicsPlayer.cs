using HSDRaw.Melee.Pl;
using HSDRawViewer.Rendering.Models;
using System.Collections.Generic;

namespace HSDRawViewer.Rendering.Animation
{
    public class PhysicsPlayer
    {
        public List<Dynamics> Sets { get; } = new List<Dynamics>();

        public List<SBM_DynamicHitBubble> Hitbubbles { get; } = new List<SBM_DynamicHitBubble>();

        public void ResetPhyics(LiveJObj manager)
        {
            foreach (var s in Sets)
                foreach (var d in s.PhysicsBones)
                {
                    d.Reset(manager);
                }
        }

        public void ProcessAndApplyDynamics(LiveJObj manager, bool apply_hit)
        {
            foreach (var v in Sets)
                v.Think(manager, apply_hit ? Hitbubbles : null, false, 8, 0);
        }

    }

}
