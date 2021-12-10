using HSDRaw;
using HSDRaw.Common.Animation;
using HSDRawViewer.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.GUI.Plugins.Melee
{
    public partial class SubactionEditor
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void compressAllAnimationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AJManager == null)
                return;

            HashSet<string> edited = new HashSet<string>();
            int oldSize = 0;
            int newSize = 0;

            foreach (var a in AllActions)
            {
                if (string.IsNullOrEmpty(a.Symbol) || edited.Contains(a.Symbol))
                    continue;

                var anim = AJManager.GetAnimationData(a.Symbol);
                if (anim != null)
                {
                    JointAnimManager ja = new JointAnimManager();
                    var animfile = new HSDRawFile(anim);
                    ja.FromFigaTree(animfile.Roots[0].Data as HSD_FigaTree);
                    //ja.Optimize(JointManager.GetJOBJ(0), 0.01f);
                    animfile.Roots[0].Data = ja.ToFigaTree(0.01f);
                    using (MemoryStream s = new MemoryStream())
                    {
                        animfile.Save(s);
                        var newfile = s.ToArray();
                        oldSize += anim.Length;
                        newSize += newfile.Length;
                        AJManager.SetAnimation(a.Symbol, newfile);
                    }
                }
                edited.Add(a.Symbol);
            }

            Console.WriteLine($"{oldSize.ToString("X")} => {newSize.ToString("X")}");
            Console.WriteLine($"Saved {(oldSize - newSize).ToString("X")} bytes");
        }
    }
}
