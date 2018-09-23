using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.DAT.Animation;
using MeleeLib.IO;

namespace MeleeLib.DAT.Helpers
{
    public class AnimationHelperInterpolation
    {
        public static float Lerp(float LHS, float RHS, float Weight)
        {
            return LHS * (1 - Weight) + RHS * Weight;
        }

        public static float Herp(float LHS, float RHS, float LS, float RS, float Diff, float Weight)
        {
            float Result;

            Result = LHS + (LHS - RHS) * (2 * Weight - 3) * Weight * Weight;
            Result += (Diff * (Weight - 1)) * (LS * (Weight - 1) + RS * Weight);

            return Result;
        }
    }

    public class AnimationHelperTrack
    {
        public AnimTrackType TrackType;
        public List<AnimationHelperKeyFrame> KeyFrames = new List<AnimationHelperKeyFrame>();

        public float GetValueAt(int Frame)
        {
            AnimationHelperKeyFrame KF = new AnimationHelperKeyFrame();

            AnimationHelperKeyFrame CurrentFrame = null;
            AnimationHelperKeyFrame NextFrame = null;
            float CurrentTan = 0, NextTan = 0;
            foreach (AnimationHelperKeyFrame f in KeyFrames)
            {
                if (f.InterpolationType == InterpolationType.HermiteCurve)
                {
                    CurrentTan = NextTan;
                    NextTan = f.Tan;
                }
                else
                {
                    CurrentFrame = NextFrame;
                    if(CurrentFrame != null)
                    {
                        CurrentTan = CurrentFrame.Tan;
                    }
                    NextFrame = f;

                    if (f.Frame >= Frame)
                    {
                        break;
                    }

                }
            }

            if (Frame > NextFrame.Frame) return NextFrame.Value;
            if (CurrentFrame == null) return NextFrame.Value;
            if (Frame == NextFrame.Frame) return NextFrame.Value;
            if (Frame == CurrentFrame.Frame) return CurrentFrame.Value;

            float FrameDiff = Frame - CurrentFrame.Frame;
            float Weight = FrameDiff / (NextFrame.Frame - CurrentFrame.Frame);

            float val = 0;
            switch (CurrentFrame.InterpolationType)
            {
                case InterpolationType.Step:
                    val = CurrentFrame.Value;
                    break;
                case InterpolationType.Constant:
                    val = CurrentFrame.Value;
                    break;
                case InterpolationType.Hermite:
                    val = AnimationHelperInterpolation.Herp(CurrentFrame.Value, NextFrame.Value, CurrentTan, NextTan, FrameDiff, Weight);
                    break;
                case InterpolationType.HermiteValue:
                    val = AnimationHelperInterpolation.Herp(CurrentFrame.Value, NextFrame.Value, CurrentTan, NextTan, FrameDiff, Weight);
                    break;
                case InterpolationType.Linear:
                    val = AnimationHelperInterpolation.Lerp(CurrentFrame.Value, NextFrame.Value, Weight);
                    break;
            }
            

            return val;
        }
    }

    public class AnimationHelperKeyFrame
    {
        public bool Degrees = false;
        public InterpolationType InterpolationType;
        public float Value
        {
            get
            {
                    return _value;
            }
            set
            {
                _value = value;
            }
        }
        private float _value;
        public float Tan
        {
            get
            {
                if (Degrees)
                    return _tan * (float)Math.PI / 180;
                else
                    return _tan;
            }
            set
            {
                _tan = value;
            }
        }
        private float _tan;
        public int Frame;

        public AnimationHelperKeyFrame()
        {

        }

        public AnimationHelperKeyFrame(int Frame, float Value, InterpolationType type)
        {
            this.Frame = Frame;
            this.Value = Value;
            this.InterpolationType = type;
        }

        public AnimationHelperKeyFrame(int Frame, float Value, float Tan, InterpolationType type)
        {
            this.Frame = Frame;
            this.Value = Value;
            this.Tan = Tan;
            this.InterpolationType = type;
        }
    }

    public class AnimationHelperNode
    {
        public List<AnimationHelperTrack> Tracks = new List<AnimationHelperTrack>();
        public String Name;
    }

    public class AnimationKeyFrameHelper
    {
        public static DatAnimationNode EncodeKeyFrames(AnimationHelperTrack[] tracks, int FrameCount)
        {
            DatAnimationNode n = new DatAnimationNode();
            
            foreach(AnimationHelperTrack r in tracks)
            {
                if (r.KeyFrames.Count == 0) continue;
                DatAnimationTrack track = new DatAnimationTrack();
                n.Tracks.Add(track);
                track.AnimationType = r.TrackType;

                track.ValueFormat = GXAnimDataFormat.Short;
                track.ValueScale = (int)Math.Pow(2, 11);
                track.TanFormat = GXAnimDataFormat.Short;
                track.TanScale = (int)Math.Pow(2, 11);

                track.Data = EncodeKeyFrames(track, r.KeyFrames, FrameCount);
            }

            return n;
        }


