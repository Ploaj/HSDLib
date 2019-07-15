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
        public float Frame;
        public float Value;
        public float Tan;
        public GXInterpolationType InterpolationType;
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
                int type = Reader.ExtendedByte();
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
                        case GXInterpolationType.Step:
                            value = ReadVal(Reader, FOBJ.ValueFormat, FOBJ.ValueScale);
                            time = Reader.ExtendedByte();
                            break;
                        case GXInterpolationType.Linear:
                            value = ReadVal(Reader, FOBJ.ValueFormat, FOBJ.ValueScale);
                            time = Reader.ExtendedByte();
                            break;
                        case GXInterpolationType.HermiteValue:
                            value = ReadVal(Reader, FOBJ.ValueFormat, FOBJ.ValueScale);
                            time = Reader.ExtendedByte();
                            break;
                        case GXInterpolationType.Hermite:
                            value = ReadVal(Reader, FOBJ.ValueFormat, FOBJ.ValueScale);
                            tan = ReadVal(Reader, FOBJ.TanFormat, FOBJ.TanScale);
                            time = Reader.ExtendedByte();
                            break;
                        case GXInterpolationType.HermiteCurve:
                            tan = ReadVal(Reader, FOBJ.TanFormat, FOBJ.TanScale);
                            break;
                        case GXInterpolationType.Constant:
                            value = ReadVal(Reader, FOBJ.ValueFormat, FOBJ.ValueScale);
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

        private static double ReadVal(BinaryReaderExt d, GXAnimDataFormat Format, float Scale)
        {
            d.BigEndian = false;
            switch (Format)
            {
                case GXAnimDataFormat.Float:
                    return d.ReadSingle();
                case GXAnimDataFormat.Short:
                    return d.ReadInt16() / (double)Scale;
                case GXAnimDataFormat.UShort:
                    return d.ReadUInt16() / (double)Scale;
                case GXAnimDataFormat.SByte:
                    return d.ReadSByte() / (double)Scale;
                case GXAnimDataFormat.Byte:
                    return d.ReadByte() / (double)Scale;
                default:
                    return 0;
            }
        }
    }
}
