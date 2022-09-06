using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.GX;
using HSDRaw.Tools;
using HSDRawViewer.Rendering.Shaders;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.Rendering.Models
{
    public class RenderLObj
    {
        public bool Enabled { get; set; } = false;

        public LObjType Type { get; set; } = LObjType.AMBIENT;

        public Vector3 Position = Vector3.Zero;

        public Vector3 Interest = Vector3.Zero;

        public Vector4 Color = Vector4.One;

        // flags
        public bool Diffuse = true;

        public bool Specular = true;

        private float A0;
        private float A1;
        private float A2;
        private float K0;
        private float K1;
        private float K2;

        private float Frame;
        private float EndFrame;
        private List<FOBJ_Player> Tracks = new List<FOBJ_Player>();

        private class SplineAnim
        {
            private float Frame;
            private float EndFrame;
            private LiveJObj jobj;
            private List<FOBJ_Player> tracksPosition = new List<FOBJ_Player>();

            private List<Vector3> Points = new List<Vector3>();
            private List<float> Lengths = new List<float>();

            public SplineAnim(HSD_AOBJ aobj)
            {
                Frame = 0;
                EndFrame = aobj.EndFrame;

                if (aobj.ObjectReference != null)
                    jobj = new LiveJObj(aobj.ObjectReference);

                if (aobj.FObjDesc != null)
                    foreach (var v in aobj.FObjDesc.List)
                        tracksPosition.Add(new FOBJ_Player(v));
            }

            /// <summary>
            /// 
            /// </summary>
            public void AdvanceAnimation(RenderLObj lobj)
            {
                foreach (var t in tracksPosition)
                {
                    switch (t.JointTrackType)
                    {
                        case JointTrackType.HSD_A_J_PATH:
                            {
                                if (jobj != null)
                                {
                                    jobj.Desc.Spline.GetPointOnPath(t.GetValue(Frame), out float X, out float Y, out float Z);
                                    lobj.Position.X = X;
                                    lobj.Position.Y = Y;
                                    lobj.Position.Z = Z;
                                    lobj.Position = Vector3.TransformPosition(lobj.Position, jobj.WorldTransform);
                                }
                            }
                            break;
                        case JointTrackType.HSD_A_J_TRAX:
                            lobj.Position.X = t.GetValue(Frame);
                            break;
                    }
                }

                Frame += 1;
                if (Frame >= EndFrame)
                    Frame = 0;
            }
        }

        private SplineAnim PositionAnim;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lightanim"></param>
        private void LoadAnimation(HSD_LightAnimPointer lightanim)
        {
            if (lightanim.LightAnim != null)
            {
                Frame = 0;
                EndFrame = lightanim.LightAnim.EndFrame;

                Tracks.Clear();
                if (lightanim.LightAnim.FObjDesc != null)
                    foreach (var v in lightanim.LightAnim.FObjDesc.List)
                        Tracks.Add(new FOBJ_Player(v));
            }

            if (lightanim.PositionAnim != null && lightanim.PositionAnim.Animation != null)
                PositionAnim = new SplineAnim(lightanim.PositionAnim.Animation);
            else
                PositionAnim = null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ProcessAnimation()
        {
            if (PositionAnim != null)
                PositionAnim.AdvanceAnimation(this);

            foreach (var t in Tracks)
            {
                float value = t.GetValue(Frame);
                switch ((LightTrackType)t.TrackType)
                {
                    case LightTrackType.HSD_A_L_LITC_R: Color.X = value; break;
                    case LightTrackType.HSD_A_L_LITC_G: Color.Y = value; break;
                    case LightTrackType.HSD_A_L_LITC_B: Color.Z = value; break;
                    case LightTrackType.HSD_A_L_LITC_A: Color.W = value; break;
                    case LightTrackType.HSD_A_L_A0: A0 = value; break;
                    case LightTrackType.HSD_A_L_A1: A1 = value; break;
                    case LightTrackType.HSD_A_L_A2: A2 = value; break;
                    case LightTrackType.HSD_A_L_K0: K0 = value; break;
                    case LightTrackType.HSD_A_L_K1: K1 = value; break;
                    case LightTrackType.HSD_A_L_K2: K2 = value; break;
                    case LightTrackType.HSD_A_L_VIS: Enabled = value == 1; break;
                    case LightTrackType.HSD_A_L_CUTOFF:
                        break;
                    case LightTrackType.HSD_A_L_REFDIST:
                        break;
                    case LightTrackType.HSD_A_L_REFBRIGHT:
                        break;
                }
            }

            Frame++;
            if (Frame >= EndFrame)
                Frame = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="index"></param>
        public void Bind(GXShader shader, int index)
        {
            // process animation if it exists
            ProcessAnimation();

            // set common params
            shader.SetBoolToInt($"light[{index}].enabled", Enabled);
            shader.SetInt($"light[{index}].type", (int)Type);
            shader.SetVector3($"light[{index}].position", Position);
            shader.SetVector3($"light[{index}].direction", (Interest - Position).Normalized());
            shader.SetVector3($"light[{index}].color", Color.Xyz);

            // set attenuation
            shader.SetBoolToInt($"light[{index}].atten_enabled", Type == LObjType.POINT || Type == LObjType.SPOT);
            shader.SetFloat($"light[{index}].a0", A0);
            shader.SetFloat($"light[{index}].a1", A1);
            shader.SetFloat($"light[{index}].a2", A2);
            shader.SetFloat($"light[{index}].k0", K0);
            shader.SetFloat($"light[{index}].k1", K1);
            shader.SetFloat($"light[{index}].k2", K2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ref_dist"></param>
        /// <param name="ref_brite"></param>
        /// <param name="dist_fn"></param>
        private void InitLightDistAttn(float ref_dist, float ref_brite, GXBrightnessDistance dist_fn)
        {
            if (ref_dist < 0.0f || ref_brite < 0.0f || ref_brite >= 1.0f) 
                dist_fn = GXBrightnessDistance.GX_DA_OFF;

            switch(dist_fn) 
            {
                case GXBrightnessDistance.GX_DA_GENTLE:
                    K0 = 1.0f;
                    K1 = (1.0f-ref_brite)/(ref_brite* ref_dist);
                    K2 = 0.0f;
                break;
                case GXBrightnessDistance.GX_DA_MEDIUM:
                    K0 = 1.0f;
                    K1 = 0.5f*(1.0f-ref_brite)/(ref_brite* ref_dist);
                    K2 = 0.5f*(1.0f-ref_brite)/(ref_brite* ref_dist* ref_dist);
                break;
                case GXBrightnessDistance.GX_DA_STEEP:
                    K0 = 1.0f;
                    K1 = 0.0f;
                    K2 = (1.0f-ref_brite)/(ref_brite* ref_dist* ref_dist);
                break;
                case GXBrightnessDistance.GX_DA_OFF:
                default:
                    K0 = 1.0f;
                    K1 = 0.0f;
                    K2 = 0.0f;
                break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cut_off"></param>
        /// <param name="spotfn"></param>
        private void InitLightSpot(float cut_off, GXSpotFunc spotfn)
        {
            if(cut_off < 0.0f ||	cut_off > 90.0f) 
                spotfn = GXSpotFunc.GX_SP_OFF;

            var r = (cut_off * Math.PI) / 180.0f;
            var cr = (float)Math.Cos(r);
            float d;

            switch(spotfn) 
            {
                case GXSpotFunc.GX_SP_FLAT:
                    A0 = -1000.0f*cr;
                    A1 = 1000.0f;
                    A2 = 0.0f;
                break;
                case GXSpotFunc.GX_SP_COS:
                    A0 = -cr/(1.0f-cr);
                    A1 = 1.0f/(1.0f-cr);
                    A2 = 0.0f;
                break;
                case GXSpotFunc.GX_SP_COS2:
                    A0 = 0.0f;
                    A1 = -cr/(1.0f-cr);
                    A2 = 1.0f/(1.0f-cr);
                break;
                case GXSpotFunc.GX_SP_SHARP:
                    d = (1.0f-cr)*(1.0f-cr);
                    A0 = cr* (cr-2.0f);
                    A1 = 2.0f/d;
                    A2 = -1.0f/d;
                break;
                case GXSpotFunc.GX_SP_RING1:
                    d = (1.0f-cr)*(1.0f-cr);
                    A0 = -4.0f*cr/d;
                    A1 = 4.0f*(1.0f+cr)/d;
                    A2 = -4.0f/d;
                break;
                case GXSpotFunc.GX_SP_RING2:
                    d = (1.0f-cr)*(1.0f-cr);
                    A0 = 1.0f-2.0f*cr* cr/d;
                    A1 = 4.0f*cr/d;
                    A2 = -2.0f/d;
                break;
                case GXSpotFunc.GX_SP_OFF:
                default:
                    A0 = 1.0f;
                    A1 = 0.0f;
                    A2 = 0.0f;
                break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public void LoadLight(HSD_Light v)
        {
            if (v == null)
                return;

            Tracks.Clear();

            if (v.LightObject != null)
            {
                var lo = v.LightObject;

                // enable light
                Enabled = true;

                // load type
                Type = (LObjType)((int)lo.Flags & 0x3);

                // load color
                Color = new Vector4(lo.ColorR, lo.ColorG, lo.ColorB, lo.ColorAlpha) / 255f;

                // load position
                if (lo.Position != null)
                    Position = new Vector3(lo.Position.V1, lo.Position.V2, lo.Position.V3);
                else
                    Position = Vector3.Zero;

                // init atten data
                if (lo.SpotData != null)
                {
                    InitLightDistAttn(lo.SpotData.RefDistance, lo.SpotData.RefBrightness, lo.SpotData.DistFunc);
                    InitLightSpot(lo.SpotData.Cutoff, lo.SpotData.SpotFunc);
                }
                else
                if (lo.PointData != null)
                {
                    InitLightDistAttn(lo.PointData.RefDistance, lo.PointData.RefBrightness, lo.PointData.Flag);
                    InitLightSpot(0, GXSpotFunc.GX_SP_OFF);
                }
                else
                if (lo.AttenuationFlags == LOBJ_AttenuationFlags.LOBJ_LIGHT_ATTN)
                {
                    var att = lo._s.GetReference<HSD_LightAttn>(0x18);
                    A0 = att.A0;
                    A1 = att.A1;
                    A2 = att.A2;
                    K0 = att.K0;
                    K1 = att.K1;
                    K2 = att.K2;
                }

                // load animation if availiable
                if (v.AnimPointer != null && v.AnimPointer.Length > 0)
                {
                    LoadAnimation(v.AnimPointer[0]);
                }
            }
        }
    }
}
