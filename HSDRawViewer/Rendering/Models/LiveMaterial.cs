using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace HSDRawViewer.Rendering.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class LiveMaterial
    {
        public Vector4 Ambient;

        public Vector4 Diffuse;

        public Vector4 Specular;

        public float Shininess;

        public float Alpha;

        public float Ref0;

        public float Ref1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobj"></param>
        public void Reset(HSD_MOBJ mobj)
        {
            if (mobj == null || mobj.Material == null)
                return;

            var color = mobj.Material;

            Ambient.X = color.AMB_R / 255f;
            Ambient.Y = color.AMB_G / 255f;
            Ambient.Z = color.AMB_B / 255f;
            Ambient.W = color.AMB_A / 255f;

            Diffuse.X = color.DIF_R / 255f;
            Diffuse.Y = color.DIF_G / 255f;
            Diffuse.Z = color.DIF_B / 255f;
            Diffuse.W = color.DIF_A / 255f;

            Specular.X = color.SPC_R / 255f;
            Specular.Y = color.SPC_G / 255f;
            Specular.Z = color.SPC_B / 255f;
            Specular.W = color.SPC_A / 255f;

            Shininess = color.Shininess;
            Alpha = color.Alpha;

            var pp = mobj.PEDesc;
            if (pp != null)
            {
                Ref0 = pp.AlphaRef0 / 255f;
                Ref1 = pp.AlphaRef1 / 255f;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tracks"></param>
        /// <param name="frame"></param>
        public void ApplyAnim(List<FOBJ_Player> tracks, float frame)
        {
            foreach (var t in tracks)
            {
                switch ((MatTrackType)t.TrackType)
                {
                    case MatTrackType.HSD_A_M_PE_REF0: Ref0 = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_PE_REF1: Ref1 = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_ALPHA: Alpha = 1 - t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_AMBIENT_R: Ambient.X = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_AMBIENT_G: Ambient.Y = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_AMBIENT_B: Ambient.Z = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_DIFFUSE_R: Diffuse.X = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_DIFFUSE_G: Diffuse.Y = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_DIFFUSE_B: Diffuse.Z = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_SPECULAR_R: Specular.X = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_SPECULAR_G: Specular.Y = t.GetValue(frame); break;
                    case MatTrackType.HSD_A_M_SPECULAR_B: Specular.Z = t.GetValue(frame); break;
                }
            }
        }
    }
}
