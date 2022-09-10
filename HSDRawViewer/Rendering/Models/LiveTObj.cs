using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.GX;
using HSDRaw.Tools;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Rendering.Models
{

    /// <summary>
    /// 
    /// </summary>
    public class LiveTObj
    {
        public HSD_TOBJ TOBJ { get; set; }

        public float Blending { get; set; }

        public Matrix4 Transform { get; set; }

        public Vector4 Konst;
        public Vector4 Tev0;
        public Vector4 Tev1;

        private float TX;
        private float TY;
        private float TZ;
        private float RX;
        private float RY;
        private float RZ;
        private float SX;
        private float SY;
        private float SZ;

        /// <summary>
        /// 
        /// </summary>
        public LiveTObj()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix4 MakeMatrix()
        {
            if (TOBJ == null)
                return Matrix4.Identity;

            //trans.x = -tobj->translate.x;
            //trans.y = -(tobj->translate.y + (tobj->wrap_t == GX_MIRROR ? 1.0F / ((f32)tobj->repeat_t / tobj->scale.y) : 0.0F));
            //trans.z = tobj->translate.z;
            var trans = Matrix4.CreateTranslation(
                -TX,
                -(TY + (TOBJ.WrapT == GXWrapMode.MIRROR ? 1f / (TOBJ.RepeatT / SY) : 0f)),
                TZ);

            //rot.x = tobj->rotate.x;
            //rot.y = tobj->rotate.y;
            //rot.z = -tobj->rotate.z;
            var rot = Math3D.CreateMatrix4FromEuler(
                RX,
                RY,
                -RZ);

            //scale.x = fabsf(tobj->scale.x) < FLT_EPSILON ? 0.0F : (f32)tobj->repeat_s / tobj->scale.x;
            //scale.y = fabsf(tobj->scale.y) < FLT_EPSILON ? 0.0F : (f32)tobj->repeat_t / tobj->scale.y;
            //scale.z = tobj->scale.z;
            var scale = Matrix4.CreateScale(
                Math.Abs(SX) < Single.Epsilon ? 0 : TOBJ.RepeatS / SX,
                Math.Abs(SY) < Single.Epsilon ? 0 : TOBJ.RepeatT / SY,
                SZ);

            // make trs matrix
            return trans * rot * scale;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobj"></param>
        public void Reset(HSD_TOBJ t)
        {
            if (t == null)
                return;

            // initialize tobj data
            TOBJ = t;
            Blending = t.Blending;

            // transform
            TX = t.TX;
            TY = t.TY;
            TZ = t.TZ;
            SX = t.SX;
            SY = t.SY;
            SZ = t.SZ;
            RX = t.RX;
            RY = t.RY;
            RZ = t.RZ;

            // calculate transform
            Transform = MakeMatrix();

            // calcualte tev
            Konst = Vector4.Zero;
            Tev0 = Vector4.Zero;
            Tev1 = Vector4.Zero;

            // initialize tev data
            if (t.TEV != null)
            {
                var k = t.TEV.constant;
                var t0 = t.TEV.tev0;
                var t1 = t.TEV.tev1;

                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.KONST_R))
                    Konst.X = k.R / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.KONST_G))
                    Konst.Y = k.G / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.KONST_B))
                    Konst.Z = k.B / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.KONST_A))
                    Konst.W = t.TEV.constantAlpha / 255f;

                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.TEV0_R))
                    Tev0.X = t0.R / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.TEV0_G))
                    Tev0.Y = t0.G / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.TEV0_B))
                    Tev0.Z = t0.B / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.TEV0_A))
                    Tev0.W = t.TEV.tev0Alpha / 255f;

                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.TEV1_R))
                    Tev1.X = t1.R / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.TEV1_G))
                    Tev1.Y = t1.G / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.TEV1_B))
                    Tev1.Z = t1.B / 255f;
                if (t.TEV.active.HasFlag(TOBJ_TEVREG_ACTIVE.TEV1_A))
                    Tev1.W = t.TEV.tev1Alpha / 255f;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tracks"></param>
        /// <param name="frame"></param>
        public void ApplyAnim(List<HSD_TOBJ> textures, List<FOBJ_Player> tracks, float frame)
        {
            foreach (var t in tracks)
            {
                var value = t.GetValue(frame);

                switch ((TexTrackType)t.TrackType)
                {
                    case TexTrackType.HSD_A_T_TIMG:
                        if (value < textures.Count)
                            TOBJ = textures[(int)value];
                        else if (textures.Count > 0)
                            TOBJ = textures[0];
                        else
                            TOBJ = null;
                        break;
                    case TexTrackType.HSD_A_T_BLEND:
                    case TexTrackType.HSD_A_T_TS_BLEND:
                        Blending = value;
                        break;
                    case TexTrackType.HSD_A_T_TRAU: TX = value; break;
                    case TexTrackType.HSD_A_T_TRAV: TY = value; break;
                    case TexTrackType.HSD_A_T_SCAU: SX = value; break;
                    case TexTrackType.HSD_A_T_SCAV: SY = value; break;
                    case TexTrackType.HSD_A_T_ROTX: RX = value; break;
                    case TexTrackType.HSD_A_T_ROTY: RY = value; break;
                    case TexTrackType.HSD_A_T_ROTZ: RZ = value; break;
                    case TexTrackType.HSD_A_T_KONST_R: Konst.X = value; break;
                    case TexTrackType.HSD_A_T_KONST_G: Konst.Y = value; break;
                    case TexTrackType.HSD_A_T_KONST_B: Konst.Z = value; break;
                    case TexTrackType.HSD_A_T_KONST_A: Konst.W = value; break;
                    case TexTrackType.HSD_A_T_TEV0_R: Tev0.X = value; break;
                    case TexTrackType.HSD_A_T_TEV0_G: Tev0.Y = value; break;
                    case TexTrackType.HSD_A_T_TEV0_B: Tev0.Z = value; break;
                    case TexTrackType.HSD_A_T_TEV0_A: Tev0.W = value; break;
                    case TexTrackType.HSD_A_T_TEV1_R: Tev1.X = value; break;
                    case TexTrackType.HSD_A_T_TEV1_G: Tev1.Y = value; break;
                    case TexTrackType.HSD_A_T_TEV1_B: Tev1.Z = value; break;
                    case TexTrackType.HSD_A_T_TEV1_A: Tev1.W = value; break;
                }
            }

            // calculate transform
            Transform = MakeMatrix();
        }
    }
}
