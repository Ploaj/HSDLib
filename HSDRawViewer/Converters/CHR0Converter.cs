using HSDRaw;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Rendering;
using OpenTK;
using System;
using System.IO;
using System.Linq;

namespace HSDRawViewer.Converters
{
    public class CHR0Converter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static AnimManager LoadCHR0(string filePath)
        {
            AnimManager anim = new AnimManager();

            using (BinaryReaderExt r = new BinaryReaderExt(new FileStream(filePath, FileMode.Open)))
            {
                r.BigEndian = true;
                if (r.BaseStream.Length < 4 || new string(r.ReadChars(4)) != "CHR0")
                    throw new InvalidDataException("CHR0 file is not valid");

                r.Skip(4);

                int versionNum = r.ReadInt32();

                if (versionNum != 4)
                    throw new InvalidDataException($"CHR0 version {versionNum} not supported");

                System.Console.WriteLine("Reading Track ");

                r.Seek(0x10);

                var indexGroupOffset = r.ReadUInt32();
                var animName = r.ReadString(r.ReadInt32(), -1);

                r.Skip(4);
                anim.FrameCount = r.ReadUInt16();
                int animDataCount = r.ReadUInt16();
                r.Skip(8);

                r.Seek(indexGroupOffset);
                var sectionOffset = r.ReadUInt32() + indexGroupOffset;
                int sectionCount = r.ReadInt32();

                for (uint i = 0; i < sectionCount; i++)
                {
                    r.Seek(indexGroupOffset + 8 + 16 * i);
                    r.Skip(4); // id and unknown
                    r.Skip(2); // let
                    r.Skip(2); // right
                    var boneName = r.ReadString(r.ReadInt32() + (int)indexGroupOffset, -1);
                    var dataOffset = r.ReadUInt32() + indexGroupOffset;
                    if (dataOffset == indexGroupOffset)
                    {
                        sectionCount += 1;
                        continue;
                    }

                    r.Seek(dataOffset);

                    var nameOff = r.Position + r.ReadUInt32();
                    var flags = r.ReadInt32();

                    int t_type = (flags >> 0x1e) & 0x3;
                    int r_type = (flags >> 0x1b) & 0x7;
                    int s_type = (flags >> 0x19) & 0x3;

                    int hasT = (flags >> 0x18) & 0x1;
                    int hasR = (flags >> 0x17) & 0x1;
                    int hasS = (flags >> 0x16) & 0x1;

                    int Zfixed = (flags >> 0x15) & 0x1;
                    int Yfixed = (flags >> 0x14) & 0x1;
                    int Xfixed = (flags >> 0x13) & 0x1;

                    int RZfixed = (flags >> 0x12) & 0x1;
                    int RYfixed = (flags >> 0x11) & 0x1;
                    int RXfixed = (flags >> 0x10) & 0x1;

                    int SZfixed = (flags >> 0xf) & 0x1;
                    int SYfixed = (flags >> 0xe) & 0x1;
                    int SXfixed = (flags >> 0xd) & 0x1;

                    int Tiso = (flags >> 0x6) & 0x1;
                    int Riso = (flags >> 0x5) & 0x1;
                    int Siso = (flags >> 0x4) & 0x1;
                    
                    AnimNode node = new AnimNode();
                    AnimTrack trackX = new AnimTrack();
                    AnimTrack trackY = new AnimTrack();
                    AnimTrack trackZ = new AnimTrack();
                    AnimTrack trackRX = new AnimTrack();
                    AnimTrack trackRY = new AnimTrack();
                    AnimTrack trackRZ = new AnimTrack();
                    AnimTrack trackSX = new AnimTrack();
                    AnimTrack trackSY = new AnimTrack();
                    AnimTrack trackSZ = new AnimTrack();

                    if (hasS == 1)
                        ReadKeys(r, node, (int)anim.FrameCount, trackX, trackY, trackZ, Siso == 1, SXfixed == 1, SYfixed == 1, SZfixed == 1, s_type, dataOffset);

                    if (hasR == 1)
                        ReadKeys(r, node, (int)anim.FrameCount, trackRX, trackRY, trackRZ, Riso == 1, RXfixed == 1, RYfixed == 1, RZfixed == 1, r_type, dataOffset);

                    if (hasT == 1)
                        ReadKeys(r, node, (int)anim.FrameCount, trackSX, trackSY, trackSZ, Tiso == 1, Xfixed == 1, Yfixed == 1, Zfixed == 1, t_type, dataOffset);

                    if (trackX.Keys.Count > 0) node.Tracks.Add(trackX);
                    if (trackY.Keys.Count > 0) node.Tracks.Add(trackY);
                    if (trackZ.Keys.Count > 0) node.Tracks.Add(trackZ);
                    if (trackRX.Keys.Count > 0) node.Tracks.Add(trackRX);
                    if (trackRY.Keys.Count > 0) node.Tracks.Add(trackRY);
                    if (trackRZ.Keys.Count > 0) node.Tracks.Add(trackRZ);
                    if (trackSX.Keys.Count > 0) node.Tracks.Add(trackSX);
                    if (trackSY.Keys.Count > 0) node.Tracks.Add(trackSY);
                    if (trackSZ.Keys.Count > 0) node.Tracks.Add(trackSZ);

                    foreach (var k in trackRX.Keys)
                        k.Value = MathHelper.DegreesToRadians(k.Value);
                    foreach (var k in trackRY.Keys)
                        k.Value = MathHelper.DegreesToRadians(k.Value);
                    foreach (var k in trackRZ.Keys)
                        k.Value = MathHelper.DegreesToRadians(k.Value);

                    Console.WriteLine(boneName + " Tracks:" + node.Tracks.Count);
                    anim.Nodes.Add(node);
                }
            }