        public static AnimationHelperTrack[] DecodeKeyFrames(DatAnimationNode node)
        {
            List<AnimationHelperTrack> Tracks = new List<AnimationHelperTrack>();
            foreach(DatAnimationTrack track in node.Tracks)
            {
                AnimationHelperTrack t = new AnimationHelperTrack();
                t.TrackType = track.AnimationType;
                Tracks.Add(t);

                DATReader f = new DATReader(track.Data);

                int clock = 0;
                while (f.Pos() < f.Size())
                {
                    int type = f.ExtendedByte();
                    InterpolationType interpolation = (InterpolationType)((type) & 0x0F);
                    int numOfKey = ((type >> 4)) + 1;
                    if (interpolation == 0) break;

                    for (int i = 0; i < numOfKey; i++)
                    {
                        double value = 0;
                        double tan = 0;
                        int time = 0;

                        switch (interpolation)
                        {
                            case InterpolationType.Step:
                                value = ReadVal(f, track.ValueFormat, track.ValueScale);
                                time = f.ExtendedByte();
                                break;
                            case InterpolationType.Linear:
                                value = ReadVal(f, track.ValueFormat, track.ValueScale);
                                time = f.ExtendedByte();
                                break;
                            case InterpolationType.HermiteValue:
                                value = ReadVal(f, track.ValueFormat, track.ValueScale);
                                time = f.ExtendedByte();
                                break;
                            case InterpolationType.Hermite:
                                value = ReadVal(f, track.ValueFormat, track.ValueScale);
                                tan = ReadVal(f, track.TanFormat, track.TanScale);
                                time = f.ExtendedByte();
                                break;
                            case InterpolationType.HermiteCurve:
                                tan = ReadVal(f, track.TanFormat, track.TanScale);
                                break;
                            case InterpolationType.Constant:
                                value = ReadVal(f, track.ValueFormat, track.ValueScale);
                                //time = f.ExtendedByte();
                                break;
                            default:
                                throw new Exception("end");
                        }

                        AnimationHelperKeyFrame kf = new AnimationHelperKeyFrame();
                        kf.InterpolationType = interpolation;
                        kf.Value = (float)value;
                        kf.Frame = clock;
                        kf.Tan = (float)(tan);
                        t.KeyFrames.Add(kf);
                        clock += time;

                        if (track.AnimationType == AnimTrackType.XROT || 
                            track.AnimationType == AnimTrackType.YROT || 
                            track.AnimationType == AnimTrackType.ZROT)
                            kf.Degrees = true;
                    }
                }
                
            }
            return Tracks.ToArray();
        }

        public static double ReadVal(DATReader d, GXAnimDataFormat Format, float Scale)
        {
            d.BigEndian = false;
            switch (Format)
            {
                case GXAnimDataFormat.Float:
                    return d.Float();
                case GXAnimDataFormat.Short:
                    return (short)d.Short() / (double)Scale;
                case GXAnimDataFormat.UShort:
                    return d.Short() / (double)Scale;
                case GXAnimDataFormat.SByte:
                    return (sbyte)d.Byte() / (double)Scale;
                case GXAnimDataFormat.Byte:
                    return d.Byte() / (double)Scale;
                default:
                    return 0;
            }
        }

        public static void WriteVal(DATWriter d, float Value, GXAnimDataFormat Format, float Scale)
        {
            switch (Format)
            {
                case GXAnimDataFormat.Float:
                    d.Float(Value);
                    break;
                case GXAnimDataFormat.Short:
                    d.Short((short)(Value * Scale));
                    break;
                case GXAnimDataFormat.UShort:
                    d.Short((short)(Value * Scale));
                    break;
                case GXAnimDataFormat.SByte:
                    d.Byte((byte)(Value * Scale));
                    break;
                case GXAnimDataFormat.Byte:
                    d.Byte((byte)(Value * Scale));
                    break;
                default:
                    throw new Exception("Unknown GXAnimDataFormat " + Format.ToString());
            }
        }

        public static byte[] EncodeKeyFrames(DatAnimationTrack track, List<AnimationHelperKeyFrame> Keys, int FrameCount)
        {
            DATWriter o = new DATWriter();
            o.BigEndian = false;

            int flag = ((Keys.Count-1) << 4) | (int)Keys[0].InterpolationType;
            o.ExtendedByte(flag);

            int clock = 0;
            foreach (AnimationHelperKeyFrame f in Keys)
            {
                int duration = FrameCount;
                if (Keys.IndexOf(f) + 1 < Keys.Count)
                    duration = Keys[Keys.IndexOf(f) + 1].Frame - f.Frame;
                switch (f.InterpolationType)
                {
                    case InterpolationType.Constant:
                        WriteVal(o, f.Value, track.ValueFormat, track.ValueScale);
                        //o.ExtendedByte(duration);
                        break;
                    case InterpolationType.Step:
                        WriteVal(o, f.Value, track.ValueFormat, track.ValueScale);
                        o.ExtendedByte(duration);
                        break;
                    case InterpolationType.Hermite:
                        WriteVal(o, f.Value, track.ValueFormat, track.ValueScale);
                        WriteVal(o, f.Tan, track.TanFormat, track.TanScale);
                        o.ExtendedByte(duration);
                        break;
                    case InterpolationType.Linear:
                        WriteVal(o, f.Value, track.ValueFormat, track.ValueScale);
                        o.ExtendedByte(duration);
                        break;
                    default:
                        throw new Exception("Interpolation Type not supported " + f.InterpolationType.ToString());
                }
                clock = f.Frame;
            }

            return o.GetBytes();
        }
    }
}
