using HSDRaw.Common.Animation;
using System;
using System.Collections.Generic;
using System.IO;

namespace HSDRaw.Tools
{
    /// <summary>
    /// A generic animation keyframe
    /// </summary>
    public class FOBJKey
    {
        public float Frame { get; set; }
        public float Value { get; set; }
        public float Tan { get; set; }
        public GXInterpolationType InterpolationType { get; set; }
    }

    /// <summary>
    /// Tool for decoding FOBJ Keys into <see cref="FOBJKey"/>
    /// </summary>
    public class FOBJFrameDecoder
    {
        public HSD_FOBJ FOBJ { get; set; }
        private BinaryReaderExt Reader;
        private MemoryStream Stream;

        public FOBJFrameDecoder(HSD_FOBJ FOBJ)
        {
            if (FOBJ.Buffer == null)
                FOBJ.SetKeys(new List<FOBJKey>() { new FOBJKey() }, JointTrackType.HSD_A_J_ROTX);
            Stream = new MemoryStream(FOBJ.Buffer);
            Reader = new BinaryReaderExt(Stream);
            this.FOBJ = FOBJ;
        }

        public static List<FOBJKey> GetKeys(HSD_FOBJ FOBJ)
        {
            FOBJFrameDecoder e = new FOBJFrameDecoder(FOBJ);
            {
                return e.GetKeys();
            }
        }

        public List<FOBJKey> GetKeys(float FrameCount = -1)
        {
            List<FOBJKey> Keys = new List<FOBJKey>();
            int clock = 0;
            Reader.Seek(0);
            while (Reader.Position < Reader.BaseStream.Length)
            {
                int type = Reader.ReadPacked();
                GXInterpolationType interpolation = (GXInterpolationType)((type) & 0x0F);
                int numOfKey = ((type >> 4)) + 1;
                if (interpolation == 0) break;

                for (int i = 0; i < numOfKey; i++)
                {
                    double value = 0;
                    double tan = 0;
                    int time = 0;
                    switch (interpolation)
                    {
                        case GXInterpolationType.HSD_A_OP_CON:
                            value = ParseFloat(Reader, FOBJ.ValueFormat, FOBJ.ValueScale);
                            time = Reader.ReadPacked();
                            break;
                        case GXInterpolationType.HSD_A_OP_LIN:
                            value = ParseFloat(Reader, FOBJ.ValueFormat, FOBJ.ValueScale);
                            time = Reader.ReadPacked();
                            break;
                        case GXInterpolationType.HSD_A_OP_SPL0:
                            value = ParseFloat(Reader, FOBJ.ValueFormat, FOBJ.ValueScale);
                            time = Reader.ReadPacked();
                            break;
                        case GXInterpolationType.HSD_A_OP_SPL:
                            value = ParseFloat(Reader, FOBJ.ValueFormat, FOBJ.ValueScale);
                            tan = ParseFloat(Reader, FOBJ.TanFormat, FOBJ.TanScale);
                            time = Reader.ReadPacked();
                            break;
                        case GXInterpolationType.HSD_A_OP_SLP:
                            tan = ParseFloat(Reader, FOBJ.TanFormat, FOBJ.TanScale);
                            break;
                        case GXInterpolationType.HSD_A_OP_KEY:
                            value = ParseFloat(Reader, FOBJ.ValueFormat, FOBJ.ValueScale);
                            break;
                        default:
                            throw new Exception("Unknown Interpolation Type " + interpolation.ToString("X"));
                    }

                    FOBJKey kf = new FOBJKey();
                    kf.InterpolationType = interpolation;
                    kf.Value = (float)value;
                    kf.Frame = clock;
                    kf.Tan = (float)tan;
                    Keys.Add(kf);
                    clock += time;
                }
            }
            return Keys;
        }

        private static double ParseFloat(BinaryReaderExt d, GXAnimDataFormat Format, float Scale)
        {
            d.BigEndian = false;
            switch (Format)
            {
                case GXAnimDataFormat.HSD_A_FRAC_FLOAT:
                    return d.ReadSingle();
                case GXAnimDataFormat.HSD_A_FRAC_S16:
                    return d.ReadInt16() / (double)Scale;
                case GXAnimDataFormat.HSD_A_FRAC_U16:
                    return d.ReadUInt16() / (double)Scale;
                case GXAnimDataFormat.HSD_A_FRAC_S8:
                    return d.ReadSByte() / (double)Scale;
                case GXAnimDataFormat.HSD_A_FRAC_U8:
                    return d.ReadByte() / (double)Scale;
                default:
                    return 0;
            }
        }
    }
}
