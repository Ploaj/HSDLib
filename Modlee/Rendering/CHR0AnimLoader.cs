using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.DAT;
using MeleeLib.DAT.Helpers;
using MeleeLib.IO;

namespace Modlee
{
    public class CHR0AnimLoader
    {

        public static List<AnimationHelperNode> GetTracks(string fname, out int FrameCount)
        {
            DATReader d = new DATReader(fname);
            d.BigEndian = true;
            d.Seek(0x8);

            int versionNum = d.Int();
            FrameCount = 0;
            d.Seek(0x10);
            if (versionNum == 4)
            {
                
                return ReadAnim(d, out FrameCount);
            }
            return null;
        }

        private static List<AnimationHelperNode> ReadAnim(DATReader d, out int FrameCount)
        {
            List<AnimationHelperNode> Nodes = new List<AnimationHelperNode>();

            int offset = d.Int();
            int nameoff = d.Int();

            d.Skip(4);
            int fCount = d.Short();
            FrameCount = fCount;
            int animDataCount = d.Short();
            d.Skip(8);

            d.Seek(offset);
            int sectionOffset = d.Int() + offset;
            int size = d.Int(); // size again 

            for (int i = 0; i < size; i++)
            {
                //			System.out.print(d.readShort()); // id
                d.Skip(4); // id and unknown
                d.Short(); //left
                d.Short(); //right
                int nameOffset = d.Int() + offset;
                int dataOffset = d.Int() + offset;
                if (dataOffset == offset)
                {
                    i--;
                    continue;
                }
                
                int temp = d.Pos();

                d.Seek(dataOffset);

                int pos = d.Pos();
                int nameOff = d.Int() + sectionOffset + (d.Pos() - sectionOffset) - 4;
                int flags = d.Int();

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

                
                AnimationHelperNode node = new AnimationHelperNode();
                Nodes.Add(node);
                node.Name = d.String(nameOff);

                if (hasS == 1)
                {
                    AnimationHelperTrack trackX = new AnimationHelperTrack(); trackX.TrackType = MeleeLib.DAT.Animation.AnimTrackType.XSCA;
                    AnimationHelperTrack trackY = new AnimationHelperTrack(); trackY.TrackType = MeleeLib.DAT.Animation.AnimTrackType.YSCA;
                    AnimationHelperTrack trackZ = new AnimationHelperTrack(); trackZ.TrackType = MeleeLib.DAT.Animation.AnimTrackType.ZSCA;
                    node.Tracks.Add(trackX);
                    node.Tracks.Add(trackY);
                    node.Tracks.Add(trackZ);
                    if (Siso == 1)
                    {
                        float iss = d.Float();
                        trackX.KeyFrames.Add(new AnimationHelperKeyFrame(0, iss, MeleeLib.DAT.Animation.InterpolationType.Constant));
                        trackY.KeyFrames.Add(new AnimationHelperKeyFrame(0, iss, MeleeLib.DAT.Animation.InterpolationType.Constant));
                        trackZ.KeyFrames.Add(new AnimationHelperKeyFrame(0, iss, MeleeLib.DAT.Animation.InterpolationType.Constant));
                    }
                    else
                    {
                        if (SXfixed == 1)
                            trackX.KeyFrames.Add(new AnimationHelperKeyFrame(0, d.Float(), MeleeLib.DAT.Animation.InterpolationType.Constant));
                        else ReadFrames(d, trackX, s_type, pos, fCount);
                        if (SYfixed == 1)
                            trackY.KeyFrames.Add(new AnimationHelperKeyFrame(0, d.Float(), MeleeLib.DAT.Animation.InterpolationType.Constant));
                        else ReadFrames(d, trackY, s_type, pos, fCount);
                        if (SZfixed == 1)
                            trackZ.KeyFrames.Add(new AnimationHelperKeyFrame(0, d.Float(), MeleeLib.DAT.Animation.InterpolationType.Constant));
                        else ReadFrames(d, trackZ, s_type, pos, fCount);
                    }
                }

                if (hasR == 1)
                {
                    AnimationHelperTrack trackX = new AnimationHelperTrack(); trackX.TrackType = MeleeLib.DAT.Animation.AnimTrackType.XROT;
                    AnimationHelperTrack trackY = new AnimationHelperTrack(); trackY.TrackType = MeleeLib.DAT.Animation.AnimTrackType.YROT;
                    AnimationHelperTrack trackZ = new AnimationHelperTrack(); trackZ.TrackType = MeleeLib.DAT.Animation.AnimTrackType.ZROT;
                    node.Tracks.Add(trackX);
                    node.Tracks.Add(trackY);
                    node.Tracks.Add(trackZ);
                    if (Riso == 1)
                    {
                        float iss = (float)(d.Float());
                        trackX.KeyFrames.Add(new AnimationHelperKeyFrame(0, iss, MeleeLib.DAT.Animation.InterpolationType.Constant) { Degrees = true });
                        trackY.KeyFrames.Add(new AnimationHelperKeyFrame(0, iss, MeleeLib.DAT.Animation.InterpolationType.Constant) { Degrees = true });
                        trackZ.KeyFrames.Add(new AnimationHelperKeyFrame(0, iss, MeleeLib.DAT.Animation.InterpolationType.Constant) { Degrees = true });
                    }
                    else
                    {
                        if (RXfixed == 1)
                            trackX.KeyFrames.Add(new AnimationHelperKeyFrame(0, d.Float(), MeleeLib.DAT.Animation.InterpolationType.Constant) { Degrees = true });
                        else ReadFrames(d, trackX, r_type, pos, fCount, true);
                        if (RYfixed == 1)
                            trackY.KeyFrames.Add(new AnimationHelperKeyFrame(0, d.Float(), MeleeLib.DAT.Animation.InterpolationType.Constant) { Degrees = true });
                        else ReadFrames(d, trackY, r_type, pos, fCount, true);
                        if (RZfixed == 1)
                            trackZ.KeyFrames.Add(new AnimationHelperKeyFrame(0, d.Float(), MeleeLib.DAT.Animation.InterpolationType.Constant) { Degrees = true });
                        else ReadFrames(d, trackZ, r_type, pos, fCount, true);
                    }
                }

                if (hasT == 1)
                {
                    AnimationHelperTrack trackX = new AnimationHelperTrack(); trackX.TrackType = MeleeLib.DAT.Animation.AnimTrackType.XPOS;
                    AnimationHelperTrack trackY = new AnimationHelperTrack(); trackY.TrackType = MeleeLib.DAT.Animation.AnimTrackType.YPOS;
                    AnimationHelperTrack trackZ = new AnimationHelperTrack(); trackZ.TrackType = MeleeLib.DAT.Animation.AnimTrackType.ZPOS;
                    node.Tracks.Add(trackX);
                    node.Tracks.Add(trackY);
                    node.Tracks.Add(trackZ);
                    if (Tiso == 1)
                    {
                        float iss = d.Float();
                        trackX.KeyFrames.Add(new AnimationHelperKeyFrame(0, iss, MeleeLib.DAT.Animation.InterpolationType.Constant));
                        trackY.KeyFrames.Add(new AnimationHelperKeyFrame(0, iss, MeleeLib.DAT.Animation.InterpolationType.Constant));
                        trackZ.KeyFrames.Add(new AnimationHelperKeyFrame(0, iss, MeleeLib.DAT.Animation.InterpolationType.Constant));
                    }
                    else
                    {
                        if (Xfixed == 1)
                            trackX.KeyFrames.Add(new AnimationHelperKeyFrame(0, d.Float(), MeleeLib.DAT.Animation.InterpolationType.Constant));
                        else ReadFrames(d, trackX, t_type, pos, fCount);
                        if (Yfixed == 1)
                            trackY.KeyFrames.Add(new AnimationHelperKeyFrame(0, d.Float(), MeleeLib.DAT.Animation.InterpolationType.Constant));
                        else ReadFrames(d, trackY, t_type, pos, fCount);
                        if (Zfixed == 1)
                            trackZ.KeyFrames.Add(new AnimationHelperKeyFrame(0, d.Float(), MeleeLib.DAT.Animation.InterpolationType.Constant));
                        else ReadFrames(d, trackZ, t_type, pos, fCount);
                    }
                }

                d.Seek(temp);
            }

            return Nodes;
        }
        
