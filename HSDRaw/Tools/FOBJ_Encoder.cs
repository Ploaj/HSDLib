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
        public static HSD_FOBJ EncodeFrames(List<FOBJKey> Keys, JointTrackType TrackType)
        {
            return EncodeFrames(Keys, (byte)TrackType);
        }

        public static HSD_FOBJ EncodeFrames(List<FOBJKey> Keys, byte TrackType)
        {
            HSD_FOBJ fobj = new HSD_FOBJ();
            fobj.JointTrackType = (JointTrackType)TrackType;

            // perform quantization
            FOBJQuantanizer valueQ = new FOBJQuantanizer();
            FOBJQuantanizer tangentQ = new FOBJQuantanizer();

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

                int time = 0;
                for (int i = 0; i < Keys.Count;)
                {
                    GXInterpolationType ip = Keys[i].InterpolationType;
                    int j;
                    for (j = 0; j < Keys.Count - i; j++)
                    {
                        if (Keys[i + j].InterpolationType != ip)
                            break;
                    }

                    int flag = ((j - 1) << 4) | (int)ip;
                    Writer.ExtendedByte(flag);

                    for (int k = i; k < i + j; k++)
                    {
                        int DeltaTime = 0;

                        if (k + 1 < Keys.Count)
                            DeltaTime = (int)(Keys[k + 1].Frame - Keys[k].Frame);

                        if (k == Keys.Count)
                            DeltaTime = 1;

                        switch (ip)
                        {
                            case GXInterpolationType.HSD_A_OP_CON:
                                valueQ.WriteValue(Writer, Keys[k].Value);
                                Writer.ExtendedByte(DeltaTime);
                                break;
                            case GXInterpolationType.HSD_A_OP_LIN:
                                valueQ.WriteValue(Writer, Keys[k].Value);
                                Writer.ExtendedByte(DeltaTime);
                                break;
                            case GXInterpolationType.HSD_A_OP_SPL0:
                                valueQ.WriteValue(Writer, Keys[k].Value);
                                Writer.ExtendedByte(DeltaTime);
                                break;
                            case GXInterpolationType.HSD_A_OP_SPL:
                                valueQ.WriteValue(Writer, Keys[k].Value);
                                tangentQ.WriteValue(Writer, Keys[k].Tan);
                                Writer.ExtendedByte(DeltaTime);
                                break;
                            case GXInterpolationType.HSD_A_OP_SLP:
                                tangentQ.WriteValue(Writer, Keys[k].Tan);
                                break;
                            case GXInterpolationType.HSD_A_OP_KEY:
                                valueQ.WriteValue(Writer, Keys[k].Value);
                                break;
                        }

                        if (ip != GXInterpolationType.HSD_A_OP_SLP)
                            time = (int)Keys[k].Frame;
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
