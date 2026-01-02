using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Rendering;
using HSDRawViewer.Tools.Animation;
using IONET;
using IONET.Core;
using IONET.Core.Animation;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using OpenTK.Graphics.OpenGL;
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

        private static IOBone ProcessJoints(HSD_JOBJ jobj, JointMap jointMap, ref int index)
        {
            // create iobone
            IOBone root = new()
            {
                Name = jointMap[index] != null ? jointMap[index] : "JOBJ_" + index,
                Scale = new System.Numerics.Vector3(jobj.SX, jobj.SY, jobj.SZ),
                RotationEuler = new System.Numerics.Vector3(jobj.RX, jobj.RY, jobj.RZ),
                Translation = new System.Numerics.Vector3(jobj.TX, jobj.TY, jobj.TZ)
            };

            // check if name is stored in jobj itself and it doesn't have a label
            if (jointMap[index] != null &&
                !string.IsNullOrEmpty(jobj.ClassName))
                root.Name = jobj.ClassName;

            index++;

            // process children
            foreach (HSD_JOBJ c in jobj.Children)
                root.AddChild(ProcessJoints(c, jointMap, ref index));

            return root;
        }

        private readonly static Dictionary<JointTrackType, IOAnimationTrackType> trackType = new Dictionary<JointTrackType, IOAnimationTrackType>()
        {
            { JointTrackType.HSD_A_J_TRAX, IOAnimationTrackType.PositionX },
            { JointTrackType.HSD_A_J_TRAY, IOAnimationTrackType.PositionY },
            { JointTrackType.HSD_A_J_TRAZ, IOAnimationTrackType.PositionZ },
            { JointTrackType.HSD_A_J_ROTX, IOAnimationTrackType.RotationEulerX },
            { JointTrackType.HSD_A_J_ROTY, IOAnimationTrackType.RotationEulerY },
            { JointTrackType.HSD_A_J_ROTZ, IOAnimationTrackType.RotationEulerZ },
            { JointTrackType.HSD_A_J_SCAX, IOAnimationTrackType.ScaleX },
            { JointTrackType.HSD_A_J_SCAY, IOAnimationTrackType.ScaleY },
            { JointTrackType.HSD_A_J_SCAZ, IOAnimationTrackType.ScaleZ },
        };

        public static void ExportAnimationToSMD(string filePath, HSD_JOBJ jobj, JointMap jointMap, JointAnimManager anim)
        {
            var scene = new IOScene();
            var model = new IOModel()
            {
                Skeleton = new IOSkeleton(),
            };
            int index = 0;
            model.Skeleton.RootBones.Add(ProcessJoints(jobj, jointMap, ref index));
            scene.Models.Add(model);

            var a = new IOAnimation()
            {
                EndFrame = anim.FrameCount
            };

            var jointlist = jobj.TreeList;
            for (int i = 0; i < anim.NodeCount; i++)
            {
                var node = new IOAnimation()
                {
                    Name = jointMap[i] != null ? jointMap[i] : "JOBJ_" + i,
                    EndFrame = anim.FrameCount,
                };

                foreach (var t in anim.Nodes[i].Tracks)
                {
                    var track = new IOAnimationTrack()
                    {
                        ChannelType = trackType[t.JointTrackType],
                    };
                    node.Tracks.Add(track);
                    for (int j = 0; j <= anim.FrameCount; j++)
                    {
                        track.KeyFrames.Add(new IOKeyFrame()
                        {
                            Frame = j,
                            Value = t.GetValue(j),
                        });
                    }
                }

                a.Groups.Add(node);
            }

            scene.Animations.Add(a);

            IOManager.ExportScene(scene, filePath);
        }
    }
}