            return anim;
        }

        private static void ReadKeys(BinaryReaderExt r, AnimNode node, int frameCount, AnimTrack xtrack, AnimTrack ytrack, AnimTrack ztrack, bool isIsotrophic, bool isXFixed, bool isYFixed, bool isZFixed, int type, uint dataOffset)
        {
            if (isIsotrophic)
            {
                float iss = r.ReadSingle();
                xtrack.Keys.Add(new FOBJKey() { Frame = 0, Value = iss, InterpolationType = GXInterpolationType.HSD_A_OP_CON});
                ytrack.Keys.Add(new FOBJKey() { Frame = 0, Value = iss, InterpolationType = GXInterpolationType.HSD_A_OP_CON });
                ztrack.Keys.Add(new FOBJKey() { Frame = 0, Value = iss, InterpolationType = GXInterpolationType.HSD_A_OP_CON });
            }
            else
            {
                if (isXFixed)
                    xtrack.Keys.Add(new FOBJKey() { Frame = 0, Value = r.ReadSingle(), InterpolationType = GXInterpolationType.HSD_A_OP_CON });
                else
                    ReadTrack(r, frameCount, type, xtrack, dataOffset, node);

                if (isYFixed)
                    ytrack.Keys.Add(new FOBJKey() { Frame = 0, Value = r.ReadSingle(), InterpolationType = GXInterpolationType.HSD_A_OP_CON });
                else
                    ReadTrack(r, frameCount, type, ytrack, dataOffset, node);

                if (isZFixed)
                    ztrack.Keys.Add(new FOBJKey() { Frame = 0, Value = r.ReadSingle(), InterpolationType = GXInterpolationType.HSD_A_OP_CON });
                else
                    ReadTrack(r, frameCount, type, ztrack, dataOffset, node);
            }
        }

        private static int Sign12Bit(int i)
        {
            if (((i >> 11) & 0x1) == 1)
            {
                i = ~i;
                i = i & 0xFFF;
                i += 1;
                i *= -1;
            }

            return i;
        }

        private static void ReadTrack(BinaryReaderExt r, int frameCount, int type, AnimTrack track, uint dataOffset, AnimNode node)
        {
            var offset = r.ReadUInt32() + dataOffset;
            var temp = r.Position;
            r.Seek(offset);

            int fCount = -1;
            float scale = 0;
            float[] frame = null, step = null, tan = null;

            if (type == 0x1)
            {
                fCount = r.ReadUInt16();
                r.Skip(2);
                scale = r.ReadSingle();
                float stepb = r.ReadSingle();
                float base2 = r.ReadSingle();

                frame = new float[fCount];
                step = new float[fCount];
                tan = new float[fCount];

                for (int i = 0; i < fCount; i++)
                {
                    var v = r.ReadInt32();
                    frame[i] = (v >> 24) & 0xFF;
                    int th = v & 0xFFFFFF;
                    step[i] = base2 + ((th >> 12) & 0xfff) * stepb;
                    tan[i] = (Sign12Bit(th & 0xfff) / 32f);

                    track.Keys.Add(new FOBJKey() { Frame = frame[i], Value = step[i], Tan = tan[i], InterpolationType = GXInterpolationType.HSD_A_OP_SPL});
                }
            }

            if (type == 0x2)
            {
                fCount = r.ReadUInt16();
                r.Skip(2);
                scale = r.ReadSingle();
                float stepb = r.ReadSingle();
                float base2 = r.ReadSingle();

                frame = new float[fCount];
                step = new float[fCount];
                tan = new float[fCount];

                for (int i = 0; i < fCount; i++)
                {
                    frame[i] = r.ReadUInt16() / 32f;
                    step[i] = base2 + r.ReadUInt16() * stepb;
                    tan[i] = (r.ReadInt16() / 256f);

                    track.Keys.Add(new FOBJKey() { Frame = frame[i], Value = step[i], Tan = tan[i], InterpolationType = GXInterpolationType.HSD_A_OP_SPL });
                }
            }

            if (type == 0x3)
            {
                fCount = r.ReadUInt16();
                r.Skip(2);
                scale = r.ReadSingle();

                frame = new float[fCount];
                step = new float[fCount];
                tan = new float[fCount];

                for (int i = 0; i < fCount; i++)
                {
                    frame[i] = r.ReadSingle();
                    step[i] = r.ReadSingle();
                    tan[i] = r.ReadSingle();

                    track.Keys.Add(new FOBJKey() { Frame = frame[i], Value = step[i], Tan = tan[i], InterpolationType = GXInterpolationType.HSD_A_OP_SPL });
                }
            }

            if (type == 0x4)
            {
                float stepb = r.ReadSingle();
                float base2 = r.ReadSingle();
                for (int i = 0; i < frameCount; i++)
                {
                    float v = base2 + stepb * (r.ReadByte());

                    track.Keys.Add(new FOBJKey() { Frame = i, Value = v, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });
                }
            }

            if (type == 0x6)
            {
                for (int i = 0; i < frameCount; i++)
                {
                    float v = r.ReadSingle();

                    track.Keys.Add(new FOBJKey() { Frame = i, Value = v, InterpolationType = GXInterpolationType.HSD_A_OP_LIN });
                }
            }

            r.Seek(temp);
        }

    }
}
