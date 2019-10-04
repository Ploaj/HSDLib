using HSDRaw.Common.Animation;
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
            return ToString(desc.FOBJ);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="lines"></param>
        public static void ImportKeys(HSD_FOBJDesc desc, string[] lines)
        {
            if (desc.FOBJ == null)
                desc.FOBJ = new HSD_FOBJ();
            
            desc.FOBJ = ImportKeys(desc.FOBJ, lines);

            desc.DataLength = desc.FOBJ.Buffer.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fobj"></param>
        /// <returns></returns>
        public static string ToString(HSD_FOBJ fobj)
        {
            StringBuilder sb = new StringBuilder();

            foreach(var v in fobj.GetDecodedKeys())
            {
                sb.AppendLine($"{v.Frame} {v.Value} {v.Tan} {v.InterpolationType}");
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
            List<FOBJKey> keys = new List<FOBJKey>();

            foreach(var v in lines)
            {
                var args = v.Split(' ');
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

            fobj.SetKeys(keys, fobj.AnimationType);

            return fobj;
        }
    }
}
