using HSDRaw.Common.Animation;
using HSDRawViewer.Rendering;
using HSDRawViewer.Tools.Animation;
using IONET;
using IONET.Core;
using IONET.Core.Animation;
using System.IO;

namespace HSDRawViewer.Converters.Animation
{
    public class JointAnimationLoader
    {
        public static readonly string SupportedImportAnimFilter = "Supported Animation Formats (*.dat*.anim*.chr0*.smd*.dae)|*.dat;*.anim;*.chr0;*.smd;*.dae";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_jointMap"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static JointAnimManager LoadJointAnimFromFile(JointMap _jointMap, string filePath = null)
        {
            if (filePath == null)
                filePath = Tools.FileIO.OpenFile(SupportedImportAnimFilter);

            if (filePath != null)
            {
                if (Path.GetExtension(filePath).ToLower().Equals(".smd"))
                {
                    return SMDConv.ImportAnimationFromSMD(filePath, _jointMap);
                }
                else
                if (Path.GetExtension(filePath).ToLower().Equals(".chr0"))
                {
                    return CHR0Converter.LoadCHR0(filePath, _jointMap);
                }
                else
                if (Path.GetExtension(filePath).ToLower().Equals(".anim"))
                {
                    return ConvMayaAnim.ImportFromMayaAnim(filePath, _jointMap);
                }
                else
                if (Path.GetExtension(filePath).ToLower().Equals(".dae"))
                {
                    IOScene scene = IOManager.LoadScene(filePath, new ImportSettings());

                    if (scene.Animations.Count == 0)
                        return null;

                    JointAnimManager ja = new();
                    ConvertIOGroup(ja, scene.Animations[0]);
                    return ja;
                }
                else
                if (Path.GetExtension(filePath).ToLower().Equals(".dat"))
                {
                    HSDRaw.HSDRawFile dat = new(filePath);

                    if (dat.Roots.Count > 0 && dat.Roots[0].Data is HSD_FigaTree tree)
                        return new JointAnimManager(tree);

                    if (dat.Roots.Count > 0 && dat.Roots[0].Data is HSD_AnimJoint joint)
                        return new JointAnimManager(joint);
                }
            }

            return null;
        }

        private static void ConvertIOGroup(JointAnimManager ja, IOAnimation group)
        {
            AnimNode anim_node = new();

            foreach (IOAnimationTrack iotrack in group.Tracks)
            {
                foreach (IOKeyFrame k in iotrack.KeyFrames)
                {
                    switch (iotrack.ChannelType)
                    {
                        case IOAnimationTrackType.PositionX: anim_node.AddLinearKey(JointTrackType.HSD_A_J_TRAX, k.Frame, k.Value); break;
                    }
                }
            }

            foreach (IOAnimation g in group.Groups)
                ConvertIOGroup(ja, g);
        }
    }
}
