using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Rendering;
using HSDRawViewer.Tools.Animation;
using IONET.Core.Animation;
using System;
using System.Collections.Generic;

namespace HSDRawViewer.Converters.Animation
{
    public class SMDConv
    {
        private static readonly Dictionary<IOAnimationTrackType, JointTrackType> Type = new()
        {
            {IOAnimationTrackType.PositionX, JointTrackType.HSD_A_J_TRAX },
            {IOAnimationTrackType.PositionY, JointTrackType.HSD_A_J_TRAY },
            {IOAnimationTrackType.PositionZ, JointTrackType.HSD_A_J_TRAZ },
            {IOAnimationTrackType.RotationEulerX, JointTrackType.HSD_A_J_ROTX },
            {IOAnimationTrackType.RotationEulerY, JointTrackType.HSD_A_J_ROTY },
            {IOAnimationTrackType.RotationEulerZ, JointTrackType.HSD_A_J_ROTZ },
            {IOAnimationTrackType.ScaleX, JointTrackType.HSD_A_J_SCAX },
            {IOAnimationTrackType.ScaleY, JointTrackType.HSD_A_J_SCAY },
            {IOAnimationTrackType.ScaleZ, JointTrackType.HSD_A_J_SCAZ },
        };

        private static void ProcessNodes(JointAnimManager manager, IOAnimation node)
        {
            AnimNode n = new();

            System.Diagnostics.Debug.WriteLine(node.Name);

            foreach (IOAnimationTrack t in node.Tracks)
            {
                if (Type.ContainsKey(t.ChannelType))
                {
                    FOBJ_Player track = new()
                    {
                        JointTrackType = Type[t.ChannelType]
                    };

                    foreach (IOKeyFrame k in t.KeyFrames)
                    {
                        FOBJKey key = new()
                        {
                            Frame = k.Frame,
                            Value = k.Value,
                            InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                        };

                        if (k is IOKeyFrameConstant)
                        {
                            key.InterpolationType = GXInterpolationType.HSD_A_OP_CON;
                        }

                        if (k is IOKeyFrameHermite hermite)
                        {
                            key.Tan = hermite.TangentSlopeOutput;
                        }

                        // TODO: bezier not supported

                        track.Keys.Add(key);
                    }

                    if (track.Keys.Count == 1)
                        track.Keys[0].InterpolationType = GXInterpolationType.HSD_A_OP_KEY;

                    n.Tracks.Add(track);
                }
                else
                {
                    throw new NotSupportedException($"Track Type {t.ChannelType} is not currently supported");
                }
            }

            manager.Nodes.Add(n);

            foreach (IOAnimation child in node.Groups)
            {
                ProcessNodes(manager, child);
            }
        }

        public static JointAnimManager ImportAnimationFromSMD(string filePath, JointMap jointMap)
        {
            // import scene
            IONET.Core.IOScene scene = IONET.IOManager.LoadScene(filePath, new IONET.ImportSettings());

            // get animation
            IOAnimation anim = scene.Animations[0];

            // create blank joint animation
            JointAnimManager animation = new();
            animation.FrameCount = anim.GetFrameCount();

            // process
            foreach (IOAnimation child in anim.Groups)
                ProcessNodes(animation, child);

            // return result
            return animation;
        }
    }
}
