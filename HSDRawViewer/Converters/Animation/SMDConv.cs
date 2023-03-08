using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Rendering;
using HSDRawViewer.Tools;
using IONET.Core.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.Converters.Animation
{
    public class SMDConv
    {
        private static Dictionary<IOAnimationTrackType, JointTrackType> Type = new Dictionary<IOAnimationTrackType, HSDRaw.Common.Animation.JointTrackType>()
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
            AnimNode n = new AnimNode();

            System.Diagnostics.Debug.WriteLine(node.Name);

            foreach (var t in node.Tracks)
            {
                if (Type.ContainsKey(t.ChannelType))
                {
                    var track = new FOBJ_Player()
                    {
                        JointTrackType = Type[t.ChannelType]
                    };

                    foreach (var k in t.KeyFrames)
                    {
                        track.Keys.Add(new FOBJKey()
                        {
                            Frame = k.Frame,
                            Value = k.Value,
                            InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                        });
                    }

                    n.Tracks.Add(track);
                }
                else
                {
                    throw new NotSupportedException($"Track Type {t.ChannelType} is not currently supported");
                }
            }

            manager.Nodes.Add(n);

            foreach (var child in node.Groups)
            {
                ProcessNodes(manager, child);
            }
        }

        public static JointAnimManager ImportAnimationFromSMD(string filePath, JointMap jointMap)
        {
            // import scene
            var scene = IONET.IOManager.LoadScene(filePath, new IONET.ImportSettings());

            // get animation
            var anim = scene.Animations[0];

            // create blank joint animation
            JointAnimManager animation = new JointAnimManager();
            animation.FrameCount = anim.GetFrameCount();

            // process
            foreach (var child in anim.Groups)
                ProcessNodes(animation, child);

            // return result
            return animation;
        }
    }
}
