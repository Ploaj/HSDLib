using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Pl;
using HSDRaw.Tools.Melee;
using HSDRawViewer.Rendering;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HSDRawViewer.Tools.Optimizers
{
    public class FighterAnimationCompress
    {

        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<string> Compress(string ftdat, string ajdat, string costumedat, float optimize = 0.01f, float compress = 0.001f)
        {
            var costume = new HSDRawFile(costumedat).Roots[0].Data as HSD_JOBJ;

            FighterAJManager manager = new FighterAJManager(File.ReadAllBytes(ajdat));

            int total_original_size = 0;
            int total_new_size = 0;

            foreach (var symbol in manager.GetAnimationSymbols())
            {
                if (symbol.Contains("Taro"))
                    continue;

                int original_size = manager.GetAnimationData(symbol).Length;
                total_original_size += original_size;

                var ftFile = new HSDRawFile(manager.GetAnimationData(symbol));

                if (ftFile[symbol] != null)
                {
                    var ft = ftFile[symbol].Data as HSD_FigaTree;

                    JointAnimManager m = new JointAnimManager(ft);
                    m.Optimize(costume, false, optimize);
                    ftFile[symbol].Data = m.ToFigaTree(compress);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        ftFile.Save(stream);
                        var newdat = stream.ToArray();

                        int new_size = newdat.Length;
                        total_new_size += new_size;

                        if (newdat.Length < manager.GetOffsetSize(symbol).Item2)
                        {
                            yield return $"{symbol} {manager.GetOffsetSize(symbol).Item2 / 1000f}KB -> {newdat.Length / 1000f}KB saved {(manager.GetOffsetSize(symbol).Item2 - newdat.Length) / 1000f}KB";
                            manager.SetAnimation(symbol, newdat);
                        }
                    }
                }
            }

            var newAJFile = manager.RebuildAJFile(manager.GetAnimationSymbols().ToArray(), true);

            HSDRawFile ftfile = new HSDRawFile(ftdat);
            if (ftfile.Roots[0].Data is SBM_FighterData data)
            {
                var sa = data.FighterActionTable.Commands;

                foreach (var action in sa)
                {
                    if (action.SymbolName != null && !string.IsNullOrEmpty(action.SymbolName.Value))
                    {
                        var sizeOffset = manager.GetOffsetSize(action.SymbolName.Value);
                        action.AnimationOffset = sizeOffset.Item1;
                        action.AnimationSize = sizeOffset.Item2;
                    }
                }

                data.FighterActionTable.Commands = sa;

                ftfile.TrimData();
                ftfile.Save(ftdat);
                File.WriteAllBytes(ajdat, newAJFile);
            }

            yield return $"{total_original_size:X8} -> {total_new_size:X8} (Saved: {(total_original_size - total_new_size) / 1000f}KB)";
        }
    }
}
