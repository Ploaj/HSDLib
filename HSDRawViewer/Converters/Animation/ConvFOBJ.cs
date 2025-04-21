﻿using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace HSDRawViewer.Converters
{
    public class ConvFOBJ
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static string ToString(HSD_FOBJDesc desc)
        {
            return ToString(desc.ToFOBJ());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="lines"></param>
        public static void ImportKeys(HSD_FOBJDesc desc, string[] lines)
        {
            desc.FromFOBJ(ImportKeys(desc.ToFOBJ(), lines));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fobj"></param>
        /// <returns></returns>
        public static string ToString(HSD_FOBJ fobj)
        {
            StringBuilder sb = new();

            sb.AppendLine(fobj.JointTrackType.ToString());

            foreach (FOBJKey v in fobj.GetDecodedKeys())
            {
                sb.AppendLine($"{v.Frame},{v.Value},{v.Tan},{v.InterpolationType}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fobj"></param>
        /// <param name="lines"></param>
        public static HSD_FOBJ ImportKeys(HSD_FOBJ fobj, string[] lines)
        {
            List<FOBJKey> keys = new();

            JointTrackType animationType = JointTrackType.HSD_A_J_ROTX;

            foreach (string v in lines)
            {
                string[] args = v.Trim().Split(',');

                if (args.Length == 1 && Enum.TryParse<JointTrackType>(args[0], out animationType))
                    continue;

                if (args.Length < 4)
                    continue;

                keys.Add(new FOBJKey()
                {
                    Frame = float.Parse(args[0]),
                    Value = float.Parse(args[1]),
                    Tan = float.Parse(args[2]),
                    InterpolationType = (GXInterpolationType)Enum.Parse(typeof(GXInterpolationType), args[3])
                });
            }

            fobj.SetKeys(keys, animationType);

            return fobj;
        }
    }
}
