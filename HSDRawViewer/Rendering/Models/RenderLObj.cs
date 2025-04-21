using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.GX;
using HSDRaw.Tools;
using HSDRawViewer.Rendering.Shaders;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using YamlDotNet.Serialization;

namespace HSDRawViewer.Rendering.Models
{
    internal class SplineAnim
    {
        private float Frame;
        private readonly float EndFrame;
        private readonly LiveJObj jobj;
        private readonly List<FOBJ_Player> tracksPosition = new();

        public SplineAnim(HSD_AOBJ aobj)
        {
            Frame = 0;
            EndFrame = aobj.EndFrame;

            if (aobj.ObjectReference != null)
                jobj = new LiveJObj(aobj.ObjectReference);

            if (aobj.FObjDesc != null)
                foreach (HSD_FOBJDesc v in aobj.FObjDesc.List)
                    tracksPosition.Add(new FOBJ_Player(v));
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 AdvanceAnimation(Vector3 input)
        {
            foreach (FOBJ_Player t in tracksPosition)
            {
                switch (t.JointTrackType)
                {
                    case JointTrackType.HSD_A_J_PATH:
                        {
                            if (jobj != null)
                            {
                                jobj.Desc.Spline.GetPointOnPath(t.GetValue(Frame), out float X, out float Y, out float Z);
                                input.X = X;
                                input.Y = Y;
                                input.Z = Z;
                                input = Vector3.TransformPosition(input, jobj.WorldTransform);
                            }
                        }
                        break;
                    case JointTrackType.HSD_A_J_TRAX:
                        input.X = t.GetValue(Frame);
                        break;
                }
            }

            Frame += 1;
            if (Frame >= EndFrame)
                Frame = 0;

            return input;
        }
    }

    public class RenderLObj
    {
        [DisplayName("Enabled")]
        public bool Enabled { get; set; } = false;

        [DisplayName("Light Kind")]
        public LObjType Type { get; set; } = LObjType.AMBIENT;

        [DisplayName("Color"), YamlIgnore]
        public Color LightColor
        {
            get
            {
                return Color.FromArgb((byte)(_color.W * 255), (byte)(_color.X * 255), (byte)(_color.Y * 255), (byte)(_color.Z * 255));
            }
            set
            {
                _color.X = value.R / 255f;
                _color.Y = value.G / 255f;
                _color.Z = value.B / 255f;
                _color.W = value.A / 255f;
            }
        }

        [DisplayName("Diffuse")]
        public bool Diffuse { get; set; } = true;

        [DisplayName("Specular")]
        public bool Specular { get; set; } = true;

        [YamlIgnore]
        public Vector3 _position = Vector3.Zero;

        [YamlIgnore]
        public Vector3 _interest = Vector3.Zero;

        [YamlIgnore]
        public Vector4 _color = Vector4.One;
        public float X { get => _position.X; set => _position.X = value; }
        public float Y { get => _position.Y; set => _position.Y = value; }
        public float Z { get => _position.Z; set => _position.Z = value; }

        [Browsable(false)]
        public byte R { get => (byte)(_color.X * 255); set => _color.X = value / 255f; }
        [Browsable(false)]
        public byte G { get => (byte)(_color.Y * 255); set => _color.Y = value / 255f; }
        [Browsable(false)]
        public byte B { get => (byte)(_color.Z * 255); set => _color.Z = value / 255f; }
        [Browsable(false)]
        public byte A { get => (byte)(_color.W * 255); set => _color.W = value / 255f; }

        private float A0;
        private float A1;
        private float A2;
        private float K0;
        private float K1;
        private float K2;

        [YamlIgnore]
        private float Frame;
        [YamlIgnore]
        private float EndFrame;
        [YamlIgnore]
        private readonly List<FOBJ_Player> Tracks = new();

        [YamlIgnore]
        private SplineAnim PositionAnim;
        [YamlIgnore]
        private SplineAnim InterestAnim;

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
                    foreach (HSD_FOBJDesc v in lightanim.LightAnim.FObjDesc.List)
                        Tracks.Add(new FOBJ_Player(v));
            }

            if (lightanim.PositionAnim != null && lightanim.PositionAnim.Animation != null)
                PositionAnim = new SplineAnim(lightanim.PositionAnim.Animation);
            else
                PositionAnim = null;

