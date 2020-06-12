using HSDRaw.Common.Animation;
using System.Collections.Generic;
using System.Linq;

namespace HSDRaw.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class FOBJAnimState
    {
        public float p0 = 0;
        public float p1 = 0;
        public float d0 = 0;
        public float d1 = 0;
        public float t0 = 0;
        public float t1 = 0;
        public GXInterpolationType op_intrp = GXInterpolationType.HSD_A_OP_CON;
        public GXInterpolationType op = GXInterpolationType.HSD_A_OP_CON;

        public override bool Equals(object obj)
        {
            if (obj is FOBJAnimState state)
            {
                return p0 == state.p0 && p1 == state.p1 &&
                    d0 == state.d0 && d1 == state.d1 &&
                    t0 == state.t0 && t1 == state.t1 &&
                    op == state.op && op_intrp == state.op_intrp;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class FOBJ_Player
    {
        public List<FOBJKey> Keys;
        public byte TrackType;
        public JointTrackType JointTrackType { get => (JointTrackType)TrackType; set => TrackType = (byte)value; }

        public FOBJ_Player()
        {
            Keys = new List<FOBJKey>();
            JointTrackType = JointTrackType.HSD_A_J_TRAX;
        }

        public FOBJ_Player(HSD_FOBJDesc fobj)
        {
            Keys = fobj.GetDecodedKeys();
            TrackType = fobj.TrackType;
        }

        public FOBJ_Player(HSD_FOBJ fobj)
        {
            Keys = fobj.GetDecodedKeys();
            TrackType = fobj.TrackType;
        }

        public FOBJ_Player(byte trackType, IEnumerable<FOBJKey> keys)
        {
            Keys = keys.ToList();
            TrackType = trackType;
        }

        public int FrameCount
        {
            get
            {
                if (Keys == null || Keys.Count == 0)
                    return 0;

                return (int)Keys.Max(e => e.Frame);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Frame"></param>
        /// <returns></returns>
        public FOBJAnimState GetState(float Frame)
        {
            // clamp
            if(Keys.Count > 1 && Frame >= Keys[Keys.Count - 1].Frame)
            {
                var key = Keys[Keys.Count - 1];
                return new FOBJAnimState()
                {
                    t0 = key.Frame,
                    t1 = key.Frame,
                    p0 = key.Value,
                    p1 = key.Value,
                    d0 = key.Tan,
                    d1 = key.Tan,
                    op = GXInterpolationType.HSD_A_OP_CON,
                    op_intrp = GXInterpolationType.HSD_A_OP_CON
                };
            }

            // register
            float p0 = 0;
            float p1 = 0;
            float d0 = 0;
            float d1 = 0;
            float t0 = 0;
            float t1 = 0;
            GXInterpolationType op_intrp = GXInterpolationType.HSD_A_OP_CON;
            GXInterpolationType op = GXInterpolationType.HSD_A_OP_CON;

            // get current frame state
            for (int i = 0; i < Keys.Count; i++)
            {
                op_intrp = op;
                op = Keys[i].InterpolationType;

                switch (op)
                {
                    case GXInterpolationType.HSD_A_OP_CON:
                        p0 = p1;
                        p1 = Keys[i].Value;
                        if (op_intrp != GXInterpolationType.HSD_A_OP_SLP)
                        {
                            d0 = d1;
                            d1 = 0;
                        }
                        t0 = t1;
                        t1 = Keys[i].Frame;
                        break;
                    case GXInterpolationType.HSD_A_OP_LIN:
                        p0 = p1;
                        p1 = Keys[i].Value;
                        if (op_intrp != GXInterpolationType.HSD_A_OP_SLP)
                        {
                            d0 = d1;
                            d1 = 0;
                        }
                        t0 = t1;
                        t1 = Keys[i].Frame;
                        break;
                    case GXInterpolationType.HSD_A_OP_SPL0:
                        p0 = p1;
                        d0 = d1;
                        p1 = Keys[i].Value;
                        d1 = 0;
                        t0 = t1;
                        t1 = Keys[i].Frame;
                        break;
                    case GXInterpolationType.HSD_A_OP_SPL:
                        p0 = p1;
                        p1 = Keys[i].Value;
                        d0 = d1;
                        d1 = Keys[i].Tan;
                        t0 = t1;
                        t1 = Keys[i].Frame;
                        break;
                    case GXInterpolationType.HSD_A_OP_SLP:
                        d0 = d1;
                        d1 = Keys[i].Tan;
                        break;
                    case GXInterpolationType.HSD_A_OP_KEY:
                        p1 = Keys[i].Value;
                        p0 = Keys[i].Value;
                        break;
                }

                if (t1 > Frame && Keys[i].InterpolationType != GXInterpolationType.HSD_A_OP_SLP)
                    break;

                op_intrp = Keys[i].InterpolationType;
            }
            return new FOBJAnimState()
            {
                t0 = t0,
                t1 = t1,
                p0 = p0,
                p1 = p1,
                d0 = d0,
                d1 = d1,
                op = op,
                op_intrp = op_intrp
            };
        }

        public float GetValue(float Frame)
        {
            var state = GetState(Frame);

            if (Frame == state.t0)
                return state.p0;

            if (Frame == state.t1)
                return state.p1;

            if (state.t0 == state.t1 || state.op_intrp == GXInterpolationType.HSD_A_OP_CON || state.op_intrp == GXInterpolationType.HSD_A_OP_KEY)
                return state.p0;

            float FrameDiff = Frame - state.t0;
            float Weight = FrameDiff / (state.t1 - state.t0);

            if (state.op_intrp == GXInterpolationType.HSD_A_OP_LIN)
                return AnimationInterpolationHelper.Lerp(state.p0, state.p1, Weight);

            if (state.op_intrp == GXInterpolationType.HSD_A_OP_SPL || state.op_intrp == GXInterpolationType.HSD_A_OP_SPL0 || state.op_intrp == GXInterpolationType.HSD_A_OP_SLP)
                return AnimationInterpolationHelper.SplineGetHermite(1 / (state.t1 - state.t0), FrameDiff, state.p0, state.p1, state.d0, state.d1);

            return state.p0;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class AnimationInterpolationHelper
    {
        public static float Lerp(float LHS, float RHS, float Weight)
        {
            return LHS * (1 - Weight) + RHS * Weight;
        }

        public static float SplineGetHermite(float fterm, float time, float p0, float p1, float d0, float d1)
        {
            float fVar1;
            float fVar2;
            float fVar3;
            float fVar4;

            fVar1 = time * time;
            fVar2 = fterm * fterm * fVar1 * time;
            fVar3 = 3.0f * fVar1 * fterm * fterm;
            fVar4 = fVar2 - fVar1 * fterm;
            fVar2 = 2.0f * fVar2 * fterm;
            return d1 * fVar4 + d0 * (time + (fVar4 - fVar1 * fterm)) + p0 * (1.0f + (fVar2 - fVar3)) + p1 * (-fVar2 + fVar3);
        }
    }
}
