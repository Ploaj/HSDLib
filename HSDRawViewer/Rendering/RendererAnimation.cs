using HSDLib.Helpers;
using HSDRaw.Common.Animation;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Rendering
{
    public class AnimTrack
    {
        public List<HSDRaw.Tools.FOBJKey> Keys;
        public JointTrackType TrackType;

        public float GetValue(float Frame)
        {
            // register
            float ValueLeft = 0;
            float ValueRight = 0;
            float TanLeft = 0;
            float TanRight = 0;
            float FrameLeft = 0;
            float FrameRight = 0;
            GXInterpolationType CurrentInterpolation = GXInterpolationType.Constant;

            // get current frame state
            for (int i = 0; i < Keys.Count; i++)
            {
                switch (Keys[i].InterpolationType)
                {
                    case GXInterpolationType.Constant:
                        ValueRight = Keys[i].Value;
                        ValueLeft = ValueRight;
                        break;
                    case GXInterpolationType.Linear:
                        ValueLeft = ValueRight;
                        FrameLeft = FrameRight;
                        ValueRight = Keys[i].Value;
                        FrameRight = Keys[i].Frame;
                        break;
                    case GXInterpolationType.Step:
                        ValueLeft = ValueRight;
                        FrameLeft = FrameRight;
                        ValueRight = Keys[i].Value;
                        FrameRight = Keys[i].Frame;
                        break;
                    case GXInterpolationType.Hermite:
                        ValueLeft = ValueRight;
                        FrameLeft = FrameRight;
                        TanLeft = TanRight;
                        ValueRight = Keys[i].Value;
                        FrameRight = Keys[i].Frame;
                        TanRight = Keys[i].Tan;
                        break;
                    case GXInterpolationType.HermiteCurve:
                        TanRight = Keys[i].Tan;
                        break;
                    case GXInterpolationType.HermiteValue:
                        ValueLeft = ValueRight;
                        FrameLeft = FrameRight;
                        ValueRight = Keys[i].Value;
                        FrameRight = Keys[i].Frame;
                        break;
                }

                if (FrameRight > Frame && Keys[i].InterpolationType != GXInterpolationType.HermiteCurve)
                    break;

                CurrentInterpolation = Keys[i].InterpolationType;
            }


            if (FrameLeft == FrameRight || CurrentInterpolation == GXInterpolationType.Step || CurrentInterpolation == GXInterpolationType.Constant)
                return ValueLeft;

            float FrameDiff = Frame - FrameLeft;
            float Weight = FrameDiff / (FrameRight - FrameLeft);

            if (CurrentInterpolation == GXInterpolationType.Linear)
                return AnimationHelperInterpolation.Lerp(ValueLeft, ValueRight, Weight);

            if (CurrentInterpolation == GXInterpolationType.Hermite || CurrentInterpolation == GXInterpolationType.HermiteValue || CurrentInterpolation == GXInterpolationType.HermiteCurve)
                return AnimationHelperInterpolation.Herp(ValueLeft, ValueRight, TanLeft, TanRight, FrameDiff, Weight);

            return ValueLeft;
        }
    }

    public class AnimNode
    {
        public List<AnimTrack> Tracks = new List<AnimTrack>();
    }

}