            if (lightanim.InterestAnim != null && lightanim.InterestAnim.Animation != null)
                InterestAnim = new SplineAnim(lightanim.InterestAnim.Animation);
            else
                InterestAnim = null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ProcessAnimation()
        {
            if (PositionAnim != null)
                _position = PositionAnim.AdvanceAnimation(_position);

            if (InterestAnim != null)
                _interest = InterestAnim.AdvanceAnimation(_interest);

            foreach (FOBJ_Player t in Tracks)
            {
                float value = t.GetValue(Frame);
                switch ((LightTrackType)t.TrackType)
                {
                    case LightTrackType.HSD_A_L_LITC_R: _color.X = value; break;
                    case LightTrackType.HSD_A_L_LITC_G: _color.Y = value; break;
                    case LightTrackType.HSD_A_L_LITC_B: _color.Z = value; break;
                    case LightTrackType.HSD_A_L_LITC_A: _color.W = value; break;
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
            shader.SetVector3($"light[{index}].position", _position);
            shader.SetVector3($"light[{index}].direction", (_position - _interest).Normalized());
            shader.SetVector3($"light[{index}].color", _color.Xyz);
            int flags = 0;
            if (Diffuse)
                flags |= 0x1;
            if (Specular)
                flags |= 0x2;
            shader.SetInt($"light[{index}].flags", flags);

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

            switch (dist_fn)
            {
                case GXBrightnessDistance.GX_DA_GENTLE:
                    K0 = 1.0f;
                    K1 = (1.0f - ref_brite) / (ref_brite * ref_dist);
                    K2 = 0.0f;
                    break;
                case GXBrightnessDistance.GX_DA_MEDIUM:
                    K0 = 1.0f;
                    K1 = 0.5f * (1.0f - ref_brite) / (ref_brite * ref_dist);
                    K2 = 0.5f * (1.0f - ref_brite) / (ref_brite * ref_dist * ref_dist);
                    break;
                case GXBrightnessDistance.GX_DA_STEEP:
                    K0 = 1.0f;
                    K1 = 0.0f;
                    K2 = (1.0f - ref_brite) / (ref_brite * ref_dist * ref_dist);
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
            if (cut_off < 0.0f || cut_off > 90.0f)
                spotfn = GXSpotFunc.GX_SP_OFF;

            double r = (cut_off * Math.PI) / 180.0f;
            float cr = (float)Math.Cos(r);
            float d;

            switch (spotfn)
            {
                case GXSpotFunc.GX_SP_FLAT:
                    A0 = -1000.0f * cr;
                    A1 = 1000.0f;
                    A2 = 0.0f;
                    break;
                case GXSpotFunc.GX_SP_COS:
                    A0 = -cr / (1.0f - cr);
                    A1 = 1.0f / (1.0f - cr);
                    A2 = 0.0f;
                    break;
                case GXSpotFunc.GX_SP_COS2:
                    A0 = 0.0f;
                    A1 = -cr / (1.0f - cr);
                    A2 = 1.0f / (1.0f - cr);
                    break;
                case GXSpotFunc.GX_SP_SHARP:
                    d = (1.0f - cr) * (1.0f - cr);
                    A0 = (1.0f / d) * cr * (cr - 2.0f);
                    A1 = 2.0f / d;
                    A2 = -1.0f / d;
                    break;
                case GXSpotFunc.GX_SP_RING1:
                    d = (1.0f - cr) * (1.0f - cr);
                    A0 = -4.0f * cr / d;
                    A1 = 4.0f * (1.0f + cr) / d;
                    A2 = -4.0f / d;
                    break;
                case GXSpotFunc.GX_SP_RING2:
                    d = (1.0f - cr) * (1.0f - cr);
                    A0 = 1.0f - 2.0f * cr * cr / d;
                    A1 = 4.0f * cr / d;
                    A2 = -2.0f / d;
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
                HSD_LOBJ lo = v.LightObject;

                // enable light
                Enabled = true;

                // flags
                Diffuse = v.LightObject.Flags.HasFlag(LOBJ_Flags.LOBJ_DIFFUSE);
                Specular = v.LightObject.Flags.HasFlag(LOBJ_Flags.LOBJ_SPECULAR);

                // load type
                Type = (LObjType)((int)lo.Flags & 0x3);

                // load color
                _color = new Vector4(lo.ColorR, lo.ColorG, lo.ColorB, lo.ColorAlpha) / 255f;

                // load position
                if (lo.Position != null)
                    _position = new Vector3(lo.Position.V1, lo.Position.V2, lo.Position.V3);
                else
                    _position = Vector3.Zero;

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
                    HSD_LightAttn att = lo._s.GetReference<HSD_LightAttn>(0x18);
                    A0 = att.A0;
                    A1 = att.A1;
                    A2 = att.A2;
                    K0 = att.K0;
                    K1 = att.K1;
                    K2 = att.K2;
                }

                // load animation if availiable
                PositionAnim = null;
                InterestAnim = null;
                Tracks.Clear();
                if (v.AnimPointer != null && v.AnimPointer.Length > 0)
                {
                    LoadAnimation(v.AnimPointer[0]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Type} {LightColor.ToString()}";
        }
    }
}
