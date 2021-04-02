using HSDRaw.Common.Animation;
using System;
using System.Collections.Generic;
using System.IO;

namespace HSDRaw.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class FOBJFrameEncoder
    {
        public static HSD_FOBJ EncodeFrames(List<FOBJKey> Keys, JointTrackType TrackType, float error = 0.0001f)
        {
            return EncodeFrames(Keys, (byte)TrackType, error);
        }

        public static HSD_FOBJ EncodeFrames(List<FOBJKey> Keys, byte TrackType, float error = 0.0001f)
        {
            HSD_FOBJ fobj = new HSD_FOBJ();
            fobj.JointTrackType = (JointTrackType)TrackType;

            if (fobj.JointTrackType == JointTrackType.HSD_A_J_PTCL)
                return fobj;

            // automatically set single key interpolation type
            if (Keys.Count == 1)
                Keys[0].InterpolationType = GXInterpolationType.HSD_A_OP_KEY;

            // perform quantization
            FOBJQuantanizer valueQ = new FOBJQuantanizer(error);
            FOBJQuantanizer tangentQ = new FOBJQuantanizer(error);

            foreach (FOBJKey key in Keys)
            {
                valueQ.AddValue(key.Value);
                tangentQ.AddValue(key.Tan);
            }

            fobj.ValueScale = valueQ.GetValueScale();
            fobj.ValueFormat = valueQ.GetDataFormat();

            fobj.TanScale = tangentQ.GetValueScale();
            fobj.TanFormat = tangentQ.GetDataFormat();

            MemoryStream o = new MemoryStream();
            using (BinaryWriterExt Writer = new BinaryWriterExt(o))
            {
                Writer.BigEndian = false;

                for (int i = 0; i < Keys.Count;)
                {
                    GXInterpolationType ip = Keys[i].InterpolationType;
                    int j;
                    for (j = 0; j < Keys.Count - i; j++)
                    {
                        if (Keys[i + j].InterpolationType != ip)
                            break;
                    }

                    if (j > 0x7FF)
                        j = 0x7FF;

                    int flag = ((j - 1) << 4) | (int)ip;
                    Writer.WritePacked(flag);

                    for (int k = i; k < i + j; k++)
                    {
                        int DeltaTime = 0;

                        if (k + 1 < Keys.Count)
                        {
                            var nextKey = 
                                Keys.Find(e => 
                                e.Frame > Keys[k].Frame && 
                                e.InterpolationType != GXInterpolationType.HSD_A_OP_SLP);

                            if (nextKey != null)
                                DeltaTime = (int)(nextKey.Frame - Keys[k].Frame);
                        }

                        if (k == Keys.Count)
                            DeltaTime = 1;

                        switch (ip)
                        {
                            case GXInterpolationType.HSD_A_OP_CON:
                                valueQ.WriteValue(Writer, Keys[k].Value);
                                Writer.WritePacked(DeltaTime);
                                break;
                            case GXInterpolationType.HSD_A_OP_LIN:
                                valueQ.WriteValue(Writer, Keys[k].Value);
                                Writer.WritePacked(DeltaTime);
                                break;
                            case GXInterpolationType.HSD_A_OP_SPL0:
                                valueQ.WriteValue(Writer, Keys[k].Value);
                                Writer.WritePacked(DeltaTime);
                                break;
                            case GXInterpolationType.HSD_A_OP_SPL:
                                valueQ.WriteValue(Writer, Keys[k].Value);
                                tangentQ.WriteValue(Writer, Keys[k].Tan);
                                Writer.WritePacked(DeltaTime);
                                break;
                            case GXInterpolationType.HSD_A_OP_SLP:
                                tangentQ.WriteValue(Writer, Keys[k].Tan);
                                break;
                            case GXInterpolationType.HSD_A_OP_KEY:
                                valueQ.WriteValue(Writer, Keys[k].Value);
                                break;
                        }
                    }

                    i += j;
                }
            }
            fobj.Buffer = o.ToArray();
            o.Dispose();
            return fobj;
        }

    }
}