        private static void ReadFrames(DATReader d, AnimationHelperTrack track, int type, int secOff, int FrameCount, bool Deg = false)
        {
            int offset = d.Int() + secOff;
            int temp = d.Pos();
            d.Seek(offset);

            int max = 0;
            int fCount = -1;
            float scale = 0;
            float[] frame = null, step = null, tan = null;

            if (type == 0x1)
            {
                fCount = d.Short();
                d.Skip(2);
                scale = d.Float();
                float stepb = d.Float();
                float base2 = d.Float();

                frame = new float[fCount];
                step = new float[fCount];
                tan = new float[fCount];

                for (int i = 0; i < fCount; i++)
                {
                    frame[i] = d.Byte();
                    int th = d.Three();
                    step[i] = base2 + ((th >> 12) & 0xfff) * stepb;
                    tan[i] = (Sign12Bit(th & 0xfff) / 32f);

                    if (frame[i] > max)
                    {
                        max = (int)frame[i];
                    }
                }
            }

            if (type == 0x2)
            {
                fCount = d.Short();
                d.Skip(2);
                scale = d.Float();
                float stepb = d.Float();
                float base2 = d.Float();

                frame = new float[fCount];
                step = new float[fCount];
                tan = new float[fCount];

                for (int i = 0; i < fCount; i++)
                {
                    frame[i] = d.Short() / 32f;
                    step[i] = base2 + d.Short() * stepb;
                    tan[i] = ((short)d.Short() / 256f);

                    if (frame[i] > max)
                    {
                        max = (int)frame[i];
                    }
                }
            }

            if (type == 0x3)
            {
                //if(debug)
                //System.out.println(part + "\tInterpolated 12 " + Integer.toHexString(offset));

                fCount = d.Short();
                d.Skip(2);
                scale = d.Float();

                frame = new float[fCount];
                step = new float[fCount];
                tan = new float[fCount];

                for (int i = 0; i < fCount; i++)
                {
                    frame[i] = d.Float();
                    step[i] = d.Float();
                    tan[i] = d.Float();

                    if (frame[i] > max)
                    {
                        max = (int)frame[i];
                    }
                }
            }

            if (frame != null)
            {
                for (int i = 0; i < fCount; i++)
                {
                    AnimationHelperKeyFrame kf = new AnimationHelperKeyFrame();
                    kf.Degrees = true;
                    kf.InterpolationType = MeleeLib.DAT.Animation.InterpolationType.Hermite;
                    kf.Value = step[i];
                    kf.Frame = (int)frame[i];
                    kf.Tan = tan[i];
                    track.KeyFrames.Add(kf);
                }
            }

            if (type == 0x4)
            {
                float stepb = d.Float();
                float base2 = d.Float();
                for (int i = 0; i < FrameCount; i++)
                {
                    float v = base2 + stepb * (d.Byte());

                    AnimationHelperKeyFrame kf = new AnimationHelperKeyFrame();
                    kf.Degrees = true;
                    kf.InterpolationType = MeleeLib.DAT.Animation.InterpolationType.Linear;
                    kf.Value = v;
                    kf.Frame = i;
                }
            }

            if (type == 0x6)
            {
                for (int i = 0; i < FrameCount; i++)
                {

                    float v = d.Float();

                    AnimationHelperKeyFrame kf = new AnimationHelperKeyFrame();
                    kf.Degrees = true;
                    kf.InterpolationType = MeleeLib.DAT.Animation.InterpolationType.Linear;
                    kf.Value = v;
                    kf.Frame = i;
                }
            }

            d.Seek(temp);
        }

        private static int Sign12Bit(int i)
        {
            if (((i >> 11) & 0x1) == 1)
            {
                //			System.out.println(i);
                i = ~i;
                i = i & 0xFFF;
                //			System.out.println(Integer.toBinaryString(i));
                //			System.out.println(i);
                i += 1;
                i *= -1;
            }

            return i;
        }

    }
}
