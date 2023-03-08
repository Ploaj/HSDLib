using HSDRaw.Common.Animation;
using HSDRawViewer.Rendering;
using HSDRawViewer.Tools;
using System.IO;

namespace HSDRawViewer.Converters.Animation
{
    public class JointAnimationLoader
    {
        public static readonly string SupportedImportAnimFilter = "Supported Animation Formats (*.dat*.anim*.chr0*.smd)|*.dat;*.anim;*.chr0;*.smd";

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
                if (Path.GetExtension(filePath).ToLower().Equals(".dat"))
                {
                    var dat = new HSDRaw.HSDRawFile(filePath);

                    if (dat.Roots.Count > 0 && dat.Roots[0].Data is HSD_FigaTree tree)
                        return new JointAnimManager(tree);

                    if (dat.Roots.Count > 0 && dat.Roots[0].Data is HSD_AnimJoint joint)
                        return new JointAnimManager(joint);
                }
            }

            return null;
        }
    }
}
