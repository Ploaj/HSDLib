using HSDRaw;
using HSDRaw.Melee.Pl;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Rendering.Shapes;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace HSDRawViewer.Rendering.Renderers
{
    public class HurtboxRenderer
    {
        private readonly Dictionary<SBM_Hurtbox, Capsule> HurtboxToCapsule = new();

        private Vector3 SelectedHurtboxColor = new(1, 1, 1);
        private Vector3 HurtboxColor = new(1, 1, 0);
        private Vector3 IntanColor = new(0, 0, 1);
        private Vector3 InvulColor = new(0, 1, 0);
        private Vector3 GrabbableColor = new(1, 0, 1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="liveJObj"></param>
        /// <param name="hurtboxes"></param>
        /// <param name="selected"></param>
        /// <param name="states"></param>
        /// <param name="bodyState"></param>
        public void Render(LiveJObj liveJObj, List<SBM_Hurtbox> hurtboxes, HSDAccessor selected, Dictionary<int, int> states = null, int bodyState = -1)
        {
            if (liveJObj == null)
                return;

            foreach (SBM_Hurtbox v in hurtboxes)
            {
                Vector3 clr = HurtboxColor;
                float a = 0.25f;
                if (selected == v)
                {
                    clr = SelectedHurtboxColor;
                    a = 0.6f;
                }

                if (states != null)
                {
                    if (states.ContainsKey(v.BoneIndex))
                    {
                        switch (states[v.BoneIndex])
                        {
                            case 1:
                                clr = InvulColor;
                                break;
                            case 2:
                                clr = IntanColor;
                                break;
                        }
                    }
                }

                switch (bodyState)
                {
                    case -1:
                        if (v.Grabbable == 0)
                            clr = GrabbableColor;
                        break;
                    case 1:
                        clr = InvulColor;
                        break;
                    case 2:
                        clr = IntanColor;
                        break;
                }

                Matrix4 transform = liveJObj.GetJObjAtIndex(v.BoneIndex).WorldTransform;

                if (!HurtboxToCapsule.ContainsKey(v))
                    HurtboxToCapsule.Add(v, new Capsule(new Vector3(v.X1, v.Y1, v.Z1), new Vector3(v.X2, v.Y2, v.Z2), v.Size));

                Capsule cap = HurtboxToCapsule[v];
                cap.SetParameters(new Vector3(v.X1, v.Y1, v.Z1), new Vector3(v.X2, v.Y2, v.Z2), v.Size);
                cap.Draw(transform, new Vector4(clr, a));
            }
        }

    }
}
